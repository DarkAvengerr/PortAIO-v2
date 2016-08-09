using System;

namespace KoreanCommon
{
    using EloBuddy;
    using LeagueSharp;
    using LeagueSharp.Common;

    public enum CommonDisableAAMode
    {
        Never,

        Always,

        SomeSkillReady,

        HarasComboReady,

        FullComboReady
    };

    public class CommonDisableAA
    {
        private CommonChampion champion;

        public CommonDisableAA(CommonChampion champion)
        {
            this.champion = champion;

            Orbwalking.BeforeAttack += CancelAA;
        }

        public CommonDisableAAMode Mode
        {
            get
            {
                if (KoreanUtils.GetParam(champion.MainMenu, "disableaa") != null)
                {
                    return (CommonDisableAAMode) KoreanUtils.GetParamStringList(champion.MainMenu, "disableaa");
                }
                else
                {
                    return CommonDisableAAMode.Never;
                }
            }
        }

        public bool CanUseAA()
        {
            bool canHit = true;

            if (KoreanUtils.GetParam(champion.MainMenu, "supportmode") != null)
            {
                if (KoreanUtils.GetParamBool(champion.MainMenu, "supportmode")
                    && champion.Player.CountAlliesInRange(1500f) > 0)
                {
                    canHit = false;
                }
            }
            return canHit;
        }

        private void CancelAA(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target != null)
            {
                if (champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    switch (Mode)
                    {
                        case CommonDisableAAMode.Always:
                            args.Process = false;
                            break;
                        case CommonDisableAAMode.Never:
                            args.Process = true;
                            break;
                        case CommonDisableAAMode.SomeSkillReady:
                            if (champion.Spells.SomeSkillReady())
                            {
                                args.Process = false;
                            }
                            break;
                        case CommonDisableAAMode.HarasComboReady:
                            if (champion.Spells.HarasReady())
                            {
                                args.Process = false;
                            }
                            break;
                        case CommonDisableAAMode.FullComboReady:
                            if (champion.Spells.ComboReady())
                            {
                                args.Process = false;
                            }
                            break;
                    }
                }
                else
                {
                    if (args.Target is Obj_AI_Base && ((Obj_AI_Base)args.Target).IsMinion && !CanUseAA())
                    {
                        args.Process = false;
                    }
                }
            }
        }
    }
}