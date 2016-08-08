using System.Linq;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Twitch.Skills
{
    class TwitchW
    {
        public static void OnExecute()
        {
            if (Variables.spells[SpellSlot.W].IsEnabledAndReady())
            {
                var wTarget = TargetSelector.GetTarget(
                    Variables.spells[SpellSlot.W].Range * 0.75f, TargetSelector.DamageType.Physical);
                if (wTarget.LSIsValidTarget())
                {
                    var prediction = Variables.spells[SpellSlot.W].GetPrediction(wTarget);
                    if (prediction.Hitchance >= HitChance.VeryHigh &&
                        (prediction.AoeTargetsHit.Count() >= 2 || ObjectManager.Player.LSCountEnemiesInRange(1500f) == 1))
                    {
                        Variables.spells[SpellSlot.W].Cast(prediction.CastPosition);
                    }
                }
            }
        }
    }
}
