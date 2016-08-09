using EloBuddy; namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Laneclear
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class LaneCrescentStrike : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[Q] Crescent Strike";

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
            this.Menu.AddItem(new MenuItem(this.Name + "LaneQEnemy", "Only If No Enemies Visible").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Name + "LaneQDistance", "Q Distance").SetValue(new Slider(730, 0, 825)));

            this.Menu.AddItem(new MenuItem(this.Name + "LaneQHit", "Min Minions Hit").SetValue(new Slider(3, 0, 6)));

            this.Menu.AddItem(new MenuItem(this.Name + "LaneQMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMinions()
        {
            var minions =
                MinionManager.GetMinions(this.Menu.Item(this.Menu.Name + "LaneQDistance").GetValue<Slider>().Value);

            if (minions == null) return;

            if (this.Menu.Item(this.Menu.Name + "LaneQEnemy").GetValue<bool>())
            {
                if (minions.Any(m => m.CountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            if (minions.Count < this.Menu.Item(this.Menu.Name + "LaneQHit").GetValue<Slider>().Value) return;

            foreach (var qPRed in minions.Select(m => Variables.Spells[SpellSlot.Q].GetPrediction(m)))
            {
                Variables.Spells[SpellSlot.Q].Cast(qPRed.CastPosition);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.Q].IsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "LaneQMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            this.GetMinions();
        }

        #endregion
    }
}