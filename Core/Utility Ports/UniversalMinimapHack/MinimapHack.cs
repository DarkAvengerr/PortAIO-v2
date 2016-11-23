using System.Collections.Generic;
using System.Linq;
using LeagueSharp;

using EloBuddy;
using LeagueSharp.Common;
using System;

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
            foreach (AIHeroClient hero in EloBuddy.SDK.EntityManager.Heroes.Enemies)
            {
                _heroTrackers.Add(new HeroTracker(hero, ImageLoader.Load(hero.ChampionName)));
            }
        }
    }
}