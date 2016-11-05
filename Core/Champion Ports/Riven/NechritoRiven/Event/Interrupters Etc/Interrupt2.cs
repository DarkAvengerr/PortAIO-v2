using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.Interrupters_Etc
{
    #region

    using LeagueSharp;
    using LeagueSharp.Common;

    using NechritoRiven.Core;

    #endregion

    internal class Interrupt2 : Core
    {
        #region Public Methods and Operators

        public static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!MenuConfig.InterruptMenu || !sender.IsEnemy || !sender.IsValidTarget(Spells.W.Range) || sender.HasBuff("FioraW"))
            {
                return;
            }

            if (Spells.W.IsReady())
            {
                Spells.W.Cast(sender);
            }

            if (Qstack != 3)
            {
                return;
            }

            Spells.Q.Cast(sender);
        }

        #endregion
    }
}