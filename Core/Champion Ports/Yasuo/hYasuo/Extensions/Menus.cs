using System.Drawing;
using System.Linq;
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
                    comboMenu.AddItem(new MenuItem("q3.combo", "Use (Q3)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("min.q3.combo", "Use (Q3)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Use (E)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("eqq.combo", "Try (E-Q) Always").SetValue(true));
                    //comboMenu.AddItem(new MenuItem("enemy.check.combo", "Dont Spam (E) ?").SetValue(true)).SetTooltip("If Enemy in E Range and Have Yasuo E Debuff, It not uses E for gapclose anymore");
                    comboMenu.AddItem(new MenuItem("disable.e.safety", "Disable (E) Safety").SetValue(false)).SetTooltip("It disables e safety check. 10/10 for under turret dives");
                    comboMenu.AddItem(new MenuItem("r.combo", "Use (R)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("min.r.count", "Min. (R) Count").SetValue(new Slider(3, 1, 5)));
                    Config.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {

                    var toggleMenu = new Menu(":: Toggle Settings", ":: Toggle Settings").SetFontStyle(FontStyle.Bold, Color.Gold);
                    {
                        toggleMenu.AddItem(new MenuItem("q.toggle", "Use (Q)").SetValue(true));
                        toggleMenu.AddItem(new MenuItem("q3.toggle", "Use (Q3)").SetValue(true));
                        toggleMenu.AddItem(new MenuItem("toggle.active", "Toggle !").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Toggle)));
                        harassMenu.AddSubMenu(toggleMenu);
                    }

                    harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)").SetValue(true));
                    harassMenu.AddItem(new MenuItem("q3.harass", "Use (Q3)").SetValue(true));
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

                        laneclearMenu.AddItem(new MenuItem("q3.clear", "Use (Q2)").SetValue(true));
                        laneclearMenu.AddItem(new MenuItem("q3.hit.x.minion", "(Q2) Min. Minion").SetValue(new Slider(2, 1, 5)));
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
                        jungleClear.AddItem(new MenuItem("q3.jungle", "Use (Q3)").SetValue(true));
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

                var esettings = new Menu(":: (1337) Dodge Settings", "::  (1337) Dodge Settings").SetFontStyle(FontStyle.Bold, Color.HotPink);
                {
                    esettings.AddItem(
                           new MenuItem("213123123", "         DISABLE (W) USAGE IN EZEVADE OR EVADE#").SetTooltip("DISABLE (W) USAGE IN EZEVADE OR EVADE#"));

                    var windwall = new Menu("(WINDWALL)","(WINDWALL)");
                    {
                        var evademenu = new Menu(":: Protectable Skillshots", ":: Protectable Skillshots");
                        {
                            foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
                            {
                                evademenu.AddItem(new MenuItem($"w.protect.{spell.SpellName}",
                                    $"{spell.ChampionName} ({spell.Slot})").SetValue(true));
                            }
                            windwall.AddSubMenu(evademenu);
                        }

                        var targettedmenu = new Menu(":: Protectable Targetted Spells", ":: Protectable Targetted Spells");
                        {
                            foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.WindwallDodgeable)))
                            {
                                targettedmenu.AddItem(new MenuItem($"windwall.targetted.{spell.SpellName}",
                                    $"{spell.ChampionName} ({spell.Slot})").SetValue(true));
                            }
                            windwall.AddSubMenu(targettedmenu);
                        }

                        esettings.AddSubMenu(windwall);
                    }

                    var q3Dodge = new Menu("(Q3 TARGETTED)", "(Q3) TARGETTED");
                    {
                        var targettedmenu = new Menu(":: Protectable Targetted Spells", ":: Protectable Targetted Spells");
                        {
                            foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.YasuoQ3Dodgeable)))
                            {
                                targettedmenu.AddItem(new MenuItem($"q3.target.{spell.SpellName}",
                                    $"{spell.ChampionName} ({spell.Slot})").SetValue(true));
                            }
                            q3Dodge.AddSubMenu(targettedmenu);
                        }
                        esettings.AddSubMenu(q3Dodge);
                    }
                    esettings.AddItem(new MenuItem("use.ww.incomingdamage", "Use (WINDWALL) for Incoming Damage ").SetValue(true))
                        .SetTooltip("If enemy damage > yasuo health. yasuo uses w for protect himself from enemy skillshots.");
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
                Config.AddItem(new MenuItem("q.type", "Q Type").SetValue(new StringList(new[] { "Normal", "After Attack" }, 1))).SetTooltip("not supports empowered q");
                Config.AddItem(new MenuItem("q.hitchance", ":: (Q) HITCHANCE").SetValue(new StringList(Utilities.HitchanceNameArray, 2))).SetFontStyle(FontStyle.Bold,Color.Crimson);
                Config.AddItem(new MenuItem("q3.hitchance", ":: (Q3) HITCHANCE").SetValue(new StringList(Utilities.HitchanceNameArray, 2))).SetFontStyle(FontStyle.Bold, Color.Crimson);
                Config.AddItem(new MenuItem("flee.key", "(FLEE)").SetValue(new KeyBind('Z', KeyBindType.Press)).SetTooltip("uses game cursor pos"));

                Config.AddItem(new MenuItem("prediction", ":: Choose Prediction").SetValue(new StringList(new[] { "Common", "Sebby", "sPrediction", "SDK" }, 1)))
                    .ValueChanged += (s, ar) =>
                    {
                        Config.Item("pred.info").Show(ar.GetNewValue<StringList>().SelectedIndex == 2);
                    };
                Config.AddItem(new MenuItem("pred.info", "                 PRESS F5 FOR LOAD SPREDICTION").SetFontStyle(System.Drawing.FontStyle.Bold))
                    .Show(Config.Item("prediction").GetValue<StringList>().SelectedIndex == 2);

                if (Config.Item("prediction").GetValue<StringList>().SelectedIndex == 2)
                {
                    SPrediction.Prediction.Initialize(Config, ":: SPREDICTION");
                }

                Config.AddItem(
                    new MenuItem("credits.x1", "                          Developed by Hikigaya").SetFontStyle(
                        FontStyle.Bold, Color.DodgerBlue));

            }
            Config.AddToMainMenu();
        }
    }
}

