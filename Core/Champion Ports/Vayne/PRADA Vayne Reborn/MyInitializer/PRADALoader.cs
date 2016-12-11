using System;
using LeagueSharp;
using LeagueSharp.Common;
using PRADA_Vayne.MyLogic.Others;

using EloBuddy;
using LeagueSharp.Common;
namespace PRADA_Vayne.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void Init()
        {
            if (ObjectManager.Player.BaseSkinName == "Vayne")
            {
                MyUtils.Cache.Load();
                LoadMenu();
                LoadSpells();
                LoadLogic();
                ShowNotifications();
                SkinHack.Load();
            }
        }
    }
}
