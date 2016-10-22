using LeagueSharp.Common;
using LeagueSharp;
using Color = System.Drawing.Color;
using Kassadin_the_Harbinger.Handlers;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kassadin_the_Harbinger.Utility
{
    public class MenuGenerator
    {
        public static float CustomRange
        {
            get { return Kassadin.Menu.Item("kassadin.misc.customERange").GetValue<Slider>().Value; }
        }

        public static void Load()
        {
            Kassadin.Menu = new Menu("Kassadin - Harbinger", "kassadin_harbinger", true);

            var owMenu = new Menu("[KH] Orbwalker", "orbwalker");
            Kassadin.Orbwalker = new Orbwalking.Orbwalker(owMenu);
            Kassadin.Menu.AddSubMenu(owMenu);

            var tsMenu = new Menu("Target Selector", "Target.Selector");
            TargetSelector.AddToMenu(tsMenu);

            var comboMenu = Kassadin.Menu.AddSubMenu(new Menu("[KH] Combo", "kassadin.combo"));
            {
                comboMenu.AddItem(new MenuItem("kassadin.combo.useQ", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("kassadin.combo.useW", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("kassadin.combo.useE", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("kassadin.combo.useR", "Use R").SetValue(true));

                var comboOptionsMenu = new Menu("Combo - Options", "kassadin.combo.options");
                {
                    comboOptionsMenu.AddItem(new MenuItem("kassadin.combo.options.rMode", "Use R Mode").SetValue(new StringList(new[] { "Always", "Check for E" })));
                    comboOptionsMenu.AddItem(new MenuItem("kassadin.combo.options.turretDive", "Dive Turrets with R").SetValue(false));
                    comboOptionsMenu.AddItem(new MenuItem("kassadin.combo.options.dontR", "Don't R if >= X Enemies").SetValue(new Slider(3, 1, 5)));
                    comboOptionsMenu.AddItem(new MenuItem("kassadin.combo.options.useIgnite", "Use Ignite").SetValue(true));

                    comboMenu.AddSubMenu(comboOptionsMenu);
                }
            }

            var harassMenu = Kassadin.Menu.AddSubMenu(new Menu("[KH] Harass", "kassadin.harass"));
            {
                harassMenu.AddItem(new MenuItem("kassadin.harass.useQ", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("kassadin.harass.useE", "Use E").SetValue(false));
            }

            var waveclearMenu = Kassadin.Menu.AddSubMenu(new Menu("[KH] Waveclear", "kassadin.waveclear"));
            {
                waveclearMenu.AddItem(new MenuItem("kassadin.waveclear.useQ", "Use Q").SetValue(false));
                waveclearMenu.AddItem(new MenuItem("kassadin.waveclear.useE", "Use E").SetValue(false));
            }

            var fleeMenu = Kassadin.Menu.AddSubMenu(new Menu("[KH] Flee", "kassadin.flee"));
            {
                fleeMenu.AddItem(new MenuItem("kassadin.flee.activated", "Flee Activated").SetValue(new KeyBind('G', KeyBindType.Press)));
            }


            var miscMenu = Kassadin.Menu.AddSubMenu(new Menu("[KH] Miscellaneous", "kassadin.misc"));
            {
                miscMenu.AddItem(new MenuItem("kassadin.misc.interruptQ", "Use Q to interrupt dangerous spells").SetValue(true));
                miscMenu.AddItem(new MenuItem("kassadin.misc.gapcloseE", "Use E against gapclosers").SetValue(true));
                miscMenu.AddItem(new MenuItem("kassadin.misc.customERange", "Custom E Range").SetValue(new Slider(400, 400, 600))).ValueChanged +=
                    (sender, args) =>
                    {
                        SkillsHandler.Spells[SpellSlot.E].Range = CustomRange;
                    };
            }

            Kassadin.ManaManager.AddToMenu(ref Kassadin.Menu);

            var drawingMenu = Kassadin.Menu.AddSubMenu(new Menu("[KH] Drawings", "kassadin.drawing"));
            {
                drawingMenu.AddItem(new MenuItem("kassadin.drawing.drawQ", "Draw Q").SetValue(new Circle(false, Color.DarkOrange)));
                //drawingMenu.AddItem(new MenuItem("kassadin.drawing.drawW", "Draw W").SetValue(new Circle(true, Color.DarkOrange)));
                drawingMenu.AddItem(new MenuItem("kassadin.drawing.drawE", "Draw E").SetValue(new Circle(false, Color.DarkOrange)));
                drawingMenu.AddItem(new MenuItem("kassadin.drawing.drawR", "Draw R").SetValue(new Circle(false, Color.DarkOrange)));
                drawingMenu.AddItem(new MenuItem("kassadin.drawing.drawDamage", "Draw Damage").SetValue(new Circle(true, Color.GreenYellow)));
                drawingMenu.AddItem(new MenuItem("kassadin.drawing.drawingsOff", "Turn drawings off").SetValue(false));
            }

            Kassadin.Menu.AddItem(new MenuItem("seperator", ""));
            Kassadin.Menu.AddItem(new MenuItem("by.blacky", "Made by blacky"));

            Kassadin.Menu.AddToMainMenu();
        }
    }
}
