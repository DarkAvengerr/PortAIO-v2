using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event
{
    #region

    using LeagueSharp.Common;

    using Core;

    #endregion

    internal class Gapclose : Core
    {
        #region Public Methods and Operators

        public static void Gapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;

            if (!t.IsEnemy || !t.IsValidTarget(Spells.W.Range + t.BoundingRadius))
            {
                return;
            }

            if (Spells.W.IsReady())
            {
                Spells.W.Cast(t);
            }

            if (Qstack != 3)
            {
                return;
            }

            Spells.Q.Cast(gapcloser.End);
        }

        #endregion
    }
}