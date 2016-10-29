
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbTracker
{
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The menu class.
    /// </summary>
    internal class Menus
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Builds the general Menu.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            ///     The general Menu.
            /// </summary>
            Vars.Menu = new Menu("nabbtracker", "NabbTracker", true);
            {
                /// <summary>
                ///     Sets the menu for the SpellTracker.
                /// </summary>
                Vars.SpellTrackerMenu = new Menu("spelltracker", "Spell Tracker");
                {
                    Vars.SpellTrackerMenu.Add(new MenuBool("me", "Me"));
                    Vars.SpellTrackerMenu.Add(new MenuBool("allies", "Allies", true));
                    Vars.SpellTrackerMenu.Add(new MenuBool("enemies", "Enemies", true));
                }
                Vars.Menu.Add(Vars.SpellTrackerMenu);

                /// <summary>
                ///     Sets the menu for the ExpTracker.
                /// </summary>
                Vars.ExpTrackerMenu = new Menu("exptracker", "Experience Tracker");
                {
                    Vars.ExpTrackerMenu.Add(new MenuBool("me", "Me"));
                    Vars.ExpTrackerMenu.Add(new MenuBool("allies", "Allies", true));
                    Vars.ExpTrackerMenu.Add(new MenuBool("enemies", "Enemies", true));
                }
                Vars.Menu.Add(Vars.ExpTrackerMenu);

                /// <summary>
                ///     The miscellaneous Menu.
                /// </summary>
                Vars.MiscMenu = new Menu("miscellaneous", "Miscellaneous", true);
                {
                    Vars.MiscMenu.Add(new MenuBool("name", "Adjust the Bars for the Summoner Names"));

                    /// <summary>
                    ///     The Colorblind Menu.
                    /// </summary>
                    Vars.ColorblindMenu = new Menu("colorblind", "Colorblind Menu");
                    {
                        Vars.ColorblindMenu.Add(new MenuSeparator("separator", "Select your colorblind mode."));
                        Vars.ColorblindMenu.Add(
                            new MenuList<string>(
                                "mode",
                                "Colorblind Mode",
                                new[] { "Normal", "Deuteranopia", "Protanopia", "Tritanopia", "Achromatopsia" }));
                    }
                    Vars.MiscMenu.Add(Vars.ColorblindMenu);
                }
                Vars.Menu.Add(Vars.MiscMenu);
            }
            Vars.Menu.Attach();
        }

        #endregion
    }
}