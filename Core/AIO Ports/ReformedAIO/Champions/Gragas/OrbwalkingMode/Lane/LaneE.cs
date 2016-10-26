namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane
{
    #region Using Directives

    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class LaneE : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[E] Body Slam";

        #endregion
        private readonly Orbwalking.Orbwalker orbwalker;

        public LaneE(Orbwalking.Orbwalker orbwalker)
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
            Menu.AddItem(new MenuItem(Name + "LaneEEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "LaneEDistance", "E Distance").SetValue(new Slider(500, 0, 600)));

            Menu.AddItem(new MenuItem(Name + "LaneEHit", "Min Minions Hit").SetValue(new Slider(3, 0, 6)));

            Menu.AddItem(new MenuItem(Name + "LaneEMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMinions()
        {
            var minions =
                MinionManager.GetMinions(Menu.Item(Menu.Name + "LaneEDistance").GetValue<Slider>().Value);

            if (minions == null) return;

            if (Menu.Item(Menu.Name + "LaneEEnemy").GetValue<bool>())
            {
                if (minions.Any(m => m.CountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            if (minions.Count < Menu.Item(Menu.Name + "LaneEHit").GetValue<Slider>().Value) return;

            var ePred = Variable.Spells[SpellSlot.E].GetCircularFarmLocation(minions);

            Variable.Spells[SpellSlot.E].Cast(ePred.Position);
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.E].IsReady()) return;

            if (Menu.Item(Menu.Name + "LaneEMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}