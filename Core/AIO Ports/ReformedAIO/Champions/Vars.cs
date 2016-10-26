namespace ReformedAIO.Champions
{
    using EloBuddy;
    using LeagueSharp.Common;

    internal class Vars
    {
        public static Orbwalking.Orbwalker Orbwalker { get; internal set; }

        public static AIHeroClient Player => ObjectManager.Player;
    }
}
