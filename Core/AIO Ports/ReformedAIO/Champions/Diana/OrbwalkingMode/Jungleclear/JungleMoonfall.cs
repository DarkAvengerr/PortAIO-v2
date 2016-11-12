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

    internal class JungleMoonfall : OrbwalkingChild
    {
        #region Public Properties

        public override string Name { get; set; } = "[E] Moonfall";

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
            Menu.AddItem(new MenuItem("JungleEHealth", "Health %").SetValue(new Slider(15, 0, 35)));

            Menu.AddItem(new MenuItem("JungleEMana", "Mana %").SetValue(new Slider(15, 0, 35)));
        }

        private static void GetMob()
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
            if (!CheckGuardians()
                || Menu.Item("JungleEMana").GetValue<Slider>().Value > Variables.Player.ManaPercent
                || Menu.Item("JungleEHealth").GetValue<Slider>().Value
                > Variables.Player.HealthPercent)
            {
                return;
            }

            GetMob();
        }

        #endregion
    }
}