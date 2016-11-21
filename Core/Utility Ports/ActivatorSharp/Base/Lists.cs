#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Base/Lists.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using Activator.Items;
using Activator.Spells;
using Activator.Summoners;
using System.Collections.Generic;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Base
{
    public class Lists
    {
        internal static List<HitType> MenuTypes = new List<HitType>
        {
            HitType.Danger,
            HitType.CrowdControl,
            HitType.Ultimate,
            HitType.ForceExhaust
        };

        internal static List<string> YoumuuBuffs = new List<string>
        {
            "aatroxr",
            "evelynnrshield",
            "ireliatranscendentbladesspell",
            "khazixr",
            "khazixrstealth",
            "lucianr",
            "highlander",
            "missfortunebulletsound",
            "rengarr",
            "rivenfengshuiengine",
            "sivirr",
            "vayneinquistion"

            // todo buffs:
            // talon r
            // zed r
            // vi r
            // twitch r
            // nassus r
            // nocturne r
            // kled r
        };

        public static List<CoreItem> Items = new List<CoreItem>();
        public static List<CoreItem> BoughtItems = new List<CoreItem>();
        public static List<CoreSpell> Spells = new List<CoreSpell>();
        public static List<CoreSum> Summoners = new List<CoreSum>();
        public static Dictionary<string, Priority> Priorities = new Dictionary<string, Priority>();
    }

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
        ForceExhaust,
        Initiator,
        Troy,
        Item,
        Buff
    }

    public enum LogType
    {
        Info,
        Warning,
        Action,
        Error
    }

    public enum MapType
    {        
        Unknown = -1,
        Common,
        SummonersRift,
        CrystalScar,
        TwistedTreeline,
        HowlingAbyss
    }

    public enum MenuType
    {
        Zhonyas,
        Stealth,
        Cleanse,
        Gapcloser,
        SlowRemoval,
        ActiveCheck,
        SelfCount,
        SelfMuchHP,
        SelfLowHP,
        SelfLowMP,
        SelfMinMP,
        SelfMinHP,
        EnemyLowHP
    }

    public enum PrimaryRole
    {
        Unknown,
        Assassin,
        Fighter,
        Mage,
        Support,
        Marksman,
        Tank
    }
}
