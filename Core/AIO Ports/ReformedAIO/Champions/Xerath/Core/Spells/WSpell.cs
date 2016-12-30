using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SebbyLib;

    using HitChance = SebbyLib.Prediction.HitChance;
    using SkillshotType = SebbyLib.Prediction.SkillshotType;

    internal sealed class WSpell : SpellChild
    {
        public override string Name { get; set; } = "SampleText";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public SebbyLib.Prediction.PredictionOutput OKTW(Obj_AI_Base target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = 1300,
                Delay = .3f,
                Range = 1100,
                From = ObjectManager.Player.ServerPosition,
                Radius = 200f,
                Unit = target,
                Type = SkillshotType.SkillshotCircle
            };

            var output = SebbyLib.Prediction.Prediction.GetPrediction(input);

            if (!OktwCommon.CollisionYasuo(ObjectManager.Player.Position, output.CastPosition) && output.Hitchance >= HitChance.High)
            {
                return output;
            }
            return null;
        }

        public SebbyLib.Movement.PredictionOutput SDK(Obj_AI_Base target)
        {
            var input = new SebbyLib.Movement.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = 1300,
                Delay = .3f,
                Range = 1100,
                From = ObjectManager.Player.ServerPosition,
                Radius = 200f,
                Unit = target,
                Type = SebbyLib.Movement.SkillshotType.SkillshotCircle
            };

            var output = SebbyLib.Movement.Prediction.GetPrediction(input);

            if (!OktwCommon.CollisionYasuo(ObjectManager.Player.Position, output.CastPosition) && output.Hitchance >= SebbyLib.Movement.HitChance.High)
            {
                return output;
            }
            return null;
        }

        public PredictionOutput Prediction(Obj_AI_Base target)
        {
            return Spell.GetPrediction(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.W, 1100);
            Spell.SetSkillshot(.5f, 200f, 1400, false, (LeagueSharp.Common.SkillshotType)SkillshotType.SkillshotCircle);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
