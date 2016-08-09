using EloBuddy; namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.JungleClear
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class QJungle : ChildBase
    {
        #region Constructors and Destructors

        public QJungle(string name)
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

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "QOverkill", "Overkill Check").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !Variable.Spells[SpellSlot.Q].IsReady() || Variable.Player.Spellbook.IsAutoAttacking) return;

            this.RangersFocus();
        }

        private void RangersFocus()
        {
            var mobs =
                MinionManager.GetMinions(
                    Variable.Player.AttackRange,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null || !mobs.IsValid) return;

            if (this.Menu.Item(this.Menu.Name + "QOverkill").GetValue<bool>()
                && mobs.Health < Variable.Player.GetAutoAttackDamage(mobs) * 2 && Variable.Player.HealthPercent >= 13) return;

            Variable.Spells[SpellSlot.Q].Cast();
        }

        #endregion
    }
}