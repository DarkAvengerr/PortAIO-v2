using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Humanizer.TickTock
{

    /// <summary>
    ///     Base used for tick manager
    /// </summary>
    internal class Tick
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Tick" /> class.
        /// </summary>
        /// <param name="currentTime">
        ///     The current time.
        /// </param>
        /// <param name="min">
        ///     The minimum.
        /// </param>
        /// <param name="max">
        ///     The maximum.
        /// </param>
        public Tick(float currentTime, float min=0, float max=100)
        {
            _nextTick=currentTime;
            _minDelay=min;
            _maxDelay=max;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     Gets the maximum delay.
        /// </summary>
        /// <returns>
        /// </returns>
        public float GetMaxDelay() {return _maxDelay;}

        /// <summary>
        ///     Gets the minimum delay.
        /// </summary>
        /// <returns>
        /// </returns>
        public float GetMinDelay() {return _minDelay;}

        /// <summary>
        ///     Determines whether the specified current time is ready.
        /// </summary>
        /// <param name="currentTime">
        ///     The current time.
        /// </param>
        /// <returns>
        ///     <c> true </c> if the specified current time is ready; otherwise, <c> false </c>.
        /// </returns>
        public bool IsReady(float currentTime) {return currentTime>_nextTick;}

        /// <summary>
        ///     Sets the minimum and maximum.
        /// </summary>
        /// <param name="min">
        ///     The minimum.
        /// </param>
        /// <param name="max">
        ///     The maximum.
        /// </param>
        public void SetMinAndMax(float min, float max)
        {
            _minDelay=min;
            _maxDelay=max;
        }

        /// <summary>
        ///     Uses the tick.
        /// </summary>
        /// <param name="next">
        ///     The next.
        /// </param>
        /// <param name="currentTime">
        ///     The current time.
        /// </param>
        public void UseTick(float next, float currentTime) {_nextTick=currentTime+next;}

        #endregion Public Methods

        #region Private Fields

        /// <summary>
        ///     The maximum seed
        /// </summary>
        private float _maxDelay;

        /// <summary>
        ///     The minimum seed
        /// </summary>
        private float _minDelay;

        /// <summary>
        ///     The next tick
        /// </summary>
        private float _nextTick;

        #endregion Private Fields
    }

}