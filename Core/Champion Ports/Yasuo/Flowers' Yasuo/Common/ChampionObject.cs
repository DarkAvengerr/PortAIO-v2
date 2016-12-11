using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Common
{
    using LeagueSharp;

    internal class ChampionObject
    {
        public ChampionObject(AIHeroClient hero)
        {
            Hero = hero;
        }

        public AIHeroClient Hero { get; private set; }
        public float LastSeen { get; set; }
    }
}
