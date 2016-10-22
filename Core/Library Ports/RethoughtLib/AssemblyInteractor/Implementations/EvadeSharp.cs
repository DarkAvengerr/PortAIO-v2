using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.AssemblyInteractor.Implementations
{
    #region Using Directives

    using System;

    using global::RethoughtLib.AssemblyInteractor.Abstract_Classes;

    using LeagueSharp.Common;

    #endregion

    public class EvadeSharp : Assembly
    {
        #region Public Methods and Operators

        public override void DisableByCustom()
        {
            throw new NotImplementedException();
        }

        public override void DisableByMenu()
        {
            Menu.GetMenu("Evade", "Evade")?.Item("Enabled")?.SetValue(false);
        }

        public override void EnableByCustom()
        {
            throw new NotImplementedException();
        }

        public override void EnableByMenu()
        {
            Menu.GetMenu("Evade", "Evade").Item("Enabled")?.SetValue(true);
        }

        #endregion
    }
}