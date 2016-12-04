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

    internal class WLane : ChildBase
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

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("LaneWEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(
                new MenuItem("LaneWMDistance", "Distance").SetValue(new Slider(600, 0, 900))
                    .SetTooltip("Put it too high and you'll miss minions"));

            Menu.AddItem(new MenuItem("LaneWMana", "Mana %").SetValue(new Slider(70, 0, 100)));
        }

        private void GetMinions()
        {
            var minions =
                MinionManager.GetMinions(Menu.Item("LaneWMDistance").GetValue<Slider>().Value);

            if (minions == null 
                || (Menu.Item("LaneWEnemy").GetValue<bool>()
                && minions.Any(m => m.CountEnemiesInRange(2000) > 0))
                || minions.Count <= 2) return;

            foreach (var m in minions)
            {
                if (m.Health < Variable.Spells[SpellSlot.W].GetDamage(m) * 1.2) return;

                Variable.Spells[SpellSlot.W].CastIfHitchanceEquals(m, HitChance.High);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.W].IsReady() || Menu.Item("LaneWMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}