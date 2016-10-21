using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso.Extensions
{
    static class Menus
    {
        /// <summary>
        /// Orbwalker shit
        /// </summary>
        public static Orbwalking.Orbwalker Orbwalker;

        /// <summary>
        /// Menu
        /// </summary>
        public static Menu Config;

        /// <summary>
        /// Initialize menu
        /// </summary>
        public static void Initialize()
        {
            Config = new Menu(":: Jhin - The Virtuoso", ":: Jhin - The Virtuoso", true);
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
                        wComboMenu.AddItem(
                            new MenuItem("w.combo.min.distance", "Min. Distance").SetValue(new Slider(400, 1, 2500)));
                        wComboMenu.AddItem(
                            new MenuItem("w.combo.max.distance", "Max. Distance").SetValue(new Slider(1000, 1, 2500)));
                        wComboMenu.AddItem(new MenuItem("w.passive.combo", "Use (W) If Enemy Is Marked").SetValue(false));
                        wComboMenu.AddItem(
                            new MenuItem("w.hit.chance", "(W) Hit Chance").SetValue(
                                new StringList(Provider.HitchanceNameArray, 2)));
                        comboMenu.AddSubMenu(wComboMenu);
                    }

                    var eComboMenu = new Menu(":: E", ":: E");
                    {
                        eComboMenu.AddItem(new MenuItem("e.combo", "Use (E)").SetValue(true));
                        eComboMenu.AddItem(new MenuItem("e.combo.teleport", "Auto (E) Teleport").SetValue(true));
                        eComboMenu.AddItem(
                            new MenuItem("e.hit.chance", "(E) Hit Chance").SetValue(
                                new StringList(Provider.HitchanceNameArray, 2)));
                        comboMenu.AddSubMenu(eComboMenu);
                    }

                    Config.AddSubMenu(comboMenu);
                }
                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {
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
                        laneclearMenu.AddItem(new MenuItem("q.clear", "Use (Q)").SetValue(true));
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
                        clearMenu.AddSubMenu(jungleClear);
                    }
                    clearMenu.AddItem(
                        new MenuItem("clear.mana", "LaneClear Min. Mana Percentage").SetValue(new Slider(50, 1, 99)));
                    clearMenu.AddItem(
                        new MenuItem("jungle.mana", "Jungle Min. Mana Percentage").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(clearMenu);
                }

                var ksMenu = new Menu(":: Kill Steal", ":: Kill Steal");
                {
                    ksMenu.AddItem(new MenuItem("q.ks", "Use (Q)").SetValue(true));
                    ksMenu.AddItem(new MenuItem("w.ks", "Use (W)").SetValue(true));
                    Config.AddSubMenu(ksMenu);
                }

                var miscMenu = new Menu(":: Miscellaneous", ":: Miscellaneous");
                {
                    miscMenu.AddItem(new MenuItem("auto.e.immobile", "Auto Cast (E) Immobile Target").SetValue(true));
                    //miscMenu.AddItem(new MenuItem("ezevade.hijacker", "ezEvade Hijacker").SetValue(true)).SetTooltip("When Jhin using (R) Disabling ezEvade for max. damage ");
                    //miscMenu.AddItem(new MenuItem("evadesharp.hijacker", "Evade# Hijacker").SetValue(true)).SetTooltip("When Jhin using (R) Disabling Evade# for max. damage ");
                    Config.AddSubMenu(miscMenu);
                }
                var rComboMenu = new Menu(":: Ultimate Settings", ":: Ultimate Settings").SetFontStyle(FontStyle.Bold,
                    SharpDX.Color.Yellow);
                {
                    var rComboWhiteMenu = new Menu(":: R - Whitelist", ":: R - Whitelist");
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValid))
                        {
                            rComboWhiteMenu.AddItem(
                                new MenuItem("r.combo." + enemy.ChampionName, "(R): " + enemy.ChampionName).SetValue(
                                    true));
                        }
                        rComboMenu.AddSubMenu(rComboWhiteMenu);
                    }
                    rComboMenu.AddItem(new MenuItem("r.combo", "Use (R)").SetValue(true));
                    rComboMenu.AddItem(
                        new MenuItem("auto.shoot.bullets", "If Jhin Casting (R) Auto Cast Bullets").SetValue(true));
                    rComboMenu.AddItem(
                        new MenuItem("r.hit.chance", "(R) Hit Chance").SetValue(
                            new StringList(Provider.HitchanceNameArray, 1)));
                    Config.AddSubMenu(rComboMenu);
                }

                var drawMenu = new Menu(":: Drawings", ":: Drawings");
                {
                    var damageDraw = new Menu(":: Damage Draw", ":: Damage Draw");
                    {
                        damageDraw.AddItem(
                            new MenuItem("aa.indicator", "(AA) Indicator").SetValue(new Circle(true, Color.Gold)));
                        damageDraw.AddItem(
                           new MenuItem("sniper.text", "Sniper Text").SetValue(new Circle(true, Color.Gold)));
                        drawMenu.AddSubMenu(damageDraw);
                    }
                    drawMenu.AddItem(new MenuItem("q.draw", "(Q) Range").SetValue(new Circle(false, Color.White)));
                    drawMenu.AddItem(new MenuItem("w.draw", "(W) Range").SetValue(new Circle(false, Color.Gold)));
                    drawMenu.AddItem(new MenuItem("e.draw", "(E) Range").SetValue(new Circle(false, Color.DodgerBlue)));
                    drawMenu.AddItem(new MenuItem("r.draw", "(R) Range").SetValue(new Circle(false, Color.GreenYellow)));
                    Config.AddSubMenu(drawMenu);
                }
                Config.AddItem(
                    new MenuItem("semi.manual.ult", "Semi-Manual (R)!").SetValue(new KeyBind("A".ToCharArray()[0],
                        KeyBindType.Press)));
                Config.AddItem(new MenuItem("use.combo", "Combo (Active)").SetValue(new KeyBind(32, KeyBindType.Press)));
                Config.AddItem(
                    new MenuItem("credits.x1", "                          Developed by Hikigaya").SetFontStyle(
                        FontStyle.Bold, SharpDX.Color.DodgerBlue));
                Config.AddItem(
                    new MenuItem("credits.x2", "       Dont forget to Upvote on Assembly Database").SetFontStyle(
                        FontStyle.Bold, SharpDX.Color.YellowGreen));

            }
            Config.AddToMainMenu();
        }
    }
}
  
