using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius
{
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class Logic
    {
        internal static int lastETime;
        internal static Spell Q, W, E, R;
        internal static SpellSlot Ignite = SpellSlot.Unknown;
        internal static Menu Menu;
        internal static AIHeroClient Me = ObjectManager.Player;
        internal static Orbwalking.Orbwalker Orbwalker;
    }
}
