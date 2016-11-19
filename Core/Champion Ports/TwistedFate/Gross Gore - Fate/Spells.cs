#region Use

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{
    #region Libs

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal static class Spells
    {
        #region Spells

        internal static Spell Q { get; private set; }
        internal static Spell W { get; private set; }
        internal static Spell E { get; private set; }
        internal static Spell R { get; private set; }

        #endregion

        internal static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 1450);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 5500);

            Q.SetSkillshot(0.25f, 40f, 1000f, false, SkillshotType.SkillshotLine);
        }
    }
}
