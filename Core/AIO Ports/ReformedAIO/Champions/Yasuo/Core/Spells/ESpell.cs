using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Yasuo.Core.Spells
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal class ESpell : SpellChild
    {
        public override string Name { get; set; } = "Sweeping Blade";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.E, 475);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
