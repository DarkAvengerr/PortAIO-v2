using System.Collections.Generic;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoTheLastMemebender.Wrapper.Spells
{
    public static class Database
    {
        //This class is dank as fuck
        public static List<DatabaseEntry> Spells = new List<DatabaseEntry>
        {
            #region akali
            new DatabaseEntry
            {
                ChampionName = "Akali",
                SpellName = "AkaliMota",
                Slot = SpellSlot.Q,
                Delay = 250,
                Range = 600,
                Radius = 600,
                MissileSpeed = 1000,
                DangerValue = 2,
                IsDangerous = true,
                MissileSpellName = "AkaliMota"
            },

            #endregion

            #region anivia
            new DatabaseEntry
            {
                ChampionName = "Anivia",
                SpellName = "Frostbite",
                Slot = SpellSlot.E,
                Delay = 250,
                Range = 650,
                Radius = 650,
                MissileSpeed = 1200,
                DangerValue = 3,
                IsDangerous = false,
                MissileSpellName = string.Empty,
                CanBeRemoved = false
            },

            #endregion

            #region Annie
            new DatabaseEntry
            {
                ChampionName = "Annie",
                SpellName = "Disintegrate",
                Slot = SpellSlot.Q,
                Delay = 250,
                Range = 625,
                Radius = 710,
                MissileSpeed = 1400,
                DangerValue = 3,
                IsDangerous = true,
                MissileSpellName = "Disintegrate",
                CanBeRemoved = false
            },

            #endregion

            #region Caitlyn
            new DatabaseEntry
            {
                ChampionName = "Caitlyn",
                SpellName = "CaitlynAceintheHole",
                Slot = SpellSlot.R,
                Delay = 3000,
                Range = 2000,
                DangerValue = 1,
                IsDangerous = false
            },

            #endregion

            #region Cassiopeia
            new DatabaseEntry
            {
                ChampionName = "Cassiopeia",
                SpellName = "CassiopeiaTwinFang",
                Slot = SpellSlot.E,
                Delay = 125,
                Range = 700,
                DangerValue = 1,
                IsDangerous = false
            },

            #endregion

            #region Elise
            new DatabaseEntry
            {
                ChampionName = "Elise",
                SpellName = "EliseHumanQ",
                Slot = SpellSlot.Q,
                Delay = 250,
                Range = 625
            },

            #endregion

            #region Evelynn
            new DatabaseEntry
            {
                ChampionName = "Evelynn",
                SpellName = "EvelynnQ",
                Slot = SpellSlot.Q,
                Delay = 250,
                Range = 500,
                Radius = 450
            },

            #endregion

            #region Fiddlesticks
            new DatabaseEntry
            {
                ChampionName = "FiddleSticks",
                SpellName = "Terrify",
                Slot = SpellSlot.Q,
                AppliedBuffsOnEnemies = new[] {BuffType.Flee},
                Delay = 250,
                Range = 575
            },

            #endregion

            #region Gangplank
            new DatabaseEntry
            {
                ChampionName = "Gangplank",
                SpellName = "GangplankQWrapper",
                Slot = SpellSlot.Q,
                Delay = 250,
                Range = 625
            },

            #endregion

            #region Janna
            new DatabaseEntry
            {
                ChampionName = "Janna",
                SpellName = "SowTheWind",
                Slot = SpellSlot.W,
                AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                Delay = 250,
                Range = 600
            },

            #endregion

            #region kassadin 
            new DatabaseEntry
            {
                ChampionName = "Kassadin",
                SpellName = "NullLance",
                Slot = SpellSlot.Q,
                AppliedBuffsOnEnemies = new[] {BuffType.Silence},
                Delay = 250,
                Range = 650
            },

            #endregion

            #region Katarina
            new DatabaseEntry(
                "Katarina",
                "KatarinaQ",
                SpellSlot.Q,
                false,
                675),

            #endregion

            #region kayle
            new DatabaseEntry("Kayle", "JudicatorReckoning", SpellSlot.Q,false,650),

            #endregion

            #region leblanc
            new DatabaseEntry
            {
                ChampionName = "Leblanc",
                SpellName = "LeblancChaosOrb",
                Slot = SpellSlot.Q,
                Range = 700
            },
            new DatabaseEntry
            {
                ChampionName = "Leblanc",
                SpellName = "LeblancChaosOrbM",
                Slot = SpellSlot.R,
                Range = 700
            },
            #endregion

            #region malphite
            new DatabaseEntry
                    {
                        ChampionName = "Malphite", SpellName = "SeismicShard", Slot = SpellSlot.Q,
                        AppliedBuffsOnEnemies = new[] { BuffType.Slow }, Range = 625
                    },
            #endregion

            #region Miss Fortune 
            new DatabaseEntry
                    {
                        ChampionName = "MissFortune", SpellName = "MissFortuneRichochetShot", Slot = SpellSlot.Q, Range = 650, Radius = 250, Angle = 60
                    },
            #endregion

            #region Nautilus
            new DatabaseEntry
                    {
                        ChampionName = "Nautilus", SpellName = "NautilusGrandLine", Slot = SpellSlot.R,
                        AppliedBuffsOnEnemies = new[] { BuffType.Knockup }, Range = 825
                    },
            #endregion

            #region Nunu
            new DatabaseEntry("Nunu", "IceBlast", SpellSlot.E, false, 550),
            #endregion

            #region Pantheon
            new DatabaseEntry(
                    "Pantheon",
                    "PantheonQ",
                    SpellSlot.Q,
                    false,
                    600),
            #endregion

            #region Ryze
            new DatabaseEntry
                    {
                        ChampionName = "Ryze", SpellName = "RyzeE", Slot = SpellSlot.E, Range = 600
                    },
            #endregion

            #region Shaco
            new DatabaseEntry
                    {
                        ChampionName = "Shaco", SpellName = "TwoShivPoisen", Slot = SpellSlot.E, Range = 625
                    },
            #endregion

            #region Shen
            /*new DatabaseEntry
            {
                ChampionName = "Shen",
                SpellName = "ShenVorpalStar",
                Slot = SpellSlot.Q,
                Range = 475
            },*/
            #endregion

            #region Sona
            new DatabaseEntry("Sona", "SonaHymnofValor", SpellSlot.Q, false, 850),
            #endregion

            #region Syndra
            new DatabaseEntry("Syndra", "SyndraR", SpellSlot.R, false, 750),
            #endregion

            #region Tristana
            new DatabaseEntry("Tristana", "TristanaE", SpellSlot.E, true, 650),
            new DatabaseEntry("Tristana", "TristanaR", SpellSlot.R, true, 650),
            #endregion

            new DatabaseEntry("Taric", "Dazzle", SpellSlot.E, false, 625),

            new DatabaseEntry("Teemo", "BlindingDart", SpellSlot.Q, true, 580),

            new DatabaseEntry("Vayne", "VayneCondemn", SpellSlot.E, false, 550),

            new DatabaseEntry("Veigar", "VeigarPrimordialBurst", SpellSlot.R, false, 650),

            new DatabaseEntry("Viktor", "ViktorPowerTransfer", SpellSlot.Q, false, 600),

            new DatabaseEntry("Valdimir", "VladimirTransfusion", SpellSlot.Q, false, 600)

        };
    }


}