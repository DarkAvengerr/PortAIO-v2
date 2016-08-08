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
    class Yasuo : Champion
    {

        private Spell Q2;

        public Yasuo()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Statikk_Shiv),
                    new ConditionalItem(ItemId.Mercurys_Treads),
                    new ConditionalItem(ItemId.Infinity_Edge),
                    new ConditionalItem(ItemId.Phantom_Dancer),
                    new ConditionalItem(ItemId.Warmogs_Armor),
                    new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only,ItemId.Banshees_Veil,ItemCondition.ENEMY_LOSING),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Zeal
                }
            };
        }


        public override void useQ(Obj_AI_Base target)
        {
            if (Q.LSIsReady())
                if (HaveQ3)
                    Q2.Cast(target);
                else
                {
                    Q.Cast(target);
                }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (W.LSIsReady())
                W.Cast(target.Position);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.LSIsReady() || target == null)
                return;
            if (safeGap(target))
            {
                if(!gapCloseE(target.Position.LSTo2D() ,new List<AIHeroClient>() {target as AIHeroClient}))
                    useESmart(target as AIHeroClient);
            }
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.LSIsReady() || target == null)
                return;
            if (safeGap(target) || MapControl.fightIsOn(target))
                 R.Cast();
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(HaveQ3?Q2.Range:Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range+600);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);

        }

        public override void farm()
        {
            if (Q.LSIsReady())
            {
                var farmL = Q.GetLineFarmLocation(MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health));
                if (farmL.MinionsHit > 0)
                    Q.Cast(farmL.Position);
            }
        }

        public bool useESmart(AIHeroClient target, List<AIHeroClient> ignore = null)
        {
            if (!E.LSIsReady())
                return false;
            float trueAARange = player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + E.Range;

            float dist = player.LSDistance(target);
            Vector2 dashPos = new Vector2();
            if (target.IsMoving && target.Path.Count() != 0)
            {
                Vector2 tpos = target.Position.LSTo2D();
                Vector2 path = target.Path[0].LSTo2D() - tpos;
                path.Normalize();
                dashPos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && player.LSDistance(dashPos) > dist) ? target.MoveSpeed : 0;
            float msDif = (player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (player.MoveSpeed - targ_ms);
            float timeToReach = (dist - trueAARange) / msDif;
            if (dist > trueAARange && dist < E.Range)
            {
                if (timeToReach > 1.7f || timeToReach < 0.0f)
                {
                    E.Cast(target);
                }
            }
            return false;
        }

        public bool gapCloseE(Vector2 pos, List<AIHeroClient> ignore = null)
        {
            if (!E.LSIsReady())
                return false;

            Vector2 pPos = player.ServerPosition.LSTo2D();
            Obj_AI_Base bestEnem = null;


            float distToPos = player.LSDistance(pos);
            Vector2 bestLoc = pPos + (Vector2.Normalize(pos - pPos) * (player.MoveSpeed * 0.35f));
            float bestDist = pos.LSDistance(pPos) - 50;
            try
            {
                foreach (Obj_AI_Base enemy in ObjectManager.Get<Obj_AI_Base>().Where(ob => enemyIsJumpable(ob, ignore)))
                {

                    float trueRange = E.Range + enemy.BoundingRadius;
                    float distToEnem = player.LSDistance(enemy);
                    if (distToEnem < trueRange && distToEnem > 15)
                    {
                        Vector2 posAfterE = pPos + (Vector2.Normalize(enemy.Position.LSTo2D() - pPos) * E.Range);
                        float distE = pos.LSDistance(posAfterE);
                        if (distE < bestDist)
                        {
                            bestLoc = posAfterE;
                            bestDist = distE;
                            bestEnem = enemy;
                            // Console.WriteLine("Gap to best enem");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (bestEnem != null)
            {
                Console.WriteLine("should use gap");
                E.Cast(bestEnem);
                return true;
            }
            return false;

        }

        public bool enemyIsJumpable(Obj_AI_Base enemy, List<AIHeroClient> ignore = null)
        {
            if (enemy.IsValid && enemy.IsEnemy && !enemy.IsInvulnerable && !enemy.MagicImmune && !enemy.IsDead && !(enemy is FollowerObject))
            {
                if (ignore != null)
                    foreach (AIHeroClient ign in ignore)
                    {
                        if (ign.NetworkId == enemy.NetworkId)
                            return false;
                    }
                foreach (BuffInstance buff in enemy.Buffs)
                {
                    if (buff.Name == "YasuoDashWrapper")
                        return false;
                }
                return true;
            }
            return false;
        }
        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 500);
            Q2 = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 475, TargetSelector.DamageType.Magical);
            R = new Spell(SpellSlot.R, 1300);
            Q.SetSkillshot(GetQDelay, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q2.SetSkillshot(GetQ2Delay, 90, 1500, false, SkillshotType.SkillshotLine);
        }

        private bool HaveQ3
        {
            get { return player.LSHasBuff("YasuoQ3W"); }
        }

        private float GetQDelay
        {
            get { return 0.4f * (1 - Math.Min((player.AttackSpeedMod - 1) * 0.58f, 0.66f)); }
        }

        private float GetQ2Delay
        {
            get { return 0.5f * (1 - Math.Min((player.AttackSpeedMod - 1) * 0.58f, 0.66f)); }
        }

    }
}
