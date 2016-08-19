#region

using LeagueSharp.Common;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ChewyMoonsShaco
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += ChewyMoonShaco.OnGameLoad;
        }
    }
}