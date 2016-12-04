using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gnar.PermaActive.Antigapcloser
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class W : OrbwalkingChild
    {
        private GnarState gnarState;

        public override string Name { get; set; } = "AntiGapcloser";

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!CheckGuardians() || gnarState.Mini || gapcloser.Sender == null)
            {
                return;
            }

            var target = gapcloser.Sender;

            if (target.IsValidTarget(Spells.W2.Range))
            {
                Spells.W2.Cast(gapcloser.End);
            }
        }


        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            gnarState = new GnarState();
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
        }
    }
}
