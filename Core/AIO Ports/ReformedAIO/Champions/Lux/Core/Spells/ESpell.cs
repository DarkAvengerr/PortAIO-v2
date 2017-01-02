using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.Core.Spells
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal sealed class ESpell : SpellChild
    {
        public override string Name { get; set; } = "Lucent Singularity";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        private GameObject gameObject;

        public bool IsActive => gameObject != null;

        public bool InRange(Obj_AI_Base target)
        {
            return target.Distance(gameObject.Position) < Spell.Width;
        }

        public SebbyLib.Movement.PredictionOutput Prediction(Obj_AI_Base target)
        {
            var input = new SebbyLib.Movement.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = Spell.Speed,
                Delay = Spell.Delay,
                Range = Spell.Range,
                Radius = Spell.Width,
                From = ObjectManager.Player.ServerPosition,
                Unit = target,
                Type = SebbyLib.Movement.SkillshotType.SkillshotCircle,
                Source = ObjectManager.Player,
                UseBoundingRadius = true,
                RangeCheckFrom = ObjectManager.Player.Position
            };

            var output = SebbyLib.Movement.Prediction.GetPrediction(input);

            return output;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.E, 1100);
            Spell.SetSkillshot(.4f, 270f, 1300, false, SkillshotType.SkillshotCircle);

            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private void OnDelete(GameObject sender, EventArgs args)
        {
            if (sender == null 
                || !sender.IsMe 
                || !sender.Name.Contains("LuxLightstrike_tar_"))
            {
                return;
            }

            gameObject = null;
        }

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("LuxLightstrike_tar_"))
            {
                return;
            }

            gameObject = sender;
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
