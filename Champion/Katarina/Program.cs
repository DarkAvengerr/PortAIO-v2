using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using TreeLib.Core;

namespace Staberina
{
    internal class Program
    {
        public static void Main()
        {
            if (ObjectManager.Player.ChampionName == "Katarina")
            {
                Bootstrap.Initialize();
                var s = new Katarina();
            }
        }
    }
}