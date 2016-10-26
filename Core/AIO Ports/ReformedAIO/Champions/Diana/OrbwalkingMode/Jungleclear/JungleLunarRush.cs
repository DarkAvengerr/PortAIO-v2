namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Jungleclear
{
    #region Using Directives

    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class JungleLunarRush : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[W] Lunar Rush";

        #endregion

        private readonly Orbwalking.Orbwalker orbwalker;

        public JungleLunarRush(Orbwalking.Orbwalker orbwalker)
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
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem(Name + "JungleWMana", "Mana %").SetValue(new Slider(10, 0, 50)));

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(true));

            
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
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.W].IsReady()) return;

            if (Menu.Item(Menu.Name + "JungleWMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            GetMob();
        }

        #endregion
    }
}