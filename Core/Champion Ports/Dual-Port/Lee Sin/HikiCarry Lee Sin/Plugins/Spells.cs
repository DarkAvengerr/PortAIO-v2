using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Lee_Sin.Plugins
{
    static class Spells
    {
        public static Spell Q, Q2, W, W2, E, E2, R;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 1100f);
            Q2 = new Spell(SpellSlot.Q, 1300);
            W = new Spell(SpellSlot.W, 700f);
            W2 = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 330f);
            E2 = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 375f);

            Q.SetSkillshot(0.25f, 65f, 1800f, true, SkillshotType.SkillshotLine);
        }
        public static bool Passive()
        {
            return ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic");
        }

        public static bool QOne(this Spell spell)
        {
            return spell.Instance.Name == "BlindMonkQOne";
        }

        public static bool QTwo(this Spell spell)
        {
            return spell.Instance.Name == "blindmonkqtwo";
        }

        public static bool WOne(this Spell spell)
        {
            return spell.Instance.Name == "BlindMonkWOne";
        }

        public static bool WTwo(this Spell spell)
        {
            return spell.Instance.Name == "blindmonketwo";
        }

        public static bool EOne(this Spell spell)
        {
            return spell.Instance.Name == "BlindMonkEOne";
        }

        public static bool ETwo(this Spell spell)
        {
            return spell.Instance.Name == "blindmonketwo";
        }
    }
}