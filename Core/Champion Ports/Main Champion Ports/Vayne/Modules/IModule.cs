using VayneHunter_Reborn.Modules.ModuleHelpers;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Modules
{
    interface IModule
    {
        void OnLoad();

        bool ShouldGetExecuted();

        ModuleType GetModuleType();

        void OnExecute();
    }
}
