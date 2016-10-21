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
    class DravenMenu
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static void MenuInit()
        {
            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("draven.q.combo", "Use Q").SetValue(true)).SetTooltip("Uses Q in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("draven.q.combo.axe.count", "Min. Axe Count").SetValue(new Slider(2, 1, 2))).SetTooltip("Min axe count for combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("draven.w.combo", "Use W").SetValue(true)).SetTooltip("Uses W in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("draven.e.combo", "Use E").SetValue(true)).SetTooltip("Uses E in Combo", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("draven.r.combo", "Use R").SetValue(true)).SetTooltip("Uses R in Combo (Only Casting If Enemy Killable)", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("draven.min.ult.distance", "(R) Min. Distance").SetValue(new Slider(200, 200, 1000))).SetTooltip("(R) Min. Distance for Ult", SharpDX.Color.GreenYellow);
                comboMenu.AddItem(new MenuItem("draven.max.ult.distance", "(R) Max. Distance").SetValue(new Slider(2000, 1000, 3000))).SetTooltip("(R) Max. Distance for Ult", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(comboMenu);
            }

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("draven.q.clear", "Use Q").SetValue(true)).SetTooltip("Uses Q in Clear", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("draven.q.lane.clear.axe.count", "Min. Axe Count").SetValue(new Slider(1, 1, 2))).SetTooltip("Min. Axe Count", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("draven.q.minion.count", "Min. Minion Count").SetValue(new Slider(4, 1, 10))).SetTooltip("Minimum Minion Count For Laneclear", SharpDX.Color.GreenYellow);
                clearMenu.AddItem(new MenuItem("draven.clear.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(clearMenu);
            }

            var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
            {
                jungleMenu.AddItem(new MenuItem("draven.q.jungle", "Use Q").SetValue(true)).SetTooltip("Uses Q in Jungle", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("draven.q.jungle.clear.axe.count", "Min. Axe Count").SetValue(new Slider(1, 1, 2))).SetTooltip("Min. Axe Count", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("draven.e.jungle", "Use E").SetValue(true)).SetTooltip("Uses E in Jungle (Using Mouse Position)", SharpDX.Color.GreenYellow);
                jungleMenu.AddItem(new MenuItem("draven.jungle.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("Manage your Mana!", SharpDX.Color.GreenYellow);
                Config.AddSubMenu(jungleMenu);
            }

            var miscMenu = new Menu("Miscellaneous", "Miscellaneous");
            {
                var gapcloseSet = new Menu("Anti-Gapclose Settings", "Anti-Gapclose Settings");
                {
                    gapcloseSet.AddItem(new MenuItem("draven.e.gapclose", "(E) Anti-Gapclose").SetValue(true));
                    miscMenu.AddSubMenu(gapcloseSet);
                }
                var interrupterSet = new Menu("Interrupter Settings", "Interrupter Settings");
                {
                    interrupterSet.AddItem(new MenuItem("draven.e.interrupter", "(E) Interrupter").SetValue(true));
                    interrupterSet.AddItem(
                        new MenuItem("min.interrupter.danger.level", "Interrupter Danger Level").SetValue(
                            new StringList(new[] { "HIGH", "MEDIUM", "LOW" })));
                    miscMenu.AddSubMenu(interrupterSet);
                }

                Config.AddSubMenu(miscMenu);
            }
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                var skillDraw = new Menu("Skill Draws", "Skill Draws");
                {
                    skillDraw.AddItem(new MenuItem("DE", "Draw E Range").SetValue(new Circle(false, System.Drawing.Color.White)));
                    skillDraw.AddItem(new MenuItem("DR", "Draw R Range").SetValue(new Circle(false, System.Drawing.Color.White)));
                    drawMenu.AddSubMenu(skillDraw);
                }
                var axeDraw = new Menu("Axe Draws", "Axe Draws");
                {
                    axeDraw.AddItem(new MenuItem("DCR", "Draw Catch Radius").SetValue(new Circle(true, System.Drawing.Color.White)));
                    axeDraw.AddItem(new MenuItem("DAR", "Draw Axe Spots").SetValue(new Circle(true, System.Drawing.Color.White)));
                    drawMenu.AddSubMenu(axeDraw);
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
