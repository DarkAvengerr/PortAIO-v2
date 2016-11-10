using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengarDecentralized.Utils
{
    using System;
    using System.Linq;

    using ElRengarDecentralized.Enumerations;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The misc.
    /// </summary>
    internal static class Misc
    {
        #region Methods
        
        /// <summary>
        ///     Gets the Ferocity stacks. 
        /// </summary>
        /// <returns></returns>
        internal static int GetFerocityStacks()
        {
            try
            {
                return (int)ObjectManager.Player.Mana;
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@Misc.cs: Can not return ferocity stacks - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     Gets the auto attack range.
        /// </summary>
        internal static float RengarAutoAttackRange
        {
            get { return Orbwalking.GetRealAutoAttackRange(ObjectManager.Player); }
        }

        /// <summary>
        ///     Gets the stun duration.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static float GetStunDuration(this Obj_AI_Base target)
        {
            return
                target.Buffs.Where(
                    b =>
                        b.IsActive && Game.Time < b.EndTime &&
                        (b.Type == BuffType.Charm || b.Type == BuffType.Knockback || b.Type == BuffType.Stun ||
                         b.Type == BuffType.Suppression || b.Type == BuffType.Snare))
                    .Aggregate(0f, (current, buff) => Math.Max(current, buff.EndTime)) - Game.Time;
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
                return TargetSelector.GetSelectedTarget()
                             ?? TargetSelector.GetTarget(range, damageType);
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