using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace SephKhazix
{
    class Khazix : Helper
    {
        public static void Main()
        {
            Khazix K6 = new Khazix();
        }

        public Khazix()
        {
            OnLoad();
        }

        public void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Khazix")
            {
                return;
            }
            Chat.Print("<font color='#1d87f2'>SephKhazix Loaded </font>");
            Init();
            GenerateMenu();
            Game.OnUpdate += OnUpdate;
            Game.OnUpdate += DoubleJump;
            Drawing.OnDraw += OnDraw;
            Spellbook.OnCastSpell += SpellCast;
            Orbwalking.BeforeAttack += BeforeAttack;
        }

        void Init()
        {
            InitSkills();
            Khazix = ObjectManager.Player;

            foreach (var t in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsEnemy))
            {
                EnemyTurrets.Add(t);
            }

            var shop = ObjectManager.Get<Obj_Shop>().FirstOrDefault(o => o.IsAlly);
            if (shop != null)
            {
                NexusPosition = shop.Position;
            }

            HeroList = HeroManager.AllHeroes;
        }


        void OnUpdate(EventArgs args)
        {
            if (Khazix.IsDead || Khazix.LSIsRecalling())
            {
                return;
            }

            EvolutionCheck();

            if (Config.GetBool("Kson"))
            {
                KillSteal();
            }

            if (Config.GetKeyBind("Harass.Key"))
            {
                Harass();
            }

            switch (Config.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Waveclear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LH();
                    break;
            }
        }


        void Mixed()
        {
            if (Config.GetBool("Harass.InMixed"))
            {
                Harass();
            }
            LH();
        }

        void Harass()
        {
            if (Config.GetBool("UseQHarass") && Q.LSIsReady())
            {
                var enemy = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (enemy.IsValidEnemy())
                {
                    Q.Cast(enemy);
                }
            }
            if (Config.GetBool("UseWHarass") && W.LSIsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(950, TargetSelector.DamageType.Physical);
                var autoWI = Config.GetBool("Harass.AutoWI");
                var autoWD = Config.GetBool("Harass.AutoWD");
                var hitchance = HarassHitChance(Config);
                if (target != null && W.LSIsReady())
                {
                    if (!EvolvedW && Khazix.LSDistance(target) <= W.Range)
                    {
                        PredictionOutput predw = W.GetPrediction(target);
                        if (predw.Hitchance == hitchance)
                        {
                            W.Cast(predw.CastPosition);
                        }
                    }
                    else if (EvolvedW && target.LSIsValidTarget(W.Range))
                    {
                        PredictionOutput pred = WE.GetPrediction(target);
                        if ((pred.Hitchance == HitChance.Immobile && autoWI) || (pred.Hitchance == HitChance.Dashing && autoWD) || pred.Hitchance >= hitchance)
                        {
                            CastWE(target, pred.UnitPosition.LSTo2D(), 0, hitchance);
                        }
                    }
                }
            }
        }


        void LH()
        {
            List<Obj_AI_Base> allMinions = MinionManager.GetMinions(Khazix.ServerPosition, Q.Range);
            if (Config.GetBool("UseQFarm") && Q.LSIsReady())
            {
                foreach (Obj_AI_Base minion in
                    allMinions.Where(
                        minion =>
                            minion.LSIsValidTarget() &&
                            HealthPrediction.GetHealthPrediction(
                                minion, (int)(Khazix.LSDistance(minion) * 1000 / 1400)) <
                            0.75 * Khazix.LSGetSpellDamage(minion, SpellSlot.Q)))
                {
                    if (Vector3.Distance(minion.ServerPosition, Khazix.ServerPosition) >
                        Orbwalking.GetRealAutoAttackRange(Khazix) && Khazix.LSDistance(minion) <= Q.Range)
                    {
                        Q.CastOnUnit(minion);
                        return;
                    }
                }

            }
            if (Config.GetBool("UseWFarm") && W.LSIsReady())
            {
                MinionManager.FarmLocation farmLocation = MinionManager.GetBestCircularFarmLocation(
                  MinionManager.GetMinions(Khazix.ServerPosition, W.Range).Where(minion => HealthPrediction.GetHealthPrediction(
                                minion, (int)(Khazix.LSDistance(minion) * 1000 / 1400)) <
                            0.75 * Khazix.LSGetSpellDamage(minion, SpellSlot.W))
                      .Select(minion => minion.ServerPosition.LSTo2D())
                      .ToList(), W.Width, W.Range);
                if (farmLocation.MinionsHit >= 1)
                {
                    if (!EvolvedW)
                    {
                        if (Khazix.LSDistance(farmLocation.Position) <= W.Range)
                        {
                            W.Cast(farmLocation.Position);
                        }
                    }

                    if (EvolvedW)
                    {
                        if (Khazix.LSDistance(farmLocation.Position) <= W.Range)
                        {
                            W.Cast(farmLocation.Position);
                        }
                    }
                }
            }

            if (Config.GetBool("UseEFarm") && E.LSIsReady())
            {

                MinionManager.FarmLocation farmLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(Khazix.ServerPosition, E.Range).Where(minion => HealthPrediction.GetHealthPrediction(
                                minion, (int)(Khazix.LSDistance(minion) * 1000 / 1400)) <
                            0.75 * Khazix.LSGetSpellDamage(minion, SpellSlot.W))
                            .Select(minion => minion.ServerPosition.LSTo2D())
                            .ToList(), E.Width, E.Range);

                if (farmLocation.MinionsHit >= 1)
                {
                    if (Khazix.LSDistance(farmLocation.Position) <= E.Range)
                        E.Cast(farmLocation.Position);
                }
            }


            if (Config.GetBool("UseItemsFarm"))
            {
                MinionManager.FarmLocation farmLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(Khazix.ServerPosition, Hydra.Range)
                            .Select(minion => minion.ServerPosition.LSTo2D())
                            .ToList(), Hydra.Range, Hydra.Range);

                if (Hydra.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Hydra.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3074, Khazix);
                }
                if (Tiamat.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Tiamat.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3077, Khazix);
                }
            }
        }

        void Waveclear()
        {
            List<Obj_AI_Minion> allMinions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.LSIsValidTarget(W.Range) && !MinionManager.IsWard(x)).ToList();

            if (Config.GetBool("UseQFarm") && Q.LSIsReady())
            {
                var minion = Orbwalker.GetTarget() as Obj_AI_Minion;
                if (minion != null && HealthPrediction.GetHealthPrediction(
                                minion, (int)(Khazix.LSDistance(minion) * 1000 / 1400)) >
                            0.35f * Khazix.LSGetSpellDamage(minion, SpellSlot.Q) && Khazix.LSDistance(minion) <= Q.Range)
                {
                    Q.Cast(minion);
                }
                else if (minion == null || !minion.IsValid)
                {
                    foreach (var min in allMinions.Where(x => x.LSIsValidTarget(Q.Range)))
                    {
                        if (HealthPrediction.GetHealthPrediction(
                                min, (int)(Khazix.LSDistance(min) * 1000 / 1400)) >
                            3 * Khazix.LSGetSpellDamage(min, SpellSlot.Q) && Khazix.LSDistance(min) <= Q.Range)
                        {
                            Q.Cast(min);
                            break;
                        }
                    }
                }
            }

            if (Config.GetBool("UseWFarm") && W.LSIsReady() && Khazix.HealthPercent <= Config.GetSlider("Farm.WHealth"))
            {
                var wmins = EvolvedW ? allMinions.Where(x => x.LSIsValidTarget(WE.Range)) : allMinions.Where(x => x.LSIsValidTarget(W.Range));
                MinionManager.FarmLocation farmLocation = MinionManager.GetBestCircularFarmLocation(wmins
                      .Select(minion => minion.ServerPosition.LSTo2D())
                      .ToList(), EvolvedW ? WE.Width : W.Width, EvolvedW ? WE.Range : W.Range);
                var distcheck = EvolvedW ? Khazix.LSDistance(farmLocation.Position) <= WE.Range : Khazix.LSDistance(farmLocation.Position) <= W.Range;
                if (distcheck)
                {
                    W.Cast(farmLocation.Position);
                }
            }

            if (Config.GetBool("UseEFarm") && E.LSIsReady())
            {
                MinionManager.FarmLocation farmLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(Khazix.ServerPosition, E.Range)
                            .Select(minion => minion.ServerPosition.LSTo2D())
                            .ToList(), E.Width, E.Range);
                if (Khazix.LSDistance(farmLocation.Position) <= E.Range)
                {
                    E.Cast(farmLocation.Position);
                }
            }


            if (Config.GetBool("UseItemsFarm"))
            {
                MinionManager.FarmLocation farmLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(Khazix.ServerPosition, Hydra.Range)
                            .Select(minion => minion.ServerPosition.LSTo2D())
                            .ToList(), Hydra.Range, Hydra.Range);

                if (Hydra.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Hydra.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3074, Khazix);
                }
                if (Tiamat.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Tiamat.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3077, Khazix);
                }
                if (Titanic.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Titanic.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3748, Khazix);
                }
            }
        }


        void Combo()
        {
            AIHeroClient target = null;

            if (SpellSlot.E.LSIsReady() && SpellSlot.Q.LSIsReady())
            {
                target = TargetSelector.GetTarget((E.Range + Q.Range) * 0.95f, TargetSelector.DamageType.Physical);
            }

            if (target == null)
            {
                target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            }

            if ((target != null))
            {
                var dist = Khazix.LSDistance(target);

                // Normal abilities

                if (Q.LSIsReady() && !Jumping && Config.GetBool("UseQCombo"))
                {
                    if (dist <= Q.Range)
                    {
                        Q.Cast(target);
                    }
                }

                if (W.LSIsReady() && !EvolvedW && dist <= W.Range && Config.GetBool("UseWCombo"))
                {
                    var pred = W.GetPrediction(target);
                    if (pred.Hitchance >= Config.GetHitChance("WHitchance"))
                    {
                        W.Cast(pred.CastPosition);
                    }
                }

                if (E.LSIsReady() && !Jumping && dist <= E.Range && Config.GetBool("UseECombo") && dist > Q.Range + (0.7 * Khazix.MoveSpeed))
                {
                    PredictionOutput pred = E.GetPrediction(target);
                    if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }

                // Use EQ AND EW Synergy
                if ((dist <= E.Range + Q.Range + (0.7 * Khazix.MoveSpeed) && dist > Q.Range && E.LSIsReady() &&
                    Config.GetBool("UseEGapclose")) || (dist <= E.Range + W.Range && dist > Q.Range && E.LSIsReady() && W.LSIsReady() &&
                    Config.GetBool("UseEGapcloseW")))
                {
                    PredictionOutput pred = E.GetPrediction(target);
                    if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                    {
                        E.Cast(pred.CastPosition);
                    }
                    if (Config.GetBool("UseRGapcloseW") && R.LSIsReady())
                    {
                        R.CastOnUnit(Khazix);
                    }
                }


                // Ult Usage
                if (R.LSIsReady() && !Q.LSIsReady() && !W.LSIsReady() && !E.LSIsReady() &&
                    Config.GetBool("UseRCombo"))
                {
                    R.Cast();
                }
                // Evolved

                if (W.LSIsReady() && EvolvedW && dist <= WE.Range && Config.GetBool("UseWCombo"))
                {
                    PredictionOutput pred = WE.GetPrediction(target);
                    if (pred.Hitchance >= Config.GetHitChance("WHitchance"))
                    {
                        CastWE(target, pred.UnitPosition.LSTo2D(), 0, Config.GetHitChance("WHitchance"));
                    }
                    if (pred.Hitchance >= HitChance.Collision)
                    {
                        List<Obj_AI_Base> PCollision = pred.CollisionObjects;
                        var x = PCollision.Where(PredCollisionChar => PredCollisionChar.LSDistance(target) <= 30).FirstOrDefault();
                        if (x != null)
                        {
                            W.Cast(x.Position);
                        }
                    }
                }

                if (dist <= E.Range + (0.7 * Khazix.MoveSpeed) && dist > Q.Range &&
                    Config.GetBool("UseECombo") && E.LSIsReady())
                {
                    PredictionOutput pred = E.GetPrediction(target);
                    if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }

                if (Config.GetBool("UseItems"))
                {
                    UseItems(target);
                }
            }
        }


        void KillSteal()
        {
            AIHeroClient target = HeroList
                .Where(x => x.LSIsValidTarget() && x.LSDistance(Khazix.Position) < 1375f && !x.IsZombie)
                .MinOrDefault(x => x.Health);

            if (target != null && target.IsInRange(Ignite.Range))
            {
                if (Config.GetBool("UseIgnite") && IgniteSlot != SpellSlot.Unknown &&
                    Khazix.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                {
                    double igniteDmg = Khazix.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                    if (igniteDmg > target.Health)
                    {
                        Khazix.Spellbook.CastSpell(IgniteSlot, target);
                        return;
                    }
                }

                if (Config.GetBool("Safety.autoescape") && !IsHealthy)
                {
                    var ally =
                        HeroList.FirstOrDefault(h => h.HealthPercent > 40 && h.LSCountEnemiesInRange(400) == 0 && !h.ServerPosition.PointUnderEnemyTurret());
                    if (ally != null && ally.IsValid)
                    {
                        E.Cast(ally.ServerPosition);
                        return;
                    }
                    var underTurret = EnemyTurrets.Any(x => x.LSDistance(Khazix.ServerPosition) <= 900f && !x.IsDead && x.IsValid);
                    if (underTurret || Khazix.LSCountEnemiesInRange(500) >= 1)
                    {
                        var bestposition = Khazix.ServerPosition.LSExtend(NexusPosition, E.Range);
                        E.Cast(bestposition);
                        return;
                    }
                }

                if (Config.GetBool("UseQKs") && Q.LSIsReady() &&
                    Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= Q.Range)
                {
                    double QDmg = GetQDamage(target);
                    if (!Jumping && target.Health <= QDmg)
                    {
                        Q.Cast(target);
                        return;
                    }
                }

                if (Config.GetBool("UseEKs") && E.LSIsReady() && !Jumping &&
                    Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= E.Range && Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) > Q.Range)
                {
                    double EDmg = Khazix.LSGetSpellDamage(target, SpellSlot.E);
                    if (!Jumping && target.Health < EDmg)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            Game.Ping + Config.GetSlider("EDelay"), delegate
                            {
                                PredictionOutput pred = E.GetPrediction(target);
                                if (target.IsValid && !target.IsDead)
                                {
                                    if (Config.GetBool("Ksbypass") || ShouldJump(pred.CastPosition))
                                    {
                                        E.Cast(pred.CastPosition);
                                    }
                                }
                            });
                    }
                }

                if (W.LSIsReady() && !EvolvedW && Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= W.Range &&
                    Config.GetBool("UseWKs"))
                {
                    double WDmg = Khazix.LSGetSpellDamage(target, SpellSlot.W);
                    if (target.Health <= WDmg)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.Medium)
                        {
                            W.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }

                if (W.LSIsReady() && EvolvedW &&
                        Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= W.Range &&
                        Config.GetBool("UseWKs"))
                {
                    double WDmg = Khazix.LSGetSpellDamage(target, SpellSlot.W);
                    PredictionOutput pred = WE.GetPrediction(target);
                    if (target.Health <= WDmg && pred.Hitchance >= HitChance.Medium)
                    {
                        CastWE(target, pred.UnitPosition.LSTo2D(), 0, Config.GetHitChance("WHitchance"));
                        return;
                    }

                    if (pred.Hitchance >= HitChance.Collision)
                    {
                        List<Obj_AI_Base> PCollision = pred.CollisionObjects;
                        var x =
                            PCollision
                                .FirstOrDefault(PredCollisionChar => Vector3.Distance(PredCollisionChar.ServerPosition, target.ServerPosition) <= 30);
                        if (x != null)
                        {
                            W.Cast(x.Position);
                            return;
                        }
                    }
                }


                // Mixed's EQ KS
                if (Q.LSIsReady() && E.LSIsReady() &&
                    Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= E.Range + Q.Range
                    && Config.GetBool("UseEQKs"))
                {
                    double QDmg = GetQDamage(target);
                    double EDmg = Khazix.LSGetSpellDamage(target, SpellSlot.E);
                    if ((target.Health <= QDmg + EDmg))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Config.GetSlider("EDelay"), delegate
                        {
                            PredictionOutput pred = E.GetPrediction(target);
                            if (target.LSIsValidTarget() && !target.IsZombie && ShouldJump(pred.CastPosition))
                            {
                                if (Config.GetBool("Ksbypass") || ShouldJump(pred.CastPosition))
                                {
                                    E.Cast(pred.CastPosition);
                                }
                            }
                        });
                    }
                }

                // MIXED EW KS
                if (W.LSIsReady() && E.LSIsReady() && !EvolvedW &&
                    Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= W.Range + E.Range
                    && Config.GetBool("UseEWKs"))
                {
                    double WDmg = Khazix.LSGetSpellDamage(target, SpellSlot.W);
                    if (target.Health <= WDmg)
                    {

                        LeagueSharp.Common.Utility.DelayAction.Add(Config.GetSlider("EDelay"), delegate
                        {
                            PredictionOutput pred = E.GetPrediction(target);
                            if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                            {
                                if (Config.GetBool("Ksbypass") || ShouldJump(pred.CastPosition))
                                {
                                    E.Cast(pred.CastPosition);
                                }
                            }
                        });
                    }
                }

                if (Tiamat.IsReady() &&
                    Vector2.Distance(Khazix.ServerPosition.LSTo2D(), target.ServerPosition.LSTo2D()) <= Tiamat.Range &&
                    Config.GetBool("UseTiamatKs"))
                {
                    double Tiamatdmg = Khazix.GetItemDamage(target, Damage.DamageItems.Tiamat);
                    if (target.Health <= Tiamatdmg)
                    {
                        Tiamat.Cast();
                        return;
                    }
                }
                if (Hydra.IsReady() &&
                    Vector2.Distance(Khazix.ServerPosition.LSTo2D(), target.ServerPosition.LSTo2D()) <= Hydra.Range &&
                    Config.GetBool("UseTiamatKs"))
                {
                    double hydradmg = Khazix.GetItemDamage(target, Damage.DamageItems.Hydra);
                    if (target.Health <= hydradmg)
                    {
                        Hydra.Cast();
                    }
                }
            }
        }

        internal bool ShouldJump(Vector3 position)
        {
            if (!Config.GetBool("Safety.Enabled") || Override)
            {
                return true;
            }
            if (Config.GetBool("Safety.TowerJump") && position.PointUnderEnemyTurret())
            {
                return false;
            }
            else if (Config.GetBool("Safety.Enabled"))
            {
                if (Khazix.HealthPercent < Config.GetSlider("Safety.MinHealth"))
                {
                    return false;
                }

                if (Config.GetBool("Safety.CountCheck"))
                {
                    var enemies = position.LSGetEnemiesInRange(400);
                    var allies = position.LSGetAlliesInRange(400);

                    var ec = enemies.Count;
                    var ac = allies.Count;
                    float setratio = Config.GetSlider("Safety.Ratio") / 5;


                    if (ec != 0 && !(ac / ec >= setratio))
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }



        internal void CastWE(Obj_AI_Base unit, Vector2 unitPosition, int minTargets = 0, HitChance hc = HitChance.Medium)
        {
            var points = new List<Vector2>();
            var hitBoxes = new List<int>();

            Vector2 startPoint = Khazix.ServerPosition.LSTo2D();
            Vector2 originalDirection = W.Range * (unitPosition - startPoint).LSNormalized();

            foreach (AIHeroClient enemy in HeroManager.Enemies)
            {
                if (enemy.LSIsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    PredictionOutput pos = WE.GetPrediction(enemy);
                    if (pos.Hitchance >= hc)
                    {
                        points.Add(pos.UnitPosition.LSTo2D());
                        hitBoxes.Add((int)enemy.BoundingRadius + 275);
                    }
                }
            }

            var posiblePositions = new List<Vector2>();

            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                    posiblePositions.Add(unitPosition + originalDirection.LSRotated(0));
                if (i == 1)
                    posiblePositions.Add(startPoint + originalDirection.LSRotated(Wangle));
                if (i == 2)
                    posiblePositions.Add(startPoint + originalDirection.LSRotated(-Wangle));
            }


            if (startPoint.LSDistance(unitPosition) < 900)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 pos = posiblePositions[i];
                    Vector2 direction = (pos - startPoint).LSNormalized().LSPerpendicular();
                    float k = (2 / 3 * (unit.BoundingRadius + W.Width));
                    posiblePositions.Add(startPoint - k * direction);
                    posiblePositions.Add(startPoint + k * direction);
                }
            }

            var bestPosition = new Vector2();
            int bestHit = -1;

            foreach (Vector2 position in posiblePositions)
            {
                int hits = CountHits(position, points, hitBoxes);
                if (hits > bestHit)
                {
                    bestPosition = position;
                    bestHit = hits;
                }
            }

            if (bestHit + 1 <= minTargets)
                return;

            W.Cast(bestPosition.To3D(), false);
        }

        int CountHits(Vector2 position, List<Vector2> points, List<int> hitBoxes)
        {
            int result = 0;

            Vector2 startPoint = Khazix.ServerPosition.LSTo2D();
            Vector2 originalDirection = W.Range * (position - startPoint).LSNormalized();
            Vector2 originalEndPoint = startPoint + originalDirection;

            for (int i = 0; i < points.Count; i++)
            {
                Vector2 point = points[i];

                for (int k = 0; k < 3; k++)
                {
                    var endPoint = new Vector2();
                    if (k == 0)
                        endPoint = originalEndPoint;
                    if (k == 1)
                        endPoint = startPoint + originalDirection.LSRotated(Wangle);
                    if (k == 2)
                        endPoint = startPoint + originalDirection.LSRotated(-Wangle);

                    if (point.LSDistance(startPoint, endPoint, true, true) <
                        (W.Width + hitBoxes[i]) * (W.Width + hitBoxes[i]))
                    {
                        result++;
                        break;
                    }
                }
            }
            return result;
        }


        void DoubleJump(EventArgs args)
        {
            if (!E.LSIsReady() || !EvolvedE || !Config.GetBool("djumpenabled") || Khazix.IsDead || Khazix.LSIsRecalling())
            {
                return;
            }

            var Targets = HeroList.Where(x => x.LSIsValidTarget() && !x.IsInvulnerable && !x.IsZombie);

            if (Q.LSIsReady() && E.LSIsReady())
            {
                var CheckQKillable = Targets.FirstOrDefault(x => Vector3.Distance(Khazix.ServerPosition, x.ServerPosition) < Q.Range - 25 && GetQDamage(x) > x.Health);

                if (CheckQKillable != null)
                {
                    Jumping = true;
                    Jumppoint1 = GetJumpPoint(CheckQKillable);
                    E.Cast(Jumppoint1);
                    Q.Cast(CheckQKillable);
                    var oldpos = Khazix.ServerPosition;
                    LeagueSharp.Common.Utility.DelayAction.Add(Config.GetSlider("JEDelay") + Game.Ping, () =>
                    {
                        if (E.LSIsReady())
                        {
                            Jumppoint2 = GetJumpPoint(CheckQKillable, false);
                            E.Cast(Jumppoint2);
                        }
                        Jumping = false;
                    });
                }
            }
        }


        Vector3 GetJumpPoint(AIHeroClient Qtarget, bool firstjump = true)
        {
            if (Khazix.ServerPosition.PointUnderEnemyTurret())
            {
                return Khazix.ServerPosition.LSExtend(NexusPosition, E.Range);
            }

            if (Config.GetSL("jumpmode") == 0)
            {
                return Khazix.ServerPosition.LSExtend(NexusPosition, E.Range);
            }

            if (firstjump && Config.GetBool("jcursor"))
            {
                return Game.CursorPos;
            }

            if (!firstjump && Config.GetBool("jcursor2"))
            {
                return Game.CursorPos;
            }

            Vector3 Position = new Vector3();
            var jumptarget = IsHealthy
                  ? HeroList
                      .FirstOrDefault(x => x.LSIsValidTarget() && !x.IsZombie && x != Qtarget &&
                              Vector3.Distance(Khazix.ServerPosition, x.ServerPosition) < E.Range)
                  :
              HeroList
                  .FirstOrDefault(x => x.IsAlly && !x.IsZombie && !x.IsDead && !x.IsMe &&
                          Vector3.Distance(Khazix.ServerPosition, x.ServerPosition) < E.Range);

            if (jumptarget != null)
            {
                Position = jumptarget.ServerPosition;
            }
            if (jumptarget == null)
            {
                return Khazix.ServerPosition.LSExtend(NexusPosition, E.Range);
            }
            return Position;
        }

        void SpellCast(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!EvolvedE || !Config.GetBool("save"))
            {
                return;
            }

            if (args.Slot.Equals(SpellSlot.Q) && args.Target is AIHeroClient && Config.GetBool("djumpenabled"))
            {
                var target = args.Target as AIHeroClient;
                var qdmg = GetQDamage(target);
                var dmg = (Khazix.LSGetAutoAttackDamage(target) * 2) + qdmg;
                if (target.Health < dmg && target.Health > qdmg)
                { //save some unnecessary q's if target is killable with 2 autos + Q instead of Q as Q is important for double jumping
                    args.Process = false;
                }
            }
        }

        void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target.Type == GameObjectType.AIHeroClient)
            {
                if (Config.GetBool("Safety.noaainult") && IsInvisible)
                {
                    args.Process = false;
                    return;
                }
                if (Config.GetBool("djumpenabled") && Config.GetBool("noauto"))
                {
                    if (args.Target.Health < GetQDamage((AIHeroClient)args.Target) &&
                        Khazix.ManaPercent > 15)
                    {
                        args.Process = false;
                    }
                }
            }
        }

        void OnDraw(EventArgs args)
        {
            if (Config.GetBool("Drawings.Disable") || Khazix.IsDead || Khazix.LSIsRecalling())
            {
                return;
            }
            if (Config.GetBool("Debugon"))
            {
                var isolatedtargs = GetIsolatedTargets();
                foreach (var x in isolatedtargs)
                {
                    var heroposwts = Drawing.WorldToScreen(x.Position);
                    Drawing.DrawText(heroposwts.X, heroposwts.Y, System.Drawing.Color.White, "Isolated");
                }
            }

            if (Config.GetBool("jumpdrawings") && Jumping)
            {
                var PlayerPosition = Drawing.WorldToScreen(Khazix.Position);
                var Jump1 = Drawing.WorldToScreen(Jumppoint1).To3D();
                var Jump2 = Drawing.WorldToScreen(Jumppoint2).To3D();
                Render.Circle.DrawCircle(Jump1, 250, System.Drawing.Color.White);
                Render.Circle.DrawCircle(Jump2, 250, System.Drawing.Color.White);
                Drawing.DrawLine(PlayerPosition.X, PlayerPosition.Y, Jump1.X, Jump1.Y, 10, System.Drawing.Color.DarkCyan);
                Drawing.DrawLine(Jump1.X, Jump1.Y, Jump2.X, Jump2.Y, 10, System.Drawing.Color.DarkCyan);
            }

            var drawq = Config.GetCircle("DrawQ");
            var draww = Config.GetCircle("DrawW");
            var drawe = Config.GetCircle("DrawE");

            if (drawq.Active)
            {
                Render.Circle.DrawCircle(Khazix.Position, Q.Range, drawq.Color);
            }
            if (draww.Active)
            {
                Render.Circle.DrawCircle(Khazix.Position, W.Range, drawq.Color);
            }

            if (drawe.Active)
            {
                Render.Circle.DrawCircle(Khazix.Position, E.Range, drawq.Color);
            }

        }
    }
}

