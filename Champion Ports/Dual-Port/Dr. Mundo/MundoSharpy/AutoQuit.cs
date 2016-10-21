using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Mundo_Sharpy
{
    class AutoQuit
    {
        internal static void Load()
        {
            MenuProvider.MenuInstance.AddItem(new MenuItem("QuitTheGameAfterGameOver", "Auto Quit the game after game over")).SetValue(false);
            Game.OnEnd += Game_OnEnd;
        }

        private static void Game_OnEnd(GameEndEventArgs args)
        {
            if (MenuProvider.MenuInstance.Item("QuitTheGameAfterGameOver").GetValue<bool>())
            {
                System.Threading.Tasks.Task.Run(
                async () =>
                {
                    await System.Threading.Tasks.Task.Delay(5000);
                    Game.QuitGame();
                });
            }
        }
    }
}
