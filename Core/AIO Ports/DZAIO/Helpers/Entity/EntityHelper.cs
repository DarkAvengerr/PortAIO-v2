using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.SDK;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Helpers.Entity
{
    static class EntityHelper
    {
        internal static bool IsJungleMob(this AttackableUnit Target)
        {
            return Target is Obj_AI_Minion && GameObjects.Jungle.Select(m => m.CharData.BaseSkinName)
                .Contains((Target as Obj_AI_Minion).CharData.BaseSkinName);
        }

        internal static bool PlayerIsClearingJungle()
        {
            //Assume we are clearing jungle based on the last target
            return !PlayerMonitor.GetLastTarget().IsDead && PlayerMonitor.GetLastTarget().IsJungleMob();
        }

        internal static AIHeroClient GetStunnedTarget(float range)
        {
            return HeroManager.Enemies.Where(en => LeagueSharp.Common.Utility.IsValidTarget(en, range) && en.HasBuffOfType(BuffType.Stun))
                .OrderBy(LeagueSharp.Common.TargetSelector.GetPriority).FirstOrDefault();

        }

        internal static bool IsCharmed(this Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Charm);
        }

        internal static bool IsHeavilyImpaired(this AIHeroClient enemy)
        {
            return enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) || enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) || enemy.HasBuffOfType(BuffType.Taunt);
        }

        internal static bool IsLightlyImpaired(this AIHeroClient enemy)
        {
            return enemy.HasBuffOfType(BuffType.Slow);
        }

        internal static SpellSlot GetSmiteSlot(this AIHeroClient entity)
        {
            string[] smiteNames =
            {
                "SummonerSmite", "s5_summonersmiteduel", "s5_summonersmiteplayerganker",
                "s5_summonersmitequick", "itemsmiteaoe"
            };

            foreach (var name in smiteNames)
            {
                var slot = LeagueSharp.Common.Utility.GetSpellSlot(entity, name);

                if(slot != SpellSlot.Unknown)
                {
                    return slot;
                }
            }

            return SpellSlot.Unknown;
        }
    }
}
