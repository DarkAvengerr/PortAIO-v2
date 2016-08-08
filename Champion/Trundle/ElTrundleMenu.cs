namespace ElTrundle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    public class ElTrundleMenu
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = new Menu("ElTrundle", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Trundle.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ElTrundle.Combo.Q", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElTrundle.Combo.W", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElTrundle.Combo.E", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElTrundle.Combo.R", "Use R").SetValue(true));
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
                {
                    comboMenu.SubMenu("Use R on")
                        .AddItem(
                            new MenuItem("ElTrundle.R.On" + hero.CharData.BaseSkinName, hero.CharData.BaseSkinName)
                                .SetValue(true));
                }

                comboMenu.AddItem(new MenuItem("ElTrundle.Combo.Ignite", "Use Ignite").SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("ElTrundle.Harass.Q", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("ElTrundle.Harass.W", "Use W").SetValue(true));
                harassMenu.AddItem(new MenuItem("ElTrundle.Harass.E", "Use E").SetValue(false));
                harassMenu.AddItem(new MenuItem("ElTrundle.Harass.Mana", "Minimum mana")).SetValue(new Slider(25));
            }

            var laneClearMenu = Menu.AddSubMenu(new Menu("Laneclear", "Laneclear"));
            {
                laneClearMenu.AddItem(new MenuItem("ElTrundle.LaneClear.Q", "Use Q").SetValue(true));

                laneClearMenu.AddItem(
                    new MenuItem("ElTrundle.LaneClear.Q.Lasthit", "Only lasthit with Q").SetValue(false));
                laneClearMenu.AddItem(new MenuItem("ElTrundle.LaneClear.W", "Use W").SetValue(true));
                laneClearMenu.AddItem(new MenuItem("ElTrundle.LaneClear.Mana", "Minimum mana")).SetValue(new Slider(25));
            }

            var jungleClearMenu = Menu.AddSubMenu(new Menu("Jungleclear", "Jungleclear"));
            {
                jungleClearMenu.AddItem(new MenuItem("ElTrundle.JungleClear.Q", "Use Q").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("ElTrundle.JungleClear.W", "Use W").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("ElTrundle.JungleClear.Mana", "Minimum mana"))
                    .SetValue(new Slider(25));
            }

            var itemMenu = Menu.AddSubMenu(new Menu("Items", "Items"));
            {
                itemMenu.AddItem(new MenuItem("ElTrundle.Items.Hydra", "Use Ravenous Hydra").SetValue(true));
                itemMenu.AddItem(new MenuItem("ElTrundle.Items.Titanic", "Use Titanic Hydra").SetValue(true));
                itemMenu.AddItem(new MenuItem("ElTrundle.Items.Youmuu", "Use Youmuu's Ghostblade").SetValue(true));
                itemMenu.AddItem(new MenuItem("ElTrundle.Items.Blade", "Use Blade of the Ruined King").SetValue(true));
                itemMenu.AddItem(
                    new MenuItem("ElTrundle.Items.Blade.EnemyEHP", "Enemy HP Percentage").SetValue(
                        new Slider(80, 100, 0)));
                itemMenu.AddItem(
                    new MenuItem("ElTrundle.Items.Blade.EnemyMHP", "My HP Percentage").SetValue(new Slider(80, 100, 0)));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(new MenuItem("ElTrundle.Draw.off", "Turn drawings off").SetValue(true));
                miscMenu.AddItem(new MenuItem("ElTrundle.Draw.Q", "Draw Q").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElTrundle.Draw.W", "Draw W").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElTrundle.Draw.E", "Draw E").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElTrundle.Draw.R", "Draw R").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElTrundle.Antigapcloser", "Antigapcloser").SetValue(false));
                miscMenu.AddItem(new MenuItem("ElTrundle.Interrupter", "Interrupter").SetValue(false));
            }

            var credits = Menu.AddSubMenu(new Menu("Credits", "jQuery"));
            {
                credits.AddItem(new MenuItem("ElTrundle.Paypal", "if you would like to donate via paypal:"));
                credits.AddItem(new MenuItem("ElTrundle.Email", "info@zavox.nl"));
            }

            Menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            Menu.AddItem(
                new MenuItem("422442fsaafsf", (string.Format("ElTrundle by jQuery v{0}", Trundle.ScriptVersion))));
            Menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            Menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}