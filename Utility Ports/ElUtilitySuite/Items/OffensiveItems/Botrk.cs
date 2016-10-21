using EloBuddy; namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Botrk : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id => ItemId.Blade_of_the_Ruined_King;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Blade of the Ruined King";

        public static EloBuddy.SDK.Item Blade_of_the_Ruined_King;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            Blade_of_the_Ruined_King = new EloBuddy.SDK.Item(ItemId.Blade_of_the_Ruined_King);
            this.Menu.AddItem(new MenuItem("UseBotrkCombo", "Use on Combo").SetValue(true));
            this.Menu.AddItem(new MenuItem("BotrkEnemyHp", "Use on Enemy Hp %").SetValue(new Slider(100))); //for myo
            this.Menu.AddItem(new MenuItem("BotrkMyHp", "Use on My Hp %").SetValue(new Slider(100))); //for myo
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseBotrkCombo").IsActive() && this.ComboModeActive
                   && (EloBuddy.SDK.EntityManager.Heroes.Enemies.Any(
                       x =>
                       x.HealthPercent < this.Menu.Item("BotrkEnemyHp").GetValue<Slider>().Value
                       && x.Distance(this.Player) < 550)
                       || this.Player.HealthPercent < this.Menu.Item("BotrkMyHp").GetValue<Slider>().Value) && Blade_of_the_Ruined_King.IsOwned() && Blade_of_the_Ruined_King.IsReady();
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Blade_of_the_Ruined_King.Cast(EloBuddy.SDK.EntityManager.Heroes.Enemies.FirstOrDefault(
                    x =>
                    x.HealthPercent < this.Menu.Item("BotrkEnemyHp").GetValue<Slider>().Value
                    && x.Distance(this.Player) < 550 && x.IsValidTarget() && x.IsHPBarRendered && x.IsVisible));
        }

        #endregion
    }
}