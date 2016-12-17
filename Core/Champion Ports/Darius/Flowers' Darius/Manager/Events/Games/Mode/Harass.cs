using LeagueSharp;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Events.Games.Mode
{
    using Spells;
    using FlowersDariusCommon;
    using LeagueSharp.Common;
    

    internal class Harass : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);

                if (target.Check(600))
                {
                    if (Menu.GetBool("HarassQLock") && Me.HasBuff("dariusqcast") && Me.CountEnemiesInRange(800) < 3)
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

                    if (Menu.GetBool("HarassE") && E.IsReady() && !Orbwalking.InAutoAttackRange(target) &&
                        !target.HaveShiled() && target.DistanceToPlayer() <= E.Range - 30)
                    {
                        var pred = E.GetPrediction(target);
                        E.Cast(pred.UnitPosition, true);
                    }

                    if (Menu.GetBool("HarassQ") && Q.IsReady() && !Orbwalking.InAutoAttackRange(target) &&
                        target.DistanceToPlayer() <= Q.Range && SpellManager.CanQHit(target))
                    {
                        Q.Cast(true);
                    }
                }
            }
        }
    }
}
