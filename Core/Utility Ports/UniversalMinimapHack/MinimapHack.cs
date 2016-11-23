using System.Collections.Generic;
using System.Linq;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace UniversalMinimapHack
{
    public class MinimapHack
    {
        private static readonly MinimapHack MinimapHackInstance = new MinimapHack();

        private readonly IList<HeroTracker> _heroTrackers = new List<HeroTracker>();

        public Menu Menu { get; private set; }

        public static MinimapHack Instance()
        {
            return MinimapHackInstance;
        }

        public void Load()
        {
            Menu = new Menu();
            foreach (AIHeroClient hero in
                ObjectManager.Get<AIHeroClient>().Where(hero => hero.Team != ObjectManager.Player.Team))
            {
                _heroTrackers.Add(new HeroTracker(hero, ImageLoader.Load(hero.ChampionName)));
            }
        }
    }
}