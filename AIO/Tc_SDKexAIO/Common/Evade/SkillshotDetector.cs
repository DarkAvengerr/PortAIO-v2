// Copyright 2014 - 2014 Esk0r
// SkillshotDetector.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Common.Evade
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using LeagueSharp;
    using SharpDX;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;

    internal static class SkillshotDetector
    {
        public delegate void OnDeleteMissileH(Skillshot skillshot, MissileClient missile);
        public delegate void OnDetectSkillshotH(Skillshot skillshot);
        public static event OnDetectSkillshotH OnDetectSkillshot;
        public static event OnDeleteMissileH OnDeleteMissile;

        static SkillshotDetector()
        {
            Obj_AI_Base.OnProcessSpellCast += ObjAiHeroOnOnProcessSpellCast;
            GameObject.OnDelete += ObjSpellMissileOnOnDelete;
            GameObject.OnCreate += ObjSpellMissileOnOnCreate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var spellData = SpellDatabase.GetBySourceObjectName(sender.Name);

            if (spellData == null)
            {
                return;
            }
            
            if (Config.Menu["Skillshots"][spellData.MenuItemName]["Enabled" + spellData.MenuItemName] == null)
            {
                return;
            }

            TriggerOnDetectSkillshot(
                DetectionType.ProcessSpell, 
                spellData, 
                Utils.TickCount - Game.Ping / 2, 
                sender.Position.ToVector2(), 
                sender.Position.ToVector2(), 
                sender.Position.ToVector2(),
                GameObjects.AllyHeroes.MinOrDefault(h => h.IsAlly ? 1 : 0));
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid && sender.Team == GameObjects.Player.Team)
            {
                return;
            }

            for (var i = Evade.DetectedSkillshots.Count - 1; i >= 0; i--)
            {
                var skillshot = Evade.DetectedSkillshots[i];

                if (skillshot.SpellData.ToggleParticleName != "" && new Regex(skillshot.SpellData.ToggleParticleName).IsMatch(sender.Name))
                {
                    Evade.DetectedSkillshots.RemoveAt(i);
                }
            }
        }

        private static void ObjSpellMissileOnOnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            var unit = missile.SpellCaster as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.Team == GameObjects.Player.Team)
            {
                return;
            }

            var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);

            if (spellData == null)
            {
                return;
            }

            DelayAction.Add(0, delegate
            {
                ObjSpellMissionOnOnCreateDelayed(sender, args);
            });
        }

        private static void ObjSpellMissionOnOnCreateDelayed(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            var unit = missile.SpellCaster as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.Team == GameObjects.Player.Team)
            {
                return;
            }

            var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);

            if (spellData == null)
            {
                return;
            }

            var missilePosition = missile.Position.ToVector2();
            var unitPosition = missile.StartPosition.ToVector2();
            var endPos = missile.EndPosition.ToVector2();
            var direction = (endPos - unitPosition).Normalized();

            if (unitPosition.Distance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = unitPosition + direction * spellData.Range;
            }

            if (spellData.ExtraRange != -1)
            {
                endPos = endPos +  Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(unitPosition)) * direction;
            }

            var castTime = Utils.TickCount - Game.Ping / 2 - (spellData.MissileDelayed ? 0 : spellData.Delay) - (int)(1000f * missilePosition.Distance(unitPosition) / spellData.MissileSpeed);

            TriggerOnDetectSkillshot(DetectionType.RecvPacket, spellData, castTime, unitPosition, endPos, endPos, unit);
        }

        private static void ObjSpellMissileOnOnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            var caster = missile.SpellCaster as AIHeroClient;

            if (caster == null || !caster.IsValid || (caster.Team == GameObjects.Player.Team))
            {
                return;
            }

            var spellName = missile.SData.Name;

            if (OnDeleteMissile != null)
            {
                foreach (var skillshot in Evade.DetectedSkillshots)
                {
                    if (
                        !skillshot.SpellData.MissileSpellName.Equals(spellName,
                            StringComparison.InvariantCultureIgnoreCase) ||
                        (skillshot.Unit.NetworkId != caster.NetworkId ||
                         !((missile.EndPosition.ToVector2() - missile.StartPosition.ToVector2()).AngleBetween(
                               skillshot.Direction) < 10)) || !skillshot.SpellData.CanBeRemoved) continue;
                    OnDeleteMissile(skillshot, missile);
                    break;
                }
            }

            Evade.DetectedSkillshots.RemoveAll(skillshot =>
                    (skillshot.SpellData.MissileSpellName.Equals(spellName, StringComparison.InvariantCultureIgnoreCase) ||
                     skillshot.SpellData.ExtraMissileNames.Contains(spellName, StringComparer.InvariantCultureIgnoreCase)) &&
                    (skillshot.Unit.NetworkId == caster.NetworkId &&
                     ((missile.EndPosition.ToVector2() - missile.StartPosition.ToVector2()).AngleBetween(skillshot.Direction) < 10) &&
                     skillshot.SpellData.CanBeRemoved ||
                     skillshot.SpellData.ForceRemove));
        }

        internal static void TriggerOnDetectSkillshot(DetectionType detectionType, SpellData spellData, int startT, Vector2 start, Vector2 end, Vector2 originalEnd, Obj_AI_Base unit)
        {
            var skillshot = new Skillshot(detectionType, spellData, startT, start, end, unit)
            {
                OriginalEnd = originalEnd
            };

            OnDetectSkillshot?.Invoke(skillshot);
        }

        private static void ObjAiHeroOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid)
            {
                return;
            }

            if (args.SData.Name == "dravenrdoublecast")
            {
                Evade.DetectedSkillshots.RemoveAll(s => s.Unit.NetworkId == sender.NetworkId && s.SpellData.SpellName == "DravenRCast");
            }

            if (!sender.IsValid || sender.Team == GameObjects.Player.Team)
            {
                return;
            }

            var spellData = SpellDatabase.GetByName(args.SData.Name);

            if (spellData == null)
            {
                return;
            }

            var startPos = new Vector2();

            if (spellData.FromObject != "")
            {
                foreach (var o in ObjectManager.Get<GameObject>())
                {
                    if (o.Name.Contains(spellData.FromObject))
                    {
                        startPos = o.Position.ToVector2();
                    }
                }
            }
            else
            {
                startPos = sender.ServerPosition.ToVector2();
            }

            if (spellData.FromObjects != null && spellData.FromObjects.Length > 0)
            {
                foreach (var obj in ObjectManager.Get<GameObject>())
                {
                    if (obj.IsEnemy && spellData.FromObjects.Contains(obj.Name))
                    {
                        var start = obj.Position.ToVector2();
                        var end = start + spellData.Range * (args.End.ToVector2() - obj.Position.ToVector2()).Normalized();

                        TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, Utils.TickCount - Game.Ping / 2, start, end, end, sender);
                    }
                }
            }

            if (!startPos.IsValid())
            {
                return;
            }

            var endPos = args.End.ToVector2();

            if (spellData.SpellName == "LucianQ" && args.Target != null && args.Target.NetworkId == GameObjects.Player.NetworkId)
            {
                return;
            }

            var direction = (endPos - startPos).Normalized();

            if (startPos.Distance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = startPos + direction * spellData.Range;
            }

            if (spellData.ExtraRange != -1)
            {
                endPos = endPos + Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(startPos)) * direction;
            }

            TriggerOnDetectSkillshot(
                DetectionType.ProcessSpell,
                spellData, 
                Utils.TickCount - Game.Ping / 2, 
                startPos, 
                endPos, 
                args.End.ToVector2(), 
                sender);
        }
    }
}
