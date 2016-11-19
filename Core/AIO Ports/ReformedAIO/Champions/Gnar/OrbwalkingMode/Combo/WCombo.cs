using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WCombo : OrbwalkingChild
    {
        private GnarState gnarState;

        public override string Name { get; set; } = "W";

        private void GameOnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.W2.Range)))
            {
               
                var prediction = Spells.W2.GetPrediction(target);

                if(prediction.Hitchance >= HitChance.High
                    || prediction.AoeTargetsHitCount > 1
                    || ObjectManager.Player.IsCastingInterruptableSpell())
                {
                    Spells.W2.Cast(prediction.CastPosition);
                }
            }
        }
        
        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            gnarState = new GnarState();
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate -= GameOnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate += GameOnUpdate;
        }
    }
}
