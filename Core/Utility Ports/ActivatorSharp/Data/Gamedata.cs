#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Data/Gamedata.cs
// Date:		28/07/2016
// Author:		Robin Kurisu
#endregion

using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Activator.Base;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Data
{
    public class Gamedata
    {
        public string SDataName { get; set; }
        public string ChampionName { get; set; }
        public SpellSlot Slot { get; set; }
        public float CastRange { get; set; }
        public float Radius { get; set; } 
        public bool Global { get; set; }
        public float Delay { get; set; } = 250f;
        public bool FixedRange { get; set; }
        public string MissileName { get; set; }
        public string[] ExtraMissileNames { get; set; } = { "" };
        public int MissileSpeed { get; set; } = int.MaxValue;
        public string[] FromObject { get; set; } 
        public HitType[] HitTypes { get; set; }

        public bool HeroNameMatch(string championname)
        {
            return ChampionName.ToLower() == championname.ToLower();
        }

        static Gamedata()
        {
            Spells.Add(new Gamedata
            {
                SDataName = "aatroxq",
                ChampionName = "aatrox",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Radius = 200f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "aatroxw",
                ChampionName = "aatrox",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {   
                SDataName = "aatroxw2",
                ChampionName = "aatrox",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "aatroxe",
                ChampionName = "aatrox",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1025f,
                Radius = 150f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "aatroxeconemissile",
                MissileSpeed = 1250
            });

            Spells.Add(new Gamedata
            {
                SDataName = "aatroxr",
                ChampionName = "aatrox",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ahriorbofdeception",
                ChampionName = "ahri",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 900f,
                Radius = 80f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "ahriorbmissile",
                ExtraMissileNames = new[] { "ahriorbreturn" },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ahrifoxfire",
                ChampionName = "ahri",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 600f,
                Radius = 600f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ahriseduce",
                ChampionName = "ahri",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 975f,
                Radius = 60f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "ahriseducemissile",
                MissileSpeed = 1550
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ahritumble",
                ChampionName = "ahri",
                Slot = SpellSlot.R,
                CastRange = 450f,
                Radius = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "akalimota",
                ChampionName = "akali",
                Slot = SpellSlot.Q,
                Radius = 171.9f,
                CastRange = 600f,
                Delay = 650f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "akalismokebomb",
                ChampionName = "akali",
                Slot = SpellSlot.W,
                CastRange = 1000f, // Range: 700 + additional for stealth detection
                Delay = 500f,
                HitTypes = new[] { HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "akalishadowswipe",
                ChampionName = "akali",
                Slot = SpellSlot.E,
                Radius = 325f,
                CastRange = 325f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "akalishadowdance",
                ChampionName = "akali",
                Slot = SpellSlot.R,
                Radius = 300f,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "pulverize",
                ChampionName = "alistar",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 365f,
                Radius = 365f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "headbutt",
                ChampionName = "alistar",
                Slot = SpellSlot.W,
                CastRange = 660f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "triumphantroar",
                ChampionName = "alistar",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "feroucioushowl",
                ChampionName = "alistar",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bandagetoss",
                ChampionName = "amumu",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1100f,
                Radius = 80f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "sadmummybandagetoss",
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "auraofdespair",
                ChampionName = "amumu",
                Slot = SpellSlot.W,
                CastRange = 300f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "tantrum",
                ChampionName = "amumu",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 150f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "curseofthesadmummy",
                ChampionName = "amumu",
                Slot = SpellSlot.R,
                CastRange = 560f,
                Radius = 560f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "flashfrost",
                ChampionName = "anivia",
                Slot = SpellSlot.Q,
                CastRange = 1150f, // 1075 + Shatter Radius
                Radius = 110f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "flashfrostspell",
                MissileSpeed = 850
            });

            Spells.Add(new Gamedata
            {
                SDataName = "crystalize",
                ChampionName = "anivia",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "frostbite",
                ChampionName = "anivia",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "glacialstorm",
                ChampionName = "anivia",
                Slot = SpellSlot.R,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "disintegrate",
                ChampionName = "annie",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Radius = 710f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "incinerate",
                ChampionName = "annie",
                Slot = SpellSlot.W,
                CastRange = 625f,
                Radius = 210f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "moltenshield",
                ChampionName = "annie",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "infernalguardian",
                ChampionName = "annie",
                Slot = SpellSlot.R,
                CastRange = 900f, // 600 + Cast Radius
                Delay = 0f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "frostshot",
                ChampionName = "ashe",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "frostarrow",
                ChampionName = "ashe",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "volley",
                ChampionName = "ashe",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 1200f,
                Radius = 250f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "volleyattack",
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ashespiritofthehawk",
                ChampionName = "ashe",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "enchantedcrystalarrow",
                ChampionName = "ashe",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 20000f,
                Global = true,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileName = "enchantedcrystalarrow",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "aurelionsolq",
                ChampionName = "aurelionsol",
                Slot = SpellSlot.Q,
                CastRange = 1500f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "aurelionsolqmissile",
                MissileSpeed = 850
            });

            Spells.Add(new Gamedata
            {
                SDataName = "aurelionsolw",
                ChampionName = "aurelionsol",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.None },
                MissileName = "aurelionsolwmis",
                MissileSpeed = 450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "aurelionsole",
                ChampionName = "aurelionsol",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.None },
                MissileName = "aurelionsole",
                MissileSpeed = 900
            });

            Spells.Add(new Gamedata
            {
                SDataName = "aurelionsolr",
                ChampionName = "aurelionsol",
                Slot = SpellSlot.R,
                CastRange = 1420f,
                Delay = 300f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Ultimate, HitType.Danger, HitType.Initiator },
                MissileName = "aurelionsolrbeammissile",
                MissileSpeed = 4600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "azirq",
                ChampionName = "azir",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Delay =  250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "azirqmissile",
                FromObject = new[] { "AzirSoldier" },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "azirqwrapper",
                ChampionName = "azir",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "azirqmissile",
                FromObject = new[] { "AzirSoldier" },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "azirr",
                ChampionName = "azir",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 475f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bardq",
                ChampionName = "bard",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 950f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "bardqmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bardw",
                ChampionName = "bard",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "barde",
                ChampionName = "bard",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 350f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bardr",
                ChampionName = "bard",
                Slot = SpellSlot.R,
                CastRange = 3400f,
                Delay = 450f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "bardr",
                MissileSpeed = 2100
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rocketgrab",
                ChampionName = "blitzcrank",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1050f,
                Radius = 70f,
                Delay = 250f,
                MissileName = "rocketgrabmissile",
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = 1800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "overdrive",
                ChampionName = "blitzcrank",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Radius = 100f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "powerfist",
                ChampionName = "blitzcrank",
                Slot = SpellSlot.E,
                CastRange = 300f,
                Radius = 210f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "staticfield",
                ChampionName = "blitzcrank",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "brandq",
                ChampionName = "brand",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1150f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "brandqmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "brandw",
                ChampionName = "brand",
                Slot = SpellSlot.W,
                CastRange = 240f,
                Delay = 550f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "",
                MissileSpeed = 20
            });

            Spells.Add(new Gamedata
            {
                SDataName = "brande",
                ChampionName = "brand",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "brandr",
                ChampionName = "brand",
                Slot = SpellSlot.R,
                CastRange = 750f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileSpeed = 1000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "braumq",
                ChampionName = "braum",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1100f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "braumqmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "braumqmissle",
                ChampionName = "braum",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1100f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "braumw",
                ChampionName = "braum",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "braume",
                ChampionName = "braum",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "braumrwrapper",
                ChampionName = "braum",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1250f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },
                MissileName = "braumrmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "caitlynpiltoverpeacemaker",
                ChampionName = "caitlyn",
                Slot = SpellSlot.Q,
                FixedRange = true,
                Radius = 60f,
                CastRange = 1300f,
                Delay = 450f,
                HitTypes = new HitType[] { },
                MissileName = "caitlynpiltoverpeacemaker",
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "caitlynyordletrap",
                ChampionName = "caitlyn",
                Slot = SpellSlot.W,
                Radius = 75f,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "caitlynentrapment",
                ChampionName = "caitlyn",
                Slot = SpellSlot.E,
                FixedRange = true,
                Radius = 70f,
                CastRange = 1050f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "caitlynentrapmentmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "cassiopeiaq",
                ChampionName = "cassiopeia",
                Slot = SpellSlot.Q,
                CastRange = 925f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "cassiopeianoxiousblast",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "caitlynaceinthehole",
                ChampionName = "caitlyn",
                Slot = SpellSlot.R,
                CastRange = 2000f,
                Radius = 100f,
                Delay = 900f,
                FixedRange = false,
                MissileName = "",
                HitTypes = new HitType[] {},
                MissileSpeed = 1500,
            });

            Spells.Add(new Gamedata
            {
                SDataName = "cassiopeiw",
                ChampionName = "cassiopeia",
                Slot = SpellSlot.W,
                CastRange = 925f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 2500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "cassiopeiae",
                ChampionName = "cassiopeia",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1900
            });

            Spells.Add(new Gamedata
            {
                SDataName = "cassiopeiar",
                ChampionName = "cassiopeia",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 875f,
                Delay = 350f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileName = "cassiopeiar",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "cassiopeiarstun",
                ChampionName = "cassiopeia",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 875f,
                Delay = 350f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileName = "cassiopeiarstun",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rupture",
                ChampionName = "chogath",
                Slot = SpellSlot.Q,
                CastRange = 950f,
                Radius = 250f,
                Delay = 900f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "feralscream",
                ChampionName = "chogath",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 300f,
                Radius = 210f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vorpalspikes",
                ChampionName = "chogath",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = 347
            });

            Spells.Add(new Gamedata
            {
                SDataName = "feast",
                ChampionName = "chogath",
                Slot = SpellSlot.R,
                CastRange = 300f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "phosphorusbomb",
                ChampionName = "corki",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "phosphorusbombmissile",
                MissileSpeed = 1125
            });

            Spells.Add(new Gamedata
            {
                SDataName = "carpetbomb",
                ChampionName = "corki",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = 700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ggun",
                ChampionName = "corki",
                Slot = SpellSlot.E,
                CastRange = 750f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "missilebarrage",
                ChampionName = "corki",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1225f,
                Delay = 150f,
                HitTypes = new HitType[] { },
                MissileName = "missilebarragemissile",
                ExtraMissileNames = new[] { "missilebarragemissile2" },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dariuscleave",
                ChampionName = "darius",
                Slot = SpellSlot.Q,
                FixedRange = true,
                Radius = 425f,
                CastRange = 425f,
                Delay = 750f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dariusnoxiantacticsonh",
                ChampionName = "darius",
                Slot = SpellSlot.W,
                CastRange = 205f,
                Delay = 150f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dariusaxegrabcone",
                ChampionName = "darius",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 555f,
                Delay = 150f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileName = "dariusaxegrabcone",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dariusexecute",
                ChampionName = "darius",
                Slot = SpellSlot.R,
                Radius = 475f,
                CastRange = 475f,
                Delay = 450f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dianaarc",
                ChampionName = "diana",
                Slot = SpellSlot.Q,
                CastRange = 830f,
                Radius = 195f,
                Delay = 300f,
                HitTypes = new HitType[] { },
                MissileName = "dianaarc",
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dianaorbs",
                ChampionName = "diana",
                Slot = SpellSlot.W,
                CastRange = 200f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dianavortex",
                ChampionName = "diana",
                Slot = SpellSlot.E,
                CastRange = 450f,
                Radius = 450f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dianateleport",
                ChampionName = "diana",
                Slot = SpellSlot.R,
                CastRange = 825f,
                Radius = 250f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Initiator },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dravenspinning",
                ChampionName = "draven",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dravenfury",
                ChampionName = "draven",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dravendoubleshot",
                ChampionName = "draven",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1050f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "dravendoubleshotmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "dravenrcast",
                ChampionName = "draven",
                Slot = SpellSlot.R,
                CastRange = 20000f,
                Global = true,
                Delay = 500f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileName = "dravenr",
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "infectedcleavermissilecast",
                ChampionName = "drmundo",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "infectedcleavermissile",
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "burningagony",
                ChampionName = "drmundo",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "masochism",
                ChampionName = "drmundo",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sadism",
                ChampionName = "drmundo",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ekkoq",
                ChampionName = "ekko",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1075f,
                Radius = 60f,
                Delay = 66f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "ekkoqmis",
                ExtraMissileNames = new[] { "ekkoqreturn" },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ekkoeattack",
                ChampionName = "ekko",
                Slot = SpellSlot.E,
                CastRange = 300f,
                Delay = 0f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ekkor",
                ChampionName = "ekko",
                Slot = SpellSlot.R,
                CastRange = 425f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                FromObject = new[] { "Ekko_Base_R_TrailEnd" },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "elisehumanq",
                ChampionName = "elise",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 550f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "elisespiderqcast",
                ChampionName = "elise",
                Slot = SpellSlot.Q,
                CastRange = 475f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "elisehumanw",
                ChampionName = "elise",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 750f,
                HitTypes = new HitType[] { },
                MissileSpeed = 5000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "elisespiderw",
                ChampionName = "elise",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "elisehumane",
                ChampionName = "elise",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1075f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileName = "elisehumane",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "elisespidereinitial",
                ChampionName = "elise",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "elisespideredescent",
                ChampionName = "elise",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "eliser",
                ChampionName = "elise",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "elisespiderr",
                ChampionName = "elise",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "evelynnq",
                ChampionName = "evelynn",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 500f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "evelynnw",
                ChampionName = "evelynn",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f, 
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "evelynne",
                ChampionName = "evelynn",
                Slot = SpellSlot.E,
                CastRange = 225f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "evelynnr",
                ChampionName = "evelynn",
                Slot = SpellSlot.R,
                CastRange = 900f, // 650f + Radius
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },
                MissileName = "evelynnr",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ezrealmysticshot",
                ChampionName = "ezreal",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1150f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "ezrealmysticshotmissile",
                ExtraMissileNames = new[] { "ezrealmysticshotpulsemissile" },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ezrealessenceflux",
                ChampionName = "ezreal",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 1050f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "ezrealessencefluxmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ezrealessencemissle",
                ChampionName = "ezreal",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 1050f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ezrealarcaneshift",
                ChampionName = "ezreal",
                Slot = SpellSlot.E,
                CastRange = 750f, // 475f + Bolt Range
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ezrealtrueshotbarrage",
                ChampionName = "ezreal",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 20000f,
                Global = true,
                Delay = 1000f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileName = "ezrealtrueshotbarrage",
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "terrify",
                ChampionName = "fiddlesticks",
                Slot = SpellSlot.Q,
                CastRange = 575f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "drain",
                ChampionName = "fiddlesticks",
                Slot = SpellSlot.W,
                CastRange = 575f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fiddlesticksdarkwind",
                ChampionName = "fiddlesticks",
                Slot = SpellSlot.E,
                CastRange = 750f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1100
            });

            Spells.Add(new Gamedata
            {
                SDataName = "crowstorm",
                ChampionName = "fiddlesticks",
                Slot = SpellSlot.R,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new[] { HitType.ForceExhaust, HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fioraq",
                ChampionName = "fiora",
                Slot = SpellSlot.Q,
                CastRange = 400f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fioraw",
                ChampionName = "fiora",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 750f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fiorae",
                ChampionName = "fiora",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fiorar",
                ChampionName = "fiora",
                Slot = SpellSlot.R,
                CastRange = 500f,
                Delay = 150f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fizzq",
                ChampionName = "fizz",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 550f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = 1900
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fizzw",
                ChampionName = "fizz",
                Slot = SpellSlot.W,
                CastRange = 175f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fizze",
                ChampionName = "fizz",
                Slot = SpellSlot.E,
                CastRange = 450f,
                Delay = 700f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fizzebuffer",
                ChampionName = "fizz",
                Slot = SpellSlot.E,
                CastRange = 330f,
                Delay = 0f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fizzejumptwo",
                ChampionName = "fizz",
                Slot = SpellSlot.E,
                CastRange = 270f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fizzr",
                ChampionName = "fizz",
                Slot = SpellSlot.R,
                CastRange = 1275f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "fizzmarinerdoommissile",
                MissileSpeed = 1350
            });

            Spells.Add(new Gamedata
            {
                SDataName = "galioresolutesmite",
                ChampionName = "galio",
                Slot = SpellSlot.Q,
                CastRange = 1040f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "galioresolutesmite",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "galiobulwark",
                ChampionName = "galio",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "galiorighteousgust",
                ChampionName = "galio",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1280f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "galiorighteousgust",
                MissileSpeed = 1300
            });

            Spells.Add(new Gamedata
            {
                SDataName = "galioidolofdurand",
                ChampionName = "galio",
                Slot = SpellSlot.R,
                CastRange = 600f,
                Delay = 0f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gangplankqwrapper",
                ChampionName = "gangplank",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gangplankqproceed",
                ChampionName = "gangplank",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gangplankw",
                ChampionName = "gangplank",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gangplanke",
                ChampionName = "gangplank",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gangplankr",
                ChampionName = "gangplank",
                Slot = SpellSlot.R,
                CastRange = 20000f,
                Global = true,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "garenq",
                ChampionName = "garen",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 300f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "garenqattack",
                ChampionName = "garen",
                Slot = SpellSlot.Q,
                CastRange = 350f,
                Delay = 0f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = int.MaxValue
            });


            Spells.Add(new Gamedata
            {
                SDataName = "gnarq",
                ChampionName = "gnar",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1185f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 2400,
                MissileName = "gnarqmissile",
                ExtraMissileNames = new[] { "gnarqmissilereturn" }
            });


            Spells.Add(new Gamedata
            {
                SDataName = "gnarbigq",
                ChampionName = "gnar",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1150f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 2000,
                MissileName = "gnarbigqmissile"
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gnarbigw",
                ChampionName = "gnar",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 600f,
                Delay = 600f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gnarult",
                ChampionName = "gnar",
                CastRange = 600f, // 590f + 10 Better safe than sorry. :)
                Slot = SpellSlot.R,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },

                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "garenw",
                ChampionName = "garen",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "garene",
                ChampionName = "garen",
                Slot = SpellSlot.E,
                CastRange = 660f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "garenr",
                ChampionName = "garen",
                Slot = SpellSlot.R,
                CastRange = 400f,
                Radius = 100f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gragasq",
                ChampionName = "gragas",
                Slot = SpellSlot.Q,
                CastRange = 1000, // 850f + Radius
                Delay = 500f,
                HitTypes = new HitType[] { },
                MissileName = "gragasqmissile",
                MissileSpeed = 1000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gragasqtoggle",
                ChampionName = "gragas",
                Slot = SpellSlot.Q,
                CastRange = 1000, // 850f + Radius
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileName = "gragasq",
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gragasqtoggle",
                ChampionName = "gragas",
                Slot = SpellSlot.Q,
                CastRange = 1100f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gragasw",
                ChampionName = "gragas",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gragase",
                ChampionName = "gragas",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 600f,
                Delay = 200f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl, HitType.Initiator },
                MissileName = "gragase",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gragasr",
                ChampionName = "gragas",
                Slot = SpellSlot.R,
                CastRange = 1150f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "gragasrboom",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gravesq",
                ChampionName = "graves",
                Slot = SpellSlot.Q,
                CastRange = 1025,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "gravesclustershotattack",
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gravesw",
                ChampionName = "graves",
                Slot = SpellSlot.W,
                CastRange = 1100f, // 950 + Radius
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1350
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gravese",
                ChampionName = "graves",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 300f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "gravesr",
                ChampionName = "graves",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                FixedRange = true,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileName = "graveschargeshotshot",
                MissileSpeed = 2100
            });

            Spells.Add(new Gamedata
            {
                SDataName = "hecarimrapidslash",
                ChampionName = "hecarim",
                Slot = SpellSlot.Q,
                CastRange = 350f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "hecarimw",
                ChampionName = "hecarim",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "hecarimramp",
                ChampionName = "hecarim",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "hecarimult",
                ChampionName = "hecarim",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1525f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },

                MissileName = "hecarimultmissile",
                ExtraMissileNames =
                    new[]
                    {
                        "hecarimultmissileskn4r1", "hecarimultmissileskn4r2", "hecarimultmissileskn4l1",
                        "hecarimultmissileskn4l2", "hecarimultmissileskn4rc"
                    },
                MissileSpeed = 1100
            });

            Spells.Add(new Gamedata
            {
                SDataName = "heimerdingerturretenergyblast",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 435f,
                HitTypes = new HitType[] { },
                FromObject = new[] { "heimerdinger_turret_idle" },
                MissileSpeed = 1650
            });

            Spells.Add(new Gamedata
            {
                SDataName = "heimerdingerturretbigenergyblast",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 350f,
                HitTypes = new HitType[] { },
                FromObject = new[] { "heimerdinger_base_r" },
                MissileSpeed = 1650
            });

            Spells.Add(new Gamedata
            {
                SDataName = "heimerdingerw",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 1100,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "heimerdingere",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.E,
                CastRange = 1025f, // 925 + Radius
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "heimerdingerespell",
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "heimerdingerr",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 230f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "heimerdingereult",
                ChampionName = "heimerdinger",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1450f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ireliagatotsu",
                ChampionName = "irelia",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 150f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ireliahitenstyle",
                ChampionName = "irelia",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 230f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ireliaequilibriumstrike",
                ChampionName = "irelia",
                Slot = SpellSlot.E,
                CastRange = 450f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ireliatranscendentblades",
                ChampionName = "irelia",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1200f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileName = "ireliatranscendentbladesspell",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "illaoiq",
                ChampionName = "illaoi",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 950f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "illaoiemis",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "illaoiw",
                ChampionName = "illaoi",
                Slot = SpellSlot.W,
                CastRange = 365f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "illaoie",
                ChampionName = "illaoi",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 950f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "illaoiemis",
                MissileSpeed = 1900
            });

            Spells.Add(new Gamedata
            {
                SDataName = "illaoir",
                ChampionName = "illaoi",
                Slot = SpellSlot.R,
                CastRange = 450f,
                Delay = 500f,
                HitTypes = new[] { HitType.Ultimate, HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ivernq",
                ChampionName = "ivern",
                Slot = SpellSlot.Q,
                Radius = 65f,
                CastRange = 1100f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "ivernq",
                MissileSpeed = 1300
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ivernq",
                ChampionName = "ivern",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "ivernw",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "iverne",
                ChampionName = "ivern",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "iverne",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "howlinggalespell",
                ChampionName = "janna",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1550f,
                Delay = 0f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "howlinggalespell",
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sowthewind",
                ChampionName = "janna",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "eyeofthestorm",
                ChampionName = "janna",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "reapthewhirlwind",
                ChampionName = "janna",
                Slot = SpellSlot.R,
                CastRange = 725f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jarvanivdragonstrike",
                ChampionName = "jarvaniv",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jarvanivgoldenaegis",
                ChampionName = "jarvaniv",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jarvanivdemacianstandard",
                ChampionName = "jarvaniv",
                Slot = SpellSlot.E,
                CastRange = 830f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "jarvanivdemacianstandard",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jarvanivcataclysm",
                ChampionName = "jarvaniv",
                Slot = SpellSlot.R,
                CastRange = 825f,
                Delay = 0f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jaxleapstrike",
                ChampionName = "jax",
                Slot = SpellSlot.Q,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jaxempowertwo",
                ChampionName = "jax",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jaxrelentlessasssault",
                ChampionName = "jax",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jaycetotheskies",
                ChampionName = "jayce",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 450f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jayceshockblast",
                ChampionName = "jayce",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1570f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileName = "jayceshockblastmis",
                MissileSpeed = 2350
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jaycestaticfield",
                ChampionName = "jayce",
                Slot = SpellSlot.W,
                CastRange = 285f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jaycehypercharge",
                ChampionName = "jayce",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 750f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jaycethunderingblow",
                ChampionName = "jayce",
                Slot = SpellSlot.E,
                CastRange = 325f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jayceaccelerationgate",
                ChampionName = "jayce",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jaycestancehtg",
                ChampionName = "jayce",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 750f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jaycestancegth",
                ChampionName = "jayce",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 750f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jhinq",
                ChampionName = "jhin",
                Slot = SpellSlot.Q,
                CastRange = 575f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jhinw",
                ChampionName = "jhin",
                Slot = SpellSlot.W,
                CastRange = 2250f,
                Delay = 750f,
                FixedRange = true,
                HitTypes = new HitType[] { },
                MissileName = "jhinwmissile",
                MissileSpeed = 5000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jhine",
                ChampionName = "jhin",
                Slot = SpellSlot.E,
                CastRange = 2250f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jhinrshot",
                ChampionName = "jhin",
                Slot = SpellSlot.R,
                CastRange = 3500f,
                Delay = 250f,
                FixedRange = true,
                MissileName = "jhinrshotmis",
                HitTypes = new HitType[] { },
                ExtraMissileNames = new[] { "jhinrmmissile", "jhinrshotmis4" },
                MissileSpeed = 5000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jinxq",
                ChampionName = "jinx",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jinxw",
                ChampionName = "jinx",
                Slot = SpellSlot.W,
                Radius = 60f,
                FixedRange = true,
                CastRange = 1500f,
                Delay = 450f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "jinxwmissile",
                MissileSpeed = 3300
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jinxe",
                ChampionName = "jinx",
                Slot = SpellSlot.E,
                CastRange = 900f,
                Radius = 315f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jinxr",
                ChampionName = "jinx",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 25000f,
                Radius = 140f,
                Delay = 450f,
                MissileName = "jinxr",
                ExtraMissileNames = new[] { "jinxrwrapper" },
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileSpeed = 1700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "karmaq",
                ChampionName = "karma",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1050f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "karmaqmissile",
                MissileSpeed = 1800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "karmaspiritbind",
                ChampionName = "karma",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "karmasolkimshield",
                ChampionName = "karma",
                Slot = SpellSlot.E,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "karmamantra",
                ChampionName = "karma",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1300
            });

            Spells.Add(new Gamedata
            {
                SDataName = "laywaste",
                ChampionName = "karthus",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Delay = 900f,
                HitTypes = new HitType[] { },
                ExtraMissileNames = new[]  {
                            "karthuslaywastea3", "karthuslaywastea1", "karthuslaywastedeada1", "karthuslaywastedeada2",
                            "karthuslaywastedeada3"
                        },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "wallofpain",
                ChampionName = "karthus",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "defile",
                ChampionName = "karthus",
                Slot = SpellSlot.E,
                CastRange = 550f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fallenone",
                ChampionName = "karthus",
                Slot = SpellSlot.R,
                CastRange = 22000f,
                Global = true,
                Delay = 2800f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nulllance",
                ChampionName = "kassadin",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = 1900
            });

            Spells.Add(new Gamedata
            {
                SDataName = "netherblade",
                ChampionName = "kassadin",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "forcepulse",
                ChampionName = "kassadin",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "riftwalk",
                ChampionName = "kassadin",
                Slot = SpellSlot.R,
                CastRange = 675f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileName = "riftwalk",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "katarinaq",
                ChampionName = "katarina",
                Slot = SpellSlot.Q,
                CastRange = 675f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "katarinaw",
                ChampionName = "katarina",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "katarinae",
                ChampionName = "katarina",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "katarinar",
                ChampionName = "katarina",
                Slot = SpellSlot.R,
                CastRange = 550f,
                Delay = 250f,
                HitTypes = new[] { HitType.ForceExhaust },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "judicatorreckoning",
                ChampionName = "kayle",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "judicatordevineblessing",
                ChampionName = "kayle",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Delay = 220f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "judicatorrighteousfury",
                ChampionName = "kayle",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "judicatorintervention",
                ChampionName = "kayle",
                Slot = SpellSlot.R,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kennenshurikenhurlmissile1",
                ChampionName = "kennen",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1175f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "kennenshurikenhurlmissile1",
                MissileSpeed = 1700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kennenbringthelight",
                ChampionName = "kennen",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kennenlightningrush",
                ChampionName = "kennen",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kennenshurikenstorm",
                ChampionName = "kennen",
                Slot = SpellSlot.R,
                CastRange = 550f,
                Delay = 500f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "khazixq",
                ChampionName = "khazix",
                Slot = SpellSlot.Q,
                CastRange = 325f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "khazixqlong",
                ChampionName = "khazix",
                Slot = SpellSlot.Q,
                CastRange = 375f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "khazixw",
                ChampionName = "khazix",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "khazixwmissile",
                MissileSpeed = 81700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "khazixwlong",
                ChampionName = "khazix",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "khazixe",
                ChampionName = "khazix",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "khazixe",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "khazixelong",
                ChampionName = "khazix",
                Slot = SpellSlot.E,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "khazixr",
                ChampionName = "khazix",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                Delay = 0f,
                HitTypes = new[] { HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "khazixrlong",
                ChampionName = "khazix",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                Delay = 0f,
                HitTypes = new[] { HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kindredq",
                ChampionName = "kindred",
                Slot = SpellSlot.Q,
                CastRange = 350f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kindrede",
                ChampionName = "kindred",
                Slot = SpellSlot.E,
                CastRange = 510f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kledq",
                ChampionName = "kled",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 800f,
                Radius = 45f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "kledqmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kledriderq",
                ChampionName = "kled",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 700f,
                Radius = 40f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "kledriderqmissile",
                MissileSpeed = 3000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kledw",
                ChampionName = "kled",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "klede",
                ChampionName = "kled",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 800f,
                Radius = 124f,
                Delay = 0f,
                HitTypes = new[] { HitType.Danger, HitType.Initiator },
                MissileName = "kledemissile",
                MissileSpeed = 1000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kledr",
                ChampionName = "kled",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kogmawq",
                ChampionName = "kogmaw",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1300f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "kogmawq",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kogmawbioarcanebarrage",
                ChampionName = "kogmaw",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kogmawvoidooze",
                ChampionName = "kogmaw",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1150f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "kogmawvoidoozemissile",
                MissileSpeed = 1250
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kogmawlivingartillery",
                ChampionName = "kogmaw",
                Slot = SpellSlot.R,
                CastRange = 2200f,
                Delay = 1200f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "kogmawlivingartillery",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leblancq",
                ChampionName = "leblanc",
                Slot = SpellSlot.Q,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leblancw",
                ChampionName = "leblanc",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Initiator },
                MissileName = "leblancw",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leblacwreturn",
                ChampionName = "leblanc",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leblance",
                ChampionName = "leblanc",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 925f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "leblancemissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leblancrq",
                ChampionName = "leblanc",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leblancrw",
                ChampionName = "leblanc",
                Slot = SpellSlot.R,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate, HitType.Initiator },
                MissileName = "leblancrw",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leblancrwreturn",
                ChampionName = "leblanc",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leblancre",
                ChampionName = "leblanc",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 925f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "leblancremissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "blindmonkqone",
                ChampionName = "leesin",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "blindmonkqone",
                MissileSpeed = 1800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "blindmonkqtwo",
                ChampionName = "leesin",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "blindmonkwone",
                ChampionName = "leesin",
                Slot = SpellSlot.W,
                CastRange = 700f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "blindmonkwtwo",
                ChampionName = "leesin",
                Slot = SpellSlot.W,
                CastRange = 700f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "blindmonkeone",
                ChampionName = "leesin",
                Slot = SpellSlot.E,
                CastRange = 425f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "blindmonketwo",
                ChampionName = "leesin",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "blindmonkrkick",
                ChampionName = "leesin",
                Slot = SpellSlot.R,
                CastRange = 375f,
                Delay = 500f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leonashieldofdaybreak",
                ChampionName = "leona",
                Slot = SpellSlot.Q,
                CastRange = 215f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leonasolarbarrier",
                ChampionName = "leona",
                Slot = SpellSlot.W,
                CastRange = 250f,
                Delay = 3000f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leonazenithblade",
                ChampionName = "leona",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 900f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileName = "leonazenithblademissile",
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "leonasolarflare",
                ChampionName = "leona",
                Slot = SpellSlot.R,
                CastRange = 1200f,
                Delay = 1200f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "leonasolarflare",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "lissandraq",
                ChampionName = "lissandra",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 725f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "lissandraqmissile",
                MissileSpeed = 2250
            });

            Spells.Add(new Gamedata
            {
                SDataName = "lissandraw",
                ChampionName = "lissandra",
                Slot = SpellSlot.W,
                CastRange = 450f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "lissandrae",
                ChampionName = "lissandra",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1050f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "lissandraemissile",
                MissileSpeed = 850
            });

            Spells.Add(new Gamedata
            {
                SDataName = "lissandrar",
                ChampionName = "lissandra",
                Slot = SpellSlot.R,
                CastRange = 550f,
                Delay = 250f,
                HitTypes = new[]
                {
                    HitType.CrowdControl, HitType.Initiator,
                    HitType.Danger, HitType.Ultimate
                },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "lucianq",
                ChampionName = "lucian",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1150f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "lucianq",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "lucianw",
                ChampionName = "lucian",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 1050f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "lucianwmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "luciane",
                ChampionName = "lucian",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "lucianr",
                ChampionName = "lucian",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1400f,
                Radius = 110,
                Delay = 500f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "lucianrmissileoffhand",
                ExtraMissileNames = new[] { "lucianrmissile" },
                MissileSpeed = 2800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "luluq",
                ChampionName = "lulu",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 925f,
                Radius = 60,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "luluqmissile",
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "luluw",
                ChampionName = "lulu",
                Slot = SpellSlot.W,
                CastRange = 650f,
                Delay = 640f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "lulue",
                ChampionName = "lulu",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 640f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "lulur",
                ChampionName = "lulu",
                Slot = SpellSlot.R,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "luxlightbinding",
                ChampionName = "lux",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1300f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "luxlightbindingmis",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "luxprismaticwave",
                ChampionName = "lux",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "luxlightstrikekugel",
                ChampionName = "lux",
                Slot = SpellSlot.E,
                CastRange = 1100f,
                Radius = 330f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "luxlightstrikekugel",
                MissileSpeed = 1300
            });

            Spells.Add(new Gamedata
            {
                SDataName = "luxlightstriketoggle",
                ChampionName = "lux",
                Slot = SpellSlot.E,
                CastRange = 1200f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "luxmalicecannon",
                ChampionName = "lux",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 3500f,
                Radius = 299.3f,
                Delay = 1000f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileName = "luxmalicecannonmis",
                MissileSpeed = 3000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kalistamysticshot",
                ChampionName = "kalista",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1200f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "kalistamysticshotmis",
                ExtraMissileNames = new[] { "kalistamysticshotmistrue" },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kalistaw",
                ChampionName = "kalista",
                Slot = SpellSlot.W,
                CastRange = 5000f,
                Delay = 800f,
                HitTypes = new HitType[] { },
                MissileSpeed = 200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "kalistaexpungewrapper",
                ChampionName = "kalista",
                Slot = SpellSlot.E,
                CastRange = 1200f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "seismicshard",
                ChampionName = "malphite",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "obduracy",
                ChampionName = "malphite",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "landslide",
                ChampionName = "malphite",
                Slot = SpellSlot.E,
                CastRange = 400f,
                Delay = 500f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ufslash",
                ChampionName = "malphite",
                Slot = SpellSlot.R,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileName = "ufslash",
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "malzaharq",
                ChampionName = "malzahar",
                Slot = SpellSlot.Q,
                CastRange = 900f,
                Delay = 600f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "alzaharcallofthevoid",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "malzaharw",
                ChampionName = "malzahar",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "malzahare",
                ChampionName = "malzahar",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "malzaharr",
                ChampionName = "malzahar",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "maokaitrunkline",
                ChampionName = "maokai",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "maokaiunstablegrowth",
                ChampionName = "maokai",
                Slot = SpellSlot.W,
                CastRange = 650f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "maokaisapling2",
                ChampionName = "maokai",
                Slot = SpellSlot.E,
                CastRange = 1100f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "maokaidrain3",
                ChampionName = "maokai",
                Slot = SpellSlot.R,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "alphastrike",
                ChampionName = "masteryi",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 600f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "meditate",
                ChampionName = "masteryi",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "wujustyle",
                ChampionName = "masteryi",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 230f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "highlander",
                ChampionName = "masteryi",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 370f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "missfortunericochetshot",
                ChampionName = "missfortune",
                Slot = SpellSlot.Q,
                CastRange = 650f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "missfortuneviciousstrikes",
                ChampionName = "missfortune",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "missfortunescattershot",
                ChampionName = "missfortune",
                Slot = SpellSlot.E,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "missfortunebullettime",
                ChampionName = "missfortune",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1400f,
                Delay = 250f,
                HitTypes = new [] { HitType.Initiator, HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "monkeykingdoubleattack",
                ChampionName = "monkeyking",
                Slot = SpellSlot.Q,
                CastRange = 300f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 20
            });

            Spells.Add(new Gamedata
            {
                SDataName = "monkeykingdecoy",
                ChampionName = "monkeyking",
                Slot = SpellSlot.W,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "monkeykingdecoyswipe",
                ChampionName = "monkeyking",
                Slot = SpellSlot.W,
                CastRange = 300f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                FromObject = new [] { "Base_W_Copy" },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "monkeykingnimbus",
                ChampionName = "monkeyking",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "monkeykingspintowin",
                ChampionName = "monkeyking",
                Slot = SpellSlot.R,
                CastRange = 450f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "monkeykingspintowinleave",
                ChampionName = "monkeyking",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = 700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "mordekaisermaceofspades",
                ChampionName = "mordekaiser",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "mordekaisercreepindeathcast",
                ChampionName = "mordekaiser",
                Slot = SpellSlot.W,
                CastRange = 750f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "mordekaisersyphoneofdestruction",
                ChampionName = "mordekaiser",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "mordekaiserchildrenofthegrave",
                ChampionName = "mordekaiser",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "darkbindingmissile",
                ChampionName = "morgana",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1175f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "darkbindingmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "tormentedsoil",
                ChampionName = "morgana",
                Slot = SpellSlot.W,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "blackshield",
                ChampionName = "morgana",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "soulshackles",
                ChampionName = "morgana",
                Slot = SpellSlot.R,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "namiq",
                ChampionName = "nami",
                Slot = SpellSlot.Q,
                CastRange = 875f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "namiqmissile",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "namiw",
                ChampionName = "nami",
                Slot = SpellSlot.W,
                CastRange = 725f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1100
            });

            Spells.Add(new Gamedata
            {
                SDataName = "namie",
                ChampionName = "nami",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "namir",
                ChampionName = "nami",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 2550f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileName = "namirmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nasusq",
                ChampionName = "nasus",
                Slot = SpellSlot.Q,
                CastRange = 450f,
                Delay = 500f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nasusw",
                ChampionName = "nasus",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nasuse",
                ChampionName = "nasus",
                Slot = SpellSlot.E,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nasusr",
                ChampionName = "nasus",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nautilusanchordrag",
                ChampionName = "nautilus",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1080f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileName = "nautilusanchordragmissile",
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nautiluspiercinggaze",
                ChampionName = "nautilus",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nautilussplashzone",
                ChampionName = "nautilus",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1300
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nautilusgrandline",
                ChampionName = "nautilus",
                Slot = SpellSlot.R,
                CastRange = 1250f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "javelintoss",
                ChampionName = "nidalee",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1500f,
                Radius = 299.3f,
                Delay = 125f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "javelintoss",
                MissileSpeed = 1300
            });

            Spells.Add(new Gamedata
            {
                SDataName = "takedown",
                ChampionName = "nidalee",
                Slot = SpellSlot.Q,
                CastRange = 500f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bushwhack",
                ChampionName = "nidalee",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "pounce",
                ChampionName = "nidalee",
                Slot = SpellSlot.W,
                CastRange = 375f,
                Radius = 210f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Initiator },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "primalsurge",
                ChampionName = "nidalee",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "swipe",
                ChampionName = "nidalee",
                FixedRange = true,
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "aspectofthecougar",
                ChampionName = "nidalee",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nocturneduskbringer",
                ChampionName = "nocturne",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1125f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nocturneshroudofdarkness",
                ChampionName = "nocturne",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nocturneunspeakablehorror",
                ChampionName = "nocturne",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "nocturneparanoia",
                ChampionName = "nocturne",
                Slot = SpellSlot.R,
                CastRange = 20000f,
                Global = true,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "consume",
                ChampionName = "nunu",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bloodboil",
                ChampionName = "nunu",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "iceblast",
                ChampionName = "nunu",
                Slot = SpellSlot.E,
                CastRange = 550f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "absolutezero",
                ChampionName = "nunu",
                Slot = SpellSlot.R,
                CastRange = 650f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "olafaxethrowcast",
                ChampionName = "olaf",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "olafaxethrow",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "olaffrenziedstrikes",
                ChampionName = "olaf",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "olafrecklessstrike",
                ChampionName = "olaf",
                Slot = SpellSlot.E,
                CastRange = 325f,
                Delay = 500f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "olafragnarok",
                ChampionName = "olaf",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "orianaizunacommand",
                ChampionName = "orianna",
                Slot = SpellSlot.Q,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "orianaizuna",
                FromObject = new[] { "yomu_ring" },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "orianadissonancecommand",
                ChampionName = "orianna",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "orianadissonancecommand",
                FromObject = new[] { "yomu_ring" },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "orianaredactcommand",
                ChampionName = "orianna",
                Slot = SpellSlot.E,
                CastRange = 1095f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "orianaredact",
                FromObject = new[] { "yomu_ring" },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "orianadetonatecommand",
                ChampionName = "orianna",
                Slot = SpellSlot.R,
                CastRange = 425f,
                Delay = 500f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },
                MissileName = "orianadetonatecommand",
                FromObject = new[] { "yomu_ring" },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "pantheonq",
                ChampionName = "pantheon",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1900
            });

            Spells.Add(new Gamedata
            {
                SDataName = "pantheonw",
                ChampionName = "pantheon",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "pantheone",
                ChampionName = "pantheon",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "pantheonrjump",
                ChampionName = "pantheon",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 1000f,
                HitTypes = new HitType[] { },
                MissileSpeed = 3000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "pantheonrfall",
                ChampionName = "pantheon",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 1000f,
                HitTypes = new HitType[] { },
                MissileSpeed = 3000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "poppyq",
                ChampionName = "poppy",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 450f,
                Delay = 500f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "poppyw",
                ChampionName = "poppy",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "poppye",
                ChampionName = "poppy",
                Slot = SpellSlot.E,
                CastRange = 525f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "poppyrspell",
                ChampionName = "poppy",
                FixedRange = true,
                Slot = SpellSlot.R,
                CastRange = 1150f,
                Delay = 300f,
                HitTypes = new HitType[] { },
                MissileName = "poppyrspellmissile",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "poppyrspellinstant",
                ChampionName = "poppy",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 450f,
                Delay = 300f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "quinnq",
                ChampionName = "quinn",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1050f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "quinnqmissile",
                ExtraMissileNames = new[] { "quinnq" },
                MissileSpeed = 1550
            });

            Spells.Add(new Gamedata
            {
                SDataName = "quinnw",
                ChampionName = "quinn",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "quinne",
                ChampionName = "quinn",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 775
            });

            Spells.Add(new Gamedata
            {
                SDataName = "quinnr",
                ChampionName = "quinn",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "quinnrfinale",
                ChampionName = "quinn",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "powerball",
                ChampionName = "rammus",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = 775
            });

            Spells.Add(new Gamedata
            {
                SDataName = "defensiveballcurl",
                ChampionName = "rammus",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "puncturingtaunt",
                ChampionName = "rammus",
                Slot = SpellSlot.E,
                CastRange = 325f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "tremors2",
                ChampionName = "rammus",
                Slot = SpellSlot.R,
                CastRange = 300f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "renektoncleave",
                ChampionName = "renekton",
                Slot = SpellSlot.Q,
                CastRange = 225f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "renektonpreexecute",
                ChampionName = "renekton",
                Slot = SpellSlot.W,
                CastRange = 275f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "renektonsliceanddice",
                ChampionName = "renekton",
                Slot = SpellSlot.E,
                CastRange = 450f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "renektonreignofthetyrant",
                ChampionName = "renekton",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rengarq2",
                ChampionName = "rengar",
                Slot = SpellSlot.Q,
                CastRange = 275f,
                Radius = 150f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rengarq2emp",
                ChampionName = "rengar",
                Slot = SpellSlot.Q,
                CastRange = 275f,
                Radius = 150f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rengarw",
                ChampionName = "rengar",
                Slot = SpellSlot.W,
                CastRange = 500f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rengare",
                ChampionName = "rengar",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "rengaremis",
                ExtraMissileNames = new[] { "rengareempmis" },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rengarr",
                ChampionName = "rengar",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "reksaiq",
                ChampionName = "reksai",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 275f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "reksaiqburrowed",
                ChampionName = "reksai",
                Slot = SpellSlot.W,
                CastRange = 1650f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "reksaiqburrowedmis",
                MissileSpeed = 1950
            });

            Spells.Add(new Gamedata
            {
                SDataName = "reksaiw",
                ChampionName = "reksai",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 350f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "reksaiwburrowed",
                ChampionName = "reksai",
                Slot = SpellSlot.W,
                CastRange = 200f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "reksaie",
                ChampionName = "reksai",
                Slot = SpellSlot.E,
                CastRange = 250f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "reksaieburrowed",
                ChampionName = "reksai",
                Slot = SpellSlot.E,
                CastRange = 350f,
                Delay = 900f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "reksair",
                ChampionName = "reksai",
                Slot = SpellSlot.R,
                CastRange = 2.147484E+09f,
                Delay = 1000f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "riventricleave",
                ChampionName = "riven",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 270f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rivenmartyr",
                ChampionName = "riven",
                Slot = SpellSlot.W,
                CastRange = 260f,
                Delay = 100f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rivenfeint",
                ChampionName = "riven",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rivenfengshuiengine",
                ChampionName = "riven",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rivenizunablade",
                ChampionName = "riven",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1075f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileName = "rivenlightsabermissile",
                ExtraMissileNames = new[] { "rivenlightsabermissileside" },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rumbleflamethrower",
                ChampionName = "rumble",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rumbleshield",
                ChampionName = "rumble",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rumbegrenade",
                ChampionName = "rumble",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "rumblecarpetbomb",
                ChampionName = "rumble",
                Slot = SpellSlot.R,
                CastRange = 1700f,
                Delay = 400f,
                HitTypes = new HitType[] { },
                MissileName = "rumblecarpetbombmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ryzeq",
                ChampionName = "ryze",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 925f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "ryzeqmissile",
                ExtraMissileNames = new[] { "ryzeq" },
                MissileSpeed = 1700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ryzew",
                ChampionName = "ryze",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ryzee",
                ChampionName = "ryze",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ryzer",
                ChampionName = "ryze",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sejuaniarcticassault",
                ChampionName = "sejuani",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 650f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl,HitType.Initiator },
                MissileName = "",
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sejuaninorthernwinds",
                ChampionName = "sejuani",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 1000f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sejuaniwintersclaw",
                ChampionName = "sejuani",
                Slot = SpellSlot.E,
                CastRange = 550f,
                Delay = 0f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sejuaniglacialprisoncast",
                ChampionName = "sejuani",
                Slot = SpellSlot.R,
                CastRange = 1200f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileName = "sejuaniglacialprison",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "deceive",
                ChampionName = "shaco",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "jackinthebox",
                ChampionName = "shaco",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "twoshivpoison",
                ChampionName = "shaco",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "hallucinatefull",
                ChampionName = "shaco",
                Slot = SpellSlot.R,
                CastRange = 1125f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 395
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shenq",
                ChampionName = "shen",
                Slot = SpellSlot.Q,
                CastRange = 1650f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                FromObject = new[] { "ShenArrowVfxHostMinion" },
                MissileSpeed = 1350
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shenw",
                ChampionName = "shen",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shene",
                ChampionName = "shen",
                Slot = SpellSlot.E,
                CastRange = 675f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "shene",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shenr",
                ChampionName = "shen",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shyvanadoubleattack",
                ChampionName = "shyvana",
                Slot = SpellSlot.Q,
                CastRange = 275f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shyvanadoubleattackdragon",
                ChampionName = "shyvana",
                Slot = SpellSlot.Q,
                CastRange = 325f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shyvanaimmolationauraqw",
                ChampionName = "shyvana",
                Slot = SpellSlot.W,
                CastRange = 275f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shyvanaimmolateddragon",
                ChampionName = "shyvana",
                Slot = SpellSlot.W,
                CastRange = 250f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shyvanafireball",
                ChampionName = "shyvana",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 925f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "shyvanafireballmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shyvanafireballdragon2",
                ChampionName = "shyvana",
                Slot = SpellSlot.E,
                CastRange = 925f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "shyvanatransformcast",
                ChampionName = "shyvana",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 100f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.CrowdControl,
                        HitType.Ultimate, HitType.Initiator
                    },
                MissileName = "shyvanatransformcast",
                MissileSpeed = 1100
            });

            Spells.Add(new Gamedata
            {
                SDataName = "poisentrail",
                ChampionName = "singed",
                Slot = SpellSlot.Q,
                CastRange = 250f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "megaadhesive",
                ChampionName = "singed",
                Slot = SpellSlot.W,
                CastRange = 1175f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "fling",
                ChampionName = "singed",
                Slot = SpellSlot.E,
                CastRange = 125f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "insanitypotion",
                ChampionName = "singed",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sionq",
                ChampionName = "sion",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sionwdetonate",
                ChampionName = "sion",
                Slot = SpellSlot.W,
                CastRange = 350f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sione",
                ChampionName = "sion",
                Slot = SpellSlot.E,
                CastRange = 725f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "sionemissile",
                MissileSpeed = 1800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sionr",
                ChampionName = "sion",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "",
                MissileSpeed = 500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sivirq",
                ChampionName = "sivir",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1165f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "sivirqmissile",
                ExtraMissileNames = new[] { "sivirqmissilereturn" },
                MissileSpeed = 1350
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sivirw",
                ChampionName = "sivir",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sivire",
                ChampionName = "sivir",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sivirr",
                ChampionName = "sivir",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "skarnervirulentslash",
                ChampionName = "skarner",
                Slot = SpellSlot.Q,
                CastRange = 350f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "skarnerexoskeleton",
                ChampionName = "skarner",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "skarnerfracture",
                ChampionName = "skarner",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1100f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "skarnerfracturemissile",
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "skarnerimpale",
                ChampionName = "skarner",
                Slot = SpellSlot.R,
                CastRange = 350f,
                Delay = 350f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl, HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sonaq",
                ChampionName = "sona",
                Slot = SpellSlot.Q,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sonaw",
                ChampionName = "sona",
                Slot = SpellSlot.W,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sonae",
                ChampionName = "sona",
                Slot = SpellSlot.E,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sonar",
                ChampionName = "sona",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },
                MissileName = "sonar",
                MissileSpeed = 2400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sorakaq",
                ChampionName = "soraka",
                Slot = SpellSlot.Q,
                CastRange = 970f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "",
                MissileSpeed = 1100
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sorakaw",
                ChampionName = "soraka",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sorakae",
                ChampionName = "soraka",
                Slot = SpellSlot.E,
                CastRange = 925f,
                Delay = 1750f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "sorakar",
                ChampionName = "soraka",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "swaindecrepify",
                ChampionName = "swain",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "swainshadowgrasp",
                ChampionName = "swain",
                Slot = SpellSlot.W,
                CastRange = 1040f,
                Delay = 1100f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "swainshadowgrasp",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "swaintorment",
                ChampionName = "swain",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "swainmetamorphism",
                ChampionName = "swain",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1950
            });

            Spells.Add(new Gamedata
            {
                SDataName = "syndraq",
                ChampionName = "syndra",
                Slot = SpellSlot.Q,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "syndraq",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "syndrawcast",
                ChampionName = "syndra",
                Slot = SpellSlot.W,
                CastRange = 950f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "syndrawcast",
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "syndrae",
                ChampionName = "syndra",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 950f,
                Delay = 300f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "syndrae",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "syndrar",
                ChampionName = "syndra",
                Slot = SpellSlot.R,
                CastRange = 675f,
                Delay = 450f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileSpeed = 1250
            });

            Spells.Add(new Gamedata
            {
                SDataName = "tahmkenchq",
                ChampionName = "tahmkench",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 950f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 2800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "taliyahq",
                ChampionName = "taliyah",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Radius = 80f,
                Delay = 250f,
                FixedRange = true,
                MissileName = "taliyahqmis",
                MissileSpeed = 1750,
            });

            Spells.Add(new Gamedata
            {
                SDataName = "taliyahwvc",
                ChampionName = "taliyah",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Radius = 150f,
                Delay = 900f,
                HitTypes = new [] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "taliyahe",
                ChampionName = "taliyah",
                Slot = SpellSlot.E,
                CastRange = 500f,
                Radius = 165f,
                Delay = 250f,
                FixedRange = true,
                HitTypes = new HitType[] { },
                MissileSpeed = 1650,
            });

            Spells.Add(new Gamedata
            {
                SDataName = "talonq",
                ChampionName = "talon",
                Slot = SpellSlot.Q,
                CastRange = 275f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "talonw",
                ChampionName = "talon",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "talonwmissileone",
                ExtraMissileNames = new [] { "talonwmissiletwo" },
                MissileSpeed = 2300
            });

            Spells.Add(new Gamedata
            {
                SDataName = "talone",
                ChampionName = "talon",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "talonr",
                ChampionName = "talon",
                Slot = SpellSlot.R,
                CastRange = 750f,
                Delay = 260f,
                MissileName = "talonrmisone",
                HitTypes = new[] { HitType.Stealth },
                ExtraMissileNames = new [] { "talonrmistwo" },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "taricq",
                ChampionName = "taric",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "taricw",
                ChampionName = "taric",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "tarice",
                ChampionName = "taric",
                Slot = SpellSlot.E,
                CastRange = 625f,
                Delay = 1000f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "taricr",
                ChampionName = "taric",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "blindingdart",
                ChampionName = "teemo",
                Slot = SpellSlot.Q,
                CastRange = 580f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "movequick",
                ChampionName = "teemo",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = 943
            });

            Spells.Add(new Gamedata
            {
                SDataName = "toxicshot",
                ChampionName = "teemo",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bantamtrap",
                ChampionName = "teemo",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "threshq",
                ChampionName = "thresh",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1175f,
                Delay = 500f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileName = "threshqmissile",
                MissileSpeed = 1900
            });

            Spells.Add(new Gamedata
            {
                SDataName = "threshw",
                ChampionName = "thresh",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "threshe",
                ChampionName = "thresh",
                Slot = SpellSlot.E,
                CastRange = 400f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "threshemissile1",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "threshrpenta",
                ChampionName = "thresh",
                Slot = SpellSlot.R,
                CastRange = 420f,
                Delay = 300f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1550
            });

            Spells.Add(new Gamedata
            {
                SDataName = "tristanaq",
                ChampionName = "tristana",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "tristanaw",
                ChampionName = "tristana",
                Slot = SpellSlot.W,
                Radius = 270f,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "tristanae",
                ChampionName = "tristana",
                Slot = SpellSlot.E,
                Radius = 210f,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 2400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "tristanar",
                ChampionName = "tristana",
                Slot = SpellSlot.R,
                Radius = 200f,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "trundleq",
                ChampionName = "trundle",
                Slot = (SpellSlot) 45,
                CastRange = 800f,
                Radius = 210f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "trundletrollsmash",
                ChampionName = "trundle",
                Slot = SpellSlot.Q,
                Radius = 210f,
                CastRange = 300f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "trundledesecrate",
                ChampionName = "trundle",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "trundlecircle",
                ChampionName = "trundle",
                Slot = SpellSlot.E,
                CastRange = 1000f,
                Radius = 340f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "trundlepain",
                ChampionName = "trundle",
                Slot = SpellSlot.R,
                CastRange = 650f,
                Radius = 300f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bloodlust",
                ChampionName = "tryndamere",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "mockingshout",
                ChampionName = "tryndamere",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "slashcast",
                ChampionName = "tryndamere",
                Slot = SpellSlot.E,
                CastRange = 660f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "slashcast",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "undyingrage",
                ChampionName = "tryndamere",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "twitchhideinshadows",
                ChampionName = "twitch",
                Slot = SpellSlot.Q,
                CastRange = 1000f,
                Delay = 450f,
                HitTypes = new[] { HitType.Stealth },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "twitchvenomcask",
                ChampionName = "twitch",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "twitchvenomcaskmissile",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "twitchvenomcaskmissle",
                ChampionName = "twitch",
                Slot = SpellSlot.W,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "twitchexpungewrapper",
                ChampionName = "twitch",
                Slot = SpellSlot.E,
                CastRange = 1200f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "twitchexpunge",
                ChampionName = "twitch",
                Slot = SpellSlot.E,
                CastRange = 1200f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "twitchfullautomatic",
                ChampionName = "twitch",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "wildcards",
                ChampionName = "twistedfate",
                Slot = SpellSlot.Q,
                CastRange = 1450f,
                FixedRange = true,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "sealfatemissile",
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "pickacard",
                ChampionName = "twistedfate",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "goldcardpreattack",
                ChampionName = "twistedfate",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "redcardpreattack",
                ChampionName = "twistedfate",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bluecardpreattack",
                ChampionName = "twistedfate",
                Slot = SpellSlot.W,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "cardmasterstack",
                ChampionName = "twistedfate",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "destiny",
                ChampionName = "twistedfate",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "udyrtigerstance",
                ChampionName = "udyr",
                Slot = SpellSlot.Q,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "udyrturtlestance",
                ChampionName = "udyr",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "udyrbearstanceattack",
                ChampionName = "udyr",
                Slot = SpellSlot.E,
                CastRange = 250f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "udyrphoenixstance",
                ChampionName = "udyr",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "urgotheatseekinglineqqmissile",
                ChampionName = "urgot",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "urgotheatseekingmissile",
                ChampionName = "urgot",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "urgotterrorcapacitoractive2",
                ChampionName = "urgot",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "urgotplasmagrenade",
                ChampionName = "urgot",
                Slot = SpellSlot.E,
                CastRange = 950f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "urgotplasmagrenadeboom",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "urgotplasmagrenadeboom",
                ChampionName = "urgot",
                Slot = SpellSlot.E,
                CastRange = 950f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "urgotswap2",
                ChampionName = "urgot",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1800
            });

            Spells.Add(new Gamedata
            {
                SDataName = "varusq",
                ChampionName = "varus",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1250f,
                Delay = 0f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "varusqmissile",
                MissileSpeed = 1900
            });

            Spells.Add(new Gamedata
            {
                SDataName = "varusw",
                ChampionName = "varus",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "varuse",
                ChampionName = "varus",
                Slot = SpellSlot.E,
                CastRange = 925f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "varuse",
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "varusr",
                ChampionName = "varus",
                Slot = SpellSlot.R,
                FixedRange = true,
                CastRange = 1300f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileName = "varusrmissile",
                MissileSpeed = 1950
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vaynetumble",
                ChampionName = "vayne",
                Slot = SpellSlot.Q,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vaynesilverbolts",
                ChampionName = "vayne",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vaynecondemnmissile",
                ChampionName = "vayne",
                Slot = SpellSlot.E,
                CastRange = 550f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vayneinquisition",
                ChampionName = "vayne",
                Slot = SpellSlot.R,
                CastRange = 1200f,
                Delay = 250f,
                HitTypes = new[] { HitType.Stealth, HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "veigarbalefulstrike",
                ChampionName = "veigar",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 950f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "veigarbalefulstrikemis",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "veigardarkmatter",
                ChampionName = "veigar",
                Slot = SpellSlot.W,
                CastRange = 900f,
                Delay = 1200f,
                HitTypes = new HitType[] { },
                MissileName = "",
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "veigareventhorizon",
                ChampionName = "veigar",
                Slot = SpellSlot.E,
                CastRange = 650f,
                Delay = 0f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "",
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "veigarprimordialburst",
                ChampionName = "veigar",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "velkozq",
                ChampionName = "velkoz",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1250f,
                Delay = 100f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "velkozqmissile",
                MissileSpeed = 1300
            });

            Spells.Add(new Gamedata
            {
                SDataName = "velkozqsplitactivate",
                ChampionName = "velkoz",
                Slot = SpellSlot.Q,
                CastRange = 1050f,
                Delay = 0f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileName = "velkozqmissilesplit",
                MissileSpeed = 2100
            });

            Spells.Add(new Gamedata
            {
                SDataName = "velkozw",
                ChampionName = "velkoz",
                Slot = SpellSlot.W,
                FixedRange = true,
                CastRange = 1050f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileName = "velkozwmissile",
                MissileSpeed = 1700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "velkoze",
                ChampionName = "velkoz",
                Slot = SpellSlot.E,
                CastRange = 950f,
                Delay = 0f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "velkozemissile",
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "velkozr",
                ChampionName = "velkoz",
                Slot = SpellSlot.R,
                CastRange = 1575f,
                Delay = 0f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "viqmissile",
                ChampionName = "vi",
                Slot = SpellSlot.Q,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl, HitType.Initiator },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "viw",
                ChampionName = "vi",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vie",
                ChampionName = "vi",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vir",
                ChampionName = "vi",
                Slot = SpellSlot.R,
                CastRange = 800f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "viktorpowertransfer",
                ChampionName = "viktor",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1050
            });

            Spells.Add(new Gamedata
            {
                SDataName = "viktorgravitonfield",
                ChampionName = "viktor",
                Slot = SpellSlot.W,
                CastRange = 815f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "viktordeathray",
                ChampionName = "viktor",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileName = "viktordeathraymis",
                ExtraMissileNames = new[] { "viktoreaugmissile" },
                MissileSpeed = 1210
            });

            Spells.Add(new Gamedata
            {
                SDataName = "viktorchaosstorm",
                ChampionName = "viktor",
                Slot = SpellSlot.R,
                CastRange = 710f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.CrowdControl, HitType.Ultimate,
                        HitType.Danger, HitType.Initiator
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vladimirq",
                ChampionName = "vladimir",
                Slot = SpellSlot.Q,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vladimirw",
                ChampionName = "vladimir",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vladimire",
                ChampionName = "vladimir",
                Slot = SpellSlot.E,
                CastRange = 610f,
                Delay = 800f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "vladimirr",
                ChampionName = "vladimir",
                Slot = SpellSlot.R,
                CastRange = 875f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "volibearq",
                ChampionName = "volibear",
                Slot = SpellSlot.Q,
                CastRange = 300f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "volibearw",
                ChampionName = "volibear",
                Slot = SpellSlot.W,
                CastRange = 400f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = 1450
            });

            Spells.Add(new Gamedata
            {
                SDataName = "volibeare",
                ChampionName = "volibear",
                Slot = SpellSlot.E,
                CastRange = 425f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 825
            });

            Spells.Add(new Gamedata
            {
                SDataName = "volibearr",
                ChampionName = "volibear",
                Slot = SpellSlot.R,
                CastRange = 425f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = 825
            });

            Spells.Add(new Gamedata
            {
                SDataName = "hungeringstrike",
                ChampionName = "warwick",
                Slot = SpellSlot.Q,
                CastRange = 400f,
                Radius = 210f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "hunterscall",
                ChampionName = "warwick",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "bloodscent",
                ChampionName = "warwick",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 0f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "infiniteduress",
                ChampionName = "warwick",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "infiniteduresschannel",
                ChampionName = "warwick",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xeratharcanopulsechargeup",
                ChampionName = "xerath",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 750f,
                Delay = 750f,
                HitTypes = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xeratharcanebarrage2",
                ChampionName = "xerath",
                Slot = SpellSlot.W,
                CastRange = 1100f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "xeratharcanebarrage2",
                MissileSpeed = 20
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xerathmagespear",
                ChampionName = "xerath",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1050f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger },
                MissileName = "xerathmagespearmissile",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xerathlocusofpower2",
                ChampionName = "xerath",
                Slot = SpellSlot.R,
                CastRange = 5600f,
                Delay = 750f,
                HitTypes = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xenzhaocombotarget",
                ChampionName = "xinzhao",
                Slot = SpellSlot.Q,
                CastRange = 375,
                Radius = 210f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xenzhaothrust",
                ChampionName = "xinzhao",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Radius = 225f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xenzhaothrust2",
                ChampionName = "xinzhao",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Radius = 225f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xenzhaothrust3",
                ChampionName = "xinzhao",
                Slot = SpellSlot.Q,
                CastRange = 625f,
                Radius = 225f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xenzhaobattlecry",
                ChampionName = "xinzhao",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Radius = 210f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xenzhaosweep",
                ChampionName = "xinzhao",
                Slot = SpellSlot.E,
                CastRange = 600f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger, HitType.Initiator },
                MissileSpeed = 2400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "xenzhaoparry",
                ChampionName = "xinzhao",
                Slot = SpellSlot.R,
                CastRange = 500f,
                Radius = 210f,
                Delay = 250f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl
                    },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yasuoqw",
                ChampionName = "yasuo",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 475f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yasuoq2w",
                ChampionName = "yasuo",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 475f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yasuoq3",
                ChampionName = "yasuo",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 1000f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "yasuoq3mis",
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yasuowmovingwall",
                ChampionName = "yasuo",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yasuodashwrapper",
                ChampionName = "yasuo",
                Slot = SpellSlot.E,
                CastRange = 475f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 20
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yasuorknockupcombow",
                ChampionName = "yasuo",
                Slot = SpellSlot.R,
                CastRange = 1200f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yorickq",
                ChampionName = "yorick",
                Slot = SpellSlot.Q,
                CastRange = 350f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yorickw",
                ChampionName = "yorick",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yoricke",
                ChampionName = "yorick",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Radius = 125f,
                Delay = 250f,
                MissileName = "yorickemissile",
                HitTypes = new HitType[] { },
                MissileSpeed = 750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "yorickr",
                ChampionName = "yorick",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zacq",
                ChampionName = "zac",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 550f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "zacq",
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zacw",
                ChampionName = "zac",
                Slot = SpellSlot.W,
                CastRange = 350f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zace",
                ChampionName = "zac",
                Slot = SpellSlot.E,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1500
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zacr",
                ChampionName = "zac",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zedq",
                ChampionName = "zed",
                Slot = SpellSlot.Q,
                FixedRange = true,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "zedqmissile",
                FromObject = new[] { "Zed_Base_W_tar.troy", "Zed_Base_W_cloneswap_buf.troy" },
                ExtraMissileNames = new[] { "zedqmissiletwo", "zedqmissilethree" },
                MissileSpeed = 1700
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zedw",
                ChampionName = "zed",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 1600
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zede",
                ChampionName = "zed",
                Slot = SpellSlot.E,
                CastRange = 300f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zedr",
                ChampionName = "zed",
                Slot = SpellSlot.R,
                CastRange = 850f,
                Delay = 450f,
                HitTypes = new[] { HitType.Danger, HitType.Initiator },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ziggsq",
                ChampionName = "ziggs",
                Slot = SpellSlot.Q,
                CastRange = 850f,
                Radius = 100f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "ziggsqspell",
                ExtraMissileNames = new[] { "ziggsqspell2", "ziggsqspell3" },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ziggsw",
                ChampionName = "ziggs",
                Slot = SpellSlot.W,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "ziggsw",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ziggswtoggle",
                ChampionName = "ziggs",
                Slot = SpellSlot.W,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ziggse",
                ChampionName = "ziggs",
                Slot = SpellSlot.E,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "ziggse",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ziggse2",
                ChampionName = "ziggs",
                Slot = SpellSlot.E,
                CastRange = 850f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "ziggsr",
                ChampionName = "ziggs",
                Slot = SpellSlot.R,
                CastRange = 2250f,
                Delay = 1800f,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                MissileName = "ziggsr",
                MissileSpeed = 1750
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zileanq",
                ChampionName = "zilean",
                Slot = SpellSlot.Q,
                CastRange = 900f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "zileanqmissile",
                MissileSpeed = 2000
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zileanw",
                ChampionName = "zilean",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zileane",
                ChampionName = "zilean",
                Slot = SpellSlot.E,
                CastRange = 700f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zileanr",
                ChampionName = "zilean",
                Slot = SpellSlot.R,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = int.MaxValue
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zyraq",
                ChampionName = "zyra",
                Slot = SpellSlot.Q,
                CastRange = 800f,
                Radius = 430f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "zyraqmissile",
                MissileSpeed = 1400
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zyraqplantmissile",
                ChampionName = "zyra",
                Slot = SpellSlot.Q,
                CastRange = 675f,
                Radius = 710f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileName = "zyraqplantmissile",
                MissileSpeed = 1200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zyraw",
                ChampionName = "zyra",
                Slot = SpellSlot.W,
                CastRange = 0f,
                Delay = 250f,
                HitTypes = new HitType[] { },
                MissileSpeed = 2200
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zyrae",
                ChampionName = "zyra",
                Slot = SpellSlot.E,
                FixedRange = true,
                CastRange = 1150f,
                Radius = 70f,
                Delay = 250f,
                HitTypes = new[] { HitType.CrowdControl },
                MissileName = "zyraemissile",
                MissileSpeed = 1150,
            });

            Spells.Add(new Gamedata
            {
                SDataName = "zyrar",
                ChampionName = "zyra",
                Slot = SpellSlot.R,
                CastRange = 700f,
                Radius = 500f,
                Delay = 500f,
                HitTypes =
                    new[]
                    {
                        HitType.Danger, HitType.Ultimate,
                        HitType.CrowdControl, HitType.Initiator
                    },
                MissileSpeed = int.MaxValue
            });
        }

        public static List<Gamedata> Spells = new List<Gamedata>();
        public static List<Gamedata> CachedSpells = new List<Gamedata>();
        public static Dictionary<SpellDamageDelegate, SpellSlot> DamageLib = new Dictionary<SpellDamageDelegate, SpellSlot>();

        public static Gamedata GetByMissileName(string missilename)
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