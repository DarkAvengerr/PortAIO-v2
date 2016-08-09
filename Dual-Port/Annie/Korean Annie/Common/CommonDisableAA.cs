using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAnnie.Common
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    class CommonDisableAA
    {
        private readonly ICommonChampion champion;

        private bool CanUseAA()
        {
            bool canHit = true;

            if (KoreanUtils.GetParam(champion.MainMenu, "supportmode") != null) 
            {
                if (KoreanUtils.GetParamBool(champion.MainMenu, "supportmode") && champion.Player.CountAlliesInRange(1500f) > 1)
                {
                    canHit = false;
                }
            }
            return canHit;
        }

        private CommonDisableAAMode Mode
        {
            get { return (CommonDisableAAMode)KoreanUtils.GetParamStringList(champion.MainMenu, "disableaa"); }
        }

        public CommonDisableAA(ICommonChampion champion)
        {
            this.champion = champion;

            Orbwalking.BeforeAttack += CancelAA;
        }

        private void CancelAA(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target == null)
            {
                return;
            }

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
