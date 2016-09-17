using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Functions
{
    public static class Menu
    {
        public static void AddBool(LeagueSharp.Common.Menu menu, string displayName, string name, bool value = true)
        {
            menu.AddItem(new MenuItem(name, displayName).SetValue(value));
        }

        public static void AddSlider(LeagueSharp.Common.Menu menu, string displayName, string name, int startVal, int minVal = 0, int maxVal = 100)
        {
            menu.AddItem(new MenuItem(name, displayName).SetValue(new Slider(startVal, minVal, maxVal)));
        }

        public static void AddKeyBind(LeagueSharp.Common.Menu menu, string displayName, string name, char key, KeyBindType type)
        {
            menu.AddItem(new MenuItem(name, displayName).SetValue(new KeyBind(key, type)));
        }

        public static void AddStringList(LeagueSharp.Common.Menu menu, string name, string displayName, string[] value, int index = 0)
        {
            menu.AddItem(new MenuItem(name, displayName).SetValue(new StringList(value, index)));
        }
    }
}