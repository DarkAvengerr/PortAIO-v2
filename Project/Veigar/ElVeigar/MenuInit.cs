using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElVeigar
{
    using System.Drawing;

    using LeagueSharp.Common;

    public class MenuInit
    {
        #region Static Fields

        public static Menu Menu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = new Menu("ElVeigar", "ElVeigar", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Entry.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ElVeigar.Combo.Q", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVeigar.Combo.W.Stun", "Use W on stun").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVeigar.Combo.W", "Use W").SetValue(false));
                comboMenu.AddItem(new MenuItem("ElVeigar.Combo.E", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVeigar.Combo.R", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVeigar.Combo.Use.Ignite", "Use Ignite").SetValue(true));
                comboMenu.AddItem(new MenuItem("ElVeigar.Combo.Multi", "Hit Multiple").SetValue(false));

            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(
                    new MenuItem("Harass.Mode", "Harass Mode").SetValue(
                        new StringList(new[] { "E - W - Q", "E - W", "Q" }, 1)));
                harassMenu.AddItem(new MenuItem("ElVeigar.Harass.Mana", "Minimum mana").SetValue(new Slider(20)));
            }

            var stackMenu = Menu.AddSubMenu(new Menu("Q Stack", "Q Stack"));
            {
                stackMenu.AddItem(
                    new MenuItem("ElVeigar.Stack.Q", "Auto Q stack").SetValue(
                        new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle)));

                stackMenu.AddItem(
                    new MenuItem("ElVeigar.Stack.Q2", "Minimum minions for Q").SetValue(new Slider(2, 1, 2)));
                stackMenu.AddItem(new MenuItem("ElVeigar.Stack.Mana", "Minimum mana").SetValue(new Slider(20)));
            }

            var laneclearMenu = Menu.AddSubMenu(new Menu("Laneclear", "Laneclear"));
            {
                laneclearMenu.AddItem(new MenuItem("ElVeigar.LaneClear.Q", "Use Q").SetValue(true));
                laneclearMenu.AddItem(new MenuItem("ElVeigar.LaneClear.W", "Use W").SetValue(true));
                laneclearMenu.AddItem(
                    new MenuItem("ElVeigar.LaneClear.W.Minions", "Minions for W ").SetValue(new Slider(2, 1, 6)));

                laneclearMenu.AddItem(new MenuItem("ElVeigar.LaneClear.Mana", "Minimum mana").SetValue(new Slider(20)));
            }

            var jungleclearMenu = Menu.AddSubMenu(new Menu("Jungleclear", "Jungleclear"));
            {
                jungleclearMenu.AddItem(new MenuItem("ElVeigar.jungleclearMenu.Q", "Use Q").SetValue(false));
                jungleclearMenu.AddItem(new MenuItem("ElVeigar.jungleclearMenu.W", "Use W").SetValue(true));
                jungleclearMenu.AddItem(
                    new MenuItem("ElVeigar.JungleClear.Mana", "Minimum mana").SetValue(new Slider(20)));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(new MenuItem("Misc.Drawings.Off", "Turn drawings off").SetValue(false));
                miscMenu.AddItem(new MenuItem("Misc.Drawings.Q", "Draw Q").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("Misc.Drawings.W", "Draw W").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("Misc.Drawings.E", "Draw E").SetValue(new Circle()));
                miscMenu.AddItem(new MenuItem("Misc.Drawings.R", "Draw R").SetValue(new Circle()));

                miscMenu.AddItem(new MenuItem("Misc.Interrupt", "Interrupt with E").SetValue(false));

                var dmgAfterE = new MenuItem("ElDiana.DrawComboDamage", "Draw combo damage").SetValue(true);
                var drawFill =
                    new MenuItem("ElDiana.DrawColour", "Fill colour", true).SetValue(
                        new Circle(true, Color.FromArgb(204, 0, 167, 255)));
                miscMenu.AddItem(drawFill);
                miscMenu.AddItem(dmgAfterE);

                DrawDamage.DamageToUnit = Entry.GetComboDamage;
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
            }

            var ksMenu = Menu.AddSubMenu(new Menu("Killsteal", "Killsteal"));
            {
                ksMenu.AddItem(new MenuItem("ElVeigar.Combo.KS.Q", "KS with Q").SetValue(true));
                ksMenu.AddItem(new MenuItem("ElVeigar.Combo.KS.W", "KS with W").SetValue(true));

                ksMenu.AddItem(new MenuItem("ElVeigar.Combo.KS.R", "KS with R").SetValue(true));

                foreach (var hero in HeroManager.Enemies)
                {
                    ksMenu.SubMenu("Use R on")
                        .AddItem(
                            new MenuItem("ElVeigar.KS.R.On" + hero.CharData.BaseSkinName, hero.CharData.BaseSkinName)
                                .SetValue(true));
                }
            }

            Menu.AddItem(new MenuItem("sep3", "Made By jQuery"));
            Menu.AddItem(new MenuItem("sep2", $"Version: {Entry.ScriptVersion}"));
            Menu.AddToMainMenu();
        }

        public static bool IsActive(string menuItem)
        {
            return Menu.Item(menuItem).GetValue<bool>();
        }

        public static StringList IsListActive(string menuItem)
        {
            return Menu.Item(menuItem).GetValue<StringList>();
        }

        #endregion
    }
}