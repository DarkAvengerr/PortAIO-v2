using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DarkMage
{
   public class SyndraMenu : Menu
    {
        private LeagueSharp.Common.Menu _comboMenu,_drawingMenu, _harassMenu, _keyMenu,_targetsRMe, _dontRIfSpellReady,_farmMenu,_laneClearMenu,_JungleClearMenu;
        public SyndraMenu(string menuName, SyndraCore core) : base(menuName, core)
        {
        }
        public override void LoadComboMenu()
        {
            _comboMenu = new LeagueSharp.Common.Menu("Combo", "Combo Menu");
            {
                _comboMenu.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CW", "Use W").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CE", "Use E").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CR", "Use R").SetValue(true));
            }
        }
        public override void LoadHarashMenu()
        {
            _harassMenu = new LeagueSharp.Common.Menu("Harass", "Harass Menu");
            {
                _harassMenu.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
                _harassMenu.AddItem(new MenuItem("HW", "Use W").SetValue(false));
                _harassMenu.AddItem(new MenuItem("HE", "Use E").SetValue(false));
            }
        }
        public override void LoadDrawings()
        {
            _drawingMenu = new LeagueSharp.Common.Menu("Drawings", "Draw Menu");
            {
                _drawingMenu.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(true));
                _drawingMenu.AddItem(new MenuItem("DW", "Draw W Range").SetValue(true));
                _drawingMenu.AddItem(new MenuItem("DE", "Draw E Range").SetValue(true));
                _drawingMenu.AddItem(new MenuItem("DR", "Draw R Range").SetValue(true));
                _drawingMenu.AddItem(new MenuItem("DTD", "Draw Total Damage").SetValue(true).SetTooltip("Q=Blue W=Orange E=Green Red=R"));
                _drawingMenu.AddItem(new MenuItem("DO", "Draw Orbs").SetValue(true));
                _drawingMenu.AddItem(new MenuItem("DST", "Draw Sphere Text").SetValue(true));
            }
            _targetsRMe = new LeagueSharp.Common.Menu("Targets R", "Targets R");
            {
                foreach (AIHeroClient hero in HeroManager.Enemies)
                {
                    _targetsRMe.AddItem(new MenuItem(hero.ChampionName, hero.ChampionName).SetValue(true));
                }
            }
            _dontRIfSpellReady = new LeagueSharp.Common.Menu("R Spells", "Dont R if");
            {
               _dontRIfSpellReady.AddItem(new MenuItem("DONTRZHONYA", "Dont R if enemy has zhonia active").SetValue(false));
                foreach (AIHeroClient hero in HeroManager.Enemies)
                {
                    foreach (String s in Lists.DontRSpellList)
                    {
                        var result = s.Split('-');
                        if (result[0].ToLower() == hero.ChampionName.ToLower())
                        {
                            _dontRIfSpellReady.AddItem(new MenuItem(hero.ChampionName+ "-"+result[1], hero.ChampionName+ " " + result[1]).SetValue(true));
                        }
                
                        //   "Fizz-E","Vladimir-W","Ekkko-R","Zed-R","Yi-Q","Zilean-R","Shaco-R","Kalista-R","Lissandra-R","Kindred-R","Kayle-R","Taric-R"
                    }
                    if (hero.ChampionName == "MasterYi")
                    {
                        _dontRIfSpellReady.AddItem(new MenuItem(hero.ChampionName + "-" +"Q", hero.ChampionName + " " +"Q").SetValue(true));
                    }
                }
            }
        }
        public override void LoadLaneClearMenu()
        {
            _laneClearMenu = new LeagueSharp.Common.Menu("Laneclear", "Laneclear");
            {
                _laneClearMenu.AddItem(new MenuItem("LQ", "Use Q").SetValue(true));
                _laneClearMenu.AddItem(new MenuItem("LW", "Use W").SetValue(true));
                _laneClearMenu.AddItem(new MenuItem("LM", "Minium Mana %").SetValue(new Slider(0, 50, 100)));
            }
            _JungleClearMenu = new LeagueSharp.Common.Menu("Jungleclear", "Jungleclear");
            {
                _JungleClearMenu.AddItem(new MenuItem("JQ", "Use Q").SetValue(true));
                _JungleClearMenu.AddItem(new MenuItem("JW", "Use W").SetValue(true));
                _JungleClearMenu.AddItem(new MenuItem("JE", "Use E").SetValue(true));
                _JungleClearMenu.AddItem(new MenuItem("JM", "Minium Mana %").SetValue(new Slider(0,50, 100)));
            }
            _farmMenu = new LeagueSharp.Common.Menu("Farm  Menu", "Farm Menu");
            {
                _farmMenu.AddSubMenu(_laneClearMenu);
                _farmMenu.AddSubMenu(_JungleClearMenu);
            }

            base.LoadLaneClearMenu();
        }
        public override void LoadkeyMenu()
        {
            _keyMenu = new LeagueSharp.Common.Menu("Keys", "Keys Menu");
            {
                _keyMenu.AddItem(new MenuItem("QEkey", "Q+E To Mouse Key").SetValue(new KeyBind('T', KeyBindType.Press)));
                _keyMenu.AddItem(new MenuItem("AUTOQE", "AUTO Q+E Stun target").SetValue(new KeyBind('X', KeyBindType.Press)));
                _keyMenu.AddItem(new MenuItem("HKey", "Harass Toggle").SetValue(new KeyBind('Z', KeyBindType.Toggle)));
                _keyMenu.AddItem(new MenuItem("RKey", "R to best target").SetValue(new KeyBind('R', KeyBindType.Press)));
            }
        }
        public override void CloseMenu()
        {
            GetMenu.AddSubMenu(_comboMenu);
            GetMenu.AddSubMenu(_harassMenu);
            GetMenu.AddSubMenu(_farmMenu);
            GetMenu.AddSubMenu(_targetsRMe);
            GetMenu.AddSubMenu(_dontRIfSpellReady);
            GetMenu.AddSubMenu(_keyMenu);
            GetMenu.AddSubMenu(_drawingMenu);
            base.CloseMenu();
        }
    }
}
