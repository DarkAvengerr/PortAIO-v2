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
 namespace BadaoKingdom.BadaoChampion.BadaoVeigar
{
    public static class BadaoVeigar
    {
        public static void BadaoActivate()
        {
            BadaoVeigarConfig.BadaoActivate();
            BadaoVeigarCombo.BadaoActivate();
            BadaoVeigarLaneClear.BadaoActivate();
        }
    }
}
