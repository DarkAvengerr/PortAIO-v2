using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh
{
    class MenuConfig : Core.Core
    {
        public static Menu Config;
        public static Menu TargetSelectorMenu;
        public static string menuName = "Dark Star Thresh";

        public static void LoadMenu()
        {
            Config = new Menu(menuName, menuName, true);
            #region TargetSelector & Orbwalker
            TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Config.AddSubMenu(TargetSelectorMenu);

            var orbwalker = new Menu("Orbwalker", "rorb");
            _orbwalker = new Orbwalking.Orbwalker(orbwalker);
            Config.AddSubMenu(orbwalker);
            #endregion

            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("ComboFlash", "Flash Combo").SetValue(new KeyBind('T', KeyBindType.Press))).SetTooltip("Does Flash Combo");
            combo.AddItem(new MenuItem("ComboR", "Min Enemies For R").SetValue(new Slider(3, 0, 5)));
            combo.AddItem(new MenuItem("ComboQ", "Max Q Range").SetValue(new Slider(110, 0, 110)));
            combo.AddItem(new MenuItem("ComboTaxi", "Taxi Mode (Beta!)").SetValue(true).SetTooltip("Will Cast Q To Minions, Logic implented ofc."));
            Config.AddSubMenu(combo);

            var Harass = new Menu("Harass", "Harass");
            Harass.AddItem(new MenuItem("HarassAA", "Disable AA In Harass").SetValue(false).SetTooltip("Wont Use AA"));
            Harass.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true).SetTooltip("Wont cast Q2"));
            Harass.AddItem(new MenuItem("HarassE", "Use E").SetValue(true).SetTooltip("Throws the target away from you"));
            Config.AddSubMenu(Harass);

            var Misc = new Menu("Misc", "Misc");
            Misc.AddItem(new MenuItem("Interrupt", "Interrupter").SetValue(true));
            Misc.AddItem(new MenuItem("Gapcloser", "Gapcloser").SetValue(true));
            Misc.AddItem(new MenuItem("UseSkin", "Use Skinchanger").SetValue(false));
            Misc.AddItem(new MenuItem("Skin", "Skin").SetValue(new StringList(new[] { "Default", "Deep Terror Thresh", "Championship Thresh", "Blood Moon Thresh", "SSW Thresh", "Dark Star Thresh" })));
            Misc.AddItem(new MenuItem("Flee", "Flee").SetValue(new KeyBind('A', KeyBindType.Press))).SetTooltip("Flee To Minion / Mobs");
            Config.AddSubMenu(Misc);


            var Draw = new Menu("Draw", "Draw");
            Draw.AddItem(new MenuItem("DrawDmg", "Draw Damage").SetValue(true).SetTooltip("Somewhat Fps Heavy, Be Careful"));
            Draw.AddItem(new MenuItem("DrawPred", "Draw Q Prediction").SetValue(true));
            Draw.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            Draw.AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(true));
            Draw.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(true));
            Draw.AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(true));
            Config.AddSubMenu(Draw);

            Config.AddItem(new MenuItem("Debug", "Debug Mode").SetValue(false).SetTooltip("Prints In Chat What's Going On"));

            Config.AddToMainMenu();
        }



        // Keybind
        public static bool ComboFlash => Config.Item("ComboFlash").GetValue<KeyBind>().Active;
        public static bool Flee => Config.Item("Flee").GetValue<KeyBind>().Active;

        // Slider
        public static int ComboR => Config.Item("ComboR").GetValue<Slider>().Value;
        public static int ComboQ => Config.Item("ComboQ").GetValue<Slider>().Value;

        // Bool
        public static bool ComboTaxi => Config.Item("ComboTaxi").GetValue<bool>();

        public static bool HarassAA => Config.Item("HarassAA").GetValue<bool>();
        public static bool HarassQ => Config.Item("HarassQ").GetValue<bool>();
        public static bool HarassE => Config.Item("HarassE").GetValue<bool>();

        public static bool Interrupt => Config.Item("Interrupt").GetValue<bool>();
        public static bool Gapcloser => Config.Item("Gapcloser").GetValue<bool>();

        public static bool UseSkin => Config.Item("UseSkin").GetValue<bool>();

        public static bool DrawDmg => Config.Item("DrawDmg").GetValue<bool>();
        public static bool DrawPred => Config.Item("DrawPred").GetValue<bool>();
        public static bool DrawQ => Config.Item("DrawQ").GetValue<bool>();
        public static bool DrawW => Config.Item("DrawW").GetValue<bool>();
        public static bool DrawE => Config.Item("DrawE").GetValue<bool>();
        public static bool DrawR => Config.Item("DrawR").GetValue<bool>();

        public static bool Debug => Config.Item("Debug").GetValue<bool>();
    }
}
