using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.SpellParent
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;

    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal interface ISpellIndex
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        Dictionary<SpellSlot, SpellChild> Spells { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets or sets the <see cref="SpellChild" /> with the specified spell slot.
        /// </summary>
        /// <value>
        ///     The <see cref="SpellChild" />.
        /// </value>
        /// <param name="spellSlot">The spell slot.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Can't return a SpellChild for a SpellSlot that is non-existing</exception>
        SpellChild this[SpellSlot spellSlot] { get; }

        #endregion
    }
}