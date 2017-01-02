using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal sealed class RSpell : SpellChild
    {
        public override string Name { get; set; } = "Final Spark";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            float dmg = target.HasBuff("luxilluminatingfraulein")
                          ? (float)ObjectManager.Player.GetAutoAttackDamage(target, true)
                          : 0f;

            var damage = Spell.GetDamage(target) + dmg;

            return damage;
        }

        private static SebbyLib.Movement.PredictionInput PredictionOutput(Obj_AI_Base target)
        {
            var input = new SebbyLib.Movement.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = float.MaxValue,
                Delay = 1f,
                Range = 3340,
                Radius = 190f,
                UseBoundingRadius = false,
                RangeCheckFrom = ObjectManager.Player.ServerPosition,
                From = ObjectManager.Player.ServerPosition,
                Source = ObjectManager.Player,
                Unit = target,
                Type = SebbyLib.Movement.SkillshotType.SkillshotLine,
            };

            return input;
        }

        public SebbyLib.Movement.PredictionOutput Prediction(Obj_AI_Base target)
        {
            return SebbyLib.Movement.Prediction.GetPrediction(PredictionOutput(target));
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.R, 3340);
            Spell.SetSkillshot(1f, 190f, float.MaxValue, false, SkillshotType.SkillshotLine);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
