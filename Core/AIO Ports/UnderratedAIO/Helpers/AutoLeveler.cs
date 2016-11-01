using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace UnderratedAIO.Helpers
{
    internal class AutoLeveler
    {
        private static Menu configMenu;
        public static AutoLevel autoLevel;
        public static bool enabled = true;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public static Menu AddToMenu(Menu config)
        {
            Menu menulvl = new Menu("Autoleveler ", "Autolevelersettings");
            menulvl.AddItem(new MenuItem(player.ChampionName + "order", "Skill order", true))
                .SetValue(new StringList(new[] { "Q->W->E", "Q->E->W", "W->Q->E", "W->E->Q", "E->Q->W", "E->W->Q" }, 0));
            menulvl.AddItem(new MenuItem(player.ChampionName + "Enabled", "Enabled")).SetValue(false);
            menulvl.AddItem(new MenuItem("Test", "Test Only"));
            config.AddSubMenu(menulvl);
            configMenu = config;
            autoLevel =
                new AutoLevel(
                    GetTree(configMenu.Item(player.ChampionName + "order", true).GetValue<StringList>().SelectedIndex));
            Game.OnUpdate += Game_OnUpdate;
            return configMenu;
        }


        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private static int[] GetTree(int p)
        {
            switch (p)
            {
                case 0: //lvl  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18  
                    return new int[] { 0, 1, 2, 0, 0, 3, 0, 1, 0, 1, 3, 1, 1, 2, 2, 3, 2, 2 };
                    break;
                case 1:
                    return new int[] { 0, 2, 1, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 };
                    break;
                case 2:
                    return new int[] { 1, 0, 2, 1, 1, 3, 1, 0, 1, 0, 3, 0, 0, 2, 2, 3, 2, 2 };
                    break;
                case 3:
                    return new int[] { 1, 2, 0, 1, 1, 3, 1, 2, 1, 2, 3, 2, 2, 0, 0, 3, 0, 0 };
                    break;
                case 4:
                    return new int[] { 2, 0, 1, 2, 2, 3, 2, 0, 2, 0, 3, 0, 0, 1, 1, 3, 1, 1 };
                    break;
                case 5:
                    return new int[] { 2, 1, 0, 2, 2, 3, 2, 1, 2, 1, 3, 1, 1, 0, 0, 3, 0, 0 };
                    break;
            }
            return new int[] { 0, 1, 2, 0, 0, 3, 0, 1, 0, 1, 3, 1, 1, 2, 2, 3, 2, 2 };
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (enabled && configMenu.Item(player.ChampionName + "Enabled").GetValue<bool>())
            {
                AutoLevel.UpdateSequence(
                    GetTree(configMenu.Item(player.ChampionName + "order", true).GetValue<StringList>().SelectedIndex));
                AutoLevel.Enable();
            }
            else
            {
                AutoLevel.Disable();
            }
        }
    }
}