using EloBuddy; namespace ElUtilitySuite.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class Potions : IPlugin
    {
        #region Delegates

        /// <summary>
        ///     Gets an health item
        /// </summary>
        /// <returns></returns>
        private delegate Items.Item GetHealthItemDelegate();

        #endregion

        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<HealthItem> Items { get; set; }

        /// <summary>
        ///     Gets the set player hp menu value.
        /// </summary>
        /// <value>
        ///     The player hp hp menu value.
        /// </value>
        private int PlayerHp => this.Menu.Item("Potions.Player.Health").GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "MiscMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Misc", "MiscMenu"))
                           : rootMenu.Children.First(predicate);

            var potionsMenu = menu.AddSubMenu(new Menu("Potions", "Potions"));
            {
                potionsMenu.AddItem(new MenuItem("Potions.Activated", "Potions activated").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.Health", "Health potions").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.Biscuit", "Biscuits").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.RefillablePotion", "Refillable Potion").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.HuntersPotion", "Hunters Potion").SetValue(true));
                potionsMenu.AddItem(new MenuItem("Potions.CorruptingPotion", "Corrupting Potion").SetValue(true));

                potionsMenu.AddItem(new MenuItem("seperator.Potions", ""));
                potionsMenu.AddItem(new MenuItem("Potions.Player.Health", "Health percentage").SetValue(new Slider(20)));
            }

            this.Menu = potionsMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = new List<HealthItem>
                             {
                                 new HealthItem { GetItem = () => ItemData.Health_Potion.GetItem(), BuffName = "RegenerationPotion" },
                                 new HealthItem { GetItem = () => ItemData.Total_Biscuit_of_Rejuvenation2.GetItem(), BuffName = "ItemMiniRegenPotion"},
                                 new HealthItem { GetItem = () => ItemData.Refillable_Potion.GetItem(), BuffName = "ItemCrystalFlask" }, 
                                 new HealthItem { GetItem = () => ItemData.Hunters_Potion.GetItem(), BuffName = "ItemCrystalFlaskJungle"},
                                 new HealthItem { GetItem = () => ItemData.Corrupting_Potion.GetItem(), BuffName = "ItemDarkCrystalFlask"}
                             };

            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the player buffs
        /// </summary>
        /// <value>
        ///     The player buffs
        /// </value>
        private bool IsBuffActive()
        {
            return
                this.Items.Any(
                    potion => this.Player.Buffs.Any(
                        b => b.Name.Equals(potion.BuffName, StringComparison.OrdinalIgnoreCase)));
        }

        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (!this.Menu.Item("Potions.Activated").IsActive() || this.Player.IsDead || this.Player.InFountain() || this.Player.Buffs.Any(
                        b => b.Name.ToLower().Contains("Recall") || b.Name.ToLower().Contains("Teleport")))
                {
                    return;
                }

                if (this.Player.HealthPercent < this.PlayerHp)
                {
                    if (this.IsBuffActive())
                    {
                        return;
                    }

                    var item = this.Items.Select(x => x.Item).FirstOrDefault(x => x.IsReady() && x.IsOwned());
                    item?.Cast();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        /// <summary>
        ///     Represents an item that can heal
        /// </summary>
        private class HealthItem
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the get item.
            /// </summary>
            /// <value>
            ///     The get item.
            /// </value>
            public GetHealthItemDelegate GetItem { get; set; }

            /// <summary>
            ///     Gets the buffname
            /// </summary>
            /// <value>
            ///     The buffname
            /// </value>
            public String BuffName { get; set; }

            /// <summary>
            ///     Gets the item.
            /// </summary>
            /// <value>
            ///     The item.
            /// </value>
            public Items.Item Item => this.GetItem();

            #endregion
        }
    }
}