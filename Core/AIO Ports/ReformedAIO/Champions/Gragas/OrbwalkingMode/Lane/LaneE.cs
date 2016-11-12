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

    internal class LaneE : OrbwalkingChild
    {
        #region Public Properties

        public override string Name { get; set; } = "[E] Body Slam";

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

            Menu.AddItem(new MenuItem("LaneEEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem("LaneEDistance", "E Distance").SetValue(new Slider(500, 0, 600)));

            Menu.AddItem(new MenuItem("LaneEHit", "Min Minions Hit").SetValue(new Slider(3, 0, 6)));

            Menu.AddItem(new MenuItem("LaneEMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMinions()
        {
            var minions =
                MinionManager.GetMinions(Menu.Item("LaneEDistance").GetValue<Slider>().Value);

            if (minions == null) return;

            if (Menu.Item("LaneEEnemy").GetValue<bool>())
            {
                if (minions.Any(m => m.CountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            if (minions.Count < Menu.Item("LaneEHit").GetValue<Slider>().Value) return;

            var ePred = Variable.Spells[SpellSlot.E].GetCircularFarmLocation(minions);

            Variable.Spells[SpellSlot.E].Cast(ePred.Position);
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("LaneEMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}