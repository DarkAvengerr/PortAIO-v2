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

    // Credit: Brian
    internal class EvadeTargetManager
    {
        public static Menu Menu;
        public static List<SpellData> Spells = new List<SpellData>();
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
            LoadSpellData();

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

        private static void LoadSpellData()
        {
            Spells.Add(
                new SpellData
                { ChampionName = "Ahri", SpellNames = new[] { "ahrifoxfiremissiletwo" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "Ahri", SpellNames = new[] { "ahritumblemissile" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "Akali", SpellNames = new[] { "akalimota" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Anivia", SpellNames = new[] { "frostbite" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Annie", SpellNames = new[] { "disintegrate" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellNames = new[] { "brandconflagrationmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellNames = new[] { "brandwildfire", "brandwildfiremissile" },
                    Slot = SpellSlot.R
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Caitlyn",
                    SpellNames = new[] { "caitlynaceintheholemissile" },
                    Slot = SpellSlot.R
                });
            Spells.Add(
                new SpellData
                { ChampionName = "Cassiopeia", SpellNames = new[] { "cassiopeiatwinfang" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Elise", SpellNames = new[] { "elisehumanq" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ezreal",
                    SpellNames = new[] { "ezrealarcaneshiftmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "FiddleSticks",
                    SpellNames = new[] { "fiddlesticksdarkwind", "fiddlesticksdarkwindmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { ChampionName = "Gangplank", SpellNames = new[] { "parley" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Janna", SpellNames = new[] { "sowthewind" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { ChampionName = "Kassadin", SpellNames = new[] { "nulllance" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Katarina",
                    SpellNames = new[] { "katarinaq", "katarinaqmis" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData
                { ChampionName = "Kayle", SpellNames = new[] { "judicatorreckoning" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellNames = new[] { "leblancchaosorb", "leblancchaosorbm" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(new SpellData { ChampionName = "Lulu", SpellNames = new[] { "luluw" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "Malphite", SpellNames = new[] { "seismicshard" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "MissFortune",
                    SpellNames = new[] { "missfortunericochetshot", "missFortunershotextra" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    SpellNames = new[] { "namiwenemy", "namiwmissileenemy" },
                    Slot = SpellSlot.W
                });
            Spells.Add(
                new SpellData { ChampionName = "Nunu", SpellNames = new[] { "iceblast" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Pantheon", SpellNames = new[] { "pantheonq" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ryze",
                    SpellNames = new[] { "spellflux", "spellfluxmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { ChampionName = "Shaco", SpellNames = new[] { "twoshivpoison" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Shen", SpellNames = new[] { "shenvorpalstar" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Sona", SpellNames = new[] { "sonaqmissile" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Swain", SpellNames = new[] { "swaintorment" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Syndra", SpellNames = new[] { "syndrar" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "Taric", SpellNames = new[] { "dazzle" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Teemo", SpellNames = new[] { "blindingdart" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                { ChampionName = "Tristana", SpellNames = new[] { "detonatingshot" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData
                { ChampionName = "TwistedFate", SpellNames = new[] { "bluecardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "TwistedFate", SpellNames = new[] { "goldcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "TwistedFate", SpellNames = new[] { "redcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Urgot",
                    SpellNames = new[] { "urgotheatseekinghomemissile" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData { ChampionName = "Vayne", SpellNames = new[] { "vaynecondemn" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData
                { ChampionName = "Veigar", SpellNames = new[] { "veigarprimordialburst" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData
                { ChampionName = "Viktor", SpellNames = new[] { "viktorpowertransfer" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Vladimir",
                    SpellNames = new[] { "vladimirtidesofbloodnuke" },
                    Slot = SpellSlot.E
                });
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
                Spells.FirstOrDefault(
                    i =>
                    i.SpellNames.Contains(missile.SData.Name.ToLower())
                    && Menu.SubMenu("ET_" + i.ChampionName).Item(i.MissileName, true) != null
                    && Menu.SubMenu("ET_" + i.ChampionName).Item(i.MissileName, true).GetValue<bool>());

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
