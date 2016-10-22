using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RyzeAssembly
{
    class Menu
    {
        private LeagueSharp.Common.Menu _drawSettingsMenu, _laneclearMenu, _jungleclearMenu, _harrashMenu,_miscMenu;
      public LeagueSharp.Common.Menu menu;
        public Orbwalking.Orbwalker orb;
        public Menu()
        {
            loadMenu();
        }
        public void loadMenu()
        {

            menu = new LeagueSharp.Common.Menu("Ryze", "Ryze", true);
            var orbWalkerMenu = new LeagueSharp.Common.Menu("Orbwalker", "Orbwalker");
      orb = new Orbwalking.Orbwalker(orbWalkerMenu);
            var TargetSelectorMenu = new LeagueSharp.Common.Menu("TargetSelector", "TargetSelector");
            loadLaneClear();
            loadDrawings();
            loadJungleClear();
            loadHarassh();
            loadMisc();
            TargetSelector.AddToMenu(TargetSelectorMenu);
            menu.AddSubMenu(orbWalkerMenu);        //ORBWALKER
            menu.AddSubMenu(TargetSelectorMenu);   //TS
                                                 // menu.AddSubMenu(itemMenu);
            menu.AddSubMenu(_harrashMenu);
           menu.AddSubMenu(_laneclearMenu);        //LANECLEAR
           menu.AddSubMenu(_jungleclearMenu);      //JUNGLECLEAR
            menu.AddSubMenu(_miscMenu);
            menu.AddSubMenu(_drawSettingsMenu);     //DRAWS
            menu.AddToMainMenu();
        }
        public void loadHarassh()
        {

            _harrashMenu = new LeagueSharp.Common.Menu("Harrash", "Harrash");
            {
                _harrashMenu.AddItem(new MenuItem("QH", "Use Q in Harrash").SetValue(true));
                _harrashMenu.AddItem(new MenuItem("ManaH", "% mana Harrash").SetValue(new Slider(40,0,100)));
            }
        }
    public void loadMisc()
        {
            _miscMenu = new LeagueSharp.Common.Menu("Misc", "Misc");
        {
                _miscMenu.AddItem(new MenuItem("%R", "% R heal ").SetValue(new Slider(30, 0, 100)));
            }
        }
        public void loadLaneClear()
        {
           _laneclearMenu = new LeagueSharp.Common.Menu("Laneclear", "Laneclear");
            {
                _laneclearMenu.AddItem(new MenuItem("QL", "Use Q in Laneclear").SetValue(true));
                _laneclearMenu.AddItem(new MenuItem("WL", "Use W in Laneclear").SetValue(true));
               _laneclearMenu.AddItem(new MenuItem("EL", "Use E in Laneclear").SetValue(true));
                _laneclearMenu.AddItem(new MenuItem("RL", "Use R in Laneclear").SetValue(true));
                _laneclearMenu.AddItem(new MenuItem("ManaL", "% mana LaneClear").SetValue(new Slider(40, 0, 100)));
            }
      
        }
        public void loadJungleClear()
        {
         _jungleclearMenu = new LeagueSharp.Common.Menu("Jungleclear", "Jungleclear");
            {
                _jungleclearMenu.AddItem(new MenuItem("QJ", "Use Q in JungleClear").SetValue(true));
                _jungleclearMenu.AddItem(new MenuItem("WJ", "Use W in JungleClear").SetValue(true));
                _jungleclearMenu.AddItem(new MenuItem("EJ", "Use E in JungleClear").SetValue(true));
                _jungleclearMenu.AddItem(new MenuItem("RJ", "Use R in JungleClear").SetValue(true));
                _jungleclearMenu.AddItem(new MenuItem("ManaJ", "% mana JungleClear").SetValue(new Slider(40, 0, 100)));
            }
        }
        public void loadDrawings()
        {
          _drawSettingsMenu = new LeagueSharp.Common.Menu("Draw Settings", "Draw Settings");
            {
                _drawSettingsMenu.AddItem(new MenuItem("Draw Q Range", "Draw Q Range").SetValue(true));
                _drawSettingsMenu.AddItem(new MenuItem("Draw W Range", "Draw W Range").SetValue(true));
                _drawSettingsMenu.AddItem(new MenuItem("Draw E Range", "Draw E Range").SetValue(true));
             
            }
        }

    }
}
