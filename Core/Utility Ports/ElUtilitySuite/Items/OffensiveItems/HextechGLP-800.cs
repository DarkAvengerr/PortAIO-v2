using EloBuddy; 
using LeagueSharp.Common; 
namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    class HextechGLP_800 : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id => (ItemId)3030;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Hextech GLP 800";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseHextech800Combo", "Use on Combo").SetValue(true));
            this.Menu.AddItem(new MenuItem("Hextech800EnemyHp", "Use on Enemy Hp %").SetValue(new Slider(70)));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseHextech800Combo").IsActive() && this.ComboModeActive
                   && HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < this.Menu.Item("Hextech800EnemyHp").GetValue<Slider>().Value
                       && x.Distance(this.Player) < 700 && !x.IsDead && !x.IsZombie);
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            var objAiHero = HeroManager.Enemies.FirstOrDefault(
                x => x.HealthPercent < this.Menu.Item("Hextech800EnemyHp").GetValue<Slider>().Value 
                && x.Distance(this.Player) < 500 && !x.IsDead && !x.IsZombie);

            if (objAiHero != null)
            {
                Items.UseItem((int)this.Id, objAiHero.ServerPosition);
            }
        }

        #endregion
    }
}
