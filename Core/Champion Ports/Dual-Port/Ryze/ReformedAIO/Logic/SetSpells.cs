using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ryze.Logic
{
    #region Using Directives

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class SetSpells
    {
        #region Public Methods and Operators

        public void Load() // Much load much wow
        {
            if (Variable.Spells == null) return;

            Variable.Spells[SpellSlot.Q].SetSkillshot(0.25f, 55f, 1400f, true, SkillshotType.SkillshotLine);
        }

        #endregion
    }
}