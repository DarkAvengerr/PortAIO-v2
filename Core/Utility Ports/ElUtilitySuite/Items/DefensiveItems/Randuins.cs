using EloBuddy; namespace ElUtilitySuite.Items.DefensiveItems
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Randuins : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id => ItemId.Randuins_Omen;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Randuin's Omen";

        public static EloBuddy.SDK.Item Randuins_Omen;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            Randuins_Omen = new EloBuddy.SDK.Item(ItemId.Randuins_Omen);
            this.Menu.AddItem(new MenuItem("UseRanduinsCombo", "Activate").SetValue(true));
            this.Menu.AddItem(new MenuItem("Mode-randuins", "Activation mode: ")).SetValue(new StringList(new[] { "Use always", "Use in combo" }, 1));
            this.Menu.AddItem(new MenuItem("RanduinsCount", "Use on enemies hit").SetValue(new Slider(3, 1, 5)));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseRanduinsCombo").IsActive() && this.Player.CountEnemiesInRange(500f) >= this.Menu.Item("RanduinsCount").GetValue<Slider>().Value && Randuins_Omen.IsOwned() && Randuins_Omen.IsReady();
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            if (this.Menu.Item("Mode-randuins").GetValue<StringList>().SelectedIndex == 1 && !this.ComboModeActive)
            {
                return;
            }

            Randuins_Omen.Cast();
        }

        #endregion
    }
}