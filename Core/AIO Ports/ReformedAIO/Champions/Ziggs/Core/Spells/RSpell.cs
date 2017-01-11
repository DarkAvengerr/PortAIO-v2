using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    using Prediction = SPrediction.Prediction;
    using SPrediction;

    internal class RSpell : SpellChild
    {
        public override string Name { get; set; } = "Mega Inferno Bomb";

        public override Spell Spell { get; set; }

        public Vector3 Position;

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public float Delay(AIHeroClient target)
        {
            var delay = Spell.Delay = 3500 * target.Distance(ObjectManager.Player) / 5300;

            return delay;
        }

        public SebbyLib.Prediction.PredictionOutput OKTW(AIHeroClient target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = true,
                Collision = target.Distance(ObjectManager.Player) > 850,
                Speed = 1500,
                Delay = 1f,
                Range = 1700,
                From = ObjectManager.Player.ServerPosition,
                Radius = 550f,
                Unit = target,
                Type = SebbyLib.Prediction.SkillshotType.SkillshotCircle,
                Source = ObjectManager.Player,
                UseBoundingRadius = true,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            var output = SebbyLib.Prediction.Prediction.GetPrediction(input);

            return output;
        }

        public Prediction.AoeResult SPredictionOutput(AIHeroClient target)
        {
            return Spell.GetAoeSPrediction();
        }

        public PredictionOutput Prediction(Obj_AI_Base target)
        {
            Position = Spell.GetPrediction(target).CastPosition;

            return Spell.GetPrediction(target);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.R, 5300);
            Spell.SetSkillshot(1f, 550f, 1700, false, SkillshotType.SkillshotCircle);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
