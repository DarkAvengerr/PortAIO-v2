using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.SpellParent
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    #endregion

    internal sealed class SpellParent : ParentBase, ISpellIndex
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellParent" /> class.
        /// </summary>
        public SpellParent()
        {
            this.Spells = new Dictionary<SpellSlot, SpellChild>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Spells";

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public Dictionary<SpellSlot, SpellChild> Spells { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets or sets the <see cref="SpellChild" /> with the specified spell slot.
        /// </summary>
        /// <value>
        ///     The <see cref="SpellChild" />.
        /// </value>
        /// <param name="spellSlot">The spell slot.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Can't return a SpellChild for a SpellSlot that is non-existing</exception>
        public SpellChild this[SpellSlot spellSlot]
        {
            get
            {
                var spellChild = this.Spells[spellSlot];

                if (spellChild == null)
                {
                    throw new InvalidOperationException(
                        "Can't return a SpellChild for a SpellSlot that is non-existing");
                }

                return spellChild;
            }
        }

        #endregion

        #region Explicit Interface Indexers

        SpellChild ISpellIndex.this[SpellSlot spellSlot] => this.Spells[spellSlot];

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [child add].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="parentBaseEventArgs">The <see cref="ParentBase.ParentBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnChildAdd(object sender, ParentBaseEventArgs parentBaseEventArgs)
        {
            base.OnChildAdd(sender, parentBaseEventArgs);
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            foreach (var spell in this.Children.OfType<SpellChild>())
            {
                if (spell.Spell == null) return;

                this.Spells[spell.Spell.Slot] = spell;
            }
        }

        /// <summary>
        ///     Sets the switch.
        /// </summary>
        protected override void SetSwitch()
        {
            this.Switch = new UnreversibleSwitch(this.Menu);
        }

        #endregion
    }
}