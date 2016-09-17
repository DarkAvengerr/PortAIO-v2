using System;
using System.Linq;
using LeagueSharp;
using Color = System.Drawing.Color;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HestiaNautilus
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

                var combo = config.AddSubMenu(new Menu("Combo Settings", "Combo"));

                var comboQ = combo.AddSubMenu(new Menu("Q Settings", "Q"));
                comboQ.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
                
                var comboW = combo.AddSubMenu(new Menu("W Settings", "W"));
                comboW.AddItem(new MenuItem("useW", "Use W").SetValue(true));
                comboW.AddItem(new MenuItem("WHealthCombo", "Use W only if HP% < (0 disabled)").SetValue(new Slider(85)));

                var comboE = combo.AddSubMenu(new Menu("E Settings", "E"));
                comboE.AddItem(new MenuItem("useE", "Use E").SetValue(true));

                var comboR = combo.AddSubMenu(new Menu("R Settings", "R"));
                comboR.AddItem(new MenuItem("useR", "Use R").SetValue(false));

                var rBlacklist = comboR.AddSubMenu(new Menu("Do not ult: ", "dontUlt"));
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                {
                    rBlacklist.AddItem(
                        new MenuItem(enemy.CharData.BaseSkinName, enemy.CharData.BaseSkinName).SetValue(false));
                }

                var harass = config.AddSubMenu(new Menu("Harass Settings", "Harass"));

                var harassE = harass.AddSubMenu(new Menu("E Settings", "E"));
                harassE.AddItem(new MenuItem("useEh", "Use E to harass").SetValue(true));
                harassE.AddItem(new MenuItem("useEhMana", "Minimum mana to use E").SetValue(new Slider(60, 1)));

                var harassW = harass.AddSubMenu(new Menu("W Settings", "W"));
                harassW.AddItem(new MenuItem("useWh", "Use W to harass").SetValue(true));
                harassW.AddItem(new MenuItem("useWhMana", "Minimum mana to use W").SetValue(new Slider(60, 1)));

                var farm = config.AddSubMenu(new Menu("Farm Settings", "Farm"));
                farm.AddItem(new MenuItem("useElc", "Use E to lane clear").SetValue(true));
                farm.AddItem(new MenuItem("useElcMinions", "Minimum minions to use E").SetValue(new Slider(4, 1, 15)));
                farm.AddItem(new MenuItem("useElcMana", "Minimum mana to use E").SetValue(new Slider(60, 1)));
                farm.AddItem(new MenuItem("useWlc", "Use W to lane clear").SetValue(true));
                farm.AddItem(new MenuItem("useWlcMinions", "Minimum minions to use W").SetValue(new Slider(4, 1, 15)));
                farm.AddItem(new MenuItem("useWlcMana", "Minimum mana to use W").SetValue(new Slider(60, 1)));

                var jungle = config.AddSubMenu(new Menu("Jungle Settings", "Jungle"));
                jungle.AddItem(new MenuItem("useEj", "Use E to jungle clear").SetValue(true));
                jungle.AddItem(new MenuItem("useEjMana", "Minimum mana to use E").SetValue(new Slider(50, 1)));
                jungle.AddItem(new MenuItem("useWj", "Use W to jungle clear").SetValue(true));
                jungle.AddItem(new MenuItem("useWjMana", "Minimum mana to use W").SetValue(new Slider(20, 1)));

                var misc = config.AddSubMenu(new Menu("Misc Settings", "Misc"));

                var miscQ = misc.AddSubMenu(new Menu("Q Settings", "Q Misc"));
                miscQ.AddItem(
                    new MenuItem("qHitchance", "Q Hitchance").SetValue(
                        new StringList(
                            new[]
                            {
                            HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(),
                            HitChance.VeryHigh.ToString()
                            }, 3)));

                var miscInterr = misc.AddSubMenu(new Menu("Interrupter Settings", "Interrupter"));
                miscInterr.AddItem(new MenuItem("useQinterrupt", "Use Q to interrupt spells").SetValue(true));
                miscInterr.AddItem(new MenuItem("useRinterrupt", "Use R to interrupt if Q not possible").SetValue(false));

                var killsteal = config.AddSubMenu(new Menu("KillSteal Settings", "KillSteal"));
                killsteal.AddItem(new MenuItem("killsteal", "Activate KillSteal").SetValue(true));
                killsteal.AddItem(new MenuItem("useQks", "Use Q to KillSteal").SetValue(true));
                killsteal.AddItem(new MenuItem("useEks", "Use E to KillSteal").SetValue(true));
                killsteal.AddItem(new MenuItem("useRks", "Use R to KillSteal").SetValue(false));
                killsteal.AddItem(new MenuItem("useIks", "Use Ignite to KillSteal").SetValue(true));

                var drawingMenu = config.AddSubMenu(new Menu("Drawings", "Drawings"));
                drawingMenu.AddItem(new MenuItem("disableDraw", "Disable all drawings").SetValue(false));
                drawingMenu.AddItem(new MenuItem("drawQ", "Q range").SetValue(new Circle(true, Color.DarkOrange, q.Range)));
                drawingMenu.AddItem(new MenuItem("drawE", "E range").SetValue(new Circle(false, Color.DarkOrange, e.Range)));
                drawingMenu.AddItem(new MenuItem("drawR", "R range").SetValue(new Circle(true, Color.DarkOrange, r.Range)));
                drawingMenu.AddItem(new MenuItem("width", "Drawings width").SetValue(new Slider(2, 1, 5)));

                config.AddItem(new MenuItem("spacer", ""));
                config.AddItem(new MenuItem("version", "Version: 6.10.0.0"));
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
