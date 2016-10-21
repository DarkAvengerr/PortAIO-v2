using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main.Modes
{
    using System.Linq;

    using DarkEzreal.Common;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;

    internal class Combo
    {
        public static void Execute()
        {
            var target = Variables.TargetSelector.GetTarget(SpellsManager.Q);
            if (target != null)
            {
                var qpred = SpellsManager.Q.GetPrediction(target);
                var wpred = SpellsManager.W.GetPrediction(target);

                if (SpellsManager.Q.IsReady() && target.WontDie(SpellsManager.Q) && Config.QMenu["Qc"].GetBool("Q") && qpred.Hitchance >= SpellsManager.Q.hitchance(Config.QMenu)
                    && SpellsManager.Q.ManaManager(Config.QMenu["Qc"]))
                {
                    if (!Config.QMenu["Qc"].GetBool("AQ"))
                    {
                        SpellsManager.Q.Cast(qpred.CastPosition);
                    }
                    else
                    {
                        if (!Config.Player.Spellbook.IsAutoAttacking)
                        {
                            SpellsManager.Q.Cast(qpred.CastPosition);
                        }
                    }
                }

                if (SpellsManager.W.IsReady() && target.WontDie(SpellsManager.W) && Config.WMenu["Wc"].GetBool("W") && wpred.Hitchance >= SpellsManager.W.hitchance(Config.WMenu)
                    && SpellsManager.W.ManaManager(Config.WMenu["Wc"]))
                {
                    if (!Config.Player.Spellbook.IsAutoAttacking)
                    {
                        SpellsManager.W.Cast(wpred.CastPosition);
                    }
                }

                var melee = GameObjects.EnemyHeroes.FirstOrDefault(e => 250 > e.DistanceToPlayer() && e.IsMelee && e.IsValidTarget());
                if (melee != null && SpellsManager.E.IsReady() && Config.EMenu["Ec"].GetBool("kiteE")
                    && Config.Player.ServerPosition.Extend(melee.ServerPosition, SpellsManager.E.Range).SafetyManager(Config.EMenu["Ec"]))
                {
                    Chat.Print("EKITE");
                    SpellsManager.E.Cast(Config.Player.ServerPosition.Extend(melee.ServerPosition, -SpellsManager.E.Range));
                }
            }
            else
            {
                var range = SpellsManager.E.Range + Config.Player.GetRealAutoAttackRange() - 50;
                var etarget = Variables.TargetSelector.GetTarget(range);
                if (etarget != null && SpellsManager.E.IsReady() && etarget.ServerPosition.Extend(Config.Player.ServerPosition, range).SafetyManager(Config.EMenu["Ec"])
                    && Config.EMenu["Ec"].GetBool("gapE") && etarget.IsValidTarget(range) && !etarget.IsValidTarget(SpellsManager.Q.Range))
                {
                    Chat.Print("gapE");
                    SpellsManager.E.Cast(etarget.ServerPosition.Extend(Config.Player.ServerPosition, range));
                }
            }

            var Rtarget = Variables.TargetSelector.GetTarget(SpellsManager.R);
            if (Rtarget == null)
            {
                return;
            }

            var Rpred = SpellsManager.R.GetPrediction(Rtarget);

            if (SpellsManager.R.IsReady() && Rtarget.WontDie(SpellsManager.R) && Rtarget.IsValidTarget(SpellsManager.R.Range) && !Rtarget.IsValidTarget(Config.MiscMenu.GetSlider("Rmin"))
                && Config.RMenu["Rc"].GetBool("R"))
            {
                if (Config.RMenu["Rc"].GetBool("Rcc") && Rtarget.IsCC())
                {
                    SpellsManager.R.Cast(Rtarget.ServerPosition);
                }

                if (Rpred.Hitchance >= SpellsManager.R.hitchance(Config.RMenu))
                {
                    if (Config.RMenu["Rc"]["Raoe"].GetSliderBool("Raoe"))
                    {
                        if (Config.RMenu["Rc"]["Raoe"].GetBool("target"))
                        {
                            SpellsManager.R.CastIfWillHit(Rtarget, Config.RMenu["Rc"]["Raoe"].GetSliderButton("Raoe"));
                        }
                        else
                        {
                            foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(SpellsManager.R.Range)))
                            {
                                if (enemy != null)
                                {
                                    SpellsManager.R.CastIfWillHit(enemy, Config.RMenu["Rc"]["Raoe"].GetSliderButton("Raoe"));
                                }
                            }
                        }
                    }

                    if (Config.RMenu["Rc"].GetBool("Rfinisher") && Config.Player.GetSpellDamage(Rtarget, SpellSlot.R) >= Rtarget.Health)
                    {
                        SpellsManager.R.Cast(Rpred.CastPosition);
                    }
                }
            }
        }
    }
}
