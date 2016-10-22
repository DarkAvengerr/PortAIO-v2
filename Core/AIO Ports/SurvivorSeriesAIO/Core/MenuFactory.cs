// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuFactory.cs" company="SurvivorSeriesAIO">
//     Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public static class MenuFactory
    {
        public static string Prefix { get; set; }

        public static Menu Create(string name)
        {
            return new Menu(name, name, true);
        }

        public static MenuItem CreateItem(Menu parent, string name)
        {
            return parent.AddItem(new MenuItem(name, Prefix + name, true));
        }

        public static MenuItem CreateItem<T>(Menu parent, string name, T value) where T : struct
        {
            return CreateItem(parent, name).SetValue(value);
        }

        public static MenuItem CreateList(Menu parent, string name, string[] list)
        {
            return CreateItem(parent, name, new StringList(list));
        }

        public static Menu CreateMenu(Menu parent, string name)
        {
            return parent.AddSubMenu(new Menu(name, name, false));
        }

        public static MenuItem CreateSlider(Menu parent, string name, int value, int max = 100)
        {
            return CreateItem(parent, name, new Slider(value, 0, max));
        }

        public static MenuItem WithTooltip(MenuItem item, string text, Color? color = null)
        {
            return item.SetTooltip(text, color);
        }
    }
}