using KoreanCommon;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAkali
{
    public static class Spells
    {
        public static void Load(CommonChampion akali)
        {
            var Q = new CommonSpell(SpellSlot.Q, 580f, TargetSelector.DamageType.Magical);
            var W = new CommonSpell(SpellSlot.W, 680f, TargetSelector.DamageType.Magical);
            var E = new CommonSpell(SpellSlot.E, 295f, TargetSelector.DamageType.Physical);
            var R = new CommonSpell(SpellSlot.R, 680f, TargetSelector.DamageType.Magical);

            Q.SetTargetted(0.317f, 1000f);
            R.SetTargetted(0.1f, float.MaxValue);

            akali.Spells.AddSpell(Q);
            akali.Spells.AddSpell(W);
            akali.Spells.AddSpell(E);
            akali.Spells.AddSpell(R);
        }
    }
}