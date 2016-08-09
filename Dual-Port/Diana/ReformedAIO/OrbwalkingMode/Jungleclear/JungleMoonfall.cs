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

    internal class JungleMoonfall : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[E] Moonfall";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate += this.OnUpdate;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu.AddItem(new MenuItem(this.Name + "JungleEHealth", "Health %").SetValue(new Slider(15, 0, 35)));

            this.Menu.AddItem(new MenuItem(this.Name + "JungleEMana", "Mana %").SetValue(new Slider(15, 0, 35)));

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(false));
        }

        private void GetMob()
        {
            var mobs =
                MinionManager.GetMinions(
                    800 + ObjectManager.Player.AttackRange,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null) return;

            Variables.Spells[SpellSlot.E].Cast(mobs);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.E].IsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "JungleEMana").GetValue<Slider>().Value > Variables.Player.ManaPercent
                || this.Menu.Item(this.Menu.Name + "JungleEHealth").GetValue<Slider>().Value
                > Variables.Player.HealthPercent) return;

            this.GetMob();
        }

        #endregion
    }
}