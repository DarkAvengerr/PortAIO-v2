using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK.Utils;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        public readonly QSpell qSpell;

        public QKillsteal(QSpell qSpell)
        {
            this.qSpell = qSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(qSpell.Spell.Range, TargetSelector.DamageType.Physical);

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

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || Target.Health > qSpell.Spell.GetDamage(Target)
                || Target.Distance(ObjectManager.Player) < ObjectManager.Player.GetRealAutoAttackRange()
                || !CheckGuardians())
            {
                return;
            }

            var pos = qSpell.Spell.GetPrediction(Target);

            qSpell.Spell.Cast(pos.CastPosition);
        }
    }
}
