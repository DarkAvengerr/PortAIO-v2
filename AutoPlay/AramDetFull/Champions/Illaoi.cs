using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Illaoi : Champion
    {
        public Illaoi()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.The_Black_Cleaver),
                    new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Spirit_Visage),
                    new ConditionalItem(ItemId.Warmogs_Armor),
                    new ConditionalItem((ItemId)3053),
                    new ConditionalItem((ItemId)3812, (ItemId)3748, ItemCondition.ENEMY_LOSING),
                },
                startingItems = new List<ItemId>
                {
                    (ItemId)3133
                }
            };
        }
        
        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.LSIsReady() || target == null)
                return;
            var enemy = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(Q.Range));
            var enemyGhost = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Name == enemy.Name);
            if (enemy != null && enemyGhost == null)
            {
                if (Q.CanCast(enemy) && Q.GetPrediction(enemy).Hitchance >= HitChance.High
                    && Q.GetPrediction(enemy).CollisionObjects.Count == 0)
                {
                    Q.Cast(enemy);
                }
            }
            if (enemy == null && enemyGhost != null)
            {
                if (Q.CanCast(enemyGhost) && Q.GetPrediction(enemyGhost).Hitchance >= HitChance.High
                    && Q.GetPrediction(enemyGhost).CollisionObjects.Count == 0)
                {
                    Q.Cast(enemyGhost);
                }
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.LSIsReady() || target == null)
                return;
            W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.LSIsReady() || target == null)
                return;
            E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!E.LSIsReady() || target == null)
                return;
            if (player.LSCountEnemiesInRange(500) >= 2)
            {
                R.Cast(target.Position);
                Aggresivity.addAgresiveMove(new AgresiveMove(1200,8000));
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
        }


        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(.484f, 0, 500, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(.066f, 50, 1900, true, SkillshotType.SkillshotLine);
        }
        
    }
}
