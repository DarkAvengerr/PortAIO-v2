// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The spells.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Jayce.Extensions
{
    #region

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    using SharpDX;

    #endregion

    /// <summary>
    ///     The spells.
    /// </summary>
    internal class Spells
    {
        #region Static Fields

        /// <summary>
        ///     The Cannon E.
        /// </summary>
        public static Spell E;

        /// <summary>
        ///     The Hammer E.
        /// </summary>
        public static Spell E1;

        /// <summary>
        ///     The gate pos.
        /// </summary>
        public static Vector3 GatePos;

        /// <summary>
        ///     The Cannon Q.
        /// </summary>
        public static Spell Q;

        /// <summary>
        ///     The Hammer Q.
        /// </summary>
        public static Spell Q1;

        /// <summary>
        ///     The QE.
        /// </summary>
        public static Spell QE;

        /// <summary>
        ///     The Cannon R.
        /// </summary>
        public static Spell R;

        /// <summary>
        ///     The Hammer R.
        /// </summary>
        public static Spell R1;

        /// <summary>
        ///     The Cannon W.
        /// </summary>
        public static Spell W;

        /// <summary>
        ///     The Hammer W.
        /// </summary>
        public static Spell W1;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 1050);
            QE = new Spell(SpellSlot.Q, 1600);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R);

            Q1 = new Spell(SpellSlot.Q, 600);
            W1 = new Spell(SpellSlot.W, 285);
            E1 = new Spell(SpellSlot.E, 240);
            R1 = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.3f, 70f, 1500, true, SkillshotType.SkillshotLine);
            QE.SetSkillshot(0.3f, 70f, 2100, true, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}