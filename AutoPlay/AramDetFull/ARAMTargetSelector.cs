using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy;
namespace ARAMDetFull
{
    class ARAMTargetSelector
    {
        public static AIHeroClient getBestTarget(float range, bool calcInRadius = false, Vector3 fromPlus = new Vector3(), List<AIHeroClient> fromEnes = null)
        {
            try
            {
                var target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Physical);
                if (target != null && target.IsValidTarget() && !target.IsDead)
                {
                    return target;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static AIHeroClient getSafeMeleeTarget(float range = 750)
        {
            return getBestTarget(range, true, new Vector3(), EloBuddy.SDK.EntityManager.Heroes.Enemies.Where(ene => ene != null && MapControl.safeGap(ene)).ToList());
        }

        public static AIHeroClient getBestTargetAly(float range, bool calcInRadius = false, Vector3 fromPlus = new Vector3())
        {
            try
            {
                List<AIHeroClient> targetable_ones =
                    EloBuddy.SDK.EntityManager.Heroes.Allies.Where(ob => ob != null && ob.HealthPercent < 80 && !IsInvulnerable(ob) && !ob.IsDead && !ob.IsZombie
                        && (ob.IsValidTarget((!calcInRadius) ? range : range + 90) || ob.IsValidTarget((!calcInRadius) ? range : range + 90, true, fromPlus))).ToList();

                if (targetable_ones.Count == 0)
                    return null;
                if (targetable_ones.Count == 1)
                    return targetable_ones.FirstOrDefault();

                AIHeroClient lowestHp = targetable_ones.OrderBy(tar => tar.Health / ARAMSimulator.player.GetAutoAttackDamage(tar)).FirstOrDefault();
                if (lowestHp != null && lowestHp.MaxHealth != 0 && lowestHp.HealthPercent < 75)
                    return lowestHp;
                AIHeroClient bestStats = targetable_ones.OrderByDescending(tar => (tar.ChampionsKilled + tar.Assists) / ((tar.Deaths == 0) ? 0.5f : tar.Deaths)).FirstOrDefault();

                return bestStats;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static bool IsInvulnerable(Obj_AI_Base target)
        {
            // Tryndamere's Undying Rage (R)
            if (target.HasBuff("Undying Rage") && target.Health <= target.Health * 0.1f)
            {
                return true;
            }

            // Kayle's Intervention (R)
            if (target.HasBuff("JudicatorIntervention"))
            {
                return true;
            }
            //ChronoShift
            if (target.HasBuff("ChronoShift"))
            {
                return true;
            }

            return false;
        }

    }
}
