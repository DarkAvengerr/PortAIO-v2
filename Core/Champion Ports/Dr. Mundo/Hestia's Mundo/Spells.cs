using System;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Mundo
{
    internal class Spells
    {
        public static Spell q, w, e, r;

        public static SpellDataInst ignite;

        public static void InitializeSpells()
        {
            try
            {
                q = new Spell(SpellSlot.Q, 1000);
                w = new Spell(SpellSlot.W, 325);
                e = new Spell(SpellSlot.E, 150);
                r = new Spell(SpellSlot.R);

                q.SetSkillshot(0.275f, 60, 2000, true, SkillshotType.SkillshotLine);

                ignite = ObjectManager.Player.Spellbook.GetSpell(ObjectManager.Player.GetSpellSlot("summonerdot"));
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load the spells - {0}", exception);
            }
        }

    }
}
