using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Utility
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class AutoLantern : IPlugin
    {
        #region Fields

        public GameObject ThreshLantern;

        #endregion

        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the click below hp menu value.
        /// </summary>
        /// <value>
        ///     The lantern below hp menu value.
        /// </value>
        private int ClickBelowHp => this.Menu.Item("ThreshLanternHPSlider").GetValue<Slider>().Value;

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
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "MiscMenu");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Misc", "MiscMenu"))
                           : rootMenu.Children.First(predicate);

            var autoLanternMenu = menu.AddSubMenu(new Menu("Thresh Lantern", "Threshlantern"));
            {
                autoLanternMenu.AddItem(new MenuItem("ThreshLantern", "Auto click Thresh lantern").SetValue(true));
                autoLanternMenu.AddItem(new MenuItem("ThreshHawkMode", "Only use key").SetValue(false));
                autoLanternMenu.AddItem(
                    new MenuItem("ThreshLanternHotkey", "Hotkey").SetValue(new KeyBind('M', KeyBindType.Press)));
                autoLanternMenu.AddItem(
                    new MenuItem("ThreshLanternHPSlider", "Click when HP %").SetValue(new Slider(20)));
            }

            this.Menu = autoLanternMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            try
            {
                Game.OnUpdate += this.OnUpdate;
                GameObject.OnCreate += this.OnCreate;
                GameObject.OnDelete += this.OnDelete;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        ///     Fired when an object is created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCreate(GameObject sender, EventArgs args)
        {
            try
            {
                if (!sender.IsValid || this.Player.IsChampion("Thresh") || !sender.IsAlly || sender.Type != GameObjectType.obj_AI_Minion)
                {
                    return;
                }

                if (sender.Name.Equals("ThreshLantern", StringComparison.OrdinalIgnoreCase))
                {
                    this.ThreshLantern = sender;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Fired when an object is deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnDelete(GameObject sender, EventArgs args)
        {
            try
            {
                if (!sender.IsValid || this.ThreshLantern == null)
                {
                    return;
                }

                if (sender.NetworkId == this.ThreshLantern.NetworkId)
                {
                    this.ThreshLantern = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead || !this.Menu.Item("ThreshLantern").IsActive() || this.ThreshLantern == null
                    || !this.ThreshLantern.IsValid)
                {
                    return;
                }

                if (this.Menu.Item("ThreshHawkMode").IsActive() ? this.Menu.Item("ThreshLanternHotkey").GetValue<KeyBind>().Active :
                    this.Menu.Item("ThreshLanternHotkey").GetValue<KeyBind>().Active || this.Player.HealthPercent < this.ClickBelowHp)
                {
                    if (this.ThreshLantern.Position.Distance(this.Player.Position) <= 500)
                    {
                        this.Player.Spellbook.CastSpell((SpellSlot)62, this.ThreshLantern);
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