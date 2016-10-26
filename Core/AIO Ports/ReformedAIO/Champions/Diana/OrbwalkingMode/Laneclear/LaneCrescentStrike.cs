namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Laneclear
{
    #region Using Directives

    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class LaneCrescentStrike : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[Q] Crescent Strike";

        #endregion

        private readonly Orbwalking.Orbwalker orbwalker;

        public LaneCrescentStrike(Orbwalking.Orbwalker orbwalker)
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
            Menu.AddItem(new MenuItem(Name + "LaneQEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "LaneQDistance", "Q Distance").SetValue(new Slider(730, 0, 825)));

            Menu.AddItem(new MenuItem(Name + "LaneQHit", "Min Minions Hit").SetValue(new Slider(3, 0, 6)));

            Menu.AddItem(new MenuItem(Name + "LaneQMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMinions()
        {
            var minions =
                MinionManager.GetMinions(Menu.Item(Menu.Name + "LaneQDistance").GetValue<Slider>().Value);

            if (minions == null) return;

            if (Menu.Item(Menu.Name + "LaneQEnemy").GetValue<bool>())
            {
                if (minions.Any(m => m.CountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            if (minions.Count < Menu.Item(Menu.Name + "LaneQHit").GetValue<Slider>().Value) return;

            foreach (var qPRed in minions.Select(m => Variables.Spells[SpellSlot.Q].GetPrediction(m)))
            {
                Variables.Spells[SpellSlot.Q].Cast(qPRed.CastPosition);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.Q].IsReady()) return;

            if (Menu.Item(Menu.Name + "LaneQMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}