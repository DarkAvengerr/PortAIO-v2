using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista
{
    static class KalistaExtensions
    {
        public static bool HasRend(this Obj_AI_Base target)
        {
            return target.GetRendBuff() != null;
        }

        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return target.Buffs.FirstOrDefault(b => b.Caster.IsMe && b.LSIsValidBuff() && b.DisplayName == "KalistaExpungeMarker");
        }

        public static bool IsRendAboutToExpire(this Obj_AI_Base target)
        {
            return target.GetRendBuff().EndTime - Game.Time < 0.35f;
        }
    }
}
