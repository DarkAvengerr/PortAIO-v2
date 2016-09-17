using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PerplexedEzreal
{
    class Config
    {
        public static Menu Settings = new Menu("Perplexed Ezreal", "menu", true);

        public static string[] Marksmen = { "Kalista", "Jinx", "Lucian", "Quinn", "Draven",  "Varus", "Graves", "Vayne", "Caitlyn",
                                                                    "Urgot", "Ezreal", "KogMaw", "Ashe", "MissFortune", "Tristana", "Teemo", "Sivir",
                                                                    "Twitch", "Corki"};

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
                Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboR", "R").SetValue(true));
                Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboRHit", "Cast R if enemies >=").SetValue(new Slider(3, 2, 5)));
            }
            //Harass
            {
                Settings.AddSubMenu(new Menu("Harass", "menuHarass"));
                Settings.SubMenu("menuHarass").AddItem(new MenuItem("harassQ", "Q").SetValue(true));
                Settings.SubMenu("menuHarass").AddItem(new MenuItem("harassW", "W").SetValue(true));
                Settings.SubMenu("menuHarass").AddItem(new MenuItem("harassMana", "Mana % >=").SetValue(new Slider(30, 1, 99)));
                //Auto Harass
                {
                    Settings.AddSubMenu(new Menu("Auto Harass", "menuAuto"));
                    Settings.SubMenu("menuAuto").AddItem(new MenuItem("toggleAuto", "Toggle Auto").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle, true)));
                    Settings.SubMenu("menuAuto").AddSubMenu(new Menu("Champions", "autoChamps"));
                    foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsValid && hero.IsEnemy))
                        Settings.SubMenu("menuAuto").SubMenu("autoChamps").AddItem(new MenuItem("auto" + hero.ChampionName, hero.ChampionName).SetValue(Marksmen.Contains(hero.ChampionName)));
                    Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoQ", "Q").SetValue(true));
                    Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoW", "W").SetValue(false));
                    Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoMana", "Mana % >= ").SetValue(new Slider(30, 1, 99)));
                    Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoTurret", "Harass Enemy Under Turret").SetValue(false));
                }
            }
            //Last Hit
            {
                Settings.AddSubMenu(new Menu("Last Hitting", "menuLastHit"));
                Settings.SubMenu("menuLastHit").AddItem(new MenuItem("lastHitQ", "Q").SetValue(true));
            }
            //Lane Clear
            {
                Settings.AddSubMenu(new Menu("Lane Clear", "menuLaneClear"));
                Settings.SubMenu("menuLaneClear").AddItem(new MenuItem("laneClearQ", "Q").SetValue(true));
                Settings.SubMenu("menuLaneClear").AddItem(new MenuItem("clearMana", "Mana % >= ").SetValue(new Slider(30, 1, 99)));
            }
            //Anti-Gapcloser
            {
                Settings.AddSubMenu(new Menu("Anti-Gapcloser", "menuGapCloser"));
                Settings.SubMenu("menuGapCloser").AddItem(new MenuItem("gapcloseE", "Dodge With E").SetValue(true));
            }
            //Ultimate
            {
                Settings.AddSubMenu(new Menu("Ult Settings", "menuUlt"));
                Settings.SubMenu("menuUlt").AddItem(new MenuItem("ultLowest", "Ult Lowest Target").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                Settings.SubMenu("menuUlt").AddItem(new MenuItem("ks", "Kill Steal With R").SetValue(true));
                Settings.SubMenu("menuUlt").AddItem(new MenuItem("ultMinRange", "Min. Range to Ult").SetValue(new Slider(550, 550, 5000)));
                Settings.SubMenu("menuUlt").AddItem(new MenuItem("ultMaxRange", "Max. Range to Ult").SetValue(new Slider(1000, 600, 5000)));
            }
            //Drawing
            {
                Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(new Circle(true, Color.Yellow)));
                Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawW", "Draw W Range").SetValue(new Circle(true, Color.Yellow)));
                Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawMinR", "Draw Min. R Range").SetValue(new Circle(true, Color.Yellow)));
                Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawMaxR", "Draw Max. R Range").SetValue(new Circle(true, Color.Yellow)));
            }
            //Finish
            {
                Settings.AddToMainMenu();
            }
        }

        public static bool ComboQ { get { return Settings.Item("comboQ").GetValue<bool>(); } }
        public static bool ComboW { get { return Settings.Item("comboW").GetValue<bool>(); } }
        public static bool ComboR { get { return Settings.Item("comboR").GetValue<bool>(); } }
        public static int ComboRHit { get { return Settings.Item("comboRHit").GetValue<Slider>().Value; } }

        public static bool HarassQ { get { return Settings.Item("harassQ").GetValue<bool>(); } }
        public static bool HarassW { get { return Settings.Item("harassW").GetValue<bool>(); } }
        public static int HarassMana { get { return Settings.Item("harassMana").GetValue<Slider>().Value; } }

        public static KeyBind ToggleAuto { get { return Settings.Item("toggleAuto").GetValue<KeyBind>(); } }
        public static bool ShouldAuto(string championName)
        {
            return Settings.Item("auto" + championName).GetValue<bool>();
        }
        public static bool AutoQ { get { return Settings.Item("autoQ").GetValue<bool>(); } }
        public static bool AutoW { get { return Settings.Item("autoW").GetValue<bool>(); } }
        public static int AutoMana { get { return Settings.Item("autoMana").GetValue<Slider>().Value; } }
        public static bool AutoTurret { get { return Settings.Item("autoTurret").GetValue<bool>(); } }

        public static bool LastHitQ { get { return Settings.Item("lastHitQ").GetValue<bool>(); } }

        public static bool LaneClearQ { get { return Settings.Item("laneClearQ").GetValue<bool>(); } }
        public static int ClearMana { get { return Settings.Item("clearMana").GetValue<Slider>().Value; } }

        public static bool GapcloseE { get { return Settings.Item("gapcloseE").GetValue<bool>(); } }

        public static KeyBind UltLowest { get { return Settings.Item("ultLowest").GetValue<KeyBind>(); } }
        public static bool Killsteal { get { return Settings.Item("ks").GetValue<bool>(); } }
        public static int UltMinRange { get { return Settings.Item("ultMinRange").GetValue<Slider>().Value; } }
        public static int UltMaxRange { get { return Settings.Item("ultMaxRange").GetValue<Slider>().Value; } }

        public static bool DrawQ { get { return Settings.Item("drawQ").GetValue<Circle>().Active; } }
        public static bool DrawW { get { return Settings.Item("drawW").GetValue<Circle>().Active; } }
        public static bool DrawMinR { get { return Settings.Item("drawMinR").GetValue<Circle>().Active; } }
        public static bool DrawMaxR { get { return Settings.Item("drawMinR").GetValue<Circle>().Active; } }
    }
}
