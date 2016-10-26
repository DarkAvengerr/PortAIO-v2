namespace ReformedAIO.Champions.Caitlyn.Killsteal
{
    using System;

    using EloBuddy;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Utils;

    using ReformedAIO.Champions.Caitlyn.Logic;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private AIHeroClient Target => TargetSelector.GetTarget(Spells.Spell[SpellSlot.R].Range, TargetSelector.DamageType.Physical);

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        private void OnUpdate(EventArgs args)
        {
             // Brian if you see this i'm sorry XD
            if (Spells.Spell[SpellSlot.R].Level < 2)
            {
                Spells.Spell[SpellSlot.R].Range = 2000;
            }
            else
            {
                Spells.Spell[SpellSlot.R].Range = 1500 + 500 * Spells.Spell[SpellSlot.R].Level;
            }

            if (Target == null
                || Target.Distance(Vars.Player) < Orbwalking.GetRealAutoAttackRange(Vars.Player) + 450
                || Target.Health > Spells.Spell[SpellSlot.R].GetDamage(Target)
                || Target.CountEnemiesInRange(Spells.Spell[SpellSlot.R].Range) > 1
                || !CheckGuardians())
            {
                return;
            }

            Spells.Spell[SpellSlot.R].Cast(Target);
        }
    }
}
