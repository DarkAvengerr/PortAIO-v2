using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Utility
{
    #region Using Directives

    using System;

    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.SDK;

    #endregion

    public class Helper
    {
        #region Fields

        /// <summary>
        ///     The E logicprovicer
        /// </summary>
        public SweepingBladeLogicProvider ProviderE = new SweepingBladeLogicProvider();

        #endregion

        #region Methods

        /// <summary>
        ///     Returns Spell Width based on Spell Name
        /// </summary>
        /// <param name="spellName"></param>
        /// <returns>float</returns>
        internal static float GetSpellWidth(string spellName)
        {
            if (spellName == "YasuoWMovingWall")
            {
                return (250 + (50 * GlobalVariables.Spells[SpellSlot.W].Level));
            }
            if (spellName == "YasuoQ")
            {
                return 20;
            }
            if (spellName == "YasuoQ2")
            {
                return 90;
            }
            return spellName != null ? SpellDatabase.GetByName(spellName).Width : 0;
        }



        #endregion
    }
}