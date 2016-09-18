using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Vars
    {
        public static Orbwalking.Orbwalker Orbwalker { get; internal set; }

        public static AIHeroClient Player => ObjectManager.Player;
    }
}
