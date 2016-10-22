using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Poppy : Champion
    {

        public Poppy()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Trinity_Force,ItemId.Iceborn_Gauntlet, ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Spirit_Visage,ItemId.Sunfire_Cape,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only,ItemId.The_Black_Cleaver,ItemCondition.ENEMY_LOSING),
                            new ConditionalItem(ItemId.Warmogs_Armor),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Ruby_Crystal,ItemId.Cloth_Armor,ItemId.Long_Sword
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.CastIfHitchanceEquals(target, HitChance.High);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            var posAger = player.Position.Extend(target.Position, 370);
            if (safeGap(posAger.To2D()) || posAger.IsWall())
                E.CastOnUnit(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            //if (player.HealthPercent < 35)
            R.CastIfHitchanceEquals(target, HitChance.Medium);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if(tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            if(E.IsReady())
                foreach (var enem in DeathWalker.AllEnemys)
                {
                    useE(enem);
                }
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q,400);
            Q.SetSkillshot(0.55f, 90f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W,250);
            E = new Spell(SpellSlot.E, 525);
            R = new Spell(SpellSlot.R, 250);
            R.SetSkillshot(0.5f, 90f, 1400, true, SkillshotType.SkillshotLine);
            R.SetCharged("PoppyR", "PoppyR", 425, 1000, 1.0f);
        }
    }
}
