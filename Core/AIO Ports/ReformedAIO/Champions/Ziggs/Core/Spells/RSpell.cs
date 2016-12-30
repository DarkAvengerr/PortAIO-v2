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

        public Prediction.Result SPredictionOutput(AIHeroClient target)
        {
            Position = Spell.GetPrediction(target).CastPosition;

            return Spell.GetSPrediction(target);
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
            Spell.SetSkillshot(.1f, 550f, 1700, false, SkillshotType.SkillshotCircle);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
