using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoUtility.BadaoActivator
{
    public static class BadaoActivatorConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {
            // main menu
            config = new Menu("BadaoKingdomActivator", "BadaoKingdomActivator", true);
            config.SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.YellowGreen);

            BadaoActivatorVariables.Youmuu = config.AddItem(new MenuItem("Youmuu","Youmuu")).SetValue(true);
            BadaoActivatorVariables.Botrk = config.AddItem(new MenuItem("Botrk", "Botrk")).SetValue(true);

            // attach to mainmenu
            config.AddToMainMenu();
        }
    }
}
