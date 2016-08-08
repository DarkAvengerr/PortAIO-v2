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
    class Riven : Champion
    {

        public static bool rushDown = false;

        public static bool rushDownQ = false;

        public static bool forceQ = false;

        public Riven()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Mercurys_Treads,ItemId.Ninja_Tabi,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only),
                            new ConditionalItem(ItemId.The_Black_Cleaver),
                            new ConditionalItem(ItemId.Youmuus_Ghostblade),
                            new ConditionalItem(ItemId.Maw_of_Malmortius),
                            new ConditionalItem(ItemId.Banshees_Veil,ItemId.Randuins_Omen,ItemCondition.ENEMY_AP)
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Tiamat_Melee_Only
                        }
            };
            DeathWalker.AfterAttack += DeathWalkerOnAfterAttack;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Obj_AI_Base.OnNewPath += OnNewPath;
        }

        private void OnPlayAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe && args.Animation != "Run")
            {
                
                if (args.Animation.Contains("pell"))
                    LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping, delegate { cancelAnim(args.Animation.Contains("Spell1")); });

                var targ = (Obj_AI_Base)DeathWalker.getBestTarget();
                if (targ != null && targ is Obj_AI_Base)
                {
                    if (args.Animation == "Spell3" && R.LSIsReady())
                    {
                        useRSmart(targ, true);
                        //LeagueSharp.Common.Utility.DelayAction.Add(10,
                        //    delegate { Riven.useRSmart(targ,true); });
                    }

                    if (sender.IsMe && args.Animation == "Spell3" &&
                        Q.LSIsReady())
                    {
                        Console.WriteLine("force W");
                        LeagueSharp.Common.Utility.DelayAction.Add(30, delegate { useWSmart(targ, false, true); });
                        //Riven.Q.Cast(targ.Position);
                        //Riven.forceQ = true;
                        // Riven.timer = new System.Threading.Timer(obj => { Riven.EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Riven.difPos()); }, null, (long)100, System.Threading.Timeout.Infinite);
                    }

                    if (sender.IsMe && args.Animation == "Spell2" &&
                        Q.LSIsReady())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(30, delegate { Q.Cast(targ.Position); });
                        Aggresivity.addAgresiveMove(new AgresiveMove(30, 3000));
                        //Console.WriteLine("force q");

                        // Riven.forceQ = true;
                        // Riven.timer = new System.Threading.Timer(obj => { Riven.EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Riven.difPos()); }, null, (long)100, System.Threading.Timeout.Infinite);
                    }


                    // useHydra(Obj_AI_Base target)

                }
            }
        }
        public bool resetAaonNewPath = false;

        private void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe && !args.IsDash)
            {

                if (resetAaonNewPath)
                {
                    resetAaonNewPath = false;
                    DeathWalker.resetAutoAttackTimer();
                }
            }
        }

        public void cancelAnim(bool aaToo = false)
        {
            if (aaToo)
            {
                resetAaonNewPath = true;
            }
            Chat.Say("/d");
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos); 
            if (DeathWalker.getBestTarget() != null)
            {
                if (W.LSIsReady())
                    useWSmart((Obj_AI_Base)DeathWalker.getBestTarget());

            }


            //  Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(fill iterator up)).Send();
        }

        private void DeathWalkerOnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (target is AIHeroClient)
            {
                Q.Cast(target.Position);
                Aggresivity.addAgresiveMove(new AgresiveMove(30, 3000));
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
        }

        public override void useW(Obj_AI_Base target)
        {
        }

        public override void useE(Obj_AI_Base target)
        {
        }


        public override void useR(Obj_AI_Base target)
        {
        }

        public override void farm()
        {
            Obj_AI_Base target = (Obj_AI_Base)DeathWalker.getBestTarget();
            if (target is Obj_AI_Minion && W.IsKillable(target))
            {
                useWSmart(target);
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(getRivenReach()+430);
            doCombo(tar);
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 280);
            W = new Spell(SpellSlot.W, 260 + 25);
            E = new Spell(SpellSlot.E, 390); 
            R = new Spell(SpellSlot.R, 900);

            R.SetSkillshot(0.25f, 300f, 1400f, false, SkillshotType.SkillshotCone);
        }

        public float getRivenReach()
        {
            int Qtimes = getQJumpCount();
            return player.AttackRange + Qtimes*200 + (E.LSIsReady() ? 390 : 0);
        }

        public  void doCombo(Obj_AI_Base target)
        {
            if (target == null)
                return;
            
            rushDownQ =  rushDmgBasedOnDist(target) * 0.7f > target.Health;
            rushDown = rushDmgBasedOnDist(target) * 1.1f > target.Health;
            if (rushDown || player.LSCountEnemiesInRange(600)>2)
                useRSmart(target);
            if(rushDown || safeGap(target))
                useESmart(target);
            useWSmart(target);
            if (DeathWalker.canMove() && (target.LSDistance(player)<700 || rushDown))
                gapWithQ(target);
        }

        public void gapWithQ(Obj_AI_Base target)
        {
            if ((E.LSIsReady() || !Q.LSIsReady()) && !rushDownQ || player.LSIsDashing())
                return;
            reachWithQ(target);
        }

        public void reachWithQ(Obj_AI_Base target)
        {
            if (!Q.LSIsReady() || player.LSIsDashing())
                return;

            float trueAARange = player.AttackRange + target.BoundingRadius + 20;
            float trueQRange = target.BoundingRadius + Q.Range + 30;

            float dist = player.LSDistance(target);
            Vector2 walkPos = new Vector2();
            if (target.IsMoving && target.Path.Count() != 0)
            {
                Vector2 tpos = target.Position.LSTo2D();
                Vector2 path = target.Path[0].LSTo2D() - tpos;
                path.Normalize();
                walkPos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && player.LSDistance(walkPos) > dist) ? target.MoveSpeed : 0;
            float msDif = (player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (player.MoveSpeed - targ_ms);
            float timeToReach = (dist - trueAARange) / msDif;
            if ((dist > trueAARange && dist < trueQRange) || rushDown)
            {
                if (timeToReach > 2.5 || timeToReach < 0.0f || rushDown)
                {
                    Vector2 to = player.Position.LSTo2D().LSExtend(target.Position.LSTo2D(), 50);
                    // EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo,to.To3D());
                    Q.Cast(target.ServerPosition);
                    Aggresivity.addAgresiveMove(new AgresiveMove(30, 3000));
                }
            }
        }

        public void useWSmart(Obj_AI_Base target, bool aaRange = false, bool rrAa = false)
        {
            if (!W.LSIsReady())
                return;
            float range = 0;
            if (aaRange)
                range = player.AttackRange + target.BoundingRadius;
            else
                range = W.Range + target.BoundingRadius - 40;
            if (W.LSIsReady() && target.LSDistance(player.ServerPosition) < range)
            {
                W.Cast();
                //LeagueSharp.Common.Utility.DelayAction.Add(50, delegate { DeathWalker.resetAutoAttackTimer(true); });
            }
        }

        public void useESmart(Obj_AI_Base target)
        {
            if (!E.LSIsReady())
                return;



            float trueAARange = player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + E.Range;



            float dist = player.LSDistance(target);

            var path = player.GetPath(target.Position);
            if (!target.IsMoving && dist < trueERange)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                E.Cast(path.Count() > 1 ? path[1] : target.ServerPosition);
            }
            if ((dist > trueAARange && dist < trueERange) || rushDown)
            {

                E.Cast(path.Count() > 1 ? path[1] : target.ServerPosition);
            }
        }

        public void useRSmart(Obj_AI_Base target, bool rrAA = false)
        {
            if (!R.LSIsReady())
                return;
            if (!ultIsOn() && !E.LSIsReady() && target.LSDistance(player.ServerPosition) < (Q.Range + target.BoundingRadius))
            {
                R.Cast();
                Aggresivity.addAgresiveMove(new AgresiveMove(150,8000));
                if (rrAA)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                    //LeagueSharp.Common.Utility.DelayAction.Add(50, delegate { DeathWalker.resetAutoAttackTimer(true); } );
                }
            }
            else if (canUseWindSlash() && target is AIHeroClient && (!(E.LSIsReady() && player.LSIsDashing()) || player.LSDistance(target) > 150))
            {
                var targ = target as AIHeroClient;
                PredictionOutput po = R.GetPrediction(targ, true);
                if (getTrueRDmgOn(targ) > ((targ.Health)) || rushDown)
                {
                    if (po.Hitchance > HitChance.Medium && player.LSDistance(po.UnitPosition) > 30)
                    {
                        R.Cast(player.LSDistance(po.UnitPosition) < 150 ? target.Position : po.UnitPosition);

                    }
                }
            }
        }

        public float getTrueQDmOn(Obj_AI_Base target)
        {
            return (float)player.CalcDamage(target, Damage.DamageType.Physical, -10 + (Q.Level * 20) +
                                                                          (0.35 + (Q.Level * 0.05)) *
                                                                          (player.FlatPhysicalDamageMod +
                                                                           player.BaseAttackDamage));
        }

        public float rushDmgBasedOnDist(Obj_AI_Base target)
        {
            float multi = 1.0f;
            if (!ultIsOn() && R.LSIsReady())
                multi = 1.2f;
            float Qdmg = getTrueQDmOn(target);
            float Wdmg = (E.LSIsReady()) ? (float)player.LSGetSpellDamage(target, SpellSlot.W) : 0;
            float ADdmg = (float)player.LSGetAutoAttackDamage(target);
            float Rdmg = (R.LSIsReady() && (canUseWindSlash() || !ultIsOn())) ? getTrueRDmgOn(target) : 0;

            float trueAARange = player.AttackRange + target.BoundingRadius - 15;
            float dist = player.LSDistance(target.ServerPosition);
            float Ecan = (E.LSIsReady()) ? E.Range : 0;
            int Qtimes = getQJumpCount();
            int ADtimes = 0;

            if (E.LSIsReady())
                ADtimes++;


            dist -= Ecan;
            dist -= trueAARange;
            while (dist > 0 && Qtimes > 0)
            {
                dist -= player.AttackRange + 50;
                Qtimes--;
            }
            if (dist < 0)
                ADtimes++;
            
            return (Qdmg * Qtimes + Wdmg + ADdmg * ADtimes + Rdmg) * multi;
        }

        public float getTrueRDmgOn(Obj_AI_Base target, float minus = 0)
        {
            float baseDmg = 40 + 40 * R.Level + 0.6f * player.FlatPhysicalDamageMod;
            float eneMissHpProc = ((((target.MaxHealth - target.Health - minus) / target.MaxHealth) * 100f) > 75f) ? 75f : (((target.MaxHealth - target.Health) / target.MaxHealth) * 100f);

            float multiplier = 1 + (eneMissHpProc * 2.66f) / 100;

            return (float)player.CalcDamage(target, Damage.DamageType.Physical, baseDmg * multiplier);
        }

        public bool ultIsOn()
        {
            foreach (var buf in player.Buffs)
            {
                if (buf.Name == "RivenFengShuiEngine")
                {
                    return true;
                }
            }
            return false;
        }

        public bool canUseWindSlash()
        {
            foreach (var buf in player.Buffs)
            {
                if (buf.Name == "rivenwindslashready")
                {
                    return true;
                }
            }
            return false;
        }

        public int getQJumpCount()
        {
            try
            {
                var buff = player.Buffs.First(buf => buf.Name == "RivenTriCleave");

                return 3 - buff.Count;
            }
            catch (Exception ex)
            {
                if (!Q.LSIsReady())
                    return 0;
                return 3;
            }
        }
    }
}
