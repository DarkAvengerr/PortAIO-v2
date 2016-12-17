using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Fiora.Evade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    // Credit: Brian
    internal class EvadeTargetManager
    {
        public static Menu Menu;
        public static List<SpellData> Spells = new List<SpellData>();
        private static readonly List<Targets> DetectedTargets = new List<Targets>();

        public static void Init()
        {
            LoadSpellData();

            Menu = Logic.Menu;

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
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

            if (!Logic.W.IsReady())
            {
                return;
            }

            foreach (var target in DetectedTargets.Where(i => ObjectManager.Player.Distance(i.Obj.Position) < 700))
            {
                if (ObjectManager.Player.UnderTurret(true) && !Menu.Item("EvadeTargetTower", true).GetValue<bool>())
                {
                    continue;
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
