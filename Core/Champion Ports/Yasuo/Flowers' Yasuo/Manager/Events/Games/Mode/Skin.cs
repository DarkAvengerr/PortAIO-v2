using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events
{
    using LeagueSharp.Common;
    using LeagueSharp;

    internal class Skin : Logic
    {
        internal static void Init()
        {
            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
            }
        }
    }
}