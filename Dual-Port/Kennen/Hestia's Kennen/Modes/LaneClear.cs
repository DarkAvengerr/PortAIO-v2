using System.Linq;
using Kennen.Core;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
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
                foreach (
                    var minion in
                        minions.Where(
                            minion =>
                                minion.IsValidTarget() &&
                                minion.Health < Spells.Q.GetDamage(minion)))
                {
                    Spells.Q.CastSpell(minion, "predMode", "hitchanceQ");
                }
            }
        }
    }
}
