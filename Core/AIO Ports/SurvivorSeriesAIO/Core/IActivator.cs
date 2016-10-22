using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public interface IActivator
    {
        Items.Item Biscuit { get; }
        Items.Item Flask { get; }
        Items.Item FlaskCorruptJG { get; }
        Items.Item FlaskHunterJG { get; }
        Items.Item FlaskRef { get; }
        Items.Item GLP800 { get; }
        Items.Item Hextech { get; }
        Items.Item HPPotion { get; }
        SpellSlot IgniteSpell { get; }
        Items.Item Protobelt { get; }
        Items.Item Seraph { get; }
        object SpellSlot { get; }
        Spell SummonerDot { get; }
    }
}