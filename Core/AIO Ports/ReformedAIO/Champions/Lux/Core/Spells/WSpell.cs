using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal sealed class WSpell : SpellChild
    {
        public override string Name { get; set; } = "SampleText";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        private static SebbyLib.Prediction.PredictionInput PredictionInput(Obj_AI_Base target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = 1400,
                Delay = .25f,
                Range = 1075,
                Radius = 150f,
                UseBoundingRadius = false,
                RangeCheckFrom = ObjectManager.Player.ServerPosition,
                From = ObjectManager.Player.ServerPosition,
                Source = ObjectManager.Player,
                Unit = target,
                Type = SebbyLib.Prediction.SkillshotType.SkillshotLine,
            };
            return input;
        }

        public SebbyLib.Prediction.PredictionOutput Prediction(Obj_AI_Base target)
        {
            return SebbyLib.Prediction.Prediction.GetPrediction(PredictionInput(target));
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.W, 1075);
            Spell.SetSkillshot(.25f, 150f, 1400, false, SkillshotType.SkillshotLine);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
