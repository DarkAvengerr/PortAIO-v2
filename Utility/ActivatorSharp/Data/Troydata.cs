#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Data/Troydata.cs
// Date:		28/07/2016
// Author:		Robin Kurisu
#endregion

using LeagueSharp;
using Activator.Base;
using System.Collections.Generic;

using EloBuddy; namespace Activator.Data
{
    public class Troydata
    {
        public string Name { get; set; }
        public string ChampionName { get; set; }
        public SpellSlot Slot { get; set; }
        public float Radius { get; set; }
        public double Interval { get; set; }
        public bool PredictDmg { get; set; }
        public HitType[] HitTypes { get; set; }
        public int DelayFromStart { get; set; }

        public static List<Troydata> Troys = new List<Troydata>(); 

        static Troydata()
        {
            Troys.Add(new Troydata
            {
                Name = "MonkeyKing_Base_R",
                ChampionName = "MonkeyKing",
                Radius = 165 + 100 + 0 + 1,
                Slot = SpellSlot.R,
                HitTypes = new [] { HitType.Danger, HitType.Ultimate, HitType.Initiator },
                PredictDmg = true,
                Interval = 0.5
            });

            Troys.Add(new Troydata
            {
                Name = "R_Cas",
                ChampionName = "Nunu",
                Radius = 650f,
                Slot = SpellSlot.R,
                HitTypes = new[] { HitType.CrowdControl },
                PredictDmg = true,
                Interval = 0.75
            });

            Troys.Add(new Troydata
            {
                Name = "Ryze_Base_E",
                ChampionName = "Ryze",
                Radius = 200f,
                Slot = SpellSlot.E,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = .75
            });

            Troys.Add(new Troydata
            {
                Name = "Hecarim_Defile",
                ChampionName = "Hecarim",
                Radius = 425f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = .75
            });

            Troys.Add(new Troydata
            {
                Name = "W_AoE",
                ChampionName = "Hecarim",
                Radius = 425f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = .75
            });

            Troys.Add(new Troydata
            {
                Name = "Gangplank_Base_R",
                ChampionName = "Gangplank",
                Radius = 400f,
                Slot = SpellSlot.R,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.5
            });

            Troys.Add(new Troydata
            {
                Name = "W_Shield",
                ChampionName = "Diana",
                Radius = 225f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "W_Shield",
                ChampionName = "Sion",
                Radius = 225f,
                Slot = SpellSlot.W,
                DelayFromStart = 2800,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "W_aoe_red",
                ChampionName = "Malzahar",
                Radius = 325f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "E_Defile",
                ChampionName = "Karthus",
                Radius = 425f,
                Slot = SpellSlot.E,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "W_volatile",
                ChampionName = "Elise",
                Radius =  250f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 0.3
            });

            Troys.Add(new Troydata
            {
                Name = "DarkWind_tar",
                ChampionName = "FiddleSticks",
                Radius = 250f,
                Slot = SpellSlot.E,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 0.8
            });

            Troys.Add(new Troydata
            {
                Name = "lr_buf",
                ChampionName = "Kennen",
                Radius = 250f,
                Slot = SpellSlot.E,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 0.8
            });

            Troys.Add(new Troydata
            {
                Name = "ss_aoe",
                ChampionName = "Kennen",
                Radius = 475f,
                Slot = SpellSlot.R,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                PredictDmg = true,
                Interval = 0.5
            });

            Troys.Add(new Troydata
            {
                Name = "Ahri_Base_FoxFire",
                ChampionName = "Ahri",
                Radius = 550f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "AurelionSol_Base_P",
                ChampionName = "AurelionSol",
                Radius = 165f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "Fizz_Ring_Red",
                ChampionName = "Fizz",
                Radius = 300f,
                Slot = SpellSlot.R,
                DelayFromStart = 800,
                HitTypes = new[] { HitType.Danger, HitType.Ultimate },
                PredictDmg = true,
                Interval = 1.0
             });

            Troys.Add(new Troydata
            {
                Name = "katarina_deathLotus_tar",
                ChampionName = "Katarina",
                Radius = 550f,
                Slot = SpellSlot.R,
                HitTypes = new[] { HitType.ForceExhaust, HitType.Danger },
                PredictDmg = true,
                Interval = 0.5
            });

            Troys.Add(new Troydata
            {
                Name = "Nautilus_R_sequence_impact",
                ChampionName = "Nautilus",
                Radius = 250f,
                Slot = SpellSlot.R,
                HitTypes = new[] { HitType.CrowdControl, HitType.Danger, HitType.Ultimate },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "Acidtrail_buf",
                ChampionName = "Singed",
                Radius = 200f,
                Slot = SpellSlot.Q,
                HitTypes = new []{ HitType.None },
                PredictDmg = true,
                Interval = 0.5
            });

            Troys.Add(new Troydata
            {
                Name = "Tremors_cas",
                ChampionName = "Rammus",
                Radius = 450f,
                Slot = SpellSlot.R,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "Crowstorm",
                ChampionName = "FiddleSticks",
                Radius = 425f,
                Slot = SpellSlot.R,
                HitTypes =  new[] { HitType.Danger, HitType.Ultimate, HitType.ForceExhaust },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "yordleTrap_idle",
                ChampionName = "Caitlyn",
                Radius = 265f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.CrowdControl },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "tar_aoe_red",
                ChampionName = "Lux",
                Radius = 400f,
                Slot = SpellSlot.E,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 2.0
            });

            Troys.Add(new Troydata
            {
                Name = "Viktor_ChaosStorm",
                ChampionName = "Viktor",
                Radius = 425f,
                Slot = SpellSlot.R,
                HitTypes = new[] { HitType.Danger, HitType.CrowdControl },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "Viktor_Catalyst",
                ChampionName = "Viktor",
                Radius = 375f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.CrowdControl },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "W_AUG",
                ChampionName = "Viktor",
                Radius = 375f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.CrowdControl },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "cryo_storm",
                ChampionName = "Anivia",
                Radius = 400f,
                Slot = SpellSlot.R,
                HitTypes = new[] { HitType.CrowdControl },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "ZiggsE",
                ChampionName = "Ziggs",
                Radius = 325f,
                Slot = SpellSlot.E,
                HitTypes = new []{ HitType.CrowdControl },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "ZiggsWRing",
                ChampionName = "Ziggs",
                Radius = 325f,
                Slot = SpellSlot.W,
                HitTypes = new []{ HitType.CrowdControl },
                PredictDmg = false,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "W_Miasma_tar",
                ChampionName = "Cassiopeia",
                Radius = 365f,
                Slot = SpellSlot.W,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "E_rune",
                ChampionName = "Soraka",
                Radius = 375f,
                Slot = SpellSlot.E,
                HitTypes = new[] { HitType.None },
                PredictDmg = true,
                Interval = 1.0
            });

            Troys.Add(new Troydata
            {
                Name = "W_Tar",
                ChampionName = "Morgana",
                Radius = 275f,
                Slot = SpellSlot.W,
                HitTypes = new []{ HitType.None },
                PredictDmg = true,
                Interval = .75
            });
        }
    }
}
