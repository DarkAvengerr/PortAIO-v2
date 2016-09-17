using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace EasyPeasyRivenSqueezy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Riven.OnGameLoad;
        }
    }
}