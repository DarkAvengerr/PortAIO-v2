using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.PermaActive.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class QKillsteal : ChildBase
    {
        public QKillsteal(string name)
        {
            this.Name = name;
        }

        private GnarState gnarState;

        public override string Name { get; set; }

        private AIHeroClient Target => TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical);

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += this.OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.gnarState = new GnarState();
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.Target == null || this.Target.Health > Spells.Q.GetDamage(Target))
            {
                return;
            }

            if (this.gnarState.Mini)
            {
                var pos = Spells.Q.GetPrediction(Target);

                if (pos.Hitchance < HitChance.High)
                {
                    return;
                }

                Spells.Q.Cast(pos.CastPosition);
            }
            else
            {
                var pos = Spells.Q2.GetPrediction(Target);

                if (pos.Hitchance < HitChance.High)
                {
                    return;
                }

                Spells.Q2.Cast(pos.CastPosition);
            }
        }
    }
}