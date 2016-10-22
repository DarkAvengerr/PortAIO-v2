using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.HealthPrediction.Implementations
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.HealthPrediction.Abstract_Classes;

    #endregion

    internal class HealthPrediction : HealthPredictionBase
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the detailed report.
        /// </summary>
        /// <value>
        ///     The detailed report.
        /// </value>
        public List<HealthPredictionOutput> DetailedReport { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Predicts the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="time"></param>
        /// <param name="predictionModules">The prediction modules.</param>
        /// <returns></returns>
        public override IEnumerable<HealthPredictionOutput> Predict(
            Obj_AI_Base obj,
            float time,
            List<HealthPredictionObserverModule> predictionModules = null)
        {
            if (predictionModules != null)
            {
                this.PredictionModules = predictionModules;
            }

            foreach (var output in this.PredictionOutputs)
            {
                this.Update(output);

                yield return output;
            }
        }

        public void Update(HealthPredictionOutput outputData)
        {

        }

        #endregion
    }
}