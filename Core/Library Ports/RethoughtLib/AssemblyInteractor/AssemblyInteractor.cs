using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.AssemblyInteractor
{
    #region Using Directives

    using global::RethoughtLib.AssemblyInteractor.Abstract_Classes;

    #endregion

    public static class AssemblyInteractor
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Disables the by custom method.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void DisableByCustom(Assembly assembly)
        {
            assembly.DisableByCustom();
        }

        /// <summary>
        ///     Disables the by menu.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void DisableByMenu(Assembly assembly)
        {
            assembly.DisableByMenu();
        }

        /// <summary>
        ///     Enables the by custom method.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void EnableByCustom(Assembly assembly)
        {
            assembly.EnableByCustom();
        }

        /// <summary>
        ///     Enables the by menu.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public static void EnableByMenu(Assembly assembly)
        {
            assembly.EnableByMenu();
        }

        #endregion
    }
}