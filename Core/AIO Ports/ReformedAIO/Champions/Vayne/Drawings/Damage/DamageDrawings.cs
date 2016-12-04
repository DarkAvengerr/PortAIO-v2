using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Drawings.Damage
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Damage;
    using ReformedAIO.Library.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    internal sealed class DamageDrawing : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        public readonly Damages Damage;

        public DamageDrawing(Damages damage)
        {
            this.Damage = damage;
        }

        private HeroHealthBarIndicator heroHealthBarIndicator;

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500)))
            {
                if (Menu.Item("Damage").GetValue<bool>())
                {
                    heroHealthBarIndicator.Unit = enemy;

                    heroHealthBarIndicator.DrawDmg(Damage.GetComboDamage(enemy),
                        enemy.Health <= Damage.GetComboDamage(enemy) * .9
                        ? Color.LawnGreen
                        : Color.Yellow);
                }

                if (Menu.Item("Counter").GetValue<bool>())
                {
                    Drawing.DrawText(
                        enemy.HPBarPosition.X + Menu.Item("Vayne.Drawings.Damage.X").GetValue<Slider>().Value,
                        enemy.HPBarPosition.Y + Menu.Item("Vayne.Drawings.Damage.Y").GetValue<Slider>().Value,
                        System.Drawing.Color.AliceBlue,
                        "Attacks: " + Damage.DamageCounter(enemy));
                }
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
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Damage", "Damage").SetValue(true));

            Menu.AddItem(new MenuItem("Counter", "AutoAttack Counter").SetValue(true));

            Menu.AddItem(new MenuItem("Vayne.Drawings.Damage.X", "AutoAttack X-Axis").SetValue(new Slider(0, -50, 50)));

            Menu.AddItem(new MenuItem("Vayne.Drawings.Damage.Y", "AutoAttack Y-Axis").SetValue(new Slider(0, -50, 50)));

            heroHealthBarIndicator = new HeroHealthBarIndicator();
        }
    }
}
