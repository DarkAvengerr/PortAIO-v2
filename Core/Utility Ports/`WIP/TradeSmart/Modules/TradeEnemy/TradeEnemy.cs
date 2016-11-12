using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TradeSmart.Modules.TradeEnemy
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal class TradeEnemy : ChildBase
    {
        public override string Name { get; set; } = "Track Trades";

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            var enemyHero = HeroManager.Enemies.FirstOrDefault(x => x.Distance(ObjectManager.Player) <= 1350);

            if (enemyHero == null || !enemyHero.IsHPBarRendered)
            {
                return;
            }

            switch (Menu.Item("Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:

                    if (enemyHero.Spellbook.IsAutoAttacking && enemyHero.Distance(ObjectManager.Player) < ObjectManager.Player.AttackRange + 50)
                    {
                        Render.Circle.DrawCircle(enemyHero.Position,
                            Menu.Item("BoundingRadius").GetValue<Slider>().Value,
                            Color.Green, Menu.Item("Width").GetValue<Slider>().Value);
                    }

                    break;

                case 1:

                    if (enemyHero.Distance(ObjectManager.Player) > ObjectManager.Player.AttackRange + 50)
                    {
                        return;
                    }

                    Render.Circle.DrawCircle(enemyHero.Position,
                        Menu.Item("BoundingRadius").GetValue<Slider>().Value, 
                        Color.Green, Menu.Item("Width").GetValue<Slider>().Value);

                    break;
            }

            if (!Menu.Item("DrawSelf").GetValue<bool>())
            {
                return;
            }

            if (ObjectManager.Player.Distance(enemyHero) <= enemyHero.AttackRange * 0.8 
                && ObjectManager.Player.CountAlliesInRange(1000) < ObjectManager.Player.CountEnemiesInRange(1000) 
                && ObjectManager.Player.Health > enemyHero.Health)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 75, Color.Red, 6);
            } 
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("Mode", "Mode").SetValue(new StringList(new [] {"Safe", "Agressive"})));

            Menu.AddItem(new MenuItem("DrawSelf", "Draw Self").SetValue(true).SetTooltip("Draws if you're in a dangerous zone"));

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
