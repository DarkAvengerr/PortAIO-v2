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
    class CorkiMenu
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static void MenuInit()
        {
            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("corki.q.combo", "Use Q").SetValue(true)).SetTooltip("Uses Q in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("corki.e.combo", "Use E").SetValue(true)).SetTooltip("Uses E in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("corki.r.combo", "Use R").SetValue(true)).SetTooltip("Uses R in Combo", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("corki.q.harass", "Use Q").SetValue(true)).SetTooltip("Uses Q in Harass", SharpDX.Color.GreenYellow);
                harassMenu.AddItem(new MenuItem("corki.r.harass", "Use W").SetValue(true)).SetTooltip("Uses R in Harass", SharpDX.Color.GreenYellow);
                harassMenu.AddItem(new MenuItem("ezreal.harass.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                var qToggleMenu = new Menu("R Toggle", "R Toggle");
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValid))
                    {
                        qToggleMenu.AddItem(new MenuItem("corki.r.toggle." + enemy.ChampionName, "(R) " + enemy.ChampionName).SetValue(true));
                    }
                    harassMenu.AddSubMenu(qToggleMenu);
                }

                Config.AddSubMenu(harassMenu);
            }

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("corki.q.clear", "Use Q").SetValue(true)).SetTooltip("Uses Q in Clear", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("corki.q.minion.hit.count", "(Q) Min. Minion Hit").SetValue(new Slider(3, 1, 5))).SetTooltip("Minimum minion count for Q", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("corki.clear.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(clearMenu);
            }

            var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                jungleMenu.AddItem(new MenuItem("corki.q.jungle", "Use Q").SetValue(true)).SetTooltip("Uses Q in Jungle", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("corki.e.jungle", "Use E").SetValue(true)).SetTooltip("Uses E in Jungle", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("corki.r.jungle", "Use R").SetValue(true)).SetTooltip("Uses R in Jungle", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("corki.jungle.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(jungleMenu);
            }

            var killStealMenu = new Menu("KillSteal Settings", "KillSteal Settings");
            {
                killStealMenu.AddItem(new MenuItem("ezreal.q.ks", "Use Q").SetValue(true)).SetTooltip("Uses Q if Enemy Killable", SharpDX.Color.GreenYellow);
                killStealMenu.AddItem(new MenuItem("ezreal.r.ks", "Use R").SetValue(true)).SetTooltip("Uses R if Enemy Killable", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(killStealMenu);
            }

            var miscMenu = new Menu("Miscellaneous", "Miscellaneous");
            {
                var gapcloseSet = new Menu("Anti-Gapclose Settings", "Anti-Gapclose Settings");
                {
                    gapcloseSet.AddItem(new MenuItem("corki.w.gapclosex", "Anti-Gapclose").SetValue(new StringList(new[] { "On", "Off" }, 1)));
                    gapcloseSet.AddItem(new MenuItem("masterracec0mb0X", "             Custom Anti-Gapcloser")).SetFontStyle(FontStyle.Bold, SharpDX.Color.LightBlue);
                    foreach (var gapclose in AntiGapcloseSpell.GapcloseableSpells.Where(x => ObjectManager.Get<AIHeroClient>().Any(y => y.ChampionName == x.ChampionName && y.IsEnemy)))
                    {
                        gapcloseSet.AddItem(new MenuItem("gapclose." + gapclose.ChampionName, "Anti-Gapclose: " + gapclose.ChampionName + " - Spell: " + gapclose.Slot).SetValue(true));
                        gapcloseSet.AddItem(new MenuItem("gapclose.slider." + gapclose.ChampionName, "" + gapclose.ChampionName + " Priorty").SetValue(new Slider(gapclose.DangerLevel, 1, 5)));
                    }
                    miscMenu.AddSubMenu(gapcloseSet);
                }

                Config.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var skillDraw = new Menu("Skill Draws", "Skill Draws");
                {
                    skillDraw.AddItem(new MenuItem("corki.q.draw", "Q Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("corki.w.draw", "W Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("corki.e.draw", "E Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("corki.r.draw", "R Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                    drawMenu.AddSubMenu(skillDraw);
                }
                var catcherMenu = new Menu("Catch Draws", "Catch Draws");
                {
                    catcherMenu.AddItem(new MenuItem("corki.catch.line", "Draw Catch Line").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("corki.catch.circle", "Draw Catch Circle").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("corki.catch.text", "Draw Catch Text").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("corki.disable.catch", "Disable Catch Drawings").SetValue(false));
                    drawMenu.AddSubMenu(catcherMenu);
                }
                Config.AddSubMenu(drawMenu);
            }
            Config.AddToMainMenu();
        }
        public static void OrbwalkerInit()
        {
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));
        }
    }
}
