using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Nidalee
{
    class Core
    {
       /// <summary>
       /// TO DO
       /// Zhonyas Logic
       /// Passive buffs, extend pounce if target has buff
       /// Summoner Killsteal
       /// Flee Wall
       /// </summary>
        public static AIHeroClient Player => ObjectManager.Player;
        public static Orbwalking.Orbwalker Orb { get; set; }
       
         internal static bool CatForm()
        {
            return (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "Takedown" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "Pounce" ||
                Player.Spellbook.GetSpell(SpellSlot.E).Name == "Swipe");
        }
        public class Champion
        {
            public static SpellSlot Ignite;
            public static Spell Javelin { get; set; }
            public static Spell Takedown { get; set; }
            public static Spell Pounce { get; set; }
            public static Spell Swipe { get; set; }
            public static Spell Bushwack { get; set; }
            public static Spell Primalsurge { get; set; }
            public static Spell Aspect { get; set; }
          
            public static void Load()
            {
                Javelin = new Spell(SpellSlot.Q, 1500);
                Takedown = new Spell(SpellSlot.Q, 400);
                Pounce = new Spell(SpellSlot.W, 375);
                Swipe = new Spell(SpellSlot.E, 300);
                Primalsurge = new Spell(SpellSlot.E, 600);
                Bushwack= new Spell(SpellSlot.W, 875);

                Aspect = new Spell(SpellSlot.R);

                Pounce.SetSkillshot(0.50f, 400f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                Swipe.SetSkillshot(0.25f, (float)(15 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
                Bushwack.SetSkillshot(0.25f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                Javelin.SetSkillshot(0.25f, 40f, 1500f, true, SkillshotType.SkillshotLine);

                Ignite = Player.GetSpellSlot("SummonerDot");
            }
        }
    }
}

