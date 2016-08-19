using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace The_Donger {
    class DongerMenu {
        public static Menu Config;

        public static void Init() {
            //Menu
            Config = new Menu("The Donger", Donger.Champion, true);

            //Targetselector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Orbwalk
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Donger.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //LaneClear
            Config.AddSubMenu(new Menu("Laneclear", "Laneclear"));
            Config.SubMenu("Laneclear").AddItem(new MenuItem("Laneclear.W", "Use W")).SetValue(true);
            Config.SubMenu("Laneclear").AddItem(new MenuItem("Laneclear.E", "Use E")).SetValue(false);
            Config.SubMenu("Laneclear").AddItem(new MenuItem("Laneclear.Mana", "Minimum Mana for clear")).SetValue(new Slider(30, 0, 100));

            //C-C-C-Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("Mode", "Prioritize Ult").SetValue(new StringList(new[] { "Rockets (R->W)", "Turret (R->Q)", "Grenade (R->E)" })));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.Q", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.W", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.E", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.R", "Use R")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.Zhonyas", "Zhonya's Defensively")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.Ignite", "Use Ignite")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.Active", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            // Harass
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.Q", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.W", "Use W").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.E", "Use E").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.Mana", "Minimum mana for harass")).SetValue(new Slider(55));

            //MISCMENU
            Config.SubMenu("Misc").AddItem(new MenuItem("AntiGap", "Anti Gapcloser - E").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Interrupt", "Interrupt Spells - E").SetValue(false));

            var dmgAfterE = new MenuItem("DrawComboDamage", "Draw combo damage").SetValue(true);
            var drawFill = new MenuItem("DrawColour", "Fill colour", true).SetValue(new Circle(true, Color.FromArgb(204, 204, 0, 0)));
            Config.SubMenu("Misc").AddItem(drawFill);
            Config.SubMenu("Misc").AddItem(dmgAfterE);

            DrawDamage.DamageToUnit = Donger.GetComboDamage;
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

            
            Config.AddItem(new MenuItem("hitChance", "Hitchance").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));

            Config.AddItem(new MenuItem("ssssss", ""));
            Config.AddItem(new MenuItem("Author", "Author: TheOBJop"));

            Config.AddToMainMenu();
        }
    }
}
