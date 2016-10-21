using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Implementations
{
    #region Using Directives

    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    public abstract class SpellChild : ChildBase
    {
        /// <summary>
        /// Gets or sets the spell.
        /// </summary>
        /// <value>
        /// The spell.
        /// </value>
        public abstract Spell Spell { get; set; }
    }
}