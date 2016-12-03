using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir_Free_elo_Machine;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Azir_Creator_of_Elo
{
    class Menu
    {
        private AzirWalker walker;
        public LeagueSharp.Common.Menu GetMenu { get; private set; }
        private string _menuName;
        public Orbwalking.Orbwalker Orb { get; private set; }

        LeagueSharp.Common.Menu _orbWalkerMenu, _targetSelectorMenu;
        public Menu(string menuName)
        {
            this._menuName = menuName;
        }
        public virtual void LoadMenu(AzirMain azir)
        {
            GetMenu = new LeagueSharp.Common.Menu(_menuName, _menuName, true);
        _orbWalkerMenu = new LeagueSharp.Common.Menu("Orbwalker", "Orbwalker");
            Orb = new AzirWalker(_orbWalkerMenu, azir);
            _targetSelectorMenu = new LeagueSharp.Common.Menu("TargetSelector", "TargetSelector");
        }
        public virtual void CloseMenu()
        {
            TargetSelector.AddToMenu(_targetSelectorMenu);
            GetMenu.AddSubMenu(_orbWalkerMenu);        //ORBWALKER
            GetMenu.AddSubMenu(_targetSelectorMenu);
          
        }
        public virtual void LoadComboMenu()
        {

        }
    }
}
