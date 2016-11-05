using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Vayne.Utility
{
    class VayneUtility
    {
        public static bool IsStealthed()
        {
            return ObjectManager.Player.Buffs.Any(m => m.Name.ToLower() == "vaynetumblefade");
        }

        public static bool Has2WStacks(AIHeroClient target)
        {
            return target.Buffs.Any(bu => bu.Name == "vaynesilvereddebuff" && bu.Count == 2);
        }
    }
}
