using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    using Orbwalking = ADCCOMMON.Orbwalking;

    internal class Kalista : Logic
    {
        private int lastWCast;
        private int lastECast;

        public Kalista()
        {
            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 5000f);
            E = new Spell(SpellSlot.E, 950f);
            R = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.35f, 40f, 2400f, true, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboEUse", "Use E| If Can Kill Minion And Slow Target", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboMana", "Save Mana to Cast E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboAttack", "Auto Attack Minion To Dash?", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassESlow", "Use E| If Can Kill Minion And Slow Target", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassELeave", "Use E| If Target Leave E Range", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassECount", "Use E| If Target E Count >= x", true).SetValue(new Slider(3, 1, 10)));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearQCount", "If Q CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearECount", "If E CanKill Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(new MenuItem("AutoELast", "Auto E LastHit?", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("EToler", "E Damage Tolerance + -", true).SetValue(new Slider(0, -100)));
                    eMenu.AddItem(new MenuItem("AutoSteal", "Auto Steal Mobs?", true).SetValue(true));
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("AutoSave", "Auto R Save Ally?", true).SetValue(true));
                    rMenu.AddItem(
                        new MenuItem("AutoSaveHp", "Auto R| When Ally Health Percent <= x%", true).SetValue(new Slider(20)));
                    rMenu.AddItem(new MenuItem("Balista", "Balista?", true).SetValue(true));
                }

                miscMenu.AddItem(new MenuItem("Forcus", "Forcus Attack Passive Target", true).SetValue(true));
            }

            var utilityMenu = Menu.AddSubMenu(new Menu("Utility", "Utility"));
            {
                var skinMenu = utilityMenu.AddSubMenu(new Menu("Skin Change", "Skin Change"));
                {
                    SkinManager.AddToMenu(skinMenu);
                }

                var autoLevelMenu = utilityMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }

                var humainzerMenu = utilityMenu.AddSubMenu(new Menu("Humanier", "Humanizer"));
                {
                    HumanizerManager.AddToMenu(humainzerMenu);
                }

                var itemsMenu = utilityMenu.AddSubMenu(new Menu("Items", "Items"));
                {
                    ItemsManager.AddToMenu(itemsMenu);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu, DamageCalculate.GetEDamage);
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

            if (!Menu.GetBool("AutoELast") || !E.IsReady())
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
            if (Menu.GetBool("Forcus"))
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
                            if (Menu.GetBool("ComboQ") && Q.IsReady())
                            {
                                if (Menu.GetBool("ComboMana"))
                                {
                                    if (Me.Mana > Q.ManaCost + E.ManaCost)
                                    {
                                        SpellManager.PredCast(Q, target);
                                    }
                                }
                                else
                                {
                                    SpellManager.PredCast(Q, target);
                                }
                            }
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
                    {
                        var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth);

                        if (mobs.Any())
                        {
                            var mob = mobs.FirstOrDefault();

                            if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                            {
                                Q.Cast(mob, true);
                            }
                        }
                    }
                }
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
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
                    FarmHarass();
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
                    if (Menu.GetBool("AutoSave") && Me.CountEnemiesInRange(R.Range) > 0 &&
                        ally.CountEnemiesInRange(R.Range) > 0 &&
                        ally.HealthPercent <= Menu.GetSlider("AutoSaveHp"))
                    {
                        R.Cast();
                    }

                    if (Menu.GetBool("Balista") && ally.ChampionName == "Blitzcrank")
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
            if (Menu.GetBool("AutoSteal") && E.IsReady() && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
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
            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    SpellManager.PredCast(Q, target);
                    return;
                }
            }

            if (Menu.GetBool("KillStealE") && E.IsReady())
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

            if (target.Check(Q.Range))
            {
                if (Menu.GetBool("ComboAttack") && target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me))
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

                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range) && !Orbwalking.InAutoAttackRange(target))
                {
                    if (Menu.GetBool("ComboMana"))
                    {
                        if (Me.Mana > Q.ManaCost + E.ManaCost)
                        {
                            SpellManager.PredCast(Q, target);
                        }
                    }
                    else
                    {
                        SpellManager.PredCast(Q, target);
                    }
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && Utils.TickCount - lastWCast > 2000)
                {
                    if (NavMesh.IsWallOfGrass(target.ServerPosition, 20) && !target.IsVisible)
                    {
                        if (Menu.GetBool("ComboMana"))
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

                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(E.Range) &&
                    target.HasBuff("kalistaexpungemarker") && Utils.TickCount - lastECast >= 500)
                {
                    if (target.Health < GetRealEDamage(target))
                    {
                        E.Cast();
                    }

                    if (Menu.GetBool("ComboEUse") && target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 100)
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
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                if (target.Check(Q.Range))
                {
                    if (Menu.GetBool("HarassQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        SpellManager.PredCast(Q, target);
                    }

                    if (Menu.GetBool("HarassE") && E.IsReady() && target.IsValidTarget(E.Range) && 
                        target.HasBuff("kalistaexpungemarker"))
                    {
                        var buffcount = target.GetBuffCount("kalistaexpungemarker");

                        if (Menu.GetBool("HarassELeave") && target.DistanceToPlayer() >= 800 && target.IsValidTarget(E.Range) &&
                            buffcount >= Menu.GetSlider("HarassECount"))
                        {
                            E.Cast();
                        }

                        if (Menu.GetBool("HarassESlow"))
                        {
                            var EKillMinion =
                                MinionManager
                                    .GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                                    .FirstOrDefault(x => x.HasBuff("kalistaexpungemarker") &&
                                                         x.DistanceToPlayer() <= E.Range && x.Health < GetRealEDamage(x));

                            if (EKillMinion != null && EKillMinion.DistanceToPlayer() <= E.Range &&
                                target.IsValidTarget(E.Range) &&
                                buffcount >= Menu.GetSlider("HarassECount"))
                            {
                                E.Cast();
                            }
                        }
                    }
                }
            }
        }

        private void FarmHarass()
        {
            if (ManaManager.SpellHarass)
            {
                Harass();
            }
        }

        private void LaneClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
            {
                var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                if (minions.Any())
                {
                    if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                    {
                        var QFarm = Q.GetLineFarmLocation(minions, Q.Width);

                        if (QFarm.MinionsHit >= Menu.GetSlider("LaneClearQCount"))
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }

                    if (Menu.GetBool("LaneClearE") && E.IsReady())
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
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.GetBool("JungleClearQ")  && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        Q.Cast(mob);
                    }

                    if (Menu.GetBool("JungleClearE") && E.IsReady())
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
                if (Menu.GetBool("DrawQ") && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.GetBool("DrawE") && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }

                if (Menu.GetBool("DrawR") && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(19, 130, 234), 1);
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

                if (target.BaseSkinName == "Moredkaiser")
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
