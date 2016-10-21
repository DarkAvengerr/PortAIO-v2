using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoElise
{
    public static class BadaoElise
    {
        public static void BadaoActivate()
        {
            BadaoEliseConfig.BadaoActivate();
            BadaoEliseSpellsManager.BadaoActivate();
            BadaoEliseCombo.BadaoActivate();
            BadaoEliseLaneClear.BadaoActivate();
            BadaoEliseJungleClear.BadaoActivate();
        }

    }
}
