using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Lane
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class W2Lane : OrbwalkingChild
    {
        private GnarState gnarState;

        public override string Name { get; set; } = "W";

        private void GameOnUpdate(EventArgs args)
        {
            if (this.gnarState.Mini
                || !CheckGuardians())
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

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            this.gnarState = new GnarState();

            Menu.AddItem(new MenuItem("W2Range", "Range").SetValue(new Slider(525, 0, 525)));
            Menu.AddItem(new MenuItem("W2HitCount", "Min Hit Count").SetValue(new Slider(3, 0, 6)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate -= GameOnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate += GameOnUpdate;
        }
    }
}
