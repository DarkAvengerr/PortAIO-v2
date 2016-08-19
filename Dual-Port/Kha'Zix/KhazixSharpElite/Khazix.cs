using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KhazixSharp
{
    class Khazix
    {
        public static AIHeroClient Player = ObjectManager.Player;

        public static Spellbook sBook = Player.Spellbook;

        public static Orbwalking.Orbwalker orbwalker;

        public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 325);
        public static Spell W = new Spell(SpellSlot.W, 1000);
        public static Spell E = new Spell(SpellSlot.E, 700);
        public static Spell R = new Spell(SpellSlot.R, 0);




        public static SummonerItems sumItems;

        public static void setSkillshots()
        {
            W.SetSkillshot(0.225f, 100f, 828.5f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.250f, 100f, 1000f, false, SkillshotType.SkillshotCircle);
            sumItems = new SummonerItems(Player);
        }

        public static void checkUpdatedSpells()
        {
            if (Qdata.Name == "khazixqlong")
                Q.Range = 375;
            if (Edata.Name == "khazixelong")
                E.Range = 1000;

               /* foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Rdata))
                {
                    string name=descriptor.Name;
                    object value = descriptor.GetValue(Rdata);
                    Console.WriteLine("{0}={1}",name,value);
                }*/
        }


        public static void doCombo(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget())
                return;

            if (target.Distance(Player) < 500)
            {
                sumItems.cast(SummonerItems.ItemIds.Ghostblade);
            }
            if (target.Distance(Player) < 500 && (Player.Health / Player.MaxHealth) * 100 < 85)
            {
                sumItems.cast(SummonerItems.ItemIds.BotRK, target);

            }
            if (isStealthed() && targIsKillabe(target) && enemiesNear() > 2)
            {
                Orbwalking.Attack = false;
            }
            else
            {
                useHydra(target);
                doQ(target);
                Orbwalking.Attack = true;
                doSmartW(target);
                if (target.Health < fullComboDmgOn(target) * 1.3f || isStealthed())
                    doSmartE(target, true);
                else
                    reachWithE(target);
            }
            doSmartR(target);
        }

        public static void doHarass(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget())
                return;

                useHydra(target);
                doQ(target);
                doSmartW(target);
        }

        public static void doQ(Obj_AI_Base target)
        {
            if (inSpellRange(target, Q) && Q.IsReady())
                Q.Cast(target);
        }

        public static void doSmartW(Obj_AI_Base target)
        {
            if (!W.IsReady() || !target.IsValidTarget())
                return;
            PredictionOutput po = W.GetPrediction(target);
            if (po.Hitchance > HitChance.Low)
                W.Cast(po.CastPosition);
        }

        public static void doSmartE(Obj_AI_Base target,bool kill = false)
        {
            if (!E.IsReady() || timeToReachAA(target) < 0.3f || !target.IsValidTarget())
                return;

            Console.WriteLine("do some jumpy");
            PredictionOutput po = E.GetPrediction(target);
            if (po.Hitchance > HitChance.Medium)
                E.Cast((kill)?po.CastPosition:po.UnitPosition);
        }

        public static void reachWithE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            float trueAARange = Player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + E.Range;

            float dist = Player.Distance(target);
            

            float timeToReach = timeToReachAA(target);
            if (dist > trueAARange && (dist < trueERange ))
            {
                if (timeToReach > 2.2f)
                {
                    doSmartE(target);
                }
            }
        }

        public static float timeToReachAA(Obj_AI_Base target)
        {
            float trueAARange = Player.AttackRange + target.BoundingRadius;

            float dist = Player.Distance(target);
            Vector2 walkPos = new Vector2();
            if (target.IsMoving && target.Path.Count()>0)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                walkPos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Player.Distance(walkPos) > dist) ? target.MoveSpeed : 0;
            float msDif = (Player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (Player.MoveSpeed - targ_ms);
            float timeToReach = (dist - trueAARange) / msDif;

            return (timeToReach>=0)?timeToReach:float.MaxValue;
        }

        public static void doSmartR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            var dist = Player.Distance(target);
            if (enemiesNear() > 2 || !gotPassiveDmg() || (timeToReachAA(target) > 1f && targIsKillabe(target)))
                if (Player.Distance(target) < 375 && (!Q.IsReady() || E.IsReady()) && (dist> Q.Range || !Q.IsReady()))
                {
                    R.Cast();
                }
        }

        public static bool useHydra(Obj_AI_Base target)
        {
            if (target.Distance(Player.ServerPosition) < (400 + target.BoundingRadius - 20))
            {
                Items.UseItem(3074, target);
                Items.UseItem(3077, target);
                return true;
            }
            return false;
        }

        public static float getBestRange()
        {
            if (E.IsReady())
                return E.Range + Orbwalking.GetRealAutoAttackRange(Player);
            if (W.IsReady())
                return W.Range;
            return Q.Range;
        }

        public static float fullComboDmgOn(Obj_AI_Base target)
        {
            float dmg = 0f;
            if (gotPassiveDmg())
                dmg +=
                    (float)
                        Player.CalcDamage(target, Damage.DamageType.Magical,
                            10 + 10*Player.Level + 0.5*Player.FlatMagicDamageMod); //DamageLib.CalcMagicDmg(10 + 10* Player.Level + 0.5*Player.FlatMagicDamageMod,target);
            if (Q.IsReady())
            {
                if (targetIsIsolated(target))
                    dmg += Q.GetDamage(target, 1);// DamageLib.getDmg(target, DamageLib.SpellType.Q, DamageLib.StageType.FirstDamage);
                else
                    dmg += Q.GetDamage(target);
            }
            if (W.IsReady())
                dmg += W.GetDamage(target);
            if(E.IsReady())
                dmg += E.GetDamage(target);


            return dmg;
        }

        public static bool targIsKillabe(Obj_AI_Base target)
        {
            return target.Health > fullComboDmgOn(target)*1.2f;
        }
        public static bool gotPassiveDmg()
        {
            return Player.Buffs.Any(buf => buf.Name == "khazixpdamage");
        }

        public static bool isStealthed()
        {
            return Player.Buffs.Any(buf => buf.Name == "khazixrstealth");
        }

        public static int enemiesNear()
        {
            return ObjectManager.Get<Obj_AI_Base>().Count(ene => ene.IsEnemy && ene.IsValidTarget(600));
        }

        public static float targIsReach(Obj_AI_Base target)
        {
            float dist = target.Distance(Player);
            float range = Orbwalking.GetRealAutoAttackRange(target);
            if (Q.IsReady())
                range = Q.Range;
            range+= E.Range;
            return dist-range;
        }

        public static bool targetIsIsolated(Obj_AI_Base target)
        {
            var enes = ObjectManager.Get<Obj_AI_Base>()
                .Where(her => her.IsEnemy && her.NetworkId != target.NetworkId && target.Distance(her) < 500)
                .ToArray();
            return !enes.Any();
        }

        public static bool inSpellRange(Obj_AI_Base target, Spell S)
        {
            var targBB = target.BoundingRadius*0.9f;
            var dist = Vector3.DistanceSquared(target.Position, Player.Position);
            return dist < (targBB + S.Range)*(targBB + S.Range);
        }

    }
}
