namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Lane
{
    using System;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class W2Lane : ChildBase
    {
        private GnarState gnarState;

        public override string Name { get; set; } = "W";

        private readonly Orbwalking.Orbwalker orbwalker;

        public W2Lane(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        private void GameOnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear 
                || ObjectManager.Player.Spellbook.IsAutoAttacking
                || this.gnarState.Mini
                || !Spells.W2.IsReady())
            {
                return;
            }

            var minions = MinionManager.GetMinions(Menu.Item("W2Range").GetValue<Slider>().Value);

            var prediction = Spells.Q2.GetLineFarmLocation(minions);

            if (prediction.MinionsHit >= Menu.Item("W2HitCount").GetValue<Slider>().Value)
            {
                Spells.W2.Cast(prediction.Position);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.gnarState = new GnarState();

            Menu.AddItem(new MenuItem("W2Range", "Range").SetValue(new Slider(525, 0, 525)));
            Menu.AddItem(new MenuItem("W2HitCount", "Min Hit Count").SetValue(new Slider(3, 0, 6)));
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
