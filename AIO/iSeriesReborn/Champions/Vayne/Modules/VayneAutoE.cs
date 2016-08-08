using DZLib.Logging;
using iSeriesReborn.Champions.Vayne.Skills;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Vayne.Modules
{
    class VayneAutoE : IModule
    {
        public string GetName()
        {
            return "Vayne_AutoE";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return MenuExtensions.GetItemValue<bool>("iseriesr.vayne.misc.condemn.autoe");
        }

        public void Run()
        {
            VayneE.ECheck(ObjectManager.Player.ServerPosition);
        }
    }
}
