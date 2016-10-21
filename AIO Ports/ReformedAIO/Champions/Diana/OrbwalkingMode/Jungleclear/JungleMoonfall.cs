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

    #endregion

    internal class JungleMoonfall : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[E] Moonfall";

        #endregion

        private readonly Orbwalking.Orbwalker orbwalker;

        public JungleMoonfall(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        protected override sealed void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem(Name + "JungleEHealth", "Health %").SetValue(new Slider(15, 0, 35)));

            Menu.AddItem(new MenuItem(Name + "JungleEMana", "Mana %").SetValue(new Slider(15, 0, 35)));

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(false));
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
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.E].IsReady()) return;

            if (Menu.Item(Menu.Name + "JungleEMana").GetValue<Slider>().Value > Variables.Player.ManaPercent
                || Menu.Item(Menu.Name + "JungleEHealth").GetValue<Slider>().Value
                > Variables.Player.HealthPercent) return;

            GetMob();
        }

        #endregion
    }
}