using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


namespace LSharpNunu
{
    public class NunuMenu
    {

        public static Menu _menu;

        public static void Initialize()
        {
            _menu = new Menu("LSharp - Nunu", "menu", true);

            //Nunu.Orbwalker
            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Nunu.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            _menu.AddSubMenu(orbwalkerMenu);

            //Nunu.TargetSelector
            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            _menu.AddSubMenu(targetSelector);

            //Nunu.Menu
            var comboMenu = _menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("Nunu.Combo.R", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("Nunu.Combo.RCount", "Enemies in Range to use R").SetValue(new Slider(3, 1, 5)));
            comboMenu.AddItem(new MenuItem("Nunu.Combo.W", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("Nunu.Combo.E", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("Nunu.Combo.separator", ""));
            comboMenu.AddItem(new MenuItem("Nunu.Combo.Ignite", "Use Ignite").SetValue(true));
            comboMenu.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //Nunu.Clear
            var clearMenu = _menu.AddSubMenu(new Menu("Jungle and laneclear", "JLC"));
            clearMenu.AddItem(new MenuItem("Nunu.Clear.Q", "Use Q").SetValue(true));
            clearMenu.AddItem(new MenuItem("Nunu.Clear.W", "Use W").SetValue(true));
            clearMenu.AddItem(new MenuItem("Nunu.Clear.E", "Use E").SetValue(true));
            clearMenu.AddItem(new MenuItem("422442fsaafsf", ""));

            //Nunu.SmiteSettinsg
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("Nunu.smiteEnabled", "Auto smite enabled").SetValue(new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle)));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("422442fsaafsf", ""));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("Selected Smite Targets", "Selected Smite Targets:"));

            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("SRU_Red", "Red Buff").SetValue(true));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("SRU_Blue", "Blue Buff").SetValue(true));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("SRU_Dragon", "Dragon").SetValue(true));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("SRU_Baron", "Baron").SetValue(true));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("Nunu.normalSmite", "Normal Smite").SetValue(true));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("Nunu.stealq", "Steal with Q - Currently not working").SetValue(true));

            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("422442fsaafsf11", ""));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("Smite Save Settings", "Smite Save Settings:"));

            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("Nunu.smiteSave", "Smite Save Active").SetValue(true));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("hpPercentSM", "WWSmite on x%").SetValue(new Slider(10, 1)));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("param1", "Dont Smite if near and hp = x%"));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("dBuffs", "Buffs").SetValue(true));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("hpBuffs", "HP %").SetValue(new Slider(30, 1)));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("dEpics", "Epics").SetValue(true));
            clearMenu.SubMenu("Smite Settings").AddItem(new MenuItem("hpEpics", "HP %").SetValue(new Slider(10, 1)));

            //Nunu.Harass
            var harassMenu = _menu.AddSubMenu(new Menu("Harass", "Harass"));
            harassMenu.AddItem(new MenuItem("Nunu.Harass.E", "Use E").SetValue(true));
            harassMenu.AddItem(new MenuItem("Nunu.Harass.Active", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            //Nunu.Healing
            var healMenu = _menu.AddSubMenu(new Menu("Heal", "SH"));
            healMenu.AddItem(new MenuItem("Nunu.Heal.AutoHeal", "Auto heal yourself").SetValue(true));
            healMenu.AddItem(new MenuItem("Nunu.Heal.HP", "Self heal at >= ").SetValue(new Slider(25, 1, 100)));

            //Nunu.Misc
            var miscMenu = _menu.AddSubMenu(new Menu("Drawings", "Misc"));
            miscMenu.AddItem(new MenuItem("Nunu.Draw.off", "[Drawing] Drawings off").SetValue(false));
            miscMenu.AddItem(new MenuItem("Nunu.Draw.q", "Draw Q").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("Nunu.Draw.W", "Draw W").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("Nunu.Draw.E", "Draw E").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("Nunu.Draw.R", "Draw R").SetValue(new Circle()));


            var dmgAfterE = new MenuItem("Nunu.DrawComboDamage", "Draw combo damage").SetValue(false);
            var drawFill = new MenuItem("Nunu.DrawColour", "Fill colour", true).SetValue(new Circle(true, Color.FromArgb(204, 204, 0, 0)));
            miscMenu.AddItem(drawFill);
            miscMenu.AddItem(dmgAfterE);

            //DrawDamage.DamageToUnit = Nunu.GetComboDamage;
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

            
            var credits = _menu.AddSubMenu(new Menu("Credits", "BillyGG"));

            _menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            _menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By BillyGG"));

            _menu.AddToMainMenu();

            Console.WriteLine("Menu Loaded");
        }
    }
}