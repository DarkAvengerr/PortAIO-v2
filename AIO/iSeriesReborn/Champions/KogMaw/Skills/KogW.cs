using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Entities;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.KogMaw.Skills
{
    class KogW
    {
        internal static void ExecuteLogic()
        {
            if (Variables.spells[SpellSlot.W].IsEnabledAndReady())
            {
                Variables.spells[SpellSlot.W].Cast();
            }
        }

        internal static void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Variables.spells[SpellSlot.W].LSIsReady())
            {
                if (MenuExtensions.GetItemValue<bool>("iseriesr.kogmaw.misc.w.on.nexus") && args.Target is Obj_HQ)
                {
                    Variables.spells[SpellSlot.W].Cast();
                }

                if (MenuExtensions.GetItemValue<bool>("iseriesr.kogmaw.misc.w.on.inhib") && args.Target is Obj_BarracksDampener)
                {
                    Variables.spells[SpellSlot.W].Cast();
                }

                if (MenuExtensions.GetItemValue<bool>("iseriesr.kogmaw.misc.w.on.nexus") && args.Target is Obj_HQ)
                {
                    Variables.spells[SpellSlot.W].Cast();
                }
            }
            
        }

        public static void ExecuteLaneclear()
        {
            if (Variables.spells[SpellSlot.W].IsEnabledAndReady())
            {
                var minionsInRange = GameObjects.EnemyMinions.Where(m => m.LSIsValidTarget(KogUtils.GetWRange()));
                if (minionsInRange.Count() > 6)
                {
                    Variables.spells[SpellSlot.W].Cast();
                }
            }
        }
    }
}
