namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Jungle
{
    #region Using Directives

    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class EJungle : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[E] Body Slam";

        #endregion
        private readonly Orbwalking.Orbwalker orbwalker;

        public EJungle(Orbwalking.Orbwalker orbwalker)
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
            Menu.AddItem(new MenuItem(Menu.Name + "EMana", "Mana %").SetValue(new Slider(10, 0, 100)));
        }

        private void BodySlam()
        {
            var mobs =
                MinionManager.GetMinions(375, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault();

            if (mobs == null || !mobs.IsValid) return;

            Variable.Spells[SpellSlot.E].Cast(mobs.Position);
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.E].IsReady()) return;

            if (Menu.Item(Menu.Name + "EMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            BodySlam();
        }

        #endregion
    }
}