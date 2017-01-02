using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal sealed class RSpell : SpellChild
    {
        public override string Name { get; set; } = "SampleText";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public Vector3 CastPosition(Obj_AI_Base target)
        {
            return target.Position.Extend(OKTW(target).CastPosition, 140);
        }

        public SebbyLib.Prediction.PredictionOutput OKTW(Obj_AI_Base target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = 1000,
                Delay = .3f,
                Range = 950,
                From = ObjectManager.Player.ServerPosition,
                Radius = 400f, // Knockback radius = 600f
                Unit = target,
                Type = SebbyLib.Prediction.SkillshotType.SkillshotCircle,
                Source = ObjectManager.Player,
                UseBoundingRadius = true,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            var output = SebbyLib.Prediction.Prediction.GetPrediction(input);

            return output;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.R, 1000);
            Spell.SetSkillshot(.3f, 400f, 1000, false, SkillshotType.SkillshotCircle);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
