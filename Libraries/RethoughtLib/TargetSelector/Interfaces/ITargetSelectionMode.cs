using EloBuddy; namespace RethoughtLib.TargetSelector.Interfaces
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;

    #endregion

    public interface ITargetSelectionMode
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <returns></returns>
        AIHeroClient GetTarget(List<AIHeroClient> targets, AIHeroClient requester);

        #endregion
    }
}