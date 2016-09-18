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

        private readonly Orbwalking.Orbwalker orbwalker;

        public CrescentStrike(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

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
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variables.Spells[SpellSlot.Q].IsReady()) return;

            if (Menu.Item(Menu.Name + "QMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            Crescent();
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
            Game.OnUpdate -= OnUpdate;
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        /// <summary>
        ///     Called when [initialize].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="FeatureBaseEventArgs" /> instance containing the event data.</param>
        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    logic = new LogicAll();
        //    qLogic = new CrescentStrikeLogic();
        //    base.OnLoad(sender, featureBaseEventArgs);
        //}

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

            Menu.AddItem(new MenuItem(Menu.Name + "QRange", "Q Range ").SetValue(new Slider(820, 0, 825)));

            Menu.AddItem(new MenuItem(Menu.Name + "QMana", "Mana %").SetValue(new Slider(10, 0, 100)));

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
                Menu.Item(Menu.Name + "QRange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            Variables.Spells[SpellSlot.Q].Cast(qLogic.QPred(target));
        }

        #endregion
    }
}