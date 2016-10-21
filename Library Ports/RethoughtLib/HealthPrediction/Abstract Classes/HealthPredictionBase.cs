using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.HealthPrediction.Abstract_Classes
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    using RethoughtLib.HealthPrediction.Implementations;

    #endregion

    public abstract class HealthPredictionBase
    {
        #region Fields

        /// <summary>
        ///     The prediction modules
        /// </summary>
        protected List<HealthPredictionObserverModule> PredictionModules = new List<HealthPredictionObserverModule>();

        protected List<HealthPredictionOutput> PredictionOutputs = new List<HealthPredictionOutput>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the prediction module.
        /// </summary>
        /// <param name="observerModule">The calculating module.</param>
        public virtual void AddPredictionModule(HealthPredictionObserverModule observerModule)
        {
            this.PredictionModules.Add(observerModule);
        }

        /// <summary>
        ///     Predicts the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="time">The time.</param>
        /// <param name="predictionModules">The prediction modules.</param>
        /// <returns></returns>
        public abstract IEnumerable<HealthPredictionOutput> Predict(
            Obj_AI_Base obj,
            float time,
            List<HealthPredictionObserverModule> predictionModules);

        /// <summary>
        ///     Removes the prediction module.
        /// </summary>
        /// <param name="observerModule">The calculating module.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public virtual void RemovePredictionModule(HealthPredictionObserverModule observerModule)
        {
            if (!this.PredictionModules.Contains(observerModule))
            {
                throw new InvalidOperationException(
                    $"{this.PredictionModules} does not contain {observerModule}, therefore it can't get removed");
            }

            this.PredictionModules.Remove(observerModule);
        }

        #endregion
    }
}