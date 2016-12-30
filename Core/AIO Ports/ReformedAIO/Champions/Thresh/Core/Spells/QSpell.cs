using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Thresh.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SebbyLib;

    using HitChance = SebbyLib.Prediction.HitChance;
    using PredictionOutput = LeagueSharp.Common.PredictionOutput;
    using SkillshotType = SebbyLib.Prediction.SkillshotType;

    internal sealed class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Death Sentence";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public SebbyLib.Prediction.PredictionOutput OKTW(Obj_AI_Base target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = false,
                Collision = true,
                Speed = 1500,
                Delay = .6f,
                Range = 1100,
                From = ObjectManager.Player.ServerPosition,
                Radius = 60f,
                Unit = target,
                Type = SkillshotType.SkillshotLine,
                Source = ObjectManager.Player,
                UseBoundingRadius = true,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            var output = SebbyLib.Prediction.Prediction.GetPrediction(input);

            return !OktwCommon.CollisionYasuo(ObjectManager.Player.Position, output.CastPosition) ? output : null;
        }

        public SebbyLib.Movement.PredictionOutput SDK(Obj_AI_Base target)
        {
            var input = new SebbyLib.Movement.PredictionInput
            {
                Aoe = false,
                Collision = true,
                Speed = 1500,
                Delay = .6f,
                Range = 1100,
                From = ObjectManager.Player.ServerPosition,
                Radius = 60f,
                Unit = target,
                Type = SebbyLib.Movement.SkillshotType.SkillshotLine,
                Source = ObjectManager.Player,
                UseBoundingRadius = true,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            var output = SebbyLib.Movement.Prediction.GetPrediction(input);

            return !OktwCommon.CollisionYasuo(ObjectManager.Player.Position, output.CastPosition) ? output : null;
        }

        public PredictionOutput Prediction(Obj_AI_Base target)
        {
            return Spell.GetPrediction(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 1100);
            Spell.SetSkillshot(.5f, 60f, 1500, true, LeagueSharp.Common.SkillshotType.SkillshotLine);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
