using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoRengar
{
    public static class Variables
    {
        public static Spell Q, W, E, R;
        public static Spell Q2, W2, E2, R2;
        public static SpellSlot Flash, Ignite, Heal, Smite;
        public static Orbwalking.Orbwalker Orbwalker;

        // menu
        public static MenuItem ComboSmite;
        public static MenuItem ComboYoumuu;
        public static MenuItem ComboMode;
        public static MenuItem ComboSwitchKey;
        public static MenuItem ComboResetAA;

        public static MenuItem AssassinateKey;
        public static MenuItem AssasinateInstruction;
        public static MenuItem AssasinateTarget;
        public static MenuItem AssasinateSwitchKey;

        public static MenuItem HarassW;
        public static MenuItem HarassE;

        public static MenuItem LaneQ;
        public static MenuItem LaneW;
        public static MenuItem LaneE;
        public static MenuItem LaneTiamat;
        public static MenuItem LaneSave;

        public static MenuItem JungQ;
        public static MenuItem JungW;
        public static MenuItem JungE;
        public static MenuItem JungTiamat;
        public static MenuItem JungSave;

        public static MenuItem AutoWHeal;
        public static MenuItem AutoEInterrupt;
        public static MenuItem AutoWKS;
        public static MenuItem AutoESK;
        public static MenuItem AutoSmiteKS;
        public static MenuItem AutoSmiteSteal;

        public static MenuItem DrawMode;
        public static MenuItem DrawSelectedTarget;
        public static MenuItem DrawAssasinate;

        public static MenuItem MagnetEnable;
        public static MenuItem MagnetRange;

        public static bool IsDoingMagnet = false;

        public static MenuItem UltSelected;
        public static MenuItem BushSelected;

    }
}
