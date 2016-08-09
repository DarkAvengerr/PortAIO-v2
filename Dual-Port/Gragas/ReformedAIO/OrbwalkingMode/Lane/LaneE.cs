using EloBuddy; namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class LaneE : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[E] Body Slam";

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
            this.Menu.AddItem(new MenuItem(this.Name + "LaneEEnemy", "Only If No Enemies Visible").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Name + "LaneEDistance", "E Distance").SetValue(new Slider(500, 0, 600)));

            this.Menu.AddItem(new MenuItem(this.Name + "LaneEHit", "Min Minions Hit").SetValue(new Slider(3, 0, 6)));

            this.Menu.AddItem(new MenuItem(this.Name + "LaneEMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMinions()
        {
            var minions =
                MinionManager.GetMinions(this.Menu.Item(this.Menu.Name + "LaneEDistance").GetValue<Slider>().Value);

            if (minions == null) return;

            if (this.Menu.Item(this.Menu.Name + "LaneEEnemy").GetValue<bool>())
            {
                if (minions.Any(m => m.CountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            if (minions.Count < this.Menu.Item(this.Menu.Name + "LaneEHit").GetValue<Slider>().Value) return;

            var ePred = Variable.Spells[SpellSlot.E].GetCircularFarmLocation(minions);

            Variable.Spells[SpellSlot.E].Cast(ePred.Position);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.E].IsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "LaneEMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.GetMinions();
        }

        #endregion
    }
}