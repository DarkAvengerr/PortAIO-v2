using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using PRADA_Vayne.MyUtils;

using EloBuddy; 
using LeagueSharp.Common; 
namespace PRADA_Vayne.MyLogic.Others
{
    public static class SkinHack
    {
        public static void Load()
        {
            LeagueSharp.Common.Utility.DelayAction.Add(250, RefreshSkin);
            Game.OnUpdate += OnUpdate;
            Game.OnNotify += OnNotify;
        }

        private static void OnNotify(GameNotifyEventArgs args)
        {
        }

        public static void OnUpdate(EventArgs args)
        {
        }

        public static void RefreshSkin()
        {
            if (Program.SkinhackMenu.Item("shkenabled").GetValue<bool>())
            {
            }
        }
    }
}
