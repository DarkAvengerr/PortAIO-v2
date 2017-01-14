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
namespace Lord_s_Vayne.Condemn.HeroExtension
{
    static class HeroExtensions
    {
        public static List<AIHeroClient> GetLhEnemiesNear(this Vector3 position, float range, float healthpercent)
        {
            return HeroManager.Enemies.Where(hero => hero.IsValidTarget(range, true, position) && hero.HealthPercent <= healthpercent).ToList();
        }
    }
}
