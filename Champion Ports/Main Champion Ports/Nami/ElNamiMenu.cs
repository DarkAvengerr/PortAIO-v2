namespace ElNamiBurrito
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    public class ElNamiMenu
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = new Menu("ElNamiReborn", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            {
                Nami.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            }
            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            {
                TargetSelector.AddToMenu(targetSelector);
            }
            Menu.AddSubMenu(targetSelector);

            var comboMenu = new Menu("Combo", "Combo");
            {
                comboMenu.AddItem(new MenuItem("ElNamiReborn.Combo.Q", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElNamiReborn.Combo.W", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElNamiReborn.Combo.E", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElNamiReborn.Combo.R", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElNamiReborn.Combo.R.Count", "Minimum targets R"))
                    .SetValue(new Slider(3, 1, 5));
                comboMenu.AddItem(new MenuItem("ElNamiReborn.Combo.Ignite", "Use ignite").SetValue(true));
            }

            Menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass", "Harass");
            {
                harassMenu.AddItem(new MenuItem("ElNamiReborn.Harass.Q", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("ElNamiReborn.Harass.W", "Use W").SetValue(true));
                harassMenu.AddItem(new MenuItem("ElNamiReborn.Harass.E", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("ElNamiReborn.Harass.Mana", "Minimum mana for harass"))
                    .SetValue(new Slider(55));
            }
            Menu.AddSubMenu(harassMenu);

            var castEMenu = Menu.AddSubMenu(new Menu("E settings", "ESettings"));
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(champ => champ.IsAlly))
            {
                castEMenu.AddItem(
                    new MenuItem(
                        "ElNamiReborn.Settings.E1" + ally.BaseSkinName,
                        string.Format("Cast E: {0}", ally.BaseSkinName)).SetValue(true));
            }

            var healMenu = new Menu("Heal settings", "HealSettings");
            {
                healMenu.AddItem(new MenuItem("ElNamiReborn.Heal.Activate", "Use heal").SetValue(true));
                healMenu.AddItem(
                    new MenuItem("ElNamiReborn.Heal.Player.HP", "HP percentage").SetValue(new Slider(25, 1, 100)));
                healMenu.AddItem(new MenuItem("ElNamiReborn.Heal.Ally.HP", "Use heal on ally's").SetValue(true));
                healMenu.AddItem(
                    new MenuItem("ElNamiReborn.Heal.Ally.HP.Percentage", "HP percentage ally's").SetValue(
                        new Slider(25, 1, 100)));
                healMenu.AddItem(new MenuItem("ElNamiReborn.Heal.Mana", "Mininum mana needed")).SetValue(new Slider(55));
            }

            Menu.AddSubMenu(healMenu);

            var interuptMenu = new Menu("Interupt settings", "interuptsettings");
            {
                interuptMenu.AddItem(new MenuItem("ElNamiReborn.Interupt.Q", "Use Q").SetValue(true));
                interuptMenu.AddItem(new MenuItem("ElNamiReborn.Interupt.R", "Use R").SetValue(false));
            }

            Menu.AddSubMenu(interuptMenu);

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddItem(new MenuItem("ElNamiReborn.Draw.off", "Turn drawings off").SetValue(false));
                miscMenu.AddItem(new MenuItem("ElNamiReborn.Draw.Q", "Draw Q").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElNamiReborn.Draw.W", "Draw W").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElNamiReborn.Draw.E", "Draw E").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElNamiReborn.Draw.R", "Draw R").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElNamiReborn.Draw.Text", "Draw Text").SetValue(true));

                miscMenu.AddItem(new MenuItem("sep1", ""));
                miscMenu.AddItem(new MenuItem("ElNamiReborn.misc.ks", "Killsteal mode").SetValue(false));
            }

            Menu.AddSubMenu(miscMenu);

            var credits = new Menu("Credits", "jQuery");
            {
                credits.AddItem(new MenuItem("ElNamiReborn.Paypal", "if you would like to donate via paypal:"));
                credits.AddItem(new MenuItem("ElNamiReborn.Email", "info@zavox.nl"));
            }
            Menu.AddSubMenu(credits);

            Menu.AddItem(new MenuItem("sep2", ""));
            Menu.AddItem(new MenuItem("sep3", "Version: 1.0.0.2"));
            Menu.AddItem(new MenuItem("sep4", "Made By jQuery"));

            Menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}