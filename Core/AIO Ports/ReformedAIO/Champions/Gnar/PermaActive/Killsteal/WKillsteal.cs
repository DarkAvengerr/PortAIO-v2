using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.PermaActive.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class WKillsteal : ChildBase
    {
        public WKillsteal(string name)
        {
            this.Name = name;
        }

        public override string Name { get; set; }

        private AIHeroClient Target => TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical);

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate += this.OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.Target == null || this.Target.Health > Spells.W2.GetDamage(Target))
            {
                return;
            }


            var pos = Spells.W2.GetPrediction(Target);

            if (pos.Hitchance < HitChance.VeryHigh)
            {
                return;
            }

            Spells.W2.Cast(pos.CastPosition);
        }
    }
}