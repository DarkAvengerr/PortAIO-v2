using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal sealed class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Barrel Roll";

        public override Spell Spell { get; set; }

        public Vector3 LastCastPosition;

        public bool HasThrown;

        public void Handle(Vector3 LastPos)
        {
            Spell.Cast(LastPos);
            LastCastPosition = LastPos;
            HasThrown = true;

            LeagueSharp.Common.Utility.DelayAction.Add(5300, () => HasThrown = false);
        }

        public void ExplodeHandler(Obj_AI_Base target)
        {
            if (target.Distance(LastCastPosition) > 115f || !HasThrown)
            {
                return;
            }

            HasThrown = false;
            Spell.Cast();
        }

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
                Speed = 1000,
                Delay = .25f,
                Range = 850,
                From = ObjectManager.Player.ServerPosition,
                Radius = 110f,
                Unit = target,
                Type = SebbyLib.Prediction.SkillshotType.SkillshotCircle,
                Source = ObjectManager.Player,
                UseBoundingRadius = true,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            var output = SebbyLib.Prediction.Prediction.GetPrediction(input);

            return output;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 850);
            Spell.SetSkillshot(.25f, 110f, 1000, false, SkillshotType.SkillshotCircle);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
