using EloBuddy; namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.LaneClear
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class WLane : ChildBase
    {
        #region Constructors and Destructors

        public WLane(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Properties

        public override string Name { get; set; }

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
            base.OnLoad(sender, featureBaseEventArgs);

            this.Menu.AddItem(new MenuItem(this.Name + "LaneWEnemy", "Only If No Enemies Visible").SetValue(true));

            this.Menu.AddItem(
                new MenuItem(this.Name + "LaneWMDistance", "Distance").SetValue(new Slider(600, 0, 900))
                    .SetTooltip("Put it too high and you'll miss minions"));

            this.Menu.AddItem(new MenuItem(this.Name + "LaneWMana", "Mana %").SetValue(new Slider(70, 0, 100)));
        }

        private void GetMinions()
        {
            var minions =
                MinionManager.GetMinions(this.Menu.Item(this.Menu.Name + "LaneWMDistance").GetValue<Slider>().Value);

            if (minions == null) return;

            if (this.Menu.Item(this.Menu.Name + "LaneWEnemy").GetValue<bool>()
                && minions.Any(m => m.LSCountEnemiesInRange(2000) > 0))
            {
                return;
            }

            if (minions.Count < 2) return;

            foreach (var m in minions)
            {
                if (m.Health < Variable.Spells[SpellSlot.W].GetDamage(m) * 1.2) return;

                Variable.Spells[SpellSlot.W].CastIfHitchanceEquals(m, HitChance.High);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.W].LSIsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "LaneWMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.GetMinions();
        }

        #endregion
    }
}