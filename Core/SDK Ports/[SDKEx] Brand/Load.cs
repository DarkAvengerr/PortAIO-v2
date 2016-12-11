using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand
{
    using System;
    using LeagueSharp.SDK;

    using static Extensions.Config;

    class Load
    {
        public static void Main()
        {
            Bootstrap.Init(null);
            OnLoad();
        }

        private static void OnLoad()
        {
            if (MyHero.ChampionName == "Brand")
            {
                Extensions.Events.Initialize();
                Extensions.Config.Initialize();
                Extensions.Spells.Initialize();
            }
        }
    }
}
