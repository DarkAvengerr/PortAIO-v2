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
    public static class BadaoMissFortuneVariables
    {
        // menu
        public static MenuItem ComboQ1;
        public static MenuItem ComboQ2;
        public static MenuItem ComboW;
        public static MenuItem ComboE;
        public static MenuItem ComboR;
        public static MenuItem ComboRWise;
        public static MenuItem ComboRifwillhit;
        public static MenuItem ComboRifhit;

        public static MenuItem HarassQ1;
        public static MenuItem HarassQ2;
        public static MenuItem HarassE;
        public static MenuItem HarassMana;

        public static MenuItem LaneClearQ;
        public static MenuItem LaneClearW;
        public static MenuItem LaneClearE;
        public static MenuItem LaneClearMana;

        public static MenuItem JungleClearQ;
        public static MenuItem JungleClearW;
        public static MenuItem JungleClearE;
        public static MenuItem JungleClearMana;

        public static MenuItem AutoMana;
        // #
        public static Obj_AI_Base TapTarget = null;
        public static AIHeroClient TargetRChanneling = null;
        public static Vector2 CenterPolar = new Vector2();
        public static Vector2 CenterEnd = new Vector2();
        public static int Rcount;
    }
}
