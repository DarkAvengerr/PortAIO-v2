using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfWind
{
    static class Utils
    {
        public static void ShowNotification(string message, System.Drawing.Color color, int duration = -1, bool dispose = true)
        {
            Notifications.AddNotification(new Notification(message, duration, dispose).SetTextColor(color));
        }
    }
}
