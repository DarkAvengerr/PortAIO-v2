using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Modules
{
    interface ISOLOModule
    {
        /// <summary>
        /// Called when the module is loaded.
        /// </summary>
        void OnLoad();

        /// <summary>
        /// Should the module get executed.
        /// </summary>
        /// <returns></returns>
        bool ShouldGetExecuted();

        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <returns></returns>
        ModuleType GetModuleType();

        /// <summary>
        /// Called when the module is executed.
        /// </summary>
        void OnExecute();
    }
}
