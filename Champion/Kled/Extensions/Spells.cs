using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Hiki.Kled.Extensions
{
    internal static class Spells
    {
        public static Spell Q, SkaarlQ, W, E, R;

        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 700);
            SkaarlQ = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, ObjectManager.Player.AttackRange);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 5000);

            Q.SetSkillshot(0.2047f, 40, 3000, true, SkillshotType.SkillshotCone);
            SkaarlQ.SetSkillshot(0.2804f, 45, 1600, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.1000f, 10, 500, false, SkillshotType.SkillshotLine);

        }

    }
}
