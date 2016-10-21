using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CjShuJinx.Champion
{
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class PlayerSpells
    {
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q, W, E, R;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q);

            W = new Spell(SpellSlot.W, 1490f);
            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 900f);
            E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 25000f);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);

            SpellList.AddRange(new[] { Q, W, E, R });
        }

        public static void CastQObjects(Obj_AI_Base t)
        {
            if (!Q.CanCast(t))
            {
                return;
            }
                Q.CastOnUnit(t);
        }

        public static void CastQCombo(Obj_AI_Base t)
        {
            if (!Q.CanCast(t))
            {
                return;
            }
        }
    }
}