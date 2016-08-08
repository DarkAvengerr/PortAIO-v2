using System;
using LeagueSharp.Common;

namespace DZLib.MenuExtensions
{
    public static class MenuExtensions
    {
        /// <summary>
        /// Adds a bool to the menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="defaultValue">the default value for the item.</param>
        /// <returns></returns>
        public static MenuItem AddBool(this LeagueSharp.Common.Menu menu, string name, string displayName, bool defaultValue = false)
        {
            return menu.AddItem(new MenuItem(name, displayName).SetValue(defaultValue));
        }

        /// <summary>
        /// Adds a slider.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static MenuItem AddSlider(this LeagueSharp.Common.Menu menu, string name, string displayName, Tuple<int, int, int> values)
        {
            return menu.AddItem(new MenuItem(name, displayName).SetValue(new Slider(values.Item1, values.Item2, values.Item3)));
        }

        /// <summary>
        /// Adds a slider.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns></returns>
        public static MenuItem AddSlider(this LeagueSharp.Common.Menu menu, string name, string displayName, int value, int min, int max)
        {
            return menu.AddItem(new MenuItem(name, displayName).SetValue(new Slider(value, min, max)));
        }

        /// <summary>
        /// Adds a keybind.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static MenuItem AddKeybind(this LeagueSharp.Common.Menu menu, string name, string displayName, Tuple<uint, KeyBindType> value)
        {
            return menu.AddItem(new MenuItem(name, displayName).SetValue(new KeyBind(value.Item1, value.Item2)));
        }

        /// <summary>
        /// Adds a text.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        public static MenuItem AddText(this LeagueSharp.Common.Menu menu, string name, string displayName)
        {
            return menu.AddItem(new MenuItem(name, displayName));
        }

        /// <summary>
        /// Adds a string list.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static MenuItem AddStringList(this LeagueSharp.Common.Menu menu, string name, string displayName, string[] value, int index = 0)
        {
            return menu.AddItem(new MenuItem(name, displayName).SetValue(new StringList(value, index)));
        }
    }
}