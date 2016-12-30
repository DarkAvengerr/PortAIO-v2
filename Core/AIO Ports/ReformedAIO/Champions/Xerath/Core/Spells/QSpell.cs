using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SebbyLib;
    using SebbyLib.Movement;

    using HitChance = SebbyLib.Prediction.HitChance;
    using PredictionOutput = LeagueSharp.Common.PredictionOutput;
    using SkillshotType = SebbyLib.Prediction.SkillshotType;

    internal sealed class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Arcanopulse";

        public override Spell Spell { get; set; }

        public bool Charging => Spell.IsCharging;

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
                Speed = float.MaxValue,
                Delay = .6f,
                Range = 1400,
                From = ObjectManager.Player.ServerPosition,
                Radius = 150f,
                Unit = target,
                Type = SkillshotType.SkillshotLine,
                Source = ObjectManager.Player,
                UseBoundingRadius = false,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            var output = SebbyLib.Prediction.Prediction.GetPrediction(input);

            if (!OktwCommon.CollisionYasuo(ObjectManager.Player.Position, output.CastPosition) && output.Hitchance >= HitChance.High && Spell.IsCharging)
            {
                return output;
            }
            return null;
        }

        /// <summary>
        /// SDK??
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public SebbyLib.Movement.PredictionOutput SDK(Obj_AI_Base target)
        {
            var input = new SebbyLib.Movement.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = float.MaxValue,
                Delay = .6f,
                Range = 1400,
                From = ObjectManager.Player.ServerPosition,
                Radius = 150f,
                Unit = target,
                Type = SebbyLib.Movement.SkillshotType.SkillshotLine,
                Source = ObjectManager.Player,
                UseBoundingRadius = false,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            var output = SebbyLib.Movement.Prediction.GetPrediction(input);

            if (!OktwCommon.CollisionYasuo(ObjectManager.Player.Position, output.CastPosition) && output.Hitchance >= SebbyLib.Movement.HitChance.High && Spell.IsCharging)
            {
                return output;
            }
            return null;
        }

        /// <summary>
        /// Common Prediction
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public PredictionOutput Prediction(Obj_AI_Base target)
        {
            return Spell.GetPrediction(target);
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 1400);
            Spell.SetCharged(600, 1400, 1.5f);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
