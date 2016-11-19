using System.Linq;
using Kennen.Core;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Kennen.Modes
{
    internal class LastHit
    {
        public static void ExecuteLastHit()
        {
            var castQ = Configs.config.Item("useQlh").GetValue<bool>() && Spells.Q.IsReady();
            var castW = Configs.config.Item("useWlh").GetValue<bool>() && Spells.Q.IsReady();

            if (!Orbwalking.CanMove(50))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            if (minions.Count > 0 && castQ &&
                ObjectManager.Player.ManaPercent >= Configs.config.Item("useQlhMana").GetValue<Slider>().Value)
            {
                //Last hits minions when AA not available
                foreach (
                    var minion in
                    minions.Where(
                        minion =>
                            minion.IsValidTarget() && !Orbwalking.CanAttack() &&
                            minion.Health < Spells.Q.GetDamage(minion)))
                {
                    Spells.Q.CastSpell(minion, "predMode", "hitchanceQ");
                }
            }
            else if (minions.Count > 0 && castW && !Orbwalking.CanAttack() )
            {
                foreach (
                    var minion in
                    minions.Where(
                        minion =>
                            minion.IsValidTarget() && !Orbwalking.CanAttack() && !Spells.Q.IsReady() &&
                            Champion.Kennen.HasMark(minion) && minion.Health < Spells.W.GetDamage(minion) && ObjectManager.Player.Distance(minion) < Spells.W.Range))
                {
                    Spells.W.Cast(minion);
                }
            }
        }
    }
}
