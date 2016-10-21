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
 namespace BadaoKingdom.BadaoChampion.BadaoMissFortune
{
    public static class BadaoMissFortune
    {
        public static void BadaoActivate()
        {
            BadaoMissFortuneConfig.BadaoActivate();
            BadaoMissFortuneTapTarget.BadaoActivate();
            BadaoMissFortuneCombo.BadaoActivate();
            BadaoMissFortuneDrawing.BadaoActivate();
            BadaoMissFortuneHarass.BadaoActivate();
            BadaoMissFortuneLaneClear.BadaoActivate();
            BadaoMissFortuneJungleClear.BadaoActivate();
            BadaoMissFortuneAuto.BadaoActivate();
        }
    }
}
