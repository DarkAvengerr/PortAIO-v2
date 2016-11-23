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
                if(Spells._q.IsReadyPerfectly())
                {
                    CastQTick = Utils.TickCount;

                    foreach (var enemy in HeroManager.Enemies)
                    {
                        if (!enemy.IsDead && enemy != null)
                        {
                            if (enemy.IsValidTarget(Spells._q.Range))
                            {
                                if(Utils.TickCount - CastQTick < 500)
                                {
                                    switch (Config.PredSemiQ)
                                    {
                                        //VeryHigh
                                        case 0:
                                        {
                                            Pred.CastSebbyPredict(Spells._q, enemy, HitChance.VeryHigh);
                                            return;
                                        }
                                        //High
                                        case 1:
                                        {
                                            Pred.CastSebbyPredict(Spells._q, enemy, Spells._q.MinHitChance);
                                            return;
                                        }
                                        //Medium
                                        case 2:
                                        {
                                            Pred.CastSebbyPredict(Spells._q, enemy, HitChance.Medium);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}