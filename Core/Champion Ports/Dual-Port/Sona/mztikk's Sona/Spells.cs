using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Spells
    {
        internal static Spell E { get; private set; }
        internal static Spell Q { get; private set; }
        internal static Spell W { get; private set; }
        internal static Spell R { get; private set; }
        internal static Spell FlashR { get; private set; }
        internal static Spell LastSpell { get; set; }
        internal static SpellSlot LastSpellSlot { get; set; }
        internal static SpellDataInst Flash { get; private set; }

        internal static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 820);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 425);
            R = new Spell(SpellSlot.R, 875);
            FlashR = new Spell(SpellSlot.R, 875);
            FlashR.SetSkillshot(0.5f, 135f, 2400f, false, SkillshotType.SkillshotLine);

            R.SetSkillshot(0.25f, 135f, 2400f, false, SkillshotType.SkillshotLine);
            Flash = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.SData.Name == "SummonerFlash");
        }
    }
}
