using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Azir_Creator_of_Elo
{
    class AzirMenu : Menu
    {
        private LeagueSharp.Common.Menu _drawSettingsMenu, _miscMenu,_jumpMenu,_comboMenu, _harashMenu, _laneClearMenu, _JungleClearMenu;
      
        public AzirMenu(String name,AzirMain  azir) : base(name)
        {
            LoadMenu(azir);
            CloseMenu();
        }
        public override void LoadMenu(AzirMain azir)
        {
            base.LoadMenu(azir);
            LoadLaneClearMenu();
            LoadHarashMenu();
            LoadComboMenu();
            LoadJungleClearMenu();
            LoadDrawings();
            LoadJumps();
            LoadMiscInterrupt(azir);
            LoadMiscMenu(azir);
        }

        public override void CloseMenu()
        {
            // add menus

            base.CloseMenu();
            base.GetMenu.AddSubMenu(_comboMenu);
            base.GetMenu.AddSubMenu(_harashMenu);
            base.GetMenu.AddSubMenu(_jumpMenu);
            base.GetMenu.AddSubMenu(_laneClearMenu);
            base.GetMenu.AddSubMenu(_JungleClearMenu);
            base.GetMenu.AddSubMenu(_miscMenu);
            base.GetMenu.AddSubMenu(_drawSettingsMenu);
            base.GetMenu.AddToMainMenu();

        }
        public void LoadDrawings()
        {
            _drawSettingsMenu = new LeagueSharp.Common.Menu("Drawings", "Draw Settings");
            {
                _drawSettingsMenu.AddItem(new MenuItem("dsl", "Draw Soldier Line").SetValue(true));
                _drawSettingsMenu.AddItem(new MenuItem("dcr", "Draw Control range").SetValue(true));
                _drawSettingsMenu.AddItem(new MenuItem("dfr", "Draw Flee range").SetValue(true));
            }
        }
        public override void LoadComboMenu()
        {
            _comboMenu = new LeagueSharp.Common.Menu("Combo", "Combo Menu");
            {
                _comboMenu.AddItem(new MenuItem("SoldiersToQ", "Soldiers to Q").SetValue(new Slider(1, 1, 3)));
                _comboMenu.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CW", "Use W").SetValue(true));
                _comboMenu.AddItem(new MenuItem("CR", "Use R killeable").SetValue(true));
            }
        }
        public void LoadLaneClearMenu()
        {
            _laneClearMenu = new LeagueSharp.Common.Menu("Laneclear", "Laneclear Menu");
            {
                _laneClearMenu.AddItem(new MenuItem("LQ", "Use Q").SetValue(true));
                _laneClearMenu.AddItem(new MenuItem("LW", "Use W").SetValue(true));
                _laneClearMenu.AddItem(new MenuItem("LWM", "Minions at W range to cast").SetValue(new Slider(3, 1, 6)));
                _laneClearMenu.AddItem(new MenuItem("LQM", "Soldiers to Q ").SetValue(new Slider(1, 1, 3)));

            }
        }
        public void LoadJungleClearMenu()
        {
            _JungleClearMenu = new LeagueSharp.Common.Menu("JungleClear", "JungleClear  Menu");
            {
                _JungleClearMenu.AddItem(new MenuItem("JW", "Use W").SetValue(true));
                _JungleClearMenu.AddItem(new MenuItem("JQ", "Use Q").SetValue(true));
            }
        }

        public void LoadMiscInterrupt(AzirMain azir)
        {
          
        }
        public void LoadMiscMenu(AzirMain azir)
        {
            List<String> spellsin = new List<string>();
            foreach (AIHeroClient hero in HeroManager.Enemies)
            {
                for (int i = 0; i < 4; i++)
                {
                    //  hero.GetSpell(Trans(i)).Name;
                    foreach (String s in azir.Interrupt)
                    {
                        if (s == hero.GetSpell(azir.Trans(i)).Name)
                        {
                            spellsin.Add("[" + hero.ChampionName + "]" + s);
                        }
                    }
                }
            }
            azir.InterruptSpell = spellsin;
            int num = 0;
            var interruptMenu = new LeagueSharp.Common.Menu("Spell Interrupt", "R Interrupt spells");
            {
                interruptMenu.AddItem(new MenuItem("UseRInterrupt", "Use R Interrupt").SetValue(true));
                foreach (String s in spellsin)
                {
                    interruptMenu.AddItem(new MenuItem("S" + num, s).SetValue(true));
                    num++;
                }
            }
            azir.InterruptNum = num;
            List<String> spellgap = new List<string>();
            foreach (AIHeroClient hero in HeroManager.Enemies)
            {
                for (int i = 0; i < 4; i++)
                {
                    foreach (String s in azir.Gapcloser)
                    {
                        if (s == hero.GetSpell(azir.Trans(i)).Name)
                        {
                            spellgap.Add("[" + hero.ChampionName + "]" + s);
                        }
                    }
                }
            }
            int numg = 0;
            azir.InterruptSpell = spellgap;
            var GapCloserMenu = new LeagueSharp.Common.Menu("Spell Gapcloser", "R to Gapcloser");
            {
                GapCloserMenu.AddItem(new MenuItem("UseRGapcloser", "Use R Gapcloser").SetValue(true));
                foreach (String s in spellgap)
                {
                    GapCloserMenu.AddItem(new MenuItem("G" + numg, s).SetValue(true));
                    numg++;
                }
            }
            numg = azir.GapcloserNum;
            _miscMenu = new LeagueSharp.Common.Menu("Misc", "Harash Menu");
            {
                _miscMenu.AddItem(new MenuItem("FMJ", "Max Range Jump Only").SetTooltip("Cast only jump to max range at flee").SetValue(true));
                _miscMenu.AddItem(new MenuItem("ARUT", "auto R under the Turret").SetTooltip("Automattly Cast R when enemy is near ally tower").SetValue(true));
                _miscMenu.AddSubMenu(interruptMenu);
                _miscMenu.AddSubMenu(GapCloserMenu);
            }
        }

        public void LoadHarashMenu()
        {
            _harashMenu = new LeagueSharp.Common.Menu("Harass", "Harass Menu");
            {
                _harashMenu.AddItem(new MenuItem("hSoldiersToQ", "Soldiers to Q").SetValue(new Slider(1, 1, 3)));
                _harashMenu.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
                _harashMenu.AddItem(new MenuItem("HW", "Use W").SetValue(true));
                _harashMenu.AddItem(new MenuItem("HW2", "Save on 1 w for flee").SetValue(true));
            }
        }
        public void LoadJumps()
        {
            _jumpMenu = new LeagueSharp.Common.Menu("Keys Menu", "Key Menu");
            {
              _jumpMenu.AddItem(new MenuItem("fleekey", "Jump key").SetValue(new KeyBind('Z', KeyBindType.Press)));
              _jumpMenu.AddItem(new MenuItem("inseckey", "Insec key").SetValue(new KeyBind('T', KeyBindType.Press)));
                _jumpMenu.AddItem(new MenuItem("insecPos", "Insec Pos").SetValue(new KeyBind('G', KeyBindType.Press)));

            }
        }
    }
    }
