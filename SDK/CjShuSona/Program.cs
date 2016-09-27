using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace CjShuSona
{
    class Program
    {
        public static void Main()
        {
            LeagueSharp.SDK.Bootstrap.Init(null);
            Sona.OnLoad();
        }
    }
}