using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BlackZilean.Handlers
{
    public class SkillsHandler
    {
        public static SpellSlot IgniteSlot { get { return ObjectManager.Player.GetSpellSlot("summonerdot"); } }

        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
                                                                         {
                                                                             { SpellSlot.Q, new Spell(SpellSlot.Q, 900) },
                                                                             { SpellSlot.W, new Spell(SpellSlot.W) },
                                                                             { SpellSlot.E, new Spell(SpellSlot.E, 700) },
                                                                             { SpellSlot.R, new Spell(SpellSlot.R, 900) }
                                                                         };

        public static void Load()
        {
            Spells[SpellSlot.Q].SetSkillshot(0.30f, 210f, 2000f, false, SkillshotType.SkillshotCircle);
        }
    }
}