using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ezreal.Drawings
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ezreal.Core.Damage;
    using ReformedAIO.Library.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    internal sealed class DmgDraw : ChildBase
    {
        public override string Name { get; set; } = "Ezreal Damage";

        public DmgDraw(EzrealDamage damage)
        {
            this.Damage = damage;
        }

        private HeroHealthBarIndicator heroHealthBarIndicator;

        public readonly EzrealDamage Damage;

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(2000)))
            {
                heroHealthBarIndicator.Unit = enemy;

                heroHealthBarIndicator.DrawDmg(Damage.GetComboDamage(enemy),
                    enemy.Health <= Damage.GetComboDamage(enemy) * 1.25
                    ? Color.LawnGreen 
                    : Color.Yellow);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);


            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            heroHealthBarIndicator = new HeroHealthBarIndicator();
        }
    }
}
