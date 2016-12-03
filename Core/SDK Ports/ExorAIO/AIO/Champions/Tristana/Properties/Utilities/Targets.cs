using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Tristana
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The targets class.
    /// </summary>
    internal class Targets
    {
        #region Public Properties

        /// <summary>
        ///     The jungle minion targets.
        /// </summary>
        public static List<Obj_AI_Minion> JungleMinions
            =>
                GameObjects.Jungle.Where(
                    m =>
                    m.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())
                    && (!GameObjects.JungleSmall.Contains(m) || m.BaseSkinName.Equals("Sru_Crab"))).ToList();

        /// <summary>
        ///     The minions target.
        /// </summary>
        public static List<Obj_AI_Minion> Minions
            =>
                GameObjects.EnemyMinions.Where(
                    m => m.IsMinion() && m.IsValidTarget(GameObjects.Player.GetRealAutoAttackRange())).ToList();

        /// <summary>
        ///     The main hero target.
        /// </summary>
        public static AIHeroClient Target
            => Variables.TargetSelector.GetTarget(GameObjects.Player.GetRealAutoAttackRange(), DamageType.Physical);

        #endregion
    }
}