using EloBuddy; 
using LeagueSharp.Common; 
namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    class FrostQueen : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id => ItemId.Frost_Queens_Claim;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Frost Queen's Claim";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseFrostQueenCombo", "Use on Combo").SetValue(true));
            this.Menu.AddItem(new MenuItem("FrostQueenEnemyHp", "Use on Enemy Hp %").SetValue(new Slider(70)));
            this.Menu.AddItem(new MenuItem("FrostQueenMyHp", "Use on My Hp %").SetValue(new Slider(100)));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseFrostQueenCombo").IsActive() && this.ComboModeActive
                   && (HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < this.Menu.Item("FrostQueenEnemyHp").GetValue<Slider>().Value
                       && x.Distance(this.Player) < 1500)
                       || this.Player.HealthPercent < this.Menu.Item("FrostQueenMyHp").GetValue<Slider>().Value);
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Items.UseItem((int)this.Id);
        }

        #endregion
    }
}