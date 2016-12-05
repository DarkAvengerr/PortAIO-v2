// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SenseElise">
//      Copyright (c) SenseElise. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Orbwalking = SebbyLib.Orbwalking;
using Prediction = SebbyLib.Prediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Sense_EliseReborn
{
    internal class Program
    {
        private static Menu Option;
        private static Orbwalking.Orbwalker orbWalker;
        private static Spell Q, W, E, R, Q2, W2, E2;

        /*private static readonly float[] HumanQcd = {6, 6, 6, 6, 6};
        private static readonly float[] HumanWcd = {12, 12, 12, 12, 12};
        private static readonly float[] HumanEcd = {14, 13, 12, 11, 10};
        private static readonly float[] SpiderQcd = {6, 6, 6, 6, 6};
        private static readonly float[] SpiderWcd = {12, 12, 12, 12, 12};
        private static readonly float[] SpiderEcd = {26, 23, 20, 17, 14};*/
        private static float _humQcd, _humWcd, _humEcd;
        private static float _spidQcd, _spidWcd, _spidEcd;
        private static float _humaQcd, _humaWcd, _humaEcd;
        private static float _spideQcd, _spideWcd, _spideEcd;
        public static HpBarIndicator Indicator = new HpBarIndicator();
        private static AIHeroClient Player => ObjectManager.Player;

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Elise") return;

            Chat.Print(
                "<font color='#5CD1E5'>[Sense Elise]</font><font color='#FF0000'> Thank you for using this assembly \n<font color='#1DDB16'>if you have any feedback, let me know that.</font>");

            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 1075f);

            Q2 = new Spell(SpellSlot.Q, 475f);
            W2 = new Spell(SpellSlot.W);
            E2 = new Spell(SpellSlot.E, 750f);

            R = new Spell(SpellSlot.R);

            W.SetSkillshot(0.25f, 100f, 1000, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 55f, 1600, true, SkillshotType.SkillshotLine);

            MainMenu();
            Game.OnUpdate += OnUpate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void OnUpate(EventArgs args)
        {
            if (Player.IsDead) return;

            KillSteal();
            Instant_Rappel();
            Cooldowns();

            switch (orbWalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }

            if (Option.Item("Spider Combo E").GetValue<KeyBind>().Active)
                CastSpiderE();
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Human() && Option_Item("GapCloser Human E") && E.IsReady())
                if (ObjectManager.Player.Distance(gapcloser.Sender) <= E.Range)
                    E.Cast(gapcloser.Sender);

            if (Spider() && Option_Item("GapCloser Spider E") && E2.IsReady())
                if (ObjectManager.Player.Distance(gapcloser.Sender) <= E2.Range)
                    E2.Cast(gapcloser.Sender);
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Human() && Option_Item("Interrupt Human E") && E.IsReady())
                if ((ObjectManager.Player.Distance(sender) <= E.Range) &&
                    (args.DangerLevel >= Interrupter2.DangerLevel.Medium) &&
                    (E.GetPrediction(sender).Hitchance >= HitChance.Medium))
                    E.Cast(sender);
        }

        private static void Harass()
        {
            if (Player.ManaPercent <= Option.Item("HMana").GetValue<Slider>().Value)
            {
                if (Option_Item("Human Harass E"))
                    CastHumanE();

                if (Option_Item("Human Harass W"))
                    CastHumanW();

                if (Option_Item("Human Harass Q"))
                    CastHumanQ();
            }


            if (Option_Item("Spider Harass Q"))
                CastSpiderQ();
            /*
                        if (Option_Item("Spider Harass W"))
                            CastSpiderW();
                            */
        }

        private static void LaneClear()
        {
            var Minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            if (Minions != null)
            {
                if (Human() && (Player.ManaPercent >= Option.Item("LMana").GetValue<Slider>().Value))
                {
                    if (Option_Item("Human Lane W") && W.IsReady())
                    {
                        var farmLocation = W.GetLineFarmLocation(Minions);
                        if (farmLocation.MinionsHit >= 3)
                            W.Cast(farmLocation.Position, true);
                    }

                    if (Option_Item("Human Lane Q") && Q.IsReady())
                    {
                        var Minion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .Where(x => x.Health < W.GetDamage(x))
                            .OrderByDescending(x => x.MaxHealth)
                            .ThenByDescending(x => x.Distance(Player))
                            .FirstOrDefault();
                        if (Minion != null)
                            Q.Cast(Minion, true);
                    }
                }
                if (Spider())
                {
                    if (Option_Item("Spider Lane Q") && Q2.IsReady())
                    {
                        var Minion = MinionManager.GetMinions(Q2.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .Where(x => x.Health < W.GetDamage(x))
                            .OrderByDescending(x => x.MaxHealth)
                            .ThenByDescending(x => x.Distance(Player))
                            .FirstOrDefault();
                        if (Minion != null)
                            Q2.Cast(Minion, true);
                    }

                    if (Option_Item("Spider Lane W") && W2.IsReady())
                    {
                        var Minion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 150, MinionTypes.All,
                            MinionTeam.NotAlly);
                        if (!Orbwalking.CanAttack() && Orbwalking.CanMove(10) && (Minion != null))
                            W2.Cast(true);
                    }
                }
            }
            if (Minions == null) return;
        }

        private static void JungleClear()
        {
            var JungleMinions = MinionManager.GetMinions(Player.ServerPosition, W.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (JungleMinions.Count >= 1)
                foreach (var Mob in JungleMinions)
                {
                    if (Human())
                    {
                        if (Option_Item("Jungle R") && R.IsReady())
                            if (!Q.IsReady() && !W.IsReady())
                                if (((_spideQcd == 0) && (_spideWcd <= 1.8f)) || (_humaQcd >= 1.2f))
                                    R.Cast(true);

                        if (Player.ManaPercent >= Option.Item("JMana").GetValue<Slider>().Value)
                        {
                            if (Option_Item("Human Jungle W") && W.IsReady())
                            {
                                var Mobs = W.GetCircularFarmLocation(JungleMinions);
                                if (JungleMinions.Count == 4)
                                    if (Mobs.MinionsHit >= 3)
                                        W.Cast(Mobs.Position, true);
                                if (JungleMinions.Count == 3)
                                    if (Mobs.MinionsHit >= 2)
                                        W.Cast(Mobs.Position, true);
                                ;
                                if (JungleMinions.Count <= 2)
                                    W.Cast(Mob.Position, true);

                                if (JungleMinions.Count == 0) return;
                            }

                            if (Option_Item("Human Jungle Q") && Q.IsReady())
                                Q.CastOnUnit(Mob, true);
                        }
                    }

                    if (Spider())
                    {
                        if (Option_Item("Jungle R") && R.IsReady())
                            if (!Q2.IsReady() && !W2.IsReady() && !Player.HasBuff("EliseSpiderW") &&
                                (Player.ManaPercent >= Option.Item("JMana").GetValue<Slider>().Value))
                                if ((_humaQcd <= 0f) && (_humaWcd <= 1.5f) &&
                                    ((_spideQcd >= 1.4f) || (_spideWcd >= 1.8f)) &&
                                    (((JungleMinions.Count == 1) && (Mob.Health >= Q.GetDamage(Mob))) ||
                                     (Mob.Health >= W.GetDamage(Mob))))
                                    R.Cast(true);

                        if (Option_Item("Spider Jungle Q") && Q.IsReady())
                            Q.CastOnUnit(Mob, true);

                        if (Option_Item("Spider Jugnle W") && W2.IsReady())
                        {
                            var JungleMinion = MinionManager.GetMinions(Player.ServerPosition, 150, MinionTypes.All,
                                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                            if (!Orbwalking.CanAttack() && Orbwalking.CanMove(10))
                                if (JungleMinion != null)
                                    W2.Cast(true);
                        }
                    }
                }

            if (JungleMinions == null) return;
        }

        private static void Combo()
        {
            if (Option_Item("Combo R"))
                CastR();

            if (Option_Item("Human Combo E"))
                CastHumanE();

            if (Option_Item("Human Combo W"))
                CastHumanW();

            if (Option_Item("Human Combo Q"))
                CastHumanQ();

            if (Option_Item("Spider Combo Q"))
                CastSpiderQ();
            /*
            if (Option_Item("Spider Combo W"))
                CastSpiderW();
            */
            if (Option_Item("Spider Combo E Auto"))
                CastSpiderAutoE();
        }

        private static void CastHumanQ()
        {
            if (Human() && Q.IsReady())
            {
                var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical, true);
                if (Target != null)
                    Q.CastOnUnit(Target, true);
            }
        }

        private static void CastHumanW()
        {
            if (Human() && W.IsReady())
            {
                var Target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical, true);
                var prediction = W.GetPrediction(Target);
                if (Target != null)
                    if ((prediction.CollisionObjects.Count == 0) && (prediction.Hitchance >= HitChance.Medium))
                        W.Cast(Target.ServerPosition, true);
            }
        }

        private static void CastHumanE()
        {
            if (Human() && E.IsReady())
            {
                var Target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (Target != null)
                    SebbySpell(E, Target);
            }
        }

        private static void CastSpiderQ()
        {
            if (Spider() && Q2.IsReady())
            {
                var Target = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Magical, true);
                if (Target != null)
                    Q2.CastOnUnit(Target, true);
            }
        }

        /*
        static void CastSpiderW()
        {
            if (Spider() && W2.IsReady())
            {
                var target = TargetSelector.GetTarget(150, TargetSelector.DamageType.Magical);
                if (target != null)
                    if (!Orbwalking.CanAttack() && Orbwalking.CanMove(10))
                        W2.Cast(true);
            }
        }
            */

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Spider() && unit.IsMe)
                if (W.IsReady())
                    if (((orbWalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) && Option_Item("Spider Harass W")) ||
                        ((orbWalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) && Option_Item("Spider Lane W")) ||
                        ((orbWalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) && Option_Item("Spider Jungle W")) ||
                        ((orbWalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) && Option_Item("Spider Combo W")))
                        if (target is AIHeroClient || target is Obj_AI_Minion || target is Obj_AI_Turret)
                            W.Cast();
        }

        private static void CastSpiderE()
        {
            if (Spider() && E2.IsReady())
            {
                var Target = TargetSelector.GetTarget(E2.Range, TargetSelector.DamageType.True);
                var EQtarget = TargetSelector.GetTarget(E2.Range + Q.Range, TargetSelector.DamageType.True);
                var sEMinions = MinionManager.GetMinions(Player.ServerPosition, E2.Range).FirstOrDefault();
                var sE2Minions =
                    MinionManager.GetMinions(E2.Range + Q.Range, MinionTypes.All, MinionTeam.Enemy,
                        MinionOrderTypes.None);

                if (Target != null)
                {
                    if (Target.CanMove && (Player.Distance(Target) < E2.Range - 10))
                        E2.Cast(Target);
                    if (!Target.CanMove)
                        E2.Cast(Target);
                }

                if (EQtarget != null)
                    if (EQtarget.CanMove && (Player.Distance(EQtarget) < E2.Range + Q2.Range - 10))
                        if (sE2Minions != null)
                        {
                            var sE2MinionsSingle = sE2Minions.FirstOrDefault(
                                x => (sEMinions != null) && (x.Distance(Player.Position) < Q.Range) &&
                                     (Player.Distance(sEMinions.Position) < E2.Range));
                            E2.Cast(sE2MinionsSingle);
                        }
            }
        }

        private static void CastSpiderAutoE()
        {
            if (Spider() && E2.IsReady())
            {
                var target = TargetSelector.GetTarget(E2.Range, TargetSelector.DamageType.True);
                if (target != null)
                    if ((!Q2.IsReady() || (Q2.Range <= Player.Distance(target))) && !W2.IsReady())
                        if (target.HasBuffOfType(BuffType.Stun))
                            E2.Cast();
            }
        }

        private static void CastR()
        {
            var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var Target2 = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Magical);

            if ((Target == null) || !R.IsReady()) return;
            if (Human())
                if (!Q.IsReady() && !W.IsReady() && !E.IsReady())
                    if ((_spideQcd <= 0f) && (_spideWcd <= 1.8f))
                        if ((Target.Health <= Q.GetDamage(Target)) && (_humaQcd <= 1.5f) &&
                            ((_humaQcd <= 1.2f) || (_humaWcd <= 2))) return;
                        else
                            R.Cast();

            if (Spider())
                if (!Q2.IsReady() && !W2.IsReady() && !Player.HasBuff("EliseSpiderW"))
                    if ((_humaQcd <= 0f) || ((_humaWcd <= 1.5f) && (_humaEcd <= 0.8f)))
                        if ((!(Target2.Health <= Q2.GetDamage(Target2)) || !(_spideQcd <= 1.0f)) && !(_spideQcd <= 1.4f) &&
                            !(_spideWcd <= 1.9f))
                            R.Cast();
        }

        private static void KillSteal()
        {
            if (Human())
            {
                if (Option_Item("KillSteal Human Q") && Q.IsReady())
                {
                    var Qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                    if ((Qtarget != null) && (Qtarget.Health <= Q.GetDamage(Qtarget)))
                        Q.CastOnUnit(Qtarget);
                }

                if (Option_Item("KillSteal Human W") && W.IsReady())
                {
                    var Wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                    var prediction = W.GetPrediction(Wtarget);
                    if ((Wtarget != null) && (Wtarget.Health <= W.GetDamage(Wtarget)) &&
                        (prediction.CollisionObjects.Count == 0))
                        W.Cast(Wtarget.ServerPosition);
                }
            }
            if (Spider())
                if (Option_Item("KillSteal Spider Q") && Q2.IsReady())
                {
                    var Q2target = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Magical);
                    if ((Q2target != null) && (Q2target.Health <= Q2.GetDamage(Q2target)))
                        Q2.CastOnUnit(Q2target);
                }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
                GetCDs(args);
        }

        private static void Instant_Rappel()
        {
            if (Option.Item("Fast Instant Rappel").GetValue<KeyBind>().Active)
            {
                if (Human() && R.IsReady())
                {
                    R.Cast();
                    E2.Cast();
                }
                if (Spider())
                    E2.Cast();
            }
        }

        private static bool Option_Item(string itemname)
        {
            return Option.Item(itemname).GetValue<bool>();
        }

        private static bool Human()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ";
        }

        private static bool Spider()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast";
        }

        private static void Cooldowns()
        {
            _humaQcd = _humQcd - Game.Time > 0 ? _humQcd - Game.Time : 0;
            _humaWcd = _humWcd - Game.Time > 0 ? _humWcd - Game.Time : 0;
            _humaEcd = _humEcd - Game.Time > 0 ? _humEcd - Game.Time : 0;
            _spideQcd = _spidQcd - Game.Time > 0 ? _spidQcd - Game.Time : 0;
            _spideWcd = _spidWcd - Game.Time > 0 ? _spidWcd - Game.Time : 0;
            _spideEcd = _spidEcd - Game.Time > 0 ? _spidEcd - Game.Time : 0;
        }

        private static void GetCDs(GameObjectProcessSpellCastEventArgs spell)
        {
            if (Human())
            {
                if (spell.SData.Name == "EliseHumanQ")
                    _humQcd = spell.SData.CooldownTime;
                if (spell.SData.Name == "EliseHumanW")
                    _humWcd = spell.SData.CooldownTime;
                if (spell.SData.Name == "EliseHumanE")
                    _humEcd = spell.SData.CooldownTime;
            }
            if (Spider())
            {
                if (spell.SData.Name == "EliseSpiderQCast")
                    _spidQcd = spell.SData.CooldownTime;
                if (spell.SData.Name == "EliseSpiderW")
                    _spidWcd = spell.SData.CooldownTime;
                if (spell.SData.Name == "EliseSpiderEInitial")
                    _spidEcd = spell.SData.CooldownTime;
            }
        }

        /*private static float CalculateCd(float time)
        {
            return time + time*Player.PercentCooldownMod;
        }*/

        private static float GetComboDamage(AIHeroClient Enemy)
        {
            float damage = 0;

            if (Q.IsReady())
                damage += Q.GetDamage(Enemy);
            if (W.IsReady())
                damage += W.GetDamage(Enemy);
            if (Q2.IsReady())
                damage += Q2.GetDamage(Enemy);
            if (W2.IsReady())
                damage += W2.GetDamage(Enemy);
            if (!ObjectManager.Player.Spellbook.IsAutoAttacking)
                damage += (float) ObjectManager.Player.GetAutoAttackDamage(Enemy, true);


            return damage;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Player.IsDead) return;
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget() && !ene.IsZombie))
                if (Option_Item("DamageAfterCombo"))
                {
                    Indicator.unit = enemy;
                    Indicator.drawDmg(GetComboDamage(enemy), new ColorBGRA(255, 204, 0, 160));
                }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var elise = Drawing.WorldToScreen(Player.Position);
            if (Human())
            {
                if (Option_Item("Human Q Draw"))
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color.White, 1);

                if (Option_Item("Human W Draw"))
                    Render.Circle.DrawCircle(Player.Position, W.Range, Color.Yellow, 1);

                if (Option_Item("Human E Draw Range"))
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color.Green, 1);

                var ETarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.True);
                if (Option_Item("Human E Draw Target"))
                    if (ETarget != null)
                        Drawing.DrawCircle(ETarget.Position, 150, Color.Green);

                if (Option_Item("Spider Skill Cooldown"))
                {
                    if ((_spideQcd <= 0) && (Q2.Level > 0))
                        Drawing.DrawText(elise[0] - 70, elise[1], Color.White, "S-Q Ready");
                    else
                        Drawing.DrawText(elise[0] - 70, elise[1], Color.Orange, "S-Q: " + _spideQcd.ToString("0.0"));

                    if ((_spideWcd <= 0) && (W2.Level > 0))
                        Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.White, "S-W Ready");
                    else
                        Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.Orange, "S-W: " + _spideWcd.ToString("0.0"));

                    if ((_spideEcd <= 0) && (E2.Level > 0))
                        Drawing.DrawText(elise[0] + 20, elise[1], Color.White, "S-E Ready");
                    else
                        Drawing.DrawText(elise[0] + 20, elise[1], Color.Orange, "S-E: " + _spideEcd.ToString("0.0"));
                }
            }

            if (Spider())
            {
                if (Option_Item("Spider Q Draw"))
                    Render.Circle.DrawCircle(Player.Position, Q2.Range, Color.White, 1);

                if (Option_Item("Spider E Draw Range"))
                    Render.Circle.DrawCircle(Player.Position, E2.Range, Color.Yellow, 1);

                var E2target = TargetSelector.GetTarget(E2.Range, TargetSelector.DamageType.True);
                if (Option_Item("Spider E Draw Target"))
                    if (E2target != null)
                        Drawing.DrawCircle(E2target.Position, 150, Color.Green);


                var EQtarget = TargetSelector.GetTarget(E2.Range + Q2.Range, TargetSelector.DamageType.True);
                var sEMinions = MinionManager.GetMinions(Player.ServerPosition, E2.Range).FirstOrDefault();
                var sE2Minions =
                    MinionManager.GetMinions(E2.Range + Q.Range, MinionTypes.All, MinionTeam.Enemy,
                            MinionOrderTypes.None)
                        .FirstOrDefault(
                            x =>
                                (x.Distance(Player.Position) < Q.Range) &&
                                (Player.Distance(sEMinions.Position) < E2.Range));
                if (Option_Item("Spider EQ Draw Target"))
                    if ((EQtarget != null) && (E2target == null) && (sE2Minions != null))
                        Drawing.DrawCircle(EQtarget.Position, 150, Color.Blue);

                if (Option_Item("Human Skill Cooldown"))
                {
                    if ((_humaQcd <= 0) && (Q.Level > 0))
                        Drawing.DrawText(elise[0] - 70, elise[1], Color.White, "H-Q Ready");
                    else
                        Drawing.DrawText(elise[0] - 70, elise[1], Color.Orange, "H-Q: " + _humaQcd.ToString("0.0"));

                    if ((_humaWcd <= 0) && (W.Level > 0))
                        Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.White, "H-W Ready");
                    else
                        Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.Orange, "H-W: " + _humaWcd.ToString("0.0"));

                    if ((_humaEcd <= 0) && (E.Level > 0))
                        Drawing.DrawText(elise[0] + 20, elise[1], Color.White, "H-E Ready");
                    else
                        Drawing.DrawText(elise[0] + 20, elise[1], Color.Orange, "H-E: " + _humaEcd.ToString("0.0"));
                }
            }
        }

        private static void SebbySpell(Spell E, Obj_AI_Base target)
        {
            var CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            var aoe2 = false;

            if (E.Type == SkillshotType.SkillshotCircle)
            {
                CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe2 = true;
            }

            if ((E.Width > 80) && !E.Collision)
                aoe2 = true;

            var predInput2 = new PredictionInput
            {
                Aoe = aoe2,
                Collision = E.Collision,
                Speed = E.Speed,
                Delay = E.Delay,
                Range = E.Range,
                From = Player.ServerPosition,
                Radius = E.Width,
                Unit = target,
                Type = CoreType2
            };
            var poutput2 = Prediction.GetPrediction(predInput2);

            if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if (OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if (Option.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                    E.Cast(poutput2.CastPosition);
            }
            else if (Option.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    E.Cast(poutput2.CastPosition);
            }
            else if (Option.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                    E.Cast(poutput2.CastPosition);
            }
        }

        private static void MainMenu()
        {
            Option = new Menu("Sense Elise", "Sense_Elise", true).SetFontStyle(FontStyle.Regular, SharpDX.Color.SkyBlue);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Option.AddSubMenu(targetSelectorMenu);

            Option.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbWalker = new Orbwalking.Orbwalker(Option.SubMenu("Orbwalker"));

            var Prediction = new Menu("Prediction Mode", "Prediction Mode");
            {
                Prediction.AddItem(
                    new MenuItem("HitChance", "Human E HitChance").SetValue(
                        new StringList(new[] {"Medium", "High", "VeryHigh"}, 1)));
            }
            Option.AddSubMenu(Prediction);

            var Harass = new Menu("Harass", "Harass");
            {
                Harass.SubMenu("Human Skill").AddItem(new MenuItem("Human Harass Q", "Use Q").SetValue(true));
                Harass.SubMenu("Human Skill").AddItem(new MenuItem("Human Harass W", "Use W").SetValue(true));
                Harass.SubMenu("Human Skill").AddItem(new MenuItem("Human Harass E", "Use E").SetValue(true));
                Harass.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Harass Q", "Use Q").SetValue(true));
                Harass.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Harass W", "Use W").SetValue(true));
                Harass.AddItem(new MenuItem("HMana", "Mana Manager (%)").SetValue(new Slider(40)));
            }
            Option.AddSubMenu(Harass);

            var LaneClear = new Menu("LaneClear", "LaneClear");
            {
                LaneClear.SubMenu("Human Skill").AddItem(new MenuItem("Human Lane Q", "Use Q").SetValue(true));
                LaneClear.SubMenu("Human Skill").AddItem(new MenuItem("Human Lane W", "Use W").SetValue(true));
                LaneClear.SubMenu("Human Skill").AddItem(new MenuItem("Human Lane E", "Use E").SetValue(true));
                LaneClear.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Lane Q", "Use Q").SetValue(true));
                LaneClear.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Lane W", "Use W").SetValue(true));
                LaneClear.AddItem(new MenuItem("LMana", "Mana Manager (%)").SetValue(new Slider(40)));
            }
            Option.AddSubMenu(LaneClear);

            var JungleClear = new Menu("JungleClear", "JungleClear");
            {
                JungleClear.SubMenu("Human Skill").AddItem(new MenuItem("Human Jungle Q", "Use Q").SetValue(true));
                JungleClear.SubMenu("Human Skill").AddItem(new MenuItem("Human Jungle W", "Use W").SetValue(true));
                JungleClear.SubMenu("Human Skill").AddItem(new MenuItem("Human Jungle E", "Use E").SetValue(true));
                JungleClear.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Jungle Q", "Use Q").SetValue(true));
                JungleClear.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Jungle W", "Use W").SetValue(true));
                JungleClear.AddItem(new MenuItem("Jungle R", "Auto Switch Form").SetValue(true));
                JungleClear.AddItem(new MenuItem("JMana", "Mana Manager (%)").SetValue(new Slider(40)));
            }
            Option.AddSubMenu(JungleClear);

            var Combo = new Menu("Combo", "Combo");
            {
                Combo.SubMenu("Human Skill").AddItem(new MenuItem("Human Combo Q", "Use Q").SetValue(true));
                Combo.SubMenu("Human Skill").AddItem(new MenuItem("Human Combo W", "Use W").SetValue(true));
                Combo.SubMenu("Human Skill").AddItem(new MenuItem("Human Combo E", "Use E").SetValue(true));
                Combo.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Combo Q", "Use Q").SetValue(true));
                Combo.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Combo W", "Use W").SetValue(true));
                Combo.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Combo E Auto", "Use Auto E").SetValue(false));
                Combo.SubMenu("Spider Skill")
                    .AddItem(new MenuItem("Spider Combo E", "Use E").SetValue(new KeyBind('T', KeyBindType.Press)));
                Combo.AddItem(new MenuItem("Combo R", "Auto Switch Form").SetValue(true));
            }
            Option.AddSubMenu(Combo);

            var Misc = new Menu("Misc", "Misc");
            {
                Misc.SubMenu("KillSteal").AddItem(new MenuItem("KillSteal Human Q", "Use Q").SetValue(true));
                Misc.SubMenu("KillSteal").AddItem(new MenuItem("KillSteal Human W", "Use W").SetValue(false));
                Misc.SubMenu("KillSteal").AddItem(new MenuItem("KillSteal Spider Q", "Use Q").SetValue(true));
                Misc.SubMenu("Interrupt").AddItem(new MenuItem("Interrupt Human E", "Use Human E").SetValue(true));
                Misc.SubMenu("Anti-GapCloser").AddItem(new MenuItem("GapCloser Human E", "Use Human E").SetValue(true));
                Misc.SubMenu("Anti-GapCloser")
                    .AddItem(new MenuItem("GapCloser Spider E", "Use Spider E").SetValue(false));
                /*
                Misc.SubMenu("Smite").AddItem(new MenuItem("Smite Blue", "Smite Use Blue").SetValue(false));
                Misc.SubMenu("Smite").AddItem(new MenuItem("Smite Red", "Smite Use Red").SetValue(false));
                Misc.SubMenu("Smite").AddItem(new MenuItem("Smite Dragon", "Smite Use Dragon").SetValue(true));
                Misc.SubMenu("Smite").AddItem(new MenuItem("Smite Baron", "Smite Use Dragon").SetValue(true));
                Misc.SubMenu("Smite").AddItem(new MenuItem("Smite Enemy", "Smite Use Enemy(Click the Target)").SetValue(true));
                */
                Misc.AddItem(
                    new MenuItem("Fast Instant Rappel", "Fast Instant_Rappel").SetValue(new KeyBind('G',
                        KeyBindType.Press)));
            }
            Option.AddSubMenu(Misc);

            var Drawing = new Menu("Drawing", "Drawing");
            {
                Drawing.SubMenu("Human Skill").AddItem(new MenuItem("Human Q Draw", "Use Q").SetValue(false));
                Drawing.SubMenu("Human Skill").AddItem(new MenuItem("Human W Draw", "Use W").SetValue(false));
                Drawing.SubMenu("Human Skill")
                    .AddItem(new MenuItem("Human E Draw Range", "Use E Range").SetValue(false));
                Drawing.SubMenu("Human Skill")
                    .AddItem(new MenuItem("Human E Draw Target", "Use E Target").SetValue(true));
                Drawing.SubMenu("Human Skill")
                    .AddItem(new MenuItem("Human Skill Cooldown", "Skill Cooldown").SetValue(true));
                Drawing.SubMenu("Spider Skill").AddItem(new MenuItem("Spider Q Draw", "Use Q").SetValue(false));
                Drawing.SubMenu("Spider Skill")
                    .AddItem(new MenuItem("Spider E Draw Range", "Use E Range").SetValue(false));
                Drawing.SubMenu("Spider Skill")
                    .AddItem(new MenuItem("Spider E Draw Target", "Use E Target").SetValue(true));
                Drawing.SubMenu("Spider Skill")
                    .AddItem(new MenuItem("Spider EQ Draw Target", "Use EQ Target").SetValue(true));
                Drawing.SubMenu("Spider Skill")
                    .AddItem(new MenuItem("Spider EQ Draw Minion", "Use EQ Target(Minion Jump)").SetValue(true));
                Drawing.SubMenu("Spider Skill")
                    .AddItem(new MenuItem("Spider Skill Cooldown", "Skill Cooldown").SetValue(true));
                Drawing.AddItem(new MenuItem("DamageAfterCombo", "Draw Combo Damage").SetValue(true));
            }
            Option.AddSubMenu(Drawing);

            Option.AddToMainMenu();
        }
    }
}