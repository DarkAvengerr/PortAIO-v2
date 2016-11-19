using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal class RSpell : SpellChild
    {
        public override string Name { get; set; } = "Ace in the Hole";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return !Spell.IsReady() ? 0 : Spell.GetDamage(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.R, 3000);
            Spell.SetTargetted(0.7f, 200f);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
