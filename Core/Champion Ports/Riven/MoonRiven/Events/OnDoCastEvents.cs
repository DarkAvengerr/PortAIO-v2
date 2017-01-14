using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class OnSpellCastEvents : Logic
    {
        internal static void Init()
        {
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || !sender.IsMe || Args.SData == null)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(Args);
                    break;
                case Orbwalking.OrbwalkingMode.Burst:
                    Burst(Args);
                    break;
            }
        }

        private static void Combo(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null)
            {
                return;
            }

            AIHeroClient target = null;

            if (myTarget.IsValidTarget())
            {
                target = myTarget;
            }
            else if (Args.Target is AIHeroClient)
            {
                target = (AIHeroClient)Args.Target;
            }

            if (target != null && target.IsValidTarget(400))
            {
                #region After Timat Logic

                if (Args.SData.Name == "ItemTiamatCleave")
                {
                    if (MenuInit.ComboW && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.Cast(true);
                        return;
                    }

                    if (Q.IsReady() && target.IsValidTarget(400) && Riven.CastQ(target))
                    {
                        return;
                    }
                }
                #endregion

                #region After W Logic

                if (Args.SData.Name == "RivenMartyr")
                {
                    if (MenuInit.ComboR && R.IsReady() && !isRActive && Riven.R1Logic(target))
                    {
                        return;
                    }

                    if (Q.IsReady() && target.IsValidTarget(400) && Riven.CastQ(target))
                    {
                        return;
                    }
                }
                #endregion

                #region After R Logic

                if (Args.SData.Name == "RivenFengShuiEngine")
                {
                    if (MenuInit.ComboW && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.Cast(true);
                        return;
                    }
                }
                #endregion

                #region After R1 Logic

                if (Args.SData.Name == "RivenIzunaBlade" || Args.SData.Name.Contains("RivenWindslash") && qStack == 2)
                {
                    if (Q.IsReady() && target.IsValidTarget(400))
                    {
                        Q.Cast(target.Position, true);
                    }
                }
                #endregion
            }
        }

        private static void Burst(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null)
            {
                return;
            }

            var target = TargetSelector.GetSelectedTarget();

            if (target != null && target.IsValidTarget())
            {
                if (MenuInit.BurstMode == 0)
                {
                    ShyBurst(target, Args);
                }
                else
                {
                    EQFlashBurst(target, Args);
                }
            }
        }

        private static void ShyBurst(AIHeroClient target, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData.Name == "ItemTiamatCleave")
            {
                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(true);
                    return;
                }

                if (Q.IsReady() && target.IsValidTarget(400) && Riven.CastQ(target))
                {
                    return;
                }
            }

            if (Args.SData.Name == "RivenIzunaBlade" || Args.SData.Name.Contains("RivenWindslash"))
            {
                if (Q.IsReady() && target.IsValidTarget(400))
                {
                    Q.Cast(target.Position, true);
                }
            }
        }

        private static void EQFlashBurst(AIHeroClient target, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData.Name == "RivenIzunaBlade" || Args.SData.Name.Contains("RivenWindslash"))
            {
                if (Q.IsReady() && target.IsValidTarget(400) && Q.Cast(target.Position, true))
                {
                    return;
                }
            }

            if (Args.SData.Name == "RivenMartyr")
            {
                if (MenuInit.ComboR && R.IsReady() && isRActive && R.Cast(target.Position, true))
                {
                    return;
                }

                if (Q.IsReady() && target.IsValidTarget(400) && Riven.CastQ(target))
                {
                    return;
                }
            }

            if (Args.SData.Name == "ItemTiamatCleave")
            {
                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(true);
                    return;
                }

                if (Q.IsReady() && target.IsValidTarget(400))
                {
                    Riven.CastQ(target);
                }
            }
        }
    }
}