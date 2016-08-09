using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kennen
{
    internal class Spells
    {
        public static Spell q, w, e, r;

        public static SpellDataInst ignite;

        public static void InitializeSpells()
        {
            try
            {
                q = new Spell(SpellSlot.Q, 1050);
                w = new Spell(SpellSlot.W, 750);
                e = new Spell(SpellSlot.E);
                r = new Spell(SpellSlot.R, 550);

                q.SetSkillshot(0.125f, 50, 1700, true, SkillshotType.SkillshotLine);

                ignite = ObjectManager.Player.Spellbook.GetSpell(ObjectManager.Player.GetSpellSlot("summonerdot"));
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load the spells - {0}", exception);
            }
        }
    }
}
