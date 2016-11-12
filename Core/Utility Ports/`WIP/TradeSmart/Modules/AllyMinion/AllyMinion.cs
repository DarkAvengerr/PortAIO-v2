using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TradeSmart.Modules.AllyMinion
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using System.Drawing;

    internal class AllyMinion : ChildBase
    {
        public override string Name { get; set; } = "Ally Minion";

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            var enemyHero = HeroManager.Enemies.FirstOrDefault(x => x.Distance(ObjectManager.Player) <= 1000);

            if (enemyHero == null || !enemyHero.IsHPBarRendered)
            {
                return;
            }

            var allyMinions = MinionManager.GetMinions(enemyHero.AttackRange, MinionTypes.All, MinionTeam.Ally);

            if (allyMinions == null)
            {
                return;
            }

            foreach (var aM in allyMinions)
            {
                if (aM.Health < enemyHero.GetAutoAttackDamage(aM))
                {
                    Render.Circle.DrawCircle(aM.Position,                  // Position
                     Menu.Item("BoundingRadius").GetValue<Slider>().Value, // Radius f the circle
                       Color.Green,                                        // Color
                       Menu.Item("Width").GetValue<Slider>().Value, true); // Width
                }
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("BoundingRadius", "Radius")).SetValue(new Slider(65, 1, 120));

            Menu.AddItem(new MenuItem("Width", "Width Of The Circle")).SetValue(new Slider(5, 1, 10));
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
    }
}
