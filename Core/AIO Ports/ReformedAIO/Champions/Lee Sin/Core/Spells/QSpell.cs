using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Core.Spells
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal sealed class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Sonic Wave";

        public override Spell Spell { get; set; }

        public SpellSlot Smite { get; set; }

        //public SpellSlot SmitePassive { get; set; }

        public double Q2Damage(Obj_AI_Base target)
        {
            return ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q, 1);
        }
      
        public int SmiteTargetableDamage()
        {
            if (SmiteTargetable() == "s5_summonersmiteplayerganker")
            {
                return 60 + 6 * ObjectManager.Player.Level;
            }
            return 20 + 8 * ObjectManager.Player.Level;
        }

        // Got this from Hoes, props to him! 
        public int SmiteMonsters()
        {
            var level = ObjectManager.Player.Level;

            var index = level / 5;

            int[] damage = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };

            return damage[index];
        }

        public void SmiteCollision(AIHeroClient target)
        {
            var collision = Spell.GetCollision(ObjectManager.Player.ServerPosition.To2D(),
                                               new List<Vector2> { Prediction(target).CastPosition.To2D() });

            var col = collision.FirstOrDefault();

            if (collision.Count == 1
                && col != null
                && col.IsMinion
                && col.Distance(ObjectManager.Player) < 500 
                && col.Health < SmiteMonsters())
            {
                Spell.Cast(Prediction(target).CastPosition);
                ObjectManager.Player.Spellbook.CastSpell(Smite, col);
            }
        }

        public bool IsQ1 => Spell.Instance.Name == "BlindMonkQOne";

        public bool HasQ2(Obj_AI_Base target)
        {
            return target.HasBuff("BlindMonkSonicWave");
        }

        public bool ShouldQ2(Obj_AI_Base target)
        {
            return target.GetBuff("BlindMonkQOne").EndTime - Game.Time <= .3f;
        }

        public bool Passive => ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic");

        public int PassiveStacks => ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic")
                                         ? ObjectManager.Player.GetBuff("blindmonkpassive_cosmetic").Count
                                         : 0;

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public SebbyLib.Movement.PredictionOutput Prediction(Obj_AI_Base target)
        {
            return SebbyLib.Movement.Prediction.GetPrediction(PredictionInput(target));
        }

        private static SebbyLib.Movement.PredictionInput PredictionInput(Obj_AI_Base target)
        {
            var input = new SebbyLib.Movement.PredictionInput
            {
                Aoe = false,
                UseBoundingRadius = true,
                Collision = true,
                Speed = 1800,
                Delay = .25f,
                Range = 1000,
                Radius = 60f,
                Unit = target,
                From = ObjectManager.Player.ServerPosition,
                Source = ObjectManager.Player,
                RangeCheckFrom = ObjectManager.Player.Position,
                Type = SebbyLib.Movement.SkillshotType.SkillshotLine,
              //  CollisionObjects = new [] { CollisionableObjects.YasuoWall, CollisionableObjects.Minions,  }
            };

            return input;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 1000);
            Spell.SetSkillshot(.25f, 60f, 1800, true, SkillshotType.SkillshotLine);

            Smite = ObjectManager.Player.GetSpellSlot(SmiteTargetable());

            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Obj_AI_Base.OnBuffGain += OnBuffAdd;
        }

        public bool Dashing;

        private void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if(!sender.IsMe) return;

            if (args.Buff.DisplayName == "BlindMonkQTwoDash")
            {
                Dashing = true;
            }
        }

        private void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.Buff.DisplayName == "BlindMonkQTwoDash")
            {
                Dashing = false;
            }
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }

        private static string SmiteTargetable()
        {
            if (SmiteBlue.Any(x => Items.HasItem(x)))
            {
                return "s5_summonersmiteplayerganker";
            }
            return SmiteRed.Any(x => Items.HasItem(x)) ? "s5_summonersmiteduel" : "summonersmite";
        }

        // Credits to xQx for this.
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
    }
}
