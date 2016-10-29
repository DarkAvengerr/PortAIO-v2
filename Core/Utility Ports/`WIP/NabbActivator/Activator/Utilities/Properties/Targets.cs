using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
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
            => GameObjects.Jungle.Where(m => m.IsValidTarget(500f) && !GameObjects.JungleSmall.Contains(m)).ToList();

        /// <summary>
        ///     The jungle minion targets.
        /// </summary>
        /// <summary>
        ///     The minions target.
        /// </summary>
        public static List<Obj_AI_Minion> Minions
            => GameObjects.AllyMinions.Where(m => m.IsMinion() && m.IsValidTarget(750f, false)).ToList();

        /// <summary>
        ///     The main enemy target.
        /// </summary>
        public static AIHeroClient Target => Variables.TargetSelector.GetTarget(1200f, DamageType.Magical);

        #endregion
    }
}