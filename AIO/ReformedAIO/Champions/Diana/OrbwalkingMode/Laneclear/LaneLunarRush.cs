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

    #endregion

    internal class LaneLunarRush : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "[W] Lunar Rush";

        #endregion

        private readonly Orbwalking.Orbwalker orbwalker;

        public LaneLunarRush(Orbwalking.Orbwalker orbwalker)
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
            Menu.AddItem(new MenuItem(Name + "LaneWEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "LaneWMana", "Mana %").SetValue(new Slider(15, 0, 100)));
        }

        private void GetMinions()
        {
            var minions = MinionManager.GetMinions(200f);

            if (minions == null) return;

            if (minions.Count < 3) return;

            if (Menu.Item(Menu.Name + "LaneWEnemy").GetValue<bool>())
            {
                if (minions.Any(m => m.CountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            Variables.Spells[SpellSlot.W].Cast();
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variables.Spells[SpellSlot.W].IsReady()) return;

            if (Menu.Item(Menu.Name + "LaneWMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}