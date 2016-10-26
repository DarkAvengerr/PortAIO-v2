namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.JungleClear
{
    #region Using Directives

    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class WJungle : ChildBase
    {
        #region Constructors and Destructors

        private readonly Orbwalking.Orbwalker orbwalker;

        public WJungle(string name, Orbwalking.Orbwalker orbwalker)
        {
            Name = name;
            this.orbwalker = orbwalker;
        }

        #endregion

        #region Public Properties

        public override string Name { get; set; }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem(Menu.Name + "WRange", "W Range ").SetValue(new Slider(600, 0, 700)));

            Menu.AddItem(new MenuItem(Menu.Name + "WMana", "Mana %").SetValue(new Slider(7, 0, 100)));
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.W].IsReady()) return;

            if (Menu.Item(Menu.Name + "WMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            Volley();
        }

        private void Volley()
        {
            var mobs =
                MinionManager.GetMinions(
                    Menu.Item(Menu.Name + "WRange").GetValue<Slider>().Value,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null || !mobs.IsValid) return;

            Variable.Spells[SpellSlot.W].CastIfHitchanceEquals(mobs, HitChance.High);
        }

        #endregion
    }
}