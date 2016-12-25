// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The jungle clear.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Jayce.Modes
{
    #region

    using System.Linq;

    using static Extensions.Config;
    using static Extensions.Other;
    using static Extensions.Spells;

    using LeagueSharp;
    using LeagueSharp.SDK;

    #endregion

    /// <summary>
    ///     The jungle clear.
    /// </summary>
    internal class JungleClear
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        public static void Execute()
        {
            if ((ObjectManager.Player.ManaPercent >= JungleMana.SValue) && JungleMana.BValue)
                if (RangeForm())
                {
                    if (JungleCannonQ) CastQRange();
                    if (JungleCannonW) CastWRange();
                }
                else
                {
                    if (JungleHammerQ) CastQMelee();
                    if (JungleHammerW) CastWMelee();
                    if (JungleHammerE) CastEMelee();
                }
            else if (!JungleMana.BValue)
                if (RangeForm())
                {
                    if (JungleCannonQ) CastQRange();
                    if (JungleCannonW) CastWRange();
                }
                else
                {
                    if (JungleHammerQ) CastQMelee();
                    if (JungleHammerW) CastWMelee();
                    if (JungleHammerE) CastEMelee();
                }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The cast E melee.
        /// </summary>
        private static void CastEMelee()
        {
            var Minions = GameObjects.JungleLarge.Where(x => x.IsValidTarget(E1.Range));

            foreach (var Minion in Minions) if (E1.IsReady()) E1.Cast(Minion);
        }

        /// <summary>
        ///     The cast Q melee.
        /// </summary>
        private static void CastQMelee()
        {
            var Minions = GameObjects.JungleLarge.Where(x => x.IsValidTarget(Q1.Range));

            foreach (var Minion in Minions) if (Q1.IsReady()) Q1.Cast(Minion);
        }

        /// <summary>
        ///     The cast Q range.
        /// </summary>
        private static void CastQRange()
        {
            var Minions = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range));

            foreach (var Minion in Minions) if (Q.IsReady()) Q.Cast(Minion);
        }

        /// <summary>
        ///     The cast W melee.
        /// </summary>
        private static void CastWMelee()
        {
            var Minions = GameObjects.JungleLarge.Where(x => x.IsValidTarget(W1.Range));

            foreach (var Minion in Minions) if (W1.IsReady()) W1.Cast();
        }

        /// <summary>
        ///     The cast W range.
        /// </summary>
        private static void CastWRange()
        {
            var Minions = GameObjects.JungleLarge.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange));

            foreach (var Minion in Minions) if (W.IsReady()) W.Cast();
        }

        #endregion
    }
}