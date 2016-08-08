using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PerplexedViktor
{
    class Config
    {
        public static Menu Settings = new Menu("Perplexed Viktor", "menu", true);

        public static Orbwalking.Orbwalker Orbwalker;

        public static void Initialize()
        {
            //Orbwalker
            {
                Menu orbMenu = new Menu("Orbwalker", "orbMenu");
                Orbwalker = new Orbwalking.Orbwalker(Settings.SubMenu("orbMenu"));
            }
            //Target Selector
            {
                Settings.AddSubMenu(new Menu("Target Selector", "ts"));
                TargetSelector.AddToMenu(Settings.SubMenu("ts"));
            }
            //Combo
            {
                Settings.AddSubMenu(new Menu("Combo", "menuCombo"));
                Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboQ", "Q").SetValue(true));
                Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboW", "W").SetValue(true));
                Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboE", "E").SetValue(true));
                Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboR", "R").SetValue(true));
                Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboRHit", "R if enemies >=").SetValue<Slider>(new Slider(3, 2, 5)));
                Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboIgnite", "Ignite").SetValue(true));
            }
            //Harass
            {
                Settings.AddSubMenu(new Menu("Harass", "menuHarass"));
                Settings.SubMenu("menuHarass").AddItem(new MenuItem("harassQ", "Q").SetValue(true));
                Settings.SubMenu("menuHarass").AddItem(new MenuItem("harassE", "E").SetValue(true));
                Settings.SubMenu("menuHarass").AddItem(new MenuItem("harassMana", "Mana % >=").SetValue<Slider>(new Slider(30, 1, 100)));
                Settings.SubMenu("menuHarass").AddItem(new MenuItem("toggleAuto", "Harass Automatically").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle, true)));
                //Auto Harass
                {
                    Settings.SubMenu("menuHarass").AddSubMenu(new Menu("Auto Settings", "autoSettings"));
                    Settings.SubMenu("menuHarass").SubMenu("autoSettings").AddSubMenu(new Menu("Champions", "autoChamps"));
                    foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsValid && hero.IsEnemy))
                        Settings.SubMenu("menuHarass").SubMenu("autoSettings").SubMenu("autoChamps").AddItem(new MenuItem("auto" + hero.ChampionName, hero.ChampionName).SetValue(false));
                    Settings.SubMenu("menuHarass").SubMenu("autoSettings").AddItem(new MenuItem("autoE", "E").SetValue(true));
                    Settings.SubMenu("menuHarass").SubMenu("autoSettings").AddItem(new MenuItem("autoTurret", "Harass Enemy Under Turret").SetValue<bool>(false));
                }
            }
            //Lane Clear
            {
                Settings.AddSubMenu(new Menu("Lane Clear", "menuClear"));
                Settings.SubMenu("menuClear").AddItem(new MenuItem("clearQ", "Q").SetValue(true));
                Settings.SubMenu("menuClear").AddItem(new MenuItem("clearE", "E").SetValue(true));
                Settings.SubMenu("menuClear").AddItem(new MenuItem("clearEHit", "E if Minions >=").SetValue<Slider>(new Slider(3, 1, 7)));
                Settings.SubMenu("menuClear").AddItem(new MenuItem("clearMana", "Mana % >=").SetValue<Slider>(new Slider(30, 1, 100)));
            }
            //Misc
            {
                Settings.AddSubMenu(new Menu("Misc", "menuMisc"));
                Settings.SubMenu("menuMisc").AddItem(new MenuItem("ultFollow", "Auto Follow (R)").SetValue(true));
                //Settings.SubMenu("menuMisc").AddItem(new MenuItem("followMode", "Follow Mode").SetValue(new StringList(new string[] { "Selected Target", "Lowest Health", "Multiple Targets" })));
                Settings.SubMenu("menuMisc").AddItem(new MenuItem("ultInterrupt", "Interrupt Spells (R)").SetValue(true));
                Settings.SubMenu("menuMisc").AddItem(new MenuItem("wInterrupt", "Interrupt Spells (W)").SetValue(true));
                Settings.SubMenu("menuMisc").AddItem(new MenuItem("wGapclose", "Anti-gapclose (W)").SetValue(true));
                Settings.SubMenu("menuMisc").AddItem(new MenuItem("wCC", "Use W on CC'd target").SetValue(true));
                Settings.SubMenu("menuMisc").AddItem(new MenuItem("ksE", "Killsteal (E)").SetValue(true));
            }
            //Drawing
            {
                Settings.AddSubMenu(new Menu("Drawing", "menuDrawing"));
                Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(new Circle(false, Color.Yellow)));
                Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawW", "Draw W Range").SetValue(new Circle(false, Color.Yellow)));
                Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawE", "Draw E Range").SetValue(new Circle(true, Color.Yellow)));
                Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawR", "Draw R Range").SetValue(new Circle(false, Color.Yellow)));
            }
            //Finish
            Settings.AddToMainMenu();
        }

        public static bool ComboQ { get { return Settings.Item("comboQ").GetValue<bool>(); } }
        public static bool ComboQChase { get { return Settings.Item("comboQAA").GetValue<bool>(); } }
        public static bool ComboW { get { return Settings.Item("comboW").GetValue<bool>(); } }
        public static bool ComboE { get { return Settings.Item("comboE").GetValue<bool>(); } }
        public static bool ComboR { get { return Settings.Item("comboR").GetValue<bool>(); } }
        public static int ComboRHit { get { return Settings.Item("comboRHit").GetValue<Slider>().Value; } }
        public static bool ComboIgnite { get { return Settings.Item("comboIgnite").GetValue<bool>(); } }

        public static bool HarassQ { get { return Settings.Item("harassQ").GetValue<bool>(); } }
        public static bool HarassE { get { return Settings.Item("harassE").GetValue<bool>(); } }
        public static int HarassMana { get { return Settings.Item("harassMana").GetValue<Slider>().Value; } }
        public static KeyBind ToggleAuto { get { return Settings.Item("toggleAuto").GetValue<KeyBind>(); } }
        public static bool ShouldAuto(string championName)
        {
            return Settings.Item("auto" + championName).GetValue<bool>();
        }
        public static bool AutoE { get { return Settings.Item("autoE").GetValue<bool>(); } }
        public static bool AutoTurret { get { return Settings.Item("autoTurret").GetValue<bool>(); } }

        public static bool ClearQ { get { return Settings.Item("clearQ").GetValue<bool>(); } }
        public static bool ClearE { get { return Settings.Item("clearE").GetValue<bool>(); } }
        public static int ClearEHit { get { return Settings.Item("clearEHit").GetValue<Slider>().Value; } }
        public static int ClearMana { get { return Settings.Item("clearMana").GetValue<Slider>().Value; } }

        public static bool AutoFollow { get { return Settings.Item("ultFollow").GetValue<bool>(); } }
        //public static StringList FollowMode { get { return Settings.Item("followMode").GetValue<StringList>(); } }
        public static bool InterruptR { get { return Settings.Item("ultInterrupt").GetValue<bool>(); } }
        public static bool InterruptW { get { return Settings.Item("wInterrupt").GetValue<bool>(); } }
        public static bool GapcloseW { get { return Settings.Item("wGapclose").GetValue<bool>(); } }
        public static bool WCC { get { return Settings.Item("wCC").GetValue<bool>(); } }
        public static bool KillstealE { get { return Settings.Item("ksE").GetValue<bool>(); } }

        public static bool DrawQ { get { return Settings.Item("drawQ").GetValue<Circle>().Active; } }
        public static bool DrawW { get { return Settings.Item("drawW").GetValue<Circle>().Active; } }
        public static bool DrawE { get { return Settings.Item("drawE").GetValue<Circle>().Active; } }
        public static bool DrawR { get { return Settings.Item("drawR").GetValue<Circle>().Active; } }
    }
}
