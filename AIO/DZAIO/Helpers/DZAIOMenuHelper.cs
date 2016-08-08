using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Helpers
{
    internal static class DZAIOMenuHelper
    {
        internal static T GetItemValue<T>(this Menu menu, string item)
        {
            return menu.Item(item).GetValue<T>();
        }
    }
}
