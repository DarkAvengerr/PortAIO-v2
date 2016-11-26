#region Use
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{

    internal static class Spells
    {
        #region Spells

        internal static Spell _q { get; private set; }
        internal static Spell _w { get; private set; }
        internal static Spell _e { get; private set; }
        internal static Spell _r { get; private set; }

        #endregion

        internal static void LoadSpells()
        {
            _q = new Spell(SpellSlot.Q, 1450f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.VeryHigh };
            _w = new Spell(SpellSlot.W, 1200f, TargetSelector.DamageType.Magical);
            _e = new Spell(SpellSlot.E);
            _r = new Spell(SpellSlot.R, 5500f);

            _q.SetSkillshot(0.25f, 40f, 1000f, false, SkillshotType.SkillshotLine);
        }
    }
}
