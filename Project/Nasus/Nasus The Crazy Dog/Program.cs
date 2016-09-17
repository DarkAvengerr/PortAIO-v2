using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nasus{

    using LeagueSharp.Common;

    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += LNasus.OnLoad;
        }
    }
}