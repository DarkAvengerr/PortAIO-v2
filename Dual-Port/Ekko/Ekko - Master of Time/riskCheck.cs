using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_master_of_time
{
    static class riskCheck
    {
        public static bool RiskChecker(Vector3 position, int range)
        {
            var riskNumber = 0;
            //this checks how much enemys you will find
            foreach (var target in HeroManager.AllHeroes)
            {
                if (!(target.Distance(position) <= range)) continue;
                if (target.IsAlly)
                {
                    riskNumber -= 15;
                }
                if (target.IsEnemy)
                {
                    riskNumber += 20;
                }
            }
            return riskNumber <= 50;
        }

      /*  public static bool Unkilleable(List<Spell> spells, AIHeroClient target)
        {
            foreach (Spell s in spells)
            {
                if (s.IsSkillshot)
                {
                    var pospred = s.GetPrediction(target);
                    if (pospred.Hitchance >= HitChance.High)
                    {
                        return false;
                    }
                }
                else
                {
                  //  if(target.Distance()s.IsKillable(target)))
                }
            }
        }
        */
        public static double GetDamageInput(List<Spell> spells, AIHeroClient target)
        {
            double damage = 0;
            foreach (Spell s in spells)
            {
                if (s.IsReady())
                {
                    damage += s.GetDamage(target);
                }
            }
            return damage;
        }

    
        public static bool WillHitEnemys(Vector3 zone, int Range, int min)
        {
            int i=0;
            foreach (AIHeroClient b in ObjectManager.Get<AIHeroClient>())
            {
                if (b.IsEnemy && !b.IsDead && b.Distance(zone) < Range)
                {
                    i++;
                }
            }
            if (i >= min)
                return true;
            else
                return false;
        }
        public static double GetTowerDamage(AIHeroClient target)
        {
            double tower_damage = 0f;
            /*
            foreach (Obj_AI_Turret turret in ObjectManager.Get<Obj_AI_Turret>())
            {
                if (target.Distance(turret) <= turret.AttackRange)
                {
                    if (turret == target)
                    {
                        for (int i = 0; i <= 10; i++)
                        {
                            var posintime = Prediction.GetPrediction(target, turret.AttackDelay*i);

                            if (posintime.UnitPosition.Distance(turret.Position) <= turret.AttackRange)
                            {
                                tower_damage += turret.TotalAttackDamage;
                            }
                        }
                    }
                }
            }
            */
            return tower_damage;
        }
    }
}
