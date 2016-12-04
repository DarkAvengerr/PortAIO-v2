using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Caitlyn.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK.Utils;

    using ReformedAIO.Champions.Caitlyn.Logic;
    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        public readonly RSpell rSpell;

        public RKillsteal(RSpell rSpell)
        {
            this.rSpell = rSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(rSpell.Spell.Range, TargetSelector.DamageType.Physical);

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

        private void OnUpdate(EventArgs args)
        {
            if (rSpell.Spell.Level <= 1)
            {
                rSpell.Spell.Range = 2000;
            }
            else
            {
                rSpell.Spell.Range = 1500 + 500 * rSpell.Spell.Level;
            }

            if (Target == null
                || ObjectManager.Player.Distance(Target) < ObjectManager.Player.GetRealAutoAttackRange() + 300
                || Target.Health > rSpell.Spell.GetDamage(Target)
                || Target.CountEnemiesInRange(rSpell.Spell.Range) > 1
                || !CheckGuardians())
            {
                return;
            }

            rSpell.Spell.Cast(Target);
        }
    }
}
