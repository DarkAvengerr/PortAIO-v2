using EloBuddy; namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Jungle
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

    internal class QJungle : ChildBase
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
            this.Menu.AddItem(new MenuItem(this.Menu.Name + "QRange", "Q Range ").SetValue(new Slider(835, 0, 850)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "QMana", "Mana %").SetValue(new Slider(0, 0, 100)));
        }

        private void BarrelRoll()
        {
            var mobs =
                MinionManager.GetMinions(
                    this.Menu.Item(this.Menu.Name + "QRange").GetValue<Slider>().Value,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null || !mobs.IsValid) return;

            Variable.Spells[SpellSlot.Q].Cast(mobs.ServerPosition);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.Q].IsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "QMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.BarrelRoll();
        }

        #endregion
    }
}