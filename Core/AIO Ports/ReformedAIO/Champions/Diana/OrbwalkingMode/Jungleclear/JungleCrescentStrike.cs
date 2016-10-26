namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Jungleclear
{
    #region Using Directives

    using System;

    using EloBuddy;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class JungleCrescentStrike : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[Q] Crescent Strike";

        #endregion

        private readonly Orbwalking.Orbwalker orbwalker;

        public JungleCrescentStrike(Orbwalking.Orbwalker orbwalker)
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
            Menu.AddItem(
                new MenuItem(Name + "JungleQDistance", "Q Distance").SetValue(new Slider(730, 0, 825)));

            Menu.AddItem(new MenuItem(Name + "JungleQMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMob()
        {
            var mobs =
                MinionManager.GetMinions(
                    Menu.Item(Menu.Name + "JungleQDistance").GetValue<Slider>().Value,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

            if (mobs == null) return;

            foreach (var m in mobs)
            {
                if (!m.IsValid) return;

                Variables.Spells[SpellSlot.Q].Cast(m);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.Q].IsReady()) return;

            if (Menu.Item(Menu.Name + "JungleQMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            GetMob();
        }

        #endregion
    }
}