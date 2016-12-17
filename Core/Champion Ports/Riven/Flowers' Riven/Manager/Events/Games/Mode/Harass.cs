using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events.Games.Mode
{
    using myCommon;
    using Spells;
    using LeagueSharp.Common;

    internal class Harass : Logic
    {
        internal static void Init()
        {
            var target = TargetSelector.GetSelectedTarget() ??
             TargetSelector.GetTarget(E.Range + Me.BoundingRadius, TargetSelector.DamageType.Physical);

            if (target.Check())
            {
                if (Menu.GetList("HarassMode") == 0)
                {
                    if (E.IsReady() && Menu.GetBool("HarassE") && qStack == 2 && Me.CanMoveMent())
                    {
                        var pos = Me.Position + (Me.Position - target.Position).Normalized() * E.Range;

                        E.Cast(Me.Position.Extend(pos, E.Range), true);
                    }

                    if (Q.IsReady() && Menu.GetBool("HarassQ") && qStack == 2 && Me.CanMoveMent())
                    {
                        var pos = Me.Position + (Me.Position - target.Position).Normalized() * E.Range;

                        LeagueSharp.Common.Utility.DelayAction.Add(100, () => Q.Cast(Me.Position.Extend(pos, Q.Range), true));
                    }

                    if (W.IsReady() && Menu.GetBool("HarassW") && target.IsValidTarget(W.Range) && qStack == 1)
                    {
                        W.Cast(true);
                    }

                    if (Q.IsReady() && Menu.GetBool("HarassQ") && qStack == 0 && Me.CanMoveMent())
                    {
                        SpellManager.CastQ(target);
                    }
                }
                else
                {
                    if (E.IsReady() && Menu.GetBool("HarassE") && Me.CanMoveMent() &&
                        target.DistanceToPlayer() <= E.Range + (Q.IsReady() ? Q.Range : Me.AttackRange))
                    {
                        E.Cast(target.Position, true);
                    }

                    if (Q.IsReady() && Menu.GetBool("HarassQ") && target.IsValidTarget(Q.Range) && qStack == 0 &&
                        Utils.TickCount - lastQTime > 500 && Me.CanMoveMent())
                    {
                        SpellManager.CastQ(target);
                    }

                    if (W.IsReady() && Menu.GetBool("HarassW") && target.IsValidTarget(W.Range) && (!Q.IsReady() || qStack == 1))
                    {
                        W.Cast(true);
                    }
                }
            }
        }
    }
}
