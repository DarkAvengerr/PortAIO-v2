using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SCommon.MenuUtils
{
    public enum Language
    {
        English = 0,
        Turkish = 1,
        Korean = 2,
    }
    
    public static class MenuLanguage
    {
        static MenuLanguage()
        {

        }

        public static string GetDisplayName(string str)
        {
            return str;
        }
    }
}
