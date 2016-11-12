using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.JungleClear
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class WJungle : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[W]";

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

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("WRange", "W Range ").SetValue(new Slider(600, 0, 700)));

            Menu.AddItem(new MenuItem("WMana", "Mana %").SetValue(new Slider(7, 0, 100)));
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.W].IsReady()) return;

            if (Menu.Item("WMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            Volley();
        }

        private void Volley()
        {
            var mobs =
                MinionManager.GetMinions(
                    Menu.Item("WRange").GetValue<Slider>().Value,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null || !mobs.IsValid) return;

            Variable.Spells[SpellSlot.W].CastIfHitchanceEquals(mobs, HitChance.High);
        }

        #endregion
    }
}