using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Hikigaya_Lux.Core;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hikigaya_Lux.Core
{
    class Spells
    {
        public static Spell Q, W, E, R;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 1150);
            W = new Spell(SpellSlot.W, 1075);
            E = new Spell(SpellSlot.E, 1100);
            R = new Spell(SpellSlot.R, 3400);

            Q.SetSkillshot(0.25f, 110f, 1300f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 110f, 1200f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 275f, 1050f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1.1f, 190f, 3000f, false, SkillshotType.SkillshotLine);
        }
    }
}
