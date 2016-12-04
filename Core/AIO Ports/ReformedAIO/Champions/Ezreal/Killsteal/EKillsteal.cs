using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ezreal.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    /// <summary>
    /// Alot can be done with E regarding killstealing...
    /// </summary>
    internal sealed class EKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell eSpell;

        public EKillsteal(ESpell eSpell)
        {
            this.eSpell = eSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(eSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || Target.Health > eSpell.GetDamage(Target) || !CheckGuardians())
            {
                return;
            }

            var qPred = eSpell.Spell.GetPrediction(Target);

            if (qPred.Hitchance > HitChance.Medium)
            {
                eSpell.Spell.Cast(Target);
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
