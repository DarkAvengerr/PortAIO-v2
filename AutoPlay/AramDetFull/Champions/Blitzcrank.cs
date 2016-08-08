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
    class BlitzcrankA : Champion
    {
        public BlitzcrankA()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Rod_of_Ages),
                    new ConditionalItem(ItemId.Ionian_Boots_of_Lucidity),
                    new ConditionalItem(ItemId.Lich_Bane),
                    new ConditionalItem(ItemId.Abyssal_Scepter, ItemId.Frozen_Heart, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Void_Staff),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Catalyst_the_Protector,ItemId.Boots_of_Speed
                }
            };

            Interrupter.OnPossibleToInterrupt += OnPossibleToInterrupt;
            DeathWalker.AfterAttack += OnAfterAttack;
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.LSIsReady())
                return;
            if (player.LSInShop())
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.LSIsReady() || W.LSIsReady())
                return;
            E.Cast();
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.LSIsReady())
                return;
                R.Cast();


        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 200);
            E = new Spell(SpellSlot.E, 175);
            R = new Spell(SpellSlot.R, 600);

            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
                if(tar != null)useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
                if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
                if (tar != null) useR(tar);
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.LSIsReady())
            {
                return;
            }

            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (target.LSHasBuff("BlackShield"))
            {
                return;
            }

            if (AllyInRange(1200)
                .Any(ally => ally.LSDistance(target) < ally.AttackRange + ally.BoundingRadius))
            {
                return;
            }
            Q.Cast(target);

        }

        public override void farm()
        {

        }

        public List<AIHeroClient> AllyInRange(float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        h =>
                            Geometry.LSDistance(ObjectManager.Player, h.Position) < range && h.IsAlly && !h.IsMe &&
                            h.IsValid && !h.IsDead)
                    .OrderBy(h => Geometry.LSDistance(ObjectManager.Player, h.Position))
                    .ToList();
        }

        public void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe)
            {
                return;
            }

            if (!target.IsValid<AIHeroClient>() && !target.Name.ToLower().Contains("ward"))
            {
                return;
            }

            if (!E.LSIsReady())
            {
                return;
            }

            if (E.Cast())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        public void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.LSIsReady() && E.Cast())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
            }

            if (Q.LSIsReady())
            {
                Q.Cast(unit);
            }

            if (R.LSIsReady())
            {
                R.Cast();
            }
        }
    }
}
