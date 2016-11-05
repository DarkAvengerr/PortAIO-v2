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
    class EkkoMenu:Menu
    {
        private LeagueSharp.Common.Menu _drawSettingsMenu, _comboMenu, _harashMenu, _laneClearMenu, _JungleClearMenu;
        public EkkoMenu(String name) : base(name)
        {
            loadMenu();
            closeMenu();
        }
        public override void loadMenu()
        {
            base.loadMenu();
            loadLaneClearMenu();
            loadHarashMenu();
            loadComboMenu();
            loadJungleClearMenu();
            loadDrawings();

        }

        public override void closeMenu()
        {
            // add menus
            base.GetMenu.AddSubMenu(_comboMenu);
            base.GetMenu.AddSubMenu(_harashMenu);
            base.GetMenu.AddSubMenu(_laneClearMenu);
            base.GetMenu.AddSubMenu(_JungleClearMenu);
            base.GetMenu.AddSubMenu(_drawSettingsMenu);

            base.closeMenu();
        }
        public void loadDrawings()
        {
            _drawSettingsMenu = new LeagueSharp.Common.Menu("Draw Settings", "Draw Settings");
            {
                _drawSettingsMenu.AddItem(new MenuItem("dq", "Draw Q range").SetValue(true));
                _drawSettingsMenu.AddItem(new MenuItem("dw", "Draw W range").SetValue(true));
                _drawSettingsMenu.AddItem(new MenuItem("de", "Draw E range").SetValue(true));

                _drawSettingsMenu.AddItem(new MenuItem("dr", "Draw R").SetValue(true));
                _drawSettingsMenu.AddItem(new MenuItem("DDM", "Draw Damage After Combo").SetValue(true));
            }
        }
        public void loadComboMenu()
        {
            _comboMenu = new LeagueSharp.Common.Menu("Combo Menu", "Combo Menu");
            {
                _comboMenu.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CW", "Use W").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CE", "Use E").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CRK", "Use R killsteal").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CR", "Use Inteligent R").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CAR", "R cast x enemys at range").SetValue(new Slider(3, 0, 5)));

                _comboMenu.AddItem(new MenuItem("CRs", "Risk check slider").SetValue(new Slider(45, 0, 100)));
                _comboMenu.AddItem(new MenuItem("CHR", "R if health less than %:").SetValue(new Slider(10,0,100)));
             
            }
        }
        public void loadLaneClearMenu()
        {
            _laneClearMenu = new LeagueSharp.Common.Menu("Laneclear Menu", "Laneclear Menu");
            {
                _laneClearMenu.AddItem(new MenuItem("LQ", "Use Q").SetValue(true));
                _laneClearMenu.AddItem(new MenuItem("LE", "Use E").SetValue(true));

            }
        }
        public void loadJungleClearMenu()
        {
            _JungleClearMenu = new LeagueSharp.Common.Menu("JungleClear Menu", "JungleClear  Menu");
            {
                _JungleClearMenu.AddItem(new MenuItem("JQ", "Use Q").SetValue(true));
                _JungleClearMenu.AddItem(new MenuItem("JW", "Use W").SetValue(true));
                _JungleClearMenu.AddItem(new MenuItem("JE", "Use E").SetValue(true));
            }
        }
        public void loadHarashMenu()
        {
            _harashMenu = new LeagueSharp.Common.Menu("Harash Menu", "Harash Menu");
            {
          
                _harashMenu.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
                _harashMenu.AddItem(new MenuItem("HW", "Use W").SetValue(false));
                _harashMenu.AddItem(new MenuItem("HE", "Use E").SetValue(false));
            }
        }
   
    }
}
