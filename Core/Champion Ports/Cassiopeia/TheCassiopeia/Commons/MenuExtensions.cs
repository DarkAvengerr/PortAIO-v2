using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace TheCassiopeia.Commons
{
    public static class MenuCommons
    {
        private static readonly Dictionary<MenuItem, EventHandler<OnValueChangeEventArgs>> HandlerMapper = new Dictionary<MenuItem, EventHandler<OnValueChangeEventArgs>>();
        private static readonly Dictionary<MenuItem, Menu> MenuMapper = new Dictionary<MenuItem, Menu>();
        private static readonly Dictionary<MenuItem, Type> MenuTypeMapper = new Dictionary<MenuItem, Type>();

        public static Menu CreateSubmenu(this Menu menu, string name)
        {
            var newMenu = new Menu(name, name.Replace(" ", ""));
            menu.AddSubMenu(newMenu);
            return newMenu;
        }

        public static MenuItem AddMItem(this Menu menu, string name, string internalName = "")
        {
            return menu.AddItem(new MenuItem(string.IsNullOrEmpty(internalName) ? menu.Name.Replace(" ", "") + "." + name.Replace(" ", "").Replace(".", "") : internalName, name));
        }

        public static MenuItem AddMItem<T>(this Menu menu, string name, T value, string internalName = "")
        {
            return menu.AddItem(new MenuItem(string.IsNullOrEmpty(internalName) ? menu.Name.Replace(" ", "") + "." + name.Replace(" ", "").Replace(".", "") : internalName, name).SetValue(value));
        }

        public static MenuItem AddMItem<T>(this Menu menu, string name, T value, EventHandler<OnValueChangeEventArgs> handler, string internalName = "")
        {
            var menuItem = new MenuItem(string.IsNullOrEmpty(internalName) ? menu.Name.Replace(" ", "") + "." + name.Replace(" ", "").Replace(".", "") : internalName, name).SetValue(value);
            menuItem.ValueChanged += handler;
            HandlerMapper.Add(menuItem, handler);
            MenuMapper.Add(menuItem, menu);
            MenuTypeMapper.Add(menuItem, typeof (T));
            return menu.AddItem(menuItem);
        }

        public static T GetValue<T>(this Menu menu, string name)
        {
            return GetMenuItem(menu, name).GetValue<T>();
        }

        public static MenuItem GetMenuItem(this Menu rootMenu, string name)
        {
            Menu result = rootMenu;
            var path = name.Split('.');
            for (int index = 0; index < path.Length - 1; index++)
            {
                result = result.SubMenu(path[index]);
            }
            return result.Item(name);
        }

        public static void ProcStoredValueChanged<T>(this MenuItem menu)
        {
            HandlerMapper[menu](menu, new OnValueChangeEventArgs(menu.GetValue<T>(), menu.GetValue<T>()));
        }

        public static void ProcStoredValueChanged<T>(this Menu menu)
        {
            foreach (var eventHandler in HandlerMapper.Where(item => MenuMapper[item.Key] == menu && MenuTypeMapper[item.Key] == typeof(T)))
                eventHandler.Value(eventHandler.Key, new OnValueChangeEventArgs(eventHandler.Key.GetValue<T>(), eventHandler.Key.GetValue<T>()));
        }
    }
}
