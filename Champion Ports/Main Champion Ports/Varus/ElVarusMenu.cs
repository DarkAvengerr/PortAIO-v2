namespace Elvarus
{
    using System;
    using System.Drawing;

    using LeagueSharp.Common;

    public class ElVarusMenu
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = new Menu("ElVarus", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Varus.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);

            var cMenu = new Menu("Combo", "Combo");

            cMenu.AddItem(new MenuItem("ElVarus.Combo.Q", "Use Q").SetValue(true));
            cMenu.AddItem(new MenuItem("ElVarus.combo.always.Q", "always Q").SetValue(false));
            cMenu.AddItem(new MenuItem("ElVarus.Combo.E", "Use E").SetValue(true));
            cMenu.AddItem(new MenuItem("ElVarus.Combo.R", "Use R").SetValue(true));
            cMenu.AddItem(new MenuItem("ElVarus.Combo.W.Focus", "Focus W target").SetValue(false));
            cMenu.AddItem(new MenuItem("ElVarus.sssss", ""));
            cMenu.AddItem(new MenuItem("ElVarus.Combo.R.Count", "R when enemies >= ")).SetValue(new Slider(1, 1, 5));
            cMenu.AddItem(new MenuItem("ElVarus.Combo.Stack.Count", "Q when stacks >= ")).SetValue(new Slider(3, 1, 3));
            cMenu.AddItem(new MenuItem("ElVarus.sssssssss", ""));
            cMenu.AddItem(
                new MenuItem("ElVarus.SemiR", "Semi-manual cast R key").SetValue(
                    new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            cMenu.AddItem(new MenuItem("ElVarus.ssssssssssss", ""));
            cMenu.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
            Menu.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "Harass");
            hMenu.AddItem(new MenuItem("ElVarus.Harass.Q", "Use Q").SetValue(true));
            hMenu.AddItem(new MenuItem("ElVarus.Harass.E", "Use E").SetValue(true));
            hMenu.AddItem(new MenuItem("ElVarus.Harasssfsass.E", ""));
            hMenu.AddItem(new MenuItem("minmanaharass", "Mana needed to clear ")).SetValue(new Slider(55));

            Menu.AddSubMenu(hMenu);

            var itemMenu = new Menu("Items", "Items");
            itemMenu.AddItem(new MenuItem("ElVarus.Items.Youmuu", "Use Youmuu's Ghostblade").SetValue(true));
            itemMenu.AddItem(new MenuItem("ElVarus.Items.Cutlass", "Use Cutlass").SetValue(true));
            itemMenu.AddItem(new MenuItem("ElVarus.Items.Blade", "Use Blade of the Ruined King").SetValue(true));
            itemMenu.AddItem(new MenuItem("ElVarus.Harasssfsddass.E", ""));
            itemMenu.AddItem(
                new MenuItem("ElVarus.Items.Blade.EnemyEHP", "Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            itemMenu.AddItem(
                new MenuItem("ElVarus.Items.Blade.EnemyMHP", "My HP Percentage").SetValue(new Slider(80, 100, 0)));

            Menu.AddSubMenu(itemMenu);

            var lMenu = new Menu("Clear", "Clear");
            lMenu.AddItem(new MenuItem("useQFarm", "Use Q").SetValue(true));
            lMenu.AddItem(
                new MenuItem("ElVarus.Count.Minions", "Killable minions with Q >=").SetValue(new Slider(2, 1, 5)));
            lMenu.AddItem(new MenuItem("useEFarm", "Use E").SetValue(true));
            lMenu.AddItem(
                new MenuItem("ElVarus.Count.Minions.E", "Killable minions with E >=").SetValue(new Slider(2, 1, 5)));
            lMenu.AddItem(new MenuItem("useEFarmddsddaadsd", ""));
            lMenu.AddItem(new MenuItem("useQFarmJungle", "Use Q in jungle").SetValue(true));
            lMenu.AddItem(new MenuItem("useEFarmJungle", "Use E in jungle").SetValue(true));
            lMenu.AddItem(new MenuItem("useEFarmddssd", ""));
            lMenu.AddItem(new MenuItem("minmanaclear", "Mana needed to clear ")).SetValue(new Slider(55));

            Menu.AddSubMenu(lMenu);

            //ElSinged.Misc
            var miscMenu = new Menu("Misc", "Misc");
            miscMenu.AddItem(new MenuItem("ElVarus.Draw.off", "Turn drawings off").SetValue(true));
            miscMenu.AddItem(new MenuItem("ElVarus.Draw.Q", "Draw Q").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElVarus.Draw.W", "Draw W").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElVarus.Draw.E", "Draw E").SetValue(new Circle()));

            miscMenu.AddItem(new MenuItem("ElVarus.KSSS", "Killsteal").SetValue(true));

            Menu.AddSubMenu(miscMenu);

            //Here comes the moneyyy, money, money, moneyyyy
            var credits = new Menu("Credits", "jQuery");
            credits.AddItem(new MenuItem("ElSinged.Paypal", "if you would like to donate via paypal:"));
            credits.AddItem(new MenuItem("ElSinged.Email", "info@zavox.nl"));
            Menu.AddSubMenu(credits);

            Menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            Menu.AddItem(new MenuItem("422442fsaafsf", "Version: 1.0.2.2"));
            Menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            Menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}