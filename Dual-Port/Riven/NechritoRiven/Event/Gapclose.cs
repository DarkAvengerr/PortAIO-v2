using LeagueSharp.Common;
using NechritoRiven.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event
{
    internal class Gapclose : Core.Core
    {
        public static void Gapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;
            if (t.IsEnemy && Spells.W.IsReady() && t.IsValidTarget() && !t.IsZombie)
            {
                if (t.IsValidTarget(Spells.W.Range + t.BoundingRadius))
                {
                    Spells.W.Cast(t);
                }
            }
        }
    }
}
