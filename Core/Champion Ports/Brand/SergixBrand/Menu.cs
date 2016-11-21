using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;


using EloBuddy; 
using LeagueSharp.Common; 
 namespace SergixBrand
{
    public class Menu
    {
        public LeagueSharp.Common.Menu GetMenu { get; private set; }
        private string _menuName;
        public Orbwalking.Orbwalker Orb { get; private set; }
        private LeagueSharp.Common.Menu _orbWalkerMenu, _targetSelectorMenu, _drawingMenu;
        public Menu(string menuName, Core core)
        {
            this._menuName = menuName;
            LoadMenu(core);
            CloseMenu();
        }
        public bool getMenuBoolOption(String s)
        {
            return GetMenu.Item(s).GetValue<bool>();
        }

        public virtual void LoadMenu(Core core)
        {
            Console.WriteLine("hEY BUDY");
            GetMenu = new LeagueSharp.Common.Menu(_menuName, _menuName, true);
            _orbWalkerMenu = new LeagueSharp.Common.Menu("Orbwalker", "Orbwalker");
            Orb = new Orbwalking.Orbwalker(_orbWalkerMenu);
            _targetSelectorMenu = new LeagueSharp.Common.Menu("TargetSelector", "TargetSelector");
            LoadLaneClearMenu();
            LoadHarashMenu();
            LoadComboMenu();
            LoadJungleClearMenu();
            LoadDrawings();
            LoadkeyMenu();
            LoadMiscMenu();

        }

        public virtual void LoadMiscMenu()
        {

        }

        public virtual void LoadkeyMenu()
        {

        }

        public virtual void LoadComboMenu()
        {

        }

        public virtual void LoadDrawings()
        {
            _drawingMenu = new LeagueSharp.Common.Menu("Drawings", "Draw Menu");
            {
                _drawingMenu.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(true));
                _drawingMenu.AddItem(new MenuItem("DW", "Draw W Range").SetValue(true));
                _drawingMenu.AddItem(new MenuItem("DE", "Draw E Range").SetValue(true));
                _drawingMenu.AddItem(new MenuItem("DR", "Draw R Range").SetValue(true));
                _drawingMenu.AddItem(new MenuItem("DTD", "Draw Total Damage").SetValue(true).SetTooltip("Q=Blue W=Orange E=Green Red=R"));
            }

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
            GetMenu.AddSubMenu(_drawingMenu);
            GetMenu.AddToMainMenu();

        }

        public static implicit operator Menu(LeagueSharp.Common.Menu v)
        {
            throw new NotImplementedException();
        }
    }
}
