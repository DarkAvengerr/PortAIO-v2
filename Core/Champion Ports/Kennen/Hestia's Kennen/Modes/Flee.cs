using Kennen.Core;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Kennen.Modes
{
    internal class Flee
    {
        public static void ExecuteFlee()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            var useQ = Configs.config.Item("qFlee").GetValue<bool>() && Spells.Q.IsReady();
            var useW = Configs.config.Item("wFlee").GetValue<bool>() && Spells.W.IsReady();
            var useE = Configs.config.Item("eFlee").GetValue<bool>() && Spells.E.IsReady();

            if (useQ && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.CastSpell(target, "predMode", "hitchanceQ");
            }

            if (useW && Champion.Kennen.CanStun(target) && ObjectManager.Player.Distance(target) <= Spells.W.Range)
            {
                Spells.W.Cast();
            }

            if (useE && !Champion.Kennen.IsRushing())
            {
                Spells.E.Cast();
            }
        }
    }
}
