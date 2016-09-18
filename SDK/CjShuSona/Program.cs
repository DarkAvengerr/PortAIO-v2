using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace CjShuSona
{
    class Program
    {
        static void Main(string[] args)
        {
            LeagueSharp.SDK.Bootstrap.Init(args);
            LeagueSharp.SDK.Events.OnLoad += Sona.OnLoad;                    
        }
    }
}