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
    public static class BadaoActivatorHelper
    {
        public static bool UseYoumuu()
        {
            return ItemData.Youmuus_Ghostblade.GetItem().IsReady() &&
                BadaoActivatorVariables.Youmuu.GetValue<bool>();
        }
        public static bool UseBotrk()
        {
            return ItemData.Blade_of_the_Ruined_King.GetItem().IsReady() &&
                BadaoActivatorVariables.Botrk.GetValue<bool>();
        }
    }
}
