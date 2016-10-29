using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    using Prediction = SPrediction.Prediction;

    #endregion

    internal class CrescentStrike : OrbwalkingChild
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
            if (!CheckGuardians())
            {
                return;
            }

            if (Menu.Item("QMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            Crescent();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            if (Variables.Spells != null)
            {
                Variables.Spells[SpellSlot.Q].SetSkillshot(0.25f, 185, 1600, false, SkillshotType.SkillshotCone);
            }

            Menu.AddItem(new MenuItem("QRange", "Q Range ").SetValue(new Slider(820, 0, 825)));

            Menu.AddItem(new MenuItem("QMana", "Mana %").SetValue(new Slider(10, 0, 100)));

            qLogic = new CrescentStrikeLogic();
            logic = new LogicAll();
            Prediction.Initialize(Menu);
        }

        /// <summary>
        ///     Crescents this instance.
        /// </summary>
        private void Crescent()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item("QRange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null)
            {
                return;
            }

            Variables.Spells[SpellSlot.Q].Cast(qLogic.QPred(target));
        }

        #endregion
    }
}