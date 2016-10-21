using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KAPPALISTAXD.Core
{
    public static class KappalistaGlobals
    {
        public static AIHeroClient inGameChampion = ObjectManager.Player;
        public static AIHeroClient soulBound = null;
        public static Spell Q, W, E, R;
        public static Menu mainMenu;
        public static Orbwalking.Orbwalker orbwalkerInstance;
    }
}
