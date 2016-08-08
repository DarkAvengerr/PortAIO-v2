using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iSeriesReborn.Champions.KogMaw.Skills;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.KogMaw
{
    class KogHooks
    {
        internal static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            KogW.OnBeforeAttack(args);
        }
    }
}
