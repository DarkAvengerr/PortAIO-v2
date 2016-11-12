using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.Drawings.Damage
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;
    using ReformedAIO.Library.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    internal sealed class GnarDamage : ChildBase
    {
        public override string Name { get; set; }

        public GnarDamage(string name)
        {
            Name = name;
        }

        private Dmg dmg;

        private HeroHealthBarIndicator heroHealthBarIndicator;

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500)))
            {

                this.heroHealthBarIndicator.Unit = enemy;
                this.heroHealthBarIndicator.DrawDmg(this.dmg.GetDamage(enemy), enemy.Health <= this.dmg.GetDamage(enemy) * 1.25 ? Color.LawnGreen : Color.Yellow);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            this.heroHealthBarIndicator = new HeroHealthBarIndicator();
            this.dmg = new Dmg();
        }
    }
}
