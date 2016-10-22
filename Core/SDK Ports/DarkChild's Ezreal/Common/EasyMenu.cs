using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Common
{
    using System.Windows.Forms;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;

    using Menu = LeagueSharp.SDK.UI.Menu;

    internal static class EasyMenu
    {
        public static Menu CreateMenu(this Menu menu, string id, string Name, bool root = false, string uniquestring = "")
        {
            return menu.Add(new Menu(id, Name, root, uniquestring));
        }

        public static MenuBool CreateBool(this Menu menu, string id, string Name, bool value = true, string uniquestring = "")
        {
            return menu.Add(new MenuBool(id, Name, value, uniquestring));
        }

        public static MenuSliderButton CreateSliderButton(this Menu menu, string id, string Name, int dValue = 0, int minValue = 0, int maxValue = 100, bool bValue = true, string uniquestring = "")
        {
            return menu.Add(new MenuSliderButton(id, Name, dValue, minValue, maxValue, bValue, uniquestring));
        }

        public static MenuSlider CreateSlider(this Menu menu, string id, string Name, int dValue = 0, int minValue = 0, int maxValue = 100, string uniquestring = "")
        {
            return menu.Add(new MenuSlider(id, Name, dValue, minValue, maxValue, uniquestring));
        }

        public static MenuKeyBind CreateKeyBind(this Menu menu, string id, string Name, Keys key, KeyBindType type, string uniquestring = "")
        {
            return menu.Add(new MenuKeyBind(id, Name, key, type));
        }

        public static MenuSeparator CreateSeparator(this Menu menu, string id, string Name)
        {
            return menu.Add(new MenuSeparator(id, Name));
        }

        public static int GetListIndex(this Menu menu, string str)
        {
            return menu[str].GetValue<MenuList>().Index;
        }

        public static int GetListIndex(this AMenuComponent menu, string str)
        {
            return menu[str].GetValue<MenuList>().Index;
        }

        public static bool GetKeyBind(this Menu menu, string str)
        {
            return menu[str].GetValue<MenuKeyBind>().Active;
        }

        public static bool GetKeyBind(this AMenuComponent menu, string str)
        {
            return menu[str].GetValue<MenuKeyBind>().Active;
        }

        public static bool GetBool(this Menu menu, string str)
        {
            return menu[str].GetValue<MenuBool>().Value;
        }

        public static bool GetBool(this AMenuComponent menu, string str)
        {
            return menu[str].GetValue<MenuBool>().Value;
        }

        public static int GetSlider(this Menu menu, string str)
        {
            return menu[str].GetValue<MenuSlider>().Value;
        }

        public static int GetSlider(this AMenuComponent menu, string str)
        {
            return menu[str].GetValue<MenuSlider>().Value;
        }

        public static int GetSliderButton(this AMenuComponent menu, string str)
        {
            return menu[str].GetValue<MenuSliderButton>().Value;
        }

        public static bool GetSliderBool(this Menu menu, string str)
        {
            return menu[str].GetValue<MenuSliderButton>().BValue;
        }

        public static bool GetSliderBool(this AMenuComponent menu, string str)
        {
            return menu[str].GetValue<MenuSliderButton>().BValue;
        }
    }
}
