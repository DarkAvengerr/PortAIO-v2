using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Jungle
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class EJungle : OrbwalkingChild
    {
        #region Public Properties

        public override string Name { get; set; } = "[E] Body Slam";

        #endregion
        
        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("EMana", "Mana %").SetValue(new Slider(10, 0, 100)));
        }

        private static void BodySlam()
        {
            var mobs =
                MinionManager.GetMinions(375, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault();

            if (mobs == null || !mobs.IsValid) return;

            Variable.Spells[SpellSlot.E].Cast(mobs.Position);
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("EMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            BodySlam();
        }

        #endregion
    }
}