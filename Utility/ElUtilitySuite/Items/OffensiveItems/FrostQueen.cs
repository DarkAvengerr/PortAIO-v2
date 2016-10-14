using EloBuddy; namespace ElUtilitySuite.Items.OffensiveItems
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

        public static EloBuddy.SDK.Item Frost_Queens_Claim;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            Frost_Queens_Claim = new EloBuddy.SDK.Item(ItemId.Frost_Queens_Claim);
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
                   && (EloBuddy.SDK.EntityManager.Heroes.Enemies.Any(
                       x =>
                       x.HealthPercent < this.Menu.Item("FrostQueenEnemyHp").GetValue<Slider>().Value
                       && x.Distance(this.Player) < 1500)
                       || this.Player.HealthPercent < this.Menu.Item("FrostQueenMyHp").GetValue<Slider>().Value) && Frost_Queens_Claim.IsReady() && Frost_Queens_Claim.IsOwned();
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Frost_Queens_Claim.Cast();
        }

        #endregion
    }
}