using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Brand.Misc
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Brand.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class BrandInterrupter : OrbwalkingChild
    {
        public override string Name { get; set; } = "Interrupter";

        private readonly QSpell spell;

        public BrandInterrupter(QSpell spell)
        {
            this.spell = spell;
        }

        private void OnInterruptable(Obj_AI_Base sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!CheckGuardians()
                || sender == null 
                || !sender.IsValidTarget(spell.Spell.Range) 
                || spell.Prediction(sender).Hitchance < HitChance.High)
            {
                return;
            }

            spell.Spell.Cast(spell.Prediction(sender).CastPosition);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Interrupter2.OnInterruptableTarget -= OnInterruptable;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Interrupter2.OnInterruptableTarget += OnInterruptable;
        }
    }
}
