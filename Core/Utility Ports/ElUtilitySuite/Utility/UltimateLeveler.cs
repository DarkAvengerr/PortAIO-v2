using EloBuddy; 
using LeagueSharp.Common; 
namespace ElUtilitySuite.Utility
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Logging;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    /// Automatically levels R.
    /// </summary>
    internal class UltimateLeveler : IPlugin
    {
        #region Static Fields

        /// <summary>
        ///     A collection of champions that should not be auto leveled.
        /// </summary>
        public static string[] BlacklistedChampions = { "Elise", "Nidalee", "Udyr" };

        /// <summary>
        /// The random
        /// </summary>
        private static Random random;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "MiscMenu");
            var menu = !rootMenu.Children.Any(predicate) ? rootMenu.AddSubMenu(new Menu("Misc", "MiscMenu")) : rootMenu.Children.First(predicate);

            this.Menu = menu.AddSubMenu(new Menu("Ultimate Leveler", "UltLeveler"));
            this.Menu.AddItem(new MenuItem("AutoLevelR", "Automaticly Level Up Ultimate").SetValue(true));

            random = new Random(Environment.TickCount);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            try
            {
                if (
                    !BlacklistedChampions.Any(
                        x => x.Equals(ObjectManager.Player.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Obj_AI_Base.OnLevelUp += this.ObjAiBaseOnOnLevelUp;
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@UltimateLeveler.cs: An error occurred: {0}", e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when an <see cref="Obj_AI_Base" /> levels up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ObjAiBaseOnOnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            try
            {
                if (!sender.IsMe)
                {
                    return;
                }

                if (this.Menu.Item("AutoLevelR").IsActive())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(random.Next(100, 1000), () => sender.Spellbook.LevelSpell(SpellSlot.R));
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@UltimateLeveler.cs: An error occurred: {0}", e);
            }
        }

        #endregion
    }
}