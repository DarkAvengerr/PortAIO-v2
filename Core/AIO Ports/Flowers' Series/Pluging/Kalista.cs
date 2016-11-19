using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Pluging
{
    using Common;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;
    using static Common.Common;

    internal class Kalista : Program
    {
        private int lastWCast;
        private int lastECast;
        private new readonly Menu Menu = Championmenu;

        public Kalista()
        {
            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 5000f);
            E = new Spell(SpellSlot.E, 950f);
            R = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.35f, 40f, 2400f, true, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboEUse", "Use E| If Can Kill Minion And Slow Target", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboMana", "Save Mana to Cast E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboAttack", "Auto Attack Minion To Dash?", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassESlow", "Use E| If Can Kill Minion And Slow Target", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassELeave", "Use E| If Target Leave E Range", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassECount", "Use E| If Target E Count >= x", true).SetValue(new Slider(3, 1, 10)));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearQCount", "If Q CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearECount", "If E CanKill Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var EMenu = MiscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    EMenu.AddItem(new MenuItem("AutoELast", "Auto E LastHit?", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("EToler", "E Damage Tolerance + -", true).SetValue(new Slider(0, -100)));
                    EMenu.AddItem(new MenuItem("AutoSteal", "Auto Steal Mobs?", true).SetValue(true));
                }

                var RMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    RMenu.AddItem(new MenuItem("AutoSave", "Auto R Save Ally?", true).SetValue(true));
                    RMenu.AddItem(
                        new MenuItem("AutoSaveHp", "Auto R| When Ally Health Percent <= x%", true).SetValue(new Slider(20)));
                    RMenu.AddItem(new MenuItem("Balista", "Balista?", true).SetValue(true));
                }

                MiscMenu.AddItem(new MenuItem("Forcus", "Forcus Attack Passive Target", true).SetValue(true));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(
                    new MenuItem("DrawDamage", "Draw Damage", true).SetValue(
                        new StringList(new[] {"Only E Damage", "Combo Damage", "Never"})));
            }

            Orbwalking.OnNonKillableMinion += OnNonKillableMinion;
            Orbwalking.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnNonKillableMinion(AttackableUnit sender)
        {
            if (Me.IsDead || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            if (!Menu.Item("AutoELast", true).GetValue<bool>() || !E.IsReady())
            {
                return;
            }

            var minion = (Obj_AI_Minion)sender;

            if (minion != null && minion.IsValidTarget(E.Range) && minion.Health < GetRealEDamage(minion) &&
                Me.CountEnemiesInRange(600) == 0 && Me.ManaPercent >= 60)
            {
                E.Cast();
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Menu.Item("Forcus", true).GetValue<bool>())
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                    Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie &&
                                                                         Orbwalking.InAutoAttackRange(x) &&
                                                                         x.HasBuff("kalistacoopstrikemarkally")))
                    {
                        Orbwalker.ForceTarget(enemy);
                    }
                }
                else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var all = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me),
                            MinionTypes.All, MinionTeam.NotAlly)
                        .Where(x => Orbwalking.InAutoAttackRange(x) && x.HasBuff("kalistacoopstrikemarkally"));

                    if (all.Any())
                    {
                        Orbwalker.ForceTarget(all.FirstOrDefault());
                    }
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Args.SData.Name.Contains("KalistaW"))
            {
                lastWCast = Utils.TickCount;
            }

            if (Args.SData.Name.Contains("KalistaExpunge") || Args.SData.Name.Contains("KalistaExpungeWrapper") ||
                Args.SData.Name.Contains("KalistaDummySpell"))
            {
                lastECast = Utils.TickCount;
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var target = Args.Target as AIHeroClient;

                    if (target != null)
                    {
                        if (!target.IsDead && !target.IsZombie)
                        {
                            if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                            {
                                if (Menu.Item("ComboMana", true).GetValue<bool>())
                                {
                                    if (Me.Mana > Q.ManaCost + E.ManaCost)
                                    {
                                        Q.CastTo(target);
                                    }
                                }
                                else
                                {
                                    Q.CastTo(target);
                                }
                            }
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (Me.ManaPercent < Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                        return;

                    var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var mob = mobs.FirstOrDefault();

                        if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            Q.Cast(mob, true);
                        }
                    }
                }
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            RLogic();
            AutoSteal();
            KillSteal();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        private void RLogic()
        {
            if (R.IsReady())
            {
                var ally =
                    HeroManager.Allies.FirstOrDefault(
                        x => !x.IsMe && !x.IsDead && !x.IsZombie && x.HasBuff("kalistacoopstrikeally"));

                if (ally != null && ally.IsVisible && ally.DistanceToPlayer() <= R.Range)
                {
                    if (Menu.Item("AutoSave", true).GetValue<bool>() && Me.CountEnemiesInRange(R.Range) > 0 &&
                        ally.CountEnemiesInRange(R.Range) > 0 &&
                        ally.HealthPercent <= Menu.Item("AutoSaveHp", true).GetValue<Slider>().Value)
                    {
                        R.Cast();
                    }

                    if (Menu.Item("Balista", true).GetValue<bool>() && ally.ChampionName == "Blitzcrank")
                    {
                        if (
                            HeroManager.Enemies.Any(
                                x => !x.IsDead && !x.IsZombie && x.IsValidTarget() && x.HasBuff("rocketgrab2")))
                        {
                            R.Cast();
                        }
                    }
                }
            }
        }

        private void AutoSteal()
        {
            if (Menu.Item("AutoSteal", true).GetValue<bool>() && E.IsReady() &&
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
            {
                var canSteal = MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth)
                    .Where(x => !x.Name.ToLower().Contains("mini"))
                    .Any(x => x.HasBuff("kalistaexpungemarker") && x.DistanceToPlayer() <= E.Range
                              && x.Health < GetRealEDamage(x) && x.DistanceToPlayer() > 600);

                if (canSteal)
                {
                    E.Cast();
                }
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    Q.CastTo(target);
                    return;
                }
            }

            if (Menu.Item("KillStealE", true).GetValue<bool>() && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && x.Health < GetRealEDamage(x)))
                {
                    E.Cast();
                    return;
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, Q.Range))
            {
                if (Menu.Item("ComboAttack", true).GetValue<bool>() &&
                    target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me))
                {
                    var minion =
                        MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me), MinionTypes.All,
                                MinionTeam.NotAlly)
                            .Where(Orbwalking.InAutoAttackRange)
                            .OrderBy(x => x.DistanceToPlayer())
                            .FirstOrDefault();

                    if (minion != null && !minion.IsDead)
                    {
                        Orbwalking.Orbwalk(minion, Game.CursorPos);
                    }
                }

                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    !Orbwalking.InAutoAttackRange(target))
                {
                    if (Menu.Item("ComboMana", true).GetValue<bool>())
                    {
                        if (Me.Mana > Q.ManaCost + E.ManaCost)
                        {
                            Q.CastTo(target);
                        }
                    }
                    else
                    {
                        Q.CastTo(target);
                    }
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && Utils.TickCount - lastWCast > 2000)
                {
                    if (NavMesh.IsWallOfGrass(target.ServerPosition, 20) && !target.IsVisible)
                    {
                        if (Menu.Item("ComboMana", true).GetValue<bool>())
                        {
                            if (Me.Mana > Q.ManaCost + E.ManaCost*2 + W.ManaCost + R.ManaCost)
                            {
                                W.Cast(target.ServerPosition);
                            }
                        }
                        else
                        {
                            W.Cast(target.ServerPosition);
                        }
                    }
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range) &&
                    target.HasBuff("kalistaexpungemarker") && Utils.TickCount - lastECast >= 500)
                {
                    if (target.Health < GetRealEDamage(target))
                    {
                        E.Cast();
                    }

                    if (Menu.Item("ComboEUse", true).GetValue<bool>() &&
                        target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 100)
                    {
                        var EKillMinion =
                            MinionManager
                                .GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                                .FirstOrDefault(x => x.HasBuff("kalistaexpungemarker") &&
                                                     x.DistanceToPlayer() <= E.Range && x.Health < GetRealEDamage(x));

                        if (EKillMinion != null && EKillMinion.DistanceToPlayer() <= E.Range &&
                            target.IsValidTarget(E.Range))
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }

        private void Harass()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, Q.Range))
                {
                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.CastTo(target);
                    }

                    if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range)
                        && target.HasBuff("kalistaexpungemarker"))
                    {
                        var buffcount = target.GetBuffCount("kalistaexpungemarker");

                        if (Menu.Item("HarassELeave", true).GetValue<bool>() && target.DistanceToPlayer() >= 800 &&
                            target.IsValidTarget(E.Range) &&
                            buffcount >= Menu.Item("HarassECount", true).GetValue<Slider>().Value)
                        {
                            E.Cast();
                        }

                        if (Menu.Item("HarassESlow", true).GetValue<bool>())
                        {
                            var EKillMinion =
                                MinionManager
                                    .GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                                    .FirstOrDefault(x => x.HasBuff("kalistaexpungemarker") &&
                                                         x.DistanceToPlayer() <= E.Range && x.Health < GetRealEDamage(x));

                            if (EKillMinion != null && EKillMinion.DistanceToPlayer() <= E.Range &&
                                target.IsValidTarget(E.Range) &&
                                buffcount >= Menu.Item("HarassECount", true).GetValue<Slider>().Value)
                            {
                                E.Cast();
                            }
                        }
                    }
                }
            }
        }

        private void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                if (minions.Any())
                {
                    if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        var QFarm = Q.GetLineFarmLocation(minions, Q.Width);

                        if (QFarm.MinionsHit >= Menu.Item("LaneClearQCount", true).GetValue<Slider>().Value)
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }

                    if (Menu.Item("LaneClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        var eMinionsCount =
                            MinionManager
                                .GetMinions(Me.Position, E.Range)
                                .Count(x => x.HasBuff("kalistaexpungemarker") && x.Health < GetRealEDamage(x));

                        if (eMinionsCount >= Menu.Item("LaneClearECount", true).GetValue<Slider>().Value)
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        Q.Cast(mob);
                    }

                    if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        if (mobs.Any(x => x.HasBuff("kalistaexpungemarker") && x.DistanceToPlayer() <= E.Range
                                          && x.Health < GetRealEDamage(x)))
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }

                if (Menu.Item("DrawR", true).GetValue<bool>() && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(19, 130, 234), 1);
                }

                if (Menu.Item("DrawDamage", true).GetValue<StringList>().SelectedIndex != 2)
                {
                    foreach (
                        var x in HeroManager.Enemies.Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg(
                            Menu.Item("DrawDamage", true).GetValue<StringList>().SelectedIndex == 1
                                ? (float) ComboDamage(x)
                                : (float) GetRealEDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
                }
            }
        }

        private double GetRealEDamage(Obj_AI_Base target)
        {
            if (target != null && !target.IsDead && !target.IsZombie && target.HasBuff("kalistaexpungemarker"))
            {
                if (target.HasBuff("KindredRNoDeathBuff"))
                {
                    return 0;
                }

                if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                {
                    return 0;
                }

                if (target.HasBuff("JudicatorIntervention"))
                {
                    return 0;
                }

                if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                {
                    return 0;
                }

                if (target.HasBuff("FioraW"))
                {
                    return 0;
                }

                if (target.HasBuff("ShroudofDarkness"))
                {
                    return 0;
                }

                if (target.HasBuff("SivirShield"))
                {
                    return 0;
                }

                var damage = 0d;

                damage += E.IsReady()
                    ? E.GetDamage(target)
                    : 0d + Menu.Item("EToler", true).GetValue<Slider>().Value - target.HPRegenRate;

                if (target.CharData.BaseSkinName == "Moredkaiser")
                {
                    damage -= target.Mana;
                }

                if (Me.HasBuff("SummonerExhaust"))
                {
                    damage = damage * 0.6f;
                }

                if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
                {
                    damage -= target.Mana / 2f;
                }

                if (target.HasBuff("GarenW"))
                {
                    damage = damage * 0.7f;
                }

                if (target.HasBuff("ferocioushowl"))
                {
                    damage = damage * 0.7f;
                }

                return damage;
            }

            return 0d;
        }
    }
}
