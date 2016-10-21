using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Spells
{
    class CorkiSpells
    {
        public static Spell Q, W, E, R,BIG;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 1300);
            BIG = new Spell(SpellSlot.R, 1500);

            Q.SetSkillshot(0.50f, 250f, 1135f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 450, 1200, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0f, (float)(45 * Math.PI / 180), 1500, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
            BIG.SetSkillshot(0.25f, 100f, 2000f, true, SkillshotType.SkillshotLine);

        }
    }
}
