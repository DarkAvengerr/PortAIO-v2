using LeagueSharp.Common;
using System;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Interfaces.Drawing
{
    public interface Champion
    {
        //LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate DamageToEnemy { get; set; }

        void OnDrawEnemy(EventArgs args);

        void OnDrawSelf(EventArgs args);
    }
}