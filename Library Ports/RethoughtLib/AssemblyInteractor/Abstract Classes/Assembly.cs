using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.AssemblyInteractor.Abstract_Classes
{
    public abstract class Assembly
    {
        #region Methods

        /// <summary>
        /// Disables the assembly by custom method.
        /// </summary>
        public abstract void DisableByCustom();

        /// <summary>
        /// Disables the assembly by menu.
        /// </summary>
        public abstract void DisableByMenu();

        /// <summary>
        /// Enables the assembly by custom method.
        /// </summary>
        public abstract void EnableByCustom();

        /// <summary>
        /// Enables the assembly by menu.
        /// </summary>
        public abstract void EnableByMenu();

        #endregion
    }
}