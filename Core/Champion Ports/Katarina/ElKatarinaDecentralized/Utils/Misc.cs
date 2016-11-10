using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElKatarinaDecentralized.Utils
{
    using System;
    using System.Linq;

    using ElKatarinaDecentralized.Components.Spells;
    using ElKatarinaDecentralized.Enumerations;

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
        ///     Spell E
        /// </summary>
        public static SpellE SpellE;

        /// <summary>
        ///     Spell R
        /// </summary>
        public static SpellR SpellR;

        /// <summary>
        ///     Gets the auto attack range.
        /// </summary>
        internal static float KatarinaAutoAttackRange
        {
            get { return Orbwalking.GetRealAutoAttackRange(ObjectManager.Player); }
        }

        /// <summary>
        ///     Checks if spell is castable.
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="target"></param>
        /// <param name="ks"></param>
        /// <param name="checkKillable"></param>
        /// <returns></returns>
        public static bool IsCastable(this Spell spell, Obj_AI_Base target, bool ks = false, bool checkKillable = true)
        {
            return spell.CanCast(target) && (!checkKillable || spell.IsKillable(target));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether player has the ultimate buff.
        /// </summary>
        public static bool HasUltimate => ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("katarinar"));

        /// <summary>
        ///     The R damage.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static float GetCalculatedRDamage(this Obj_AI_Base target, int ticks)
        {
            var dmg = ObjectManager.Player.GetDamageSpell(target, SpellSlot.R).CalculatedDamage;
            return (float)(dmg * ticks);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static bool LastCastedDelay(this Spell spell, int delay)
        {
            var casted = ObjectManager.Player.LastCastedspell();
            return casted != null && casted.Name == spell.Instance.Name && Utils.TickCount - casted.Tick < delay;
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
                Logging.AddEntry(LoggingEntryTrype.Error, "@Misc.cs: Can not return target - {0}", e);
                throw;
            }
        }

        #endregion
    }
}