using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class MenuConfig : Core.Core
    {
        public static Menu Config;
        public static Menu TargetSelectorMenu;
        public static string MenuName = "Dark Star Thresh";

        public static void LoadMenu()
        {
            Config = new Menu(MenuName, MenuName, true);
          
            var orbwalker = new Menu("Orbwalker", "rorb");
            Orbwalker = new Orbwalking.Orbwalker(orbwalker);
            Config.AddSubMenu(orbwalker);

            var spell = new Menu("Q Spell", "Spell");
            spell.AddItem(new MenuItem("blank", "F8 To Reload Values"));
            spell.AddItem(new MenuItem("Q2", "Use Q2").SetValue(new KeyBind('G', KeyBindType.Toggle)));
            spell.AddItem(new MenuItem("Prediction", "Prediction").SetValue(new StringList(new[] { "OKTW", "Common" })));
            spell.AddItem(new MenuItem("Hitchance", "Hitchance").SetValue(new StringList(new[] { "High", "Very High" })));
            spell.AddItem(new MenuItem("Speed", "Speed").SetValue(new Slider(1500, 1000, 1750)));
            spell.AddItem(new MenuItem("Range", "Range").SetValue(new Slider(1100, 0, 1100)));
            spell.AddItem(new MenuItem("Width", "Width").SetValue(new Slider(60, 0, 80)));
            Config.AddSubMenu(spell);

            var blackList = new Menu("Blacklist", "Blacklist");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
            {
                blackList.AddItem(new MenuItem("blacklist" + enemy.CharData.BaseSkinName, $"{enemy.CharData.BaseSkinName}").SetValue(false));
            }
             Config.AddSubMenu(blackList);

            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("ComboFlash", "Flash Combo").SetValue(new KeyBind('T', KeyBindType.Press)));
            combo.AddItem(new MenuItem("ComboR", "Enemies For R").SetValue(new Slider(3, 0, 5)));
            combo.AddItem(new MenuItem("ComboQ", "Max Q Range").SetValue(new Slider(1100, 0, 1100)));
            combo.AddItem(new MenuItem("ComboTaxi", "Taxi Mode").SetValue(true).SetTooltip("Will Cast Q To Minions near Enemies"));
            combo.AddItem(new MenuItem("WJungler", "Auto W To Ally Jungler").SetValue(true).SetTooltip("W To Jungler"));
            combo.AddItem(new MenuItem("ESmart", "Smart E").SetValue(true).SetTooltip("No E On CC'd targets"));
            Config.AddSubMenu(combo);

            var harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HarassAA", "Disable AA In Harass").SetValue(false));
            harass.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HarassE", "Use E").SetValue(true));
            Config.AddSubMenu(harass);

            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("AutoCC", "Auto Q On CC").SetValue(true));
            misc.AddItem(new MenuItem("AutoDashing", "Auto Q On Dashing").SetValue(true));
            misc.AddItem(new MenuItem("Interrupt", "Interrupter").SetValue(true));
            misc.AddItem(new MenuItem("Gapcloser", "Gapcloser").SetValue(true));
            misc.AddItem(new MenuItem("UseSkin", "Use Skinchanger").SetValue(false));
            misc.AddItem(new MenuItem("Skin", "Skin").SetValue(new StringList(new[] { "Default", "Deep Terror Thresh", "Championship Thresh", "Blood Moon Thresh", "SSW Thresh", "Dark Star Thresh" })));
            misc.AddItem(new MenuItem("Flee", "Flee").SetValue(new KeyBind('A', KeyBindType.Press))).SetTooltip("Flee To Minion / Mobs");
            Config.AddSubMenu(misc);


            var draw = new Menu("Draw", "Draw");
            draw.AddItem(new MenuItem("DrawDmg", "Draw Damage").SetValue(true));
            draw.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            draw.AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(true));
            draw.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(true));
            draw.AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(true));
            Config.AddSubMenu(draw);

            Config.AddItem(new MenuItem("Debug", "Debug Mode").SetValue(false));

            Config.AddToMainMenu();
        }



        // Keybind
        public static bool ComboFlash => Config.Item("ComboFlash").GetValue<KeyBind>().Active;
        public static bool Flee => Config.Item("Flee").GetValue<KeyBind>().Active;

        // Slider
        public static int ComboR => Config.Item("ComboR").GetValue<Slider>().Value;
        public static int ComboQ => Config.Item("ComboQ").GetValue<Slider>().Value;

        public static int Speed => Config.Item("Speed").GetValue<Slider>().Value;
        public static int Range => Config.Item("Range").GetValue<Slider>().Value;
        public static int Width => Config.Item("Width").GetValue<Slider>().Value;

        public static bool Q2 => Config.Item("Q2").GetValue<KeyBind>().Active;


        // Array
        public static StringList PredMode => Config.Item("Prediction").GetValue<StringList>();

        // Bool
        public static bool ComboTaxi => Config.Item("ComboTaxi").GetValue<bool>();

        public static bool HarassAa => Config.Item("HarassAA").GetValue<bool>();
        public static bool HarassQ => Config.Item("HarassQ").GetValue<bool>();
        public static bool HarassE => Config.Item("HarassE").GetValue<bool>();

        public static bool ESmart => Config.Item("ESmart").GetValue<bool>();

        public static StringList Hitchance => Config.Item("Hitchance").GetValue<StringList>();

        public static bool WJungler => Config.Item("WJungler").GetValue<bool>();
        public static bool AutoCC => Config.Item("AutoCC").GetValue<bool>();
        public static bool AutoDashing => Config.Item("AutoDashing").GetValue<bool>();
        public static bool Interrupt => Config.Item("Interrupt").GetValue<bool>();
        public static bool Gapcloser => Config.Item("Gapcloser").GetValue<bool>();

        public static bool UseSkin => Config.Item("UseSkin").GetValue<bool>();

        public static bool DrawDmg => Config.Item("DrawDmg").GetValue<bool>();
        public static bool DrawQ => Config.Item("DrawQ").GetValue<bool>();
        public static bool DrawW => Config.Item("DrawW").GetValue<bool>();
        public static bool DrawE => Config.Item("DrawE").GetValue<bool>();
        public static bool DrawR => Config.Item("DrawR").GetValue<bool>();

        public static bool Debug => Config.Item("Debug").GetValue<bool>();
    }
}
