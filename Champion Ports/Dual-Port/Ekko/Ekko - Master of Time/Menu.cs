using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_master_of_time
{
   
        class Menu
        {
            private LeagueSharp.Common.Menu menu;
            public LeagueSharp.Common.Menu GetMenu
            {
                get { return menu; }
            }
            private string _menuName;
            private Orbwalking.Orbwalker orb;
            public Orbwalking.Orbwalker Orb
            {
                get { return orb; }
            }
            LeagueSharp.Common.Menu _orbWalkerMenu, _targetSelectorMenu;
            public Menu(string menuName)
            {
                this._menuName = menuName;
            }
            public virtual void loadMenu()
            {
                menu = new LeagueSharp.Common.Menu(_menuName, _menuName, true);
                _orbWalkerMenu = new LeagueSharp.Common.Menu("Orbwalker", "Orbwalker");
                orb = new Orbwalking.Orbwalker(_orbWalkerMenu);
                _targetSelectorMenu = new LeagueSharp.Common.Menu("TargetSelector", "TargetSelector");
            }
            public virtual void closeMenu()
            {
                TargetSelector.AddToMenu(_targetSelectorMenu);
                menu.AddSubMenu(_orbWalkerMenu);        //ORBWALKER
                menu.AddSubMenu(_targetSelectorMenu);
                menu.AddToMainMenu();
            }
            public virtual void loadComboMenu()
            {

            }
        }
    }
