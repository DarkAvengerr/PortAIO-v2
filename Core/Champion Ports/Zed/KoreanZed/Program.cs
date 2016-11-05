using EloBuddy; namespace KoreanZed
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() == "zed")
            {
                var ZedZeppelin = new Zed();
            }
        }
    }
}
