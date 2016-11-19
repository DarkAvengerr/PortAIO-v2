using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Viktor.Extensions
{
    internal class VSpells
    {
        public static readonly int ERange = 550;
        public static readonly int EMaxRange = 1225;
        public static readonly int LenghtE = 700;
        public static readonly int SpeedE = 1050;

        public enum Spells
        {
            Q, W, E, R
        };

        public static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {
                Spells.Q,
                new Spell(SpellSlot.Q, 665)
            },
            {
                Spells.W,
                new Spell(SpellSlot.W, 700)
            },
            {
                Spells.E,
                new Spell(SpellSlot.E, 525)
            },
            {
                Spells.R,
                new Spell(SpellSlot.R, 700)
            },
        };
    }
}
