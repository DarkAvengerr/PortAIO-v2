using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LCS_LeBlanc.Extensions
{
    internal static class Menus
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Initialize()
        {
            Config = new Menu(":: LCS LeBlanc", ":: LCS LeBlanc", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu(":: Orbwalker Settings"));
                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    var qComboMenu = new Menu(":: Q", ":: Q");
                    {
                        qComboMenu.AddItem(new MenuItem("q.combo", "Use (Q)").SetValue(true));
                        comboMenu.AddSubMenu(qComboMenu);
                    }

                    var wComboMenu = new Menu(":: W", ":: W");
                    {
                        wComboMenu.AddItem(new MenuItem("w.combo", "Use (W)").SetValue(true));
                        wComboMenu.AddItem(new MenuItem("w.combo.back", "Back Old (W) Location ?").SetValue(true));
                        wComboMenu.AddItem(
                            new MenuItem("w.hit.chance", "(W) Hit Chance").SetValue(
                                new StringList(Utilities.HitchanceNameArray, 1)));
                        comboMenu.AddSubMenu(wComboMenu);
                    }

                    var eComboMenu = new Menu(":: E", ":: E");
                    {
                        eComboMenu.AddItem(new MenuItem("e.combo", "Use (E)").SetValue(true));
                        eComboMenu.AddItem(
                            new MenuItem("e.hit.chance", "(E) Hit Chance").SetValue(
                                new StringList(Utilities.HitchanceNameArray, 1)));
                        comboMenu.AddSubMenu(eComboMenu);
                    }

                    var rComboMenu = new Menu(":: R", ":: R");
                    {
                        rComboMenu.AddItem(new MenuItem("r.combo", "Use (R)").SetValue(true));
                        comboMenu.AddSubMenu(rComboMenu);
                    }

                    Config.AddSubMenu(comboMenu);
                }
                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {
                    var qHarassMenu = new Menu(":: Q", ":: Q");
                    {
                        qHarassMenu.AddItem(new MenuItem("q.harass", "Use (Q)").SetValue(true));
                        harassMenu.AddSubMenu(qHarassMenu);
                    }

                    var wHarassMenu = new Menu(":: W", ":: W");
                    {
                        wHarassMenu.AddItem(new MenuItem("w.harass", "Use (W)").SetValue(true));
                        harassMenu.AddSubMenu(wHarassMenu);
                    }

                    harassMenu.AddItem(
                        new MenuItem("harass.mana", "Min. Mana Percentage").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(harassMenu);
                }
                var clearMenu = new Menu(":: Clear Settings", ":: Clear Settings");
                {
                    var laneclearMenu = new Menu(":: Wave Clear", ":: Wave Clear");
                    {
                        laneclearMenu.AddItem(
                            new MenuItem("keysinfo1", "                  (Q) Settings").SetTooltip("Q Settings"));
                        laneclearMenu.AddItem(new MenuItem("q.lasthit", "Use (Q) - [lasthit] - [only siege minions]").SetValue(true));
                        laneclearMenu.AddItem(
                            new MenuItem("keysinfo2", "                  (W) Settings").SetTooltip("W Settings"));
                        laneclearMenu.AddItem(new MenuItem("w.clear", "Use (W)").SetValue(true));
                        laneclearMenu.AddItem(new MenuItem("w.hit.x.minion", "Min. Minion").SetValue(new Slider(4, 1, 5)));
                        clearMenu.AddSubMenu(laneclearMenu);
                    }

                    var jungleClear = new Menu(":: Jungle Clear", ":: Jungle Clear");
                    {
                        jungleClear.AddItem(
                            new MenuItem("keysinfo1X", "                  (Q) Settings").SetTooltip("Q Settings"));
                        jungleClear.AddItem(new MenuItem("q.jungle", "Use (Q)").SetValue(true));
                        jungleClear.AddItem(
                            new MenuItem("keysinfo2X", "                  (W) Settings").SetTooltip("W Settings"));
                        jungleClear.AddItem(new MenuItem("w.jungle", "Use (W)").SetValue(true));
                        jungleClear.AddItem(
                            new MenuItem("keysinfo3X", "                  (E) Settings").SetTooltip("E Settings"));
                        jungleClear.AddItem(new MenuItem("e.jungle", "Use (E)").SetValue(true));
                        clearMenu.AddSubMenu(jungleClear);
                    }

                    clearMenu.AddItem(
                        new MenuItem("clear.mana", "LaneClear Min. Mana Percentage").SetValue(new Slider(50, 1, 99)));
                    clearMenu.AddItem(
                        new MenuItem("jungle.mana", "Jungle Min. Mana Percentage").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(clearMenu);
                }

                var miscMenu = new Menu(":: Miscellaneous", ":: Miscellaneous");
                {
                    miscMenu.AddItem(new MenuItem("anti-gapcloser.e", "Anti-Gapcloser (E) ?").SetValue(true));
                    Config.AddSubMenu(miscMenu);
                }

                var drawMenu = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
                    var drawFill = new MenuItem("RushDrawEDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.Gold));

                    drawMenu.SubMenu(":: Damage Draws").AddItem(drawDamageMenu);
                    drawMenu.SubMenu(":: Damage Draws").AddItem(drawFill);

                    DamageIndicator.DamageToUnit = DamageCalculator.TotalDamage;
                    DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                    DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                    DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                    drawDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };

                    drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };
                    Config.AddSubMenu(drawMenu);
                }


                Config.AddItem(
                    new MenuItem("combo.mode", "Default Combo Mode :").SetValue(
                        new StringList(new[] {"QRWE", "WRQE"})));
                Config.AddItem(
                   new MenuItem("combo.style", "Combo Style :").SetValue(
                       new StringList(new[] { "Selected Target", "Auto" })));
                Config.AddItem(
                    new MenuItem("credits.x1", "                          Developed by Hikigaya").SetFontStyle(
                        FontStyle.Bold, SharpDX.Color.DodgerBlue));
                Config.AddItem(
                    new MenuItem("credits.x2", "                 " +
                                               " \u221A \u221A \u221A \u221A \u221A LCS QUALITY " +
                                               "\u221A \u221A \u221A \u221A \u221A").SetFontStyle(
                        FontStyle.Bold, SharpDX.Color.YellowGreen));

            }
            Config.AddToMainMenu();
        }
    }
}
