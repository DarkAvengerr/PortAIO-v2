using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Combo : Logic
    {
        internal static void Init()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target.Check(Q.Range))
            {
                if (Menu.GetBool("ComboIgnite") && Ignite != SpellSlot.Unknown && Ignite.IsReady() &&
                    target.IsValidTarget(600) &&
                    (target.Health < DamageCalculate.GetComboDamage(target) ||
                     target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() && target.IsValidTarget(R.Range) &&
                    !SpellManager.HaveBear)
                {
                    var rPred = R.GetPrediction(target, true);

                    if (rPred.Hitchance >= HitChance.VeryHigh)
                    {
                        if (rPred.AoeTargetsHitCount >= Menu.GetSlider("ComboRCount") && SpellManager.HaveStun)
                        {
                            R.Cast(rPred.CastPosition, true);
                        }

                        if (target.Health < DamageCalculate.GetComboDamage(target))
                        {
                            if (SpellManager.BuffCounts == 3 && Menu.GetBool("ComboE") && E.IsReady())
                            {
                                E.Cast();
                            }

                            R.Cast(rPred.CastPosition, true);
                        }
                    }
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    ((R.IsReady() && SpellManager.HaveBear) || !R.IsReady() ||
                     (target.Health > DamageCalculate.GetComboDamage(target) && SpellManager.HaveStun)))
                {
                    Q.CastOnUnit(target, true);
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range - 50) &&
                    ((R.IsReady() && SpellManager.HaveBear) || !R.IsReady() ||
                     (target.Health > DamageCalculate.GetComboDamage(target) && SpellManager.HaveStun)))
                {
                    W.Cast(target.Position, true);
                }
            }
        }
    }
}
