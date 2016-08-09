using EloBuddy; namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using Prediction = SPrediction.Prediction;

    #endregion

    internal class CrescentStrike : ChildBase
    {
        
        #region Fields

        /// <summary>
        ///     The logic
        /// </summary>
        private LogicAll logic;

        /// <summary>
        ///     The q logic
        /// </summary>
        private CrescentStrikeLogic qLogic;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public sealed override string Name { get; set; } = "[Q] Crescent Strike";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUpdate(EventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variables.Spells[SpellSlot.Q].IsReady()) return;

            if (this.Menu.Item(this.Menu.Name + "QMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            this.Crescent();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate -= this.OnUpdate;
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Events.OnUpdate += this.OnUpdate;
        }

        /// <summary>
        ///     Called when [initialize].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.logic = new LogicAll();
            this.qLogic = new CrescentStrikeLogic();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            if (Variables.Spells != null)
            {
                Variables.Spells[SpellSlot.Q].SetSkillshot(0.25f, 185, 1600, false, SkillshotType.SkillshotCone);
            }

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "QRange", "Q Range ").SetValue(new Slider(820, 0, 825)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "QMana", "Mana %").SetValue(new Slider(10, 0, 100)));

            Prediction.Initialize(this.Menu);
        }

        /// <summary>
        ///     Crescents this instance.
        /// </summary>
        private void Crescent()
        {
            var target = TargetSelector.GetTarget(
                this.Menu.Item(this.Menu.Name + "QRange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            Variables.Spells[SpellSlot.Q].Cast(this.qLogic.QPred(target));
        }

        #endregion
    }
}