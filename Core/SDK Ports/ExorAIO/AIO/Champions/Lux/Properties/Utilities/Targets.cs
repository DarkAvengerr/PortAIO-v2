using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Lux
{
    using System.Collections.Generic;
    using System.Linq;

    using ExorAIO.Utilities;

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
        ///     The jungle minions hit by the E missile.
        /// </summary>
        public static List<Obj_AI_Minion> EJungleMinions
            => JungleMinions.Where(m => m.Distance(Lux.EMissile.Position) < Vars.E.Width).ToList();

        /// <summary>
        ///     The minions hit by the E missile.
        /// </summary>
        public static List<Obj_AI_Minion> EMinions
            => Minions.Where(m => m.Distance(Lux.EMissile.Position) < Vars.E.Width).ToList();

        /// <summary>
        ///     The jungle minion targets.
        /// </summary>
        public static List<Obj_AI_Minion> JungleMinions
            =>
                GameObjects.Jungle.Where(
                    m =>
                    m.IsValidTarget(Vars.E.Range)
                    && (!GameObjects.JungleSmall.Contains(m) || m.BaseSkinName.Equals("Sru_Crab"))).ToList();

        /// <summary>
        ///     The lowest ally in range.
        /// </summary>
        public static AIHeroClient LowestAlly
            =>
                GameObjects.AllyHeroes.Where(a => !a.IsMe && a.IsValidTarget(Vars.W.Range, false))
                    .OrderBy(o => o.Health)
                    .LastOrDefault();

        /// <summary>
        ///     The minions target.
        /// </summary>
        public static List<Obj_AI_Minion> Minions
            => GameObjects.EnemyMinions.Where(m => m.IsMinion() && m.IsValidTarget(Vars.E.Range)).ToList();

        /// <summary>
        ///     The main hero target.
        /// </summary>
        public static AIHeroClient Target => Variables.TargetSelector.GetTarget(Vars.R.Range, DamageType.Magical);

        #endregion
    }
}