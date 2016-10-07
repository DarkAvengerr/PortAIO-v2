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
    class Lissandra : Champion
    {
        public Lissandra()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Rod_of_Ages),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Liandrys_Torment),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Void_Staff),
                            new ConditionalItem(ItemId.Banshees_Veil,ItemId.Zhonyas_Hourglass,ItemCondition.ENEMY_AP),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Catalyst_of_Aeons
                        }
            };

            Game.OnUpdate += MonitorMissilePosition;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }
        
        private void MonitorMissilePosition(EventArgs args)
        {
            if (LissEMissile == null || player.IsDead)
            {
                return;
            }
            if (!LissEMissile.IsValid)
                LissEMissile = null;
            MissilePosition = LissEMissile.Position.To2D();

        }

        void OnCreate(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss != null && miss.IsValid)
            {
                if (miss.SpellCaster.IsMe && miss.SpellCaster.IsValid && miss.SData.Name == "LissandraEMissile")
                {
                    LissEMissile = miss;
                }
            }
        }

        void OnDelete(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss == null || !miss.IsValid) return;
            if (miss.SpellCaster is AIHeroClient && miss.SpellCaster.IsValid && miss.SpellCaster.IsMe && miss.SData.Name == "LissandraEMissile")
            {
                LissEMissile = null;
                MissilePosition = new Vector2(0, 0);
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

        public override void useSpells()
        {
            try
            {
                ComboHandler();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public override void farm()
        {
            if (player.ManaPercent < 55)
            {
                return;
            }
            var Minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        m =>
                            m.IsValidTarget() &&
                            (Vector3.Distance(m.ServerPosition, player.ServerPosition) <= Q.Range ||
                             Vector3.Distance(m.ServerPosition, player.ServerPosition) <= W.Range ||
                             Vector3.Distance(m.ServerPosition, player.ServerPosition) <= E.Range));

            if (Q.IsReady() )
            {
                var KillableMinionsQ = Minions.Where(m => m.Health < player.GetSpellDamage(m, SpellSlot.Q) && Vector3.Distance(m.ServerPosition, player.ServerPosition) > player.AttackRange);
                if (KillableMinionsQ.Any())
                {
                    Q.Cast(KillableMinionsQ.FirstOrDefault().ServerPosition);
                }
            }
            if (W.IsReady())
            {
                var KillableMinionsW = Minions.Where(m => m.Health < player.GetSpellDamage(m, SpellSlot.W) && Vector3.Distance(player.ServerPosition, m.ServerPosition) < W.Range);
                if (KillableMinionsW.Any())
                {
                    W.CastOnUnit(player);
                }
            }

            if (E.IsReady() && LissEMissile == null && !LissUtils.CanSecondE() && LissEMissile == null)
            {
                var KillableMinionsE = Minions.Where(m => m.Health < player.GetSpellDamage(m, SpellSlot.E) && Vector3.Distance(m.ServerPosition, player.ServerPosition) > player.AttackRange);
                if (KillableMinionsE.Any())
                {
                    E.Cast(KillableMinionsE.FirstOrDefault().ServerPosition);
                }
            }
        }

        private static Vector2 MissilePosition;
        private MissileClient LissEMissile;

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 715f);
            W = new Spell(SpellSlot.W, 450f);
            E = new Spell(SpellSlot.W, 1050f);
            R = new Spell(SpellSlot.R, 700f);
            Q.SetSkillshot(0.250f, 75f, 2200f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.250f, 125f, 850f, false, SkillshotType.SkillshotLine);
        }

        private void ComboHandler()
        {
            var Target = ARAMTargetSelector.getBestTarget(E.Range * 0.94f, true);
            
            if (Target != null && !Target.IsInvulnerable)
            {
                if (Q.IsReady())
                {
                    if (CastQ(Target))
                        return;
                }
                if (E.IsReady())
                {
                    if (CastE(Target))
                        return;
                }
                if (W.IsReady())
                {
                    if (CastW(Target))
                        return;
                }
                if (R.IsReady() && !Target.IsZombie)
                {
                    if (CastR(Target))
                        return;
                }
            }
        }

        private bool CastQ(AIHeroClient target)
        {
            Dictionary<SharpDX.Vector3, int> maxhit = (from hero in HeroManager.Enemies.Where(h => h.IsValidTarget() && 
                                                       !h.IsInvulnerable && Vector3.Distance(h.ServerPosition, player.ServerPosition) < Q.Range)
                                                       select Q.GetPrediction(hero) into prediction where prediction.CollisionObjects.Count > 0
                                                                                    && prediction.Hitchance >= HitChance.High let enemieshit = prediction.CollisionObjects.Where(x => x is AIHeroClient) select prediction).ToDictionary(prediction => prediction.CastPosition, prediction => prediction.CollisionObjects.Count);
            
            var bestpair = maxhit.MaxOrDefault(x => x.Value);
            if (bestpair.Value > 0)
            {
                Vector3 bestpos = bestpair.Key;
                Q.Cast(bestpos);
                return true;
            }


            var distbw = Vector3.Distance(player.ServerPosition, target.ServerPosition);

            if (distbw < Q.Range)
            {
                var prediction2 = Q.GetPrediction(target);
                if (prediction2.Hitchance >= HitChance.High)
                {
                    Q.Cast(prediction2.CastPosition);
                    return true;
                }
            }

            if (distbw > Q.Range && distbw < Q.Range)
            {
                var testQ = Q.GetPrediction(target);
                var collobjs = testQ.CollisionObjects;
                if ((testQ.Hitchance == HitChance.Collision || collobjs.Count > 0) && collobjs.All(x => x.IsTargetable))
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CastW(AIHeroClient target)
        {
            if (player.CountEnemiesInRange(W.Range)>0)
            {
                W.Cast();
                return true;
            }
            return false;
        }

        private bool CastE(AIHeroClient target)
        {
            if (LissEMissile == null && !LissUtils.CanSecondE())
            {
                var pred = E.GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    E.Cast(pred.CastPosition);
                    return true;
                }
            }
            return SecondEChecker(target);
        }

        //return asap to check the most amount of times 
        bool SecondEChecker(AIHeroClient target)
        {
            if (LissUtils.isHealthy() && LissEMissile != null && E.IsReady())
            {
                if (Vector2.Distance(MissilePosition, target.ServerPosition.To2D()) < Vector3.Distance(player.ServerPosition, target.ServerPosition)
                    && !LissUtils.PointUnderEnemyTurret(MissilePosition)
                    && Vector3.Distance(target.ServerPosition, LissEMissile.EndPosition) > Vector3.Distance(player.ServerPosition, target.ServerPosition))
                {
                    if (safeGap(LissEMissile.Position.To2D()))
                    {
                        E.Cast();
                        return true;
                    }
                }
                var Enemiesatpoint = LissEMissile.Position.GetEnemiesInRange(R.Range);
                var enemiesatpointR = Enemiesatpoint.Count;

                if ((enemiesatpointR >= 2 && SpellSlot.R.IsReady()) || (Enemiesatpoint.Any(e => IsKillableFromPoint(e,LissEMissile.Position) && Vector3.Distance(LissEMissile.Position, e.ServerPosition) < Vector3.Distance(player.ServerPosition, e.ServerPosition))))
                {
                    if (safeGap(LissEMissile.Position.To2D()))
                    {
                        E.Cast();
                        return true;
                    }
                }
                var enemiesatpointW = LissEMissile.Position.CountEnemiesInRange(W.Range);
                if (enemiesatpointW >= 2 && SpellSlot.W.IsReady())
                {
                    if (safeGap(LissEMissile.Position.To2D()))
                    {
                        E.Cast();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsKillableFromPoint(AIHeroClient target, Vector3 Point, bool ExcludeE = false)
        {
            double totaldmgavailable = 0;
            if (SpellSlot.Q.IsReady() && Vector3.Distance(Point, target.ServerPosition) < Q.Range + 35)
            {
                totaldmgavailable += player.GetSpellDamage(target, SpellSlot.Q);
            }
            if (SpellSlot.W.IsReady() && Vector3.Distance(Point, target.ServerPosition) < W.Range + 35)
            {
                totaldmgavailable += player.GetSpellDamage(target, SpellSlot.W);
            }
            if (SpellSlot.E.IsReady() && Vector3.Distance(Point, target.ServerPosition) < E.Range + 35 && !LissUtils.CanSecondE() && LissEMissile == null && !ExcludeE)
            {
                totaldmgavailable += player.GetSpellDamage(target, SpellSlot.E);
            }
            if (SpellSlot.R.IsReady() && Vector3.Distance(Point, target.ServerPosition) < Q.Range + 35)
            {
                totaldmgavailable += player.GetSpellDamage(target, SpellSlot.R);
            }
            
            return totaldmgavailable > target.Health;
        }


        float GetAvailableDamage(AIHeroClient target)
        {
            if (player.Distance(target) > 3000) // save some fps
            {
                return 0;
            }

            bool ExcludeE = false;
            double totaldmgavailable = 0;

            if (Q.IsReady())
            {
                totaldmgavailable += Q.GetDamage(target);
            }
            if (W.IsReady())
            {
                totaldmgavailable += W.GetDamage(target);
            }
            if (E.IsReady() && !LissUtils.CanSecondE() && LissEMissile == null)
            {
                totaldmgavailable += E.GetDamage(target);
            }
            if (R.IsReady() )
            {
                totaldmgavailable += R.GetDamage(target);
            }
            
            return (float)totaldmgavailable;

        }


        bool CastR(AIHeroClient currenttarget)
        {
            var Check =
                HeroManager.Enemies
                    .Where(
                        h => h.IsValidTarget(R.Range) && h.CountEnemiesInRange(R.Range) >=2 && h.HealthPercent >22).ToList();

            if (player.CountEnemiesInRange(R.Range) >= 2)
            {
                Check.Add(player);
            }

            if (Check.Any())
            {
                if (Check.Contains(player) && !LissUtils.isHealthy())
                {
                    R.CastOnUnit(player);
                    return true;
                }
                var target = Check.MaxOrDefault(TargetSelector.GetPriority);
                if (target != null)
                {
                    R.Cast(target);
                    return true;
                }
            }
            
            if (IsKillableFromPoint(currenttarget, player.ServerPosition) && player.Distance(currenttarget) < R.Range)
            {
                R.Cast(currenttarget);
                return true;
            }


            if (LissUtils.PointUnderAllyTurret(currenttarget.ServerPosition))
            {
                R.Cast(currenttarget);
                return true;
            }

            var dmgto = player.GetSpellDamage(currenttarget, SpellSlot.R);
            if (dmgto > currenttarget.Health && currenttarget.Health >= 0.40 * dmgto)
            {
                R.Cast(currenttarget);
                return true;
            }

            var enemycount = 2;
            if (!LissUtils.isHealthy() && player.CountEnemiesInRange(R.Range - 100) >= enemycount)
            {
                R.CastOnUnit(player);
                return true;
            }

            var possibilities = HeroManager.Enemies.Where(h => (h.IsValidTarget() && Vector3.Distance(h.ServerPosition, player.ServerPosition) <= R.Range || (IsKillableFromPoint(h, player.ServerPosition) && h.IsValidTarget() && !h.IsInvulnerable))).ToList();

            var arranged = possibilities.OrderByDescending(h => h.CountEnemiesInRange(R.Range));
            

            var UltTarget = arranged.FirstOrDefault();

            if (UltTarget != null)
            {
                if (!LissUtils.isHealthy() &&
                    player.CountEnemiesInRange(R.Range) >
                    UltTarget.CountEnemiesInRange(R.Range) + 1)
                {
                    R.CastOnUnit(player);
                    return true;
                }
                if (R.Cast(UltTarget) == Spell.CastStates.SuccessfullyCasted)
                    return true;

            }
            return false;
        }


    }
    class LissUtils
    {
        private static AIHeroClient player = ObjectManager.Player;

        public static bool isHealthy()
        {
            return player.HealthPercent > 25;
        }

        public static bool PointUnderEnemyTurret(Vector2 Point)
        {
            var EnemyTurrets =
                ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsEnemy && Vector2.Distance(t.Position.To2D(), Point) < 900f);
            return EnemyTurrets.Any();
        }

        public static bool PointUnderAllyTurret(Vector3 Point)
        {
            var AllyTurrets =
                ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsAlly && Vector3.Distance(t.Position, Point) < 900f);
            return AllyTurrets.Any();
        }

        public static bool CanSecondE()
        {
            return player.HasBuff("LissandraE");
        }

        public static bool AutoSecondE()
        {
            return true;
        }

    }

}
