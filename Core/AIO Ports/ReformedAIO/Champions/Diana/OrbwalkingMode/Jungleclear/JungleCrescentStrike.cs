using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Jungleclear
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class JungleCrescentStrike : OrbwalkingChild
    {
        #region Public Properties

        public override string Name { get; set; } = "[Q] Crescent Strike";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            Menu.AddItem(
                new MenuItem("JungleQDistance", "Q Distance").SetValue(new Slider(730, 0, 825)));

            Menu.AddItem(new MenuItem("JungleQMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMob()
        {
            var mobs =
                MinionManager.GetMinions(
                    Menu.Item("JungleQDistance").GetValue<Slider>().Value,
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
            if (!CheckGuardians())
            {
                return;
            }

            if (Menu.Item("JungleQMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            GetMob();
        }

        #endregion
    }
}