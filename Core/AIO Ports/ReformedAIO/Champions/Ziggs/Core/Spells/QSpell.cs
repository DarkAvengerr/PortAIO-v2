using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SPrediction;

    using SebbyLib;

    using PredictionOutput = LeagueSharp.Common.PredictionOutput;
    using SkillshotType = SebbyLib.Prediction.SkillshotType;

    using Geometry = LeagueSharp.Common.Geometry;
    using Prediction = SPrediction.Prediction;

    internal class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Bouncing Bomb";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public Geometry.Polygon.Rectangle Rectangle;

        public SebbyLib.Prediction.PredictionOutput OKTW(Obj_AI_Base target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = true,
                Collision = target.Distance(ObjectManager.Player) > 850,
                Speed = 1500,
                Delay = .4f,
                Range = 1400,
                From = ObjectManager.Player.ServerPosition,
                Radius = 125f,
                Unit = target,
                Type = SkillshotType.SkillshotCircle,
                Source = ObjectManager.Player,
                UseBoundingRadius = true,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            var output = SebbyLib.Prediction.Prediction.GetPrediction(input);

            return !OktwCommon.CollisionYasuo(ObjectManager.Player.Position, output.CastPosition) ? output : null;
        }

        public Prediction.Result SPredictionOutput(AIHeroClient target)
        {
            return Spell.GetSPrediction(target);
        }

        public PredictionOutput Prediction(Obj_AI_Base target)
        {
            bool AoE = target.Distance(ObjectManager.Player) <= 850;

            return Spell.GetPrediction(target, AoE);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 1400);
            Spell.SetSkillshot(.35f, 130f, 1700, false, LeagueSharp.Common.SkillshotType.SkillshotCircle);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
