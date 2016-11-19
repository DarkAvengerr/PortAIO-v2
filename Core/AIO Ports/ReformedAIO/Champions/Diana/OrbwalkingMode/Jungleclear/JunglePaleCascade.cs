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

    internal class JunglePaleCascade : OrbwalkingChild
    {
        #region Public Properties

        public override string Name { get; set; } = "[R] Pale Cascade";

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
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem("JungleRMana", "Mana %").SetValue(new Slider(35, 0, 100)));

            Menu.AddItem(
                new MenuItem("Enabled", "Enabled").SetValue(true)
                    .SetTooltip("Wont cast unless Reset avaible"));
        }

        private void GetMob()
        {
            var mobs =
                MinionManager.GetMinions(825, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault();

            if (mobs == null) return;

            if (!mobs.HasBuff("dianamoonlight")) return;

            Variables.Spells[SpellSlot.R].Cast(mobs);
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("JungleRMana").GetValue<Slider>().Value > Variables.Player.ManaPercent)
            {
                return;
            }

            GetMob();
        }

        #endregion
    }
}