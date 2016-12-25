// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The harass.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Jayce.Modes
{
    #region

    using static Extensions.Config;
    using static Extensions.Spells;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    #endregion

    /// <summary>
    ///     The harass.
    /// </summary>
    internal class Harass
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        public static void Execute()
        {
            if ((ObjectManager.Player.ManaPercent >= HarassMana.SValue) && HarassMana.BValue)
            {
                if (HarassCannonQ) CastQRange();
                CastQERange();
            }
            else if (!HarassMana.BValue)
            {
                if (HarassCannonQ) CastQRange();
                CastQERange();
            }
        }

        #endregion

        #region Methods

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
        ///     The cast Q range.
        /// </summary>
        private static void CastQRange()
        {
            var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target != null)
                if (Q.IsReady() && (!E.IsReady() || !HarassCannonE) && target.IsValidTarget(Q.Range))
                {
                    var Prediction = Q.GetPrediction(target);
                    if (Prediction.Hitchance >= HitChance.VeryHigh) Q.Cast(Prediction.CastPosition);
                }
        }

        #endregion
    }
}