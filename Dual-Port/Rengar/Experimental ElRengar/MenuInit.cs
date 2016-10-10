using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengar
{
    using LeagueSharp.Common;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class MenuInit
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = new Menu("ElRengar EXP", "ElRengar", true);
            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Standards.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            TargetSelector.AddToMenu(TargetSelectorMenu());

            var comboMenu = Menu.AddSubMenu(new Menu("Modes", "Modes"));
            {
                comboMenu.SubMenu("Summoner spells")
                    .AddItem(new MenuItem("Combo.Use.Ignite", "Use Ignite").SetValue(true));

                comboMenu.SubMenu("Combo").AddItem(new MenuItem("Combo.Use.items", "Use Items").SetValue(true));
                comboMenu.SubMenu("Combo").AddItem(new MenuItem("Combo.Use.Q", "Use Q").SetValue(true));
                comboMenu.SubMenu("Combo").AddItem(new MenuItem("Combo.Use.W", "Use W").SetValue(true));
                comboMenu.SubMenu("Combo").AddItem(new MenuItem("Combo.Use.E", "Use E").SetValue(true));
                comboMenu.SubMenu("Combo")
                    .AddItem(new MenuItem("Combo.Switch.E", "Switch E prio to Q after E cast").SetValue(true));
                comboMenu.SubMenu("Combo")
                    .AddItem(
                        new MenuItem("Combo.Prio", "Prioritize").SetValue(new StringList(new[] { "E", "W", "Q" }, 2)));
                comboMenu.SubMenu("Combo")
                    .AddItem(
                        new MenuItem("Combo.Switch", "Switch priority").SetValue(
                            new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));

                comboMenu.SubMenu("Combo").AddItem(new MenuItem("Combo.Use.QQ", "5 ferocity Q reset").SetValue(true));


                comboMenu.SubMenu("Harass").AddItem(new MenuItem("Harass.Use.Q", "Use Q").SetValue(true));
                comboMenu.SubMenu("Harass").AddItem(new MenuItem("Harass.Use.W", "Use W").SetValue(true));
                comboMenu.SubMenu("Harass").AddItem(new MenuItem("Harass.Use.E", "Use E").SetValue(true));
                comboMenu.SubMenu("Harass")
                    .AddItem(new MenuItem("Harass.Prio", "Prioritize").SetValue(new StringList(new[] { "E", "Q" }, 1)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "clear"));
            {
                clearMenu.SubMenu("Laneclear").AddItem(new MenuItem("Clear.Use.Q", "Use Q").SetValue(true));
                clearMenu.SubMenu("Laneclear").AddItem(new MenuItem("Clear.Use.W", "Use W").SetValue(true));
                clearMenu.SubMenu("Laneclear").AddItem(new MenuItem("Clear.Use.E", "Use E").SetValue(true));
                clearMenu.SubMenu("Laneclear")
                    .AddItem(new MenuItem("Clear.Save.Ferocity", "Save ferocity").SetValue(false));

                clearMenu.SubMenu("Jungleclear").AddItem(new MenuItem("Jungle.Use.Q", "Use Q").SetValue(true));
                clearMenu.SubMenu("Jungleclear").AddItem(new MenuItem("Jungle.Use.W", "Use W").SetValue(true));
                clearMenu.SubMenu("Jungleclear").AddItem(new MenuItem("Jungle.Use.E", "Use E").SetValue(true));
                clearMenu.SubMenu("Jungleclear").AddItem(new MenuItem("Jungle.Save.Ferocity", "Save ferocity").SetValue(false));
            }

            var healMenu = Menu.AddSubMenu(new Menu("Heal", "heal"));
            {
                healMenu.AddItem(new MenuItem("Heal.AutoHeal", "Auto heal yourself").SetValue(true));
                healMenu.AddItem(new MenuItem("Heal.HP", "Self heal at >= ").SetValue(new Slider(25, 1, 100)));
            }

            var killstealMenu = Menu.AddSubMenu(new Menu("Killsteal", "Killsteal"));
            {
                killstealMenu.AddItem(new MenuItem("Killsteal.On", "Active").SetValue(true));
                killstealMenu.AddItem(new MenuItem("Killsteal.Use.W", "Use W").SetValue(true));
            }

            var betaMenu = Menu.AddSubMenu(new Menu("Beta options", "BetaOptions"));
            {
                betaMenu.AddItem(new MenuItem("Beta.Cast.Q1", "Use beta Q").SetValue(true));
                betaMenu.AddItem(
                    new MenuItem("Beta.Cast.Q1.Delay", "Cast Q delay").SetValue(new Slider(300, 100, 2000)));
                betaMenu.AddItem(new MenuItem("Assassin.searchrange", "Assassin search range"));

                betaMenu.AddItem(
                    new MenuItem("Beta.searchrange", "Search range").SetValue(new Slider(1500, 1000, 2500)));

                betaMenu.AddItem(
                    new MenuItem("Beta.searchrange.Q", "Q cast range").SetValue(new Slider(600, 500, 1500)));

                betaMenu.AddItem(new MenuItem("Beta.Search.Range", "Draw search range").SetValue(new Circle()));
                betaMenu.AddItem(new MenuItem("Beta.Search.QCastRange", "Draw Q cast range").SetValue(new Circle()));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(new MenuItem("Misc.Drawings.Off", "Turn drawings off").SetValue(false));
                miscMenu.AddItem(
                    new MenuItem("Misc.Drawings.Exclamation", "Draw exclamation mark range").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("Misc.Drawings.Prioritized", "Draw Prioritized").SetValue(true));
                miscMenu.AddItem(new MenuItem("Misc.Drawings.W", "Draw W").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("Misc.Drawings.E", "Draw E").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("Misc.Drawings.Minimap", "Draw R on minimap").SetValue(true));

                miscMenu.AddItem(new MenuItem("Misc.Root", "Auto E on stunned targets").SetValue(false));
            }

            var credits = Menu.AddSubMenu(new Menu("Credits", "jQuery"));
            {
                credits.AddItem(new MenuItem("Paypal", "if you would like to donate via paypal:"));
                credits.AddItem(new MenuItem("Email", "info@zavox.nl"));
            }

            Menu.AddItem(new MenuItem("sep1", ""));
            Menu.AddItem(new MenuItem("sep2", $"Version: {Standards.ScriptVersion}"));
            Menu.AddItem(new MenuItem("sep3", "Made By jQuery"));

            Menu.AddToMainMenu();
        }

        #endregion

        #region Methods

        private static Menu TargetSelectorMenu()
        {
            return Menu.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
        }

        #endregion
    }
}