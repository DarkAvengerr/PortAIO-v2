using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DarkMage
{
    public class Menu
    {
        public LeagueSharp.Common.Menu GetMenu { get; private set; }
        private string _menuName;
        public Orbwalking.Orbwalker Orb { get; private set; }
        LeagueSharp.Common.Menu _orbWalkerMenu, _targetSelectorMenu;
        public Menu(string menuName, SyndraCore core)
        {
            this._menuName = menuName;
            LoadMenu(core);
            CloseMenu();
        }
        public virtual void LoadMenu(SyndraCore azir)
        {
            GetMenu = new LeagueSharp.Common.Menu(_menuName, _menuName, true).SetFontStyle(FontStyle.Regular, Color.YellowGreen); ;
            _orbWalkerMenu = new LeagueSharp.Common.Menu("Orbwalker", "Orbwalker");
            Orb = new Orbwalking.Orbwalker(_orbWalkerMenu);
            _targetSelectorMenu = new LeagueSharp.Common.Menu("TargetSelector", "TargetSelector");
            LoadLaneClearMenu();
            LoadHarashMenu();
            LoadComboMenu();
            LoadJungleClearMenu();
            LoadDrawings();
            LoadkeyMenu();
          //  LoadMiscInterrupt(azir);
          //  LoadMiscMenu(azir);

        }

        private void LoadMiscMenu(SyndraCore azir)
        {
            throw new NotImplementedException();
        }

        private void LoadMiscInterrupt(SyndraCore azir)
        {
            throw new NotImplementedException();
        }

        public virtual void LoadkeyMenu()
        {
 
        }

       public virtual void LoadComboMenu()
        {
         
        }

      public virtual void LoadDrawings()
        {
        }

        private void LoadJungleClearMenu()
        {
        }

        public virtual void LoadHarashMenu()
        {
  
        }

        public virtual void LoadLaneClearMenu()
        {
        }

        public virtual void CloseMenu()
        {
            TargetSelector.AddToMenu(_targetSelectorMenu);
            GetMenu.AddSubMenu(_orbWalkerMenu);        //ORBWALKER
            GetMenu.AddSubMenu(_targetSelectorMenu);
            GetMenu.AddToMainMenu();

        }
    }
}
