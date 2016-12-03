using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze
{
    using Manager.Events;
    using Manager.Menu;
    using Manager.Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class Logic
    {
        internal static float Qcd, QcdEnd;
        internal static float Wcd, WcdEnd;
        internal static float Ecd, EcdEnd;
        internal static int LastCastTime;
        internal static bool CanShield;
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        internal static SpellSlot Ignite = SpellSlot.Unknown;
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