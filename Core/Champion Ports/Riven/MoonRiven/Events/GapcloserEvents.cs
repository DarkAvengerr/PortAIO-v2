using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using LeagueSharp.Common;

    internal class GapcloserEvents : Logic
    {
        internal static void Init()
        {
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private static void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (!MenuInit.AntiGapcloserW || !W.IsReady() || Args.Sender == null)
            {
                return;
            }

            if (Args.Sender.IsValidTarget(W.Range) || Args.End.DistanceToPlayer() <= W.Range)
            {
                W.Cast(true);
            }
        }
    }
}