using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.AssemblyDisabler.Implementations
{
    #region Using Directives

    using System;

    using global::RethoughtLib.AssemblyDisabler.Interfaces;

    using LeagueSharp.Common;

    #endregion

    public class EvadeSharp : IAssembly
    {
        #region Public Methods and Operators

        public void DisableByCustom()
        {
            throw new NotImplementedException();
        }

        public void DisableByMenu()
        {
            Menu.GetMenu("Evade", "Evade")?.Item("Enabled")?.SetValue(false);
        }

        public void EnableByCustom()
        {
            throw new NotImplementedException();
        }

        public void EnableByMenu()
        {
            Menu.GetMenu("Evade", "Evade").Item("Enabled").SetValue(true);
        }

        #endregion
    }
}