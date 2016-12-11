using System;
﻿using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PRADA_Vayne_Old.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void Init()
        {
            MyUtils.Cache.Load();
            LoadMenu();
            LoadSpells();
            LoadLogic();
            ShowNotifications();
        }
    }
}
