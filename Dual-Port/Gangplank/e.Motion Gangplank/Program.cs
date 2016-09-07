using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using SharpDX.Direct3D;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace e.Motion_Gangplank
{
    class Program
    {
        #region Declaration

        //private static int BarrelTime;
        private static Random Rand = new Random();
        private static DelayManager QDelay;
        private static AIHeroClient UltimateTarget;
        private static bool UltimateToBeUsed;
        private static Dictionary<string, BuffType> Buffs = new Dictionary<string, BuffType>()
        {
            {"charm",BuffType.Charm},
            {"slow",BuffType.Slow },
            {"poison",BuffType.Poison},
            {"blind",BuffType.Blind},
            {"silence",BuffType.Silence},
            {"stun",BuffType.Stun},
            {"fear",BuffType.Flee},
            {"polymorph",BuffType.Polymorph},
            {"snare",BuffType.Snare},
            {"taunt",BuffType.Taunt},
            {"suppression",BuffType.Suppression}
        };
        private static readonly List<Vector2> BarrelPositions = new List<Vector2>()
        {
            new Vector2(1205, 12097),
            new Vector2(1335, 12468),
            new Vector2(1577, 12820),
            new Vector2(1872, 13011),
            new Vector2(2252, 13299),
            new Vector2(2632, 13520)
        };
        public static Spell Q, W, E, R;
        public static AIHeroClient Player => ObjectManager.Player;
        public static List<Barrel> AllBarrel = new List<Barrel>();
        public static Vector3 EnemyPosition;
        
        
        #endregion


        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Gangplank")
            {
                return;
            }
            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R);
            #region Menu

            Config Menu = new Config();
            Menu.Initialize();
            #endregion

            QDelay = new DelayManager(Q,1500);
            Chat.Print("<font color='#bb0000'>e</font>.<font color='#0000cc'>Motion</font> Gangplank loaded");
            //SetBarrelTime();
             

            #region Subscriptions

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += GameOnUpdate;
            GameObject.OnCreate += OnCreate;
            Obj_AI_Base.OnSpellCast += CheckForBarrel;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
           #endregion

        }

        private static void ForceCast(AIHeroClient target, Vector3 barrelPosition)
        {
            E.Cast(barrelPosition.ExtendToMaxRange(Player.Position.ExtendToMaxRange(target.Position, 980), 685));
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.Q && E.IsReady(200) && args.Target.Name == "Barrel")
            {
                List<Barrel> barrelsInRange = GetBarrelsInRange(AllBarrel.Find(b => b.GetNetworkID() == args.Target.NetworkId)).ToList();
                if (Config.Item("combo.triplee").GetValue<bool>() && barrelsInRange.Any())
                {
                    //Triple-Logic
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        if (enemy.Position.Distance(args.Target.Position) >= 350 &&
                            !barrelsInRange.Any(b => b.GetBarrel().Distance(enemy) <= 350) &&
                             barrelsInRange.Any(b => b.GetBarrel().Distance(enemy) <= 850))
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(400 + Game.Ping/2, () => ForceCast(enemy,barrelsInRange.First(b => b.GetBarrel().Distance(enemy) >= 350 && b.GetBarrel().Distance(enemy) <= 850).GetBarrel().Position));
                        }
                    }
                }
                Barrel attackedBarrel = AllBarrel.Find(b => b.GetNetworkID() == args.Target.NetworkId);
                if (Config.Item("combo.doublee").GetValue<bool>() && attackedBarrel.GetBarrel().Distance(Player) >= 610)
                {
                    //Double Logic
                    foreach (var enemy in HeroManager.Enemies)
                    {
                        if (args.Target.Position.Distance(enemy.Position) >= 350 && args.Target.Position.Distance(enemy.Position) <= 850)
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(200 + Game.Ping / 2, () => ForceCast(enemy,args.Target.Position));
                        }
                    }
                }
                
            }
        }


        private static void OnDraw(EventArgs args)
        {
            KillstealDrawing();
            Warning();
            DrawE();
        }

        private static void KillSteal()
        {
            if (Config.Item("killsteal.q").GetValue<bool>() && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (enemy.Health < Q.GetDamage(enemy) && Player.Distance(enemy) <= Q.Range)
                    {
                        Q.Cast(enemy);
                    }
                }
            }
            if (Config.Item("killsteal.r").GetValue<bool>() && Config.Item("key.r").GetValue<KeyBind>().Active && R.IsReady() && UltimateToBeUsed && UltimateTarget != null)
            {
                R.Cast(SPrediction.Prediction.GetFastUnitPosition(UltimateTarget,150));
            }
        }

        private static void CheckForBarrel(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target != null && args.Target.Name == "Barrel")
            {
                for (int i = 0; i < AllBarrel.Count; i++)
                {
                    
                    if (AllBarrel.ElementAt(i).GetBarrel().NetworkId == args.Target.NetworkId)
                    {
                        if (sender.IsMelee)
                        {
                            AllBarrel.ElementAt(i).ReduceBarrelAttackTick();
                        }
                        else
                        {
                            int i1 = i;
                            LeagueSharp.Common.Utility.DelayAction.Add((int)(args.Start.Distance(args.End)/args.SData.MissileSpeed), () => { AllBarrel.ElementAt(i1).ReduceBarrelAttackTick(); });
                        }
                    }
                }
            }
        }

        private static void CleanBarrel()
        {
            for (int i = AllBarrel.Count - 1; i >= 0; i--)
            {
                //Console.WriteLine("Looped");
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (AllBarrel.ElementAt(i).GetBarrel() == null || AllBarrel.ElementAt(i).GetBarrel().Health == 0)
                {
                    AllBarrel.RemoveAt(i);
                    break;
                }
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                AllBarrel.Add(new Barrel((Obj_AI_Minion)sender));
            }
        }
        

        private static void GameOnUpdate(EventArgs args)
        {
            KillSteal();
            Harass();
            QDelay.CheckEachTick();
            AutoE();
            CleanBarrel();
            Combo();
            Lasthit();
            Cleanse();
            SemiAutomaticE();
        }

        private static void SemiAutomaticE()
        {
            if (E.IsReady() && Config.Item("key.eenabled").GetValue<bool>() &&
                Config.Item("key.e").GetValue<KeyBind>().Active)
            {
                float lowest = 1600;
                Vector3 bPos = Vector3.Zero;
                foreach (Barrel barrel in AllBarrel)
                {
                    if (barrel.GetBarrel().Distance(Game.CursorPos) < lowest)
                    {
                        bPos = barrel.GetBarrel().Position;
                        lowest = barrel.GetBarrel().Distance(Game.CursorPos);
                    }
                }
                if (lowest != 1600f)
                {
                    E.Cast(bPos.Extend(Game.CursorPos, Math.Min(685, lowest)));
                }
            }
        }

        private static void Harass()
        {
            if (Q.IsReady() && Config.Item("harass.q").GetValue<bool>() && Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                AIHeroClient target = TargetSelector.GetTarget(685, TargetSelector.DamageType.Physical);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
        }

        private static void AutoE()
        {
            //Auto E - Static List
            if (Config.Item("misc.autoE").GetValue<bool>() && E.IsReady() && E.Instance.Ammo > 1 && !AllBarrel.Any(b => b.GetBarrel().Distance(Player) <= 1200))
            {
                AIHeroClient target = TargetSelector.GetTarget(1400,TargetSelector.DamageType.Physical);
                List<Vector2> possiblePositions = BarrelPositions.Where(pos => pos.Distance(Player) <= E.Range).ToList();
                if (target != null && possiblePositions.Count != 0)
                {
                    float minDist = 2000;
                    Vector2 castPos = Vector2.Zero;
                    foreach (var pos in possiblePositions.Where(pos => pos.Distance(target) < minDist))
                    {
                        castPos = new Vector2(pos.X + Rand.Next(0,21) - 10, pos.Y + Rand.Next(0,21) - 10);
                        minDist = pos.Distance(target);
                    }
                    E.Cast(castPos);
                    
                }
                
            }
        }

        private static void DrawE()
        {
            if (E.IsReady() && Config.Item("drawings.ex").GetValue<bool>())
            {
                float lowest = 1600;
                Vector3 bPos = Vector3.Zero;
                foreach (Barrel barrel in AllBarrel)
                {
                    if (barrel.GetBarrel().Distance(Game.CursorPos) < lowest)
                    {
                        bPos = barrel.GetBarrel().Position;
                        lowest = barrel.GetBarrel().Distance(Game.CursorPos);
                    }
                }
                if (lowest != 1600f)
                {
                    Render.Circle.DrawCircle(bPos.ExtendToMaxRange(Game.CursorPos,685),350,Color.ForestGreen);
                    Drawing.DrawLine(Drawing.WorldToScreen(bPos),Drawing.WorldToScreen(bPos.ExtendToMaxRange(Game.CursorPos,685)),5,Color.ForestGreen);
                }
            }
        }

        private static void Warning()
        {
            if ((Player.Position.Distance(new Vector3(394, 461, 171)) <= 1000 ||
                 Player.Position.Distance(new Vector3(14340, 14391, 170)) <= 1000) &&
                Player.GetBuffCount("gangplankbilgewatertoken") >= 500 && Config.Item("drawings.warning").GetValue<bool>())
            {
                Drawing.DrawText(200,200,Color.Red,"Don't forget to buy Ultimate Upgrade with Silver Serpents");
            }
        }

        private static void KillstealDrawing()
        {
            if (Config.Item("killsteal.r").GetValue<bool>() && R.IsReady())
            {
                int minKillWave = 20;
                UltimateTarget = null;
                foreach (AIHeroClient enemy in HeroManager.Enemies)
                {
                    if (enemy.IsTargetable && !enemy.IsZombie && enemy.IsVisible && !enemy.IsDead)
                    {
                        int killWave = 1 + (int)((enemy.Health - (Player.HasBuff("GangplankRUpgrade2")?(R.Instance.Level + 20 + Player.TotalMagicalDamage*0.1)*3:0))/R.GetDamage(enemy));
                        if (killWave < minKillWave)
                        {
                            minKillWave = killWave;
                            UltimateTarget = enemy;
                        }
                    }
                }
                if (UltimateTarget != null && minKillWave <= (Player.HasBuff("GangplankRUpgrade1") ? 18 : 12) &&
                    minKillWave <= Config.Item("killsteal.minwave").GetValue<Slider>().Value)
                {
                    UltimateToBeUsed = true;
                    Drawing.DrawText(200, 260, Color.Tomato,
                        UltimateTarget.ChampionName + " is killable " +
                        (minKillWave < 1 ? "only with Death Daughter [R] Upgrade" : "with " + minKillWave + " R Waves"));
                }
                else
                {
                    UltimateToBeUsed = false;
                }
            }
        }


        private static void Combo(bool extended = false,AIHeroClient sender = null)
        {
            if (Config.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            //if (Config.Item("combo.r").GetValue<bool>())
            //{
            //    AIHeroClient RTarget = HeroManager.Enemies.FirstOrDefault(t => t.CountAlliesInRange(660) > 0);
            //    if (RTarget != null)
            //    {
            //        R.CastIfWillHit(RTarget, Config.Item("combo.rmin").GetValue<Slider>().Value);
            //    }
            //}

            if (Config.Menu.Item("combo.qe").GetValue<bool>()  && Q.IsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);
                if (target != null)
                {
                    EnemyPosition = target.Position;
                    Helper.GetPredPos(target);
                    if (extended && target != sender)
                    {
                        extended = false;
                    }

                    foreach (var b in AllBarrel)
                    {
                        if (b.CanQNow() && (b.GetBarrel().Position.CannotEscape(b.GetBarrel().Position, target, extended) || GetBarrelsInRange(b).Any(bb => bb.GetBarrel().Position.CannotEscape(b.GetBarrel().Position, target, extended, true))))
                        {
                            QDelay.Delay(b.GetBarrel());
                            break;
                        }
                    }
                    if (E.IsReady() && !QDelay.Active())
                    {
                        if (Config.Item("combo.doublee").GetValue<bool>())
                        {
                            foreach (var b in AllBarrel)
                            {
                                if (b.CanQNow() && b.GetBarrel().Distance(Player) > 615 &&
                                    b.GetBarrel().Distance(target) < 850)
                                {
                                    Q.Cast(b.GetBarrel());
                                    break;
                                }
                            }
                        }
                        if (Config.Item("combo.ex").GetValue<bool>())
                        {
                            foreach (var b in AllBarrel)
                            {
                                var castPos = b.GetBarrel().Position.ExtendToMaxRange(Helper.PredPos.To3D(),685);

                                if (b.CanQNow() && castPos.Distance(Player.Position) < 1000 &&
                                    castPos.CannotEscape(b.GetBarrel().Position, target, extended, true))
                                {
                                    E.Cast(castPos);
                                    QDelay.Delay(b.GetBarrel());
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //Triple - Logic
            if (Q.IsReady() && E.Instance.Ammo >= 2 && Config.Item("combo.triplee").GetValue<bool>())
            {
                List<Barrel> GetValidBarrels = AllBarrel.Where(b => b.CanQNow(400) && b.GetBarrel().Distance(Player) <= 625).ToList();
                AIHeroClient target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);
                if (target != null && GetValidBarrels.Any(b => b.GetBarrel().Distance(target) <= 1200))
                {
                    E.Cast(GetValidBarrels.First(b => b.GetBarrel().Distance(target) <= 1200).GetBarrel().Position.ExtendToMaxRange(Player.Position.ExtendToMaxRange(target.Position, 980), 685));
                    LeagueSharp.Common.Utility.DelayAction.Add(600, () => QDelay.Delay(GetValidBarrels.First().GetBarrel()));
                }
            }

            if (Config.Item("combo.q").GetValue<bool>() && Q.IsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (target != null && (Config.Item("key.q").GetValue<KeyBind>().Active || (!E.IsReady() && !AllBarrel.Any(b => b.GetBarrel().Position.Distance(target.Position) < 600))))
                {
                    Q.Cast(target);
                }
            }
            if (E.IsReady() && E.Instance.Ammo > 1 && Config.Item("combo.e").GetValue<bool>() && !AllBarrel.Any(b => b.GetBarrel().Position.Distance(Player.Position) <= 1200))
            {
                AIHeroClient target = TargetSelector.GetTarget(1000,TargetSelector.DamageType.Physical);
                if (target == null) return;
                Helper.GetPredPos(target);
                Vector2 castPos = target.Position.Extend(Helper.PredPos.To3D(), 200).To2D();
                if (Player.Distance(castPos) <= E.Range)
                {
                    E.Cast(castPos);
                }
                else
                {
                    E.Cast(Player.Position.Extend(castPos.To3D(), 1000));
                }
            }
        }

        private static IEnumerable<Barrel> GetBarrelsInRange (Barrel initalBarrel)
        {
            return AllBarrel.Where(b => b.GetBarrel().Position.Distance(initalBarrel.GetBarrel().Position) <= 685 && b != initalBarrel);
        }

        private static void Lasthit()
        {
            if (Config.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit || Player.ManaPercent <= Config.Item("lasthit.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (Q.IsReady())
            {
                foreach (Barrel barrel in AllBarrel)
                {
                    if (barrel.CanQNow() && MinionManager.GetMinions(barrel.GetBarrel().Position,650).Any(m => m.Health < Q.GetDamage(m) && m.Distance(barrel.GetBarrel()) <= 380))
                    {
                        Q.Cast(barrel.GetBarrel());
                    }
                }
                
                if (Config.Item("lasthit.q").GetValue<bool>() && (!AllBarrel.Any(b => b.GetBarrel().Position.Distance(Player.Position) < 1200) || Config.Item("key.q").GetValue<KeyBind>().Active))
                {
                    var lowHealthMinion = MinionManager.GetMinions(Player.Position, Q.Range).FirstOrDefault();
                    if (lowHealthMinion != null && lowHealthMinion.Health <= Q.GetDamage(lowHealthMinion))
                        Q.Cast(lowHealthMinion);
                }
            }
        }

        private static void Cleanse()
        {
            if (W.IsReady() && Config.Item("cleanse.w").GetValue<bool>())
            {
                if (Buffs.Any(entry => Config.Item("cleanse.bufftypes." + entry.Key).GetValue<bool>() && Player.HasBuffOfType(entry.Value)))
                {
                    W.Cast();
                }
            }
        }
    }
}
