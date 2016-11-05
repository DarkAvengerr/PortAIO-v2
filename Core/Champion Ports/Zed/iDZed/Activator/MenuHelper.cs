using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iDZed.Activator
{
    static class MenuHelper
    {
        public static bool IsMenuEnabled(string item)
        {
            return Zed.Menu.Item(item).GetValue<bool>();
        }

        public static int GetSliderValue(string item)
        {
            return Zed.Menu.Item(item) != null ? Zed.Menu.Item(item).GetValue<Slider>().Value : -1;
        }

        public static bool GetKeybindValue(string item)
        {
            return Zed.Menu.Item(item).GetValue<KeyBind>().Active;
        }
    }
}
