using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using RandomUlt.Helpers;
using EloBuddy;

namespace RandomUlt
{
    internal class Program
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static LastPositions positions;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public static void Game_OnGameLoad()
        {
            config = new Menu("RandomUlt Beta", "RandomUlt Beta", true);
            Menu RandomUltM = new Menu("Options", "Options");
            positions = new LastPositions(RandomUltM);
            config.AddSubMenu(RandomUltM);
            config.AddItem(new MenuItem("RandomUlt ", "by Soresu"));
            config.AddToMainMenu();
            Notifications.AddNotification(new Notification("RandomUlt by Soresu", 3000, true).SetTextColor(Color.Peru));
        }
    }
}