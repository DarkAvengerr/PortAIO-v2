using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.Core.Spells
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal sealed class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Light Binding";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public SebbyLib.Prediction.PredictionOutput Prediction(AIHeroClient target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = 1200,
                Delay = .25f,
                Range = 1175,
                Radius = 80f,
                UseBoundingRadius = true,
                RangeCheckFrom = ObjectManager.Player.ServerPosition,
                From = ObjectManager.Player.ServerPosition,
                Source = ObjectManager.Player,
                Unit = target,
                Type = SebbyLib.Prediction.SkillshotType.SkillshotLine,
                CollisionObjects = new[] { SebbyLib.Prediction.CollisionableObjects.YasuoWall }
            };

            return SebbyLib.Prediction.Prediction.GetPrediction(input);
        }

        public bool Collision(AIHeroClient target)
        {
            var commonPrediction = Spell.GetPrediction(target).CastPosition;

            var collision = Spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(), new List<Vector2>
                        {
                            commonPrediction.To2D()
                        });

            return collision.Count <= 1;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 1175);
            Spell.SetSkillshot(.25f, 80f, 1200, false, SkillshotType.SkillshotLine);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
