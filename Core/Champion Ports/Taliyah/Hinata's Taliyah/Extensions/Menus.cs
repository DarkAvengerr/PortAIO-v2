using System.Drawing;
using System.Linq;
using System.Security.Policy;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Hinata_s_Taliyah.Extensions
{
    internal static class Menus
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Initialize()
        {
            Config = new Menu("Hinata's Taliyah", "Hinatas's Taliyah", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var combomenu = new Menu("Combo Settings","Combo Settings");
                {
                    combomenu.AddItem(new MenuItem("q.combo", "Use [Q] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("w.combo", "Use [W] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("w.mode",  "[W] Mode").SetValue(new StringList(new[] { "Pull", "Push" })));
                    combomenu.AddItem(new MenuItem("e.combo", "Use [E] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("e.combo.type2", "Use [E] only if enemy is immobile").SetValue(true));
                    Config.AddSubMenu(combomenu);
                }

                var harassmenu = new Menu("Harass Settings", "Harass Settings");
                {
                    harassmenu.AddItem(new MenuItem("q.harass", "Use [Q] ").SetValue(true));
                    harassmenu.AddItem(new MenuItem("harassmana", "Mana Manager").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(harassmenu);
                }

                var clearmenu = new Menu("Wave Settings", "Wave Settings");
                {
                    clearmenu.AddItem(new MenuItem("q.clear", "Use [Q]").SetValue(true));
                    clearmenu.AddItem(new MenuItem("q.min.count", "[Q] Min. Minion Count").SetValue(new Slider(3, 1, 5)));

                    clearmenu.AddItem(new MenuItem("e.clear", "Use [E]").SetValue(true));
                    clearmenu.AddItem(new MenuItem("e.min.count", "[E] Min. Minion Count").SetValue(new Slider(3, 1, 5)));

                    clearmenu.AddItem(new MenuItem("clearmana", "Mana Manager").SetValue(new Slider(50, 1, 99)));

                    Config.AddSubMenu(clearmenu);
                }

                var junglemenu = new Menu("Jungle Clear Settings", "Jungle Clear Settings");
                {
                    junglemenu.AddItem(new MenuItem("q.jungle", "Use [Q]").SetValue(true));
                    junglemenu.AddItem(new MenuItem("w.jungle", "Use [W]").SetValue(true));
                    junglemenu.AddItem(new MenuItem("e.jungle", "Use [E]").SetValue(true));
                    junglemenu.AddItem(new MenuItem("junglemana", "Mana Manager").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(junglemenu);
                }

                Config.AddItem(new MenuItem("keysinfo", "                           General Settings").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Gold));
                
                Config.AddItem(new MenuItem("hitchance", "Hit Chance ?").SetValue(new StringList(Utilities.HitchanceNameArray, 2)));
                Config.AddItem(new MenuItem("e.gapcloser", "[E] TO GAPCLOSE")).SetValue(true);
                Config.AddItem(new MenuItem("w.interrupt", "[W] TO INTERRUPT")).SetValue(true);

                
                Config.AddToMainMenu();
            }
        }
    }
}
