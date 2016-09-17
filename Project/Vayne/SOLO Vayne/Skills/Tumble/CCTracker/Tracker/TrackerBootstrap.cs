using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Skills.Tumble.CCTracker.Tracker
{
    class TrackerBootstrap
    {
        public Dictionary<AIHeroClient, TrackerModule> modules = new Dictionary<AIHeroClient, TrackerModule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerBootstrap"/> class.
        /// </summary>
        public TrackerBootstrap()
        {
            Init();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Init()
        {
            foreach (var champion in HeroManager.Enemies)
            {
                var module = new TrackerModule(champion);

                modules.Add(champion, module);
            }
        }

        /// <summary>
        /// Gets the module of the module by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TrackerModule GetModuleByName(string name)
        {
            foreach (var kvp in modules)
            {
                if (kvp.Key.ChampionName.ToLower() == name.ToLower())
                {
                    return kvp.Value;
                }
            }

            return new TrackerModule();
        }
    }
}
