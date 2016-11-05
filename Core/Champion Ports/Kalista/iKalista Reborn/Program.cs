using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy;
using LeagueSharp.Common;
namespace iKalistaReborn
{
    internal class Program
    {
        public static void Main()
        {
            if (ObjectManager.Player.ChampionName != "Kalista")
                return;

            new Kalista();
        }
    }
}