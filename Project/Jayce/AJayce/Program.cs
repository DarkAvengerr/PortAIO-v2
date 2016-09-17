using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace AJayce
{
    class Program 
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Jayce.OnLoad;
        }
    }
}
