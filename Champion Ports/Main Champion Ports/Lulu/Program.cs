using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using TreeLib.Core;

namespace LuluLicious
{
    internal class Program
    {
        public static void Main()
        {
            if (ObjectManager.Player.ChampionName == "Lulu")
            {
                Bootstrap.Initialize();
                var s = new Lulu();
            }
        }
    }
}