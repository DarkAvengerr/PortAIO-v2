using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using Menu = LeagueSharp.Common.Menu;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace SergixBrand
{
    class BrandMenu : Menu
    {
        private LeagueSharp.Common.Menu _comboMenu,_harassMenu;
        public BrandMenu(string menuName, Core core) : base(menuName, core)
        {
        }

        public override void LoadComboMenu()
        {
            var _targetsRMe = new LeagueSharp.Common.Menu("Targets R killsteal", "Targets R kill");
            {
                foreach (var hero in HeroManager.Enemies)
                {
                    _targetsRMe.AddItem(new MenuItem("R"+hero.ChampionName, hero.ChampionName).SetValue(true));
                }
            }
            var _RMenu =new LeagueSharp.Common.Menu("R Options","R Menu");
            {
                _RMenu.AddSubMenu(_targetsRMe);
                _RMenu.AddItem(new MenuItem("CR", "Use R killsteal").SetValue(true));
                _RMenu.AddItem(new MenuItem("CRM", "Use R when x enemys on range").SetValue(new Slider(2, 0, 5)));
            }
            _comboMenu = new LeagueSharp.Common.Menu("Combo", "Combo");
            {
                _comboMenu.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CQS", "Use Q only when can stun").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CW", "Use W").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CE", "Use E").SetValue(true));
                _comboMenu.AddSubMenu(_RMenu);

            };
        GetMenu.AddItem(new MenuItem("Prediction", "Prediction").SetValue(
           new StringList(new[] { "OKTW Prediction", "Common Prediction" }, 0)));



            base.LoadComboMenu();
        }

        public override void LoadHarashMenu()
        {
            _harassMenu = new LeagueSharp.Common.Menu("Harass Menu", "Harass");
            {
                _harassMenu.AddItem(new MenuItem("HQ", "Use Q harass").SetValue(true));
                _harassMenu.AddItem(new MenuItem("HW", "Use W harass").SetValue(true));
                _harassMenu.AddItem(new MenuItem("HE", "Use E harass").SetValue(true));
            }
        }
        public  override void CloseMenu()
        {
            GetMenu.AddSubMenu(_comboMenu);
            GetMenu.AddSubMenu(_harassMenu);
            base.CloseMenu();
        }
    }
}
