#region copyright

// Copyright (c) KyonLeague 2016
// If you want to copy parts of the code, please inform the author and give appropiate credits
// File: Config.cs
// Author: KyonLeague
// Contact: "cryz3rx" on Skype 

#endregion

#region usage

using LeagueSharp.SDK.UI;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RAREKarthus
{
    internal static class Config
    {
        #region Static Fields

        private static int _cBlank = -1;

        #endregion

        #region Public Methods and Operators
        /// <summary>
        /// lets you create a new menupoint inside a <seealso cref="Menu"/>.
        /// </summary>
        /// <param name="subMenu">Your SubMenu to add it to</param>
        /// <param name="name">the so called ID</param>
        /// <param name="display">The displayed name inside the game</param>
        /// <param name="state">the default state of the menu</param>
        /// <returns>returns a <seealso cref="MenuBool"/> the can be used.</returns>
        public static MenuBool Bool(this Menu subMenu, string name, string display, bool state = true)
        {
            return subMenu.Add(new MenuBool(name, display, state));
        }

        public static MenuList List(this Menu subMenu, string name, string display, string[] array, int value = 0)
        {
            return subMenu.Add(new MenuList<string>(name, display, array) {Index = value});
        }

        public static MenuSeparator Separator(this Menu subMenu, string display)
        {
            _cBlank += 1;
            return subMenu.Add(new MenuSeparator("blank" + _cBlank, display));
        }

        public static MenuSlider Slider(this Menu subMenu, string name, string display,
            int cur, int min = 0, int max = 100)
        {
            return subMenu.Add(new MenuSlider(name, display, cur, min, max));
        }

        #endregion
    }
}