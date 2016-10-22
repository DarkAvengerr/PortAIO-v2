using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DicasteAshe
{
    using DicasteAshe.Handlers;

    using LeagueSharp;

    using static LeagueSharp.SDK.GameObjects;

    internal static class Core
    {
        private static string ChampionName { get; } = Player.ChampionName;

        internal static void Init()
        {
            SpellHandler.Init();

            MenuHandler.Init();

            DrawingHandler.Init();

            ModeHandler.Init();

            Chat.Print($"{ChampionName} Loaded!");
        }
    }
}