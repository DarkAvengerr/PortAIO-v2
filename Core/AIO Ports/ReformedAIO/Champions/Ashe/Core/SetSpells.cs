using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ashe.Core
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SetSpells
    {
        #region Public Methods and Operators

        public void Load()
        {
            if (Variable.Spells != null)
            {
                Variable.Spells[SpellSlot.W].SetSkillshot(
                    0.25f,
                    (float)(57.5f * Math.PI / 180),
                    1500f,
                    true,
                    SkillshotType.SkillshotCone);

                Variable.Spells[SpellSlot.E].SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);
                Variable.Spells[SpellSlot.R].SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);
            }
        }

        #endregion
    }
}