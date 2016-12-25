// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The Combo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Jayce.Modes
{
    #region

    using static Extensions.Config;
    using static Extensions.Other;
    using static Extensions.Spells;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    #endregion

    /// <summary>
    ///     The Combo.
    /// </summary>
    internal class Combo
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The Execute.
        /// </summary>
        public static void Execute()
        {
            if (RangeForm())
            {
                if (ComboCannonQ.Value) CastQRange();
                if (ComboCannonQ.Value && ComboCannonE.Value) CastQERange();
                if (ComboCannonW.Value) CastWRange();
            }
            else
            {
                if (ComboHammerQ.Value) CastQMelee();
                if (ComboHammerW.Value) CastWMelee();
                if (ComboHammerE.Value) CastEMelee();
            }

            CastR();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The cast E melee.
        /// </summary>
        private static void CastEMelee()
        {
            var target = Variables.TargetSelector.GetTarget(E1.Range, DamageType.Physical);

            if (target != null) if (E1.IsReady() && target.IsValidTarget(E1.Range)) E1.Cast(target);
        }

        /// <summary>
        ///     The cast QE range.
        /// </summary>
        private static void CastQERange()
        {
            var target = Variables.TargetSelector.GetTarget(QE.Range, DamageType.Physical);

            if (target != null)
                if (QE.IsReady() && E.IsReady() && target.IsValidTarget(QE.Range)
                    && (ObjectManager.Player.Mana > Q.Instance.SData.Mana + E.Instance.SData.Mana))
                {
                    var Prediction = QE.GetPrediction(target);
                    if (Prediction.Hitchance >= HitChance.VeryHigh) QE.Cast(Prediction.CastPosition);
                }
        }

        /// <summary>
        ///     The cast Q melee.
        /// </summary>
        private static void CastQMelee()
        {
            var target = Variables.TargetSelector.GetTarget(Q1.Range, DamageType.Physical);

            if (target != null) if (Q1.IsReady() && target.IsValidTarget(Q1.Range)) Q1.Cast(target);
        }

        /// <summary>
        ///     The cast Q range.
        /// </summary>
        private static void CastQRange()
        {
            var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target != null)
                if (Q.IsReady() && (!E.IsReady() || !ComboCannonE) && target.IsValidTarget(Q.Range))
                {
                    var Prediction = Q.GetPrediction(target);
                    if (Prediction.Hitchance >= HitChance.VeryHigh) Q.Cast(Prediction.CastPosition);
                }
        }

        /// <summary>
        ///     The cast R.
        /// </summary>
        private static void CastR()
        {
            var target = Variables.TargetSelector.GetTarget(Q1.Range);

            if (R.IsReady() && ComboR.Value && target.IsValidTarget())
                if (RangeForm())
                {
                    if (!Q.IsReady() && !W.IsReady() && (HammerQ_CD_R == 0) && (target.DistanceToPlayer() < Q1.Range)) R.Cast();
                }
                else
                {
                    if (!Q1.IsReady() && !W1.IsReady() && (CannonQ_CD_R == 0)
                        && ((CannonW_CD_R == 0) || (CannonE_CD_R == 0))) R.Cast();

                    if (!Q1.IsReady() && !W1.IsReady() && !E1.IsReady()) R.Cast();
                }
        }

        /// <summary>
        ///     The cast W melee.
        /// </summary>
        private static void CastWMelee()
        {
            var target = Variables.TargetSelector.GetTarget(W1.Range, DamageType.Physical);

            if (target != null) if (W1.IsReady() && target.IsValidTarget(W1.Range)) W1.Cast();
        }

        /// <summary>
        ///     The cast W range.
        /// </summary>
        private static void CastWRange()
        {
            var target = Variables.TargetSelector.GetTarget(800);

            if ((target != null) && target.IsValidTarget(Q1.Range + 100)) if (!Q.IsReady() && R.IsReady()) if (W.IsReady()) W.Cast();
        }

        #endregion
    }
}