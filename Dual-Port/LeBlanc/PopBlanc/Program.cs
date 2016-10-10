using LeagueSharp;
using LeagueSharp.Common;
using TreeLib.Core;

using EloBuddy;
using LeagueSharp.Common;
namespace PopBlanc
{
    internal class Program
    {
        public static void Main()
        {
            if (ObjectManager.Player.ChampionName.Equals("Leblanc"))
            {
                Bootstrap.Initialize();
                var s = new LeBlanc();
            }
        }
    }
}