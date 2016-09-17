using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElTalon
{
    internal class Program
    {
        internal static object _menu;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Talon.Game_OnGameLoad;
        }
    }
}