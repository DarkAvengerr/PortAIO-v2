using EloBuddy; namespace RethoughtLib.AssemblyDisabler
{
    #region Using Directives

    using global::RethoughtLib.AssemblyDisabler.Interfaces;

    #endregion

    public static class AssemblyDisabler
    {
        #region Public Methods and Operators

        public static void DisableByCustom(IAssembly assembly)
        {
            assembly.DisableByCustom();
        }

        public static void DisableByMenu(IAssembly assembly)
        {
            assembly.DisableByMenu();
        }

        public static void EnableByCustom(IAssembly assemlby)
        {
            assemlby.EnableByCustom();
        }

        public static void EnableByMenu(IAssembly assembly)
        {
            assembly.EnableByMenu();
        }

        #endregion
    }
}