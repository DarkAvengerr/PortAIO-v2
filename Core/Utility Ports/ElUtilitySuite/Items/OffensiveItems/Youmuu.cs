using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Youmuu : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id => ItemId.Youmuus_Ghostblade;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Youmuu";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("Youmuucombo").IsActive() && this.ComboModeActive
                   && HeroManager.Enemies.Any(
                       x => x.Distance(this.Player) < (ObjectManager.Player.IsChampion("Jhin") ? 550f : Orbwalking.GetRealAutoAttackRange(this.Player)));
        }


        #endregion
    }
}