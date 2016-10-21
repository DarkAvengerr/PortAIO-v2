using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAnnie.Common
{
    using System;

    using LeagueSharp.Common;

    internal abstract class CommonOrbwalkImplementation
    {
        #region Properties

        protected ICommonChampion Champion { get; set; }

        #endregion

        #region Public Methods and Operators

        public abstract void ComboMode();

        public abstract void Ultimate();

        #endregion

        #region Methods

        protected abstract void HarasMode();

        protected abstract void LaneClearMode();

        protected abstract void LastHitMode();

        protected void UseSkills(EventArgs args)
        {
            if (Champion == null)
            {
                return;
            }

            switch (Champion.MainMenu.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHitMode();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    HarasMode();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClearMode();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    ComboMode();
                    break;
            }
        }

        #endregion
    }
}