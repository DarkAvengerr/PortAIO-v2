using EloBuddy; 
using LeagueSharp.Common; 
namespace HandicapEzreal.Utils
{
    using System;
    using System.Linq;

    using HandicapEzreal.Components.Spells;
    using HandicapEzreal.Enumerations;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The misc.
    /// </summary>
    internal static class Misc
    {
        #region Methods

        /// <summary>
        ///     Spell Q
        /// </summary>
        public static SpellQ SpellQ;

        /// <summary>
        ///     Spell W
        /// </summary>
        public static SpellW SpellW;

        /// <summary>
        ///     Spell R
        /// </summary>
        public static SpellR SpellR;

        /// <summary>
        ///     Gets the auto attack range.
        /// </summary>
        internal static float EzrealAutoAttackRange
        {
            get { return Orbwalking.GetRealAutoAttackRange(ObjectManager.Player); }
        }

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
                Logging.AddEntry(LoggingEntryType.Error, "@Misc.cs: Can not return target - {0}", e);
                throw;
            }
        }

        #endregion
    }
}