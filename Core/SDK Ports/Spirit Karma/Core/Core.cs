#region

using LeagueSharp;
using LeagueSharp.SDK;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Spirit_Karma.Core
{
    internal class Core
    {
        public static Orbwalker Orbwalker => Variables.Orbwalker;
        public static AIHeroClient Player => ObjectManager.Player;
        public static AIHeroClient Target => Variables.TargetSelector.GetTarget(1050, DamageType.Magical);
    }
}
