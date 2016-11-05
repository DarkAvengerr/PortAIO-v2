using System;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Mundo
{
    internal class ConfigMenu : Spells
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;

        public static void InitializeMenu()
        {
            try
            {
                config = new Menu(ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName, true);

                //Adds the Orbwalker to the main menu
                var orbwalkerMenu = config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                orbwalker.RegisterCustomMode("flee", "Flee", (uint) Keys.Z);

                var combo = config.AddSubMenu(new Menu("Combo Settings", "Combo"));

                var comboQ = combo.AddSubMenu(new Menu("Q Settings", "Q"));
                comboQ.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
                comboQ.AddItem(new MenuItem("QHealthCombo", "Minimum HP% to use Q").SetValue(new Slider(10, 1)));

                var comboW = combo.AddSubMenu(new Menu("W Settings", "W"));
                comboW.AddItem(new MenuItem("useW", "Use W").SetValue(true));
                comboW.AddItem(new MenuItem("WHealthCombo", "Minimum HP% to use W").SetValue(new Slider(25, 1)));

                var comboE = combo.AddSubMenu(new Menu("E Settings", "E"));
                comboE.AddItem(new MenuItem("useE", "Use E").SetValue(true));

                var harass = config.AddSubMenu(new Menu("Harass Settings", "Harass"));

                var harassQ = harass.AddSubMenu(new Menu("Q Settings", "Q"));
                harassQ.AddItem(new MenuItem("useQHarass", "Use Q").SetValue(true));
                harassQ.AddItem(new MenuItem("useQHarassHP", "Minimum HP% to use Q").SetValue(new Slider(50, 1)));

                var killsteal = config.AddSubMenu(new Menu("KillSteal Settings", "KillSteal"));
                killsteal.AddItem(new MenuItem("killsteal", "Activate KillSteal").SetValue(true));
                killsteal.AddItem(new MenuItem("useQks", "Use Q to KillSteal").SetValue(true));
                killsteal.AddItem(new MenuItem("useIks", "Use Ignite to KillSteal").SetValue(true));

                var misc = config.AddSubMenu(new Menu("Misc Settings", "Misc"));

                var miscQ = misc.AddSubMenu(new Menu("Q Settings", "Q"));
                miscQ.AddItem(
                    new MenuItem("autoQ", "Auto Q on enemies").SetValue(
                        new KeyBind("J".ToCharArray()[0], KeyBindType.Toggle)));
                miscQ.AddItem(new MenuItem("autoQhp", "Minimum HP% to auto Q").SetValue(new Slider(50, 1)));
                miscQ.AddItem(
                    new MenuItem("hitchanceQ", "Global Q Hitchance").SetValue(
                        new StringList(
                            new[]
                            {
                                HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(),
                                HitChance.VeryHigh.ToString()
                            }, 3)));
                var miscW = misc.AddSubMenu(new Menu("W Settings", "W"));
                miscW.AddItem(new MenuItem("handleW", "Automatically handle W").SetValue(true));

                var miscR = misc.AddSubMenu(new Menu("R Settings", "R"));
                miscR.AddItem(new MenuItem("useR", "Use R").SetValue(true));
                miscR.AddItem(new MenuItem("RHealth", "Minimum HP% to use R").SetValue(new Slider(20, 1)));
                miscR.AddItem(new MenuItem("RHealthEnemies", "If enemies nearby").SetValue(true));

                var miscItems = misc.AddSubMenu(new Menu("Items settings", "Items"));
                miscItems.AddItem(new MenuItem("titanicC", "Use titanic Hydra in combo").SetValue(true));
                miscItems.AddItem(new MenuItem("tiamatC", "Use Tiamat in combo").SetValue(true));
                miscItems.AddItem(new MenuItem("ravenousC", "Use Ravenous Hydra in combo").SetValue(true));
                miscItems.AddItem(new MenuItem("titanicF", "Use titanic Hydra in Farm").SetValue(true));
                miscItems.AddItem(new MenuItem("tiamatF", "Use Tiamat in Farm").SetValue(true));
                miscItems.AddItem(new MenuItem("ravenousF", "Use Ravenous Hydra in Farm").SetValue(true));

                var lasthit = config.AddSubMenu(new Menu("Last Hit Settings", "LastHit"));
                lasthit.AddItem(new MenuItem("useQlh", "Use Q to last hit minions").SetValue(true));
                lasthit.AddItem(new MenuItem("useQlhHP", "Minimum HP% to use Q to lasthit").SetValue(new Slider(40, 1)));
                lasthit.AddItem(new MenuItem("qRange", "Only use Q if far from minions").SetValue(true));

                var clear = config.AddSubMenu(new Menu("Clear Settings", "Clear"));
                clear.AddItem(new MenuItem("spacerLC", "-- Lane Clear --"));
                clear.AddItem(new MenuItem("useQlc", "Use Q to last hit in laneclear").SetValue(true));
                clear.AddItem(new MenuItem("useQlcHP", "Minimum HP% to use Q to laneclear").SetValue(new Slider(50, 1)));
                clear.AddItem(new MenuItem("useWlc", "Use W in laneclear").SetValue(true));
                clear.AddItem(new MenuItem("useWlcHP", "Minimum HP% to use W to laneclear").SetValue(new Slider(60, 1)));
                clear.AddItem(new MenuItem("useWlcMinions", "Minimum minions to W in laneclear").SetValue(new Slider(3, 1, 10)));
                clear.AddItem(new MenuItem("spacerJC", "-- Jungle Clear --"));
                clear.AddItem(new MenuItem("useQj", "Use Q to jungle").SetValue(true));
                clear.AddItem(new MenuItem("useQjHP", "Minimum HP% to use Q in jungle").SetValue(new Slider(20, 1)));
                clear.AddItem(new MenuItem("useWj", "Use W to jungle").SetValue(true));
                clear.AddItem(new MenuItem("useWjHP", "Minimum HP% to use W to jungle").SetValue(new Slider(20, 1)));
                clear.AddItem(new MenuItem("useEj", "Use E to jungle").SetValue(true));

                var flee = config.AddSubMenu(new Menu("Flee Settings", "Flee"));
                flee.AddItem(new MenuItem("qFlee", "Q while fleeing").SetValue(true));
                flee.AddItem(new MenuItem("rFlee", "R to boost speed while fleeing").SetValue(false));

                var drawingMenu = config.AddSubMenu(new Menu("Drawings", "Drawings"));
                drawingMenu.AddItem(new MenuItem("disableDraw", "Disable all drawings").SetValue(true));
                drawingMenu.AddItem(new MenuItem("drawQ", "Q range").SetValue(new Circle(false, Color.DarkOrange, q.Range)));
                drawingMenu.AddItem(new MenuItem("drawW", "W range").SetValue(new Circle(false, Color.DarkOrange, w.Range)));
                drawingMenu.AddItem(new MenuItem("width", "Drawings width").SetValue(new Slider(2, 1, 5)));
                drawingMenu.AddItem(new MenuItem("drawAutoQ", "Draw AutoQ status").SetValue(false));

                config.AddItem(new MenuItem("spacer", ""));
                config.AddItem(new MenuItem("version", "Version: 6.12.0.0"));
                config.AddItem(new MenuItem("author", "Author: Hestia"));

                config.AddToMainMenu();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load the menu - {0}", exception);
            }
        }
    }
}
