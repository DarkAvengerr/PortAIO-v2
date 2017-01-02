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
    class EzrealMenu
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static void MenuInit()
        {
            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("ezreal.q.combo", "Use Q").SetValue(true)).SetTooltip("Uses Q in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("ezreal.w.combo", "Use W").SetValue(true)).SetTooltip("Uses W in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("ezreal.r.combo", "Use R").SetValue(true)).SetTooltip("Uses R in Combo (Only Casting If Enemy Killable)", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("ezreal.min.ult.distance", "(R) Min. Distance").SetValue(new Slider(200, 200, 1000))).SetTooltip("(R) Min. Distance for Ult", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("ezreal.max.ult.distance", "(R) Max. Distance").SetValue(new Slider(1000, 1000, 2000))).SetTooltip("(R) Max. Distance for Ult", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("ezreal.q.harass", "Use Q").SetValue(true)).SetTooltip("Uses Q in Harass", SharpDX.Color.GreenYellow);
                harassMenu.AddItem(new MenuItem("ezreal.w.harass", "Use W").SetValue(true)).SetTooltip("Uses W in Harass", SharpDX.Color.GreenYellow);
                harassMenu.AddItem(new MenuItem("ezreal.harass.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                var qToggleMenu = new Menu("Q Toggle", "Q Toggle");
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValid))
                    {
                        qToggleMenu.AddItem(new MenuItem("ezreal.q.toggle." + enemy.ChampionName, "(Q) " + enemy.ChampionName).SetValue(true));
                    }
                    harassMenu.AddSubMenu(qToggleMenu);
                }

                Config.AddSubMenu(harassMenu);
            }

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("ezreal.q.clear", "Use Q").SetValue(true)).SetTooltip("Uses Q in Clear", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("ezreal.clear.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(clearMenu);
            }

            var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                jungleMenu.AddItem(new MenuItem("ezreal.q.jungle", "Use Q").SetValue(true)).SetTooltip("Uses Q in Jungle", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("ezreal.e.jungle", "Use E").SetValue(true)).SetTooltip("Uses E in Jungle (Using Mouse Position)", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("ezreal.jungle.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(jungleMenu);
            }

            var killStealMenu = new Menu("KillSteal Settings", "KillSteal Settings");
            {
                killStealMenu.AddItem(new MenuItem("ezreal.q.ks", "Use Q").SetValue(true)).SetTooltip("Uses Q if Enemy Killable", SharpDX.Color.GreenYellow);
                killStealMenu.AddItem(new MenuItem("ezreal.w.ks", "Use E").SetValue(true)).SetTooltip("Uses W if Enemy Killable", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(killStealMenu);
            }

            var miscMenu = new Menu("Miscellaneous", "Miscellaneous");
            {
                var gapcloseSet = new Menu("Anti-Gapclose Settings", "Anti-Gapclose Settings");
                {
                    gapcloseSet.AddItem(new MenuItem("ezreal.e.gapclosex", "Anti-Gapclose").SetValue(new StringList(new[] { "On", "Off" }, 1)));
                    gapcloseSet.AddItem(new MenuItem("masterracec0mb0X", "             Custom Anti-Gapcloser")).SetFontStyle(FontStyle.Bold, SharpDX.Color.LightBlue);
                    foreach (var gapclose in AntiGapcloseSpell.GapcloseableSpells.Where(x => ObjectManager.Get<AIHeroClient>().Any(y => y.ChampionName == x.ChampionName && y.IsEnemy)))
                    {
                        gapcloseSet.AddItem(new MenuItem("gapclose." + gapclose.ChampionName, "Anti-Gapclose: " + gapclose.ChampionName + " - Spell: " + gapclose.Slot).SetValue(true));
                        gapcloseSet.AddItem(new MenuItem("gapclose.slider." + gapclose.ChampionName, "" + gapclose.ChampionName + " Priorty").SetValue(new Slider(gapclose.DangerLevel, 1, 5)));
                    }
                    miscMenu.AddSubMenu(gapcloseSet);
                }
                miscMenu.AddItem(new MenuItem("ezreal.random.blue.trinket", "Random Blue Trinket ?").SetValue(false)).SetTooltip("Casting Blue Trinket to Safe Positions", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(miscMenu);
            }
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var skillDraw = new Menu("Skill Draws", "Skill Draws");
                {
                    skillDraw.AddItem(new MenuItem("ezreal.q.draw", "Q Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("ezreal.w.draw", "W Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("ezreal.e.draw", "E Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("ezreal.r.draw", "R Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    drawMenu.AddSubMenu(skillDraw);
                }
                var catcherMenu = new Menu("Catch Draws", "Catch Draws");
                {
                    catcherMenu.AddItem(new MenuItem("ezreal.catch.line", "Draw Catch Line").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("ezreal.catch.circle", "Draw Catch Circle").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("ezreal.catch.text", "Draw Catch Text").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("ezreal.disable.catch", "Disable Catch Drawings").SetValue(true));
                    drawMenu.AddSubMenu(catcherMenu);
                }
                Config.AddSubMenu(drawMenu);
            }
            var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, System.Drawing.Color.Gold));

            drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
            drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

            //DamageIndicator.DamageToUnit = Calculators.EzrealCalculator.EzrealTotalDamage;
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
