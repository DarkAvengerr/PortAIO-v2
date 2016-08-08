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
    class Herimerdinger : Champion
    {
        private Spell Q1;
        private Spell W1;
        private Spell E1;
        public Spell E2;
        public Spell E3;

        public Herimerdinger()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                            new ConditionalItem(ItemId.Liandrys_Torment),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Needlessly_Large_Rod
                        }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.LSIsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.LSIsReady() || target == null)
                return;
            if (!Q.LSIsReady(4500) && player.Mana > 200)
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.LSIsReady() || target == null)
                return;
            E.CastOnUnit(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
        }

        public override void useSpells()
        {

            Combo();
            ZhoUlt();

        }


        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 325);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 100);

            W1 = new Spell(SpellSlot.W, 1100);
            E1 = new Spell(SpellSlot.E, 925);
            E2 = new Spell(SpellSlot.E, 1125);
            E3 = new Spell(SpellSlot.E, 1325);

            Q.SetSkillshot(0.5f, 40f, 1100f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            W1.SetSkillshot(0.5f, 40f, 3000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);
            E1.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotCircle);
            E2.SetSkillshot(0.25f + E1.Delay, 120f, 1200f, false, SkillshotType.SkillshotLine);
            E3.SetSkillshot(0.3f + E2.Delay, 120f, 1200f, false, SkillshotType.SkillshotLine);
            Chat.Print("heimer loaded");
        }

        public override void farm()
        {
            if(player.ManaPercent < 55)
                return;
            //var MinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width);
            var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width);
            //var Wfarmpos = W.GetLineFarmLocation(MinionsW, W.Width);
            var Efarmpos = E.GetCircularFarmLocation(MinionsE, E.Width);

           /* if ( Wfarmpos.MinionsHit >= 3 
               && player.ManaPercent >= 55)
            {
                //W.Cast(Wfarmpos.Position);
            }*/
            if (Efarmpos.MinionsHit >= 3 && MinionsE.Count >= 1
                && player.ManaPercent >= 5)
            {
                E.Cast(Efarmpos.Position);
            }
        }

        public override void killSteal()
        {
            var target = ARAMTargetSelector.getBestTarget(E.Range + 200);
            if (target == null) return;
            if (target.Health < GetEDamage(target))
            {
                E.CastIfHitchanceEquals(target, HitChance.Medium, true);
                E.CastIfHitchanceEquals(target, HitChance.High, true);
                return;
            }


            target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;
            if (target.Health < GetWDamage(target))
            {
                var prediction = W.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High &&
                    prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                {

                    W.Cast(prediction.CastPosition);
                    return;
                }
            }
        }


        private void CastER(Obj_AI_Base target) // copied from ScienceARK
        {

            PredictionOutput prediction;

            if (ObjectManager.Player.LSDistance(target) < E1.Range)
            {
                var oldrange = E1.Range;
                E1.Range = E2.Range;
                prediction = E1.GetPrediction(target, true);
                E1.Range = oldrange;
            }
            else if (ObjectManager.Player.LSDistance(target) < E2.Range)
            {
                var oldrange = E2.Range;
                E2.Range = E3.Range;
                prediction = E2.GetPrediction(target, true);
                E2.Range = oldrange;
            }
            else if (ObjectManager.Player.LSDistance(target) < E3.Range)
            {
                prediction = E3.GetPrediction(target, true);
            }
            else
            {
                return;
            }

            if (prediction.Hitchance >= HitChance.High)
            {
                if (ObjectManager.Player.ServerPosition.LSDistance(prediction.CastPosition) <= E1.Range + E1.Width)
                {
                    Vector3 p;
                    if (ObjectManager.Player.ServerPosition.LSDistance(prediction.CastPosition) > 300)
                    {
                        p = prediction.CastPosition -
                            100 *
                            (prediction.CastPosition.LSTo2D() - ObjectManager.Player.ServerPosition.LSTo2D()).LSNormalized()
                                .To3D();
                    }
                    else
                    {
                        p = prediction.CastPosition;
                    }
                    R.Cast();
                    E1.Cast(p);
                }
                else if (ObjectManager.Player.ServerPosition.LSDistance(prediction.CastPosition) <=
                         ((E1.Range + E1.Range) / 2))
                {
                    var p = ObjectManager.Player.ServerPosition.LSTo2D()
                        .LSExtend(prediction.CastPosition.LSTo2D(), E1.Range - 100);
                    {
                        R.Cast();
                        E1.Cast(p.To3D());
                    }
                }
                else
                {
                    var p = ObjectManager.Player.ServerPosition.LSTo2D() +
                            E1.Range *
                            (prediction.CastPosition.LSTo2D() - ObjectManager.Player.ServerPosition.LSTo2D()).LSNormalized
                                ();

                    {
                        R.Cast();
                        E1.Cast(p.To3D());
                    }
                }
            }
        }

        private  void ZhoUlt()
        {
            var fullHP = player.MaxHealth;
            var HP = player.Health;
            var critHP = fullHP / 4;
            if (HP <= critHP)
            {
                var target = ARAMTargetSelector.getBestTarget(1000);
                if (target == null) return;
                R.Cast();
                LeagueSharp.Common.Utility.DelayAction.Add(1010, () => Q.Cast(player.Position));
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => Q.Cast(player.Position));
            }

        }

        private void Combo()
        {
            var target = ARAMTargetSelector.getBestTarget(W.Range);
            if (target == null)
                return;
            var qtarget = ARAMTargetSelector.getBestTarget(600);
            if (qtarget == null)
                return;
            var wpred = W.GetPrediction(target);

            if (E.LSIsReady() && target.LSIsValidTarget(E.Range))
            {
                E.CastIfHitchanceEquals(target, HitChance.Medium, true);
            }
            if (W.LSIsReady() &&
                R.LSIsReady() && target.LSIsValidTarget(W.Range) &&
                wpred.Hitchance >= HitChance.High && CalcDamage(target) > target.Health)
            {
                R.Cast();

                LeagueSharp.Common.Utility.DelayAction.Add(1010,
                    () => W.CastIfHitchanceEquals(target, HitChance.High, true));
            }
            else
            {
                if (W.LSIsReady() && target.LSIsValidTarget(W.Range))
                {
                    W.CastIfHitchanceEquals(target, HitChance.High, true);
                }
            }

                if (Q.LSIsReady() && R.LSIsReady()  &&
                     qtarget.LSIsValidTarget(650) &&
                    player.Position.LSCountEnemiesInRange(650) >=
                    2)
                {
                    R.Cast();
                    Q.Cast(player.Position.LSExtend(target.Position, +300));
                }
                else
                {
                    if (Q.LSIsReady()  && qtarget.LSIsValidTarget(650) &&

                        player.Position.LSCountEnemiesInRange(650) >= 1)
                    {
                        Q.Cast(player.Position.LSExtend(target.Position, +300));
                    }
                }
                if (E3.LSIsReady()  &&
                    target.Position.LSCountEnemiesInRange(450 - 250) >=
                    2)
                {
                    CastER(target);
                }
                else
                {
                    
                }
        }
        private float GetDistance(AttackableUnit target)
        {
            return Vector3.Distance(player.Position, target.Position);
        }

        private int CalcDamage(Obj_AI_Base target)
        {



            //Calculate Combo Damage

            var aa = player.LSGetAutoAttackDamage(target, true);
            var damage = aa;



                if (E.LSIsReady())
                {
                    damage += E.GetDamage(target);
                }

            if (E.LSIsReady() ) // rdamage
            {

                damage += E.GetDamage(target);
            }

            if (W.LSIsReady() )
            {
                damage += W.GetDamage(target);
            }
            if (W.LSIsReady())
            {
                if (R.LSIsReady() )
                    damage += W.GetDamage(target) * 2.2;
            }
            return (int)damage;

        }

        private float GetWDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W.LSIsReady())
                damage += player.LSGetSpellDamage(target, SpellSlot.W);

            return (float)damage * 2;
        }

        private float GetW1Damage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W1.LSIsReady() && R.LSIsReady())
                damage += player.LSGetSpellDamage(target, SpellSlot.W, 1);

            return (float)damage * 2;
        }

        private float GetEDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (E.LSIsReady())
                damage += player.LSGetSpellDamage(target, SpellSlot.E);

            return (float)damage * 2;
        }

    }
}
