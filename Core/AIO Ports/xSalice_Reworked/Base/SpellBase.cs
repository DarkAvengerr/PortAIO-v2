using EloBuddy; 
using LeagueSharp.Common; 
 namespace xSaliceResurrected_Rework.Base
{
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Managers;

    public abstract class SpellBase
    {
        protected static List<Spell> SpellList => SpellManager.SpellList;

        protected static Spell P => SpellManager.P;

        protected static Spell Q => SpellManager.Q;

        protected static Spell Q2 => SpellManager.Q2;

        protected static Spell QExtend => SpellManager.QExtend;

        protected static Spell W => SpellManager.W;

        protected static Spell W2 => SpellManager.W2;

        protected static Spell E => SpellManager.E;

        protected static Spell E2 => SpellManager.E2;

        protected static Spell R => SpellManager.R;

        protected static Spell R2 => SpellManager.R2;

        protected static SpellSlot Flash => SpellManager.Flash;

        protected static SpellDataInst QSpell => SpellManager.QSpell;

        protected static SpellDataInst WSpell => SpellManager.WSpell;

        protected static SpellDataInst ESpell => SpellManager.ESpell;

        protected static SpellDataInst RSpell => SpellManager.RSpell;
    }
}
