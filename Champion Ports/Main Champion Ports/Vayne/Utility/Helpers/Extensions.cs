using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using VayneHunter_Reborn.Utility.MenuUtility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Utility.Helpers
{
    static class Extensions
    {
        public static List<AIHeroClient> GetLhEnemiesNear(this Vector3 position, float range, float healthpercent)
        {
            return HeroManager.Enemies.Where(hero => hero.IsValidTarget(range, true, position) && hero.HealthPercent <= healthpercent).ToList();
        }

        public static bool UnderAllyTurret_Ex(this Vector3 position)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(t => t.IsAlly && !t.IsDead);
        }

        public static bool IsJ4Flag(this Vector3 endPosition, Obj_AI_Base target)
        {
            return MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.condemn.condemnflag")
                && ObjectManager.Get<Obj_AI_Base>().Any(m => m.Distance(endPosition) <= target.BoundingRadius && m.Name == "Beacon");
        }

        public static bool Has2WStacks(this AIHeroClient target)
        {
            return target.Buffs.Any(bu => bu.Name == "vaynesilvereddebuff" && bu.Count == 2);
        }

        public static BuffInstance GetWBuff(this AIHeroClient target)
        {
            return target.Buffs.FirstOrDefault(bu => bu.Name == "vaynesilvereddebuff");
        }
    }
}
