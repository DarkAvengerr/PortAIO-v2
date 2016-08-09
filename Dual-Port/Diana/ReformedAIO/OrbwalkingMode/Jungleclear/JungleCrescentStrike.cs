using EloBuddy; namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Jungleclear
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class JungleCrescentStrike : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[Q] Crescent Strike";

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
            this.Menu.AddItem(
                new MenuItem(this.Name + "JungleQDistance", "Q Distance").SetValue(new Slider(730, 0, 825)));

            this.Menu.AddItem(new MenuItem(this.Name + "JungleQMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMob()
        {
            var mobs =
                MinionManager.GetMinions(
                    this.Menu.Item(this.Menu.Name + "JungleQDistance").GetValue<Slider>().Value,
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
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.Q].IsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "JungleQMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            this.GetMob();
        }

        #endregion
    }
}