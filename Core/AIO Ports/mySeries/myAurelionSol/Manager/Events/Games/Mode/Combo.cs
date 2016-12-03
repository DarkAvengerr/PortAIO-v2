using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol.Manager.Events.Games.Mode
{
    using System.Linq;
    using Spells;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Combo : Logic
    {
        internal static void Init()
        {
            if (Menu.GetBool("ComboQ"))
            {
                var target = TargetSelector.GetTarget(Q.Range * 2, TargetSelector.DamageType.Magical);

                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    var qPred = Q.GetPrediction(target, true);

                    if (qPred.Hitchance >= HitChance.VeryHigh || qPred.Hitchance == HitChance.Immobile)
                    {
                        Q.Cast(qPred.CastPosition, true);
                    }
                }
            }

            if (Menu.GetBool("ComboQFollow"))
            {
                if (SpellManager.SecondQ && qMillile != null)
                {
                    Orbwalker.SetMovement(false);
                    Orbwalker.SetAttack(false);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, qMillile.Position);
                }
                else if (!SpellManager.SecondQ && qMillile == null)
                {
                    Orbwalker.SetAttack(true);
                    Orbwalker.SetMovement(true);
                }
            }

            if (Menu.GetBool("ComboW") && SpellManager.HavePassive && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                if (!SpellManager.IsWActive)
                {
                    if (target != null && target.IsValidTarget(W.Range) && !target.IsValidTarget(420))
                    {
                        W.Cast();
                    }
                }
                else if (SpellManager.IsWActive)
                {
                    if (!target.IsValidTarget(800) || target.IsValidTarget(420))
                    {
                        W.Cast();
                    }
                }
            }

            if (Menu.GetBool("ComboR") && R.IsReady())
            {
                foreach (var enemy in from enemy in HeroManager.Enemies
                                      let startPos = enemy.ServerPosition
                                      let endPos = Me.ServerPosition.Extend(startPos, Me.Distance(enemy) + R.Range)
                                      let rectangle = new Geometry.Polygon.Rectangle(startPos, endPos, R.Width)
                                      where HeroManager.Enemies.Count(rectangle.IsInside) >= Menu.GetSlider("ComboRHit")
                                      select enemy)
                {
                    var rPred = R.GetPrediction(enemy, true);

                    if (rPred.Hitchance >= HitChance.VeryHigh || rPred.Hitchance == HitChance.Immobile)
                    {
                        R.Cast(rPred.CastPosition, true);
                    }
                }
            }
        }
    }
}
