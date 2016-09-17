using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheEkko.ComboSystem
{
    /// <summary>
    /// The context needed by the ComboProvider, to access common data
    /// </summary>
    public interface IMainContext
    {
        Menu GetRootMenu();
        Orbwalking.Orbwalker GetOrbwalker();
    }
}
