using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Marksman.Common
{
    public static class CommonAlly
    {
        public static Menu MenuLocal { get; private set; }

        public static void Init(Menu nParentMenu)
        {
            MenuLocal = new Menu("Ally Support", "Menu.Ally.Support");
            {
                if (HeroManager.Allies.Find(e => e.CharData.BaseSkinName.ToLower() == "thresh") != null)
                {
                    Common.CommonAllySupport.AllyThreshLantern.Init(MenuLocal);
                }
                else
                {
                    MenuLocal.AddItem(new MenuItem("Ally.Support.None", "There are no supporting an Ally Hero"));
                }
            }
            nParentMenu.AddSubMenu(MenuLocal);
        }
    }
}
