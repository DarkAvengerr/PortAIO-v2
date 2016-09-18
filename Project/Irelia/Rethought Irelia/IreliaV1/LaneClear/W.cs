using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.LaneClear
{
    #region Using Directives

    using System.Linq;

    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;

    using Rethought_Irelia.IreliaV1.Spells;

    #endregion

    internal class W : OrbwalkingChild
    {
        #region Fields

        /// <summary>
        ///     The irelia w
        /// </summary>
        private readonly IreliaW ireliaW;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="W" /> class.
        /// </summary>
        /// <param name="ireliaW">The irelia w.</param>
        public W(IreliaW ireliaW)
        {
            this.ireliaW = ireliaW;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "W";

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Orbwalking.BeforeAttack -= this.OrbwalkingOnBeforeAttack;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Orbwalking.BeforeAttack += this.OrbwalkingOnBeforeAttack;
        }

        /// <summary>
        ///     Triggers on before attack
        /// </summary>
        /// <param name="args">The <see cref="Orbwalking.BeforeAttackEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!this.CheckGuardians()) return;

            if (MinionManager.GetMinions(1000, MinionTypes.All, MinionTeam.NotAlly).Count > 3)
            {
                this.ireliaW.Spell.Cast();
            }

            if (MinionManager.GetMinions(400, MinionTypes.All, MinionTeam.Neutral).Any(x => x.HealthPercent > 20))
            {
                this.ireliaW.Spell.Cast();
            }
        }

        #endregion
    }
}