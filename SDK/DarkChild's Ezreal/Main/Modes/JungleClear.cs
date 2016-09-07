using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main.Modes
{
    using System.Linq;

    using DarkEzreal.Common;

    using LeagueSharp.SDK;

    internal class JungleClear
    {
        public static void Execute()
        {
            if (Config.QMenu["Qjc"].GetBool("Qprio"))
            {
                foreach (var minion in GameObjects.Jungle.OrderByDescending(m => m.MaxHealth).Where(m => m.IsValidTarget(SpellsManager.Q.Range)))
                {
                    var qpred = SpellsManager.Q.GetPrediction(minion);
                    if (Config.QMenu["Qjc"].GetBool("Q") && SpellsManager.Q.IsReady() && !Config.Player.Spellbook.IsAutoAttacking && SpellsManager.Q.ManaManager(Config.QMenu["Qjc"])
                        && qpred.Hitchance >= SpellsManager.Q.hitchance(Config.QMenu))
                    {
                        if (minion.WontDie(SpellsManager.Q))
                        {
                            SpellsManager.Q.Cast(qpred.CastPosition);
                        }
                    }
                }
            }
            else
            {
                foreach (var minion in GameObjects.Jungle.Where(m => m.IsValidTarget(SpellsManager.Q.Range)))
                {
                    var qpred = SpellsManager.Q.GetPrediction(minion);
                    if (Config.QMenu["Qjc"].GetBool("Q") && SpellsManager.Q.IsReady() && !Config.Player.Spellbook.IsAutoAttacking && SpellsManager.Q.ManaManager(Config.QMenu["Qjc"])
                        && qpred.Hitchance >= SpellsManager.Q.hitchance(Config.QMenu))
                    {
                        if (minion.WontDie(SpellsManager.Q))
                        {
                            SpellsManager.Q.Cast(qpred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}
