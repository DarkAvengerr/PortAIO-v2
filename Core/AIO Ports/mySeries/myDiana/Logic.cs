using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana
{
    using Manager.Events;
    using Manager.Menu;
    using Manager.Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class Logic
    {
        internal static int qCd;
        internal static int lastRCast;
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        internal static SpellSlot Ignite = SpellSlot.Unknown;
        internal static SpellSlot Flash = SpellSlot.Unknown;
        internal static Menu Menu;
        internal static AIHeroClient Me;
        internal static Orbwalking.Orbwalker Orbwalker;

        internal static void LoadAssembly()
        {
            Me = ObjectManager.Player;

            SpellManager.Init();
            MenuManager.Init();
            EventManager.Init();
        }
    }
}