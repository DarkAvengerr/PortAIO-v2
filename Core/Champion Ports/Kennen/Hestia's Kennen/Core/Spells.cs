using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kennen.Core
{
    internal class Spells
    {
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }

        public static SpellDataInst Ignite;

        public static void Initialize()
        {
            try
            {
                Q = new Spell(SpellSlot.Q, 1000);
                W = new Spell(SpellSlot.W, 850);
                E = new Spell(SpellSlot.E);
                R = new Spell(SpellSlot.R, 500);

                Q.SetSkillshot(0.125f, 50, 1700, true, SkillshotType.SkillshotLine);

                Ignite = ObjectManager.Player.Spellbook.GetSpell(ObjectManager.Player.GetSpellSlot("summonerdot"));
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load the spells - {0}", exception);
            }
        }
    }
}
