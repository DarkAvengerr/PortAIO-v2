using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The resetters class.
    /// </summary>
    internal class Resetters
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Sets the resetter slots.
        /// </summary>
        public static void Initialize()
        {
            if (ObjectManager.Player.Spellbook.Spells.Any(s => AutoAttack.IsAutoAttackReset(s.Name.ToLower())))
            {
                Vars.HasAnyReset = true;
            }
        }

        #endregion
    }
}