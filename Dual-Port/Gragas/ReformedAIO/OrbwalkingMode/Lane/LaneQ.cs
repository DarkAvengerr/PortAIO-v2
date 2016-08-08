using EloBuddy; namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class LaneQ : ChildBase
    {
        #region Fields

        private QLogic qLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[Q] Barrel Roll";

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

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.qLogic = new QLogic();
            base.OnInitialize(sender, featureBaseEventArgs);
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
                if (minions.Any(m => m.LSCountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            if (minions.Count < this.Menu.Item(this.Menu.Name + "LaneQHit").GetValue<Slider>().Value) return;

            var qPred = Variable.Spells[SpellSlot.Q].GetCircularFarmLocation(minions);

            foreach (var m in minions)
            {
                if (this.qLogic.CanThrowQ())
                {
                    Variable.Spells[SpellSlot.Q].Cast(qPred.Position);
                }

                if (!(Variable.Spells[SpellSlot.Q].GetDamage(m) > m.Health)) continue;

                if (this.qLogic.CanExplodeQ(m))
                {
                    Variable.Spells[SpellSlot.Q].Cast();
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.Q].LSIsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "LaneQMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.GetMinions();
        }

        #endregion
    }
}