using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Interrupter
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class VayneInterrupter : OrbwalkingChild
    {
        public override string Name { get; set; } = "Interrupter";

        private readonly ESpell eSpell;

        public VayneInterrupter(ESpell espell)
        {  
            this.eSpell = espell;
        }

        private void OnInterruptable(Obj_AI_Base sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!CheckGuardians() || sender == null || args.DangerLevel < Interrupter2.DangerLevel.High)
            {
                return;
            }

            if (sender.IsValidTarget(eSpell.Spell.Range))
            {
                eSpell.Spell.CastOnUnit(sender);
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
