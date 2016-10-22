using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace JayceSharpV2
{
    class Jayce
    {
        public static AIHeroClient Player = ObjectManager.Player;


        public static SummonerItems sumItems = new SummonerItems(Player);

        public static Spellbook sBook = Player.Spellbook;


        public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public static Spell Q1 = new Spell(SpellSlot.Q, 1050);//Emp 1470
        public static Spell QEmp1 = new Spell(SpellSlot.Q, 1600);//Emp 1470
        public static Spell W1 = new Spell(SpellSlot.W, 0);
        public static Spell E1 = new Spell(SpellSlot.E, 650);
        public static Spell R1 = new Spell(SpellSlot.R, 0);

        public static Spell Q2 = new Spell(SpellSlot.Q, 600);
        public static Spell W2 = new Spell(SpellSlot.W, 285);
        public static Spell E2 = new Spell(SpellSlot.E, 240);
        public static Spell R2 = new Spell(SpellSlot.R, 0);

        public static GameObjectProcessSpellCastEventArgs castEonQ = null;
        public static int castedTimeUnreach = 0;

        public static MissileClient myCastedQ = null;

        public static AIHeroClient lockedTarg = null;

        public static AIHeroClient castedQon = null;

        public static Vector3 castQon = new Vector3(0, 0, 0);

        /* COOLDOWN STUFF */
        public static float[] rangTrueQcd = { 8, 8, 8, 8, 8, 8 };
        public static float[] rangTrueWcd = { 13, 11.4f, 9.8f, 8.2f, 6.6f, 5 };
        public static float[] rangTrueEcd = { 16, 16, 16, 16, 16, 16 };

        public static float[] hamTrueQcd = { 16, 14, 12, 10, 8, 6 };
        public static float[] hamTrueWcd = { 10, 10, 10, 10, 10, 10 };
        public static float[] hamTrueEcd = { 15, 14, 13, 12, 11, 10 };

        public static float rangQCD = 0, rangWCD = 0, rangECD = 0;
        public static float hamQCD = 0, hamWCD = 0, hamECD = 0;

        public static float rangQCDRem = 0, rangWCDRem = 0, rangECDRem = 0;
        public static float hamQCDRem = 0, hamWCDRem = 0, hamECDRem = 0;


        /* COOLDOWN STUFF END */
        public static bool isHammer = false;

        public static void setSkillShots()
        {
            Q1.SetSkillshot(0.3f, 70f, 1500, true, SkillshotType.SkillshotLine);
            QEmp1.SetSkillshot(0.3f, 70f, 2180, true, SkillshotType.SkillshotLine);
            // QEmp1.SetSkillshot(0.25f, 70f, float.MaxValue, false, Prediction.SkillshotType.SkillshotLine);
        }


        public static void doCombo(AIHeroClient target)
        {
            if (target == null)
                return;
            castOmen(target);
            if (!isHammer)
            {
                //if (castEonQ != null)
                //    castEonSpell(target);
                //DO QE combo first
                if (E1.IsReady() && Q1.IsReady() && gotManaFor(true, false, true))
                {
                    castQEPred(target);
                }
                else if (Q1.IsReady() && gotManaFor(true))
                {
                    castQPred(target);
                }
                else if (W1.IsReady() && gotManaFor(false, true) && targetInRange(getClosestEnem(), 650f))
                {
                    W1.Cast();
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                }//and wont die wih 1 AA
                else if (!Q1.IsReady() && !W1.IsReady() && R1.IsReady() && hammerWillKill(target) && hamQCDRem == 0 && hamECDRem == 0)// will need to add check if other form skills ready
                {
                    R1.Cast();
                }
            }
            else
            {
                if (!Q2.IsReady() && R2.IsReady() && Player.Distance(getClosestEnem()) > 350)
                {
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    R2.Cast();
                }
                if (Q2.IsReady() && gotManaFor(true) && targetInRange(target, Q2.Range) && Player.Distance(target) > 300)
                {
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    Q2.Cast(target);
                }
                if (E2.IsReady() && gotManaFor(false, false, true) && targetInRange(target, E2.Range) && shouldIKnockDatMadaFaka(target))
                {
                    E2.Cast(target);
                }
                if (W2.IsReady() && gotManaFor(false, true) && targetInRange(target, W2.Range))
                {
                    W2.Cast();
                }

            }
        }


        public static void doFullDmg(AIHeroClient target)
        {
            if (target == null)
                return;;
            castIgnite(target);
            if (!isHammer)
            {
                if (castEonQ != null)
                {
                    castEonSpell(target);
                }
                //DO QE combo first
                if (E1.IsReady() && Q1.IsReady() && gotManaFor(true, false, true))
                {
                    castQEPred(target);
                }
                else if (Q1.IsReady() && gotManaFor(true))
                {
                    castQPred(target);
                }
                else if (W1.IsReady() && gotManaFor(false, true) && targetInRange(getClosestEnem(), 1000f))
                {

                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    W1.Cast();
                }
                else if (!Q1.IsReady() && !W1.IsReady() && R1.IsReady() && hamQCDRem == 0 && hamECDRem == 0)// will need to add check if other form skills ready
                {
                    R1.Cast();
                }
            }
            else
            {
                if (!Q2.IsReady() && R2.IsReady() && Player.Distance(getClosestEnem()) > 350)
                {

                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    R2.Cast();
                }
                if (Q2.IsReady() && gotManaFor(true) && targetInRange(target, Q2.Range))
                {
                    Q2.Cast(target);
                }
                if (E2.IsReady() && gotManaFor(false, false, true) && targetInRange(target, E2.Range) && (!gotSpeedBuff()) || (getJayceEHamDmg(target) > target.Health))
                {
                    E2.Cast(target);
                }
                if (W2.IsReady() && gotManaFor(false, true) && targetInRange(target, W2.Range))
                {
                    W2.Cast();
                }

            }
        }

        public static void doJayceInj(AIHeroClient target)
        {
            if (lockedTarg != null)
                target = lockedTarg;
            else
                lockedTarg = target;


            if (isHammer)
            {
                castIgnite(target);

                if (/*inMyTowerRange(posAfterHammer(target)) &&*/ E2.IsReady())
                    E2.Cast(target);

                //If not in flash range  Q to get in it
                if (Player.Distance(target) > 400 && targetInRange(target, 600f))
                    Q2.Cast(target);

                if (!E2.IsReady() && !Q2.IsReady())
                    R2.Cast();
                Obj_AI_Base tower = ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly && tur.Health > 0).OrderBy(tur => Player.Distance(tur)).First();
                if (Player.Distance(getBestPosToHammer(target.ServerPosition)) < 400 && tower.Distance(target)<1500)
                {
                    Player.Spellbook.CastSpell(Player.GetSpellSlot("SummonerFlash"), getBestPosToHammer(target.ServerPosition));
                }
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
            else
            {
                if (E1.IsReady() && Q1.IsReady() && gotManaFor(true, false, true))
                {
                    PredictionOutput po = QEmp1.GetPrediction(target);
                    var dist = Player.Distance(po.UnitPosition);
                    if (dist <= E1.Range && getJayceEQDmg(target)<target.Health)
                    {
                       // if (JayceSharp.Config.Item("useExploit").GetValue<bool>())
                       //     doExploit(target);
                       // else
                        if (shootQE(po.CastPosition, dist>550))
                            castedQon = target;
                    }
                    else
                    {
                        if (po.Hitchance >= HitChance.Medium && Player.Distance(po.UnitPosition) < (QEmp1.Range + target.BoundingRadius))
                        {
                            castQon = po.CastPosition;
                            castedQon = target;
                        }
                    }

                    // QEmp1.CastIfHitchanceEquals(target, Prediction.HitChance.HighHitchance);
                }
                else if (Q1.IsReady() && gotManaFor(true) && !E1.IsReady(1000))
                {
                    if (Q1.Cast(target.Position))
                        castedQon = target;
                }
                else if (W1.IsReady() && gotManaFor(false, true) && targetInRange(getClosestEnem(), 1000f))
                {
                    W1.Cast();
                }
            }
        }


      /*  public static Vector3 posAfterInj(Obj_AI_Base target)
        {
            Vector3 ve = getBestPosToHammer(target.ServerPosition);
            return posAfterHammer()
        }*/


        public static void doKillSteal()
        {
            try
            {
                if (rangQCDRem == 0 && rangECDRem == 0 && gotManaFor(true, false, true))
                {
                    List<AIHeroClient> deadEnes = ObjectManager.Get<AIHeroClient>().Where(ene => getJayceEQDmg(ene) > ene.Health && ene.IsEnemy && ene.IsValid && ene.Distance(Player.ServerPosition) < 1800).ToList();
                    foreach (var enem in deadEnes)
                    {
                        if (Player.Distance(enem) < 300)
                            continue;
                        if (QEmp1.GetPrediction(enem).Hitchance >= HitChance.Medium)
                        {
                            if (isHammer && R2.IsReady())
                            {
                                R2.Cast();
                            }
                            castQEPred(enem);
                        }
                    }
                }
                else if (rangQCDRem == 0 && gotManaFor(true))
                {
                    List<AIHeroClient> deadEnes = ObjectManager.Get<AIHeroClient>().Where(ene => getJayceQDmg(ene) > ene.Health && ene.IsEnemy && ene.IsValid && ene.Distance(Player.ServerPosition) < 1200).ToList();
                    foreach (var enem in deadEnes)
                    {
                        if (Q1.GetPrediction(enem).Hitchance >= HitChance.Medium)
                        {
                            if (isHammer && R2.IsReady())
                            {
                                R2.Cast();
                            }
                            castQPred(enem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public static void castQEPred(AIHeroClient target)
        {
            if (isHammer)
                return;
            PredictionOutput po = QEmp1.GetPrediction(target);
            var dist = Player.Distance(po.UnitPosition);
            if (po.Hitchance >= HitChance.Low && dist < (QEmp1.Range + target.BoundingRadius))
            {
               // if()
                //doExploit(target);
               // else
               // {
                    if(shootQE(po.CastPosition, dist>550))
                        castedQon = target;
               // }
            }
            else if (po.Hitchance == HitChance.Collision && JayceSharp.Config.Item("useMunions").GetValue<bool>())
            {
                Obj_AI_Base fistCol = po.CollisionObjects.OrderBy(unit => unit.Distance(Player.ServerPosition)).First();
                if (fistCol.Distance(po.UnitPosition) < (180 - fistCol.BoundingRadius / 2) && fistCol.Distance(target.ServerPosition) < (180 - fistCol.BoundingRadius / 2))
                {
                    shootQE(po.CastPosition);
                }
            }
        }

        public static void castQPred(AIHeroClient target)
        {
            if (isHammer)
                return;
            PredictionOutput po = Q1.GetPrediction(target);
            if (po.Hitchance == HitChance.Collision && JayceSharp.Config.Item("useMunions").GetValue<bool>())
            {
                Obj_AI_Base fistCol = po.CollisionObjects.OrderBy(unit => unit.Distance(Player.ServerPosition)).First();
                if (fistCol.Distance(po.UnitPosition) < (180 - fistCol.BoundingRadius / 2) && fistCol.Distance(target.ServerPosition) < (100 - fistCol.BoundingRadius / 2))
                {
                    if (Q1.Cast(po.CastPosition))
                        castedQon = target;
                }

            }
            else
            {
                Q1.Cast(target);
            }
        }

        public static Vector3 getBestPosToHammer(Vector3 target)
        {
            Obj_AI_Base tower = ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly && tur.Health > 0).OrderBy(tur => Player.Distance(tur)).First();
            return target + Vector3.Normalize(tower.ServerPosition - target) * (-120);
        }

        public static Vector3 posAfterHammer(Obj_AI_Base target)
        {
            return getBestPosToHammer(target.ServerPosition) + Vector3.Normalize(getBestPosToHammer(target.ServerPosition) - Player.ServerPosition) * 600;
        }

        public static AIHeroClient getClosestEnem()
        {
            return ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsEnemy && ene.IsValidTarget()).OrderBy(ene => Player.Distance(ene)).First();
        }

        public static float getBestRange()
        {
            float range = 0;
            if (!isHammer)
            {
                if (Q1.IsReady() && E1.IsReady() && gotManaFor(true, false, true))
                {
                    range = 1750;
                }
                else if (Q1.IsReady() && gotManaFor(true))
                {
                    range = 1150;
                }
                else
                {
                    range = 500;
                }
            }
            else
            {
                if (Q1.IsReady() && gotManaFor(true))
                {
                    range = 600;
                }
                else
                {
                    range = 300;
                }
            }
            return range + 50;
        }




        public static bool shootQE(Vector3 pos,bool man = false)
        {
            try
            {
                if (isHammer && R2.IsReady())
                    R2.Cast();
                if (!E1.IsReady() || !Q1.IsReady() || isHammer)
                    return false;

                if (JayceSharp.Config.Item("packets").GetValue<bool>())
                {
                    packetCastQ(pos.To2D());
                    //packetCastE(getParalelVec(pos));
                }
                else
                {
                    Vector3 bPos = Player.ServerPosition - Vector3.Normalize(pos - Player.ServerPosition)*50;

                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, bPos);
                    Q1.Cast(pos);
                    if(man)
                        E1.Cast(getParalelVec(pos));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        public static bool shouldIKnockDatMadaFaka(AIHeroClient target)
        {
            //if (useSmartKnock(target) && R2.IsReady() && target.CombatType == GameObjectCombatType.Melee)
            // {
            //  return true;
            // }
            float damageOn = getJayceEHamDmg(target);

            if (damageOn > target.Health * 0.9f)
            {
                return true;
            }
            if (((Player.Health / Player.MaxHealth) < 0.15f) /*&& target.CombatType == GameObjectCombatType.Melee*/)
            {
                return true;
            }
            Vector3 posAfter = target.ServerPosition + Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * 450;
            if (inMyTowerRange(posAfter))
            {
                return true;
            }

            return false;
        }

        public static bool useSmartKnock(AIHeroClient target)
        {
            float trueAARange = Player.BoundingRadius + target.AttackRange;
            float trueERange = target.BoundingRadius + E2.Range;

            float dist = Player.Distance(target);
            Vector2 movePos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                movePos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Player.Distance(movePos) < dist) ? target.MoveSpeed : 0;
            float msDif = (Player.MoveSpeed * 0.7f - targ_ms) == 0 ? 0.0001f : (targ_ms - Player.MoveSpeed * 0.7f);
            float timeToReach = (dist - trueAARange) / msDif;
            if (dist > trueAARange && dist < trueERange && target.IsMoving)
            {
                if (timeToReach > 1.7f || timeToReach < 0.0f)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool inMyTowerRange(Vector3 pos)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly && tur.Health > 0).Any(tur => pos.Distance(tur.Position) < (850 + Player.BoundingRadius));
        }

        public static void castEonSpell(GameObject mis)
        {
            if (isHammer || !E1.IsReady())
                return;
            if (Player.Distance(myCastedQ.Position) < 250)
            {
                E1.Cast(getParalelVec(mis.Position));
            }

        }


        public static bool targetInRange(Obj_AI_Base target, float range)
        {
            if (target == null)
                return false;
            float dist2 = Vector2.DistanceSquared(target.ServerPosition.To2D(), Player.ServerPosition.To2D());
            float range2 = range * range + target.BoundingRadius * target.BoundingRadius;
            return dist2 < range2;
        }

        public static void checkForm()
        {
            isHammer = !Qdata.SData.Name.ToLower().Contains("jayceshockblast");
        }


        public static bool gotSpeedBuff()//jaycehypercharge
        {
            return Player.Buffs.Any(bi => bi.Name.ToLower().Contains("jaycehypercharge"));
        }

        public static Vector2 getParalelVec(Vector3 pos)
        {
            if (JayceSharp.Config.Item("parlelE").GetValue<bool>())
            {
                Random rnd = new Random();
                int neg = rnd.Next(0, 1);
                int away = JayceSharp.Config.Item("eAway").GetValue<Slider>().Value;
                away = (neg == 1) ? away : -away;
                var v2 = Vector3.Normalize(pos - Player.ServerPosition) * away;
                var bom = new Vector2(v2.Y, -v2.X);
                return Player.ServerPosition.To2D() + bom;
            }
            else
            {
                var dpos = Player.Distance(pos);
                var v2 = Vector3.Normalize(pos - Player.ServerPosition) * ((dpos<300)?dpos+10:300);
                var bom = new Vector2(v2.X, v2.Y);
                return Player.ServerPosition.To2D() + bom;
            }
        }

        //Need to fix!!
        public static bool gotManaFor(bool q = false, bool w = false, bool e = false)
        {
            float manaNeeded = 0;
            if (q)
                manaNeeded += Qdata.SData.Mana;
            if (w)
                manaNeeded += Wdata.SData.Mana;
            if (e)
                manaNeeded += Edata.SData.Mana;
            // Console.WriteLine("Mana: " + manaNeeded);
            return manaNeeded <= Player.Mana;
        }

        public static float calcRealCD(float time)
        {
            return time + (time * Player.PercentCooldownMod);
        }

        public static void processCDs()
        {
            hamQCDRem = ((hamQCD - Game.Time) > 0) ? (hamQCD - Game.Time) : 0;
            hamWCDRem = ((hamWCD - Game.Time) > 0) ? (hamWCD - Game.Time) : 0;
            hamECDRem = ((hamECD - Game.Time) > 0) ? (hamECD - Game.Time) : 0;

            rangQCDRem = ((rangQCD - Game.Time) > 0) ? (rangQCD - Game.Time) : 0;
            rangWCDRem = ((rangWCD - Game.Time) > 0) ? (rangWCD - Game.Time) : 0;
            rangECDRem = ((rangECD - Game.Time) > 0) ? (rangECD - Game.Time) : 0;
        }

        public static void getCDs(GameObjectProcessSpellCastEventArgs spell)
        {
            try
            {
                //Console.WriteLine(spell.SData.Name + ": " + Q2.Level);

                if (spell.SData.Name == "JayceToTheSkies")
                    hamQCD = Game.Time + calcRealCD(hamTrueQcd[Q2.Level - 1]);
                if (spell.SData.Name == "JayceStaticField")
                    hamWCD = Game.Time + calcRealCD(hamTrueWcd[W2.Level - 1]);
                if (spell.SData.Name == "JayceThunderingBlow")
                    hamECD = Game.Time + calcRealCD(hamTrueEcd[E2.Level - 1]);

                if (spell.SData.Name.ToLower() == "jayceshockblast")
                    rangQCD = Game.Time + calcRealCD(rangTrueQcd[Q1.Level - 1]);
                if (spell.SData.Name.ToLower() == "jaycehypercharge")
                    rangWCD = Game.Time + calcRealCD(rangTrueWcd[W1.Level - 1]);
                if (spell.SData.Name.ToLower() == "jayceaccelerationgate")
                    rangECD = Game.Time + calcRealCD(rangTrueEcd[E1.Level - 1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void drawCD()
        {
            var pScreen = Drawing.WorldToScreen(Player.Position);

            // Drawing.DrawText(Drawing.WorldToScreen(Player.Position)[0], Drawing.WorldToScreen(Player.Position)[1], System.Drawing.Color.Green, "Q: wdeawd ");
            pScreen[0] -= 20;

            if (isHammer)
            {
                if (rangQCDRem == 0)
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Green, "Q: Rdy");
                else
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Red, "Q: " + rangQCDRem.ToString("0.0"));

                if (rangWCDRem == 0)
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Green, "W: Rdy");
                else
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Red, "W: " + rangWCDRem.ToString("0.0"));

                if (rangECDRem == 0)
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Green, "E: Rdy");
                else
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Red, "E: " + rangECDRem.ToString("0.0"));
            }
            else
            {
                // pScreen.Y += 30;
                if (hamQCDRem == 0)
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Green, "Q: Rdy");
                else
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Red, "Q: " + hamQCDRem.ToString("0.0"));

                if (hamWCDRem == 0)
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Green, "W: Rdy");
                else
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Red, "W: " + hamWCDRem.ToString("0.0"));

                if (hamECDRem == 0)
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Green, "E: Rdy");
                else
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Red, "E: " + hamECDRem.ToString("0.0"));
            }
        }


        public static void packetCastQ(Vector2 pos)
        {
            Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(0, SpellSlot.Q, Player.NetworkId, pos.X, pos.Y, Player.ServerPosition.X, Player.ServerPosition.Y)).Send();
        }

        public static void packetCastE(Vector2 pos)
        {
            Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(0, SpellSlot.E, Player.NetworkId, pos.X, pos.Y, Player.Position.X, Player.Position.Y)).Send();
        }

        public static void knockAway(Obj_AI_Base target)
        {
            if (!targetInRange(target, 270) || hamECDRem != 0 || E1.Level == 0)
                return;

            if (!isHammer && R2.IsReady())
                R1.Cast();
            if (isHammer && E2.IsReady() && targetInRange(target, 260))
                E2.Cast(target);

        }

        public static bool hammerWillKill(Obj_AI_Base target)
        {
            if (!JayceSharp.Config.Item("hammerKill").GetValue<bool>() || target == null)
                return false;
            float damage = (float)Player.GetAutoAttackDamage(target) + 50;
            damage += getJayceEHamDmg(target);
            damage += getJayceQHamDmg(target);

            return (target.Health < damage);
        }


        public static float getJayceFullComoDmg(Obj_AI_Base target)
        {
            if (target == null)
                return 0f;
            float dmg = 0;
            //Ranged
            if (!isHammer || R1.IsReady())
            {
                if (rangECDRem == 0 && rangQCDRem == 0 && Q1.Level != 0 && E1.Level != 0)
                {
                    dmg += getJayceEQDmg(target);
                }
                else if (rangQCDRem == 0 && Q1.Level != 0)
                {
                    dmg += getJayceQDmg(target);
                }
                float hyperMulti = W1.Level * 0.15f + 0.7f;
                if (rangWCDRem == 0 && W1.Level != 0)
                {
                    dmg += getJayceAADmg(target) * 3 * hyperMulti;
                }
            }
            //Hamer
            if (isHammer || R1.IsReady())
            {
                if (hamECDRem == 0 && E2.Level != 0)
                {
                    dmg += getJayceEHamDmg(target);
                }
                if (hamQCDRem == 0 && Q2.Level != 0)
                {
                    dmg += getJayceQHamDmg(target);
                }
            }
            return dmg;
        }

        public static float getJayceAADmg(Obj_AI_Base target)
        {
            return (float)Player.GetAutoAttackDamage(target);

        }

        public static float getJayceEQDmg(Obj_AI_Base target)
        {
            return
                (float)
                    Player.CalcDamage(target, Damage.DamageType.Physical,
                        (7 + (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level * 77)) +
                        (1.68 * ObjectManager.Player.FlatPhysicalDamageMod));


        }

        public static float getJayceQDmg(Obj_AI_Base target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Physical,
                                    (5 + (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level * 55)) +
                                    (1.2 * ObjectManager.Player.FlatPhysicalDamageMod));
        }

        public static float getJayceEHamDmg(Obj_AI_Base target)
        {
            if (target == null)
                return 0f;
            double percentage = 5 + (3 * Player.Spellbook.GetSpell(SpellSlot.E).Level);
            return (float)Player.CalcDamage(target, Damage.DamageType.Magical,
                    ((target.MaxHealth / 100) * percentage) + (ObjectManager.Player.FlatPhysicalDamageMod));
        }

        public static float getJayceQHamDmg(Obj_AI_Base target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Physical,
                                (-25 + (Player.Spellbook.GetSpell(SpellSlot.Q).Level * 45)) +
                                (1.0 * Player.FlatPhysicalDamageMod));
        }

        public static void castIgnite(AIHeroClient target)
        {
            if (targetInRange(target, 600) && (target.Health / target.MaxHealth) * 100 < 25)
                sumItems.castIgnite(target);
        }

        public static void castOmen(AIHeroClient target)
        {
            if (Player.Distance(target) < 430)
                sumItems.cast(SummonerItems.ItemIds.Omen);
        }

        public static void activateMura()
        {
            if (Player.Buffs.Count(buf => buf.Name == "Muramana") == 0)
                sumItems.cast(SummonerItems.ItemIds.Muramana);
        }

        public static void deActivateMura()
        {
            if (Player.Buffs.Count(buf => buf.Name == "Muramana") != 0)
                sumItems.cast(SummonerItems.ItemIds.Muramana);
        }

    }
}
