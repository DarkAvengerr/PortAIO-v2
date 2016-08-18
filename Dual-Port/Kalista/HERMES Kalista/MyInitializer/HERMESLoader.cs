using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy;
using LeagueSharp.Common;
namespace HERMES_Kalista.MyInitializer
{
    public static partial class HERMESLoader
    {
        public static void Init()
        {
            if (ObjectManager.Player.CharData.BaseSkinName == "Kalista")
            {
                MyUtils.Cache.Load();
                LoadMenu();
                LoadSpells();
                LoadLogic();
                ShowNotifications();
                Draw();
            }
        }
    }
}
