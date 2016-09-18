using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Guardians
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;

    #endregion

    public class SpellMustBeReady : GuardianBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellMustBeReady" /> class.
        /// </summary>
        /// <param name="spellSlot">The spell slot.</param>
        public SpellMustBeReady(SpellSlot spellSlot)
        {
            this.Func = () => ObjectManager.Player.Spellbook.GetSpell(spellSlot).State == SpellState.Ready;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellMustBeReady" /> class.
        /// </summary>
        /// <param name="spellSlots">The spell slots.</param>
        public SpellMustBeReady(IEnumerable<SpellSlot> spellSlots)
        {
            this.Func =
                () =>
                spellSlots.All(
                    spellSlot => ObjectManager.Player.Spellbook.GetSpell(spellSlot).State == SpellState.Ready);
        }

        #endregion
    }
}