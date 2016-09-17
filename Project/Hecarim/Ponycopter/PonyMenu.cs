using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using System.Drawing;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ponycopter {
    class PonyMenu {
        public static Menu Config;

        public static void Init() {
            Config = new Menu("Ponycopter", Ponycopter.Champion, true);

            // Target Selector
            var targetselector = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetselector);
            Config.AddSubMenu(targetselector);

            // Orbwalker
            Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Ponycopter.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            // Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.Q", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.W", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.E", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.R", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.RCount", "Minimum Enemies for R").SetValue(new Slider(2, 0, 5)));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.Ghost", "Use Ghost").SetValue(true));

            // Harass
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.Q", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.W", "Use W").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.MinMana", "Minimum % Mana to Harass").SetValue(new Slider(35)));
            
            // Last Hit
            Config.AddSubMenu(new Menu("Last Hitting", "LastHit"));
            Config.SubMenu("LastHit").AddItem(new MenuItem("LastHit.Q", "Use Q").SetValue(true));
            Config.SubMenu("LastHit").AddItem(new MenuItem("LastHit.MinMana", "Minimum % Mana to Last Hit").SetValue(new Slider(35)));

            // Lane Clear
            Config.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClear.Q", "Use Q").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClear.W", "Use W").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClear.MinMana", "Min. % Mana for Laneclear").SetValue(new Slider(35)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClear.MinMinions", "Minimum Minions for W").SetValue(new Slider(2, 0, 5)));

            // Jungle Clear
            Config.AddSubMenu(new Menu("Jungle Clear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("JungleClear.Q", "Use Q").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("JungleClear.W", "Use W").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("JungleClear.E", "Use E").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("InteruptE", "Interrupt with E").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("InteruptR", "Interrupt with R").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoHarass", "Auto Harass").SetValue(new KeyBind('H', KeyBindType.Toggle, true)));

            Config.AddItem(new MenuItem("hitChance", "Hitchance").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));

            // Credits
            Config.AddItem(new MenuItem("space", " "));
            Config.AddItem(new MenuItem("sssss", "Author: TheOBJop"));

            var dmgAfterE = new MenuItem("DrawComboDamage", "Draw combo damage").SetValue(true);
            var drawFill = new MenuItem("DrawColour", "Fill colour", true).SetValue(new Circle(true, Color.FromArgb(204, 204, 0, 0)));
            Config.SubMenu("Misc").AddItem(drawFill);
            Config.SubMenu("Misc").AddItem(dmgAfterE);

            DrawDamage.DamageToUnit = Ponycopter.GetComboDamage;
            DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
            DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

            dmgAfterE.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs) {
                DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs) {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            Config.AddToMainMenu();
        }
    }
}
