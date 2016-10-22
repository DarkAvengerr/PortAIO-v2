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
    class VayneMenu
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static void MenuInit()
        {
            var comboMenu = new Menu(":: General Settings (1337)", ":: General Settings (1337)");
            {
                comboMenu.AddItem(new MenuItem("vayne.q.combo", "Use (Q)").SetValue(true)); //+
                comboMenu.AddItem(new MenuItem("vayne.e.combo", "Use (E)").SetValue(true)); //+
                comboMenu.AddItem(new MenuItem("vayne.r.combo", "Use (R)").SetValue(true));
                comboMenu.AddItem(new MenuItem("vayne.q.type", "Tumble Method").SetValue(new StringList(new[] { "Safe", "Cursor Position" }))); //+
                comboMenu.AddItem(new MenuItem("vayne.e.type", "Condemn Method").SetValue(new StringList(new[] { "PRADASMART", "VHR:BASIC","SHINE" ,"MARKSMAN", "SHARPSHOOTER", "360" }))); //+
                comboMenu.AddItem(new MenuItem("vayne.e.push.distance", "(E) Push Distance").SetValue(new Slider(390, 300, 475))); //+
                comboMenu.AddItem(new MenuItem("vayne.condemn.jungle.mobs", "Condemn Jungle Mobs").SetValue(true)).SetTooltip("Red & Blue & Crimson Raptor & Gromp & Krug & Rift Scuttler"); // +
                comboMenu.AddItem(new MenuItem("vayne.tumble.jungle.mobs", "Tumble Jungle Mobs").SetValue(true)).SetTooltip("Red & Blue & Crimson Raptor & Gromp & Krug & Rift Scuttler"); // +
                comboMenu.AddItem(new MenuItem("vayne.q.after.aa", "(Q) -> After -> (AA) ?").SetValue(true)); // +
                comboMenu.AddItem(new MenuItem("vayne.auto.r.enemy.count", "Auto (R) Enemy Count").SetValue(new Slider(5, 1, 5)));
                comboMenu.AddItem(new MenuItem("vayne.auto.r.search.range", "Auto (R) Search Range").SetValue(new Slider(700, 1, 2000)));
                comboMenu.AddItem(new MenuItem("vayne.auto.r.minimum.health", "Auto (R) Minimum Health").SetValue(new Slider(69, 1, 99)));
                comboMenu.AddItem(new MenuItem("vayne.auto.q.if.enemy.has.2.stack", "Auto (Q) If Enemy Has 2 Silver Bolt").SetValue(false)).SetTooltip("Casting (Q) to Safe Position"); //+
                Config.AddSubMenu(comboMenu);
            }

            var miscMenu = new Menu(":: Miscellaneous", ":: Miscellaneous");
            {
                var gapcloseSet = new Menu("Anti-Gapclose Settings", "Anti-Gapclose Settings");
                {
                    gapcloseSet.AddItem(new MenuItem("vayne.e.gapclosex", "(E) Anti-Gapclose").SetValue(new StringList(new[] { "On", "Off" }, 1)));
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
            
            var drawMenu = new Menu(":: Draw Settings", ":: Draw Settings");
            {
                var skillDraw = new Menu("Skill Draws", "Skill Draws");
                {
                    skillDraw.AddItem(new MenuItem("vayne.q.draw", "Q Range").SetValue(new Circle(false, System.Drawing.Color.Gold)));
                    skillDraw.AddItem(new MenuItem("vayne.e.draw", "E Range").SetValue(new Circle(false, System.Drawing.Color.Gold)));
                    drawMenu.AddSubMenu(skillDraw);
                }
                var catcherMenu = new Menu("Catch Draws", "Catch Draws");
                {
                    catcherMenu.AddItem(new MenuItem("vayne.catch.line", "Draw Catch Line").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("vayne.catch.circle", "Draw Catch Circle").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("vayne.catch.text", "Draw Catch Text").SetValue(true));
                    catcherMenu.AddItem(new MenuItem("vayne.disable.catch", "Disable Catch Drawings").SetValue(true));
                    drawMenu.AddSubMenu(catcherMenu);
                }
                Config.AddSubMenu(drawMenu);
            }
            Config.AddItem(new MenuItem("harass.type", "Harass Method").SetValue(new StringList(new[] { "AA -> AA -> (Q) Harass", "AA -> AA -> (E) Harass" })));
            Config.AddItem(new MenuItem("vayne.harass.mana", "Min. Mana Percent").SetValue(new Slider(450, 300, 475))).SetTooltip("Manage your mana for harass");
            Config.AddToMainMenu();
        }
        public static void OrbwalkerInit()
        {
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));
        }
    }
}
