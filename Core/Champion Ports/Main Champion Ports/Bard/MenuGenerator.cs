using System.Reflection;
using LeagueSharp.Common;

namespace DZBard
{
    class MenuGenerator
    {
        internal static void OnLoad(Menu rootMenu)
        {
            var OrbwalkerMenu = new Menu("Orbwalker","dz191.bard.orbwalker");
            Bard.BardOrbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);
            rootMenu.AddSubMenu(OrbwalkerMenu);

            var TSMenu = new Menu("TargetSelector","dz191.bard.ts");

            TargetSelector.AddToMenu(TSMenu);

            rootMenu.AddSubMenu(TSMenu);

            var comboMenu = new Menu("Combo","dz191.bard.combo");
            {
                comboMenu.AddItem(new MenuItem("dz191.bard.combo.useq", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("dz191.bard.combo.usew", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("dz191.bard.combo.qks", "Use Q KS").SetValue(true));

                rootMenu.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("Harass", "dz191.bard.mixed");
            {
                var QMenu = new Menu("Q Targets (Harass Only)", "dz191.bard.mixed");
                {
                    foreach (var hero in HeroManager.Enemies)
                    {
                        QMenu.AddItem(
                            new MenuItem(string.Format("dz191.bard.qtarget.{0}", hero.ChampionName.ToLower()),
                                hero.ChampionName).SetValue(true));
                        
                    }
                }

                harassMenu.AddItem(new MenuItem("dz191.bard.mixed.useq", "Use Q").SetValue(true));
                harassMenu.AddSubMenu(QMenu);
                rootMenu.AddSubMenu(harassMenu);
            }

            var fleeMenu = new Menu("Flee", "dz191.bard.flee");
            {
                fleeMenu.AddItem(new MenuItem("dz191.bard.flee.q", "Q Flee").SetValue(true));
                fleeMenu.AddItem(new MenuItem("dz191.bard.flee.w", "W Flee").SetValue(true));
                fleeMenu.AddItem(new MenuItem("dz191.bard.flee.e", "E Flee").SetValue(true));

                fleeMenu.AddItem(new MenuItem("dz191.bard.flee.flee", "Flee").SetValue(new KeyBind('T', KeyBindType.Press)));
                rootMenu.AddSubMenu(fleeMenu);
            }

            var miscMenu = new Menu("Misc","dz191.bard.misc");
            {
                var DontWMenu = new Menu("W Settings", "dz191.bard.wtarget");
                {
                    foreach (var hero in HeroManager.Allies)
                    {
                        DontWMenu.AddItem(
                            new MenuItem(string.Format("dz191.bard.wtarget.{0}", hero.ChampionName.ToLower()),
                                hero.ChampionName).SetValue(true));
                        DontWMenu.AddItem(
                            new MenuItem("dz191.bard.wtarget.healthpercent", "Health % for W").SetValue(new Slider(25, 1)));
                    }
                    miscMenu.AddSubMenu(DontWMenu);
                }

                miscMenu.AddItem(new MenuItem("dz191.bard.misc.sep1", "                     Q - Cosmic Binding          "));
                miscMenu.AddItem(new MenuItem("dz191.bard.misc.distance", "Calculation distance").SetValue(new Slider(250, 100, 450)));
                miscMenu.AddItem(new MenuItem("dz191.bard.misc.accuracy", "Accuracy").SetValue(new Slider(20, 1, 50)));
                miscMenu.AddItem(new MenuItem("dz191.bard.misc.sep2", ""));
                miscMenu.AddItem(new MenuItem("dz191.bard.misc.attackMinions", "Don't attack Minions aka Support Mode").SetValue(true));
                miscMenu.AddItem(new MenuItem("dz191.bard.misc.attackMinionsRange", "Allies in range to not attack Minions").SetValue(new Slider(1200, 700, 2000)));
                rootMenu.AddSubMenu(miscMenu);
            }

            rootMenu.AddItem(new MenuItem("dz191.bard.info", "Bard - Dreamless Wanderer by Asuna v." + Assembly.GetExecutingAssembly().GetName().Version));

            rootMenu.AddToMainMenu();
        }
    }
}
