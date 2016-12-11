using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand.Modes
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    using static Brand.Extensions.Config;
    using static Brand.Extensions.Spells;
    using static Brand.Extensions.Other;

    internal class Combo
    {
        private static void CastQ()
        {
            var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target != null && target.IsValidTarget())
            {
                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (CM_Q_M == "Always")
                    {
                        var Predinction = Q.GetPrediction(target);
                        if (Predinction.Hitchance >= HitChance.VeryHigh)
                        {
                            Q.Cast(Predinction.CastPosition);
                        }
                    }
                    else
                    {
                        if (target.HasBuff("brandablaze"))
                        {
                            var Predinction = Q.GetPrediction(target);
                            if (Predinction.Hitchance >= HitChance.VeryHigh)
                            {
                                Q.Cast(Predinction.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void CastW()
        {
            var target = Variables.TargetSelector.GetTarget(W.Range, DamageType.Magical);

            if (target != null && target.IsValidTarget())
            {
                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    var Predinction = W.GetPrediction(target);
                    if (Predinction.Hitchance >= HitChance.VeryHigh)
                    {
                        W.Cast(Predinction.CastPosition);
                    }
                }
            }
        }

        private static void CastE()
        {
            var target = Variables.TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (target != null && target.IsValidTarget())
            {
                if (E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }
            }
        }

        private static void CastR()
        {
            var target = Variables.TargetSelector.GetTarget(R.Range + 450, DamageType.Magical);

            if (target != null && target.IsValidTarget())
            {
                if (R.IsReady())
                {
                    if (CM_R_S == 1)
                    {
                        if (target.IsValidTarget(R.Range))
                        {
                            if ((ComboDamage(target) > target.Health && CountMinionsInRange(400, target.Position) > 2) || target.CountEnemyHeroesInRange(450) >= 2)
                            {
                                R.Cast(target);
                            }
                        }
                    }
                    else
                    {
                        if (target.IsValidTarget(R.Range) && target.CountEnemyHeroesInRange(450) >= CM_R_S)
                        {
                            R.Cast(target);
                        }
                    }
                }
            }
        }

        public static void Execute()
        {
            if (CM_E)
            {
                CastE();
            }
            if (CM_Q)
            {
                CastQ();
            }
            if (CM_W)
            {
                CastW();
            }
            if (CM_R_B)
            {
                CastR();
            }
        }
    }
}
