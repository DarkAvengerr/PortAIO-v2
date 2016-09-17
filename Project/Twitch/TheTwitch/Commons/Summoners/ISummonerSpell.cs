using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch.Commons.Summoners
{
    public interface ISummonerSpell
    {
        void Initialize(Menu menu);
        void Update();
        string GetDisplayName();
        bool IsAvailable();
    }
}
