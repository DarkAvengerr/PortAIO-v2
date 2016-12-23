using EloBuddy; 
using LeagueSharp.Common; 
namespace ElUtilitySuite.Items.DefensiveItems
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Logging;
    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Redemption : Item
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public Redemption()
        {
            IncomingDamageManager.RemoveDelay = 500;
            IncomingDamageManager.Skillshots = true;
            Game.OnUpdate += this.Game_OnUpdate;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id => (ItemId)3107;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Redemption";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseRedemptionCombo", "Activate").SetValue(true));
            this.Menu.AddItem(new MenuItem("Mode-Redemption", "Activation mode: "))
                .SetValue(new StringList(new[] { "Use always", "Use in combo" }, 1));
            this.Menu.AddItem(new MenuItem("Redemption-min-health", "Health percentage").SetValue(new Slider(50, 1)));
            this.Menu.AddItem(
                new MenuItem("Redemption-min-damage", "Incoming damage percentage").SetValue(new Slider(50, 1)));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when the game updates
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Game_OnUpdate(EventArgs args)
        {
            try
            {
                if (!this.Menu.Item("UseRedemptionCombo").IsActive() || !Items.HasItem((int)this.Id) || !Items.CanUseItem((int)this.Id))
                {
                    return;
                }

                if (this.Menu.Item("Mode-Redemption").GetValue<StringList>().SelectedIndex == 1 && !this.ComboModeActive)
                {
                    return;
                }

                foreach (var ally in HeroManager.Allies.Where(a => a.IsValidTarget(2500f, false) && !a.IsRecalling()))
                {
                    var enemies = ally.CountEnemiesInRange(800f);
                    var totalDamage = IncomingDamageManager.GetDamage(ally) * 1.1f;

                    if (ally.HealthPercent <= this.Menu.Item("Redemption-min-health").GetValue<Slider>().Value
                        && enemies >= 1)
                    {
                        if ((int)(totalDamage / ally.Health)
                            > this.Menu.Item("Redemption-min-damage").GetValue<Slider>().Value
                            || ally.HealthPercent < this.Menu.Item("Redemption-min-health").GetValue<Slider>().Value)
                        {
                            Items.UseItem((int)this.Id, Prediction.GetPrediction(ObjectManager.Player, 2500f).UnitPosition);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@Redemtpion.cs: An error occurred: {0}", e);
            }
        }

        #endregion
    }
}