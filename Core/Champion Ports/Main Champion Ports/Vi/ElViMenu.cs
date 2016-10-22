using EloBuddy; namespace ElVi
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class ElViMenu
    {
        #region Static Fields

        public static Menu _menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            _menu = new Menu("ElVi", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            {
                Vi.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            }

            _menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            {
                TargetSelector.AddToMenu(targetSelector);
            }

            _menu.AddSubMenu(targetSelector);

            var cMenu = new Menu("Combo", "Combo");
            {
                cMenu.AddItem(new MenuItem("ElVi.Combo.Q", "Use Q").SetValue(true));
                cMenu.AddItem(new MenuItem("ElVi.Combo.E", "Use E").SetValue(true));
                cMenu.AddItem(new MenuItem("ElVi.Combo.R", "Use R").SetValue(true));
                cMenu.AddItem(new MenuItem("ElVi.Combo.I", "Use Ignite").SetValue(true));
                cMenu.AddItem(
                    new MenuItem("ElVi.Combo.Flash", "Flash Q").SetValue(
                        new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                cMenu.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
            }

            _menu.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "Harass");
            {
                hMenu.AddItem(new MenuItem("ElVi.Harass.Q", "Use Q").SetValue(true));
                hMenu.AddItem(new MenuItem("ElVi.Harass.E", "Use E").SetValue(true));
            }

            _menu.AddSubMenu(hMenu);

            var rMenu = new Menu("R", "R");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(champ => champ.IsEnemy))
            {
                rMenu.AddItem(
                    new MenuItem("ElVi.Settings.R" + enemy.BaseSkinName, string.Format("Ult: {0}", enemy.BaseSkinName))
                        .SetValue(true));
            }

            _menu.AddSubMenu(rMenu);

            var clearMenu = new Menu("Clear", "Clear");
            {
                clearMenu.SubMenu("Laneclear").AddItem(new MenuItem("ElVi.LaneClear.Q", "Use Q").SetValue(true));
                clearMenu.SubMenu("Laneclear").AddItem(new MenuItem("ElVi.LaneClear.E", "Use E").SetValue(true));
                clearMenu.SubMenu("Jungleclear").AddItem(new MenuItem("ElVi.JungleClear.Q", "Use Q").SetValue(true));
                clearMenu.SubMenu("Jungleclear").AddItem(new MenuItem("ElVi.JungleClear.E", "Use E").SetValue(true));
                clearMenu.AddItem(
                    new MenuItem("ElVi.Clear.Player.Mana", "Minimum Mana for clear").SetValue(new Slider(55)));
            }

            _menu.AddSubMenu(clearMenu);

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddItem(new MenuItem("ElVi.Draw.off", "Turn drawings off").SetValue(false));
                miscMenu.AddItem(new MenuItem("ElVi.Draw.Q", "Draw Q").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElVi.Draw.E", "Draw E").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("ElVi.Draw.R", "Draw R").SetValue(new Circle()));

                var dmgAfterE = new MenuItem("ElDiana.DrawComboDamage", "Draw combo damage").SetValue(true);
                var drawFill =
                    new MenuItem("ElDiana.DrawColour", "Fill colour", true).SetValue(
                        new Circle(true, Color.FromArgb(204, 204, 0, 0)));
                miscMenu.AddItem(drawFill);
                miscMenu.AddItem(dmgAfterE);

                DrawDamage.DamageToUnit = Vi.GetComboDamage;
                DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
                DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
                DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

                dmgAfterE.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                        {
                            DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                        };

                drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };

                miscMenu.AddItem(new MenuItem("xxx", ""));

                miscMenu.AddItem(new MenuItem("ElVi.misc.AntiGapCloser", "Use Antigabcloser").SetValue(true));
                miscMenu.AddItem(new MenuItem("ElVi.misc.Interrupter", "Use Interrupter").SetValue(true));
            }

            _menu.AddSubMenu(miscMenu);

            //Here comes the moneyyy, money, money, moneyyyy
            var credits = new Menu("Credits", "jQuery");
            {
                credits.AddItem(new MenuItem("ElVi.Paypal", "if you would like to donate via paypal:"));
                credits.AddItem(new MenuItem("ElVi.Email", "info@zavox.nl"));
            }
            _menu.AddSubMenu(credits);

            _menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            _menu.AddItem(new MenuItem("422442fsaafsf", "Version: 1.0.0.1"));
            _menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            _menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}
