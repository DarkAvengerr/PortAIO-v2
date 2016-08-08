using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Feedlesticks.Core
{
    class Spells
    {
        /// <summary>
        /// last w game time
        /// </summary>
        public static float LastW;
        /// <summary>
        /// Spells
        /// </summary>
        public static Spell Q, W, E, R;
        /// <summary>
        /// Ignite Slot
        /// </summary>
        public static readonly SpellSlot Ignite = ObjectManager.Player.LSGetSpellSlot("summonerdot");
        /// <summary>
        /// Fiddle
        /// </summary>
        public static readonly AIHeroClient FiddleStick = ObjectManager.Player;

        /// <summary>
        /// Ignite Ready Check
        /// </summary>
        public static bool Igniteable
        {
            get
            {
                return Ignite.LSIsReady() && Ignite != SpellSlot.Unknown &&
                    FiddleStick.Spellbook.CanUseSpell(Ignite) == SpellState.Ready;
            }
        }
        /// <summary>
        /// Combo Ready Check
        /// </summary>
        public static bool ComboIsReady
        {
            get
            {
                return Q.LSIsReady() && W.LSIsReady() && E.LSIsReady() && R.LSIsReady();
            }
        }

        public static bool Wable 
        {
            get { return Game.Time - LastW > 2; }
        }
        
        /// <summary>
        /// Spell data
        /// </summary>
        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 575);
            W = new Spell(SpellSlot.W, 575);
            E = new Spell(SpellSlot.E, 750);
            R = new Spell(SpellSlot.R, 800);
        }
    }
}
