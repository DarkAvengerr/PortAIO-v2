using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RQQKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "RQQ Combo";

        private readonly QSpell qSpell;

        private readonly RSpell rSpell;

        public RQQKillsteal(QSpell qSpell, RSpell rSpell)
        {
            this.qSpell = qSpell;
            this.rSpell = rSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(qSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Target.Health > qSpell.GetDamage(Target) + qSpell.Q2Damage(Target) + rSpell.GetDamage(Target))
            {
                return;
            }

            rSpell.Spell.CastOnUnit(Target);

            if (!qSpell.HasQ2(Target))
            {
                qSpell.Spell.Cast(qSpell.Prediction(Target).CastPosition);
            }
            else
            {
                qSpell.Spell.Cast();
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);
        }
    }
}
