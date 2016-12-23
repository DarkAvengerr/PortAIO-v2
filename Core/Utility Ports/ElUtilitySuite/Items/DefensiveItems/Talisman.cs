using EloBuddy; 
using LeagueSharp.Common; 
namespace ElUtilitySuite.Items.DefensiveItems
{
    using System.Linq;
    using System.Runtime.CompilerServices;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Talisman : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id => ItemId.Talisman_of_Ascension;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Talisman of Ascension";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseTalismanCombo", "Activated").SetValue(true));
            this.Menu.AddItem(new MenuItem("Mode-talisman", "Activation mode: ")).SetValue(new StringList(new[] { "Use always", "Use in combo" }, 1));
            this.Menu.AddItem(new MenuItem("TalismanEnemyHp", "Use on Enemy Hp %").SetValue(new Slider(70)));
            this.Menu.AddItem(new MenuItem("TalismanMyHp", "Use on My Hp %").SetValue(new Slider(50))); 
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return this.Menu.Item("UseTalismanCombo").IsActive() 
                  && (HeroManager.Enemies.Any(
                      x =>
                      x.HealthPercent < this.Menu.Item("TalismanEnemyHp").GetValue<Slider>().Value
                      && x.Distance(this.Player) < 550)
                      || this.Player.HealthPercent < this.Menu.Item("TalismanMyHp").GetValue<Slider>().Value);
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            if (this.Menu.Item("Mode-talisman").GetValue<StringList>().SelectedIndex == 1 && !this.ComboModeActive)
            {
                return;
            }

            Items.UseItem((int)this.Id);
        }

        #endregion
    }
}
