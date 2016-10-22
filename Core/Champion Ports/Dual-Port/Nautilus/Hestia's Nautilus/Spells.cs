using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HestiaNautilus
{
    internal class Spells
    {
        public static Spell q, w, e, r;

        public static SpellDataInst ignite;

        public void InitializeSpells()
        {
            try
            {
                q = new Spell(SpellSlot.Q, 1100);
                w = new Spell(SpellSlot.W);
                e = new Spell(SpellSlot.E, 525);
                r = new Spell(SpellSlot.R, 800);

                q.SetSkillshot(0.25f, 90, 2000, true, SkillshotType.SkillshotLine);

                ignite = ObjectManager.Player.Spellbook.GetSpell(ObjectManager.Player.GetSpellSlot("summonerdot"));
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load the spells - {0}", exception);
            }
        }
    }
}
