using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Kogmaw : Champion
    {

        public Kogmaw()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Runaans_Hurricane_Ranged_Only),
                            new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                            new ConditionalItem(ItemId.Guinsoos_Rageblade),
                            new ConditionalItem(ItemId.Infinity_Edge,ItemId.Ludens_Echo,ItemCondition.ENEMY_MR),
                            new ConditionalItem(ItemId.Maw_of_Malmortius,ItemId.Rabadons_Deathcap,ItemCondition.ENEMY_MR),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Berserkers_Greaves
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            E.Cast(target);

        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if (GetUltimateBuffStacks() < 3 || R.GetDamage(target) > target.Health)
                R.Cast(target);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, float.MaxValue);
            E = new Spell(SpellSlot.E, 1360f);
            R = new Spell(SpellSlot.R, float.MaxValue);

            Q.SetSkillshot(0.25f, 70f, 1650f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.2f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public static bool EnemyInRange(int numOfEnemy, float range)
        {
            return LeagueSharp.Common.Utility.CountEnemysInRange(ObjectManager.Player, (int)range) >= numOfEnemy;
        }

        private static int GetUltimateBuffStacks()
        {
            return (from buff in ObjectManager.Player.Buffs
                    where buff.DisplayName.ToLower() == "kogmawlivingartillery"
                    select buff.Count).FirstOrDefault();
        }

    }
}
