namespace ReformedAIO.Champions.Lucian.Spells.SpellParent
{
    using System.Collections.Generic;

    using EloBuddy;

    using RethoughtLib.FeatureSystem.Implementations;

    internal interface ISpellIndex
    {
        Dictionary<SpellSlot, SpellChild> Spells { get; set; }

        SpellChild this[SpellSlot spellSlot] { get; }
    }
}
