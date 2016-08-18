using DZLib.Logging;
using iSeriesReborn.Champions.Vayne.Utility;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Geometry;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Vayne.Modules
{
    class VayneFocus2W : IModule
    {
        public string GetName()
        {
            return "Vayne_Focus2W";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return MenuExtensions.GetItemValue<bool>("iseriesr.vayne.misc.general.focus2w");
        }

        public void Run()
        {
            var target = HeroManager.Enemies.Find(enemy => enemy.IsValidTarget(ObjectManager.Player.AttackRange + 65f + 65f)
                    && VayneUtility.Has2WStacks(enemy));
            if (target != null)
            {
                TargetSelector.SetTarget(target);
            }
        }
    }
}
