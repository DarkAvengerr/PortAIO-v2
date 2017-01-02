using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Menus
{
    class SivirMenu
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static void MenuInit()
        {
            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("sivir.q.combo", "Use Q").SetValue(true)).SetTooltip("Uses Q in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("sivir.w.combo", "Use W").SetValue(true)).SetTooltip("Uses W in Combo", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("sivir.q.harass", "Use Q").SetValue(true)).SetTooltip("Uses Q in Harass", SharpDX.Color.GreenYellow);
                harassMenu.AddItem(new MenuItem("sivir.w.harass", "Use W").SetValue(true)).SetTooltip("Uses W in Harass", SharpDX.Color.GreenYellow);
                harassMenu.AddItem(new MenuItem("sivir.harass.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                var qToggleMenu = new Menu("Q Toggle", "Q Toggle");
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValid))
                    {
                        qToggleMenu.AddItem(new MenuItem("sivir.q.toggle." + enemy.ChampionName, "(Q) " + enemy.ChampionName).SetValue(true));
                    }

                    harassMenu.AddSubMenu(qToggleMenu);
                }

                Config.AddSubMenu(harassMenu);
            }

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("sivir.q.clear", "Use Q").SetValue(true)).SetTooltip("Uses Q in Clear", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("sivir.q.minion.hit.count", "(Q) Min. Minion Hit").SetValue(new Slider(3, 1, 5))).SetTooltip("Minimum minion count for Q", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("sivir.clear.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(clearMenu);
            }

            var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                jungleMenu.AddItem(new MenuItem("sivir.q.jungle", "Use Q").SetValue(true)).SetTooltip("Uses Q in Jungle", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("sivir.w.jungle", "Use W").SetValue(true)).SetTooltip("Uses W in Jungle", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("sivir.jungle.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(jungleMenu);
            }
            var miscMenu = new Menu("Miscellaneous", "Miscellaneous");
            {
                var gapcloseSet = new Menu("(E) Spell Block", "(E) Spell Block");
                {
                    gapcloseSet.AddItem(new MenuItem("sivir.e.spell.block", "(E) Spell Block ?").SetValue(new StringList(new[] { "On", "Off" })));
                    gapcloseSet.AddItem(new MenuItem("masterracec0mb0X", "                     Blockable Spells")).SetFontStyle(FontStyle.Bold, SharpDX.Color.LightBlue);

                    foreach (var spellblock in EvadeDb.SpellData.SpellDatabase.Spells.Where(x => ObjectManager.Get<AIHeroClient>().Any(y => y.ChampionName == x.charName && y.IsEnemy)))
                    {
                        gapcloseSet.AddItem(new MenuItem("block." + spellblock.spellName, "(E) Block: " + spellblock.charName + " - Spell: " + spellblock.spellKey).SetValue(true));
                    }
                    miscMenu.AddSubMenu(gapcloseSet);
                }

                Config.AddSubMenu(miscMenu);
            }
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var skillDraw = new Menu("Skill Draws", "Skill Draws");
                {
                    skillDraw.AddItem(new MenuItem("sivir.q.draw", "Q Range").SetValue(new Circle(false, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("sivir.w.draw", "W Range").SetValue(new Circle(false, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("sivir.e.draw", "E Range").SetValue(new Circle(false, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("sivir.r.draw", "R Range").SetValue(new Circle(false, System.Drawing.Color.Gold)));
                    drawMenu.AddSubMenu(skillDraw);
                }
                var catcherMenu = new Menu("Catch Draws", "Catch Draws");
                {
                    catcherMenu.AddItem(new MenuItem("corki.catch.line", "Draw Catch Line").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("corki.catch.circle", "Draw Catch Circle").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("corki.catch.text", "Draw Catch Text").SetValue(true));
                    drawMenu.AddSubMenu(catcherMenu);
                }
                Config.AddSubMenu(drawMenu);
            }
            var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, System.Drawing.Color.Gold));

            drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
            drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

            //DamageIndicator.DamageToUnit = Calculators.SivirCalculator.SivirTotalDamage;
            //DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
            //DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            //DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

            drawDamageMenu.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            Config.AddToMainMenu();
        }
        public static void OrbwalkerInit()
        {
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));
        }
    }
}
