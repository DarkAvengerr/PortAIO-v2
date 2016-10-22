// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Misc.cs" company="LeagueSharp">
//   legacy@joduska.me
// </copyright>
// <summary>
//   The misc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace AniviaSharp.Utils
{
    using System;
    using System.Linq;

    using AniviaSharp.Enumerations;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The misc.
    /// </summary>
    internal static class Misc
    {
        #region Methods

        /// <summary>
        ///     Gets a target from the common target selector.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="damageType">
        ///     The damage type.
        /// </param>
        /// <returns>
        ///     <see cref="AIHeroClient" />
        /// </returns>
        internal static AIHeroClient GetTarget(float range, TargetSelector.DamageType damageType)
        {
            try
            {
                return TargetSelector.GetTarget(range, damageType);
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@Misc.cs: Can not return target - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The has buff until.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <param name="displayName">
        ///     The display name.
        /// </param>
        /// <param name="tickCount">
        ///     The tick count.
        /// </param>
        /// <param name="includePing">
        ///     The include ping.
        /// </param>
        /// <returns>
        ///     <see cref="bool" />
        /// </returns>
        internal static bool HasBuffUntil(
            this Obj_AI_Base unit, 
            string displayName, 
            float tickCount, 
            bool includePing = true)
        {
            try
            {
                return
                    unit.Buffs.Any(
                        buff =>
                        buff.IsValid
                        && string.Equals(buff.DisplayName, displayName, StringComparison.CurrentCultureIgnoreCase)
                        && buff.EndTime - Game.Time > tickCount - (includePing ? (Game.Ping / 2000f) : 0));
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@Misc.cs: Can not execute HasBuffUntil - {0}", e);
                throw;
            }
        }

        #endregion
    }
}