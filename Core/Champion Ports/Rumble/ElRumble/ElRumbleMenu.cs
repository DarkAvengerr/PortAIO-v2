using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRumble
{
    public class ElRumbleMenu
    {
        public static Menu _menu;

        public static void Initialize()
        {
            _menu = new Menu("ElRumble", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Rumble.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            _menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);

            _menu.AddSubMenu(targetSelector);

            var comboMenu = new Menu("Combo", "Combo");
            comboMenu.AddItem(new MenuItem("ElRumble.Combo.Q", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("ElRumble.Combo.W", "Use W").SetValue(false));
            comboMenu.AddItem(new MenuItem("ElRumble.Combo.E", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("ElRumble.Combo.R", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("ElRumble.Combo.Count.Enemies", "Enemies in range for R").SetValue(new Slider(2, 1, 5)));
            comboMenu.AddItem(new MenuItem("ElRumble.Combo.Ignite", "Use Ignite").SetValue(true));

            _menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass", "Harass");
            harassMenu.AddItem(new MenuItem("ElRumble.Harass.Q", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("ElRumble.Harass.E", "Use E").SetValue(true));

            _menu.AddSubMenu(harassMenu);

            var heatMenu = new Menu("Heat", "Heat");
            heatMenu.AddItem(new MenuItem("ElRumble.KeepHeat.Activated", "Auto harass", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));
            heatMenu.AddItem(new MenuItem("ElRumble.Heat.Q", "Use Q").SetValue(true));
            heatMenu.AddItem(new MenuItem("ElRumble.Heat.W", "Use W").SetValue(true));

            _menu.AddSubMenu(heatMenu);

            var clearMenu = new Menu("Clear", "Clear");
            clearMenu.SubMenu("LastHit").AddItem(new MenuItem("ElRumble.LastHit.E", "Lasthit with E").SetValue(true));
            clearMenu.SubMenu("Laneclear").AddItem(new MenuItem("ElRumble.LaneClear.Q", "Use Q").SetValue(true));
            clearMenu.SubMenu("Laneclear").AddItem(new MenuItem("ElRumble.LaneClear.E", "Use E").SetValue(true));
            clearMenu.SubMenu("Jungleclear").AddItem(new MenuItem("ElRumble.JungleClear.Q", "Use Q").SetValue(true));
            clearMenu.SubMenu("Jungleclear").AddItem(new MenuItem("ElRumble.JungleClear.E", "Use E").SetValue(true));

            _menu.AddSubMenu(clearMenu);

            var miscMenu = new Menu("Misc", "Misc");
            miscMenu.AddItem(new MenuItem("ElRumble.Draw.off", "Turn drawings off").SetValue(false));
            miscMenu.AddItem(new MenuItem("ElRumble.Draw.Q", "Draw Q").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElRumble.Draw.E", "Draw E").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElRumble.Draw.R", "Draw R").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("xxxxx", ""));
            miscMenu.AddItem(new MenuItem("ElRumble.Misc.R", "Manual R").SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));

            var dmgAfterE = new MenuItem("ElRumble.DrawComboDamage", "Draw combo damage").SetValue(true);
            var drawFill = new MenuItem("ElRumble.DrawColour", "Fill colour", true).SetValue(new Circle(true, Color.FromArgb(204, 204, 0, 0)));
            miscMenu.AddItem(drawFill);
            miscMenu.AddItem(dmgAfterE);

            //DrawDamage.DamageToUnit = Rumble.GetComboDamage;
            //DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
            //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

            dmgAfterE.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            _menu.AddSubMenu(miscMenu);

            _menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            _menu.AddItem(new MenuItem("422442fsaafsf", "Version: 1.0.0.0"));
            _menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            _menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }
    }
}