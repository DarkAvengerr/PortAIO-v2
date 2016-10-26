namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Jungle
{
    #region Using Directives

    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class QJungle : ChildBase
    {
        #region Fields

        private QLogic qLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[Q] Barrel Roll";

        #endregion
        private readonly Orbwalking.Orbwalker orbwalker;

        public QJungle(Orbwalking.Orbwalker orbwalker)
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

        protected override sealed void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem(Menu.Name + "QRange", "Q Range ").SetValue(new Slider(835, 0, 850)));

            Menu.AddItem(new MenuItem(Menu.Name + "QMana", "Mana %").SetValue(new Slider(0, 0, 100)));

            qLogic = new QLogic();
        }

        private void BarrelRoll()
        {
            var mobs =
                MinionManager.GetMinions(
                    Menu.Item(Menu.Name + "QRange").GetValue<Slider>().Value,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null || !mobs.IsValid) return;

            Variable.Spells[SpellSlot.Q].Cast(mobs.ServerPosition);
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.Q].IsReady()) return;

            if (Menu.Item(Menu.Name + "QMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            BarrelRoll();
        }

        #endregion
    }
}