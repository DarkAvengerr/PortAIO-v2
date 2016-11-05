using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Lucian.Skills
{
    class Farm
    {
        public static void ExecuteLogic()
        {
            if(Variables.spells[SpellSlot.Q].IsEnabledAndReady())
            {
                var minionsAround = MinionManager.GetMinions(Variables.spells[SpellSlot.Q].Range);
                var farmLocation = Variables.spells[SpellSlot.Q].GetCircularFarmLocation(minionsAround, 60);

                if (farmLocation.MinionsHit >= 3)
                {
                    var adjacentMinions = minionsAround.Where(m => m.Distance(farmLocation.Position) <= 45);
                    if (!adjacentMinions.Any())
                    {
                        return;
                    }

                    var firstMinion = adjacentMinions.OrderBy(m => m.Distance(farmLocation.Position)).First();

                    if (firstMinion.IsValidTarget(Variables.spells[SpellSlot.Q].Range))
                    {
                        if (!LucianHooks.HasPassive && Orbwalking.InAutoAttackRange(firstMinion))
                        {
                            Variables.spells[SpellSlot.Q].Cast(firstMinion);
                        }
                    }
                }
            }
            
        }
    }
}
