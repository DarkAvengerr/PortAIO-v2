using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using EloBuddy;

namespace Feedlesticks.Core
{
    class Menus
    {
        /// <summary>
        /// Menu
        /// </summary>
        public static Menu Config;
        /// <summary>
        /// Orbwalker
        /// </summary>
        public static Orbwalking.Orbwalker Orbwalker;

        /// <summary>
        /// General Menu
        /// </summary>
        public static void Init()
        {
            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("w.combo", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
                Config.AddSubMenu(comboMenu);
            }
            var qMenu = new Menu("Q Settings", "Q Settings");
            {
                var qWhite = new Menu("Q Whitelist", "Q Whitelist");
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                    {
                        qWhite.AddItem(new MenuItem("q.enemy." + enemy.ChampionName, string.Format("Q: {0}", enemy.CharData.BaseSkinName)).SetValue(Piorty.HighChamps.Contains(enemy.CharData.BaseSkinName)));

                    }
                    qMenu.AddSubMenu(qWhite);
                }
                qMenu.AddItem(new MenuItem("auto.q.immobile", "Auto (Q) If Enemy Immobile").SetValue(true));
                qMenu.AddItem(new MenuItem("auto.q.channeling", "Auto (Q) If Enemy Casting Channeling Spell").SetValue(true));
                Config.AddSubMenu(qMenu);
            }

            var wMenu = new Menu("W Settings", "W Settings");
            {
                var wHite = new Menu("W Whitelist", "W Whitelist");
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                    {
                        wHite.AddItem(new MenuItem("w.enemy." + enemy.ChampionName, string.Format("W: {0}", enemy.CharData.BaseSkinName)).SetValue(Piorty.HighChamps.Contains(enemy.CharData.BaseSkinName)));

                    }
                    wMenu.AddSubMenu(wHite);
                }
                Config.AddSubMenu(wMenu);
            }

            var eMenu = new Menu("E Settings", "E Settings");
            {
                var eWhite = new Menu("E Whitelist", "E Whitelist");
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                    {
                        eWhite.AddItem(new MenuItem("e.enemy." + enemy.ChampionName, string.Format("E: {0}", enemy.CharData.BaseSkinName)).SetValue(Piorty.HighChamps.Contains(enemy.CharData.BaseSkinName)));

                    }
                    eMenu.AddSubMenu(eWhite);

                    eMenu.AddItem(new MenuItem("e.enemy.count", "(E) Min. Enemy").SetValue(new Slider(2, 1, 5)));
                    eMenu.AddItem(new MenuItem("auto.e.immobile", "Auto (E) If Enemy Immobile").SetValue(true));
                    eMenu.AddItem(new MenuItem("auto.e.channeling", "Auto (E) If Enemy Casting Channeling Spell").SetValue(true));
                    Config.AddSubMenu(eMenu);
                }

                var harassMenu = new Menu("Harass Settings", "Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("e.harass", "Use E").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(harassMenu);
                }

                var clearMenu = new Menu("Clear Settings", "Clear Settings");
                {
                    clearMenu.AddItem(new MenuItem("w.clear", "Use W").SetValue(true));
                    clearMenu.AddItem(new MenuItem("e.clear", "Use E").SetValue(true));
                    clearMenu.AddItem(new MenuItem("e.minion.hit.count", "(E) Min. Minion").SetValue(new Slider(3, 1, 5)));
                    clearMenu.AddItem(new MenuItem("clear.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(clearMenu);
                }

                var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
                {
                    jungleMenu.AddItem(new MenuItem("q.jungle", "Use Q").SetValue(true));
                    jungleMenu.AddItem(new MenuItem("w.jungle", "Use W").SetValue(true));
                    jungleMenu.AddItem(new MenuItem("e.jungle", "Use E").SetValue(true));
                    jungleMenu.AddItem(new MenuItem("jungle.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(jungleMenu);
                }
                var drawMenu = new Menu("Draw Settings", "Draw Settings");
                {
                    drawMenu.AddItem(new MenuItem("q.draw", "Q Range").SetValue(new Circle(true, Color.White)));
                    drawMenu.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, Color.DarkSeaGreen)));
                    drawMenu.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, Color.Gold)));
                    drawMenu.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, Color.DodgerBlue)));
                    Config.AddSubMenu(drawMenu);
                }
            }
            Config.AddToMainMenu();
        }
        /// <summary>
        /// Orbwalker Init
        /// </summary>
        public static void OrbwalkerInit()
        {
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));
        }
    }
}
