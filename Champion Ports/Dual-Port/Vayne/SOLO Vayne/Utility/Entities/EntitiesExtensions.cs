using System.Linq;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Utility.Entities
{
    static class EntitiesExtensions
    {
        /// <summary>
        /// Does the target have 2W stacks.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static bool Has2WStacks(this AIHeroClient target)
        {
            return target.Buffs.Any(bu => bu.Name == "vaynesilvereddebuff" && bu.Count == 2);
        }

        /// <summary>
        /// Gets the W buff.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static BuffInstance GetWBuff(this AIHeroClient target)
        {
            return target.Buffs.FirstOrDefault(bu => bu.Name == "vaynesilvereddebuff");
        }
    }
}
