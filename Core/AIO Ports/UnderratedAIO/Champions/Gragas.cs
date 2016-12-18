using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;

using Prediction = LeagueSharp.Common.Prediction;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Gragas
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, E2, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justQ, useIgnite, justE, canUlt, justR;
        public Vector3 qPos, rPos;
        public const int QExplosionRange = 300;
        public static GragasQ savedQ = null;
        public double[] Rwave = new double[] { 50, 70, 90 };

        public Gragas()
        {
            InitGragas();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Gragas</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (config.Item("usewgc", true).GetValue<bool>() && gapcloser.End.Distance(player.Position) < 200)
            {
                W.Cast();
            }
            if (config.Item("useegc", true).GetValue<bool>() && gapcloser.End.Distance(player.Position) < E.Range &&
                E.CanCast(gapcloser.Sender))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.IsEnemy && config.Item("useegc", true).GetValue<bool>() && sender is AIHeroClient &&
                args.EndPos.Distance(player.Position) < E.Range && E.CanCast(sender))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(args.Duration, () => { E.Cast(args.EndPos); });
            }
        }

        private void OnInterruptableTarget(AIHeroClient target, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.CanCast(target) && config.Item("useEint", true).GetValue<bool>())
            {
                if (Program.IsSPrediction)
                {
                    if (E.SPredictionCast(target, HitChance.High))
                    {
                        return;
                    }
                }
                else
                {
                    if (E.CastIfHitchanceEquals(target, HitChance.High))
                    {
                        return;
                    }
                }
            }
            if (R.CanCast(target) && config.Item("useRint", true).GetValue<bool>())
            {
                if (savedQ != null && !SimpleQ && !target.IsMoving && target.Distance(qPos) > QExplosionRange &&
                    target.Distance(player) < R.Range - 100 &&
                    target.Position.Distance(savedQ.position) < 550 + QExplosionRange / 2 &&
                    !target.HasBuffOfType(BuffType.Knockback))
                {
                    var cast = Prediction.GetPrediction(target, 1000f).UnitPosition.Extend(savedQ.position, -100);
                    R.Cast(cast);
                }
                else if (target.Distance(player) < R.Range - 100)
                {
                    if (player.CountEnemiesInRange(2000) <= player.CountAlliesInRange(2000))
                    {
                        var cast = target.Position.Extend(player.Position, -100);
                        R.Cast(cast);
                    }
                    else
                    {
                        var cast = target.Position.Extend(player.Position, 100);
                        R.Cast(cast);
                    }
                }
            }
        }

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                savedQ = null;
                qPos = Vector3.Zero;
            }
        }

        private void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                savedQ = new GragasQ(sender.Position, System.Environment.TickCount);
            }
        }

        private void InitGragas()
        {
            Q = new Spell(SpellSlot.Q, 800);
            Q.SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 600);
            E.SetSkillshot(0.25f, 200, 1200, true, SkillshotType.SkillshotLine);
            E2 = new Spell(SpellSlot.E, 600);
            E2.SetSkillshot(0f, 200, 2000, true, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 1050);
            R.SetSkillshot(0.25f, 150, 1750, false, SkillshotType.SkillshotCircle);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            orbwalker.SetAttack(true);
            AIHeroClient target = DrawHelper.GetBetterTarget(1300, TargetSelector.DamageType.Magical, true);
            var combodmg = 0f;
            if (target != null)
            {
                combodmg = ComboDamage(target);
            }
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(combodmg);
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                default:
                    break;
            }
            if (config.Item("autoQ", true).GetValue<bool>())
            {
                if (Q.IsReady() && savedQ != null)
                {
                    DetonateQ();
                }
            }
            if (savedQ != null && !SimpleQ)
            {
                var mob = Jungle.GetNearest(player.Position);
                if (mob != null && getQdamage(mob) > mob.Health)
                {
                    Q.Cast();
                }
            }
            if (config.Item("insec", true).GetValue<KeyBind>().Active)
            {
                if (target == null)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    return;
                }
                CastE(target);
                if (savedQ != null)
                {
                    if (savedQ != null && !SimpleQ /*&& target.Distance(qPos) > QExplosionRange*/&&
                        target.Distance(player) < R.Range - 100 &&
                        target.Position.Distance(savedQ.position) < 550 + QExplosionRange / 2)
                    {
                        HandeR(target, savedQ.position, true);
                    }
                }
                else
                {
                    castInsec(target);
                }

                Orbwalking.MoveTo(Game.CursorPos);
                DetonateQ();
            }
        }

        private void castInsec(AIHeroClient target)
        {
            if (Q.IsReady() && SimpleQ)
            {
                var pred = R.GetPrediction(target);
                if (R.IsReady() &&
                    target.Buffs.Any(
                        buff =>
                            buff.Type == BuffType.Snare || buff.Type == BuffType.Stun ||
                            buff.Type == BuffType.Suppression || buff.Type == BuffType.Knockup))
                {
                    if (pred.Hitchance >= HitChance.Medium &&
                        pred.CastPosition.Distance(player.Position) < R.Range - 150)
                    {
                        R.Cast(pred.CastPosition.Extend(player.Position, -150));
                    }
                }
                if (justR && rPos.IsValid())
                {
                    Q.Cast(rPos.Extend(pred.UnitPosition, 550 + QExplosionRange / 2f));
                }
            }
        }


        private void Harass()
        {
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            AIHeroClient target = DrawHelper.GetBetterTarget(1300, TargetSelector.DamageType.Magical, true);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }
            if (Q.CanCast(target) && config.Item("useqH", true).GetValue<bool>() && savedQ == null && SimpleQ)
            {
                if (Program.IsSPrediction)
                {
                    Q.SPredictionCast(target, HitChance.High);
                }
                else
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }
            }
            if (Q.IsReady() && config.Item("useqH", true).GetValue<bool>() && savedQ != null &&
                target.Distance(savedQ.position) < QExplosionRange)
            {
                DetonateQ();
            }
            if (E.CanCast(target) && config.Item("useeH", true).GetValue<bool>())
            {
                CastE(target);
            }
        }

        private void DetonateQ()
        {
            if (SimpleQ)
            {
                return;
            }
            var targethero =
                HeroManager.Enemies.Where(e => e.Distance(savedQ.position) < QExplosionRange && e.IsValidTarget())
                    .OrderByDescending(e => e.Distance(savedQ.position))
                    .FirstOrDefault();

            if (targethero == null)
            {
                return;
            }

            if (savedQ.deltaT() < 2000 &&
                Prediction.GetPrediction(targethero, 0.1f).UnitPosition.Distance(savedQ.position) < QExplosionRange &&
                HeroManager.Enemies.Count(
                    h => h.Distance(savedQ.position) < QExplosionRange && h.IsValidTarget() && h.Health < getQdamage(h)) ==
                0)
            {
                //waiting
            }
            else
            {
                Q.Cast();
            }
        }

        private static bool SimpleQ
        {
            get { return player.Spellbook.GetSpell(SpellSlot.Q).Name == "GragasQ"; }
        }

        private void Clear()
        {
            if (Q.IsReady() && savedQ != null &&
                ((Environment.Minion.countMinionsInrange(savedQ.position, QExplosionRange) >
                  config.Item("qMinHit", true).GetValue<Slider>().Value && savedQ.deltaT() > 2000) ||
                 MinionManager.GetMinions(
                     ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                     .Count(m => HealthPrediction.GetHealthPrediction(m, 600) < 0 || m.Health < 35) > 0))
            {
                Q.Cast();
            }

            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (Q.IsReady() && savedQ == null && SimpleQ && config.Item("useqLC", true).GetValue<bool>())
            {
                MinionManager.FarmLocation bestPositionQ =
                    Q.GetCircularFarmLocation(
                        MinionManager.GetMinions(
                            ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly),
                        QExplosionRange);
                if (bestPositionQ.MinionsHit > config.Item("qMinHit", true).GetValue<Slider>().Value)
                {
                    Q.Cast(bestPositionQ.Position);
                    return;
                }
            }

            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady())
            {
                MinionManager.FarmLocation bestPositionE =
                    E.GetLineFarmLocation(
                        MinionManager.GetMinions(
                            ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly));

                if (bestPositionE.MinionsHit >= config.Item("eMinHit", true).GetValue<Slider>().Value)
                {
                    E.Cast(bestPositionE.Position);
                }
            }
            if (W.IsReady() && config.Item("usewLC", true).GetValue<bool>() &&
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Count(m => m.Health > 600) > 0)
            {
                W.Cast();
            }
        }

        private void Combo(float combodmg)
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(1100, TargetSelector.DamageType.Magical, true);
            if (target == null || target.IsInvulnerable || target.MagicImmune)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 700) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) &&
                ((savedQ == null ||
                  (savedQ != null && target.Distance(savedQ.position) < QExplosionRange &&
                   getQdamage(target) > target.Health)) || useIgnite))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            var rqCombo = R.GetDamage(target) + getQdamage(target) + (hasIgnite ? ignitedmg : 0);
            if (Q.CanCast(target) && config.Item("useq", true).GetValue<bool>() && savedQ == null && SimpleQ)
            {
                if (Program.IsSPrediction)
                {
                    if (Q.SPredictionCast(target, HitChance.High))
                    {
                        return;
                    }
                }
                else
                {
                    if (Q.CastIfHitchanceEquals(target, HitChance.High))
                    {
                        return;
                    }
                }
            }
            if (Q.IsReady() && config.Item("useq", true).GetValue<bool>() && savedQ != null &&
                target.Distance(savedQ.position) < QExplosionRange)
            {
                DetonateQ();
            }
            if (config.Item("usee", true).GetValue<bool>())
            {
                CastE(target);
            }
            if (W.IsReady() && (!SimpleQ || !Q.IsReady()) && config.Item("usew", true).GetValue<bool>() &&
                player.Distance(target) < 300 && Orbwalking.CanMove(100) &&
                target.Health > combodmg - getWdamage(target))
            {
                W.Cast();
            }
            if (R.IsReady())
            {
                if (R.CastIfWillHit(target, config.Item("Rmin", true).GetValue<Slider>().Value))
                {
                    return;
                }
                var logic = config.Item("user", true).GetValue<bool>();
                if (config.Item("rtoq", true).GetValue<bool>() && savedQ != null && !SimpleQ &&
                    (target.Distance(qPos) > QExplosionRange ||
                     (target.Health < rqCombo && target.Health > getQdamage(target))) &&
                    target.Distance(player) < R.Range - 100 &&
                    (target.Health < rqCombo || CheckRPushForAlly(target, rqCombo)) &&
                    target.Position.Distance(savedQ.position) < 550 + QExplosionRange / 2)
                {
                    var cast = Prediction.GetPrediction(target, 1000f).UnitPosition.Extend(savedQ.position, -200);
                    if (cast.Distance(player.Position) < R.Range)
                    {
                        //Console.WriteLine("R to Q");
                        if (target.Health < rqCombo && target.Health > rqCombo - ignitedmg &&
                            player.Distance(target) < 580)
                        {
                            useIgnite = true;
                        }

                        LeagueSharp.Common.Utility.DelayAction.Add(400, () => useIgnite = false);
                        HandeR(target, savedQ.position, true);
                        return;
                    }
                }

                if (config.Item("rtoq", true).GetValue<bool>() && logic &&
                    (target.Health < rqCombo && target.Health > getQdamage(target)))
                {
                    castInsec(target);
                }
                if (config.Item("rtoally", true).GetValue<bool>() && logic &&
                    target.Health - rqCombo < target.MaxHealth * 0.5f)
                {
                    var allies =
                        HeroManager.Allies.Where(
                            a =>
                                !a.IsDead && !a.IsMe && a.HealthPercent > 40 && a.Distance(target) < 700 &&
                                a.Distance(target) > 300).OrderByDescending(a => TargetSelector.GetPriority(a));
                    if (allies.Any())
                    {
                        foreach (var ally in allies)
                        {
                            var cast =
                                Prediction.GetPrediction(target, 1000f)
                                    .UnitPosition.Extend(Prediction.GetPrediction(ally, 400f).UnitPosition, -200);
                            if (cast.CountEnemiesInRange(1000) <= cast.CountAlliesInRange(1000) &&
                                cast.Distance(player.Position) < R.Range &&
                                cast.Extend(target.Position, 500).Distance(ally.Position) <
                                target.Distance(ally.Position))
                            {
                                //Console.WriteLine("R to Ally: " + ally.Name);
                                HandeR(target, Prediction.GetPrediction(ally, 400f).UnitPosition, false);
                                return;
                            }
                        }
                    }
                    var turret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .OrderBy(t => t.Distance(target))
                            .FirstOrDefault(t => t.Distance(target) < 2000 && t.IsAlly && !t.IsDead);

                    if (config.Item("rtoturret", true).GetValue<bool>() && turret != null)
                    {
                        var pos = target.Position.Extend(turret.Position, -200);
                        if (target.Distance(turret) > pos.Extend(target.Position, 500).Distance(turret.Position))
                        {
                            //nothing
                        }
                        else if ((pos.CountEnemiesInRange(1000) < pos.CountAlliesInRange(1000) &&
                                  target.Health - rqCombo < target.MaxHealth * 0.4f) ||
                                 (ObjectManager.Get<Obj_AI_Turret>()
                                     .Count(t => t.Distance(pos) < 950 && t.IsAlly && t.IsValid && !t.IsDead) > 0 &&
                                  target.Health - combodmg < target.MaxHealth * 0.5f))
                        {
                            //Console.WriteLine("R to Turret");
                            HandeR(target, turret.Position, false);
                            return;
                        }
                    }
                }
                if (config.Item("rtokill", true).GetValue<bool>() && config.Item("user", true).GetValue<bool>() &&
                    R.GetDamage(target) > target.Health && !justE && !justQ &&
                    (savedQ == null ||
                     (savedQ != null && !qPos.IsValid() && target.Distance(savedQ.position) > QExplosionRange)) &&
                    (target.CountAlliesInRange(700) <= 1 || player.HealthPercent < 35))
                {
                    //Console.WriteLine("R to Kill");
                    var pred = R.GetPrediction(target, true);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        R.Cast(pred.CastPosition);
                    }
                    //R.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    return;
                }
            }
        }

        private bool checkMana()
        {
            var manareq = 0f;
            if (Q.IsReady())
            {
                manareq += Q.ManaCost;
            }
            if (W.IsReady())
            {
                manareq += W.ManaCost;
            }
            if (E.IsReady())
            {
                manareq += E.ManaCost;
            }
            if (R.IsReady())
            {
                manareq += R.ManaCost;
            }
            return player.Mana > manareq;
        }

        private void HandeR(Obj_AI_Base target, Vector3 toVector3, bool toBarrel)
        {
            if (target == null || !toVector3.IsValid())
            {
                return;
            }
            var pred = Prediction.GetPrediction(target, target.Distance(player.ServerPosition) / R.Speed);
            if (pred.Hitchance >= HitChance.VeryHigh && !justE && !target.IsDashing())
            {
                var cast = pred.UnitPosition.Extend(toVector3, -100);
                if (player.Distance(cast) < R.Range && checkBuffs(target, player.Distance(cast)) &&
                    pred.UnitPosition.Distance(target.Position) < 15 &&
                    ((!CombatHelper.CheckWalls(target.Position, toVector3)) ||
                     (toBarrel && savedQ.position.Distance(target.Position) < QExplosionRange)))
                {
                    if (toBarrel &&
                        4000 - savedQ.deltaT() > (player.Distance(cast) + cast.Distance(savedQ.position)) / R.Speed)
                    {
                        R.Cast(cast);
                        return;
                    }
                    else if (!toBarrel)
                    {
                        R.Cast(cast);
                    }
                }
            }
            /*
            if (!config.Item("insecOnlyStun", true).GetValue<bool>())
            {
                var cast = R.GetPrediction(target, true, 90);
                if (cast.Hitchance >= HitChance.VeryHigh)
                {
                    R.Cast(cast.CastPosition.Extend(savedQ.position, -100));
                }
            }*/
        }

        private bool checkBuffs(Obj_AI_Base hero, float distance)
        {
            var stun =
                hero.Buffs.Where(
                    buff =>
                        buff.Type == BuffType.Snare || buff.Type == BuffType.Stun || buff.Type == BuffType.Suppression ||
                        buff.Type == BuffType.Knockup)
                    .OrderByDescending(buff => CombatHelper.GetBuffTime(buff))
                    .FirstOrDefault();
            if (stun != null)
            {
                if (CombatHelper.GetBuffTime(stun) > distance / R.Speed)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckRPushForAlly(AIHeroClient target, float combodmg)
        {
            var pos = target.Position.Extend(savedQ.position, 550);
            var turret =
                ObjectManager.Get<Obj_AI_Turret>()
                    .OrderBy(t => t.Distance(target))
                    .FirstOrDefault(t => t.Distance(target) < 2000 && t.IsEnemy && t.IsValidTarget());
            if (turret != null && target.Distance(turret) > pos.Extend(target.Position, 500).Distance(turret.Position))
            {
                return false;
            }
            if ((pos.CountEnemiesInRange(1000) < pos.CountAlliesInRange(1000) &&
                 target.Health - combodmg < target.MaxHealth * 0.4f) ||
                (ObjectManager.Get<Obj_AI_Turret>()
                    .Count(t => t.Distance(pos) < 950 && t.IsAlly && t.IsValid && !t.IsDead) > 0 &&
                 target.Health - combodmg < target.MaxHealth * 0.5f))
            {
                return true;
            }
            return false;
        }

        private void CastE(AIHeroClient target)
        {
            if (player.Distance(target) < 200)
            {
                E.Cast(target.Position);
                return;
            }
            if (E.CanCast(target))
            {
                if (Program.IsSPrediction)
                {
                    E.SPredictionCast(target, HitChance.High);
                }
                else
                {
                    var pred = E.GetPrediction(target);
                    var pred2 = E2.GetPrediction(target);
                    if (pred2.Hitchance > HitChance.Low)
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }
        }


        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            damage += getQdamage(hero);
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if (W.IsReady() || player.HasBuff("gragaswattackbuff"))
            {
                damage += getWdamage(hero);
            }
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R);
            }
            //damage += ItemHandler.GetItemsDamage(target);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private static double getWdamage(AIHeroClient target)
        {
            var dmg = new double[] { 20, 50, 80, 110, 140 }[W.Level - 1] + 8f / 100f * target.MaxHealth +
                      0.3f * player.FlatMagicDamageMod;
            return Damage.CalcDamage(player, target, Damage.DamageType.Magical, dmg);
        }

        public static float getQdamage(Obj_AI_Base target)
        {
            var damage = 0d;
            if (Q.IsReady())
            {
                if (savedQ == null)
                {
                    damage += Damage.GetSpellDamage(player, target, SpellSlot.Q);
                }
                else
                {
                    if (savedQ.deltaT() > 2000)
                    {
                        damage += Damage.GetSpellDamage(player, target, SpellSlot.Q) * 1.5f;
                    }
                    else
                    {
                        damage += Damage.GetSpellDamage(player, target, SpellSlot.Q);
                    }
                }
            }
            if (target.Name.Contains("SRU_Dragon"))
            {
                var dsBuff = player.GetBuff("s5test_dragonslayerbuff");
                if (dsBuff != null)
                {
                    damage = damage * (1f - 0.07f * dsBuff.Count);
                }
            }
            if (target.Name.Contains("SRU_Baron"))
            {
                var bBuff = player.GetBuff("barontarget");
                if (bBuff != null)
                {
                    damage = damage * 0.5f;
                }
            }
            return (float) damage;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "GragasQ")
                {
                    if (!justQ)
                    {
                        justQ = true;
                        qPos = args.End;
                        savedQ = new GragasQ(args.End, System.Environment.TickCount - 500);
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => justQ = false);
                    }
                }
                if (args.SData.Name == "GragasE")
                {
                    if (!justE)
                    {
                        justE = true;
                        var dist = player.Distance(args.End);
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            (int) Math.Min(((dist > E.Range ? E.Range : dist) / E.Speed * 1000f), 250),
                            () => justE = false);
                    }
                }
                if (args.Slot == SpellSlot.R)
                {
                    if (!justR)
                    {
                        justR = true;
                        rPos = args.End;
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            300, () =>
                            {
                                justR = false;
                                rPos = Vector3.Zero;
                            });
                    }
                }
            }
        }

        private void InitMenu()
        {
            config = new Menu("Gragas ", "Gragas", true);
            // Target Selector
            Menu menuTS = new Menu("Selector", "tselect");
            TargetSelector.AddToMenu(menuTS);
            config.AddSubMenu(menuTS);
            // Orbwalker
            Menu menuOrb = new Menu("Orbwalker", "orbwalker");
            orbwalker = new Orbwalking.Orbwalker(menuOrb);
            config.AddSubMenu(menuOrb);
            // Draw settings
            Menu menuD = new Menu("Drawings ", "dsettings");
            menuD.AddItem(new MenuItem("drawqq", "Draw Q range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings 
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R 1v1", true)).SetValue(true);
            menuC.AddItem(new MenuItem("rtoally", "   To ally", true)).SetValue(false);
            menuC.AddItem(new MenuItem("rtoq", "   To Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("rtoturret", "   To turret", true)).SetValue(false);
            menuC.AddItem(new MenuItem("rtokill", "   To kill", true)).SetValue(true);
            menuC.AddItem(new MenuItem("Rmin", "Use R teamfight", true)).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("insec", "E-R combo to Q", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            //menuC.AddItem(new MenuItem("insecOnlyStun", "   Only Stunned enemy", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q harass", true)).SetValue(true);
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("qMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("eMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);

            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("useEint", "Use E interrupt", true)).SetValue(true);
            menuM.AddItem(new MenuItem("useRint", "Use R interrupt", true)).SetValue(false);
            menuM.AddItem(new MenuItem("usewgc", "Use W gapclosers", true)).SetValue(false);
            menuM.AddItem(new MenuItem("useegc", "Use E gapclosers", true)).SetValue(true);
            menuM.AddItem(new MenuItem("autoQ", "Auto detonate Q", true)).SetValue(true);
            menuM = DrawHelper.AddMisc(menuM);

            config.AddSubMenu(menuM);


            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }

    internal class GragasQ
    {
        public Vector3 position;
        public int time;

        public GragasQ(Vector3 _position, int _tickCount)
        {
            position = _position;
            time = _tickCount;
        }

        public float deltaT()
        {
            return System.Environment.TickCount - time;
        }
    }
}