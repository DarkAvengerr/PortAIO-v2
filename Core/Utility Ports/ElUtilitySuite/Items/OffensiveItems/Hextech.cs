using EloBuddy; 
using LeagueSharp.Common; 
namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Hextech : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id => ItemId.Hextech_Gunblade;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Hextech Gunblade";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseHextechCombo", "Use on Combo").SetValue(true));
            this.Menu.AddItem(new MenuItem("HextechEnemyHp", "Use on Enemy Hp %").SetValue(new Slider(70)));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseHextechCombo").IsActive() && this.ComboModeActive
                   && HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < this.Menu.Item("HextechEnemyHp").GetValue<Slider>().Value
                       && x.Distance(this.Player) < 700 && !x.IsDead && !x.IsZombie);
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
                    x.HealthPercent < this.Menu.Item("HextechEnemyHp").GetValue<Slider>().Value
                    && x.Distance(this.Player) < 700 && !x.IsDead && !x.IsZombie));
        }

        #endregion
    }
}