using LeagueSharp;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Nocturne
{
    internal class Program
    {
        public static string ChampionName => "Einstein Exory";
        public static void Main()
        {
            if (ObjectManager.Player.ChampionName == "Nocturne")
            {
                Nocturne.Init();
            }
        }
    }
}
