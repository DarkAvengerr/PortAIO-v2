using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;

namespace DZBard
{
    class BardBootstrap
    {
        internal static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Bard")
            {
                return;
            }

            Bard.BardMenu = new Menu("Bard - Dreamless Wanderer","dz191.bard", true);

            MenuGenerator.OnLoad(Bard.BardMenu);
            Bard.OnLoad();
        }
    }
}
