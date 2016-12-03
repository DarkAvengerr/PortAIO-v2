using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events.Games.Mode
{
    using myCommon;
    using LeagueSharp.Common;

    internal class Harass : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("Harassmana")))
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (target.Check(Q.Range))
                {
                    if (Menu.GetBool("HarassQ") && Q.IsReady())
                    {
                        var qPred = Q.GetPrediction(target, true, -1, new[] { CollisionableObjects.YasuoWall });

                        if (qPred.Hitchance >= HitChance.VeryHigh || qPred.Hitchance == HitChance.Immobile)
                        {
                            Q.Cast(qPred.CastPosition, true);
                        }
                    }
                }
            }
        }
    }
}
