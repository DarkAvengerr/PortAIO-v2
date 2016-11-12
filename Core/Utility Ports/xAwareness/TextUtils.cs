using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace xAwareness
{
    class TextUtils
    {
        static TextUtils()
        {

        }

        public static void DrawText(float x, float y, Color c, string text)
        {
            if (text != null)
            {
                Drawing.DrawText(x, y, c, text);
            }
        }

        public static System.Drawing.Size GetTextEntent(string text)
        {
            if (text != null)
            {
                return Drawing.GetTextEntent((text), 15);
            }
            else
            {
                return Drawing.GetTextEntent(("A"), 15);
            }
        }

        public static string FormatTime(double time)
        {
            if (time > 0)
            {
                return Utils.FormatTime(time);
            }
            else
            {
                return "00:00";
            }
        }
    }
}
