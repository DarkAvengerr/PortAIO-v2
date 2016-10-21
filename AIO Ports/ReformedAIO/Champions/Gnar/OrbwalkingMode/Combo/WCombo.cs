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

    internal sealed class WCombo : ChildBase
    {
        private GnarState gnarState;

        public override string Name { get; set; } = "W";

        private readonly Orbwalking.Orbwalker orbwalker;

        public WCombo(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        private void GameOnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !Spells.W2.IsReady())
            {
                return;
            }

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.W2.Range)))
            {
               
                var prediction = Spells.W2.GetPrediction(target);

                if(prediction.Hitchance >= HitChance.High
                    || prediction.AoeTargetsHitCount > 1
                    || Vars.Player.IsCastingInterruptableSpell())
                {
                    Spells.W2.Cast(prediction.CastPosition);
                }
            }
        }
        
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            gnarState = new GnarState();
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= GameOnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += GameOnUpdate;
        }
    }
}
