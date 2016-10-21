using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Interfaces.Core
{
    public interface Menu
    {
        LeagueSharp.Common.Menu GetMenu();

        void Load();
    }
}