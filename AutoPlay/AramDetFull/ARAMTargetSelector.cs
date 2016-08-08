using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ARAMDetFull
{
    class ARAMTargetSelector
    {
        public static AIHeroClient getBestTarget(float range,bool calcInRadius = false, Vector3 fromPlus = new Vector3(), List<AIHeroClient> fromEnes = null  )
        {
            if (fromEnes == null)
                fromEnes = HeroManager.Enemies.Where(ene => ene != null && (ene.MaxMana<300 || (ene.MaxMana>=300 && ene.ManaPercent>15)) || ene.HealthPercent>15 || ene.FlatPhysicalDamageMod>40+ene.Level*6).ToList();

            List<AIHeroClient> targetable_ones =
                fromEnes.Where(ob => ob != null && !IsInvulnerable(ob) && !ob.IsDead && !ob.IsZombie
                    && (ob.LSIsValidTarget((!calcInRadius) ? range : range + 90) || ob.LSIsValidTarget((!calcInRadius) ? range : range + 90, true, fromPlus))).ToList();

            if (targetable_ones.Count == 0)
                return null;
            if (targetable_ones.Count == 1)
                return targetable_ones.FirstOrDefault();

            AIHeroClient lowestHp = targetable_ones.OrderBy(tar => tar.Health / ARAMSimulator.player.LSGetAutoAttackDamage(tar)).FirstOrDefault();
            if (lowestHp != null && lowestHp.MaxHealth != 0 && lowestHp.HealthPercent < 75)
                return lowestHp;
            AIHeroClient bestStats = targetable_ones.OrderByDescending(tar => (tar.ChampionsKilled + tar.Assists) / ((tar.Deaths == 0) ? 0.5f : tar.Deaths)).FirstOrDefault();

            return bestStats;
        }

        public static AIHeroClient getSafeMeleeTarget(float range = 750)
        {
            return getBestTarget(range,true, new Vector3(), HeroManager.Enemies.Where(ene => ene!=null && MapControl.safeGap(ene)).ToList());
        }

        public static AIHeroClient getBestTargetAly(float range, bool calcInRadius = false, Vector3 fromPlus = new Vector3())
        {
            List<AIHeroClient> targetable_ones =
                HeroManager.Allies.Where(ob => ob != null && ob.HealthPercent < 80 && !IsInvulnerable(ob) && !ob.IsDead && !ob.IsZombie
                    && (ob.LSIsValidTarget((!calcInRadius) ? range : range + 90) || ob.LSIsValidTarget((!calcInRadius) ? range : range + 90, true, fromPlus))).ToList();

            if (targetable_ones.Count == 0)
                return null;
            if (targetable_ones.Count == 1)
                return targetable_ones.FirstOrDefault();

            AIHeroClient lowestHp = targetable_ones.OrderBy(tar => tar.Health / ARAMSimulator.player.LSGetAutoAttackDamage(tar)).FirstOrDefault();
            if (lowestHp != null && lowestHp.MaxHealth != 0 && lowestHp.HealthPercent < 75)
                return lowestHp;
            AIHeroClient bestStats = targetable_ones.OrderByDescending(tar => (tar.ChampionsKilled + tar.Assists) / ((tar.Deaths == 0) ? 0.5f : tar.Deaths)).FirstOrDefault();

            return bestStats;
        }

        public static bool IsInvulnerable(Obj_AI_Base target)
        {
            // Tryndamere's Undying Rage (R)
            if (target.LSHasBuff("Undying Rage") && target.Health <= target.Health*0.1f)
            {
                return true;
            }

            // Kayle's Intervention (R)
            if (target.LSHasBuff("JudicatorIntervention"))
            {
                return true;
            }
            //ChronoShift
            if (target.LSHasBuff("ChronoShift"))
            {
                return true;
            }

            return false;
        }

    }
}
