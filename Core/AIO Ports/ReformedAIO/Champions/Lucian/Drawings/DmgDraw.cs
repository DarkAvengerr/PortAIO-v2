using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.Drawings
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Damage;
    using ReformedAIO.Library.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    internal sealed class DmgDraw : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        public DmgDraw(LucDamage damage)
        {
            this.Damage = damage;
        }

        private HeroHealthBarIndicator heroHealthBarIndicator;

        public readonly LucDamage Damage;

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500) && ene.IsVisible))
            {
                heroHealthBarIndicator.Unit = enemy;

                heroHealthBarIndicator.DrawDmg(Damage.GetComboDamage(enemy), enemy.Health <= Damage.GetComboDamage(enemy) * 1.25 
                    ? Color.LawnGreen
                    : Color.Yellow);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            heroHealthBarIndicator = new HeroHealthBarIndicator();
        }
    }
}
