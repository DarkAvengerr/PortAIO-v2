using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main.Modes
{
    using System.Linq;

    using DarkEzreal.Common;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;

    internal class LastHit
    {
        public static void Execute()
        {
            foreach (var minion in GameObjects.EnemyMinions.Where(m => m.IsValidTarget(SpellsManager.Q.Range) && m.IsKillable(SpellsManager.Q)))
            {
                var qpred = SpellsManager.Q.GetPrediction(minion);
                if (Config.QMenu["Qlh"].GetKeyBind("Q") && SpellsManager.Q.IsReady() && !Config.Player.Spellbook.IsAutoAttacking && SpellsManager.Q.ManaManager(Config.QMenu["Qlc"])
                    && qpred.Hitchance >= SpellsManager.Q.hitchance(Config.QMenu))
                {
                    if (Config.QMenu["Qlh"].GetBool("Qunk"))
                    {
                        if (!minion.IsValidTarget(Config.Player.GetRealAutoAttackRange()))
                        {
                            SpellsManager.Q.Cast(qpred.CastPosition);
                        }

                        return;
                    }

                    SpellsManager.Q.Cast(qpred.CastPosition);
                }
            }
        }

        public static void Auto()
        {
            foreach (var minion in GameObjects.EnemyMinions.Where(m => m.IsValidTarget(SpellsManager.Q.Range) && m.IsKillable(SpellsManager.Q)))
            {
                var qpred = SpellsManager.Q.GetPrediction(minion);
                if (Config.QMenu["Qlh"].GetKeyBind("Q") && Config.QMenu["Qlh"].GetBool("autoQ") && SpellsManager.Q.IsReady() && !Config.Player.Spellbook.IsAutoAttacking
                    && SpellsManager.Q.ManaManager(Config.QMenu["Qlc"]) && qpred.Hitchance >= SpellsManager.Q.hitchance(Config.QMenu))
                {
                    if (Config.QMenu["Qlh"].GetBool("Qunk"))
                    {
                        if (!minion.IsValidTarget(Config.Player.GetRealAutoAttackRange()))
                        {
                            SpellsManager.Q.Cast(qpred.CastPosition);
                        }

                        return;
                    }

                    SpellsManager.Q.Cast(qpred.CastPosition);
                }
            }
        }
    }
}
