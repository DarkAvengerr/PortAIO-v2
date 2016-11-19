using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Jungle
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class W2Jungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private void GameOnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }
           
            foreach (var m in MinionManager.GetMinions(Menu.Item("W2Range").GetValue<Slider>().Value, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
            {
                Spells.W2.Cast(m, false, true);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("W2Range", "Range").SetValue(new Slider(525, 0, 525)));
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
