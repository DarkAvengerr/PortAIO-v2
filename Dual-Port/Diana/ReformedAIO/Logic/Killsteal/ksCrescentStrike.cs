using EloBuddy; namespace ReformedAIO.Champions.Diana.Logic.Killsteal
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class KsCrescentStrike : ChildBase
    {
        #region Fields

        private CrescentStrikeLogic qLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[Q] Crescent Strike";

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
            this.qLogic = new CrescentStrikeLogic();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs) // TODO Add Dmg Multiplier(?)
        {
            this.Menu.AddItem(new MenuItem(this.Menu.Name + "QRange", "Q Range ").SetValue(new Slider(820, 0, 825)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "QMana", "Mana %").SetValue(new Slider(45, 0, 100)));
        }

        private void OnUpdate(EventArgs args)
        {
            var target =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                    !x.IsDead && x.LSIsValidTarget(this.Menu.Item(this.Menu.Name + "QRange").GetValue<Slider>().Value));

            if (this.Menu.Item(this.Menu.Name + "QMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            if (target != null && target.Health < this.qLogic.GetDmg(target))
            {
                Variables.Spells[SpellSlot.Q].Cast(this.qLogic.QPred(target));
            }
        }

        #endregion
    }
}