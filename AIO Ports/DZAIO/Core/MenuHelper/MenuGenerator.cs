using LeagueSharp.Common;
using TargetSelector = LeagueSharp.Common.TargetSelector;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Core.MenuHelper
{
    class MenuGenerator
    {
        public static void GenerateMenu()
        {
            var rootMenu = Variables.AssemblyMenu;
            TargetSelector.AddToMenu(rootMenu);
        }
    }
}
