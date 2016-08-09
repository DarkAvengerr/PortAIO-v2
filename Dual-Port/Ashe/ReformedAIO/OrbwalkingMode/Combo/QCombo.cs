using EloBuddy; namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ashe.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class QCombo : ChildBase
    {
        #region Fields

        private QLogic qLogic;

        #endregion

        #region Constructors and Destructors

        public QCombo(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Properties

        public override string Name { get; set; }

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
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "QMana", "Mana %").SetValue(new Slider(0, 0, 50)));

            this.Menu.AddItem(new MenuItem(this.Name + "AAQ", "AA Before Q").SetValue(true).SetTooltip("AA Q Reset"));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variable.Spells[SpellSlot.Q].IsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "QMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.RangersFocus();
        }

        private void RangersFocus()
        {
            var target = HeroManager.Enemies.Where(Orbwalking.InAutoAttackRange).FirstOrDefault(x => !x.HasBuff(""));

            if (target == null || !target.IsValid) return;

            if (this.Menu.Item(this.Menu.Name + "AAQ").GetValue<bool>() && Variable.Player.Spellbook.IsAutoAttacking) return;

            Variable.Spells[SpellSlot.Q].Cast();

            this.qLogic.Kite(target);
        }

        #endregion
    }
}