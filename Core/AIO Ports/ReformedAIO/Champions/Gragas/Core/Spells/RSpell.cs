using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.Core.Spells
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal sealed class RSpell : SpellChild
    {
        public override string Name { get; set; } = "SampleText";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public Vector3 InsecPositioner(Obj_AI_Base target, bool turret = false, bool ally = false)
        {
            var allies = Ally;
            var allyTurret = AllyTurret;

            if (turret && allyTurret != null)
            {
                return InsecPosition(target, allyTurret.Position);
            }

            if (ally && allies != null)
            {
                return InsecPosition(target, allies.Position);
            }
            return InsecPosition(target, Game.CursorPos, true);
        }

        public Vector3 InsecPosition(Obj_AI_Base target, Vector3 Object, bool reverted = false)
        {
            var amount = 260;

            var revertedPos = target.Position + (Object - target.Position).Normalized() * amount;

            var pos = target.Position + (target.Position - Object).Normalized() * amount;

            return reverted ? revertedPos : pos;
        }

        public Vector3 CastPosition(Obj_AI_Base target)
        {
            return InsecPosition(target, ObjectManager.Player.Position);
        }

        public SebbyLib.Prediction.PredictionOutput OKTW(Obj_AI_Base target)
        {
            var input = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = 1000,
                Delay = .3f,
                Range = 950,
                From = ObjectManager.Player.ServerPosition,
                Radius = 400f, // Knockback radius = 600f
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

            Spell = new Spell(SpellSlot.R, 1000);
            Spell.SetSkillshot(.3f, 400f, 1000, false, SkillshotType.SkillshotCircle);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }

        private static AIHeroClient Ally
          => HeroManager.Allies.Where(x => x.IsAlly && !x.IsMe && !x.IsMinion)
              .FirstOrDefault(x => x.Distance(ObjectManager.Player) < 1000);

        private static Obj_AI_Turret AllyTurret
            => ObjectManager.Get<Obj_AI_Turret>()
                .FirstOrDefault(x => x.Distance(ObjectManager.Player) < 600 && x.IsAlly && !x.IsDead);
    }
}
