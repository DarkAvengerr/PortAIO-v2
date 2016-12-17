using EloBuddy; 
using LeagueSharp.Common; 
namespace FlowersRivenCommon
{
    using LeagueSharp.Common;
    using System.Drawing;

    public static class MenuManagers
    {
        public static bool GetBool(this Menu Menu, string MenuItemName, bool unique = true)
        {
            return Menu.Item(MenuItemName, unique).GetValue<bool>();
        }

        public static bool GetKey(this Menu Menu, string MenuItemName, bool unique = true)
        {
            return Menu.Item(MenuItemName, unique).GetValue<KeyBind>().Active;
        }

        public static int GetSlider(this Menu Menu, string MenuItemName, bool unique = true)
        {
            return Menu.Item(MenuItemName, unique).GetValue<Slider>().Value;
        }

        public static int GetList(this Menu Menu, string MenuItemName, bool unique = true)
        {
            return Menu.Item(MenuItemName, unique).GetValue<StringList>().SelectedIndex;
        }

        public static Color GetColor(this Menu Menu, string MenuItemName, bool unique = true)
        {
            return Menu.Item(MenuItemName, unique).GetValue<Circle>().Color;
        }

        public static Circle GetCircle(this Menu Menu, string MenuItemName, bool unique = true)
        {
            return Menu.Item(MenuItemName, unique).GetValue<Circle>();
        }
    }
}
