using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lucian.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class W : OrbwalkingChild
    {
        public override string Name { get; set; } = nameof(W); 

        private readonly WSpell wSpell;

        public W(WSpell wSpell)
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

            if (Target.Distance(ObjectManager.Player) <= wSpell.Spell.Range)
            {
                var wPred = wSpell.Spell.GetPrediction(Target, true);

                if (wPred.Hitchance > HitChance.Medium)
                {
                    wSpell.Spell.Cast(Target);
                }
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
