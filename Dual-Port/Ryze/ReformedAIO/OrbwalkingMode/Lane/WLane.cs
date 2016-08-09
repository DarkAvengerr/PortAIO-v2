using EloBuddy; namespace ReformedAIO.Champions.Ryze.OrbwalkingMode.Lane
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
        #region Public Properties

        public override string Name { get; set; } = "[W] Rune Prison";

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
            this.Menu.AddItem(new MenuItem(this.Name + "LaneWEnemy", "Only If No Enemies Visible").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Name + "LaneWMana", "Mana %").SetValue(new Slider(70, 0, 100)));
        }

        private void GetMinions()
        {
            var minions = MinionManager.GetMinions(Variable.Spells[SpellSlot.W].Range);

            if (minions == null) return;

            if (this.Menu.Item(this.Menu.Name + "LaneWEnemy").GetValue<bool>()
                && minions.Any(m => m.CountEnemiesInRange(2000) > 0))
            {
                return;
            }

            foreach (var m in minions)
            {
                if (m.Health > Variable.Spells[SpellSlot.W].GetDamage(m)) return;

                Variable.Spells[SpellSlot.W].Cast(m);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.W].IsReady() || !Variable.Player.Spellbook.IsAutoAttacking) return;

            if (this.Menu.Item(this.Menu.Name + "LaneWMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.GetMinions();
        }

        #endregion
    }
}