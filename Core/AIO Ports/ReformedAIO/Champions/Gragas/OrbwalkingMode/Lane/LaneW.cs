namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane
{
    #region Using Directives

    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class LaneW : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[W] Drunken Rage";

        #endregion
        private readonly Orbwalking.Orbwalker orbwalker;

        public LaneW(Orbwalking.Orbwalker orbwalker)
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
            Menu.AddItem(new MenuItem(Name + "LaneWEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "LaneWMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMinions()
        {
            var minions = MinionManager.GetMinions(300);

            if (minions == null) return;

            if (Menu.Item(Menu.Name + "LaneWEnemy").GetValue<bool>())
            {
                if (minions.Any(m => m.CountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            foreach (var m in minions.Where(m => m.Distance(Variable.Player) <= 250))
            {
                Variable.Spells[SpellSlot.W].Cast();
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.W].IsReady()) return;

            if (Menu.Item(Menu.Name + "LaneWMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}