namespace BrianSharp.Evade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using EloBuddy;

    internal static class SkillshotDetector
    {
        #region Static Fields

        public static List<Skillshot> DetectedSkillshots = new List<Skillshot>();

        #endregion

        #region Constructors and Destructors

        static SkillshotDetector()
        {
            GameObject.OnDelete += ObjOnDelete;
            GameObject.OnCreate += ObjMissileClientOnCreate;
            GameObject.OnDelete += ObjMissileClientOnDelete;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        #endregion

        #region Delegates

        public delegate void OnDeleteMissileH(Skillshot skillshot, MissileClient missile);

        public delegate void OnDetectSkillshotH(Skillshot skillshot);

        #endregion

        #region Public Events

        public static event OnDeleteMissileH OnDeleteMissile;

        public static event OnDetectSkillshotH OnDetectSkillshot;

        #endregion

        #region Methods

        private static void ObjMissileClientOnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
            {
                return;
            }
            var missile = (MissileClient)sender;
            if (!missile.SpellCaster.IsValid<AIHeroClient>() || missile.SpellCaster.Team == ObjectManager.Player.Team)
            {
                return;
            }
            var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);
            if (spellData == null)
            {
                return;
            }
            LeagueSharp.Common.Utility.DelayAction.Add(0, () => ObjMissileClientOnCreateDelayed(missile, spellData));
        }

        private static void ObjMissileClientOnCreateDelayed(MissileClient missile, SpellData spellData)
        {
            var unit = missile.SpellCaster as AIHeroClient;
            var missilePosition = missile.Position.LSTo2D();
            var unitPosition = missile.StartPosition.LSTo2D();
            var endPos = missile.EndPosition.LSTo2D();
            var direction = (endPos - unitPosition).LSNormalized();
            if (unitPosition.LSDistance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = unitPosition + direction * spellData.Range;
            }
            if (spellData.ExtraRange != -1)
            {
                endPos += Math.Min(spellData.ExtraRange, spellData.Range - endPos.LSDistance(unitPosition)) * direction;
            }
            var castTime = Utils.GameTimeTickCount - Game.Ping / 2 - (spellData.MissileDelayed ? 0 : spellData.Delay)
                           - (int)(1000 * missilePosition.LSDistance(unitPosition) / spellData.MissileSpeed);
            TriggerOnDetectSkillshot(DetectionType.RecvPacket, spellData, castTime, unitPosition, endPos, unit);
        }

        private static void ObjMissileClientOnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
            {
                return;
            }
            var missile = (MissileClient)sender;
            if (!missile.SpellCaster.IsValid<AIHeroClient>() || missile.SpellCaster.Team == ObjectManager.Player.Team)
            {
                return;
            }
            var unit = (AIHeroClient)missile.SpellCaster;
            var spellName = missile.SData.Name;
            if (OnDeleteMissile != null)
            {
                foreach (var skillshot in
                    DetectedSkillshots.Where(
                        i =>
                        i.SpellData.MissileSpellName == spellName && i.Unit.NetworkId == unit.NetworkId
                        && (missile.EndPosition.LSTo2D() - missile.StartPosition.LSTo2D()).LSAngleBetween(i.Direction) < 10
                        && i.SpellData.CanBeRemoved))
                {
                    OnDeleteMissile(skillshot, missile);
                    break;
                }
            }
            DetectedSkillshots.RemoveAll(
                i =>
                (i.SpellData.MissileSpellName == spellName || i.SpellData.ExtraMissileNames.Contains(spellName))
                && i.Unit.NetworkId == unit.NetworkId
                && (missile.EndPosition - missile.StartPosition).LSTo2D().LSAngleBetween(i.Direction) < 10
                && (i.SpellData.CanBeRemoved || i.SpellData.ForceRemove));
        }

        private static void ObjOnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || sender.Team == ObjectManager.Player.Team)
            {
                return;
            }
            for (var i = DetectedSkillshots.Count - 1; i >= 0; i--)
            {
                var skillshot = DetectedSkillshots[i];
                if (skillshot.SpellData.ToggleParticleName != ""
                    && new Regex(skillshot.SpellData.ToggleParticleName).IsMatch(sender.Name))
                {
                    DetectedSkillshots.RemoveAt(i);
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValid<AIHeroClient>() || sender.Team == ObjectManager.Player.Team)
            {
                return;
            }
            if (args.SData.Name == "dravenrdoublecast")
            {
                DetectedSkillshots.RemoveAll(
                    i => i.Unit.NetworkId == sender.NetworkId && i.SpellData.SpellName == "DravenRCast");
            }
            var spellData = SpellDatabase.GetByName(args.SData.Name);
            if (spellData == null)
            {
                return;
            }
            var startPos = new Vector2();
            if (spellData.FromObject != "")
            {
                foreach (var obj in ObjectManager.Get<GameObject>().Where(i => i.Name.Contains(spellData.FromObject)))
                {
                    startPos = obj.Position.LSTo2D();
                }
            }
            else
            {
                startPos = sender.ServerPosition.LSTo2D();
            }
            if (spellData.FromObjects != null && spellData.FromObjects.Length > 0)
            {
                foreach (var obj in
                    ObjectManager.Get<GameObject>().Where(i => i.IsEnemy && spellData.FromObject.Contains(i.Name)))
                {
                    var start = obj.Position.LSTo2D();
                    var end = start + spellData.Range * (args.End.LSTo2D() - obj.Position.LSTo2D()).LSNormalized();
                    TriggerOnDetectSkillshot(
                        DetectionType.ProcessSpell,
                        spellData,
                        Utils.GameTimeTickCount - Game.Ping / 2,
                        start,
                        end,
                        sender);
                }
            }
            if (!startPos.LSIsValid())
            {
                return;
            }
            var endPos = args.End.LSTo2D();
            if (spellData.SpellName == "LucianQ" && args.Target != null
                && args.Target.NetworkId == ObjectManager.Player.NetworkId)
            {
                return;
            }
            var direction = (endPos - startPos).LSNormalized();
            if (startPos.LSDistance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = startPos + direction * spellData.Range;
            }
            if (spellData.ExtraRange != -1)
            {
                endPos = endPos
                         + Math.Min(spellData.ExtraRange, spellData.Range - endPos.LSDistance(startPos)) * direction;
            }
            TriggerOnDetectSkillshot(
                DetectionType.ProcessSpell,
                spellData,
                Utils.GameTimeTickCount - Game.Ping / 2,
                startPos,
                endPos,
                sender);
        }

        private static void TriggerOnDetectSkillshot(
            DetectionType detectionType,
            SpellData spellData,
            int startT,
            Vector2 start,
            Vector2 end,
            Obj_AI_Base unit)
        {
            if (OnDetectSkillshot != null)
            {
                OnDetectSkillshot(new Skillshot(detectionType, spellData, startT, start, end, unit));
            }
        }

        #endregion
    }
}