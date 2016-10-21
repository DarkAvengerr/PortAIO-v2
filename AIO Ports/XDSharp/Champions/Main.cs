using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace XDSharp.Champions
{
    class Main
    {
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static SpellSlot TSSpell()
        {
            switch (Player.ChampionName)
            {/*
                case "Cassiopeia":
                    return SpellSlot.E;
                case "LeeSin":
                    return SpellSlot.Q;
                case "Blitzcrank":
                    return SpellSlot.Q;
                case "Ekko":
                    return SpellSlot.Q;*/
                case "Karthus":
                    return SpellSlot.Q;
            }
            return SpellSlot.Q;
        }
    }
}
