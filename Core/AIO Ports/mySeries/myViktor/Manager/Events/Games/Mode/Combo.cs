using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Linq;

    internal class Combo : Logic
    {
        internal static void Init()
        {
            var target = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Magical);

            if (target.Check(1300))
            {
                if (Menu.GetBool("ComboIgnite") && Ignite != SpellSlot.Unknown && Ignite.IsReady() &&
                    target.IsValidTarget(600) &&
                    (target.Health < DamageCalculate.GetComboDamage(target) ||
                     target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }

                if (Menu.GetBool("ComboE") && E.IsReady())
                {
                    SpellManager.CastE(target);
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(target, true);
                }

                if (Menu.GetBool("ComboQGap") && Q.IsReady() && Me.Level >= Menu.GetSlider("ComboQGapLevel") &&
                    Me.Buffs.Any(
                        x =>
                            x.Name == "viktorqaug" || x.Name == "viktorqwaug" || x.Name == "viktorqweaug" ||
                            x.Name == "viktorqeauh") &&
                    target.DistanceToPlayer() > Q.Range + 80 && target.DistanceToPlayer() <= Q.Range + 300)
                {
                    var qMinion =
                        MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .FirstOrDefault(x => Q.CanCast(x));

                    if (qMinion != null && qMinion.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(qMinion, true);
                    }
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range + W.Width))
                {
                    if (Menu.GetBool("ComboWLogic") && Me.CountEnemiesInRange(W.Range + W.Width) <= 2)
                    {
                        var wPred = W.GetPrediction(target);

                        if (wPred.CastPosition.Distance(target.Position) > 100 && wPred.Hitchance >= HitChance.VeryHigh)
                        {
                            if (Me.Position.Distance(target.ServerPosition) > Me.Position.Distance(target.Position))
                            {
                                if (target.Position.Distance(Me.ServerPosition) < target.Position.Distance(Me.Position))
                                {
                                    if (wPred.Hitchance >= HitChance.VeryHigh)
                                    {
                                        W.Cast(W.GetPrediction(target).CastPosition, true);
                                    }
                                }
                            }
                            else
                            {
                                if (target.Position.Distance(Me.ServerPosition) > target.Position.Distance(Me.Position))
                                {
                                    if (wPred.Hitchance >= HitChance.VeryHigh)
                                    {
                                        W.Cast(W.GetPrediction(target).CastPosition, true);
                                    }
                                }
                            }
                        }
                    }

                    if (Menu.GetBool("ComboWTeam") && Me.CountEnemiesInRange(W.Range + W.Width) >= Menu.GetSlider("ComboWTeamHit"))
                    {
                        W.CastIfWillHit(target, Menu.GetSlider("ComboWTeamHit"), true);
                    }
                }

                if (Menu.GetBool("ComboR") && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu.GetBool("ComboRalways"))
                    {
                        var rPred = R.GetPrediction(target, true);

                        if (rPred.Hitchance >= HitChance.VeryHigh)
                        {
                            R.Cast(rPred.CastPosition, true);
                        }
                    }

                    if (Menu.GetBool("ComboRKill") &&
                        ((target.Health < DamageCalculate.GetComboDamage(target) && target.IsValidTarget(R.Range - 200)) ||
                         target.Health < R.GetDamage(target)))
                    {
                        var rPred = R.GetPrediction(target, true);

                        if (rPred.Hitchance >= HitChance.VeryHigh)
                        {
                            R.Cast(rPred.CastPosition, true);
                        }
                    }

                    if (Menu.GetBool("ComboRTeam") && Me.CountEnemiesInRange(R.Range + R.Width) >= Menu.GetSlider("ComboRTeamHit"))
                    {
                        R.CastIfWillHit(target, Menu.GetSlider("ComboRTeamHit"), true);
                    }
                }
            }
        }
    }
}
