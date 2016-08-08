using System;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;

namespace SCommon.MenuUtils
{
    public static class MenuExtension
    {
        public static MenuItem AddItem(this Menu root, string str)
        {
            return root.AddItem(new MenuItem(str, MenuLanguage.GetDisplayName(str)));
        }
    }
}
