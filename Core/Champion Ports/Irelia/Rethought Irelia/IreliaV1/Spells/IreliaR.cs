using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.Spells
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using Rethought_Irelia.IreliaV1.DamageCalculator;

    #endregion

    internal class IreliaR : SpellChild, IDamageCalculatorModule
    {
        #region Public Properties

        public int EstimatedAmountInOneCombo { get; } = 4;

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Transcendent Blades";

        /// <summary>
        ///     Gets or sets the spell.
        /// </summary>
        /// <value>
        ///     The spell.
        /// </value>
        public override Spell Spell { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the damage.
        /// </summary>
        /// <param name="target">The get damage.</param>
        /// <returns></returns>
        public float GetDamage(Obj_AI_Base target)
        {
            return !this.Spell.IsReady() ? 0 : this.Spell.GetDamage(target);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.Spell = new Spell(SpellSlot.R, 1000);
            this.Spell.SetSkillshot(0.25f, 45, 1600, false, SkillshotType.SkillshotLine);
        }

        /// <summary>
        ///     Sets the switch.
        /// </summary>
        protected override void SetSwitch()
        {
            this.Switch = new UnreversibleSwitch(this.Menu);
        }

        #endregion
    }
}