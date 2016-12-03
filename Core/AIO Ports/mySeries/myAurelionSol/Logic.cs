using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol
{
    using Manager.Events;
    using Manager.Menu;
    using Manager.Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class Logic
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;
        internal static MissileClient qMillile;
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