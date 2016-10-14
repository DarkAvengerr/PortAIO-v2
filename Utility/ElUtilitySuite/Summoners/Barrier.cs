using EloBuddy; namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using EloBuddy.SDK;

    using LeagueSharp.Common;

    public class Barrier : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the barrier spell.
        /// </summary>
        /// <value>
        ///     The barrier spell.
        /// </value>
        public EloBuddy.SDK.Spell.Active BarrierSpell { get; private set; }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public LeagueSharp.Common.Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(LeagueSharp.Common.Menu rootMenu)
        {
            if (this.Player.GetSpellSlotFromName("SummonerBarrier") == SpellSlot.Unknown)
            {
                return;
            }

            var predicate = new Func<LeagueSharp.Common.Menu, bool>(x => x.Name == "SummonersMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new LeagueSharp.Common.Menu("Summoners", "SummonersMenu"))
                           : rootMenu.Children.First(predicate);

                var barrierMenu = menu.AddSubMenu(new LeagueSharp.Common.Menu("Barrier", "Barrier"));
            {
                barrierMenu.AddItem(new LeagueSharp.Common.MenuItem("Barrier.Activated", "Barrier activated").SetValue(true));
                barrierMenu.AddItem(new LeagueSharp.Common.MenuItem("barrier.min-health", "Health percentage").SetValue(new LeagueSharp.Common.Slider(20, 1)));
                barrierMenu.AddItem(new LeagueSharp.Common.MenuItem("barrier.min-damage", "Heal on % incoming damage").SetValue(new LeagueSharp.Common.Slider(20, 1)));
                }

            this.Menu = barrierMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            var slot = EloBuddy.Player.Instance.GetSpellSlotFromName("SummonerBarrier");
            if (slot != SpellSlot.Unknown)
            {
                BarrierSpell = new EloBuddy.SDK.Spell.Active(slot);
            }

            if (slot == SpellSlot.Unknown)
            {
                return;
            }

            IncomingDamageManager.RemoveDelay = 500;
            IncomingDamageManager.Skillshots = true;
            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead || this.Player.HasBuff("ChronoShift") ||  !this.BarrierSpell.IsReady() || this.Player.InFountain() || this.Player.HasBuff("recall") || !this.Menu.Item("Barrier.Activated").IsActive())
                {
                    return;
                }

                var enemies = this.Player.CountEnemiesInRange(750f);
                var totalDamage = IncomingDamageManager.GetDamage(this.Player) * 1.1f; 

                if (this.Player.HealthPercent <= this.Menu.Item("barrier.min-health").GetValue<Slider>().Value &&
                    this.BarrierSpell.IsInRange(this.Player) && enemies >= 1)
                {
                    if ((int)(totalDamage / this.Player.Health) > this.Menu.Item("barrier.min-damage").GetValue<Slider>().Value
                        || this.Player.HealthPercent < this.Menu.Item("barrier.min-health").GetValue<Slider>().Value)
                    {
                        BarrierSpell.Cast();
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
