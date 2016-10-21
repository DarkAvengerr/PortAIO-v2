#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Data/SpellData.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System.Linq;
using LeagueSharp;
using System.Collections.Generic;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KurisuSivir
{
    public enum HitType
    {
        None,
        AutoAttack,
        MinionAttack,
        TurretAttack,
        Spell,
        Danger,
        Ultimate,
        CrowdControl,
        Stealth,
        ForceExhaust
    }

    public class KurisuLib
    {
        public string SDataName { get; set; }
        public string ChampionName { get; set; }
        public SpellSlot Slot { get; set; }
        public float CastRange { get; set; }
        public bool Global { get; set; }
        public float Delay { get; set; }
        public string MissileName { get; set; }
        public string[] ExtraMissileNames { get; set; }
        public int MissileSpeed { get; set; }
        public string[] FromObject { get; set; }
        public HitType[] HitType { get; set; }

        public static List<KurisuLib> Spells = new List<KurisuLib>();

        static KurisuLib()
        {
            Spells.Add(new KurisuLib
            {
                SDataName = "aatroxq",
                ChampionName = "aatrox",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "aatroxw",
                ChampionName = "aatrox",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "aatroxw2",
                ChampionName = "aatrox",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "aatroxe",
                ChampionName = "aatrox",
                Slot = SpellSlot.E,
                CastRange = 1025f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "aatroxeconemissile",
                MissileSpeed = 1250
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "aatroxr",
                ChampionName = "aatrox",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ahriorbofdeception",
                ChampionName = "ahri",
                Slot = SpellSlot.Q,
                CastRange = 880f,
                Delay = 250f,
                HitType = new HitType[] { global::KurisuSivir.HitType.Danger },
                MissileName = "ahriorbmissile",
                ExtraMissileNames = new[] { "ahriorbreturn" },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ahrifoxfire",
                ChampionName = "ahri",
                Slot = SpellSlot.W,
                CastRange = 550f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1800
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ahriseduce",
                ChampionName = "ahri",
                Slot = SpellSlot.E,
                CastRange = 975f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "ahriseducemissile",
                MissileSpeed = 1550
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ahritumble",
                ChampionName = "ahri",
                Slot = SpellSlot.R,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "akalimota",
                ChampionName = "akali",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "akalismokebomb",
                ChampionName = "akali",
                Slot = SpellSlot.W,
                CastRange = 1000f, // Range: 700 + additional for stealth detection
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "akalishadowswipe",
                ChampionName = "akali",
                Slot = SpellSlot.E,
                CastRange = 325f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "akalishadowdance",
                ChampionName = "akali",
                Slot = SpellSlot.R,
                CastRange = 710f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "pulverize",
                ChampionName = "alistar",
                Slot = SpellSlot.Q,
                CastRange = 365f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "headbutt",
                ChampionName = "alistar",
                Slot = SpellSlot.W,
                CastRange = 650f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "triumphantroar",
                ChampionName = "alistar",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "feroucioushowl",
                ChampionName = "alistar",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 828
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bandagetoss",
                ChampionName = "amumu",
                Slot = SpellSlot.Q,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "sadmummybandagetoss",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "auraofdespair",
                ChampionName = "amumu",
                Slot = SpellSlot.W,
                CastRange = 300f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "tantrum",
                ChampionName = "amumu",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 150f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "curseofthesadmummy",
                ChampionName = "amumu",
                Slot = SpellSlot.R,
                CastRange = 550f,
                Delay = 150f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "flashfrost",
                ChampionName = "anivia",
                Slot = SpellSlot.Q,
                CastRange = 0f, 
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "flashfrostspell",
                MissileSpeed = 850
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "crystalize",
                ChampionName = "anivia",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "frostbite",
                ChampionName = "anivia",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "glacialstorm",
                ChampionName = "anivia",
                Slot = SpellSlot.R,
                CastRange = 625f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "disintegrate",
                ChampionName = "annie",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "incinerate",
                ChampionName = "annie",
                Slot = SpellSlot.W,
                CastRange = 625f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "moltenshield",
                ChampionName = "annie",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "infernalguardian",
                ChampionName = "annie",
                Slot = SpellSlot.R,
                CastRange = 890f, // 600 + Cast Radius
                Delay = 0f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "frostshot",
                ChampionName = "ashe",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "frostarrow",
                ChampionName = "ashe",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "volley",
                ChampionName = "ashe",
                Slot = SpellSlot.W,
                CastRange = 1200f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "volleyattack",
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ashespiritofthehawk",
                ChampionName = "ashe",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "enchantedcrystalarrow",
                ChampionName = "ashe",
                Slot = SpellSlot.R,
                CastRange = 20000f,
                Global = true,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "enchantedcrystalarrow",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "azirq",
                ChampionName = "azir",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "azirsoldiermissile",
                FromObject = new[] { "AzirSoldier" },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "azirr",
                ChampionName = "azir",
                Slot = SpellSlot.R,
                CastRange = 475f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bardq",
                ChampionName = "bard",
                Slot = SpellSlot.Q,
                CastRange = 950f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "bardqmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bardw",
                ChampionName = "bard",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "barde",
                ChampionName = "bard",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 350f,
                HitType = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bardr",
                ChampionName = "bard",
                Slot = SpellSlot.R,
                CastRange = 3400f,
                Delay = 450f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "bardr",
                MissileSpeed = 2100
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rocketgrabmissile",
                ChampionName = "blitzcrank",
                Slot = SpellSlot.Q,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1800
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "overdrive",
                ChampionName = "blitzcrank",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "powerfist",
                ChampionName = "blitzcrank",
                Slot = SpellSlot.E,
                CastRange = 100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "staticfield",
                ChampionName = "blitzcrank",
                Slot = SpellSlot.R,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "brandblaze",
                ChampionName = "brand",
                Slot = SpellSlot.Q,
                CastRange = 1150f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "brandblazemissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "brandfissure",
                ChampionName = "brand",
                Slot = SpellSlot.W,
                CastRange = 240f,
                Delay = 550f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "",
                MissileSpeed = 20
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "brandconflagration",
                ChampionName = "brand",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "brandwildfire",
                ChampionName = "brand",
                Slot = SpellSlot.R,
                CastRange = 750f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileSpeed = 1000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "braumq",
                ChampionName = "braum",
                Slot = SpellSlot.Q,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "braumqmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "braumqmissle",
                ChampionName = "braum",
                Slot = SpellSlot.Q,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "braumw",
                ChampionName = "braum",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "braume",
                ChampionName = "braum",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "braumrwrapper",
                ChampionName = "braum",
                Slot = SpellSlot.R,
                CastRange = 1250f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "braumrmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "caitlynpiltoverpeacemaker",
                ChampionName = "caitlyn",
                Slot = SpellSlot.Q,
                CastRange = 2000f,
                Delay = 625f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "caitlynpiltoverpeacemaker",
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "caitlynyordletrap",
                ChampionName = "caitlyn",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 550f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "caitlynentrapment",
                ChampionName = "caitlyn",
                Slot = SpellSlot.E,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "caitlynentrapmentmissile",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "cassiopeianoxiousblast",
                ChampionName = "cassiopeia",
                Slot = SpellSlot.Q,
                CastRange = 925f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "cassiopeianoxiousblast",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "cassiopeiamiasma",
                ChampionName = "cassiopeia",
                Slot = SpellSlot.W,
                CastRange = 925f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 2500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "cassiopeiatwinfang",
                ChampionName = "cassiopeia",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1900
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "cassiopeiapetrifyinggaze",
                ChampionName = "cassiopeia",
                Slot = SpellSlot.R,
                CastRange = 875f,
                Delay = 350f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "cassiopeiapetrifyinggaze",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rupture",
                ChampionName = "chogath",
                Slot = SpellSlot.Q,
                CastRange = 950f,
                Delay = 1200f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "rupture",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "feralscream",
                ChampionName = "chogath",
                Slot = SpellSlot.W,
                CastRange = 675f,
                Delay = 175f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vorpalspikes",
                ChampionName = "chogath",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 347
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "feast",
                ChampionName = "chogath",
                Slot = SpellSlot.R,
                CastRange = 500f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "phosphorusbomb",
                ChampionName = "corki",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "phosphorusbombmissile",
                MissileSpeed = 1125
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "carpetbomb",
                ChampionName = "corki",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 700
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ggun",
                ChampionName = "corki",
                Slot = SpellSlot.E,
                CastRange = 750f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "missilebarrage",
                ChampionName = "corki",
                Slot = SpellSlot.R,
                CastRange = 1225f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "missilebarragemissile",
                MissileSpeed = 828
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dariuscleave",
                ChampionName = "darius",
                Slot = SpellSlot.Q,
                CastRange = 425f,
                Delay = 750f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dariusnoxiantacticsonh",
                ChampionName = "darius",
                Slot = SpellSlot.W,
                CastRange = 205f,
                Delay = 150f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dariusaxegrabcone",
                ChampionName = "darius",
                Slot = SpellSlot.E,
                CastRange = 555f,
                Delay = 150f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileName = "dariusaxegrabcone",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dariusexecute",
                ChampionName = "darius",
                Slot = SpellSlot.R,
                CastRange = 465f,
                Delay = 450f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dianaarc",
                ChampionName = "diana",
                Slot = SpellSlot.Q,
                CastRange = 830f,
                Delay = 300f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "dianaarc",
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dianaorbs",
                ChampionName = "diana",
                Slot = SpellSlot.W,
                CastRange = 200f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dianavortex",
                ChampionName = "diana",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dianateleport",
                ChampionName = "diana",
                Slot = SpellSlot.R,
                CastRange = 825f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dravenspinning",
                ChampionName = "draven",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dravenfury",
                ChampionName = "draven",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dravendoubleshot",
                ChampionName = "draven",
                Slot = SpellSlot.E,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "dravendoubleshotmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dravenrcast",
                ChampionName = "draven",
                Slot = SpellSlot.R,
                CastRange = 20000f,
                Global = true,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileName = "dravenr",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "infectedcleavermissilecast",
                ChampionName = "drmundo",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "infectedcleavermissile",
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "burningagony",
                ChampionName = "drmundo",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "masochism",
                ChampionName = "drmundo",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sadism",
                ChampionName = "drmundo",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ekkoq",
                ChampionName = "ekko",
                Slot = SpellSlot.Q,
                CastRange = 1075f,
                Delay = 66f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "ekkoqmis",
                ExtraMissileNames = new[] { "ekkoqreturn" },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ekkoeattack",
                ChampionName = "ekko",
                Slot = SpellSlot.E,
                CastRange = 300f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ekkor",
                ChampionName = "ekko",
                Slot = SpellSlot.R,
                CastRange = 425f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                FromObject = new[] { "Ekko_Base_R_TrailEnd" },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "elisehumanq",
                ChampionName = "elise",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 550f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "elisespiderqcast",
                ChampionName = "elise",
                Slot = SpellSlot.Q,
                CastRange = 475f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "elisehumanw",
                ChampionName = "elise",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 750f,
                HitType = new HitType[] { },
                MissileSpeed = 5000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "elisespiderw",
                ChampionName = "elise",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "elisehumane",
                ChampionName = "elise",
                Slot = SpellSlot.E,
                CastRange = 1075f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileName = "elisehumane",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "elisespidereinitial",
                ChampionName = "elise",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "elisespideredescent",
                ChampionName = "elise",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "eliser",
                ChampionName = "elise",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "elisespiderr",
                ChampionName = "elise",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "evelynnq",
                ChampionName = "evelynn",
                Slot = SpellSlot.Q,
                CastRange = 500f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "evelynnw",
                ChampionName = "evelynn",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "evelynne",
                ChampionName = "evelynn",
                Slot = SpellSlot.E,
                CastRange = 225f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "evelynnr",
                ChampionName = "evelynn",
                Slot = SpellSlot.R,
                CastRange = 900f, // 650f + Radius
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "evelynnr",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ezrealmysticshot",
                ChampionName = "ezreal",
                Slot = SpellSlot.Q,
                CastRange = 1150f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "ezrealmysticshotmissile",
                ExtraMissileNames = new[] { "ezrealmysticshotpulsemissile" },
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ezrealessenceflux",
                ChampionName = "ezreal",
                Slot = SpellSlot.W,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "ezrealessencefluxmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ezrealessencemissle",
                ChampionName = "ezreal",
                Slot = SpellSlot.W,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ezrealarcaneshift",
                ChampionName = "ezreal",
                Slot = SpellSlot.E,
                CastRange = 750f, // 475f + Bolt Range
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ezrealtrueshotbarrage",
                ChampionName = "ezreal",
                Slot = SpellSlot.R,
                CastRange = 20000f,
                Global = true,
                Delay = 1000f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileName = "ezrealtrueshotbarrage",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "terrify",
                ChampionName = "fiddlesticks",
                Slot = SpellSlot.Q,
                CastRange = 575f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "drain",
                ChampionName = "fiddlesticks",
                Slot = SpellSlot.W,
                CastRange = 575f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fiddlesticksdarkwind",
                ChampionName = "fiddlesticks",
                Slot = SpellSlot.E,
                CastRange = 750f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1100
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "crowstorm",
                ChampionName = "fiddlesticks",
                Slot = SpellSlot.R,
                CastRange = 800f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.ForceExhaust },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fioraq",
                ChampionName = "fiora",
                Slot = SpellSlot.Q,
                CastRange = 400f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fioraw",
                ChampionName = "fiora",
                Slot = SpellSlot.W,
                CastRange = 750f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fiorae",
                ChampionName = "fiora",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fiorar",
                ChampionName = "fiora",
                Slot = SpellSlot.R,
                CastRange = 500f,
                Delay = 150f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fizzpiercingstrike",
                ChampionName = "fizz",
                Slot = SpellSlot.Q,
                CastRange = 550f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1900
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fizzseastonepassive",
                ChampionName = "fizz",
                Slot = SpellSlot.W,
                CastRange = 175f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fizzjump",
                ChampionName = "fizz",
                Slot = SpellSlot.E,
                CastRange = 450f,
                Delay = 700f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fizzjumpbuffer",
                ChampionName = "fizz",
                Slot = SpellSlot.E,
                CastRange = 330f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fizzjumptwo",
                ChampionName = "fizz",
                Slot = SpellSlot.E,
                CastRange = 270f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fizzmarinerdoom",
                ChampionName = "fizz",
                Slot = SpellSlot.R,
                CastRange = 1275f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "fizzmarinerdoommissile",
                MissileSpeed = 1350
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "galioresolutesmite",
                ChampionName = "galio",
                Slot = SpellSlot.Q,
                CastRange = 1040f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "galioresolutesmite",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "galiobulwark",
                ChampionName = "galio",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "galiorighteousgust",
                ChampionName = "galio",
                Slot = SpellSlot.E,
                CastRange = 1280f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "galiorighteousgust",
                MissileSpeed = 1300
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "galioidolofdurand",
                ChampionName = "galio",
                Slot = SpellSlot.R,
                CastRange = 600f,
                Delay = 0f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gangplankqwrapper",
                ChampionName = "gangplank",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gangplankqproceed",
                ChampionName = "gangplank",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gangplankw",
                ChampionName = "gangplank",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gangplanke",
                ChampionName = "gangplank",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gangplankr",
                ChampionName = "gangplank",
                Slot = SpellSlot.R,
                CastRange = 20000f,
                Global = true,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "garenq",
                ChampionName = "garen",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 300f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "garenqattack",
                ChampionName = "garen",
                Slot = SpellSlot.Q,
                CastRange = 350f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });


            Spells.Add(new KurisuLib
            {
                SDataName = "gnarq",
                ChampionName = "gnar",
                Slot = SpellSlot.Q,
                CastRange = 1185f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 2400,
                MissileName = "gnarqmissile",
                ExtraMissileNames = new[] { "gnarqmissilereturn" }
            });


            Spells.Add(new KurisuLib
            {
                SDataName = "gnarbigq",
                ChampionName = "gnar",
                Slot = SpellSlot.Q,
                CastRange = 1150f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 2000,
                MissileName = "gnarbigqmissile"
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gnarbigw",
                ChampionName = "gnar",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 600f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gnarult",
                ChampionName = "gnar",
                CastRange = 600f, // 590f + 10 Better safe than sorry. :)
                Slot = SpellSlot.R,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },

                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "garenw",
                ChampionName = "garen",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "garene",
                ChampionName = "garen",
                Slot = SpellSlot.E,
                CastRange = 300f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "garenr",
                ChampionName = "garen",
                Slot = SpellSlot.R,
                CastRange = 400f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gragasq",
                ChampionName = "gragas",
                Slot = SpellSlot.Q,
                CastRange = 1000, // 850f + Radius
                Delay = 500f,
                HitType = new HitType[] { },
                MissileName = "gragasqmissile",
                MissileSpeed = 1000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gragasqtoggle",
                ChampionName = "gragas",
                Slot = SpellSlot.Q,
                CastRange = 1100f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gragasw",
                ChampionName = "gragas",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gragase",
                ChampionName = "gragas",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 200f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "gragase",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gragasr",
                ChampionName = "gragas",
                Slot = SpellSlot.R,
                CastRange = 1150f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "gragasrboom",
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gravesclustershot",
                ChampionName = "graves",
                Slot = SpellSlot.Q,
                CastRange = 1025,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "gravesclustershotattack",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gravessmokegrenade",
                ChampionName = "graves",
                Slot = SpellSlot.W,
                CastRange = 1100f,
                Delay = 300f,
                HitType = new HitType[] { },
                MissileSpeed = 1650
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gravessmokegrenadeboom",
                ChampionName = "graves",
                Slot = SpellSlot.W,
                CastRange = 1100f, // 950 + Radius
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1350
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "gravesmove",
                ChampionName = "graves",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 300f,
                HitType = new HitType[] { },
                MissileSpeed = 1000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "graveschargeshot",
                ChampionName = "graves",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileName = "graveschargeshotshot",
                MissileSpeed = 2100
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "hecarimrapidslash",
                ChampionName = "hecarim",
                Slot = SpellSlot.Q,
                CastRange = 350f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "hecarimw",
                ChampionName = "hecarim",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "hecarimramp",
                ChampionName = "hecarim",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "hecarimult",
                ChampionName = "hecarim",
                Slot = SpellSlot.R,
                CastRange = 1350f,
                Delay = 50f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "heimerdingerq",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "heimerdingerw",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.W,
                CastRange = 1100,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "heimerdingere",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.E,
                CastRange = 1025f, // 925 + Radius
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "heimerdingerespell",
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "heimerdingerr",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 230f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "heimerdingereult",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.E,
                CastRange = 1250f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ireliagatotsu",
                ChampionName = "irelia",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 150f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ireliahitenstyle",
                ChampionName = "irelia",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 230f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ireliaequilibriumstrike",
                ChampionName = "irelia",
                Slot = SpellSlot.E,
                CastRange = 450f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ireliatranscendentblades",
                ChampionName = "irelia",
                Slot = SpellSlot.R,
                CastRange = 1200f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "ireliatranscendentblades",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "howlinggale",
                ChampionName = "janna",
                Slot = SpellSlot.Q,
                CastRange = 850f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sowthewind",
                ChampionName = "janna",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "eyeofthestorm",
                ChampionName = "janna",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "reapthewhirlwind",
                ChampionName = "janna",
                Slot = SpellSlot.R,
                CastRange = 725f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jarvanivdragonstrike",
                ChampionName = "jarvaniv",
                Slot = SpellSlot.Q,
                CastRange = 700f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jarvanivgoldenaegis",
                ChampionName = "jarvaniv",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jarvanivdemacianstandard",
                ChampionName = "jarvaniv",
                Slot = SpellSlot.E,
                CastRange = 830f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "jarvanivdemacianstandard",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jarvanivcataclysm",
                ChampionName = "jarvaniv",
                Slot = SpellSlot.R,
                CastRange = 650f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jaxleapstrike",
                ChampionName = "jax",
                Slot = SpellSlot.Q,
                CastRange = 210f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jaxempowertwo",
                ChampionName = "jax",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jaxrelentlessasssault",
                ChampionName = "jax",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jaycetotheskies",
                ChampionName = "jayce",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jayceshockblast",
                ChampionName = "jayce",
                Slot = SpellSlot.Q,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileName = "jayceshockblastmis",
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jaycestaticfield",
                ChampionName = "jayce",
                Slot = SpellSlot.W,
                CastRange = 285f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jaycehypercharge",
                ChampionName = "jayce",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 750f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jaycethunderingblow",
                ChampionName = "jayce",
                Slot = SpellSlot.E,
                CastRange = 325f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jayceaccelerationgate",
                ChampionName = "jayce",
                Slot = SpellSlot.E,
                CastRange = 685f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jaycestancehtg",
                ChampionName = "jayce",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 750f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jaycestancegth",
                ChampionName = "jayce",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 750f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jinxq",
                ChampionName = "jinx",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jinxw",
                ChampionName = "jinx",
                Slot = SpellSlot.W,
                CastRange = 1550f,
                Delay = 600f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "jinxwmissile",
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jinxe",
                ChampionName = "jinx",
                Slot = SpellSlot.E,
                CastRange = 900f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jinxr",
                ChampionName = "jinx",
                Slot = SpellSlot.R,
                CastRange = 25000f,
                Global = true,
                Delay = 600f,
                MissileName = "jinxr",
                ExtraMissileNames = new[] { "jinxrwrapper" },
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileSpeed = 1700
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "karmaq",
                ChampionName = "karma",
                Slot = SpellSlot.Q,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "karmaqmissile",
                MissileSpeed = 1800
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "karmaspiritbind",
                ChampionName = "karma",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "karmasolkimshield",
                ChampionName = "karma",
                Slot = SpellSlot.E,
                CastRange = 800f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "karmamantra",
                ChampionName = "karma",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1300
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "laywaste",
                ChampionName = "karthus",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Delay = 250f,
                HitType = new HitType[] { },
                ExtraMissileNames = new[]  {
                            "karthuslaywastea3", "karthuslaywastea1", "karthuslaywastedeada1", "karthuslaywastedeada2",
                            "karthuslaywastedeada3"
                        },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "wallofpain",
                ChampionName = "karthus",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "defile",
                ChampionName = "karthus",
                Slot = SpellSlot.E,
                CastRange = 550f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fallenone",
                ChampionName = "karthus",
                Slot = SpellSlot.R,
                CastRange = 22000f,
                Global = true,
                Delay = 2800f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nulllance",
                ChampionName = "kassadin",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1900
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "netherblade",
                ChampionName = "kassadin",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "forcepulse",
                ChampionName = "kassadin",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "riftwalk",
                ChampionName = "kassadin",
                Slot = SpellSlot.R,
                CastRange = 675f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "riftwalk",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "katarinaq",
                ChampionName = "katarina",
                Slot = SpellSlot.Q,
                CastRange = 675f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1800
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "katarinaw",
                ChampionName = "katarina",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1800
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "katarinae",
                ChampionName = "katarina",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "katarinar",
                ChampionName = "katarina",
                Slot = SpellSlot.R,
                CastRange = 550f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.ForceExhaust },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "judicatorreckoning",
                ChampionName = "kayle",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "judicatordevineblessing",
                ChampionName = "kayle",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Delay = 220f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "judicatorrighteousfury",
                ChampionName = "kayle",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "judicatorintervention",
                ChampionName = "kayle",
                Slot = SpellSlot.R,
                CastRange = 900f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kennenshurikenhurlmissile1",
                ChampionName = "kennen",
                Slot = SpellSlot.Q,
                CastRange = 1175f,
                Delay = 180f,
                HitType = new HitType[] { },
                MissileName = "kennenshurikenhurlmissile1",
                MissileSpeed = 1700
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kennenbringthelight",
                ChampionName = "kennen",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kennenlightningrush",
                ChampionName = "kennen",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kennenshurikenstorm",
                ChampionName = "kennen",
                Slot = SpellSlot.R,
                CastRange = 550f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "khazixq",
                ChampionName = "khazix",
                Slot = SpellSlot.Q,
                CastRange = 325f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "khazixqlong",
                ChampionName = "khazix",
                Slot = SpellSlot.Q,
                CastRange = 375f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "khazixw",
                ChampionName = "khazix",
                Slot = SpellSlot.W,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "khazixwmissile",
                MissileSpeed = 81700
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "khazixwlong",
                ChampionName = "khazix",
                Slot = SpellSlot.W,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1700
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "khazixe",
                ChampionName = "khazix",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "khazixe",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "khazixelong",
                ChampionName = "khazix",
                Slot = SpellSlot.E,
                CastRange = 900f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "khazixr",
                ChampionName = "khazix",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "khazixrlong",
                ChampionName = "khazix",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kogmawcausticspittle",
                ChampionName = "kogmaw",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kogmawbioarcanbarrage",
                ChampionName = "kogmaw",
                Slot = SpellSlot.W,
                CastRange = 130f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kogmawvoidooze",
                ChampionName = "kogmaw",
                Slot = SpellSlot.E,
                CastRange = 1150f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "kogmawvoidoozemissile",
                MissileSpeed = 1250
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kogmawlivingartillery",
                ChampionName = "kogmaw",
                Slot = SpellSlot.R,
                CastRange = 2200f,
                Delay = 1200f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "kogmawlivingartillery",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leblancchaosorb",
                ChampionName = "leblanc",
                Slot = SpellSlot.Q,
                CastRange = 700f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leblancslide",
                ChampionName = "leblanc",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "leblancslide",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leblacslidereturn",
                ChampionName = "leblanc",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leblancsoulshackle",
                ChampionName = "leblanc",
                Slot = SpellSlot.E,
                CastRange = 925f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "leblancsoulshackle",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leblancchaosorbm",
                ChampionName = "leblanc",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leblancslidem",
                ChampionName = "leblanc",
                Slot = SpellSlot.R,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileName = "leblancslidem",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leblancslidereturnm",
                ChampionName = "leblanc",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leblancsoulshacklem",
                ChampionName = "leblanc",
                Slot = SpellSlot.R,
                CastRange = 925f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "leblancsoulshacklem",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "blindmonkqone",
                ChampionName = "leesin",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "blindmonkqone",
                MissileSpeed = 1800
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "blindmonkqtwo",
                ChampionName = "leesin",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "blindmonkwone",
                ChampionName = "leesin",
                Slot = SpellSlot.W,
                CastRange = 700f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "blindmonkwtwo",
                ChampionName = "leesin",
                Slot = SpellSlot.W,
                CastRange = 700f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "blindmonkeone",
                ChampionName = "leesin",
                Slot = SpellSlot.E,
                CastRange = 425f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "blindmonketwo",
                ChampionName = "leesin",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "blindmonkrkick",
                ChampionName = "leesin",
                Slot = SpellSlot.R,
                CastRange = 375f,
                Delay = 500f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leonashieldofdaybreak",
                ChampionName = "leona",
                Slot = SpellSlot.Q,
                CastRange = 215f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leonasolarbarrier",
                ChampionName = "leona",
                Slot = SpellSlot.W,
                CastRange = 250f,
                Delay = 3000f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leonazenithblade",
                ChampionName = "leona",
                Slot = SpellSlot.E,
                CastRange = 900f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileName = "leonazenithblademissile",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "leonasolarflare",
                ChampionName = "leona",
                Slot = SpellSlot.R,
                CastRange = 1200f,
                Delay = 1200f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "leonasolarflare",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "lissandraq",
                ChampionName = "lissandra",
                Slot = SpellSlot.Q,
                CastRange = 725f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "lissandraqmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "lissandraw",
                ChampionName = "lissandra",
                Slot = SpellSlot.W,
                CastRange = 450f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "lissandrae",
                ChampionName = "lissandra",
                Slot = SpellSlot.E,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "lissandraemissile",
                MissileSpeed = 850
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "lissandrar",
                ChampionName = "lissandra",
                Slot = SpellSlot.R,
                CastRange = 550f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "lucianq",
                ChampionName = "lucian",
                Slot = SpellSlot.Q,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "lucianq",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "lucianw",
                ChampionName = "lucian",
                Slot = SpellSlot.W,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "lucianwmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "luciane",
                ChampionName = "lucian",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "lucianr",
                ChampionName = "lucian",
                Slot = SpellSlot.R,
                CastRange = 1400f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "lucianrmissileoffhand",
                ExtraMissileNames = new[] { "lucianrmissile" },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "luluq",
                ChampionName = "lulu",
                Slot = SpellSlot.Q,
                CastRange = 925f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "luluqmissile",
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "luluw",
                ChampionName = "lulu",
                Slot = SpellSlot.W,
                CastRange = 650f,
                Delay = 640f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "lulue",
                ChampionName = "lulu",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 640f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "lulur",
                ChampionName = "lulu",
                Slot = SpellSlot.R,
                CastRange = 900f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "luxlightbinding",
                ChampionName = "lux",
                Slot = SpellSlot.Q,
                CastRange = 1300f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "luxlightbindingmis",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "luxprismaticwave",
                ChampionName = "lux",
                Slot = SpellSlot.W,
                CastRange = 1075f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "luxlightstrikekugel",
                ChampionName = "lux",
                Slot = SpellSlot.E,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "luxlightstrikekugel",
                MissileSpeed = 1300
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "luxlightstriketoggle",
                ChampionName = "lux",
                Slot = SpellSlot.E,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "luxmalicecannon",
                ChampionName = "lux",
                Slot = SpellSlot.R,
                CastRange = 3340f,
                Delay = 1750f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileName = "luxmalicecannonmis",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kalistamysticshot",
                ChampionName = "kalista",
                Slot = SpellSlot.Q,
                CastRange = 1200f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "kalistamysticshotmis",
                ExtraMissileNames = new[] { "kalistamysticshotmistrue" },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kalistaw",
                ChampionName = "kalista",
                Slot = SpellSlot.W,
                CastRange = 5000f,
                Delay = 800f,
                HitType = new HitType[] { },
                MissileSpeed = 200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "kalistaexpungewrapper",
                ChampionName = "kalista",
                Slot = SpellSlot.E,
                CastRange = 1200f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "seismicshard",
                ChampionName = "malphite",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "obduracy",
                ChampionName = "malphite",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "landslide",
                ChampionName = "malphite",
                Slot = SpellSlot.E,
                CastRange = 400f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ufslash",
                ChampionName = "malphite",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "ufslash",
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "alzaharcallofthevoid",
                ChampionName = "malzahar",
                Slot = SpellSlot.Q,
                CastRange = 900f,
                Delay = 600f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "alzaharcallofthevoid",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "alzaharnullzone",
                ChampionName = "malzahar",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "alzaharmaleficvisions",
                ChampionName = "malzahar",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "alzaharnethergrasp",
                ChampionName = "malzahar",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "maokaitrunkline",
                ChampionName = "maokai",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "maokaiunstablegrowth",
                ChampionName = "maokai",
                Slot = SpellSlot.W,
                CastRange = 650f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "maokaisapling2",
                ChampionName = "maokai",
                Slot = SpellSlot.E,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "maokaidrain3",
                ChampionName = "maokai",
                Slot = SpellSlot.R,
                CastRange = 625f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "alphastrike",
                ChampionName = "masteryi",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 600f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "meditate",
                ChampionName = "masteryi",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "wujustyle",
                ChampionName = "masteryi",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 230f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "highlander",
                ChampionName = "masteryi",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 370f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "missfortunericochetshot",
                ChampionName = "missfortune",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "missfortuneviciousstrikes",
                ChampionName = "missfortune",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "missfortunescattershot",
                ChampionName = "missfortune",
                Slot = SpellSlot.E,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "missfortunebullettime",
                ChampionName = "missfortune",
                Slot = SpellSlot.R,
                CastRange = 1400f,
                Delay = 500f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "monkeykingdoubleattack",
                ChampionName = "monkeyking",
                Slot = SpellSlot.Q,
                CastRange = 300f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 20
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "monkeykingdecoy",
                ChampionName = "monkeyking",
                Slot = SpellSlot.W,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "monkeykingdecoyswipe",
                ChampionName = "monkeyking",
                Slot = SpellSlot.W,
                CastRange = 325f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "monkeykingnimbus",
                ChampionName = "monkeyking",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "monkeykingspintowin",
                ChampionName = "monkeyking",
                Slot = SpellSlot.R,
                CastRange = 450f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "monkeykingspintowinleave",
                ChampionName = "monkeyking",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 700
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "mordekaisermaceofspades",
                ChampionName = "mordekaiser",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "mordekaisercreepindeathcast",
                ChampionName = "mordekaiser",
                Slot = SpellSlot.W,
                CastRange = 750f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "mordekaisersyphoneofdestruction",
                ChampionName = "mordekaiser",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "mordekaiserchildrenofthegrave",
                ChampionName = "mordekaiser",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "darkbindingmissile",
                ChampionName = "morgana",
                Slot = SpellSlot.Q,
                CastRange = 1175f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "darkbindingmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "tormentedsoil",
                ChampionName = "morgana",
                Slot = SpellSlot.W,
                CastRange = 850f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "blackshield",
                ChampionName = "morgana",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "soulshackles",
                ChampionName = "morgana",
                Slot = SpellSlot.R,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "namiq",
                ChampionName = "nami",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "namiqmissile",
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "namiw",
                ChampionName = "nami",
                Slot = SpellSlot.W,
                CastRange = 725f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1100
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "namie",
                ChampionName = "nami",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "namir",
                ChampionName = "nami",
                Slot = SpellSlot.R,
                CastRange = 2550f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileName = "namirmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nasusq",
                ChampionName = "nasus",
                Slot = SpellSlot.Q,
                CastRange = 450f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nasusw",
                ChampionName = "nasus",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nasuse",
                ChampionName = "nasus",
                Slot = SpellSlot.E,
                CastRange = 850f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nasusr",
                ChampionName = "nasus",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nautilusanchordrag",
                ChampionName = "nautilus",
                Slot = SpellSlot.Q,
                CastRange = 1080f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileName = "nautilusanchordragmissile",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nautiluspiercinggaze",
                ChampionName = "nautilus",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nautilussplashzone",
                ChampionName = "nautilus",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1300
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nautilusgandline",
                ChampionName = "nautilus",
                Slot = SpellSlot.R,
                CastRange = 1250f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "javelintoss",
                ChampionName = "nidalee",
                Slot = SpellSlot.Q,
                CastRange = 1500f,
                Delay = 125f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "javelintoss",
                MissileSpeed = 1300
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "takedown",
                ChampionName = "nidalee",
                Slot = SpellSlot.Q,
                CastRange = 150f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bushwhack",
                ChampionName = "nidalee",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "pounce",
                ChampionName = "nidalee",
                Slot = SpellSlot.W,
                CastRange = 375f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "primalsurge",
                ChampionName = "nidalee",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "swipe",
                ChampionName = "nidalee",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "aspectofthecougar",
                ChampionName = "nidalee",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nocturneduskbringer",
                ChampionName = "nocturne",
                Slot = SpellSlot.Q,
                CastRange = 1125f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nocturneshroudofdarkness",
                ChampionName = "nocturne",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nocturneunspeakablehorror",
                ChampionName = "nocturne",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "nocturneparanoia",
                ChampionName = "nocturne",
                Slot = SpellSlot.R,
                CastRange = 20000f,
                Global = true,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "consume",
                ChampionName = "nunu",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bloodboil",
                ChampionName = "nunu",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "iceblast",
                ChampionName = "nunu",
                Slot = SpellSlot.E,
                CastRange = 550f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "absolutezero",
                ChampionName = "nunu",
                Slot = SpellSlot.R,
                CastRange = 650f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "olafaxethrowcast",
                ChampionName = "olaf",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "olafaxethrow",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "olaffrenziedstrikes",
                ChampionName = "olaf",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "olafrecklessstrike",
                ChampionName = "olaf",
                Slot = SpellSlot.E,
                CastRange = 325f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "olafragnarok",
                ChampionName = "olaf",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "orianaizunacommand",
                ChampionName = "orianna",
                Slot = SpellSlot.Q,
                CastRange = 900f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "orianaizuna",
                FromObject = new[] { "yomu_ring" },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "orianadissonancecommand",
                ChampionName = "orianna",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 350f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "orianadissonancecommand",
                FromObject = new[] { "yomu_ring" },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "orianaredactcommand",
                ChampionName = "orianna",
                Slot = SpellSlot.E,
                CastRange = 1095f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "orianaredact",
                FromObject = new[] { "yomu_ring" },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "orianadetonatecommand",
                ChampionName = "orianna",
                Slot = SpellSlot.R,
                CastRange = 425f,
                Delay = 500f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "orianadetonatecommand",
                FromObject = new[] { "yomu_ring" },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "pantheonq",
                ChampionName = "pantheon",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1900
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "pantheonw",
                ChampionName = "pantheon",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "pantheone",
                ChampionName = "pantheon",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "pantheonrjump",
                ChampionName = "pantheon",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 1000f,
                HitType = new HitType[] { },
                MissileSpeed = 3000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "pantheonrfall",
                ChampionName = "pantheon",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 1000f,
                HitType = new HitType[] { },
                MissileSpeed = 3000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "poppydevastatingblow",
                ChampionName = "poppy",
                Slot = SpellSlot.Q,
                CastRange = 300f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "poppyparagonofdemacia",
                ChampionName = "poppy",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "poppyheroiccharge",
                ChampionName = "poppy",
                Slot = SpellSlot.E,
                CastRange = 525f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "poppydiplomaticimmunity",
                ChampionName = "poppy",
                Slot = SpellSlot.R,
                CastRange = 900f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "quinnq",
                ChampionName = "quinn",
                Slot = SpellSlot.Q,
                CastRange = 1025f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "quinnqmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "quinnw",
                ChampionName = "quinn",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "quinne",
                ChampionName = "quinn",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 775
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "quinnr",
                ChampionName = "quinn",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "quinnrfinale",
                ChampionName = "quinn",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "powerball",
                ChampionName = "rammus",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 775
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "defensiveballcurl",
                ChampionName = "rammus",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "puncturingtaunt",
                ChampionName = "rammus",
                Slot = SpellSlot.E,
                CastRange = 325f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "tremors2",
                ChampionName = "rammus",
                Slot = SpellSlot.R,
                CastRange = 300f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "renektoncleave",
                ChampionName = "renekton",
                Slot = SpellSlot.Q,
                CastRange = 225f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "renektonpreexecute",
                ChampionName = "renekton",
                Slot = SpellSlot.W,
                CastRange = 275f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "renektonsliceanddice",
                ChampionName = "renekton",
                Slot = SpellSlot.E,
                CastRange = 450f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "renektonreignofthetyrant",
                ChampionName = "renekton",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rengarq",
                ChampionName = "rengar",
                Slot = SpellSlot.Q,
                CastRange = 275f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rengarw",
                ChampionName = "rengar",
                Slot = SpellSlot.W,
                CastRange = 500f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rengare",
                ChampionName = "rengar",
                Slot = SpellSlot.E,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "rengarefinal",
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rengarr",
                ChampionName = "rengar",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "reksaiq",
                ChampionName = "reksai",
                Slot = SpellSlot.Q,
                CastRange = 275f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "reksaiqburrowed",
                ChampionName = "reksai",
                Slot = SpellSlot.W,
                CastRange = 1650f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "reksaiqburrowedmis",
                MissileSpeed = 1950
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "reksaiw",
                ChampionName = "reksai",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 350f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "reksaiwburrowed",
                ChampionName = "reksai",
                Slot = SpellSlot.W,
                CastRange = 200f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "reksaie",
                ChampionName = "reksai",
                Slot = SpellSlot.E,
                CastRange = 250f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "reksaieburrowed",
                ChampionName = "reksai",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 900f,
                HitType = new HitType[] { },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "reksair",
                ChampionName = "reksai",
                Slot = SpellSlot.R,
                CastRange = 2.147484E+09f,
                Delay = 1000f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "riventricleave",
                ChampionName = "riven",
                Slot = SpellSlot.Q,
                CastRange = 270f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rivenmartyr",
                ChampionName = "riven",
                Slot = SpellSlot.W,
                CastRange = 260f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rivenfeint",
                ChampionName = "riven",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rivenfengshuiengine",
                ChampionName = "riven",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rivenizunablade",
                ChampionName = "riven",
                Slot = SpellSlot.R,
                CastRange = 1075f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileName = "rivenlightsabermissile",
                ExtraMissileNames = new[] { "rivenlightsabermissileside" },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rumbleflamethrower",
                ChampionName = "rumble",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rumbleshield",
                ChampionName = "rumble",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rumbegrenade",
                ChampionName = "rumble",
                Slot = SpellSlot.E,
                CastRange = 850f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "rumblecarpetbomb",
                ChampionName = "rumble",
                Slot = SpellSlot.R,
                CastRange = 1700f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ryzeq",
                ChampionName = "ryze",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ryzew",
                ChampionName = "ryze",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ryzee",
                ChampionName = "ryze",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ryzer",
                ChampionName = "ryze",
                Slot = SpellSlot.R,
                CastRange = 625f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sejuaniarcticassault",
                ChampionName = "sejuani",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sejuaninorthernwinds",
                ChampionName = "sejuani",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 1000f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sejuaniwintersclaw",
                ChampionName = "sejuani",
                Slot = SpellSlot.E,
                CastRange = 550f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sejuaniglacialprisoncast",
                ChampionName = "sejuani",
                Slot = SpellSlot.R,
                CastRange = 1200f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "sejuaniglacialprison",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "deceive",
                ChampionName = "shaco",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "jackinthebox",
                ChampionName = "shaco",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "twoshivpoison",
                ChampionName = "shaco",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "hallucinatefull",
                ChampionName = "shaco",
                Slot = SpellSlot.R,
                CastRange = 1125f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 395
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shenvorpalstar",
                ChampionName = "shen",
                Slot = SpellSlot.Q,
                CastRange = 475f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shenfeint",
                ChampionName = "shen",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shenshadowdash",
                ChampionName = "shen",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "shenshadowdash",
                MissileSpeed = 1250
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shenstandunited",
                ChampionName = "shen",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shyvanadoubleattack",
                ChampionName = "shyvana",
                Slot = SpellSlot.Q,
                CastRange = 275f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shyvanadoubleattackdragon",
                ChampionName = "shyvana",
                Slot = SpellSlot.Q,
                CastRange = 325f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shyvanaimmolationauraqw",
                ChampionName = "shyvana",
                Slot = SpellSlot.W,
                CastRange = 275f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shyvanaimmolateddragon",
                ChampionName = "shyvana",
                Slot = SpellSlot.W,
                CastRange = 250f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shyvanafireball",
                ChampionName = "shyvana",
                Slot = SpellSlot.E,
                CastRange = 925f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "shyvanafireballmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shyvanafireballdragon2",
                ChampionName = "shyvana",
                Slot = SpellSlot.E,
                CastRange = 925f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shyvanatransformcast",
                ChampionName = "shyvana",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                Delay = 100f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl,
                        global::KurisuSivir.HitType.Ultimate
                    },
                MissileName = "shyvanatransformcast",
                MissileSpeed = 1100
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "poisentrail",
                ChampionName = "singed",
                Slot = SpellSlot.Q,
                CastRange = 250f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "megaadhesive",
                ChampionName = "singed",
                Slot = SpellSlot.W,
                CastRange = 1175f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 700
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fling",
                ChampionName = "singed",
                Slot = SpellSlot.E,
                CastRange = 125f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "insanitypotion",
                ChampionName = "singed",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sionq",
                ChampionName = "sion",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 2000f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sionw",
                ChampionName = "sion",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sione",
                ChampionName = "sion",
                Slot = SpellSlot.E,
                CastRange = 725f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "sionemissile",
                MissileSpeed = 1800
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sionr",
                ChampionName = "sion",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "",
                MissileSpeed = 500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sivirq",
                ChampionName = "sivir",
                Slot = SpellSlot.Q,
                CastRange = 1165f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "sivirqmissile",
                ExtraMissileNames = new[] { "sivirqmissilereturn" },
                MissileSpeed = 1350
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sivirw",
                ChampionName = "sivir",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sivire",
                ChampionName = "sivir",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sivirr",
                ChampionName = "sivir",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "skarnervirulentslash",
                ChampionName = "skarner",
                Slot = SpellSlot.Q,
                CastRange = 350f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "skarnerexoskeleton",
                ChampionName = "skarner",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "skarnerfracture",
                ChampionName = "skarner",
                Slot = SpellSlot.E,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "skarnerfracturemissile",
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "skarnerimpale",
                ChampionName = "skarner",
                Slot = SpellSlot.R,
                CastRange = 350f,
                Delay = 350f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sonaq",
                ChampionName = "sona",
                Slot = SpellSlot.Q,
                CastRange = 700f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sonaw",
                ChampionName = "sona",
                Slot = SpellSlot.W,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sonae",
                ChampionName = "sona",
                Slot = SpellSlot.E,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sonar",
                ChampionName = "sona",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "sonar",
                MissileSpeed = 2400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sorakaq",
                ChampionName = "soraka",
                Slot = SpellSlot.Q,
                CastRange = 970f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "",
                MissileSpeed = 1100
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sorakaw",
                ChampionName = "soraka",
                Slot = SpellSlot.W,
                CastRange = 750f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sorakae",
                ChampionName = "soraka",
                Slot = SpellSlot.E,
                CastRange = 925f,
                Delay = 1750f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "sorakar",
                ChampionName = "soraka",
                Slot = SpellSlot.R,
                CastRange = 25000f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "swaindecrepify",
                ChampionName = "swain",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "swainshadowgrasp",
                ChampionName = "swain",
                Slot = SpellSlot.W,
                CastRange = 1040f,
                Delay = 1100f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "swainshadowgrasp",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "swaintorment",
                ChampionName = "swain",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "swainmetamorphism",
                ChampionName = "swain",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 950
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "syndraq",
                ChampionName = "syndra",
                Slot = SpellSlot.Q,
                CastRange = 800f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "syndraq",
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "syndrawcast",
                ChampionName = "syndra",
                Slot = SpellSlot.W,
                CastRange = 950f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "syndrawcast",
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "syndrae",
                ChampionName = "syndra",
                Slot = SpellSlot.E,
                CastRange = 950f,
                Delay = 300f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "syndrae",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "syndrar",
                ChampionName = "syndra",
                Slot = SpellSlot.R,
                CastRange = 675f,
                Delay = 450f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileSpeed = 1250
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "talonnoxiandiplomacy",
                ChampionName = "talon",
                Slot = SpellSlot.Q,
                CastRange = 275f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "talonrake",
                ChampionName = "talon",
                Slot = SpellSlot.W,
                CastRange = 750f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "talonrakemissileone",
                MissileSpeed = 2300
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "taloncutthroat",
                ChampionName = "talon",
                Slot = SpellSlot.E,
                CastRange = 750f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "talonshadowassault",
                ChampionName = "talon",
                Slot = SpellSlot.R,
                CastRange = 750f,
                Delay = 260f,
                HitType = new[] { global::KurisuSivir.HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "imbue",
                ChampionName = "taric",
                Slot = SpellSlot.Q,
                CastRange = 750f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "shatter",
                ChampionName = "taric",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "dazzle",
                ChampionName = "taric",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "tarichammersmash",
                ChampionName = "taric",
                Slot = SpellSlot.R,
                CastRange = 400f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "blindingdart",
                ChampionName = "teemo",
                Slot = SpellSlot.Q,
                CastRange = 580f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "movequick",
                ChampionName = "teemo",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 943
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "toxicshot",
                ChampionName = "teemo",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bantamtrap",
                ChampionName = "teemo",
                Slot = SpellSlot.R,
                CastRange = 230f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "threshq",
                ChampionName = "thresh",
                Slot = SpellSlot.Q,
                CastRange = 1175f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileName = "threshqmissile",
                MissileSpeed = 1900
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "threshw",
                ChampionName = "thresh",
                Slot = SpellSlot.W,
                CastRange = 950f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "threshe",
                ChampionName = "thresh",
                Slot = SpellSlot.E,
                CastRange = 400f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileName = "threshemissile1",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "threshrpenta",
                ChampionName = "thresh",
                Slot = SpellSlot.R,
                CastRange = 420f,
                Delay = 300f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "tristanaq",
                ChampionName = "tristana",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "tristanaw",
                ChampionName = "tristana",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1150
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "tristanae",
                ChampionName = "tristana",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "tristanar",
                ChampionName = "tristana",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "trundletrollsmash",
                ChampionName = "trundle",
                Slot = SpellSlot.Q,
                CastRange = 275f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "trundledesecrate",
                ChampionName = "trundle",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "trundlecircle",
                ChampionName = "trundle",
                Slot = SpellSlot.E,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "trundlepain",
                ChampionName = "trundle",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bloodlust",
                ChampionName = "tryndamere",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "mockingshout",
                ChampionName = "tryndamere",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "slashcast",
                ChampionName = "tryndamere",
                Slot = SpellSlot.E,
                CastRange = 660f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "slashcast",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "undyingrage",
                ChampionName = "tryndamere",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "hideinshadows",
                ChampionName = "twich",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 450f,
                HitType = new[] { global::KurisuSivir.HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "twitchvenomcask",
                ChampionName = "twich",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "twitchvenomcaskmissile",
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "twitchvenomcaskmissle",
                ChampionName = "twich",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "expunge",
                ChampionName = "twich",
                Slot = SpellSlot.E,
                CastRange = 1200f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "fullautomatic",
                ChampionName = "twich",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "wildcards",
                ChampionName = "twistedfate",
                Slot = SpellSlot.Q,
                CastRange = 1450f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "sealfatemissile",
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "pickacard",
                ChampionName = "twistedfate",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "goldcardpreattack",
                ChampionName = "twistedfate",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "redcardpreattack",
                ChampionName = "twistedfate",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bluecardpreattack",
                ChampionName = "twistedfate",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "cardmasterstack",
                ChampionName = "twistedfate",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "destiny",
                ChampionName = "twistedfate",
                Slot = SpellSlot.R,
                CastRange = 5250f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "udyrtigerstance",
                ChampionName = "udyr",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "udyrturtlestance",
                ChampionName = "udyr",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "udyrbearstanceattack",
                ChampionName = "udyr",
                Slot = SpellSlot.E,
                CastRange = 250f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "udyrphoenixstance",
                ChampionName = "udyr",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "urgotheatseekinglineqqmissile",
                ChampionName = "urgot",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "urgotheatseekingmissile",
                ChampionName = "urgot",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "urgotterrorcapacitoractive2",
                ChampionName = "urgot",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "urgotplasmagrenade",
                ChampionName = "urgot",
                Slot = SpellSlot.E,
                CastRange = 950f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "urgotplasmagrenadeboom",
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "urgotplasmagrenadeboom",
                ChampionName = "urgot",
                Slot = SpellSlot.E,
                CastRange = 950f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "urgotswap2",
                ChampionName = "urgot",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1800
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "varusq",
                ChampionName = "varus",
                Slot = SpellSlot.Q,
                CastRange = 1250f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "varusqmissile",
                MissileSpeed = 1900
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "varusw",
                ChampionName = "varus",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "varuse",
                ChampionName = "varus",
                Slot = SpellSlot.E,
                CastRange = 925f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "varuse",
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "varusr",
                ChampionName = "varus",
                Slot = SpellSlot.R,
                CastRange = 1300f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileName = "varusrmissile",
                MissileSpeed = 1950
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vaynetumble",
                ChampionName = "vayne",
                Slot = SpellSlot.Q,
                CastRange = 850f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vaynesilverbolts",
                ChampionName = "vayne",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vaynecondemnmissile",
                ChampionName = "vayne",
                Slot = SpellSlot.E,
                CastRange = 450f,
                Delay = 500f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vayneinquisition",
                ChampionName = "vayne",
                Slot = SpellSlot.R,
                CastRange = 1200f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "veigarbalefulstrike",
                ChampionName = "veigar",
                Slot = SpellSlot.Q,
                CastRange = 950f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "veigarbalefulstrikemis",
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "veigardarkmatter",
                ChampionName = "veigar",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Delay = 1200f,
                HitType = new HitType[] { },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "veigareventhorizon",
                ChampionName = "veigar",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "veigarprimordialburst",
                ChampionName = "veigar",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "velkozq",
                ChampionName = "velkoz",
                Slot = SpellSlot.Q,
                CastRange = 1050f,
                Delay = 300f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "velkozqmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "velkozqmissle",
                ChampionName = "velkoz",
                Slot = SpellSlot.Q,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "velkozqplitactive",
                ChampionName = "velkoz",
                Slot = SpellSlot.Q,
                CastRange = 1050f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "velkozw",
                ChampionName = "velkoz",
                Slot = SpellSlot.W,
                CastRange = 1050f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileName = "velkozwmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "velkoze",
                ChampionName = "velkoz",
                Slot = SpellSlot.E,
                CastRange = 850f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "velkozemissile",
                MissileSpeed = 1700
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "velkozr",
                ChampionName = "velkoz",
                Slot = SpellSlot.R,
                CastRange = 1575f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "viq",
                ChampionName = "vi",
                Slot = SpellSlot.Q,
                CastRange = 800f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "viw",
                ChampionName = "vi",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vie",
                ChampionName = "vi",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vir",
                ChampionName = "vi",
                Slot = SpellSlot.R,
                CastRange = 800f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "viktorpowertransfer",
                ChampionName = "viktor",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "viktorgravitonfield",
                ChampionName = "viktor",
                Slot = SpellSlot.W,
                CastRange = 815f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "viktordeathray",
                ChampionName = "viktor",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "viktordeathraymis",
                MissileSpeed = 1210
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "viktorchaosstorm",
                ChampionName = "viktor",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 350f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.Danger
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vladimirtransfusion",
                ChampionName = "vladimir",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vladimirsanguinepool",
                ChampionName = "vladimir",
                Slot = SpellSlot.W,
                CastRange = 350f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vladimirtidesofblood",
                ChampionName = "vladimir",
                Slot = SpellSlot.E,
                CastRange = 610f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1100
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "vladimirhemoplague",
                ChampionName = "vladimir",
                Slot = SpellSlot.R,
                CastRange = 875f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "volibearq",
                ChampionName = "volibear",
                Slot = SpellSlot.Q,
                CastRange = 300f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "volibearw",
                ChampionName = "volibear",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1450
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "volibeare",
                ChampionName = "volibear",
                Slot = SpellSlot.E,
                CastRange = 425f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 825
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "volibearr",
                ChampionName = "volibear",
                Slot = SpellSlot.R,
                CastRange = 425f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 825
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "hungeringstrike",
                ChampionName = "warwick",
                Slot = SpellSlot.Q,
                CastRange = 400f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "hunterscall",
                ChampionName = "warwick",
                Slot = SpellSlot.W,
                CastRange = 1000f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "bloodscent",
                ChampionName = "warwick",
                Slot = SpellSlot.E,
                CastRange = 1250f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "infiniteduress",
                ChampionName = "warwick",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "xeratharcanopulsechargeup",
                ChampionName = "xerath",
                Slot = SpellSlot.Q,
                CastRange = 750f,
                Delay = 750f,
                HitType = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "xeratharcanebarrage2",
                ChampionName = "xerath",
                Slot = SpellSlot.W,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "xeratharcanebarrage2",
                MissileSpeed = 20
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "xerathmagespear",
                ChampionName = "xerath",
                Slot = SpellSlot.E,
                CastRange = 1050f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileName = "xerathmagespearmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "xerathlocusofpower2",
                ChampionName = "xerath",
                Slot = SpellSlot.R,
                CastRange = 5600f,
                Delay = 750f,
                HitType = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "xenzhaothrust3",
                ChampionName = "xinzhao",
                Slot = SpellSlot.Q,
                CastRange = 400f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "xenzhaobattlecry",
                ChampionName = "xinzhao",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitType = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "xenzhaosweep",
                ChampionName = "xinzhao",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl, global::KurisuSivir.HitType.Danger },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "xenzhaoparry",
                ChampionName = "xinzhao",
                Slot = SpellSlot.R,
                CastRange = 375f,
                Delay = 250f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yasuoqw",
                ChampionName = "yasuo",
                Slot = SpellSlot.Q,
                CastRange = 475f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yasuoq2w",
                ChampionName = "yasuo",
                Slot = SpellSlot.Q,
                CastRange = 475f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yasuoq3",
                ChampionName = "yasuo",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "yasuoq3mis",
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yasuowmovingwall",
                ChampionName = "yasuo",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yasuodashwrapper",
                ChampionName = "yasuo",
                Slot = SpellSlot.E,
                CastRange = 475f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 20
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yasuorknockupcombow",
                ChampionName = "yasuo",
                Slot = SpellSlot.R,
                CastRange = 1200f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yorickspectral",
                ChampionName = "yorick",
                Slot = SpellSlot.Q,
                CastRange = 350f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yorickdecayed",
                ChampionName = "yorick",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yorickravenous",
                ChampionName = "yorick",
                Slot = SpellSlot.E,
                CastRange = 550f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "yorickreviveally",
                ChampionName = "yorick",
                Slot = SpellSlot.R,
                CastRange = 900f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zacq",
                ChampionName = "zac",
                Slot = SpellSlot.Q,
                CastRange = 550f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "zacq",
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zacw",
                ChampionName = "zac",
                Slot = SpellSlot.W,
                CastRange = 350f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zace",
                ChampionName = "zac",
                Slot = SpellSlot.E,
                CastRange = 1550f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zacr",
                ChampionName = "zac",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zedq",
                ChampionName = "zed",
                Slot = SpellSlot.Q,
                CastRange = 900f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "zedshurikenmisone",
                FromObject = new[] { "Zed_Base_W_tar.troy", "Zed_Base_W_cloneswap_buf.troy" },
                ExtraMissileNames = new[] { "zedshurikenmistwo", "zedshurikenmisthree" },
                MissileSpeed = 1700
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zedw",
                ChampionName = "zed",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zede",
                ChampionName = "zed",
                Slot = SpellSlot.E,
                CastRange = 300f,
                Delay = 0f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zedr",
                ChampionName = "zed",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ziggsq",
                ChampionName = "ziggs",
                Slot = SpellSlot.Q,
                CastRange = 850f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "ziggsqspell",
                ExtraMissileNames = new[] { "ziggsqspell2", "ziggsqspell3" },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ziggsw",
                ChampionName = "ziggs",
                Slot = SpellSlot.W,
                CastRange = 850f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "ziggsw",
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ziggswtoggle",
                ChampionName = "ziggs",
                Slot = SpellSlot.W,
                CastRange = 850f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ziggse",
                ChampionName = "ziggs",
                Slot = SpellSlot.E,
                CastRange = 850f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileName = "ziggse",
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ziggse2",
                ChampionName = "ziggs",
                Slot = SpellSlot.E,
                CastRange = 850f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "ziggsr",
                ChampionName = "ziggs",
                Slot = SpellSlot.R,
                CastRange = 2250f,
                Delay = 1800f,
                HitType = new[] { global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate },
                MissileName = "ziggsr",
                MissileSpeed = 1750
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zileanq",
                ChampionName = "zilean",
                Slot = SpellSlot.Q,
                CastRange = 900f,
                Delay = 300f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "zileanqmissile",
                MissileSpeed = 2000
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zileanw",
                ChampionName = "zilean",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zileane",
                ChampionName = "zilean",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileSpeed = 1100
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zileanr",
                ChampionName = "zilean",
                Slot = SpellSlot.R,
                CastRange = 780f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zyraqfissure",
                ChampionName = "zyra",
                Slot = SpellSlot.Q,
                CastRange = 800f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.Danger },
                MissileName = "zyraqfissure",
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zyraseed",
                ChampionName = "zyra",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 250f,
                HitType = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zyragraspingroots",
                ChampionName = "zyra",
                Slot = SpellSlot.E,
                CastRange = 1100f,
                Delay = 250f,
                HitType = new[] { global::KurisuSivir.HitType.CrowdControl },
                MissileName = "zyragraspingroots",
                MissileSpeed = 1400
            });

            Spells.Add(new KurisuLib
            {
                SDataName = "zyrabramblezone",
                ChampionName = "zyra",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 500f,
                HitType =
                    new[]
                    {
                        global::KurisuSivir.HitType.Danger, global::KurisuSivir.HitType.Ultimate,
                        global::KurisuSivir.HitType.CrowdControl
                    },
                MissileSpeed = int.MaxValue
            });
        }

        public static KurisuLib GetByMissileName(string missilename)
        {
            foreach (var sdata in Spells)
            {
                if (sdata.MissileName != null && sdata.MissileName.ToLower() == missilename ||
                    sdata.ExtraMissileNames != null && sdata.ExtraMissileNames.Contains(missilename))
                {
                    return sdata;
                }
            }

            return null;
        }
    }
}