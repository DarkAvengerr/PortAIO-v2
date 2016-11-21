using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Items
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     Represents an item.
    /// </summary>
    internal abstract class Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive => Entry.Menu.Item("usecombo").GetValue<KeyBind>().Active || Orbwalking.Orbwalker.Instances.Any(x => x.ActiveMode == Orbwalking.OrbwalkingMode.Combo);

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public virtual ItemId Id => 0;

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public virtual string Name => "Unknown Item";

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        public AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public virtual void CreateMenu()
        {
            this.Menu.AddItem(new MenuItem(this.Name + "combo", "Use in Combo").SetValue(true));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldUseItem()
        {
            return false;
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public virtual void UseItem()
        {
            Items.UseItem((int)this.Id);
        }

        #endregion
    }
}
