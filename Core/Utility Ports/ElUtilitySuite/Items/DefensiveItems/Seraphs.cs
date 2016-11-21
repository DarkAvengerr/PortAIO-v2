using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Items.DefensiveItems
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class Seraphs : Item
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public Seraphs()
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
        public override ItemId Id => (ItemId)3040;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Seraph's embrace";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseSeraphsCombo", "Activated").SetValue(true));
            this.Menu.AddItem(new MenuItem("Mode-seraphs", "Activation mode: ")).SetValue(new StringList(new[] { "Use always", "Use in combo" }, 1));
            this.Menu.AddItem(new MenuItem("seraphs-min-health", "Health percentage").SetValue(new Slider(20, 1)));
            this.Menu.AddItem(new MenuItem("seraphs-min-damage", "Incoming damage percentage").SetValue(new Slider(20, 1)));
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
                if (!ItemData.Seraphs_Embrace.GetItem().IsOwned() || !this.Menu.Item("UseSeraphsCombo").IsActive())
                {
                    return;
                }

                if (this.Menu.Item("Mode-seraphs").GetValue<StringList>().SelectedIndex == 1 && !this.ComboModeActive)
                {
                    return;
                }

                var enemies = this.Player.CountEnemiesInRange(800);
                var totalDamage = IncomingDamageManager.GetDamage(this.Player) * 1.1f;

                if (this.Player.HealthPercent <= this.Menu.Item("seraphs-min-health").GetValue<Slider>().Value && enemies >= 1)
                {
                    if ((int)(totalDamage / this.Player.Health)
                        > this.Menu.Item("seraphs-min-damage").GetValue<Slider>().Value
                        || this.Player.HealthPercent < this.Menu.Item("seraphs-min-health").GetValue<Slider>().Value)
                    {
                        Items.UseItem((int)this.Id, this.Player);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[ELUTILITYSUITE - SERAPHS] Used for: {0} - health percentage: {1}%", this.Player.ChampionName, (int)this.Player.HealthPercent);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}