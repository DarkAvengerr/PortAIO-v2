using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal sealed class ESpell : SpellChild
    {
        public override string Name { get; set; } = "Body Slam";

        public override Spell Spell { get; set; }

        public SpellSlot Flash { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        private SebbyLib.Prediction.PredictionInput OKTWInput(Obj_AI_Base target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = false,
                Collision = true,
                Speed = 900,
                Delay = .25f,
                Range = 600,
                From = ObjectManager.Player.ServerPosition,
                Radius = 50f,
                Unit = target,
                Type = SebbyLib.Prediction.SkillshotType.SkillshotLine,
                Source = ObjectManager.Player,
                UseBoundingRadius = true,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            return input;
        }

        public SebbyLib.Prediction.PredictionOutput OKTW(Obj_AI_Base target)
        {
            return SebbyLib.Prediction.Prediction.GetPrediction(OKTWInput(target));
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.E, 600);
            Spell.SetSkillshot(.25f, 50f, 900, true, SkillshotType.SkillshotLine);

            Flash = ObjectManager.Player.GetSpellSlot("SummonerFlash");
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
