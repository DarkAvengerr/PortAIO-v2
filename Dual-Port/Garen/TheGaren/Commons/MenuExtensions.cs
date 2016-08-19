using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.Common;

namespace TheGaren
{
    public static class MenuExtensions
    {
        public delegate void ValueChangedHandler<in T>(T value);

        public static MenuItem AddMItem(this Menu menu, string name, string internalName = "")
        {
            return menu.AddItem(new MenuItem(string.IsNullOrEmpty(internalName) ? GetValidName(menu, name) : internalName, name));
        }

        public static MenuItem AddMItem<T>(this Menu menu, string name, T value, string internalName = "")
        {
            return menu.AddItem(new MenuItem(string.IsNullOrEmpty(internalName) ? GetValidName(menu, name) : internalName, name).SetValue(GetValidValue(value)));
        }

        public static MenuItem AddMItem<T>(this Menu menu, string name, T value, ValueChangedHandler<T> handler, string internalName = "")
        {
            var menuItem = new MenuItem(string.IsNullOrEmpty(internalName) ? GetValidName(menu, name) : internalName, name).SetValue(GetValidValue(value));
            menuItem.ValueChanged += (sender, args) => handler(args.GetNewValue<T>());
            handler(menuItem.GetValue<T>());
            return menu.AddItem(menuItem);
        }

        public static MenuItem AddMItem<T>(this Menu menu, string name, T value, EventHandler<OnValueChangeEventArgs> handler, string internalName = "")
        {
            var menuItem = new MenuItem(string.IsNullOrEmpty(internalName) ? GetValidName(menu, name) : internalName, name).SetValue(GetValidValue(value));
            menuItem.ValueChanged += handler;
            handler(menuItem, new OnValueChangeEventArgs(menuItem.GetValue<T>(), menuItem.GetValue<T>()));
            return menu.AddItem(menuItem);
        }

        public static Menu CreateSubmenu(this Menu menu, string name)
        {
            return menu.AddSubMenu(new Menu(name, GetValidName(menu, name)));
        }

        private static string GetValidName(string name)
        {
            return name.Replace(" ", "").Replace(".", "");
        }

        private static string GetValidName(Menu parent, string name)
        {
            var validName = parent.Name + "." + GetValidName(name);
            while (parent.Items.Any(item => item.Name == validName) || parent.Children.Any(child => child.Name == validName))
                validName += "_";
            return validName;
        }

        private static T GetValidValue<T>(T value)
        {
            if (value is Slider)
            {
                var slider = (Slider)(object)value;
                if (slider.MaxValue <= slider.MinValue) slider.MaxValue = slider.MinValue + 1;
                if (slider.Value < slider.MinValue) slider.Value = slider.MinValue;
                if (slider.Value > slider.MaxValue) slider.Value = slider.MaxValue;
                return (T)(object)slider;
            }
            //Todo: find other cases

            return value;
        }
    }
}