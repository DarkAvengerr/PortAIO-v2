using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Config
{
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using System.Windows.Forms;
    using Menu = LeagueSharp.SDK.UI.Menu;


    internal static class ExterienceManager
    {

        private static int cBlank = -1;

        public static MenuBool GetBool(this Menu Menu, string name, string display, bool state = true)
        {
            return Menu.Add(new MenuBool(name, display, state));
        }

        public static MenuKeyBind GetKeyBind(this Menu Menu, string name, string display, Keys key, KeyBindType type = KeyBindType.Press)
        {
            return Menu.Add(new MenuKeyBind(name, display, key, type));
        }

        public static MenuSeparator GetSeparator(this Menu Menu, string display)
        {
            cBlank += 1;
            return Menu.Add(new MenuSeparator("blank" + cBlank, display));
        }

        public static MenuList GetList(this Menu Menu, string name, string display, string[] array, int value = 0)
        {
            return Menu.Add(new MenuList<string>(name, display, array) { Index = value });
        }

        public static MenuSlider GetSlider(this Menu Menu, string name, string display, int cur, int min = 0, int max = 100)
        {
            return Menu.Add(new MenuSlider(name, display, cur, min, max));
        }

        public static MenuSliderButton GetSliderButton(this Menu Menu, string name, string display, int cur, int min = 0, int max = 100, bool state = true)
        {
            return Menu.Add(new MenuSliderButton(name, display, cur, min, max, state) { BValue = true });
        }
    }
}