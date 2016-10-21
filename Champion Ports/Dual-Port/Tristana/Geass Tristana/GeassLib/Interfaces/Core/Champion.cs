using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Interfaces.Core
{
    // ReSharper disable once InconsistentNaming
    public interface Champion
    {
        int GetManaPercent { get; }
        int HealthPercent { get; }
        Spell GetSpellE { get; set; }
        Spell GetSpellQ { get; set; }
        Spell GetSpellR { get; set; }
        Spell GetSpellW { get; set; }
        AIHeroClient Player { get; set; }
    }
}