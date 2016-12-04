using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Mixed
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class QMixed : OrbwalkingChild
    {
        #region Fields

        private QLogic qLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[Q] Barrel Roll";

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

            Menu.AddItem(new MenuItem("QRange", "Q Range ").SetValue(new Slider(835, 0, 850)));

            Menu.AddItem(new MenuItem("QMana", "Mana %").SetValue(new Slider(45, 0, 100)));

            qLogic = new QLogic();
        }

        private void BarrelRoll()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item("QRange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (target.HasBuffOfType(BuffType.Knockback)) return;

            if (qLogic.CanThrowQ())
            {
                Variable.Spells[SpellSlot.Q].Cast(qLogic.QPred(target));
            }

            if (qLogic.CanExplodeQ(target))
            {
                Variable.Spells[SpellSlot.Q].Cast();
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("QMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            BarrelRoll();
        }

        #endregion
    }
}