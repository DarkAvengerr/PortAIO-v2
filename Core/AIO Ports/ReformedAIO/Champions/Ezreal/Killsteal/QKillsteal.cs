using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ezreal.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell qSpell;

        public QKillsteal(QSpell qSpell)
        {
            this.qSpell = qSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(qSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || Target.Health > qSpell.GetDamage(Target) || !CheckGuardians())
            {
                return;
            }

            var qPred = qSpell.Spell.GetPrediction(Target);

            if (qPred.Hitchance > HitChance.Medium)
            {
                qSpell.Spell.Cast(Target);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);
        }
    }
}
