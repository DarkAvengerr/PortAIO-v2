using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.JungleClear
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class QJungle : ChildBase
    {
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

            Menu.AddItem(new MenuItem("QOverkill", "Overkill Check").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.Q].IsReady() || Variable.Player.Spellbook.IsAutoAttacking) return;

            RangersFocus();
        }

        private void RangersFocus()
        {
            var mobs =
                MinionManager.GetMinions(
                    Variable.Player.AttackRange,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null 
                || (Menu.Item("QOverkill").GetValue<bool>()
                && mobs.Health < Variable.Player.GetAutoAttackDamage(mobs) * 2 
                && Variable.Player.HealthPercent >= 13)) return;

            Variable.Spells[SpellSlot.Q].Cast();
        }

        #endregion
    }
}