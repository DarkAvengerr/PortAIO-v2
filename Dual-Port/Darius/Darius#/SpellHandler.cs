using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DariusSharp
{
    internal class SpellHandler
    {
        //Spells
        public static Spell Q { get; private set; }
        public static Spell W { get; private set; }
        public static Spell E { get; private set; }
        public static Spell R { get; private set; }

        //Initialize SpellHandler
        public static void Initialize()
        {
            //Initialize our Spells
            Q = new Spell(SpellSlot.Q, 425);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 540);
            R = new Spell(SpellSlot.R, 460);
        }

        /* 4.21 disabled
        public static bool PacketCasting()
        {
            return ConfigHandler.BoolLinks["packetCast"].Value;
        }*/

        public static double AdjustDamage()
        {
            return ConfigHandler.SliderLinks["adjustDmg"].Value.Value;
        }

        public static bool IsEnabled(string name)
        {
            return ConfigHandler.BoolLinks[name].Value;
        }
    }
}
