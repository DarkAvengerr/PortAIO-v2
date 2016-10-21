using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Hikigaya_Lux.Core;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hikigaya_Lux.Logic
{
    class AutoSpells
    {
        public static void AutoQIfHit2Target(AIHeroClient enemy)
        {
            if (Spells.Q.GetPrediction(enemy).CollisionObjects.Count == 2)
            {
                if (Spells.Q.GetPrediction(enemy).CollisionObjects[0].IsChampion() && Spells.Q.GetPrediction(enemy).CollisionObjects[1].IsChampion())
                {
                    Spells.Q.Cast(Spells.Q.GetPrediction(enemy).CastPosition);
                }
            }
        }
        public static void AutoQIfEnemyImmobile(AIHeroClient enemy)
        {
            if (Helper.IsEnemyImmobile(enemy) && Spells.Q.GetPrediction(enemy).Hitchance >= Helper.HikiChance("q.hit.chance"))
            {
                Spells.Q.Cast(enemy);
            }
        }
        public static void AutoEIfHitXTarget(AIHeroClient enemy)
        {
            if (Spells.E.CastIfWillHit(enemy, Helper.Slider("min.e.hit")) && Spells.E.GetPrediction(enemy).Hitchance >= Helper.HikiChance("e.hit.chance"))
            {
                Spells.E.Cast(enemy);
            }
            if (Helper.LuxE != null && Helper.EInsCheck() == 2)
            {
                Spells.E.Cast();
            }
        }
        public static void AutoEIfEnemyImmobile(AIHeroClient enemy)
        {
            if (Helper.IsEnemyImmobile(enemy) && Spells.E.GetPrediction(enemy).Hitchance >= Helper.HikiChance("e.hit.chance"))
            {
                Spells.E.Cast(enemy);
            }
            if (Helper.LuxE != null && Helper.EInsCheck() == 2)
            {
                Spells.E.Cast();
            }
        }
        public static void AutoRIfEnemyKillable(AIHeroClient enemy)
        {
            if (Spells.R.GetPrediction(enemy).Hitchance >= Helper.HikiChance("r.hit.chance.x") && Calculators.R(enemy) > enemy.Health)
            {
                Spells.R.Cast(enemy);
            }
        }

        public static void KillStealWithQ(AIHeroClient enemy)
        {
            if (Spells.Q.GetPrediction(enemy).Hitchance >= Helper.HikiChance("q.hit.chance") && Calculators.Q(enemy) > enemy.Health)
            {
                Spells.Q.Cast(enemy);
            }
        }
        public static void KillStealWithE(AIHeroClient enemy)
        {
            if (Spells.E.GetPrediction(enemy).Hitchance >= Helper.HikiChance("e.hit.chance") && Calculators.E(enemy) > enemy.Health)
            {
                Spells.E.Cast(enemy);
            }
            if (Helper.LuxE != null && Helper.EInsCheck() == 2)
            {
                Spells.E.Cast();
            }
        }
    }
}
