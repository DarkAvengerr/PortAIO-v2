// OBSERVATION: After some Event I don't know yet, its not accurate anymore, until Max Flow.

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.LogicProvider
{
    using System;

    using CommonEx.Utility;

    using LeagueSharp.Common;

    using SharpDX;

    // TODO
    internal class FlowLogicProvider
    {
        #region Fields

        /// <summary>
        ///     The current units
        /// </summary>
        public float TraveledDistance; 

        /// <summary>
        ///     The last position
        /// </summary>
        private Vector3 lastPosition = Vector3.Zero;

        /// <summary>
        ///     The last reset
        /// </summary>
        private float lastReset;

        #endregion

        public FlowLogicProvider()
        {
            
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Checks the flow.
        /// </summary>
        public void CheckFlow()
        {
            if ((int)GlobalVariables.Player.Mana == (int)GlobalVariables.Player.MaxMana)
            {
                this.Reset();
                return;
            }

            if (!this.lastPosition.Equals(Vector3.Zero))
            {
                this.TraveledDistance += GlobalVariables.Player.Position.Distance(this.lastPosition);
            }
            this.lastPosition = GlobalVariables.Player.Position;

            if (this.TraveledDistance >= this.GetUnitsUntilMaxFlow())
            {
                this.Reset();
            }
        }

        /// <summary>
        ///     Gets the remaining units.
        /// </summary>
        /// <returns></returns>
        public float GetRemainingUnits()
        {
            return this.GetUnitsUntilMaxFlow() - this.TraveledDistance;
        }

        /// <summary>
        ///     Gets the units until maximum flow.
        /// </summary>
        /// <returns></returns>
        public float GetUnitsUntilMaxFlow()
        {
            return GlobalVariables.Player.Level >= 13 ? 4600f : (GlobalVariables.Player.Level >= 7 ? 5200f : 5900f);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        private void Reset()
        {
            this.lastReset = GetTick();
            this.TraveledDistance = 0;
            this.lastPosition = Vector3.Zero;
        }

        private static float GetTick()
        {
            return (int)DateTime.Now.Subtract(GlobalVariables.AssemblyLoadTime).TotalMilliseconds;
        }

        #endregion
    }
}