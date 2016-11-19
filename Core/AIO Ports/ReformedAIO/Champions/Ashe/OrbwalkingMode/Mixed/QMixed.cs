using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Mixed
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ashe.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class QMixed : ChildBase
    {
        #region Fields

        private QLogic qLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[Q]";

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

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("QMana", "Mana %").SetValue(new Slider(80, 0, 100)));

            Menu.AddItem(new MenuItem("AAQ", "AA Before Q").SetValue(true).SetTooltip("AA Q Reset"));

            qLogic = new QLogic();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.Q].IsReady() || Menu.Item("QMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            RangersFocus();
        }

        private void RangersFocus()
        {
            var target = TargetSelector.GetTarget(
                Orbwalking.GetRealAutoAttackRange(Variable.Player) + 65,
                TargetSelector.DamageType.Physical);

            if (target == null || !target.IsValid) return;

            if (Menu.Item("AAQ").GetValue<bool>() && Variable.Player.Spellbook.IsAutoAttacking) return;

            Variable.Spells[SpellSlot.Q].Cast();

            qLogic.Kite(target);
        }

        #endregion
    }
}