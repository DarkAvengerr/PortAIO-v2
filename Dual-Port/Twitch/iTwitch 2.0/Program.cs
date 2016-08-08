using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iTwitch
{
    internal class Program
    {
        public static void Main()
        {
            var twitch = new Twitch();
            twitch.OnGameLoad();
        }
    }
}