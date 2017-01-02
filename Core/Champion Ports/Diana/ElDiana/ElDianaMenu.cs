namespace ElDiana
{
    using System;
    using System.Drawing;

    using LeagueSharp.Common;

    public class ElDianaMenu
    {
        #region Static Fields

        public static Menu _menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            _menu = new Menu("ElDiana", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Diana.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            _menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);

            _menu.AddSubMenu(targetSelector);

            var cMenu = new Menu("Combo", "Combo");
            cMenu.SubMenu("R")
                .AddItem(
                    new MenuItem("ElDiana.Combo.R.Mode", "Mode").SetValue(
                        new StringList(new[] { "Normal (Q->R)", "Misaya Combo (R->Q)" })));
            cMenu.SubMenu("R").AddItem(new MenuItem("ElDiana.Combo.R", "Use R").SetValue(true));
            cMenu.SubMenu("R")
                .AddItem(
                    new MenuItem("ElDiana.Combo.R.MisayaMinRange", "R Minimum Range for Misaya ").SetValue(
                        new Slider(
                            Convert.ToInt32(Diana.spells[Spells.R].Range * 0.8),
                            0,
                            Convert.ToInt32(Diana.spells[Spells.R].Range))));
            cMenu.SubMenu("R")
                .AddItem(
                    new MenuItem("ElDiana.Combo.R.PreventUnderTower", "Don't use ult if HP% <  ").SetValue(
                        new Slider(20)));

            cMenu.AddItem(new MenuItem("ElDiana.Combo.Q", "Use Q").SetValue(true));
            cMenu.AddItem(new MenuItem("ElDiana.Combo.W", "Use W").SetValue(true));
            cMenu.AddItem(new MenuItem("ElDiana.Combo.E", "Use E").SetValue(true));
            cMenu.AddItem(new MenuItem("ElDiana.Combo.Secure", "Use R to secure kill").SetValue(true));
            cMenu.AddItem(
                new MenuItem("ElDiana.Combo.UseSecondRLimitation", "Max close enemies for secure kill with R").SetValue(
                    new Slider(5, 1, 5)));
            cMenu.AddItem(new MenuItem("ElDiana.Combo.Ignite", "Use Ignite").SetValue(true));
            cMenu.AddItem(new MenuItem("ElDiana.ssssssssssss", ""));
            cMenu.AddItem(
                new MenuItem("ElDiana.hitChance", "Hitchance Q").SetValue(
                    new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));


            var switchComboMenu =
    new MenuItem("ElDiana.Hotkey.ToggleComboMode", "Toggle Combo Mode Hotkey").SetValue(
        new KeyBind(84, KeyBindType.Press));
            cMenu.AddItem(switchComboMenu);
            switchComboMenu.ValueChanged += (sender, eventArgs) =>
            {
                if (eventArgs.GetNewValue<KeyBind>().Active)
                {
                    Diana.Orbwalker.ActiveMode = Orbwalking.OrbwalkingMode.Combo;
                }
                else
                {
                    Diana.Orbwalker.ActiveMode = Orbwalking.OrbwalkingMode.None;
                }
            };

            _menu.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "Harass");
            hMenu.AddItem(new MenuItem("ElDiana.Harass.Q", "Use Q").SetValue(true));
            hMenu.AddItem(new MenuItem("ElDiana.Harass.W", "Use W").SetValue(true));
            hMenu.AddItem(new MenuItem("ElDiana.Harass.E", "Use E").SetValue(true));
            hMenu.AddItem(new MenuItem("ElDiana.Harass.Mana", "Minimum mana for harass")).SetValue(new Slider(55));

            _menu.AddSubMenu(hMenu);

            var lMenu = new Menu("Laneclear", "Laneclear");
            lMenu.AddItem(new MenuItem("ElDiana.LaneClear.Q", "Use Q").SetValue(true));
            lMenu.AddItem(new MenuItem("ElDiana.LaneClear.W", "Use W").SetValue(true));
            lMenu.AddItem(new MenuItem("ElDiana.LaneClear.E", "Use E").SetValue(true));
            lMenu.AddItem(new MenuItem("ElDiana.LaneClear.R", "Use R").SetValue(false));
            lMenu.AddItem(new MenuItem("xxx", ""));

            lMenu.AddItem(
                new MenuItem("ElDiana.LaneClear.Count.Minions.Q", "Minions in range for Q").SetValue(
                    new Slider(2, 1, 5)));
            lMenu.AddItem(
                new MenuItem("ElDiana.LaneClear.Count.Minions.W", "Minions in range for W").SetValue(
                    new Slider(2, 1, 5)));
            lMenu.AddItem(
                new MenuItem("ElDiana.LaneClear.Count.Minions.E", "Minions in range for E").SetValue(
                    new Slider(2, 1, 5)));

            _menu.AddSubMenu(lMenu);

            var jMenu = new Menu("Jungleclear", "Jungleclear");
            jMenu.AddItem(new MenuItem("ElDiana.JungleClear.Q", "Use Q").SetValue(true));
            jMenu.AddItem(new MenuItem("ElDiana.JungleClear.W", "Use W").SetValue(true));
            jMenu.AddItem(new MenuItem("ElDiana.JungleClear.E", "Use E").SetValue(true));
            jMenu.AddItem(new MenuItem("ElDiana.JungleClear.R", "Use R").SetValue(false));

            _menu.AddSubMenu(jMenu);

            var interruptMenu = new Menu("Interrupt", "Interrupt");
            interruptMenu.AddItem(new MenuItem("ElDiana.Interrupt.UseEInterrupt", "Use E to interrupt").SetValue(true));
            interruptMenu.AddItem(
                new MenuItem("ElDiana.Interrupt.UseEDashes", "Use E to interrupt dashes").SetValue(true));

            _menu.AddSubMenu(interruptMenu);

            var miscMenu = new Menu("Misc", "Misc");
            miscMenu.AddItem(new MenuItem("ElDiana.Draw.off", "Turn drawings off").SetValue(false));
            miscMenu.AddItem(new MenuItem("ElDiana.Draw.Q", "Draw Q").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElDiana.Draw.W", "Draw W").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElDiana.Draw.E", "Draw E").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElDiana.Draw.R", "Draw R").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElDiana.Draw.RMisaya", "Draw Misaya Combo Range").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElDiana.Draw.Text", "Draw Text").SetValue(true));
            miscMenu.AddItem(new MenuItem("ezeazeezaze", ""));



            var dmgAfterE = new MenuItem("ElDiana.DrawComboDamage", "Draw combo damage").SetValue(true);
            var drawFill =
                new MenuItem("ElDiana.DrawColour", "Fill colour", true).SetValue(
                    new Circle(true, Color.FromArgb(204, 204, 0, 0)));
            miscMenu.AddItem(drawFill);
            miscMenu.AddItem(dmgAfterE);

            //DrawDamage.DamageToUnit = Diana.GetComboDamage;
            //DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
            //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

            dmgAfterE.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                    };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                    //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };

            _menu.AddSubMenu(miscMenu);

            //Here comes the moneyyy, money, money, moneyyyy
            var credits = new Menu("Credits", "jQuery");
            credits.AddItem(new MenuItem("ElDiana.Paypal", "if you would like to donate via paypal:"));
            credits.AddItem(new MenuItem("ElDiana.Email", "info@zavox.nl"));
            _menu.AddSubMenu(credits);

            _menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            _menu.AddItem(new MenuItem("422442fsaafsf", string.Format("Version: {0}", Diana.ScriptVersion)));
            _menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            _menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }

        #endregion
    }
}