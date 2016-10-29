using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Jungleclear
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class JungleLunarRush : OrbwalkingChild
    {
        #region Public Properties

        public override string Name { get; set; } = "[W] Lunar Rush";

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
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem("JungleWMana", "Mana %").SetValue(new Slider(10, 0, 50)));

            Menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));

            
        }

        private static void GetMob()
        {
            var mobs =
                MinionManager.GetMinions(
                    150 + ObjectManager.Player.AttackRange,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null) return;

            Variables.Spells[SpellSlot.W].Cast(mobs);
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("JungleWMana").GetValue<Slider>().Value > Variables.Player.ManaPercent)
            {
                return;
            }

            GetMob();
        }

        #endregion
    }
}