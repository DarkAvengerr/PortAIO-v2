using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

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
        private readonly Orbwalking.Orbwalker orbwalker;

        public LaneQ(Orbwalking.Orbwalker orbwalker)
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

        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    qLogic = new QLogic();
        //    base.OnLoad(sender, featureBaseEventArgs);
        //}

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem(Name + "LaneQEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "LaneQDistance", "Q Distance").SetValue(new Slider(730, 0, 825)));

            Menu.AddItem(new MenuItem(Name + "LaneQHit", "Min Minions Hit").SetValue(new Slider(3, 0, 6)));

            Menu.AddItem(new MenuItem(Name + "LaneQMana", "Mana %").SetValue(new Slider(15, 0, 100)));

            qLogic = new QLogic();
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

            var qPred = Variable.Spells[SpellSlot.Q].GetCircularFarmLocation(minions);

            foreach (var m in minions)
            {
                if (qLogic.CanThrowQ())
                {
                    Variable.Spells[SpellSlot.Q].Cast(qPred.Position);
                }

                if (!(Variable.Spells[SpellSlot.Q].GetDamage(m) > m.Health)) continue;

                if (qLogic.CanExplodeQ(m))
                {
                    Variable.Spells[SpellSlot.Q].Cast();
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.Q].IsReady()) return;

            if (Menu.Item(Menu.Name + "LaneQMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}