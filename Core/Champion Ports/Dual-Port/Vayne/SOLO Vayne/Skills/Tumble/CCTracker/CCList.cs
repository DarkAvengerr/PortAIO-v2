using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Skills.Tumble.CCTracker
{
    class CCList
    {
        public List<CC> CcList { get; private set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
            //TODO For Annie. Pyro stacks > 3
            CcList = new List<CC>()
            {
                new CC("Annie", SpellSlot.Q, 1400, CCRange.Ranged, CCType.Targetted, () => Variables.tracker.GetModuleByName("Annie").GetChampion() != null
                    ? Variables.tracker.GetModuleByName("Annie").GetChampion().HasBuff("pyromania_particle")
                    : false),
                new CC("Annie",SpellSlot.W, 650f, CCRange.Ranged, CCType.AOEFromChamp, () => Variables.tracker.GetModuleByName("Annie").GetChampion() != null
                    ? Variables.tracker.GetModuleByName("Annie").GetChampion().HasBuff("pyromania_particle")
                    : false),
                new CC("Annie",SpellSlot.R, 1400f, CCRange.Ranged, CCType.AOE, () => Variables.tracker.GetModuleByName("Annie").GetChampion() != null
                    ? Variables.tracker.GetModuleByName("Annie").GetChampion().HasBuff("pyromania_particle")
                    : false),
            };
        }

        /// <summary>
        /// Gets the champion instanc by its name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public AIHeroClient GetChampionByName(string name)
        {
            return HeroManager.Enemies.FirstOrDefault(m => m.ChampionName.ToLower() == name.ToLower());
        }
    }
}
