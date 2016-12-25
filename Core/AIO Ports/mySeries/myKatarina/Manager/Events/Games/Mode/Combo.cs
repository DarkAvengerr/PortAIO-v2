using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events.Games.Mode
{
    using Spells;
    using System.Linq;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Combo : Logic
    {
        internal static void Init()
        {
            if (SpellManager.isCastingUlt)
            {
                return;
            }

            var target = TargetSelector.GetTarget(E.Range + 300f, TargetSelector.DamageType.Magical);

            if (target.Check(E.Range + 300f))
            {
                if (Menu.GetBool("ComboDot") && Ignite != SpellSlot.Unknown && Ignite.IsReady() &&
                    target.IsValidTarget(600) &&
                    (target.Health < DamageCalculate.GetComboDamage(target) ||
                     target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)))
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }

                switch (Menu.GetList("ComboMode"))
                {
                    case 0:
                        SpellManager.QEWLogic(Menu.GetBool("ComboQ"), Menu.GetBool("ComboW"), Menu.GetBool("ComboE"));
                        break;
                    case 1:
                        SpellManager.EQWLogic(Menu.GetBool("ComboQ"), Menu.GetBool("ComboW"), Menu.GetBool("ComboE"));
                        break;
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (Menu.GetBool("ComboR") && R.IsReady() &&
                    HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= R.Range) && !Q.IsReady() && !W.IsReady() &&
                    !E.IsReady())
                {
                    if (Menu.GetBool("ComboRalways"))
                    {
                        Orbwalker.SetAttack(false);
                        Orbwalker.SetMovement(false);
                        R.Cast();
                    }

                    if (Menu.GetBool("ComboRcanKill") &&
                        (target.Health <= DamageCalculate.GetComboDamage(target) ||
                         target.Health <= DamageCalculate.GetRDamage(target)*0.8))
                    {
                        Orbwalker.SetAttack(false);
                        Orbwalker.SetMovement(false);
                        R.Cast();
                    }

                    if (Menu.GetBool("ComboRhitCount") &&
                        HeroManager.Enemies.Count(x => !x.IsDead && x.DistanceToPlayer() <= R.Range) >=
                        Menu.GetSlider("ComboRhitCountcount"))
                    {
                        Orbwalker.SetAttack(false);
                        Orbwalker.SetMovement(false);
                        R.Cast();
                    }
                }
            }
        }
    }
}
