using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Pluging
{
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;
    using static Common.Common;

    internal class Draven : Program
    {
        private readonly Random random = new Random();
        private new readonly Menu Menu = Championmenu;
        private static readonly List<AllAxe> AxeList = new List<AllAxe>();

        public static int AxeCount => (Me.HasBuff("dravenspinning") ? 1 : 0)
                                      + (Me.HasBuff("dravenspinningleft") ? 1 : 0) + AxeList.Count;

        public Draven()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 950f);
            R = new Spell(SpellSlot.R, 3000f);

            E.SetSkillshot(0.25f, 100f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.4f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboWLogic", "Use W| If Target Not In Attack Range", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRSolo", "Use R|Solo Mode", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRTeam", "Use R|TeamFight", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearECount", "If E CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(false));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    KillStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(),
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                FleeMenu.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qSettings = MiscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qSettings.AddItem(
                        new MenuItem("CatchMode", "Catch Axe Mode: ", true).SetValue(
                            new StringList(new[] {"All Mode", "Only Combo", "Off"})));
                    qSettings.AddItem(
                        new MenuItem("CatchRange", "Max Catch Range(Mouse is Center)", true).SetValue(new Slider(600,
                            150, 1500)));
                    qSettings.AddItem(new MenuItem("UnderTurret", "Dont Cast In Under Turret", true).SetValue(true));
                    qSettings.AddItem(new MenuItem("CheckSafe", "Check Axe Position is Safe", true).SetValue(true));
                    qSettings.AddItem(new MenuItem("MaxAxeCount", "Max Axe Count <= x", true).SetValue(new Slider(2, 1, 3)));
                }

                var wSettings = MiscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wSettings.AddItem(
                        new MenuItem("WCatchAxe", "If Axe too Far Auto Use", true).SetValue(
                            new StringList(new[] {"Combo/Harass Mode", "Only Combo", "Off" })));
                    wSettings.AddItem(new MenuItem("AutoWSlow", "Auto W|If Player Have Slow Debuff", true).SetValue(true));
                }

                var eSettings = MiscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eSettings.AddItem(new MenuItem("Interrupt", "Interrupt Spell", true).SetValue(true));
                    eSettings.AddItem(new MenuItem("Anti", "Anti Gapcloser", true).SetValue(false));
                    eSettings.AddItem(new MenuItem("AntiRengar", "Anti Rengar", true).SetValue(true));
                    eSettings.AddItem(new MenuItem("AntiKhazix", "Anti Khazix", true).SetValue(true));
                    eSettings.AddItem(new MenuItem("AntiMelee", "Anti Melee", true).SetValue(true));
                }

                var rSettings = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rSettings.AddItem(
                        new MenuItem("RMenuSemi", "Semi R Key", true).SetValue(
                            new KeyBind('T', KeyBindType.Press)));
                    rSettings.AddItem(
                        new MenuItem("RMenuMin", "Use R| Min Range >= x", true).SetValue(new Slider(1000, 500, 2500)));
                    rSettings.AddItem(
                        new MenuItem("RMenuMax", "Use R| Man Range <= x", true).SetValue(new Slider(3000, 1500, 3500)));
                }
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(
                    new MenuItem("DrawCatchAxe", "Draw Catch Axe Range", true).SetValue(new Circle(true,
                        Color.FromArgb(251, 0, 255))));
                DrawMenu.AddItem(
                    new MenuItem("DrawAxe", "Draw Axe Position", true).SetValue(new Circle(true,
                        Color.FromArgb(45, 255, 0))));
                DrawMenu.AddItem(
                    new MenuItem("DrawThinkness", "Draw Circle Thinkness", true).SetValue(new Slider(3, 1, 10)));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Orbwalking.BeforeAttack += BeforeAttack;
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Drawing.OnDraw += OnDraw;
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.Item("AntiMelee", true).GetValue<bool>() && E.IsReady())
            {
                if (sender != null && sender.IsEnemy && Args.Target != null && Args.Target.IsMe)
                {
                    if (sender.Type == Me.Type && sender.IsMelee)
                    {
                        E.Cast(sender.Position);
                    }
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (Menu.Item("Anti", true).GetValue<bool>() && E.IsReady())
            {
                if (Args.End.Distance(Me.Position) <= 200 && Args.Sender.IsValidTarget(E.Range))
                {
                    E.Cast(Args.Sender);
                }
            }
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.Item("Interrupt").GetValue<bool>() && E.IsReady())
            {
                if (Args.DangerLevel >= Interrupter2.DangerLevel.High && sender.IsValidTarget(E.Range))
                {
                    E.Cast(sender);
                }
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() &&
                        AxeCount < Menu.Item("MaxAxeCount", true).GetValue<Slider>().Value)
                    {
                        var target = Args.Target as AIHeroClient;

                        if (CheckTarget(target))
                        {
                            Q.Cast();
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (Me.UnderTurret(true))
                    {
                        return;
                    }

                    if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
                    {
                        if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() && AxeCount < 2)
                        {
                            var target = Args.Target as AIHeroClient;

                            if (CheckTarget(target))
                            {
                                Q.Cast();
                            }
                        }
                    }
                    break;
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            AxeList.RemoveAll(x => x.Axe.IsDead || !x.Axe.IsValid);

            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            AutoCatchLogic();
            SemiRLogic();
            AutoUseLogic();
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
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
                    break;
            }
        }

        private void AutoCatchLogic()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee)
            {
                return;
            }

            if ((Menu.Item("CatchMode", true).GetValue<StringList>().SelectedIndex == 2) ||
                (Menu.Item("CatchMode", true).GetValue<StringList>().SelectedIndex == 1 &&
                 Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo))
            {
                return;
            }

            var catchRange = Menu.Item("CatchRange", true).GetValue<Slider>().Value;

            var bestAxe =
                AxeList.Where(x => x.Axe.IsValid && !x.Axe.IsDead && x.Axe.Position.DistanceToMouse() <= catchRange)
                    .OrderBy(x => x.AxeTime)
                    .ThenBy(x => x.Axe.Position.DistanceToPlayer())
                    .ThenBy(x => x.Axe.Position.DistanceToMouse())
                    .FirstOrDefault();

            if (bestAxe != null)
            {
                if (Menu.Item("UnderTurret", true).GetValue<bool>() &&
                    ((Me.UnderTurret(true) && bestAxe.Axe.Position.UnderTurret(true)) || (bestAxe.Axe.Position.
                                                                                              UnderTurret(true) &&
                                                                                          !Me.UnderTurret(true))))
                {
                    return;
                }

                if (Menu.Item("CheckSafe", true).GetValue<bool>() &&
                    (HeroManager.Enemies.Count(x => x.Distance(bestAxe.Axe.Position) < 350) > 3 ||
                     HeroManager.Enemies.Count(x => x.Distance(bestAxe.Axe.Position) < 350 && x.IsMelee) > 1))
                {
                    return;
                }

                if (((Menu.Item("WCatchAxe", true).GetValue<StringList>().SelectedIndex == 0 &&
                      (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                       Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)) ||
                     (Menu.Item("WCatchAxe", true).GetValue<StringList>().SelectedIndex == 1 &&
                      Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)) && W.IsReady() &&
                    (bestAxe.Axe.Position.DistanceToPlayer()/Me.MoveSpeed*1000 >= bestAxe.AxeTime - Utils.TickCount))
                {
                    W.Cast();
                }

                if (bestAxe.Axe.Position.DistanceToPlayer() > 100)
                {
                    if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
                    {
                        Orbwalker.SetOrbwalkingPoint(bestAxe.Axe.Position);
                    }
                    else
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, bestAxe.Axe.Position);
                    }
                }
                else
                {
                    if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
                    {
                        Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                    }
                }
            }
            else
            {
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
                {
                    Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                }
            }
        }

        private void SemiRLogic()
        {
            if (Menu.Item("RMenuSemi", true).GetValue<KeyBind>().Active && R.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, R.Range))
                {
                    R.CastTo(target);
                }
            }
        }

        private void AutoUseLogic()
        {
            if (Menu.Item("AutoWSlow", true).GetValue<bool>() && W.IsReady() && Me.HasBuffOfType(BuffType.Slow))
            {
                W.Cast();
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealE", true).GetValue<bool>() && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x)))
                {
                    E.CastTo(target);
                    return;
                }
            }

            if (Menu.Item("KillStealR", true).GetValue<bool>() && R.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.IsValidTarget(R.Range) &&
                            x.DistanceToPlayer() > Menu.Item("RMenuMin", true).GetValue<Slider>().Value &&
                            Menu.Item("KillStealR" + x.ChampionName.ToLower(), true).GetValue<bool>() &&
                            x.Health < R.GetDamage(x)))
                {
                    R.CastTo(target);
                    return;
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, E.Range))
            {
                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && !Me.HasBuff("dravenfurybuff"))
                {
                    if (Menu.Item("ComboWLogic", true).GetValue<bool>())
                    {
                        if (target.DistanceToPlayer() >= 600)
                        {
                            W.Cast();
                        }
                        else
                        {
                            if (target.Health <
                                (AxeCount > 0 ? Q.GetDamage(target)*5 : Me.GetAutoAttackDamage(target)*5))
                            {
                                W.Cast();
                            }
                        }
                    }
                    else
                    {
                        W.Cast();
                    }
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
                {
                    if (!Orbwalker.InAutoAttackRange(target) ||
                        target.Health < (AxeCount > 0 ? Q.GetDamage(target)*3 : Me.GetAutoAttackDamage(target)*3) ||
                        Me.HealthPercent < 40)
                    {
                        E.CastTo(target);
                    }
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
                {
                    if (Menu.Item("ComboRSolo", true).GetValue<bool>())
                    {
                        if ((target.Health <
                             R.GetDamage(target) +
                             (AxeCount > 0 ? Q.GetDamage(target)*2 : Me.GetAutoAttackDamage(target)*2) +
                             (E.IsReady() ? E.GetDamage(target) : 0)) &&
                            target.Health > (AxeCount > 0 ? Q.GetDamage(target)*3 : Me.GetAutoAttackDamage(target)*3) &&
                            (Me.CountEnemiesInRange(1000) == 1 ||
                             (Me.CountEnemiesInRange(1000) == 2 && Me.HealthPercent >= 60)))
                        {
                            var rPred = R.GetPrediction(target);

                            if (rPred.Hitchance >= HitChance.VeryHigh)
                            {
                                R.Cast(rPred.CastPosition);
                            }
                        }
                    }

                    if (Menu.Item("ComboRTeam", true).GetValue<bool>())
                    {
                        if (Me.CountAlliesInRange(1000) <= 3 && Me.CountEnemiesInRange(1000) <= 3)
                        {
                            var rPred = R.GetPrediction(target);

                            if (rPred.AoeTargetsHitCount >= 3)
                            {
                                R.Cast(rPred.CastPosition);
                            }
                            else if (rPred.AoeTargetsHitCount >= 2)
                            {
                                R.Cast(rPred.CastPosition);
                            }
                        }
                        else if (Me.CountAlliesInRange(1000) <= 2 && Me.CountEnemiesInRange(1000) <= 4)
                        {
                            var rPred = R.GetPrediction(target);

                            if (rPred.AoeTargetsHitCount >= 3)
                            {
                                R.Cast(rPred.CastPosition);
                            }
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
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target))
                {
                    if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady())
                    {
                        E.CastIfWillHit(target, 2);
                    }
                }
            }
        }

        private void LaneClear()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady() && AxeCount < 2 &&
                    !Me.Spellbook.IsAutoAttacking)
                {
                    var minions = MinionManager.GetMinions(Me.Position, 600);

                    if (minions.Any() && minions.Count >= 2)
                    {
                        Q.Cast();
                    }
                }

                if (Menu.Item("LaneClearE", true).GetValue<bool>() && E.IsReady())
                {
                    var minions = MinionManager.GetMinions(Me.Position, E.Range);

                    if (minions.Any())
                    {
                        var eFarm = E.GetLineFarmLocation(minions, E.Width);

                        if (eFarm.MinionsHit >= Menu.Item("LaneClearECount", true).GetValue<Slider>().Value)
                        {
                            E.Cast(eFarm.Position);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        E.Cast(mob, true);
                    }

                    if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady() && !Me.HasBuff("dravenfurybuff") &&
                        AxeCount > 0)
                    {
                        foreach (
                            var m in
                            mobs.Where(
                                x =>
                                    x.DistanceToPlayer() <= 600 && !x.Name.ToLower().Contains("mini") &&
                                    !x.Name.ToLower().Contains("crab") && x.MaxHealth > 1500 &&
                                    x.Health > Me.GetAutoAttackDamage(x)*2))
                        {
                            W.Cast();
                        }
                    }

                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && AxeCount < 2 && 
                        !Me.Spellbook.IsAutoAttacking)
                    {
                        var qmobs = MinionManager.GetMinions(600f, MinionTypes.All, MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth);

                        if (qmobs.Any())
                        {
                            if (qmobs.Count >= 2)
                            {
                                Q.Cast();
                            }

                            var qmob = qmobs.FirstOrDefault();
                            if (qmob != null && qmobs.Count == 1 && qmob.Health > Me.GetAutoAttackDamage(qmob) * 5)
                            {
                                Q.Cast();
                            }
                        }
                    }
                }
            }
        }

        private void Flee()
        {
            if (Menu.Item("FleeW", true).GetValue<bool>() && W.IsReady())
            {
                W.Cast();
            }
        }

        private void OnCreate(GameObject sender, EventArgs Args)
        {
            if (sender.Name.Contains("Draven_Base_Q_reticle_self.troy"))
            {
                AxeList.Add(new AllAxe(sender, Utils.TickCount + 1800));
            }

            if (E.IsReady())
            {
                var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));
                var Khazix = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Khazix"));

                if (Menu.Item("AntiRengar", true).GetValue<bool>() && Rengar != null)
                {
                    if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(Me.Position) < E.Range)
                    {
                        E.Cast(Rengar.Position, true);
                    }
                }

                if (Menu.Item("AntiKhazix", true).GetValue<bool>() && Khazix != null)
                {
                    if (sender.Name == "Khazix_Base_E_Tar.troy" && sender.Position.Distance(Me.Position) <= 300)
                    {
                        E.Cast(Khazix.Position, true);
                    }
                }
            }
        }

        private void OnDelete(GameObject sender, EventArgs Args)
        {
            if (sender.Name.Contains("Draven_Base_Q_reticle_self.troy"))
            {
                AxeList.RemoveAll(x => x.Axe.NetworkId == sender.NetworkId);
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
                if (Menu.Item("DrawCatchAxe", true).GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(Game.CursorPos, Menu.Item("CatchRange", true).GetValue<Slider>().Value,
                        Menu.Item("DrawCatchAxe", true).GetValue<Circle>().Color,
                        Menu.Item("DrawThinkness", true).GetValue<Slider>().Value);
                }

                if (Menu.Item("DrawAxe", true).GetValue<Circle>().Active)
                {
                    foreach (var Axe in AxeList.Where(x => !x.Axe.IsDead && x.Axe.IsValid))
                    {
                        Render.Circle.DrawCircle(Axe.Axe.Position, 120,
                            Menu.Item("DrawAxe", true).GetValue<Circle>().Color,
                            Menu.Item("DrawThinkness", true).GetValue<Slider>().Value);
                    }
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }

                if (Menu.Item("DrawDamage", true).GetValue<bool>())
                {
                    foreach (
                        var x in HeroManager.Enemies.Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg((float)ComboDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
                }
            }
        }

        internal class AllAxe
        {
            public int AxeTime;
            public GameObject Axe;

            public AllAxe(GameObject axe, int time)
            {
                Axe = axe;
                AxeTime = time;
            }
        }
    }
}
