/**
 * 
 * Love Ya Lads!
 * 
 * 
 **/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SurvivorSeriesAIO.Core;
using SurvivorSeriesAIO.SurvivorMain;
using SurvivorSeriesAIO.Utility;
using Orbwalking = SebbyLib.Orbwalking;

#pragma warning disable CS0649

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Champions
{
    internal class Ryze : ChampionBase
    {
        public static SpellSlot igniteslot;
        private float QRealDamage;
        //private AIHeroClient Player { get { return ObjectManager.Player; } }
        private float RangeR;
        //private Spell Q, W, E, R;
        public List<Spell> SpellList = new List<Spell>();

        public Ryze(IRootMenu menu, Orbwalking.Orbwalker Orbwalker)
            : base(menu, Orbwalker)
        {
            // manual override - default is spell values from LeagueSharp.Data
            Q.SetSkillshot(0.7f, 55f, float.MaxValue, true, SkillshotType.SkillshotLine);
            W.SetTargetted(0.103f, 550f);
            E.SetTargetted(.5f, 550f);
            R.SetSkillshot(2.5f, 450f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += OnWndProc;
            Drawing.OnEndScene += OnEndScene;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Config = new Configuration(menu.Champion);
            InitHpbarOverlay();
        }

        public Configuration Config { get; }

        /*public void LoadSS()
        {

            #region Subscriptions
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            #endregion
        }*/

        private void InitHpbarOverlay()
        {
            DrawDamage.DamageToUnit = CalculateDamage;
            DrawDamage.Enabled = () => Config.DrawComboDamage.GetValue<bool>();
            DrawDamage.Fill = () => Config.FillColor.GetValue<Circle>().Active;
            DrawDamage.FillColor = () => Config.FillColor.GetValue<Circle>().Color;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            switch (R.Level)
            {
                case 1:
                    RangeR = 1750f;
                    break;
                case 2:
                    RangeR = 3000f;
                    break;
            }
            if (Config.drawQ.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Chartreuse);
            if (Config.drawWE.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.DeepPink);
            if (Config.drawR.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, RangeR, Color.Aqua);

            if (!Config.DrawSpellFarm.GetValue<bool>())
                return;

            if (Config.EnableFarming.GetValue<bool>())
            {
                var drawPos = Drawing.WorldToScreen(Player.Position);
                var textSize = Drawing.GetTextEntent(("Spell Farm: ON"), 15);
                Drawing.DrawText(drawPos.X - textSize.Width - 70f, drawPos.Y, Color.Chartreuse, "Spell Farm: ON");
            }
            else
            {
                var drawPos = Drawing.WorldToScreen(Player.Position);
                var textSize = Drawing.GetTextEntent(("Spell Farm: OFF"), 15);
                Drawing.DrawText(drawPos.X - textSize.Width - 70f, drawPos.Y, Color.DeepPink, "Spell Farm: OFF");
            }
        }

        private void OnWndProc(WndEventArgs args)
        {
            if (!Config.EnableScrollToFarm.GetValue<bool>())
                return;

            if (args.Msg == 0x20a)
                Config.EnableFarming.SetValue(!Config.EnableFarming.GetValue<bool>());
        }

        private void AABlock()
        {
            var enemy = TargetSelector.GetTarget(550, TargetSelector.DamageType.Magical);
            if (enemy.IsValidTarget() && (enemy.Health < Player.GetAutoAttackDamage(enemy)))
                Orbwalker.ForceTarget(enemy);
            else
                Orbwalker.SetAttack(!Config.CBlockAA.GetValue<bool>());
            //SebbyLib.OktwCommon.blockAttack = Config.Item("CBlockAA").GetValue<bool>();
        }

        private void StackItems()
        {
            if (Player.InFountain() ||
                (Player.HasBuff("CrestoftheAncientGolem") && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None) &&
                 Config.StackTearNF.GetValue<bool>())) // Add if Player has Blue Buff
                if (Items.HasItem(3004, Player) || Items.HasItem(3003, Player) || Items.HasItem(3070, Player) ||
                    Items.HasItem(3072, Player) || Items.HasItem(3073, Player) || Items.HasItem(3008, Player))
                    Q.Cast(Player.ServerPosition);
        }

        private void OnEndScene(EventArgs args)
        {
            switch (R.Level)
            {
                case 1:
                    RangeR = 1750f;
                    break;
                case 2:
                    RangeR = 3000f;
                    break;
            }

            if (Config.DrawRMinimap.GetValue<bool>() && (R.Level > 0) && R.IsReady())
            {
#pragma warning disable CS0618 // Type or member is obsolete
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, RangeR, Color.DeepPink, 2, 45, true);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            if (Config.StackTear.GetValue<bool>())
                StackItems();
            KSCheck();
            // Item Usage
            if (Config.UseSkin.GetValue<bool>())
                //Player.SetSkin(Player.CharData.BaseSkinName, Config.SkinID.GetValue<Slider>().Value);
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    AABlock();
                    Combo();
                    ComboPlusCheck();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear();
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    Orbwalker.SetMovement(true);
                    Orbwalker.SetAttack(true);
                    break;
            }
            if (Config.UltimateUseR.GetValue<KeyBind>().Active)
            {
                Orbwalker.SetMovement(false);
                Orbwalker.SetAttack(false);
                REscape();
            }
        }

        private void JungleClear()
        {
            if (Player.ManaPercent < Config.jungleclearMinimumMana.GetValue<Slider>().Value)
                return;

            var jgcq = Config.UseQJC.GetValue<bool>();
            var jgcw = Config.UseWJC.GetValue<bool>();
            var jgce = Config.UseEJC.GetValue<bool>();

            var mob =
                MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (mob == null || !mob.IsValidTarget())
                return;
            if (jgcq && jgce && Q.IsReady() && E.IsReady())
            {
                Q.Cast(mob.Position);
                E.CastOnUnit(mob);
                Q.Cast(mob.Position);
                if (jgcw && W.IsReady() && !Q.IsReady())
                {
                    W.CastOnUnit(mob);
                    Q.Cast(mob.Position);
                }
            }
            else if (jgcq && jgce && !Q.IsReady() && E.IsReady())
            {
                E.CastOnUnit(mob);
                Q.Cast(mob.Position);
                if (jgcw && W.IsReady() && !Q.IsReady())
                {
                    W.CastOnUnit(mob);
                    Q.Cast(mob.Position);
                }
            }
            else if (jgcq && jgce && jgcw && !Q.IsReady() && !E.IsReady() && W.IsReady())
            {
                W.CastOnUnit(mob);
                Q.Cast(mob.Position);
                if (E.IsReady())
                {
                    E.CastOnUnit(mob);
                    Q.Cast(mob.Position);
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.WGapCloser.GetValue<bool>() || (Player.Mana < W.Instance.SData.Mana + Q.Instance.SData.Mana))
                return;

            var t = gapcloser.Sender;

            if (gapcloser.End.Distance(Player.ServerPosition) < W.Range)
                W.Cast(t);
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            var WCast = Config.InterruptWithW.GetValue<bool>();
            if (!WCast || !t.IsValidTarget(W.Range) || !W.IsReady()) return;
            W.Cast(t);
        }

        private void KSCheck()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if ((target == null) || !target.IsValidTarget(Q.Range) || target.IsInvulnerable)
                return;

            var ksQ = Config.KSQ.GetValue<bool>();
            var ksW = Config.KSW.GetValue<bool>();
            var ksE = Config.KSE.GetValue<bool>();

            // KS
            if (ksQ && (OktwCommon.GetKsDamage(target, Q) > target.Health) && target.IsValidTarget(Q.Range))
                SpellCast.SebbySpellMain(Q, target);
            if (ksW && (OktwCommon.GetKsDamage(target, W) > target.Health) && target.IsValidTarget(W.Range))
                W.CastOnUnit(target);
            if (ksE && (OktwCommon.GetKsDamage(target, E) > target.Health) && target.IsValidTarget(E.Range))
                E.CastOnUnit(target);
        }

        public bool RyzeCharge0()
        {
            return Player.HasBuff("ryzeqiconnocharge");
        }

        public bool RyzeCharge1()
        {
            return Player.HasBuff("ryzeqiconhalfcharge");
        }

        public bool RyzeCharge2()
        {
            return Player.HasBuff("ryzeqiconfullcharge");
        }

        private float QGetRealDamage(Obj_AI_Base target)
        {
            if (!target.HasBuff("RyzeE"))
                return Q.GetDamage(target);
            if (((E.IsReady() && !Q.IsReady()) || (E.IsReady() && Q.IsReady()) || (!E.IsReady() && Q.IsReady())) &&
                target.HasBuff("RyzeE"))
            {
                switch (E.Level)
                {
                    case 1:
                        QRealDamage = Q.GetDamage(target)/40*100;
                        break;
                    case 2:
                        QRealDamage = Q.GetDamage(target)/55*100;
                        break;
                    case 3:
                        QRealDamage = Q.GetDamage(target)/70*100;
                        break;
                    case 4:
                        QRealDamage = Q.GetDamage(target)/85*100;
                        break;
                    case 5:
                        QRealDamage = Q.GetDamage(target)/100*100;
                        break;
                }
                //Chat.Print("Inside V2 qRealDamage:" + QRealDamage);
                return QRealDamage;
            }
            //Chat.Print("Inside else at end:" + Q.GetDamage(target));
            return Q.GetDamage(target);
        }

        private void ComboPlusCheck()
        {
            // Combo
            var CUseQ = Config.ComboQUse.GetValue<bool>();
            //var CUseW = Menu.Item("CUseW").GetValue<bool>();
            var CUseE = Config.ComboEUse.GetValue<bool>();
            // Checks
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if ((target == null) || !target.IsValidTarget(Q.Range) || target.IsInvulnerable)
                return;

            var ryzeebuffed =
                MinionManager.GetMinions(Player.Position, Q.Range)
                    .Find(x => x.HasBuff("RyzeE") && x.IsValidTarget(Q.Range));
            var noebuffed =
                MinionManager.GetMinions(Player.Position, Q.Range)
                    .Find(x => x.IsValidTarget(Q.Range) && (x.Distance(target) < 200));

            if (CUseQ && CUseE && target.IsValidTarget(Q.Range))
                if ((ryzeebuffed != null) && ryzeebuffed.IsValidTarget(Q.Range))
                {
                    if (ryzeebuffed.Health < QGetRealDamage(ryzeebuffed))
                    {
                        //Chat.Print("<font color='#9400D3'>DEBUG: Spread</font>");
                        if (!Q.IsReady() && E.IsReady())
                        {
                            E.CastOnUnit(ryzeebuffed);
                            Q.Cast(ryzeebuffed);
                            //Chat.Print("<font color='#9400D3'>DEBUG: Spreading [Reset with E]</font>");
                        }
                        Q.Cast(ryzeebuffed);
                    }
                    if (target.HasBuff("RyzeE") && (target.Distance(ryzeebuffed) < 200) &&
                        ryzeebuffed.IsValidTarget(Q.Range))
                    {
                        //Chat.Print("<font color='#9400D3'>DEBUG: Got to Part 1</font>");
                        Q.Cast(ryzeebuffed);
                    }
                    else if (!target.HasBuff("RyzeE"))
                    {
                        E.CastOnUnit(target);
                        if (target.Distance(ryzeebuffed) < 200)
                            Q.Cast(ryzeebuffed);
                    }
                }
                else if ((ryzeebuffed == null) || !ryzeebuffed.IsValidTarget())
                {
                    if ((noebuffed != null) && noebuffed.IsValidTarget(E.Range) &&
                        (noebuffed.Health < QGetRealDamage(noebuffed)))
                        if (E.IsReady())
                        {
                            E.CastOnUnit(noebuffed);
                            if (Q.IsReady())
                                Q.Cast(noebuffed);
                            //Chat.Print("<font color='#9400D3'>DEBUG: Spreading [Reset with E]</font>");
                        }
                }
        }

        private void Combo()
        {
            // Combo
            var ComUseQ = Config.ComboQUse.GetValue<bool>();
            var ComUseW = Config.ComboWUse.GetValue<bool>();
            var ComUseE = Config.ComboEUse.GetValue<bool>();
            // Checks
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if ((target == null) || !target.IsValidTarget(Q.Range) || target.IsInvulnerable)
                return;

            switch (Config.ComboMode.GetValue<StringList>().SelectedIndex)
            {
                case 0:

                    #region Burst Mode

                    // Execute the Lad
                    if (Player.Mana >= Q.Instance.SData.Mana + W.Instance.SData.Mana + E.Instance.SData.Mana)
                    {
                        if (ComUseW && target.IsValidTarget(W.Range) && W.IsReady())
                            W.CastOnUnit(target);
                        if (ComUseQ && target.IsValidTarget(Q.Range))
                            SpellCast.SebbySpellMain(Q, target);
                        if (ComUseE && target.IsValidTarget(E.Range) && E.IsReady())
                            E.CastOnUnit(target);
                    }
                    else
                    {
                        if (ComUseW && target.IsValidTarget(W.Range) && W.IsReady())
                            W.CastOnUnit(target);
                        if (ComUseQ && target.IsValidTarget(Q.Range))
                            SpellCast.SebbySpellMain(Q, target);
                        if (ComUseE && target.IsValidTarget(E.Range) && E.IsReady())
                            E.CastOnUnit(target);
                    }

                    #endregion

                    break;
                case 1:

                    #region SurvivorMode

                    if ((Q.Level >= 1) && (W.Level >= 1) && (E.Level >= 1))
                    {
                        if (!target.IsValidTarget(W.Range - 15f) && Q.IsReady())
                            SpellCast.SebbySpellMain(Q, target);
                        // Try having Full Charge if either W or E spells are ready...
                        if (RyzeCharge1() && Q.IsReady() && (W.IsReady() || E.IsReady()))
                        {
                            if (E.IsReady())
                                E.Cast(target);
                            if (W.IsReady())
                                W.Cast(target);
                        }
                        if (RyzeCharge1() && !E.IsReady() && !W.IsReady())
                            SpellCast.SebbySpellMain(Q, target);

                        if (RyzeCharge0() && !E.IsReady() && !W.IsReady())
                            SpellCast.SebbySpellMain(Q, target);

                        if (!RyzeCharge2())
                        {
                            E.Cast(target);
                            W.Cast(target);
                        }
                        else
                        {
                            SpellCast.SebbySpellMain(Q, target);
                        }
                    }
                    else
                    {
                        if (target.IsValidTarget(Q.Range) && Q.IsReady())
                            SpellCast.SebbySpellMain(Q, target);

                        if (target.IsValidTarget(W.Range) && W.IsReady())
                            W.Cast(target);

                        if (target.IsValidTarget(E.Range) && E.IsReady())
                            E.Cast(target);
                    }

                    #endregion

                    break;
            }
        }

        private void Harass()
        {
            // Harass
            var HarassUseQ = Config.HarassQ.GetValue<bool>();
            var HarassUseW = Config.HarassW.GetValue<bool>();
            var HarassUseE = Config.HarassE.GetValue<bool>();
            // Checks
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var ryzeebuffed =
                MinionManager.GetMinions(Player.Position, E.Range)
                    .Find(x => x.HasBuff("RyzeE") && x.IsValidTarget(E.Range));
            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if ((target == null) || !target.IsValidTarget(Q.Range) || target.IsInvulnerable)
                return;

            // Execute the Lad
            if (Player.ManaPercent > Config.HarassManaManager.GetValue<Slider>().Value)
            {
                if (HarassUseW && target.IsValidTarget(W.Range))
                    W.CastOnUnit(target);
                if (HarassUseQ && target.IsValidTarget(Q.Range))
                    SpellCast.SebbySpellMain(Q, target);
                if (HarassUseE && ryzeebuffed.IsValidTarget() && (target.Distance(ryzeebuffed) < 200))
                    E.CastOnUnit(ryzeebuffed);
                else if (HarassUseE && (!ryzeebuffed.IsValidTarget() || (ryzeebuffed == null)) &&
                         target.IsValidTarget(W.Range))
                    E.CastOnUnit(target);
            }
        }

        private void LastHit()
        {
            var useQ = Config.UseQLH.GetValue<bool>();
            var useE = Config.UseELH.GetValue<bool>();
            // To be Done
            if (Player.ManaPercent > Config.lanehitMinimumMana.GetValue<Slider>().Value)
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Enemy);
                var allMinionsE = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Enemy);
                if (Q.IsReady() && useQ)
                {
                    if (allMinionsQ.Count > 0)
                        foreach (var minion in allMinionsQ)
                        {
                            if (!minion.IsValidTarget() || (minion == null))
                                return;
                            if (minion.Health < QGetRealDamage(minion))
                                Q.Cast(minion.Position);
                        }
                }
                else if (E.IsReady() && useE)
                {
                    if (allMinionsE.Count > 0)
                        foreach (var minion in allMinionsE)
                        {
                            if (!minion.IsValidTarget() || (minion == null))
                                return;
                            if (minion.Health < E.GetDamage(minion))
                                E.CastOnUnit(minion);
                        }
                }
            }
        }

        private void LaneClear()
        {
            if (!Config.EnableFarming.GetValue<bool>())
                return;

            // LaneClear | Notes: Rework on early levels not using that much abilities since Spell Damage is lower, higher Lvl is fine
            if (Config.UseQLC.GetValue<bool>() || Config.UseELC.GetValue<bool>())
                if (Player.ManaPercent > Config.laneclearMinimumMana.GetValue<Slider>().Value)
                {
                    var ryzeebuffed =
                        MinionManager.GetMinions(Player.Position, Q.Range)
                            .Find(x => x.HasBuff("RyzeE") && x.IsValidTarget(Q.Range));
                    var ryzenotebuffed =
                        MinionManager.GetMinions(Player.Position, Q.Range)
                            .Find(x => !x.HasBuff("RyzeE") && x.IsValidTarget(Q.Range));
                    var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Enemy);
                    var allMinions = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Enemy);
                    if (Q.IsReady() && !E.IsReady())
                        if (allMinionsQ.Count > 0)
                            foreach (var minion in allMinionsQ)
                            {
                                if (!minion.IsValidTarget() || (minion == null))
                                    return;
                                if (minion.Health < QGetRealDamage(minion))
                                    Q.Cast(minion);
                                else if ((minion.Health < QGetRealDamage(minion) + Player.GetAutoAttackDamage(minion)) &&
                                         minion.IsValidTarget(Orbwalking.GetRealAutoAttackRange(minion)))
                                {
                                    Q.Cast(minion);
                                    Orbwalker.ForceTarget(minion);
                                }
                            }
                    if (!Q.IsReady() && (Q.Level > 0) && E.IsReady())
                        if (ryzeebuffed != null)
                        {
                            if ((ryzeebuffed.Health < E.GetDamage(ryzeebuffed) + QGetRealDamage(ryzeebuffed)) &&
                                ryzeebuffed.IsValidTarget(E.Range))
                            {
                                E.CastOnUnit(ryzeebuffed);
                                if (Q.IsReady())
                                    Q.Cast(ryzeebuffed);

                                Orbwalker.ForceTarget(ryzeebuffed);
                            }
                        }
                        else if (ryzeebuffed == null)
                        {
                            foreach (var minion in allMinions)
                                if (minion.IsValidTarget(E.Range) &&
                                    (minion.Health < E.GetDamage(minion) + QGetRealDamage(minion)))
                                {
                                    E.CastOnUnit(minion);
                                    if (Q.IsReady())
                                        Q.Cast(ryzeebuffed);
                                }
                        }
                    if (Q.IsReady() && E.IsReady())
                        if (ryzeebuffed != null)
                        {
                            if ((ryzeebuffed.Health <
                                 Q.GetDamage(ryzeebuffed) + E.GetDamage(ryzeebuffed) + Q.GetDamage(ryzeebuffed)) &&
                                ryzeebuffed.IsValidTarget(E.Range))
                            {
                                Q.Cast(ryzeebuffed);
                                if (ryzeebuffed.IsValidTarget(E.Range))
                                    E.CastOnUnit(ryzeebuffed);
                                if (!E.IsReady() && Q.IsReady())
                                    Q.Cast(ryzeebuffed);
                            }
                        }
                        else if (ryzeebuffed == null)
                        {
                            if ((ryzenotebuffed.Health <
                                 Q.GetDamage(ryzenotebuffed) + E.GetDamage(ryzenotebuffed) + Q.GetDamage(ryzenotebuffed)) &&
                                ryzenotebuffed.IsValidTarget(E.Range))
                            {
                                Q.Cast(ryzenotebuffed);
                                if (ryzenotebuffed.IsValidTarget(E.Range))
                                {
                                    Orbwalker.ForceTarget(ryzenotebuffed);
                                    E.CastOnUnit(ryzenotebuffed);
                                }
                                if (!E.IsReady() && Q.IsReady())
                                    Q.Cast(ryzenotebuffed);
                            }
                        }
                }
        } // LaneClear End

        private void REscape()
        {
            switch (R.Level)
            {
                case 1:
                    RangeR = 1750f;
                    break;
                case 2:
                    RangeR = 3000f;
                    break;
            }
            var NearByTurrets =
                ObjectManager.Get<Obj_AI_Turret>().Find(turret => (turret.Distance(Player) < RangeR) && turret.IsAlly);
            if (NearByTurrets != null)
                R.Cast(NearByTurrets.Position);
        }

        //RUsage

        private float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;
            if (Q.IsReady() || (Player.Mana <= Q.Instance.SData.Mana + E.Instance.SData.Mana))
                damage += QGetRealDamage(enemy);
            else if (Q.IsReady() || (Player.Mana <= Q.Instance.SData.Mana))
                damage += Q.GetDamage(enemy);

            if (W.IsReady() || (Player.Mana <= W.Instance.SData.Mana + W.Instance.SData.Mana))
                damage += W.GetDamage(enemy) + W.GetDamage(enemy);
            else if (W.IsReady() || (Player.Mana <= W.Instance.SData.Mana))
                damage += W.GetDamage(enemy);

            if (E.IsReady() || (Player.Mana <= E.Instance.SData.Mana + E.Instance.SData.Mana))
                damage += E.GetDamage(enemy) + E.GetDamage(enemy);
            else if (E.IsReady() || (Player.Mana <= E.Instance.SData.Mana))
                damage += E.GetDamage(enemy);

            /*if (igniteslot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(igniteslot) == SpellState.Ready)
            {
                if (Program.Config.Item("UseIgnite").GetValue<bool>())
                {
                    damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
                }
            }*/

            return damage;
        }

        public class Configuration
        {
            public Configuration(Menu root)
            {
                ComboMenu = MenuFactory.CreateMenu(root, "Combo");
                HarassMenu = MenuFactory.CreateMenu(root, "Harass");
                LaneClearMenu = MenuFactory.CreateMenu(root, "Lane Clear");
                JungleClearMenu = MenuFactory.CreateMenu(root, "Jungle Clear");
                LastHitMenu = MenuFactory.CreateMenu(root, "Last Hit");
                MiscMenu = MenuFactory.CreateMenu(root, "Misc");
                SkinsMenu = MenuFactory.CreateMenu(root, "Skins Menu");
                UltimateMenu = MenuFactory.CreateMenu(root, "Ultimate Menu");
                DrawingMenu = MenuFactory.CreateMenu(root, "Drawing");

                Combos(MenuItemFactory.Create(ComboMenu));
                Harass(MenuItemFactory.Create(HarassMenu));
                LaneClear(MenuItemFactory.Create(LaneClearMenu));
                JungleClear(MenuItemFactory.Create(JungleClearMenu));
                LastHit(MenuItemFactory.Create(LastHitMenu));
                Misc(MenuItemFactory.Create(MiscMenu));
                Skins(MenuItemFactory.Create(SkinsMenu));
                Ultimate(MenuItemFactory.Create(UltimateMenu));
                Drawings(MenuItemFactory.Create(DrawingMenu));
            }

            public Menu ComboMenu { get; }

            public Menu LastHitMenu { get; }

            public MenuItem DontAAInCombo { get; private set; }

            public MenuItem DrawComboDamage { get; private set; }

            public Menu DrawingMenu { get; }

            public MenuItem FillColor { get; private set; }

            public MenuItem HarassE { get; set; }

            public MenuItem HarassMana { get; set; }

            public Menu HarassMenu { get; }

            public MenuItem HarassQ { get; set; }

            public Menu LaneClearMenu { get; }

            public Menu MiscMenu { get; }

            public MenuItem drawQ { get; private set; }

            public MenuItem drawW { get; set; }

            public MenuItem drawE { get; set; }

            public MenuItem drawR { get; set; }

            public MenuItem laneclearE { get; private set; }

            public MenuItem laneclearQ { get; private set; }

            public MenuItem laneclearW { get; private set; }

            public MenuItem LaneClearMinions { get; private set; }

            public MenuItem LaneClearEMinMinions { get; private set; }

            public MenuItem laneclearMinimumMana { get; private set; }

            public MenuItem interruptQ { get; private set; }

            public MenuItem useQAntiGapCloser { get; private set; }

            public MenuItem UseQLH { get; private set; }

            public MenuItem UseELH { get; private set; }

            public MenuItem lanehitMinimumMana { get; private set; }

            public MenuItem UseELC { get; private set; }

            public MenuItem UseQLC { get; private set; }

            public MenuItem HarassW { get; private set; }

            public MenuItem HarassManaManager { get; private set; }

            public MenuItem DrawRMinimap { get; set; }

            public MenuItem drawWE { get; set; }

            public MenuItem KSQ { get; private set; }

            public MenuItem KSW { get; private set; }

            public MenuItem KSE { get; private set; }

            public MenuItem InterruptWithW { get; private set; }

            public MenuItem WGapCloser { get; private set; }

            public MenuItem ChaseWithR { get; private set; }

            public MenuItem EscapeWithR { get; private set; }

            public Menu UltimateMenu { get; }

            public MenuItem Combo2TimesMana { get; private set; }

            public MenuItem CBlockAA { get; private set; }

            public MenuItem ComboMode { get; private set; }

            public MenuItem UltimateUseR { get; private set; }
            public MenuItem StackTear { get; private set; }
            public MenuItem StackTearNF { get; private set; }
            public Menu SkinsMenu { get; }
            public MenuItem SkinID { get; private set; }
            public MenuItem UseSkin { get; private set; }
            public MenuItem EnableFarming { get; set; }
            public MenuItem ComboEUse { get; set; }
            public MenuItem ComboWUse { get; set; }
            public MenuItem ComboQUse { get; set; }
            public MenuItem ComboRUse { get; set; }
            public MenuItem EnableScrollToFarm { get; set; }
            public MenuItem UseQJC { get; private set; }
            public MenuItem UseWJC { get; private set; }
            public MenuItem UseEJC { get; private set; }
            public MenuItem jungleclearMinimumMana { get; private set; }
            public MenuItem DrawSpellFarm { get; private set; }
            public Menu JungleClearMenu { get; }

            private void Combos(MenuItemFactory factory)
            {
                ComboQUse = null;
                ComboWUse = null;
                ComboEUse = null;
                ComboQUse = factory.WithName("[Combo] Use Q").WithValue(true).Build();
                //Chat.Print("ComboQUse Set to: "+ComboQUse.ValueSet + " | " + ComboQUse.GetValue<bool>().ToString());
                ComboWUse = factory.WithName("[Combo] Use W").WithValue(true).WithValue(true).Build();
                //Chat.Print("ComboWUse Set to: " + ComboWUse.ValueSet + " | " + ComboWUse.GetValue<bool>().ToString());
                ComboEUse = factory.WithName("[Combo] Use E").WithValue(true).WithValue(true).Build();
                //Chat.Print("ComboEUse Set to: " + ComboEUse.ValueSet + " | " + ComboEUse.GetValue<bool>().ToString());
                CBlockAA = factory.WithName("Don't AA while doing Combo").WithValue(true).Build();
                ComboRUse = factory.WithName("Ultimate (R) in Ultimate Menu").Build();
                ComboMode =
                    factory.WithName("[Combo Mode]")
                        .WithValue(new StringList(new[] {"Burst", "Survivor Mode"}, 0))
                        .Build();
            }

            private void Skins(MenuItemFactory factory)
            {
                // Skins Menu
                SkinID = factory.WithName("Skin ID").WithValue(new Slider(6, 0, 10)).Build();
                UseSkin = factory.WithName("Enabled?").WithValue(true).Build();
                UseSkin.ValueChanged += (sender, eventArgs) =>
                {
                };
            }

            private void Drawings(MenuItemFactory factory)
            {
                // Drawing Menu
                drawQ = factory.WithName("Draw Q Range").WithValue(true).Build();
                drawWE = factory.WithName("Draw W/E Range").WithValue(true).Build();
                drawR = factory.WithName("Draw R Range").WithValue(false).Build();
                DrawRMinimap = factory.WithName("Draw R Range | On Minimap").WithValue(true).Build();
                DrawSpellFarm = factory.WithName("Draw Spell Farm State? [On/Off]").WithValue(true).Build();
                DrawComboDamage = factory
                    .WithName("Draw Combo Damage")
                    .WithValue(true)
                    .Build();

                FillColor = factory
                    .WithName("Fill Color")
                    .WithValue(new Circle(true, Color.FromArgb(204, 255, 0, 1)))
                    .Build();
            }

            private void Harass(MenuItemFactory factory)
            {
                HarassQ = factory.WithName("Use Q").WithValue(true).Build();
                HarassW = factory.WithName("Use W").WithValue(false).Build();
                HarassE = factory.WithName("Use E").WithValue(false).Build();
                HarassManaManager = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(30))
                    .WithTooltip("Minimum Mana that you need to have to AutoHarass with Q/E.")
                    .Build();
            }

            private void Ultimate(MenuItemFactory factory)
            {
                UltimateUseR = factory
                    .WithName("Use R Automatically (Beta)")
                    .WithValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press))
                    .WithTooltip("It'll Use the Ultimate if there's Ally turret nearby to teleport you to it")
                    .Build();
            }

            private void LaneClear(MenuItemFactory factory)
            {
                // LaneClear Menu
                EnableScrollToFarm =
                    factory.WithName("Enable Mouse Scroll Farm")
                        .WithValue(true)
                        .WithTooltip("Enable using mouse scroll to enable/disable spell usage in farming?")
                        .Build();
                EnableFarming =
                    factory.WithName("Enable Farming with Spells?")
                        .WithValue(true)
                        .WithTooltip("You either change the value here by clicking or by Scrolling Down using the mouse")
                        .WithPerma("Farming with Spells?").Build();

                UseQLC = factory.WithName("Use Q to LaneClear").WithValue(true).Build();
                UseELC = factory.WithName("Use E to LaneClear").WithValue(true).Build();

                laneclearMinimumMana = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(50))
                    .WithTooltip("Minimum Mana that you need to have to LaneClear with Q/E.")
                    .Build();
            }

            private void JungleClear(MenuItemFactory factory)
            {
                // JungleClear Menu
                UseQJC = factory.WithName("Use Q to JungleClear").WithValue(true).Build();
                UseWJC = factory.WithName("Use W to JungleClear").WithValue(true).Build();
                UseEJC = factory.WithName("Use E to JungleClear").WithValue(true).Build();

                jungleclearMinimumMana = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(30))
                    .WithTooltip("Minimum Mana that you need to have to JungleClear with Q/W/E.")
                    .Build();
            }

            private void LastHit(MenuItemFactory factory)
            {
                // LastHit Menu
                UseQLH = factory.WithName("Use Q to LaneHit").WithValue(true).Build();
                UseELH = factory.WithName("Use E to LaneHit").WithValue(true).Build();

                lanehitMinimumMana = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(30))
                    .WithTooltip("Minimum Mana that you need to have to LaneHit with Q/E.")
                    .Build();
            }

            private void Misc(MenuItemFactory factory)
            {
                // Misc Menu
                KSQ = factory.WithName("Use Q to KillSteal").WithValue(true).Build();
                KSW = factory.WithName("Use W to KillSteal").WithValue(true).Build();
                KSE = factory.WithName("Use E to KillSteal").WithValue(true).Build();
                StackTear = factory.WithName("Stack Tear/Manamune/Archangel in Fountain?").WithValue(true).Build();
                StackTearNF =
                    factory.WithName("Stack Tear/Manamune/Archangel if You've Blue Buff?").WithValue(true).Build();
                InterruptWithW = factory.WithName("Interrupt Spells W").WithValue(true).Build();
                WGapCloser = factory.WithName("Use W on Enemy GapCloser (Ex: Irelia's Q)").WithValue(true).Build();
                ChaseWithR = factory.WithName("Use R to Chase (Being Added)").Build();
                EscapeWithR = factory.WithName("Use R to Escape (Ultimate Menu)").Build();
            }
        }
    }
}