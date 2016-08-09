using EloBuddy; namespace ElUtilitySuite.Items.DefensiveItems
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Locket : Item
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public Locket()
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
        public override ItemId Id => ItemId.Locket_of_the_Iron_Solari;

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name => "Locket of the Iron Solari";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem("UseLocketCombo", "Activate").SetValue(true));
            this.Menu.AddItem(new MenuItem("Mode-locket", "Activation mode: "))
                .SetValue(new StringList(new[] { "Use always", "Use in combo" }, 1));
            this.Menu.AddItem(new MenuItem("locket-min-health", "Health percentage").SetValue(new Slider(50, 1)));
            this.Menu.AddItem(
                new MenuItem("locket-min-damage", "Incoming damage percentage").SetValue(new Slider(50, 1)));
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
                if (!this.Menu.Item("UseLocketCombo").IsActive() || !Items.HasItem((int)this.Id) || !Items.CanUseItem((int)this.Id))
                {
                    return;
                }

                if (this.Menu.Item("Mode-locket").GetValue<StringList>().SelectedIndex == 1 && !this.ComboModeActive)
                {
                    return;
                }

                foreach (var ally in HeroManager.Allies.Where(a => a.IsValidTarget(600f, false) && !a.IsRecalling()))
                {
                    var enemies = ally.CountEnemiesInRange(600f);
                    var totalDamage = IncomingDamageManager.GetDamage(ally) * 1.1f;

                    if (ally.HealthPercent <= this.Menu.Item("locket-min-health").GetValue<Slider>().Value
                        && enemies >= 1)
                    {
                        if ((int)(totalDamage / ally.Health)
                            > this.Menu.Item("locket-min-damage").GetValue<Slider>().Value
                            || ally.HealthPercent < this.Menu.Item("locket-min-health").GetValue<Slider>().Value)
                        {
                            Items.UseItem((int)this.Id, ally);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[ELUTILITYSUITE - LOCKET] Used for: {0} - health percentage: {1}%", ally.ChampionName, (int)ally.HealthPercent);
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
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