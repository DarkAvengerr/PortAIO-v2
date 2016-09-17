#region

using LeagueSharp.Common;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Sophies_Soraka
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += SophiesSoraka.OnGameLoad;
        }
    }
}