using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ezreal.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell wSpell;

        public WKillsteal(WSpell wSpell)
        {
            this.wSpell = wSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(wSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || Target.Health > wSpell.GetDamage(Target) || !CheckGuardians())
            {
                return;
            }

            var qPred = wSpell.Spell.GetPrediction(Target, true);

            if (qPred.Hitchance > HitChance.Medium)
            {
                wSpell.Spell.Cast(Target);
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
