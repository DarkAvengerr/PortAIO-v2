namespace ElCorki
{
    using System;

    using LeagueSharp.Common;

    public class ElCorkiMenu
    {
        #region Static Fields

        public static Menu _menu;

        #endregion

        #region Public Properties

        public static string ScriptVersion
        {
            get
            {
                return typeof(Corki).Assembly.GetName().Version.ToString();
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            _menu = new Menu("ElCorki", "menu", true);

            //ElCorki.Orbwalker
            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Corki.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            _menu.AddSubMenu(orbwalkerMenu);

            //ElCorki.TargetSelector
            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);

            _menu.AddSubMenu(targetSelector);

            var cMenu = new Menu("Combo", "Combo");
            cMenu.AddItem(new MenuItem("ElCorki.Combo.Q", "Use Q").SetValue(true));
            cMenu.AddItem(new MenuItem("ElCorki.Combo.E", "Use E").SetValue(true));
            cMenu.AddItem(new MenuItem("ElCorki.Combo.R", "Use R").SetValue(true));
            cMenu.AddItem(new MenuItem("ElCorki.Combo.Ignite", "Use Ignite").SetValue(true));
            cMenu.AddItem(new MenuItem("ElCorki.ssssssssssss", ""));
            cMenu.AddItem(new MenuItem("ElCorki.Combo.RStacks", "Keep R Stacks").SetValue(new Slider(0, 0, 7)));
            cMenu.AddItem(
                new MenuItem("ElCorki.hitChance", "Hitchance Q").SetValue(
                    new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));
            cMenu.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            _menu.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "Harass");
            hMenu.AddItem(new MenuItem("ElCorki.Harass.Q", "Use Q").SetValue(true));
            hMenu.AddItem(new MenuItem("ElCorki.Harass.E", "Use E").SetValue(false));
            hMenu.AddItem(new MenuItem("ElCorki.Harass.R", "Use R").SetValue(true));
            hMenu.AddItem(new MenuItem("ElCorki.Harass.RStacks", "Keep R Stacks").SetValue(new Slider(0, 0, 7)));
            hMenu.AddItem(new MenuItem("ElCorki.harass.mana2", "Harass mana").SetValue(new Slider(55)));

            hMenu.SubMenu("AutoHarass")
                .AddItem(
                    new MenuItem("ElCorki.AutoHarass", "[Toggle] Auto harass", false).SetValue(
                        new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)));
            hMenu.SubMenu("AutoHarass").AddItem(new MenuItem("ElCorki.UseQAutoHarass", "Use Q").SetValue(true));
            hMenu.SubMenu("AutoHarass").AddItem(new MenuItem("ElCorki.UseRAutoHarass", "Use R").SetValue(true));
            hMenu.SubMenu("AutoHarass")
                .AddItem(new MenuItem("ElCorki.harass.mana", "Auto harass mana"))
                .SetValue(new Slider(55));

            _menu.AddSubMenu(hMenu);

            var lMenu = new Menu("Clear", "Clear");
            lMenu.AddItem(new MenuItem("useQFarm", "Use Q").SetValue(true));
            lMenu.AddItem(
                new MenuItem("ElCorki.Count.Minions", "Killable minions with Q >=").SetValue(new Slider(2, 1, 5)));
            lMenu.AddItem(new MenuItem("useEFarm", "Use E").SetValue(true));
            lMenu.AddItem(
                new MenuItem("ElCorki.Count.Minions.E", "Killable minions with E >=").SetValue(new Slider(2, 1, 5)));
            lMenu.AddItem(new MenuItem("useRFarm", "Use R").SetValue(true));
            lMenu.AddItem(
                new MenuItem("ElCorki.Count.Minions.R", "Killable minions with R >=").SetValue(new Slider(2, 1, 5)));
            lMenu.AddItem(new MenuItem("useEFarmddsdsaasaafsdaadsd", ""));
            lMenu.AddItem(new MenuItem("useQFarmJungle", "Use Q in jungle").SetValue(true));
            lMenu.AddItem(new MenuItem("useEFarmJungle", "Use E in jungle").SetValue(true));
            lMenu.AddItem(new MenuItem("useRFarmJungle", "Use R in jungle").SetValue(true));
            lMenu.AddItem(new MenuItem("useEFarmddssd", ""));
            lMenu.AddItem(new MenuItem("minmanaclear", "Mana needed to clear ")).SetValue(new Slider(55));

            _menu.AddSubMenu(lMenu);

            var itemMenu = new Menu("Items", "Items");
            itemMenu.AddItem(new MenuItem("ElCorki.Items.Youmuu", "Use Youmuu's Ghostblade").SetValue(true));
            itemMenu.AddItem(new MenuItem("ElCorki.Items.Cutlass", "Use Cutlass").SetValue(true));
            itemMenu.AddItem(new MenuItem("ElCorki.Items.Blade", "Use Blade of the Ruined King").SetValue(true));
            itemMenu.AddItem(new MenuItem("ElCorki.Harasssfsddass.E", ""));
            itemMenu.AddItem(
                new MenuItem("ElCorki.Items.Blade.EnemyEHP", "Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            itemMenu.AddItem(
                new MenuItem("ElCorki.Items.Blade.EnemyMHP", "My HP Percentage").SetValue(new Slider(80, 100, 0)));

            _menu.AddSubMenu(itemMenu);

            //ElCorki.Misc
            var miscMenu = new Menu("Misc", "Misc");
            miscMenu.AddItem(new MenuItem("ElCorki.Draw.off", "Turn drawings off").SetValue(false));
            miscMenu.AddItem(new MenuItem("ElCorki.Draw.Q", "Draw Q").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElCorki.Draw.W", "Draw W").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElCorki.Draw.E", "Draw E").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElCorki.Draw.R", "Draw R").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElCorki.Draw.Text", "Draw Text").SetValue(true));

            miscMenu.AddItem(new MenuItem("useEFafsdsgdrmddsddsasfsasdsdsaadsd", ""));
            miscMenu.AddItem(new MenuItem("ElCorki.misc.ks", "Killsteal mode").SetValue(false));
            miscMenu.AddItem(new MenuItem("ElCorki.misc.junglesteal", "Jungle steal mode").SetValue(false));

            _menu.AddSubMenu(miscMenu);

            //Here comes the moneyyy, money, money, moneyyyy
            var credits = new Menu("Credits", "jQuery");
            credits.AddItem(new MenuItem("ElCorki.Paypal", "if you would like to donate via paypal:"));
            credits.AddItem(new MenuItem("ElCorki.Email", "info@zavox.nl"));
            _menu.AddSubMenu(credits);

            _menu.AddItem(new MenuItem("422442fsaafs4242f", ""));

            _menu.AddItem(new MenuItem("422442fsaafsf", (string.Format("ElCorki by jQuery v{0}", ScriptVersion))));
            _menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            _menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}