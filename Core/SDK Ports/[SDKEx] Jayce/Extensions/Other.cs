// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The other.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Jayce.Extensions
{
    #region

    using static Config;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using static Spells;

    #endregion

    /// <summary>
    ///     The other.
    /// </summary>
    internal class Other
    {
        #region Static Fields

        /// <summary>
        ///     The cannon E cd.
        /// </summary>
        public static float CannonE_CD;

        /// <summary>
        ///     The cannon E.
        /// </summary>
        public static float CannonE_CD_R;

        /// <summary>
        ///     The cannon E true cd.
        /// </summary>
        public static float[] CannonE_TrueCD = { 16, 16, 16, 16, 16, 16 };

        /// <summary>
        ///     The cannon Q cd.
        /// </summary>
        public static float CannonQ_CD;

        /// <summary>
        ///     The cannon Q.
        /// </summary>
        public static float CannonQ_CD_R;

        /// <summary>
        ///     The cannon Q true cd.
        /// </summary>
        public static float[] CannonQ_TrueCD = { 8, 8, 8, 8, 8, 8 };

        /// <summary>
        ///     The cannon W cd.
        /// </summary>
        public static float CannonW_CD;

        /// <summary>
        ///     The cannon W.
        /// </summary>
        public static float CannonW_CD_R;

        /// <summary>
        ///     The cannon W true cd.
        /// </summary>
        public static float[] CannonW_TrueCD = { 13, 11.4f, 9.8f, 8.2f, 6.6f, 5 };

        /// <summary>
        ///     The hammer E cd.
        /// </summary>
        public static float HammerE_CD;

        /// <summary>
        ///     The hammer E.
        /// </summary>
        public static float HammerE_CD_R;

        /// <summary>
        ///     The hammer E true cd.
        /// </summary>
        public static float[] HammerE_TrueCD = { 15, 14, 13, 12, 11, 10 };

        /// <summary>
        ///     The hammer Q cd.
        /// </summary>
        public static float HammerQ_CD;

        /// <summary>
        ///     The hammer Q.
        /// </summary>
        public static float HammerQ_CD_R;

        /// <summary>
        ///     The hammer Q true cd.
        /// </summary>
        public static float[] HammerQ_TrueCD = { 16, 14, 12, 10, 8, 6 };

        /// <summary>
        ///     The hammer W cd.
        /// </summary>
        public static float HammerW_CD;

        /// <summary>
        ///     The hammer W.
        /// </summary>
        public static float HammerW_CD_R;

        /// <summary>
        ///     The hammer W true cd.
        /// </summary>
        public static float[] HammerW_TrueCD = { 10, 10, 10, 10, 10, 10 };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The cannon Q dmg.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double CannonQDmg(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamage(
                target,
                DamageType.Physical,
                new float[] { 0, 40, 80, 120, 160, 200, 240 }[Q.Level]
                + 1.2f * ObjectManager.Player.FlatPhysicalDamageMod);
        }

        /// <summary>
        /// The cannon QE dmg.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double CannonQEDmg(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamage(
                target,
                DamageType.Physical,
                new float[] { 0, 70, 120, 170, 220, 270, 320 }[Q.Level]
                + 1.2f * ObjectManager.Player.FlatPhysicalDamageMod);
        }

        /// <summary>
        ///     The CD.
        /// </summary>
        public static void CD()
        {
            HammerQ_CD_R = HammerQ_CD - Game.Time > 0 ? HammerQ_CD - Game.Time : 0;
            HammerW_CD_R = HammerW_CD - Game.Time > 0 ? HammerW_CD - Game.Time : 0;
            HammerE_CD_R = HammerE_CD - Game.Time > 0 ? HammerE_CD - Game.Time : 0;

            CannonQ_CD_R = CannonQ_CD - Game.Time > 0 ? CannonQ_CD - Game.Time : 0;
            CannonW_CD_R = CannonW_CD - Game.Time > 0 ? CannonW_CD - Game.Time : 0;
            CannonE_CD_R = CannonE_CD - Game.Time > 0 ? CannonE_CD - Game.Time : 0;
        }

        /// <summary>
        /// The combo damage.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ComboDamage(Obj_AI_Base target)
        {
            double damage = 0;
            if ((CannonQ_CD_R == 0) && (CannonE_CD_R != 0)) damage += CannonQDmg(target);
            if ((CannonQ_CD_R == 0) && (CannonE_CD_R == 0)) damage += CannonQEDmg(target);
            if (HammerQ_CD_R == 0) damage += Q1.GetDamage(target);
            if (HammerW_CD_R == 0) damage += W1.GetDamage(target);
            if (HammerE_CD_R == 0) damage += HammerEDmg(target);
            return damage;
        }

        /// <summary>
        /// The hammer E dmg.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double HammerEDmg(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamage(
                target,
                DamageType.Physical,
                new[] { 0, 8f, 10.4f, 12.8f, 15.2f, 17.6f, 20f }[E.Level] * (target.MaxHealth / 100)
                + ObjectManager.Player.FlatPhysicalDamageMod);
        }

        /// <summary>
        ///     The range form.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool RangeForm()
        {
            return ObjectManager.Player.HasBuff("jaycestancegun");
        }

        /// <summary>
        /// The calc real cd.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float RealCD(float time)
        {
            return time + time * ObjectManager.Player.PercentCooldownMod;
        }

        /// <summary>
        /// The skin changer.
        /// </summary>
        public static void SkinChanger()
        {
            //ObjectManager.//Player.SetSkin(ObjectManager.Player.BaseSkinName, SkinChangerM.Index);
        }

        #endregion
    }
}