#region Use
using System;
using System.Windows.Input;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common; 
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using Config = GrossGoreTwistedFate.Config;

    internal static class QChampions
    {

        #region Prop

        internal static int CastQTick;

        #endregion

        #region Methods

        internal static void Execute()
        {
            if (Config.UseQEnemy)
            {
                if (Spells._q.IsReadyPerfectly())
                {
                    CastQTick = Utils.TickCount;

                    foreach (var enemy in HeroManager.Enemies.Where(e => e.IsValidTarget(Spells._q.Range) && !e.IsDead))
                    {
                        if(Utils.TickCount - CastQTick < 500)
                        {
                            Pred.CastSebbyPredict(Spells._q, enemy, Spells._q.MinHitChance);
                        }
                    }
                }
            }
        }

        #endregion
    }
}