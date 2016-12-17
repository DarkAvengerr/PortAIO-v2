using System.Linq;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Events.Games.Mode
{
    using Spells;
    using FlowersDariusCommon;
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class Combo : Logic
    {
        internal static void Init()
        {
            var target = TargetSelector.GetTarget(600f, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                if (Menu.GetBool("ComboIgnite") && Ignite != SpellSlot.Unknown && Ignite.IsReady() &&
                    target.DistanceToPlayer() <= 600f &&
                    (target.Health < DamageCalculate.GetComboDamage(target) ||
                     target.Health < DamageCalculate.GetIgniteDmage(target)))
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }

                if (HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= 400))
                {
                    SpellManager.CastItem();
                }

                if (Menu.GetKey("ComboR") && R.IsReady())
                {
                    foreach (var rTarget in HeroManager.Enemies.Where(x => x.DistanceToPlayer() <= R.Range &&
                                                                           x.Health <= DamageCalculate.GetRDamage(x) &&
                                                                           !x.HasBuff("willrevive")))
                    {
                        if (rTarget.IsValidTarget(R.Range))
                        {
                            R.CastOnUnit(target, true);
                        }
                    }
                }

                if (Menu.GetBool("ComboQLock") && Me.HasBuff("dariusqcast") && Me.CountEnemiesInRange(800) < 3)
                {
                    Orbwalker.SetMovement(false);
                    Orbwalker.SetAttack(false);

                    if (target.DistanceToPlayer() <= 250)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Me.Position.Extend(target.Position, -Q.Range));
                    }
                    else if (target.DistanceToPlayer() <= Q.Range)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                    else
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                    }
                }
                else
                {
                    Orbwalker.SetMovement(true);
                    Orbwalker.SetAttack(true);
                }

                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.DistanceToPlayer() <= Q.Range &&
                    SpellManager.CanQHit(target) && Me.CanMoveMent())
                {
                    if (Menu.GetBool("ComboSaveMana") && Me.Mana < SpellManager.RMana + Q.Instance.SData.Mana)
                    {
                        return;
                    }

                    if (Utils.TickCount - lastETime > 1000)
                    {
                        Q.Cast(true);
                    }
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && target.DistanceToPlayer() <= E.Range - 30 &&
                    !Orbwalking.InAutoAttackRange(target) && !target.HaveShiled())
                {
                    if (Menu.GetBool("ComboSaveMana") && Me.Mana < SpellManager.RMana + Q.Instance.SData.Mana)
                    {
                        return;
                    }

                    var pred = E.GetPrediction(target);
                    E.Cast(pred.UnitPosition, true);
                }
            }
        }
    }
}
