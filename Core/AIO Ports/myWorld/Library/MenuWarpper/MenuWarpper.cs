using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Library.MenuWarpper
{
    static class MenuWarpper
    {
        public static void AddBool(this Menu menu, string name, string param, bool def = true)
        {
            menu.AddItem(new MenuItem(name, param).SetValue(def));
        }

        public static void AddSlice(this Menu menu, string name, string param = "Dont use if my mana => (%)", int f = 0, int s = 0, int t = 100)
        {
            menu.AddItem(new MenuItem(name, param).SetValue(new Slider(f, s, t)));
        }

        public static void AddSlice(this Menu menu, string name, int f, int s = 0, int t = 100)
        {
            menu.AddItem(new MenuItem(name, "Dont use if my mana => (%)").SetValue(new Slider(f, s, t)));
        }

        public static void AddInfo(this Menu menu, string name = "Blank", string param = "")
        {
            menu.AddItem(new MenuItem(name, param));
        }

        public static void AddList(this Menu menu, string name, string param, StringList List)
        {
            menu.AddItem(new MenuItem(name, param).SetValue(List));
        }

        public static void AddKeyToggle(this Menu menu, string name, string param, string Key)
        {
            menu.AddItem(new MenuItem(name, param).SetValue(new KeyBind(Key.ToCharArray()[0], KeyBindType.Toggle)));
        }

        public static void AddKeyPress(this Menu menu, string name, string param, string Key)
        {
            menu.AddItem(new MenuItem(name, param).SetValue(new KeyBind(Key.ToCharArray()[0], KeyBindType.Press)));
        }

        public static bool GetBool(this Menu menu, string name)
        {
            return menu.Item(name).GetValue<bool>();
        }

        public static int GetSlice(this Menu menu, string name)
        {
            return menu.Item(name).GetValue<Slider>().Value;
        }

        public static int GetListIndex(this Menu menu, string name)
        {
            return menu.Item(name).GetValue<StringList>().SelectedIndex;
        }

        public static string GetList(this Menu menu, string name)
        {
            return menu.Item(name).GetValue<StringList>().SelectedValue;
        }

        public static bool GetKeyToggle(this Menu menu, string name)
        {
            return menu.Item(name).GetValue<KeyBind>().Active;
        }

        public static bool GetKeyPress(this Menu menu, string name)
        {
            return menu.Item(name).GetValue<KeyBind>().Active;
        }
    }
}
