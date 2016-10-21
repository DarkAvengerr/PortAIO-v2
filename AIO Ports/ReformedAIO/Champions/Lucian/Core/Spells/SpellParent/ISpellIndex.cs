using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.Core.Spells.SpellParent
{
    using System.Collections.Generic;

    using LeagueSharp;

    using RethoughtLib.FeatureSystem.Implementations;

    internal interface ISpellIndex
    {
        Dictionary<SpellSlot, SpellChild> Spells { get; set; }

        SpellChild this[SpellSlot spellSlot] { get; }
    }
}
