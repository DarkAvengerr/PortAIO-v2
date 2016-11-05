using System.Linq;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.Positioning;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.KogMaw.Skills
{
    class KogE
    {
        public static void ExecuteLogic()
        {
            if (Variables.spells[SpellSlot.E].IsEnabledAndReady())
            {
                var firstTarget = TargetSelector.GetTarget(Variables.spells[SpellSlot.E].Range * 0.70f, TargetSelector.DamageType.Magical);

                if (firstTarget.IsValidTarget())
                {
                    var polygon = new iSRPolygon(iSRPolygon.Rectangle(
                        ObjectManager.Player.ServerPosition.To2D(), firstTarget.ServerPosition.To2D(), 100f));

                    if (HeroManager.Enemies.Count(m => polygon.Contains(m.ServerPosition.To2D())) >= 2)
                    {
                        Variables.spells[SpellSlot.E].Cast(firstTarget.ServerPosition);
                    }
                }
            }
        }
    }
}
