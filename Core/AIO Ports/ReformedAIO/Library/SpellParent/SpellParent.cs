using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Library.SpellParent
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal class SpellParent : ParentBase, ISpellIndex
    {
        public SpellParent()
        {
            this.Spells = new Dictionary<SpellSlot, SpellChild>();
        }

        public override string Name { get; set; } = "Spells";

        public Dictionary<SpellSlot, SpellChild> Spells { get; set; }

        public SpellChild this[SpellSlot spellSlot]
        {
            get
            {
                var spellChild = this.Spells[spellSlot];

                if (spellChild == null)
                {
                    throw new InvalidOperationException("Can't return a SpellChild for a SpellSlot that is non-existing");
                }

                return spellChild;
            }
        }

        protected override void OnChildAdd(object sender, ParentBaseEventArgs parentBaseEventArgs)
        {
            base.OnChildAdd(sender, parentBaseEventArgs);
        }

        protected override void SetSwitch()
        {
            this.Switch = new UnreversibleSwitch(this.Menu);
        }
    }
}
