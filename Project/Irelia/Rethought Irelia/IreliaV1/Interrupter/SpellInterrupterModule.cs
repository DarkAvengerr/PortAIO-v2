using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.Interrupter
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class SpellInterrupterModule : ChildBase
    {
        #region Fields

        private readonly List<AIHeroClient> currentlyExecuting = new List<AIHeroClient>();

        /// <summary>
        ///     The dangerlevel
        /// </summary>
        private Interrupter2.DangerLevel dangerlevel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellInterrupterModule" /> class.
        /// </summary>
        /// <param name="spells">The spells.</param>
        public SpellInterrupterModule(params SpellInterrupter[] spells)
        {
            this.Spells = spells;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Spell Interrupter";

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public SpellInterrupter[] Spells { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Interrupter2.OnInterruptableTarget -= this.OnInterruptableTarget;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Interrupter2.OnInterruptableTarget += this.OnInterruptableTarget;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            // Gets all values as Type[]
            var values = Enum.GetValues(typeof(Interrupter2.DangerLevel));

            // Converts Type[] to string[]
            var stringList = (from object value in values select value.ToString()).ToArray();

            // Puts all values into the Menu
            var minValue =
                this.Menu.AddItem(
                    new MenuItem(this.Path + "." + "minvalue", "Minimal Danger Value").SetValue(
                        new StringList(stringList, (int)Interrupter2.DangerLevel.Medium)));

            minValue.ValueChanged +=
                (o, args) =>
                    {
                        this.dangerlevel = (Interrupter2.DangerLevel)args.GetNewValue<StringList>().SelectedIndex;
                    };

            this.dangerlevel = (Interrupter2.DangerLevel)minValue.GetValue<StringList>().SelectedIndex;
        }

        /// <summary>
        ///     Called when [interruptable target].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Interrupter2.InterruptableTargetEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsAlly || sender.IsMe || args.DangerLevel < this.dangerlevel
                || this.currentlyExecuting.Contains(sender) || this.Spells.All(x => !x.Spell.IsReady())) return;

            var spellInterrupter =
                this.Spells.Where(x => x.Spell.IsReady() && x.AdditionalCondition.Invoke(sender))
                    .MaxOrDefault(x => x.Spell.Speed);

            if (spellInterrupter.Spell.Instance.SData.TargettingType == SpellDataTargetType.Unit)
            {
                spellInterrupter.Spell.Cast(sender);
            }
            else
            {
                var pred = spellInterrupter.Spell.GetPrediction(sender);

                if (pred.Hitchance < HitChance.Medium) return;

                spellInterrupter.Spell.Cast(pred.CastPosition);
            }

            this.currentlyExecuting.Add(sender);

            // just an estimate
            LeagueSharp.Common.Utility.DelayAction.Add(
                (int)
                (ObjectManager.Player.ServerPosition.Distance(sender.ServerPosition) / spellInterrupter.Spell.Speed),
                () => this.currentlyExecuting.Remove(sender));
        }

        #endregion
    }

    internal class SpellInterrupter
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellInterrupter" /> class.
        /// </summary>
        /// <param name="spell">The spell.</param>
        /// <param name="additionalCondition">The additional condition.</param>
        public SpellInterrupter(Spell spell, Func<AIHeroClient, bool> additionalCondition = null)
        {
            this.Spell = spell;

            if (additionalCondition == null) return;

            this.AdditionalCondition = additionalCondition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the additional condition.
        /// </summary>
        /// <value>
        ///     The additional condition.
        /// </value>
        public Func<AIHeroClient, bool> AdditionalCondition { get; } = hero => true;

        /// <summary>
        ///     Gets the spell.
        /// </summary>
        /// <value>
        ///     The spell.
        /// </value>
        public Spell Spell { get; }

        #endregion
    }
}