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
 namespace BadaoKingdom.BadaoChampion.BadaoJhin
{
    public static class BadaoJhin
    {
        //Jhin_Base_E_passive_mark.troy
        //jhinpassiveattackbuff, JhinPassiveReload
        //qrange = 600 ,  Qbound 500
        //W rang = 2500, W rad = 100
        //E range = 750, Ebound = 260
        // R range = 3500
        public static void BadaoActivate()
        {
            BadaoJhinAuto.BadaoActiavate();
            BadaoJhinCombo.BadaoActivate();
            BadaoJhinConfig.BadaoActivate();
            BadaoJhinHarass.BadaoActivate();
            BadaoJhinJungleClear.BadaoActivate();
            BadaoJhinLaneClear.BadaoActivate();
            BadaoJhinPassive.BadaoActiavte();
            BadaoJhinDrawing.BadaoActivate();
        }
    }
}
