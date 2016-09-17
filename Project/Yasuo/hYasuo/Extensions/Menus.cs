using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hYasuo.Extensions
{
    internal static class Menus
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Initialize()
        {
            Config = new Menu(":: hYasuo", ":: hYasuo", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu(":: Orbwalker Settings"));

                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("q2.combo", "Use (Q2)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Use (E)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.combo", "Use (R)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("min.r.count", "Min. (R) Count").SetValue(new Slider(3, 1, 5)));
                    Config.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)").SetValue(true));
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
                        laneclearMenu.AddItem(new MenuItem("q.hit.x.minion", "(Q) Min. Minion").SetValue(new Slider(2, 1, 5)));

                        laneclearMenu.AddItem(new MenuItem("q2.clear", "Use (Q2)").SetValue(true));
                        laneclearMenu.AddItem(new MenuItem("q2.hit.x.minion", "(Q2) Min. Minion").SetValue(new Slider(2, 1, 5)));
                        laneclearMenu.AddItem(
                            new MenuItem("keysinfo2", "                  (E) Settings").SetTooltip("E Settings"));
                        laneclearMenu.AddItem(new MenuItem("e.clear", "Use (E)").SetValue(true));
                        clearMenu.AddSubMenu(laneclearMenu);
                    }

                    var jungleClear = new Menu(":: Jungle Clear", ":: Jungle Clear");
                    {
                        jungleClear.AddItem(
                            new MenuItem("keysinfo1X", "                  (Q) Settings").SetTooltip("Q Settings"));
                        jungleClear.AddItem(new MenuItem("q.jungle", "Use (Q)").SetValue(true));
                        jungleClear.AddItem(new MenuItem("q2.jungle", "Use (Q)").SetValue(true));
                        jungleClear.AddItem(
                            new MenuItem("keysinfo3X", "                  (E) Settings").SetTooltip("E Settings"));
                        jungleClear.AddItem(new MenuItem("e.jungle", "Use (E)").SetValue(true));
                        clearMenu.AddSubMenu(jungleClear);
                    }

                    var lasthit = new Menu(":: Last Hit", ":: Last Hit");
                    {
                        lasthit.AddItem(
                            new MenuItem("keysinfo4X", "                  (Q) Settings").SetTooltip("Q Settings"));
                        lasthit.AddItem(new MenuItem("q.lasthit", "Use (Q)").SetValue(true));
                        lasthit.AddItem(
                            new MenuItem("keysinfo5X", "                  (E) Settings").SetTooltip("E Settings"));
                        lasthit.AddItem(new MenuItem("e.lasthit", "Use (E)").SetValue(true));
                        clearMenu.AddSubMenu(lasthit);
                    }
                    Config.AddSubMenu(clearMenu);
                }

                var esettings = new Menu(":: (WINDWALL) Settings", ":: (WINDWALL) Settings").SetFontStyle(FontStyle.Bold, Color.HotPink);
                {
                    esettings.AddItem(
                           new MenuItem("213123123", "                  NOT INCLUDED YET").SetTooltip("NOT INCLUDED USE EZEVADE"));
                    var evademenu = new Menu(":: Protectable Skillshots", ":: Protectable Skillshots");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
                        {
                            evademenu.AddItem(new MenuItem(string.Format("w.protect.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        esettings.AddSubMenu(evademenu);
                    }
                    var targettedmenu = new Menu(":: Protectable Targetted Spells", ":: Protectable Targetted Spells");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted)))
                        {
                            targettedmenu.AddItem(new MenuItem(string.Format("q2.protect.targetted.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        esettings.AddSubMenu(targettedmenu);
                    }

                    Config.AddSubMenu(esettings);
                }

                var ultimatewhitelist = new Menu(":: (R) Whitelist", ":: (R) Whitelist");
                {
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        ultimatewhitelist.AddItem(new MenuItem("r."+enemy.ChampionName, "(R) "+enemy.ChampionName).SetValue(Utilities.HighChamps.Contains(enemy.ChampionName)));
                    }
                    Config.AddSubMenu(ultimatewhitelist);
                }

                var killstealsettings = new Menu(":: Killsteal Settings", ":: Killsteal Settings");
                {
                    killstealsettings.AddItem(
                            new MenuItem("ks.q", "                  (Q) KS Settings").SetTooltip("(Q) KS Settings"));

                    killstealsettings.AddItem(new MenuItem("q.ks", "(Q) KS")).SetValue(true);
                    killstealsettings.AddItem(new MenuItem("q2.ks", "(Q2) KS")).SetValue(true);
                    
                    ////////////////////////////
                    killstealsettings.AddItem(
                            new MenuItem("ks.e", "                  (E) KS Settings").SetTooltip("(R) KS Settings"));

                    killstealsettings.AddItem(new MenuItem("e.ks", "(E) KS")).SetValue(true);
                    killstealsettings.AddItem(new MenuItem("e.ks.safety.range", "(E) KS Safety Range").SetValue(new Slider(1000, 1, 1110)));
                    
                    ////////////////////////////
                    killstealsettings.AddItem(
                            new MenuItem("ks.r", "                  (R) KS Settings").SetTooltip("(R) KS Settings"));

                    killstealsettings.AddItem(new MenuItem("r.ks", "(R) KS")).SetValue(true);
                    killstealsettings.AddItem(new MenuItem("r.ks.safety.range", "(R) KS Safety Range").SetValue(new Slider(1000, 1, 1110)));
                    ////////////////////////////
                    killstealsettings.AddItem(
                            new MenuItem("ks.general", "                  (GENERAL) KS Settings").SetTooltip("(GENERAL) KS Settings"));
                    killstealsettings.AddItem(new MenuItem("ks.status", "KS Enabled ?")).SetValue(true);

                    Config.AddSubMenu(killstealsettings);
                }

                var miscMenu = new Menu(":: Miscellaneous", ":: Miscellaneous");
                {
                    miscMenu.AddItem(new MenuItem("anti-gapcloser.e", "Anti-Gapcloser (E) ?").SetValue(true));
                    miscMenu.AddItem(new MenuItem("auto.stack", "Auto Stack (Q)").SetValue(true));
                    Config.AddSubMenu(miscMenu);
                }
                Config.AddItem(new MenuItem("q.hitchance", ":: (Q) HITCHANCE").SetValue(new StringList(Utilities.HitchanceNameArray, 2))).SetFontStyle(FontStyle.Bold,Color.Crimson);
                Config.AddItem(new MenuItem("q2.hitchance", ":: (Q2) HITCHANCE").SetValue(new StringList(Utilities.HitchanceNameArray, 2))).SetFontStyle(FontStyle.Bold, Color.Crimson);


                Config.AddItem(
                    new MenuItem("credits.x1", "                          Developed by Hikigaya").SetFontStyle(
                        FontStyle.Bold, Color.DodgerBlue));

                /*Config.AddItem(
                new MenuItem("credits.x3", "                 " +
                                           " \u221A \u221A \u221A \u221A \u221A #FREEKARL " +
                                           "\u221A \u221A \u221A \u221A \u221A").SetFontStyle(
                    FontStyle.Bold, Color.Crimson));*/

            }
            Config.AddToMainMenu();
        }
    }
}

