using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Items.OffensiveItems
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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
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
                   && (HeroManager.Enemies.Any(
                       x => (x.Distance(this.Player) < 550) &&
                       x.HealthPercent < this.Menu.Item("BotrkEnemyHp").GetValue<Slider>().Value)
                       || this.Player.HealthPercent < this.Menu.Item("BotrkMyHp").GetValue<Slider>().Value);
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Items.UseItem(
                (int)this.Id,
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                    x.HealthPercent < this.Menu.Item("BotrkEnemyHp").GetValue<Slider>().Value
                    && x.Distance(this.Player) < 550));
        }

        #endregion
    }
}