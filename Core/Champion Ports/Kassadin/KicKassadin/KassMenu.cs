using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KicKassadin {
    class KassMenu {
        public static Menu Config;

        public static void Init() {
            Config = new Menu("KicKassadin", KicKassadin.Champion, true);
            
            // Targetselector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Orbwalk
            Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            KicKassadin.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            // Laneclear
            Config.AddSubMenu(new Menu("Laneclear", "Laneclear"));
            Config.SubMenu("Laneclear").AddItem(new MenuItem("Laneclear.Q", "Use Q").SetValue<bool>(true));
            Config.SubMenu("Laneclear").AddItem(new MenuItem("Laneclear.W", "Use W").SetValue<bool>(true));
            Config.SubMenu("Laneclear").AddItem(new MenuItem("Laneclear.E", "Use E").SetValue<bool>(true));
            // Default to false, uses too much mana
            Config.SubMenu("Laneclear").AddItem(new MenuItem("Laneclear.R", "Use R").SetValue<bool>(false));
            Config.SubMenu("Laneclear").AddItem(new MenuItem("Laneclear.Mana", "Minimum Mana to Laneclear").SetValue<Slider>(new Slider(35, 0, 100)));

            // Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.Q", "Use Q").SetValue<bool>(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.W", "Use W").SetValue<bool>(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.E", "Use E").SetValue<bool>(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.R", "Use R").SetValue<bool>(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.DontR", "Enemy Count >= Dont R").SetValue<Slider>(new Slider(2, 0, 5)));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.Ignite", "Use Ignite").SetValue<bool>(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.GapcloseR", "Use R to Gapclose").SetValue<bool>(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.MaxStacksForR", "Max Stacks for Gapcloser").SetValue<Slider>(new Slider(2, 0, 4)));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.GapCloseHealth", "ENEMY MinHP % For Gapcloser").SetValue<Slider>(new Slider(20, 0, 100)));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo.Dive", "Force Dive Turret").SetValue(new KeyBind('G', KeyBindType.Press)));

            // Harass
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.Q", "Use Q").SetValue<bool>(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.E", "Use E").SetValue<bool>(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass.MinMana", "Minimum % Mana to Harass").SetValue(new Slider(35, 0, 100)));

            // Drawings
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Drawings.Q", "Draw Q").SetValue<bool>(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Drawings.W", "Draw W").SetValue<bool>(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Drawings.E", "Draw E").SetValue<bool>(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Drawings.R", "Draw R").SetValue<bool>(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Drawings.Off", "Drawings Off").SetValue<bool>(false));

            // Misc
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("Antigap", "Anti-Gapcloser with Ult").SetValue<bool>(true));

            Config.SubMenu("Misc").AddItem(new MenuItem("Flee", "Flee").SetValue<KeyBind>(new KeyBind('T', KeyBindType.Press)));

            var dmgAfterE = new MenuItem("DrawComboDamage", "Draw Combo Damage").SetValue(true);
            var drawFill = new MenuItem("DrawColour", "Fill colour", true).SetValue(new Circle(true, Color.FromArgb(204, 204, 0, 0)));
            Config.SubMenu("Misc").AddItem(drawFill);
            Config.SubMenu("Misc").AddItem(dmgAfterE);

            //DrawDamage.DamageToUnit = KicKassadin.GetComboDamage;
            //DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
            //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

            dmgAfterE.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs) {
                //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs) {
                //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            
            Config.AddItem(new MenuItem("hitChance", "Hitchance").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));

            Config.AddItem(new MenuItem("ssssss", ""));
            Config.AddItem(new MenuItem("Author", "Author: TheOBJop"));

            Config.AddToMainMenu();
        }
    }
}
