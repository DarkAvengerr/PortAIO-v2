using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Jungle
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class W2Jungle : ChildBase
    {
        public override string Name { get; set; } = "W";

        private readonly Orbwalking.Orbwalker orbwalker;

        public W2Jungle(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        private void GameOnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear || ObjectManager.Player.Spellbook.IsAutoAttacking || !Spells.W2.IsReady())
            {
                return;
            }
           
            foreach (var m in MinionManager.GetMinions(Menu.Item("W2Range").GetValue<Slider>().Value, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
            {
                Spells.W2.Cast(m, false, true);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("W2Range", "Range").SetValue(new Slider(525, 0, 525)));
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
