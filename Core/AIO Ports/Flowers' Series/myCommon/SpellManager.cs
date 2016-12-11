using EloBuddy; 
using LeagueSharp.Common; 
namespace ADCCOMMON
{
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class SpellManager
    {
        public static void PredCast(Spell spell, Obj_AI_Base target, bool isAOE = false)
        {
            if (!spell.IsReady())
            {
                return;
            }

            var pred = spell.GetPrediction(target, isAOE);

            if (pred.Hitchance >= HitChance.VeryHigh)
            {
                spell.Cast(pred.CastPosition, true);
            }
        }
    }
}
