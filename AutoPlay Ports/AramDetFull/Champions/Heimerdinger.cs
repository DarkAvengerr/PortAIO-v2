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
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            if (!Q.IsReady(4500) && player.Mana > 200)
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
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

            if (ObjectManager.Player.Distance(target) < E1.Range)
            {
                var oldrange = E1.Range;
                E1.Range = E2.Range;
                prediction = E1.GetPrediction(target, true);
                E1.Range = oldrange;
            }
            else if (ObjectManager.Player.Distance(target) < E2.Range)
            {
                var oldrange = E2.Range;
                E2.Range = E3.Range;
                prediction = E2.GetPrediction(target, true);
                E2.Range = oldrange;
            }
            else if (ObjectManager.Player.Distance(target) < E3.Range)
            {
                prediction = E3.GetPrediction(target, true);
            }
            else
            {
                return;
            }

            if (prediction.Hitchance >= HitChance.High)
            {
                if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <= E1.Range + E1.Width)
                {
                    Vector3 p;
                    if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) > 300)
                    {
                        p = prediction.CastPosition -
                            100 *
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized()
                                .To3D();
                    }
                    else
                    {
                        p = prediction.CastPosition;
                    }
                    R.Cast();
                    E1.Cast(p);
                }
                else if (ObjectManager.Player.ServerPosition.Distance(prediction.CastPosition) <=
                         ((E1.Range + E1.Range) / 2))
                {
                    var p = ObjectManager.Player.ServerPosition.To2D()
                        .Extend(prediction.CastPosition.To2D(), E1.Range - 100);
                    {
                        R.Cast();
                        E1.Cast(p.To3D());
                    }
                }
                else
                {
                    var p = ObjectManager.Player.ServerPosition.To2D() +
                            E1.Range *
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized
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

            if (E.IsReady() && target.IsValidTarget(E.Range))
            {
                E.CastIfHitchanceEquals(target, HitChance.Medium, true);
            }
            if (W.IsReady() &&
                R.IsReady() && target.IsValidTarget(W.Range) &&
                wpred.Hitchance >= HitChance.High && CalcDamage(target) > target.Health)
            {
                R.Cast();

                LeagueSharp.Common.Utility.DelayAction.Add(1010,
                    () => W.CastIfHitchanceEquals(target, HitChance.High, true));
            }
            else
            {
                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.CastIfHitchanceEquals(target, HitChance.High, true);
                }
            }

                if (Q.IsReady() && R.IsReady()  &&
                     qtarget.IsValidTarget(650) &&
                    player.Position.CountEnemiesInRange(650) >=
                    2)
                {
                    R.Cast();
                    Q.Cast(player.Position.Extend(target.Position, +300));
                }
                else
                {
                    if (Q.IsReady()  && qtarget.IsValidTarget(650) &&

                        player.Position.CountEnemiesInRange(650) >= 1)
                    {
                        Q.Cast(player.Position.Extend(target.Position, +300));
                    }
                }
                if (E3.IsReady()  &&
                    target.Position.CountEnemiesInRange(450 - 250) >=
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

            var aa = player.GetAutoAttackDamage(target, true);
            var damage = aa;



                if (E.IsReady())
                {
                    damage += E.GetDamage(target);
                }

            if (E.IsReady() ) // rdamage
            {

                damage += E.GetDamage(target);
            }

            if (W.IsReady() )
            {
                damage += W.GetDamage(target);
            }
            if (W.IsReady())
            {
                if (R.IsReady() )
                    damage += W.GetDamage(target) * 2.2;
            }
            return (int)damage;

        }

        private float GetWDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W.IsReady())
                damage += player.GetSpellDamage(target, SpellSlot.W);

            return (float)damage * 2;
        }

        private float GetW1Damage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (W1.IsReady() && R.IsReady())
                damage += player.GetSpellDamage(target, SpellSlot.W, 1);

            return (float)damage * 2;
        }

        private float GetEDamage(Obj_AI_Base enemy)
        {
            var target = TargetSelector.GetTarget(W.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return (float)0;
            double damage = 0d;

            if (E.IsReady())
                damage += player.GetSpellDamage(target, SpellSlot.E);

            return (float)damage * 2;
        }

    }
}
