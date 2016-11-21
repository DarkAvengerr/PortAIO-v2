using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Items.OffensiveItems
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Tiamat : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id => ItemId.Tiamat_Melee_Only;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Tiamat";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return (!ObjectManager.Player.IsChampion("RekSai") || !ObjectManager.Player.IsChampion("Riven")) && this.Menu.Item("Tiamatcombo").IsActive() && this.ComboModeActive
                   && HeroManager.Enemies.Any(x => x.IsValidTarget(385) && !x.IsDead && !x.IsZombie);

        }

        #endregion
    }
}