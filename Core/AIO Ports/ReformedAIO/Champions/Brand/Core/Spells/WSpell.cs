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

    internal class WSpell : SpellChild
    {
        public override string Name { get; set; } = "SampleText";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
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

            Spell = new Spell(SpellSlot.W, 900);
            Spell.SetSkillshot(.1f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
