using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Annie.Core.Spells
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal class WSpell : SpellChild
    {
        public override string Name { get; set; } = "Incinerate";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.W, 625);
            Spell.SetSkillshot(.6f, (float)(50f * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
