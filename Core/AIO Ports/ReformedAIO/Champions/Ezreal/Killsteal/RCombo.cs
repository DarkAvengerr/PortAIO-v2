using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ezreal.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell rSpell;

        public RKillsteal(RSpell rSpell)
        {
            this.rSpell = rSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(rSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || Target.Health > rSpell.GetDamage(Target) || !CheckGuardians() || Target.Distance(ObjectManager.Player) <= ObjectManager.Player.AttackRange + 100)
            {
                return;
            }

            var qPred = rSpell.Spell.GetPrediction(Target, true);

            if (qPred.Hitchance >= HitChance.High)
            {
                rSpell.Spell.Cast(Target);
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
