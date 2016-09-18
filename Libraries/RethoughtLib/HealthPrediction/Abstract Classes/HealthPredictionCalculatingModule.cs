using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.HealthPrediction.Abstract_Classes
{
    #region Using Directives

    using System;

    using LeagueSharp;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.HealthPrediction.Implementations;

    #endregion

    public abstract class HealthPredictionObserverModule : ChildBase
    {
        #region Fields

        /// <summary>
        ///     The health prediction output
        /// </summary>
        protected HealthPredictionOutput HealthPredictionOutput = new HealthPredictionOutput();

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when [on observed].
        /// </summary>
        public event EventHandler<HealthPredictionObserverEventArgs> OnObserved;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public virtual HealthPredictionOutput Get(Obj_AI_Base obj)
        {
            return this.HealthPredictionOutput;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [on observed].
        /// </summary>
        /// <param name="observedResult">The observed result.</param>
        protected virtual void OnOnObserved(HealthPredictionOutput observedResult)
        {
            this.OnObserved?.Invoke(this, new HealthPredictionObserverEventArgs(observedResult));
        }

        #endregion
    }

    public class HealthPredictionObserverEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HealthPredictionObserverEventArgs" /> class.
        /// </summary>
        /// <param name="output">The output.</param>
        public HealthPredictionObserverEventArgs(HealthPredictionOutput output)
        {
            this.Output = output;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the output.
        /// </summary>
        /// <value>
        ///     The output.
        /// </value>
        public HealthPredictionOutput Output { get; set; }

        #endregion
    }
}