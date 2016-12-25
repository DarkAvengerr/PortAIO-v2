using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events.Games.Mode
{
    using myCommon;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class KillSteal : Logic
    {
        internal static void Init()
        {
            foreach (var target in
              HeroManager.Enemies.Where(
                  e => e.IsValidTarget() && !e.IsDead && !e.IsZombie && e.IsUnKillable()))
            {
                if (Menu.GetBool("KillStealQ") && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    Me.GetSpellDamage(target, SpellSlot.Q) > target.Health + target.MagicShield)
                {
                    var qPred = Q.GetPrediction(target, true);

                    if (qPred.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(qPred.CastPosition, true);
                    }
                }

                if (Menu.GetBool("KillStealR") && Menu.GetBool("KillStealR" + target.ChampionName.ToLower()) &&
                    R.IsReady() && target.IsValidTarget(R.Range) &&
                    Me.GetSpellDamage(target, SpellSlot.R) + Me.TotalAttackDamage > target.Health + target.MagicShield)
                {
                    R.CastOnUnit(target, true);
                }
            }
        }
    }
}
