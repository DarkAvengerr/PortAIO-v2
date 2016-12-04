using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.LaneClear
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class QLane : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[Q]";

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

            Menu.AddItem(new MenuItem("LaneQEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem("LaneQMana", "Mana %").SetValue(new Slider(65, 0, 100)));
        }

        private void GetMinions()
        {
            var minions = MinionManager.GetMinions(Variable.Player.AttackRange);

            if (minions == null) return;

            if (Menu.Item("LaneQEnemy").GetValue<bool>()
                && minions.Any(m => m.CountEnemiesInRange(2500) > 0))
            {
                return;
            }

            Variable.Spells[SpellSlot.Q].Cast();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.Q].IsReady()) return;

            if (Menu.Item("LaneQMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}