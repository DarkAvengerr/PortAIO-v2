using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hCamille.Extensions
{
    public static class Menus
    {
        public static Menu Config { get; set; }
        public static Orbwalking.Orbwalker Orbwalker { get; set; }

        public static void Initializer()
        {
            Config = new Menu("hCamille", "hCamille", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var combomenu = new Menu("Combo Settings", "Combo Settings");
                {
                    combomenu.AddItem(new MenuItem("q.combo", "Use [Q] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("q.mode", "[Q] Type").SetValue(new StringList(new[] { "After Attack", "In AA Range" })));
                    combomenu.AddItem(new MenuItem("w.combo", "Use [W] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("e.combo", "Use [E] ").SetValue(true));
                    Config.AddSubMenu(combomenu);
                }

                var ultimatemenu = new Menu("Ultimate Settings", "Ultimate Settings");
                {
                    ultimatemenu.AddItem(new MenuItem("r.combo", "Use [R] ").SetValue(true));
                    ultimatemenu.AddItem(new MenuItem("enemy.health.percent", "[R] Enemy Health Percentage").SetValue(new Slider(30, 1, 99)));

                    var whitelist = new Menu("Ultimate Whitelist", "Ultimate Whitelist");
                    {
                        foreach (var enemy in HeroManager.Enemies)
                        {
                            whitelist.AddItem(new MenuItem("r."+enemy.ChampionName, "Use [R]:  "+enemy.ChampionName).SetValue(Utilities.HighChamps.Contains(enemy.ChampionName)));
                        }
                        ultimatemenu.AddSubMenu(whitelist);
                    }

                    Config.AddSubMenu(ultimatemenu);
                }

                var harassmenu = new Menu("Harass Settings", "Harass Settings");
                {
                    harassmenu.AddItem(new MenuItem("q.harass", "Use [Q] ").SetValue(true));
                    harassmenu.AddItem(new MenuItem("w.harass", "Use [W] ").SetValue(true));
                    Config.AddSubMenu(harassmenu);
                }

                var clearmenu = new Menu("Wave Settings", "Wave Settings");
                {
                    clearmenu.AddItem(new MenuItem("q.clear", "Use [Q]").SetValue(true));
                    clearmenu.AddItem(new MenuItem("w.clear", "Use [W]").SetValue(true));
                    clearmenu.AddItem(new MenuItem("min.count", "[W] Min. Minion Count").SetValue(new Slider(3, 1, 5)));
                    Config.AddSubMenu(clearmenu);
                }

                var junglemenu = new Menu("Jungle Clear Settings", "Jungle Clear Settings");
                {
                    junglemenu.AddItem(new MenuItem("q.jungle", "Use [Q]").SetValue(true));
                    junglemenu.AddItem(new MenuItem("w.jungle", "Use [W]").SetValue(true));
                    Config.AddSubMenu(junglemenu);
                }
                
                Config.AddToMainMenu();
            }
        }
    }
}
