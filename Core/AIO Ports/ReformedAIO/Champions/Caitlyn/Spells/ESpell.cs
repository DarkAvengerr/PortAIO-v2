using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal class ESpell : SpellChild
    {
        public override string Name { get; set; } = "90 Caliber Net";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return !Spell.IsReady() ? 0 : Spell.GetDamage(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Spell = new Spell(SpellSlot.E, 750);
            Spell.SetSkillshot(0.30f, 70f, 2000f, true, SkillshotType.SkillshotLine);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
