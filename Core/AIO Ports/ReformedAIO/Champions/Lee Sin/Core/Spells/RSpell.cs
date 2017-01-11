using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Library.Geometry.Insec;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal sealed class RSpell : SpellChild
    {
        public override string Name { get; set; } = "Dragon's Rage";

        public override Spell Spell { get; set; }

        public LeeSinHelper Insec;

        public SpellSlot Flash { get; set; }

        public Geometry.Polygon.Rectangle Rectangle;

        public Vector3 MultipleInsec(Obj_AI_Base insecTarget, Obj_AI_Base otherTargets)
        {
            var pred = Prediction(otherTargets).CastPosition;

            var pos = insecTarget.Position + (insecTarget.Position - pred).Normalized() * 233;

            return pos;
        }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public SebbyLib.Movement.PredictionOutput Prediction(Obj_AI_Base target)
        {
            return SebbyLib.Movement.Prediction.GetPrediction(PredictionInput(target));
        }

        private SebbyLib.Movement.PredictionInput PredictionInput(Obj_AI_Base target)
        {
            var input = new SebbyLib.Movement.PredictionInput
            {
                Aoe = true,
                UseBoundingRadius = true,
                Collision = true,
                Speed = 1500,
                Delay = .25f,
                Range = 1200,
                Radius = 100,
                Unit = target,
                From = ObjectManager.Player.Position,
                RangeCheckFrom = ObjectManager.Player.Position,
                Source = ObjectManager.Player,
                Type = SebbyLib.Movement.SkillshotType.SkillshotLine,
            };

            return input;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.R, 375); // Cast range 375, knockback 1200
            Spell.SetSkillshot(.25f, 100f, 1500, false, SkillshotType.SkillshotLine);

            Flash = ObjectManager.Player.GetSpellSlot("SummonerFlash");

            Insec = new LeeSinHelper();
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
