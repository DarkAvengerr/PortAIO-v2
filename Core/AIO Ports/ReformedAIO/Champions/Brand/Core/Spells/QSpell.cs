using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Brand.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using Prediction = SPrediction.Prediction;
    using SPrediction;

    internal class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Sear";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public bool Stunnable(Obj_AI_Base target)
        {
            return target.HasBuff("brandablaze");
        }

        public Prediction.Result SPredictionOutput(AIHeroClient target)
        {
            return Spell.GetSPrediction(target);
        }

        public PredictionOutput Prediction(Obj_AI_Base target)
        {
            return Spell.GetPrediction(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 1050);
            Spell.SetSkillshot(.25f, 60f, 1600, true, SkillshotType.SkillshotLine);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
