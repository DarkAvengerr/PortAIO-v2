using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class LaneW : OrbwalkingChild
    {
        #region Public Properties

        public override string Name { get; set; } = "[W] Drunken Rage";

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
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("LaneWEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem("LaneWMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMinions()
        {
            var minions = MinionManager.GetMinions(300);

            if (minions == null) return;

            if (Menu.Item("LaneWEnemy").GetValue<bool>())
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
            if (!CheckGuardians() || Menu.Item("LaneWMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}