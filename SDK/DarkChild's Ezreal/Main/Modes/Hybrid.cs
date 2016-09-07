using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main.Modes
{
    using DarkEzreal.Common;

    using LeagueSharp.SDK;

    internal class Hybrid
    {
        public static void Execute()
        {
            var target = Variables.TargetSelector.GetTarget(SpellsManager.Q);
            if (target == null)
            {
                return;
            }

            var qpred = SpellsManager.Q.GetPrediction(target);
            if (Config.QMenu["Qh"].GetKeyBind("Q") && qpred.Hitchance >= SpellsManager.Q.hitchance(Config.QMenu) && !Config.Player.Spellbook.IsAutoAttacking
                && SpellsManager.Q.ManaManager(Config.QMenu["Qh"]))
            {
                SpellsManager.Q.Cast(qpred.CastPosition);
            }

            var wpred = SpellsManager.W.GetPrediction(target);
            if (Config.WMenu["Wh"].GetKeyBind("W") && wpred.Hitchance >= SpellsManager.W.hitchance(Config.WMenu) && !Config.Player.Spellbook.IsAutoAttacking
                && SpellsManager.W.ManaManager(Config.WMenu["Wh"]))
            {
                SpellsManager.W.Cast(wpred.CastPosition);
            }
        }

        public static void Auto()
        {
            var target = Variables.TargetSelector.GetTarget(SpellsManager.Q);
            if (target == null)
            {
                return;
            }

            var qpred = SpellsManager.Q.GetPrediction(target);
            if (Config.QMenu["Qh"].GetKeyBind("Q") && qpred.Hitchance >= SpellsManager.Q.hitchance(Config.QMenu) && Config.QMenu["Qh"].GetBool("autoQ") && !Config.Player.Spellbook.IsAutoAttacking
                && SpellsManager.Q.ManaManager(Config.QMenu["Qh"]))
            {
                SpellsManager.Q.Cast(qpred.CastPosition);
            }

            var wpred = SpellsManager.W.GetPrediction(target);
            if (Config.WMenu["Wh"].GetKeyBind("W") && wpred.Hitchance >= SpellsManager.W.hitchance(Config.WMenu) && Config.WMenu["Wh"].GetBool("autoW") && !Config.Player.Spellbook.IsAutoAttacking
                && SpellsManager.W.ManaManager(Config.WMenu["Wh"]))
            {
                SpellsManager.W.Cast(wpred.CastPosition);
            }
        }
    }
}
