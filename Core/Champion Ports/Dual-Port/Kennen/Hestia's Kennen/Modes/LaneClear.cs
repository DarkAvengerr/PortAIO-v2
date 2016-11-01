using Kennen.Core;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kennen.Modes
{
    internal class LaneClear
    {
        public static void ExecuteLaneClear()
        {
            var castQ = Configs.config.Item("useQlc").GetValue<bool>() && Spells.Q.IsReady();

            if (!Orbwalking.CanMove(50))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range);

            if (minions.Count > 0 && castQ && ObjectManager.Player.ManaPercent >= Configs.config.Item("useQlcMana").GetValue<Slider>().Value)
            {
                var minion = minions[0];
                Spells.Q.CastSpell(minion, "predMode", "hitchanceQ");
            }
        }
    }
}
