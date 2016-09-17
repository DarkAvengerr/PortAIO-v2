using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElLux
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class ElLuxMenu
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = new Menu("ElLux", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Lux.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);

            var cMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                cMenu.AddItem(new MenuItem("ElLux.Combo.Q", "Use Q").SetValue(true));
                cMenu.AddItem(new MenuItem("ElLux.Combo.E", "Use E").SetValue(true));

                cMenu.SubMenu("Ultimate").AddItem(new MenuItem("ElLux.Combo.R", "Use R").SetValue(true));

                cMenu.SubMenu("Ultimate")
                    .AddItem(new MenuItem("ElLux.Combo.R.Rooted", "Use R when rooted").SetValue(false));
                cMenu.SubMenu("Ultimate")
                    .AddItem(new MenuItem("ElLux.Combo.R.Kill", "Use R when killable").SetValue(true));
                cMenu.SubMenu("Ultimate").AddItem(new MenuItem("ElLux.Combo.R.AOE", "Use R AOE").SetValue(true));
                cMenu.SubMenu("Ultimate")
                    .AddItem(new MenuItem("ElLux.Combo.R.Count", "Hit count").SetValue(new Slider(3, 1, 5)));
            }

            var hMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                hMenu.AddItem(new MenuItem("ElLux.Harass.Q", "Use Q").SetValue(true));
                hMenu.AddItem(new MenuItem("ElLux.Harass.E", "Use E").SetValue(true));
            }

            var wMenu = Menu.AddSubMenu(new Menu("W settings", "WSettings"));
            {
                wMenu.AddItem(new MenuItem("W.Activated", "Use W").SetValue(true));
                wMenu.AddItem(new MenuItem("W.Damage", "W on damage dealt %").SetValue(new Slider(80, 1)));
                wMenu.AddItem(new MenuItem("seperator21", ""));
                foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
                {
                    wMenu.AddItem(new MenuItem("wOn" + x.ChampionName, "Use for " + x.ChampionName)).SetValue(true);
                }
            }

            var lMenu = Menu.AddSubMenu(new Menu("Laneclear", "Laneclear"));
            {
                lMenu.AddItem(new MenuItem("ElLux.Laneclear.Q", "Use Q").SetValue(true));
                lMenu.AddItem(new MenuItem("ElLux.Laneclear.W", "Use W").SetValue(false));
                lMenu.AddItem(new MenuItem("ElLux.Laneclear.E", "Use E").SetValue(true));
                lMenu.AddItem(new MenuItem("ElLux.Laneclear.E.Count", "Minion count").SetValue(new Slider(1, 1, 5)));
            }

            var jMenu = Menu.AddSubMenu(new Menu("Jungleclear", "Jungleclear"));
            {
                jMenu.AddItem(new MenuItem("ElLux.JungleClear.Q", "Use Q").SetValue(true));
                jMenu.AddItem(new MenuItem("ElLux.JungleClear.W", "Use W").SetValue(true));
                jMenu.AddItem(new MenuItem("ElLux.JungleClear.E", "Use E").SetValue(true));
                jMenu.AddItem(new MenuItem("ElLux.JungleClear.E.Count", "Minion count").SetValue(new Slider(1, 1, 5)));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(new MenuItem("ElLux.Auto.Q", "Auto Q on stuns").SetValue(true));
                miscMenu.AddItem(new MenuItem("ElLux.Auto.E", "Auto E on stuns").SetValue(true));
            }

            var ksMenu = Menu.AddSubMenu(new Menu("Killsteal", "Killsteal"));
            {
                ksMenu.AddItem(new MenuItem("ElLux.KS.R", "KS with R").SetValue(true));
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("ElLux.Draw.off", "Turn drawings off").SetValue(false));
                drawMenu.AddItem(new MenuItem("ElLux.Draw.Q", "Draw Q").SetValue(new Circle()));
                drawMenu.AddItem(new MenuItem("ElLux.Draw.W", "Draw W").SetValue(new Circle()));
                drawMenu.AddItem(new MenuItem("ElLux.Draw.E", "Draw E").SetValue(new Circle()));
                drawMenu.AddItem(new MenuItem("ElLux.Draw.R", "Draw R").SetValue(new Circle()));
            }

            Menu.AddItem(new MenuItem("422442fssddsaafs4242f", ""));
            Menu.AddItem(new MenuItem("ElLux.Combo.Semi.R", "Semi-manual R").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Menu.AddItem(new MenuItem("ElLux.Combo.Semi.Q", "Semi-manual Q").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press)));


            Menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            Menu.AddItem(new MenuItem("422442fsaafsf", (string.Format("ElLux by jQuery v{0}", Lux.ScriptVersion))));
            Menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            Menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}