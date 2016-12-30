using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SebbyLib;

    using SkillshotType = SebbyLib.Prediction.SkillshotType;

    internal sealed class RSpell : SpellChild
    {
        public override string Name { get; set; } = "SampleText";

        public override Spell Spell { get; set; }

        public bool IsCasting => ObjectManager.Player.HasBuff("XerathLocusOfPower2");

        public int RealRange => Spell.Level * 1300 + 2000;

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target) * 3;
        }

        public SebbyLib.Prediction.PredictionOutput OKTW(Obj_AI_Base target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = float.MaxValue,
                Delay = .7f,
                Range = RealRange,
                From = ObjectManager.Player.ServerPosition,
                Radius = 200f,
                Unit = target,
                Type = SkillshotType.SkillshotCircle
            };

            var output = SebbyLib.Prediction.Prediction.GetPrediction(input);

            if (!OktwCommon.CollisionYasuo(ObjectManager.Player.Position, output.CastPosition) && output.Hitchance >= SebbyLib.Prediction.HitChance.High)
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
                Speed = float.MaxValue,
                Delay = .7f,
                Range = RealRange,
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

            Spell = new Spell(SpellSlot.R, 3520);
            Spell.SetSkillshot(.6f, 200f, float.MaxValue, false, (LeagueSharp.Common.SkillshotType)SkillshotType.SkillshotCircle);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
