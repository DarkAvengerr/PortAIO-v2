using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Laneclear
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class LaneCrescentStrike : OrbwalkingChild
    {
        #region Public Properties

        public override string Name { get; set; } = "[Q] Crescent Strike";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem("LaneQEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem("LaneQDistance", "Q Distance").SetValue(new Slider(730, 0, 825)));

            Menu.AddItem(new MenuItem("LaneQHit", "Min Minions Hit").SetValue(new Slider(3, 0, 6)));

            Menu.AddItem(new MenuItem("LaneQMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMinions()
        {
            var minions =
                MinionManager.GetMinions(Menu.Item("LaneQDistance").GetValue<Slider>().Value);

            if (minions == null) return;

            if (Menu.Item("LaneQEnemy").GetValue<bool>())
            {
                if (minions.Any(m => m.CountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            if (minions.Count < Menu.Item("LaneQHit").GetValue<Slider>().Value) return;

            foreach (var qPRed in minions.Select(m => Variables.Spells[SpellSlot.Q].GetPrediction(m)))
            {
                Variables.Spells[SpellSlot.Q].Cast(qPRed.CastPosition);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("LaneQMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;
          
            GetMinions();
        }

        #endregion
    }
}