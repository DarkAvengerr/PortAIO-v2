using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.Misc
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class GragasInterrupter : OrbwalkingChild
    {
        public override string Name { get; set; } = "Interrupter";

        private readonly RSpell spell;

        public GragasInterrupter(RSpell spell)
        {
            this.spell = spell;
        }

        private void OnInterruptable(Obj_AI_Base sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!CheckGuardians() || sender == null || args.DangerLevel < Interrupter2.DangerLevel.High)
            {
                return;
            }

            if (sender.IsValidTarget(spell.Spell.Range))
            {
                spell.Spell.Cast(spell.CastPosition(sender));
            }
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
