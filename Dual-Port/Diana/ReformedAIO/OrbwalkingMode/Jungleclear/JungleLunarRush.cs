using EloBuddy; namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Jungleclear
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class JungleLunarRush : ChildBase
    {

        #region Public Properties

        public override string Name { get; set; } = "[W] Lunar Rush";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate -= this.OnUpdate;

        }

        protected override void OnEnable(object sender, Base.FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate += this.OnUpdate;
            
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu = new Menu(this.Name, this.Name);

            Menu.AddItem(new MenuItem(this.Name + "JungleWMana", "Mana %").SetValue(new Slider(10, 0, 50)));

            Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            
        }

        private void GetMob()
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
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.W].LSIsReady()) return;

            if (Menu.Item(Menu.Name + "JungleWMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            this.GetMob();
        }

        #endregion
    }
}