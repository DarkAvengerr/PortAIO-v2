using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ashe.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class QCombo : ChildBase
    {
        #region Fields

        private QLogic qLogic;

        #endregion

        #region Constructors and Destructors

        private readonly Orbwalking.Orbwalker orbwalker;

        public QCombo(string name, Orbwalking.Orbwalker orbwalker)
        {
            Name = name;
            this.orbwalker = orbwalker;
        }

        #endregion

        #region Public Properties

        public override string Name { get; set; }

        #endregion

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
        //}

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem(Menu.Name + "QMana", "Mana %").SetValue(new Slider(0, 0, 50)));

            Menu.AddItem(new MenuItem(Name + "AAQ", "AA Before Q").SetValue(true).SetTooltip("Wont cancel AA with Q"));

            qLogic = new QLogic();
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !Variable.Spells[SpellSlot.Q].IsReady()) return;

            if (Menu.Item(Menu.Name + "QMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            RangersFocus();
        }

        private void RangersFocus()
        {
            var target = HeroManager.Enemies.Where(Orbwalking.InAutoAttackRange).FirstOrDefault();

            if (target == null || !target.IsValid) return;

            if (Menu.Item(Menu.Name + "AAQ").GetValue<bool>() && Variable.Player.Spellbook.IsAutoAttacking) return;

            Variable.Spells[SpellSlot.Q].Cast();

            qLogic.Kite(target);
        }

        #endregion
    }
}