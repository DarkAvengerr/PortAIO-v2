using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.HealthPrediction.Implementations
{
    #region Using Directives

    using System;

    using LeagueSharp;

    using SharpDX;

    #endregion

    public class HealthPredictionOutput
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HealthPredictionOutput" /> class.
        /// </summary>
        public HealthPredictionOutput()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HealthPredictionOutput" /> class.
        /// </summary>
        /// <param name="damageIncoming">The damage incoming.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="damageCalculation"></param>
        public HealthPredictionOutput(
            double damageIncoming,
            Obj_AI_Base source,
            Obj_AI_Base target,
            Vector3 start,
            Vector3 end,
            int startTime,
            float delay,
            float speed,
            Func<Obj_AI_Base, Obj_AI_Base, double> damageCalculation = null)
        {
            this.DamageIncoming = damageIncoming;
            this.Source = source;
            this.Target = target;
            this.Start = start;
            this.End = end;
            this.StartTime = startTime;
            this.Delay = delay;
            this.Speed = speed;
            this.DamageCalculation = damageCalculation;
        }

        #endregion

        #region Public Properties

        public Func<Obj_AI_Base, Obj_AI_Base, double> DamageCalculation { get; set; }

        /// <summary>
        ///     Gets or sets the damage incoming.
        /// </summary>
        /// <value>
        ///     The damage incoming.
        /// </value>
        public double DamageIncoming { get; set; }

        public float Delay { get; set; }

        /// <summary>
        ///     Gets or sets the end.
        /// </summary>
        /// <value>
        ///     The end.
        /// </value>
        public Vector3 End { get; set; }

        public GameObject GameObject { get; set; }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        /// <value>
        ///     The source.
        /// </value>
        public Obj_AI_Base Source { get; set; }

        public float Speed { get; set; }

        /// <summary>
        ///     Gets or sets the start.
        /// </summary>
        /// <value>
        ///     The start.
        /// </value>
        public Vector3 Start { get; set; }

        /// <summary>
        ///     Gets or sets the start time.
        /// </summary>
        /// <value>
        ///     The start time.
        /// </value>
        public int StartTime { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        /// <value>
        ///     The target.
        /// </value>
        public Obj_AI_Base Target { get; set; }

        public GameObject unknown { get; set; }

        #endregion
    }
}