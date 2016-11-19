using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Annie.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Disintegrate";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 625);
            Spell.SetTargetted(.25f, 1400f);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
