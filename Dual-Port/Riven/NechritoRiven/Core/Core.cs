using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Core
{
    internal partial class Core
    {
        public static AttackableUnit qTarget;

        public const string IsFirstR = "RivenFengShuiEngine";
        public const string IsSecondR = "RivenIzunaBlade";
       
        public static int Qstack = 1;

        public static Orbwalking.Orbwalker Orbwalker;
        public static AIHeroClient Player => ObjectManager.Player;
    }
}
