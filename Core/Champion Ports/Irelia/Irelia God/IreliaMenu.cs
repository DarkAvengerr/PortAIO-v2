using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace IreliaGod
{
    class IreliaMenu
    {
        public static Menu Config = new Menu("Irelia God", "IreliaGod", true);

        public static void Initialize()
        {
            var orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            {
                Program.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                Config.AddSubMenu(orbwalkerMenu);
            }

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            {
                TargetSelector.AddToMenu(targetSelectorMenu);
                Config.AddSubMenu(targetSelectorMenu);
                Config.AddItem(new MenuItem("force.target", "Force focus selected target").SetValue(true));
                Config.AddItem(new MenuItem("force.target.range", "if within:").SetValue(new Slider(1500, 0, 2500)));
            }

            var comboMenu = new Menu("Combo", "Combo settings");
            {
                comboMenu.AddItem(new MenuItem("combo.q", "Use Q on enemy").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.q.minrange", "   Minimum range to Q enemy").SetValue(new Slider(450, 0, 650)));
                comboMenu.AddItem(new MenuItem("combo.q.undertower", "   Q enemy under tower only if their health % under").SetValue(new Slider(40)));
                comboMenu.AddItem(new MenuItem("combo.q.lastsecond", "   Use Q to target always before W buff ends (range doesnt matter)").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.q.gc", "Use Q to gapclose (killable minions)").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.w", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.e", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.e.logic", "   advanced logic").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.r", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.r.weave", "   sheen synergy").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.r.selfactivated", "   only if self activated").SetValue(false));
                comboMenu.AddItem(new MenuItem("combo.items", "Use items").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.ignite", "Use ignite if combo killable").SetValue(true));
                Config.AddSubMenu(comboMenu);
            }
            var harassMenu = new Menu("Harass", "Harass settings");
            {
                harassMenu.AddItem(new MenuItem("harass.q", "Use Q on enemy").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.q.minrange", "   Minimum range to Q enemy").SetValue(new Slider(450, 0, 650)));
                harassMenu.AddItem(new MenuItem("harass.q.undertower", "   Q enemy under tower only if their health % under").SetValue(new Slider(40)));
                harassMenu.AddItem(new MenuItem("harass.q.lastsecond", "   Use Q to target always before W buff ends (range doesnt matter)").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.q.gc", "Use Q to gapclose (killable minions)").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.w", "Use W").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.e", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.e.logic", "   advanced logic").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.r", "Use R").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.r.weave", "   sheen synergy").SetValue(true)); 
                harassMenu.AddItem(new MenuItem("harass.mana", "Mana manager (%)").SetValue(new Slider(40, 1)));
                Config.AddSubMenu(harassMenu);
            }
            var laneclearMenu = new Menu("Laneclear", "Laneclear settings");
            {
                laneclearMenu.AddItem(new MenuItem("laneclear.q", "Use Q").SetValue(true));
                laneclearMenu.AddItem(new MenuItem("laneclear.r", "Use R").SetValue(true));
                laneclearMenu.AddItem(new MenuItem("laneclear.r.minimum", "   minimum minions").SetValue(new Slider(2, 1, 6)));
                laneclearMenu.AddItem(new MenuItem("laneclear.mana", "Mana manager (%)").SetValue(new Slider(40, 1)));
                Config.AddSubMenu(laneclearMenu);
            }
            var drawingsMenu = new Menu("Drawings", "Drawings settings");
            {
                drawingsMenu.AddItem(new MenuItem("drawings.q", "Draw Q").SetValue(true));
                drawingsMenu.AddItem(new MenuItem("drawings.e", "Draw E").SetValue(true));
                drawingsMenu.AddItem(new MenuItem("drawings.r", "Draw R").SetValue(true));
                var dmgAfterCombo = new MenuItem("dmgAfterCombo", "Draw combo damage on target").SetValue(true);
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.DamageToUnit = Program.ComboDamage;
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = dmgAfterCombo.GetValue<bool>();
                drawingsMenu.AddItem(dmgAfterCombo);
                Config.AddSubMenu(drawingsMenu);
            }
            var miscMenu = new Menu("Misc", "Misc. settings"); // R to heal
            {
                miscMenu.AddItem(new MenuItem("misc.ks.q", "Killsteal Q").SetValue(true));
                miscMenu.AddItem(new MenuItem("misc.ks.e", "Killsteal E").SetValue(true));
                miscMenu.AddItem(new MenuItem("misc.ks.r", "Killsteal R").SetValue(true));
                miscMenu.AddItem(new MenuItem("misc.age", "Anti-Gapclose E").SetValue(true));
                miscMenu.AddItem(new MenuItem("misc.interrupt", "Stun interruptable spells").SetValue(true));
                miscMenu.AddItem(new MenuItem("misc.stunundertower", "Stun enemy with tower aggro").SetValue(true));
                Config.AddSubMenu(miscMenu);
            }
            var fleeMEnu = new Menu("Flee", "Flee settings");
            {
                fleeMEnu.AddItem(new MenuItem("flee.q", "Use Q").SetValue(true));
                fleeMEnu.AddItem(new MenuItem("flee.e", "Use E").SetValue(true));
                fleeMEnu.AddItem(new MenuItem("flee.r", "Use R").SetValue(true));
                fleeMEnu.AddItem(
                    new MenuItem("flee", "Flee!").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
                Config.AddSubMenu(fleeMEnu);
            }

            Config.AddToMainMenu();
        }
    }
}
