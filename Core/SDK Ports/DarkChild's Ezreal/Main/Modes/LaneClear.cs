using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main.Modes
{
    using System.Linq;

    using DarkEzreal.Common;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;

    internal class LaneClear
    {
        public static void Execute()
        {
            foreach (var minion in GameObjects.EnemyMinions.Where(m => m.IsValidTarget(SpellsManager.Q.Range)))
            {
                var qpred = SpellsManager.Q.GetPrediction(minion);
                if (Config.QMenu["Qlc"].GetKeyBind("Q") && SpellsManager.Q.IsReady() && !Config.Player.Spellbook.IsAutoAttacking && SpellsManager.Q.ManaManager(Config.QMenu["Qlc"])
                    && qpred.Hitchance >= SpellsManager.Q.hitchance(Config.QMenu))
                {
                    if (Config.QMenu["Qlc"].GetBool("Qunk"))
                    {
                        if (!minion.IsValidTarget(Config.Player.GetRealAutoAttackRange()) && minion.IsKillable(SpellsManager.Q))
                        {
                            SpellsManager.Q.Cast(qpred.CastPosition);
                        }

                        return;
                    }

                    if (Config.QMenu["Qlc"].GetBool("lhQ"))
                    {
                        if (minion.IsKillable(SpellsManager.Q))
                        {
                            SpellsManager.Q.Cast(qpred.CastPosition);
                        }

                        return;
                    }

                    if (minion.WontDie(SpellsManager.Q))
                    {
                        SpellsManager.Q.Cast(qpred.CastPosition);
                    }
                }
            }
        }

        public static void Auto()
        {
            foreach (var minion in GameObjects.EnemyMinions.Where(m => m.IsValidTarget(SpellsManager.Q.Range)))
            {
                var qpred = SpellsManager.Q.GetPrediction(minion);
                if (Config.QMenu["Qlc"].GetKeyBind("Q") && Config.QMenu["Qlc"].GetBool("autoQ") && SpellsManager.Q.IsReady() && !Config.Player.Spellbook.IsAutoAttacking
                    && SpellsManager.Q.ManaManager(Config.QMenu["Qlc"]) && qpred.Hitchance >= SpellsManager.Q.hitchance(Config.QMenu))
                {
                    if (Config.QMenu["Qlc"].GetBool("Qunk"))
                    {
                        if (!minion.IsValidTarget(Config.Player.GetRealAutoAttackRange()) && minion.IsKillable(SpellsManager.Q))
                        {
                            SpellsManager.Q.Cast(qpred.CastPosition);
                        }

                        return;
                    }

                    if (Config.QMenu["Qlc"].GetBool("lhQ"))
                    {
                        if (minion.IsKillable(SpellsManager.Q))
                        {
                            SpellsManager.Q.Cast(qpred.CastPosition);
                        }

                        return;
                    }

                    if (minion.WontDie(SpellsManager.Q))
                    {
                        SpellsManager.Q.Cast(qpred.CastPosition);
                    }
                }
            }
        }
    }
}
