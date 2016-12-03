using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Evade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Manager.Spells;

    internal class EvadeTargetManager // Credit By Brian
    {
        public static Menu Menu;
        private static Vector2 wallCastedPos;
        private static readonly List<Targets> DetectedTargets = new List<Targets>();

        private static GameObject Wall
        {
            get
            {
                return
                    ObjectManager.Get<GameObject>()
                        .FirstOrDefault(
                            i => i.IsValid && Regex.IsMatch(i.Name, "_w_windwall.\\.troy", RegexOptions.IgnoreCase));
            }
        }

        public static void Init()
        {
            Menu = Logic.Menu;

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static bool GoThroughWall(Vector2 pos1, Vector2 pos2)
        {
            if (Wall == null)
            {
                return false;
            }

            var wallWidth = 300 + 50 * Convert.ToInt32(Wall.Name.Substring(Wall.Name.Length - 6, 1));
            var wallDirection = (Wall.Position.To2D() - wallCastedPos).Normalized().Perpendicular();
            var wallStart = Wall.Position.To2D() + wallWidth / 2f * wallDirection;
            var wallEnd = wallStart - wallWidth * wallDirection;
            var wallPolygon = new Geometry.Polygon.Rectangle(wallStart, wallEnd, 75);
            var intersections = new List<Vector2>();

            for (var i = 0; i < wallPolygon.Points.Count; i++)
            {
                var inter =
                    wallPolygon.Points[i].Intersection(
                        wallPolygon.Points[i != wallPolygon.Points.Count - 1 ? i + 1 : 0],
                        pos1,
                        pos2);

                if (inter.Intersects)
                {
                    intersections.Add(inter.Point);
                }
            }

            return intersections.Any();
        }

        private static void OnCreate(GameObject sender, EventArgs args)
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
            var spellData =
                SpellManager.Spells.FirstOrDefault(
                    i =>
                    i.SpellNames.Contains(missile.SData.Name.ToLower())
                    && Menu.Item(i.MissileName, true) != null
                    && Menu.Item(i.MissileName, true).GetValue<bool>());

            if (spellData == null && missile.SData.IsAutoAttack()
                && (!missile.SData.Name.ToLower().Contains("crit")
                        ? Menu.Item("BAttack", true).GetValue<bool>()
                          && ObjectManager.Player.HealthPercent < Menu.Item("BAttackHpU", true).GetValue<Slider>().Value
                        : Menu.Item("CAttack", true).GetValue<bool>()
                          && ObjectManager.Player.HealthPercent < Menu.Item("CAttackHpU", true).GetValue<Slider>().Value))
            {
                spellData = new SpellData
                    {ChampionName = unit.ChampionName, SpellNames = new[] {missile.SData.Name}};
            }

            if (spellData == null || !missile.Target.IsMe)
            {
                return;
            }

            DetectedTargets.Add(new Targets {Start = unit.ServerPosition, Obj = missile});
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
            {
                return;
            }

            var missile = (MissileClient)sender;

            if (missile.SpellCaster.IsValid<AIHeroClient>() && missile.SpellCaster.Team != ObjectManager.Player.Team)
            {
                DetectedTargets.RemoveAll(i => i.Obj.NetworkId == missile.NetworkId);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValid || sender.Team != ObjectManager.Player.Team || args.SData.Name != "YasuoWMovingWall")
            {
                return;
            }

            wallCastedPos = sender.ServerPosition.To2D();
            Logic.W.LastCastAttemptT = Utils.TickCount;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity) || ObjectManager.Player.HasBuffOfType(BuffType.SpellShield))
            {
                return;
            }

            if (!Logic.W.IsReady(300) && (Wall == null || !Logic.E.IsReady(200)))
            {
                return;
            }

            foreach (var target in DetectedTargets.Where(i => ObjectManager.Player.Distance(i.Obj.Position) < 700))
            {
                if (Logic.E.IsReady() && Menu.Item("EvadeTargetE", true).GetValue<bool>() && Wall != null
                    && Utils.TickCount - Logic.W.LastCastAttemptT > 1000
                    && !GoThroughWall(ObjectManager.Player.ServerPosition.To2D(), target.Obj.Position.To2D())
                    && Logic.W.IsInRange(target.Obj, 250))
                {
                    var obj = new List<Obj_AI_Base>();

                    obj.AddRange(MinionManager.GetMinions(Logic.E.Range, MinionTypes.All, MinionTeam.NotAlly));
                    obj.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(Logic.E.Range)));

                    if (
                        obj.Where(
                            i =>
                            SpellManager.CanCastE(i) && EvadeManager.IsSafe(i.ServerPosition.To2D()).IsSafe
                            && EvadeManager.IsSafe(Logic.PosAfterE(i).To2D()).IsSafe
                            && (!Logic.UnderTower(Logic.PosAfterE(i)) || Menu.Item("EvadeTargetETower", true).GetValue<bool>())
                            && GoThroughWall(ObjectManager.Player.ServerPosition.To2D(), Logic.PosAfterE(i).To2D()))
                            .OrderBy(i => Logic.PosAfterE(i).Distance(Game.CursorPos))
                            .Any(i => Logic.E.CastOnUnit(i, true)))
                    {
                        return;
                    }
                }

                if (Logic.W.IsReady() && Menu.Item("EvadeTargetW", true).GetValue<bool>() && Logic.W.IsInRange(target.Obj, 500)
                    && Logic.W.Cast(ObjectManager.Player.ServerPosition.Extend(target.Start, 100), true))
                {
                    return;
                }
            }
        }

        public class SpellData
        {
            public string ChampionName;
            public SpellSlot Slot;
            public string[] SpellNames = { };

            public string MissileName => SpellNames.First();
        }

        private class Targets
        {
            public MissileClient Obj;
            public Vector3 Start;
        }
    }
}
