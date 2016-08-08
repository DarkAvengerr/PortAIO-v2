using EloBuddy; namespace RethoughtLib.Menu
{
    #region Using Directives

    using System;

    using LeagueSharp.Common;

    using global::RethoughtLib.Menu.Interfaces;

    #endregion

    internal class MenuTranslator
    {
        #region Fields

        /// <summary>
        ///     The menu
        /// </summary>
        internal Menu Menu;

        /// <summary>
        ///     The menu translation
        /// </summary>
        private readonly ITranslation menuTranslation;

        /// <summary>
        ///     Whether the menu got translated
        /// </summary>
        private bool translated;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuTranslator" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="translation">The translation.</param>
        public MenuTranslator(Menu menu, ITranslation translation)
        {
            this.Menu = menu;
            this.menuTranslation = translation;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Translates this instance.
        /// </summary>
        public void Translate()
        {
            if (this.translated || this.menuTranslation == null)
            {
                return;
            }

            this.translated = true;

            foreach (var entry in this.menuTranslation.Strings())
            {
                this.SearchAndTranslate(entry.Key, entry.Value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Searches the menu-item and translates it.
        /// </summary>
        /// <param name="internalName">Name of the internal.</param>
        /// <param name="newDisplayName">New name of the display.</param>
        private void SearchAndTranslate(string internalName, string newDisplayName)
        {
            try
            {
                var item = this.Menu.Item(internalName);

                if (item != null)
                {
                    item.DisplayName = newDisplayName;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed translating > {internalName} into {newDisplayName}. Exception: {ex}");
            }
        }

        #endregion
    }
}