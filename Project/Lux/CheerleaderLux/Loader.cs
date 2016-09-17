using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CheerleaderLux
{
    class Loader : Lux
    {
        static void Main(string[] args)
        {          
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
    }
}
