using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events.Games.Mode
{
    using Spells;
    using System.Linq;
    using myCommon;
    using LeagueSharp.Common;

    internal class KillSteal : Logic
    {
        internal static void Init()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)))
            {
                if (target.Check(R.Range + E.Range - 100))
                {
                    if (W.IsReady() && Menu.GetBool("KillStealW") && target.IsValidTarget(W.Range) &&
                        target.Health < DamageCalculate.GetWDamage(target))
                    {
                        W.Cast(true);
                    }

                    if (R.IsReady() && Menu.GetBool("KillStealR") && R.Instance.Name == "RivenIzunaBlade" &&
                        DamageCalculate.GetRDamage(target) > target.Health + target.HPRegenRate)
                    {
                        if (E.IsReady() && Menu.GetBool("KillStealE"))
                        {
                            if (Me.ServerPosition.CountEnemiesInRange(R.Range + E.Range) < 3 &&
                                Me.HealthPercent > 50 && target.IsValidTarget(R.Range + E.Range - 100))
                            {
                                if (E.IsReady())
                                {
                                    E.Cast(target.Position, true);
                                    LeagueSharp.Common.Utility.DelayAction.Add(100,
                                        () => R.CastIfHitchanceEquals(target, HitChance.High, true));
                                }
                            }
                        }
                        else
                        {
                            if (target.IsValidTarget(R.Range - 50))
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                            }
                        }
                    }
                }
            }
        }
    }
}
