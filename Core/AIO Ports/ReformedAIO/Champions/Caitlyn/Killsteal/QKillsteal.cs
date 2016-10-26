namespace ReformedAIO.Champions.Caitlyn.Killsteal
{
    using System;

    using EloBuddy;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Utils;

    using ReformedAIO.Champions.Caitlyn.Logic;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private AIHeroClient Target => TargetSelector.GetTarget(Spells.Spell[SpellSlot.Q].Range, TargetSelector.DamageType.Physical);

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
        }

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || Target.Health > Spells.Spell[SpellSlot.Q].GetDamage(Target)
                || Target.Distance(Vars.Player) < Orbwalking.GetRealAutoAttackRange(Vars.Player)
                || Spells.Spell[SpellSlot.Q].Delay + Spells.Spell[SpellSlot.Q].Speed < Target.MoveSpeed
                || !CheckGuardians())
            {
                return;
            }

            var pos = Spells.Spell[SpellSlot.Q].GetPrediction(Target);

            Spells.Spell[SpellSlot.Q].Cast(pos.CastPosition);
        }
    }
}
