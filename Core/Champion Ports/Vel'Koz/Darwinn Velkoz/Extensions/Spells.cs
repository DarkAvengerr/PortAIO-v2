using LeagueSharp;
using LeagueSharp.Common;
using System;


using EloBuddy; 
using LeagueSharp.Common; 
namespace Darwinn_s_velkoz.Extensions
{
    internal static class Spells
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell QSplit;
        public static Spell QDummy;


        public static void Initialize()
        {

            Q = new Spell(SpellSlot.Q, 1180);
            QSplit = new Spell(SpellSlot.Q, 1000);
            QDummy = new Spell(SpellSlot.Q, (float)Math.Sqrt(Math.Pow(Q.Range, 2) + Math.Pow(QSplit.Range, 2)));
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 1500);

            Q.SetSkillshot(0.25f, 70f, 1300f, true, SkillshotType.SkillshotLine);
            QSplit.SetSkillshot(0.1f, 70f, 2100f, true, SkillshotType.SkillshotLine);
            QDummy.SetSkillshot(0.5f, 55f, 1200, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 85f, 1700f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(1f, 180f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.1f, 80f, float.MaxValue, false, SkillshotType.SkillshotLine);

        }

    }
}