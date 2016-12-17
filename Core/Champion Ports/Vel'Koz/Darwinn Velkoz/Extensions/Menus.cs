using System.Drawing;
using System.Linq;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Darwinn_s_velkoz.Extensions
{
    internal static class Menus
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Initialize()
        {

            Config = new Menu("Darwinn's Vel'koz", "Darwinn's Vel'koz", true).SetFontStyle(FontStyle.Bold, SharpDX.Color.Gold);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var combomenu = new Menu("Combo Settings", "Combo Settings");
                {
                    combomenu.AddItem(new MenuItem("q.combo", "Use [Q] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("w.combo", "Use [W] ").SetValue(true));
                    combomenu.AddItem(new MenuItem("e.combo", "Use [E] ").SetValue(true));
                    Config.AddSubMenu(combomenu);
                }

                var harassmenu = new Menu("Harass Settings", "Harass Settings");
                {
                    harassmenu.AddItem(new MenuItem("q.harass", "Use [Q] ").SetValue(true));
                    harassmenu.AddItem(new MenuItem("w.harass", "Use [W] ").SetValue(true));
                    harassmenu.AddItem(new MenuItem("e.harass", "Use [E] ").SetValue(true));
                    harassmenu.AddItem(new MenuItem("harassmana", "Mana Manager").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(harassmenu);
                }

                var clearmenu = new Menu("Wave Settings", "Wave Settings");
                {
                    clearmenu.AddItem(new MenuItem("q.clear", "Use [Q]").SetValue(true));
                    clearmenu.AddItem(new MenuItem("w.clear", "Use [W]").SetValue(true));
                    clearmenu.AddItem(new MenuItem("e.clear", "Use [E]").SetValue(true));
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

            /*    var ultimenu = new Menu("Ultimate Settings", "Ultimate Settings").SetFontStyle(FontStyle.Bold, SharpDX.Color.GreenYellow);
                {
                    var whitelist = new Menu("Whitelist", "Whitelist").SetFontStyle(FontStyle.Bold, SharpDX.Color.Gold);
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValid))
                        {
                            whitelist.AddItem(new MenuItem("r." + enemy.ChampionName, "Use [R] : " + enemy.ChampionName).SetValue(Utilities.HighChamps.Contains(enemy.ChampionName)));
                        }

                        ultimenu.AddSubMenu(whitelist);
                    }
                    ultimenu.AddItem(new MenuItem("r.combo", "Use [R]").SetValue(true));
                    Config.AddSubMenu(ultimenu);
                } */

                Config.AddItem(new MenuItem("keysinfo", "                           General Settings").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Gold));
                Config.AddItem(new MenuItem("block.dashes", "BLOCK DASHES").SetValue(true));
                Config.AddItem(new MenuItem("hitchance", "Hit Chance ?").SetValue(new StringList(Utilities.HitchanceNameArray, 2)));
                Config.AddItem(new MenuItem("prediction", "Prediction ?").SetValue(new StringList(new[] { "Common", "Sebby", "sPrediction" }, 1)))
                    .ValueChanged += (s, ar) =>
                    {
                        Config.Item("pred.info").Show(ar.GetNewValue<StringList>().SelectedIndex == 2);
                    };
                Config.AddItem(new MenuItem("pred.info", "                 PRESS F5 FOR LOAD SPREDICTION").SetFontStyle(System.Drawing.FontStyle.Bold)).Show(Config.Item("prediction").GetValue<StringList>().SelectedIndex == 0);
                if (Config.Item("prediction").GetValue<StringList>().SelectedIndex == 2)
                {
                    SPrediction.Prediction.Initialize(Config, ":: sPrediction Settings");
                }

                Config.AddToMainMenu();
            }
        }
    }
}