using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand.Modes
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    using static Brand.Extensions.Config;
    using static Brand.Extensions.Spells;

    internal class Harass
    {
        private static void CastQ()
        {
            var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target != null && target.IsValidTarget())
            {
                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (HM_Q_M == "Always")
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

        public static void Execute()
        {         
            if (MyHero.ManaPercent >= HM_M_SB_S && HM_M_SB_B)
            {
                if (HM_E)
                {
                    CastE();
                }
                if (HM_Q)
                {
                    CastQ();
                }
                if (HM_W)
                {
                    CastW();
                }
            }
            else if (!HM_M_SB_B)
            {
                if (HM_E)
                {
                    CastE();
                }
                if (HM_Q)
                {
                    CastQ();
                }
                if (HM_W)
                {
                    CastW();
                }
            }
        }
    }
}
