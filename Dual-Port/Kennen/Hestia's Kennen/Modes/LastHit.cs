using System.Linq;
using Kennen.Core;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
namespace Kennen.Modes
{
    internal class LastHit
    {
        public static void ExecuteLastHit()
        {
            var castQ = Configs.config.Item("useQlh").GetValue<bool>() && Spells.Q.IsReady();

            if (!Orbwalking.CanMove(40))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            if (minions.Count > 0 && castQ &&
                ObjectManager.Player.ManaPercent >= Configs.config.Item("useQlhMana").GetValue<Slider>().Value)
            {
                if (Configs.config.Item("qRange").GetValue<bool>())
                {
                    foreach (
                        var minion in
                            minions.Where(
                                minion =>
                                    minion.IsValidTarget() && !Orbwalking.InAutoAttackRange(minion) &&
                                    minion.Health < Spells.Q.GetDamage(minion)))
                    {
                        Spells.Q.CastSpell(minion, "predMode", "hitchanceQ");
                    }
                }
                else
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
}
