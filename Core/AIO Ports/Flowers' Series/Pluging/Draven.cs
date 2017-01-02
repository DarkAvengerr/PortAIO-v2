using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    

    internal class Draven : Logic
    {
        private static readonly List<AllAxe> AxeList = new List<AllAxe>();
        private static int CatchTime;

        private static int AxeCount => (Me.HasBuff("dravenspinning") ? 1 : 0)
                                      + (Me.HasBuff("dravenspinningleft") ? 1 : 0) + AxeList.Count;

        public Draven()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 950f);
            R = new Spell(SpellSlot.R, 3000f);

            E.SetSkillshot(0.25f, 100f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.4f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboWLogic", "Use W| If Target Not In Attack Range", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRSolo", "Use R|Solo Mode", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRTeam", "Use R|TeamFight", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearECount", "If E CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(false));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    killStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(),
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeKey", "Flee Key", true).SetValue(new KeyBind('Z', KeyBindType.Press)));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qSettings = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
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
                    qSettings.AddItem(new MenuItem("EnableControl", "Enable Cancel Catch Axe Key?", true).SetValue(false));
                    qSettings.AddItem(
                            new MenuItem("ControlKey", "Cancel Key", true).SetValue(new KeyBind('G', KeyBindType.Press)))
                        .ValueChanged += CatchTimeValueChange;
                    qSettings.AddItem(
                        new MenuItem("ControlKey2", "Or Right Click?", true).SetValue(true));
                    qSettings.AddItem(
                        new MenuItem("ControlKey3", "Or Mouse Scroll?", true).SetValue(false));
                }

                var wSettings = miscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wSettings.AddItem(
                        new MenuItem("WCatchAxe", "If Axe too Far Auto Use", true).SetValue(
                            new StringList(new[] {"Combo/Harass Mode", "Only Combo", "Off" })));
                    wSettings.AddItem(new MenuItem("AutoWSlow", "Auto W|If Player Have Slow Debuff", true).SetValue(true));
                }

                var eSettings = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eSettings.AddItem(new MenuItem("Interrupt", "Interrupt Spell", true).SetValue(true));
                    eSettings.AddItem(new MenuItem("Anti", "Anti Gapcloser", true).SetValue(false));
                    eSettings.AddItem(new MenuItem("AntiRengar", "Anti Rengar", true).SetValue(true));
                    eSettings.AddItem(new MenuItem("AntiKhazix", "Anti Khazix", true).SetValue(true));
                    eSettings.AddItem(new MenuItem("AntiMelee", "Anti Melee", true).SetValue(true));
                }

                var rSettings = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rSettings.AddItem(
                        new MenuItem("rMenuSemi", "Semi R Key", true).SetValue(
                            new KeyBind('T', KeyBindType.Press)));
                    rSettings.AddItem(
                        new MenuItem("rMenuMin", "Use R| Min Range >= x", true).SetValue(new Slider(1000, 500, 2500)));
                    rSettings.AddItem(
                        new MenuItem("rMenuMax", "Use R| Man Range <= x", true).SetValue(new Slider(3000, 1500, 3500)));
                }
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
                drawMenu.AddItem(
                    new MenuItem("DrawCatchAxe", "Draw Catch Axe Range", true).SetValue(new Circle(true,
                        Color.FromArgb(251, 0, 255))));
                drawMenu.AddItem(
                    new MenuItem("DrawAxe", "Draw Axe Position", true).SetValue(new Circle(true,
                        Color.FromArgb(45, 255, 0))));
                drawMenu.AddItem(
                    new MenuItem("DrawThinkness", "Draw Circle Thinkness", true).SetValue(new Slider(3, 1, 10)));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                //DamageIndicator.AddToMenu(drawMenu);
            }

            Game.OnWndProc += OnWndProc;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Orbwalking.BeforeAttack += BeforeAttack;
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Drawing.OnDraw += OnDraw;
        }

        private void OnWndProc(WndEventArgs Args)
        {
            if (Menu.GetBool("EnableControl"))
            {
                if (Menu.GetBool("ControlKey2") && (Args.Msg == 516 || Args.Msg == 517))
                {
                    if (Utils.TickCount - CatchTime > 1800)
                    {
                        CatchTime = Utils.TickCount;
                    }
                }

                if (Menu.GetBool("ControlKey3") && Args.Msg == 0x20a)
                {
                    if (Utils.TickCount - CatchTime > 1800)
                    {
                        CatchTime = Utils.TickCount;
                    }
                }
            }
        }

        private void CatchTimeValueChange(object obj, OnValueChangeEventArgs Args)
        {
            if (Menu.GetBool("EnableControl") && Args.GetNewValue<KeyBind>().Active)
            {
                if (Utils.TickCount - CatchTime > 1800)
                {
                    CatchTime = Utils.TickCount;
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.GetBool("AntiMelee") && E.IsReady())
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
            if (Menu.GetBool("Anti") && E.IsReady())
            {
                if (Args.End.Distance(Me.Position) <= 200 && Args.Sender.IsValidTarget(E.Range))
                {
                    E.Cast(Args.Sender);
                }
            }
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.GetBool("Interrupt") && E.IsReady())
            {
                if (Args.DangerLevel >= Interrupter2.DangerLevel.Medium && sender.IsValidTarget(E.Range))
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
                    if (Menu.GetBool("ComboQ") && Q.IsReady() && AxeCount < Menu.GetSlider("MaxAxeCount"))
                    {
                        var target = Args.Target as AIHeroClient;

                        if (target.Check())
                        {
                            Q.Cast();
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
                    {
                        if (Menu.GetBool("HarassQ") && Q.IsReady() && AxeCount < 2)
                        {
                            var target = Args.Target as AIHeroClient;

                            if (target.Check())
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

            if (Menu.GetKey("FleeKey"))
            {
                Flee();
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
                    FarmHarass();
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        private void AutoCatchLogic()
        {
            if ((Menu.GetList("CatchMode") == 2) || (Menu.GetList("CatchMode") == 1 && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo))
            {
                Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                return;
            }

            var catchRange = Menu.GetSlider("CatchRange");

            var bestAxe =
                AxeList.Where(x => x.Axe.IsValid && !x.Axe.IsDead && x.Axe.Position.DistanceToMouse() <= catchRange)
                    .OrderBy(x => x.AxeTime)
                    .ThenBy(x => x.Axe.Position.DistanceToPlayer())
                    .ThenBy(x => x.Axe.Position.DistanceToMouse())
                    .FirstOrDefault();

            if (bestAxe != null)
            {
                if (Menu.GetBool("UnderTurret") &&
                    ((Me.UnderTurret(true) && bestAxe.Axe.Position.UnderTurret(true)) || (bestAxe.Axe.Position.
                                                                                              UnderTurret(true) &&
                                                                                          !Me.UnderTurret(true))))
                {
                    return;
                }

                if (Menu.GetBool("CheckSafe") &&
                    (HeroManager.Enemies.Count(x => x.Distance(bestAxe.Axe.Position) < 350) > 3 ||
                     HeroManager.Enemies.Count(x => x.Distance(bestAxe.Axe.Position) < 350 && x.IsMelee) > 1))
                {
                    return;
                }

                if (((Menu.GetList("WCatchAxe") == 0 && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)) || 
                    (Menu.GetList("WCatchAxe") == 1 && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)) && W.IsReady() &&
                    (bestAxe.Axe.Position.DistanceToPlayer()/Me.MoveSpeed*1000 >= bestAxe.AxeTime - Utils.TickCount))
                {
                    W.Cast();
                }

                if (bestAxe.Axe.Position.DistanceToPlayer() > 100)
                {
                    if (Utils.TickCount - CatchTime > 1800)
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
            if (Menu.GetKey("rMenuSemi") && R.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (target.Check(R.Range))
                {
                    SpellManager.PredCast(R, target, true);
                }
            }
        }

        private void AutoUseLogic()
        {
            if (Menu.GetBool("AutoWSlow") && W.IsReady() && Me.HasBuffOfType(BuffType.Slow))
            {
                W.Cast();
            }
        }

        private void KillSteal()
        {
            if (Menu.GetBool("KillStealE") && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x)))
                {
                    SpellManager.PredCast(E, target);
                    return;
                }
            }

            if (Menu.GetBool("KillStealR") && R.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.IsValidTarget(R.Range) && x.DistanceToPlayer() > Menu.GetSlider("rMenuMin") &&
                            Menu.GetBool("KillStealR" + x.ChampionName.ToLower()) && x.Health < R.GetDamage(x)))
                {
                    SpellManager.PredCast(R, target, true);
                    return;
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target.Check(E.Range))
            {
                if (Menu.GetBool("ComboW") && W.IsReady() && !Me.HasBuff("dravenfurybuff"))
                {
                    if (Menu.GetBool("ComboWLogic"))
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

                if (Menu.GetBool("ComboE") && E.IsReady())
                {
                    if (!Orbwalking.InAutoAttackRange(target) ||
                        target.Health < (AxeCount > 0 ? Q.GetDamage(target)*3 : Me.GetAutoAttackDamage(target)*3) ||
                        Me.HealthPercent < 40)
                    {
                        SpellManager.PredCast(E, target);
                    }
                }

                if (Menu.GetBool("ComboR") && R.IsReady())
                {
                    if (Menu.GetBool("ComboRSolo"))
                    {
                        if ((target.Health <
                             R.GetDamage(target) +
                             (AxeCount > 0 ? Q.GetDamage(target)*2 : Me.GetAutoAttackDamage(target)*2) +
                             (E.IsReady() ? E.GetDamage(target) : 0)) &&
                            target.Health > (AxeCount > 0 ? Q.GetDamage(target)*3 : Me.GetAutoAttackDamage(target)*3) &&
                            (Me.CountEnemiesInRange(1000) == 1 ||
                             (Me.CountEnemiesInRange(1000) == 2 && Me.HealthPercent >= 60)))
                        {
                            SpellManager.PredCast(R, target, true);
                        }
                    }

                    if (Menu.GetBool("ComboRTeam"))
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
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (target.Check(E.Range))
                {
                    if (Menu.GetBool("HarassE") && E.IsReady())
                    {
                        E.CastIfWillHit(target, 2);
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
                if (Menu.GetBool("LaneClearQ") && Q.IsReady() && AxeCount < 2 && !Me.Spellbook.IsAutoAttacking)
                {
                    var minions = MinionManager.GetMinions(Me.Position, 600);

                    if (minions.Any() && minions.Count >= 2)
                    {
                        Q.Cast();
                    }
                }

                if (Menu.GetBool("LaneClearE") && E.IsReady())
                {
                    var minions = MinionManager.GetMinions(Me.Position, E.Range);

                    if (minions.Any())
                    {
                        var eFarm = E.GetLineFarmLocation(minions, E.Width);

                        if (eFarm.MinionsHit >= Menu.GetSlider("LaneClearECount"))
                        {
                            E.Cast(eFarm.Position);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.GetBool("JungleClearE") && E.IsReady())
                    {
                        E.Cast(mob, true);
                    }

                    if (Menu.GetBool("JungleClearW") && W.IsReady() && !Me.HasBuff("dravenfurybuff") &&
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

                    if (Menu.GetBool("JungleClearQ") && Q.IsReady() && AxeCount < 2 && 
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
            Orbwalking.MoveTo(Game.CursorPos);

            if (Menu.GetBool("FleeW") && W.IsReady())
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

                if (Menu.GetBool("AntiRengar") && Rengar != null)
                {
                    if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(Me.Position) < E.Range)
                    {
                        E.Cast(Rengar.Position, true);
                    }
                }

                if (Menu.GetBool("AntiKhazix") && Khazix != null)
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
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetCircle("DrawCatchAxe").Active)
                {
                    Render.Circle.DrawCircle(Game.CursorPos, Menu.GetSlider("CatchRange"),
                        Menu.GetCircle("DrawCatchAxe").Color, Menu.GetSlider("DrawThinkness"));
                }

                if (Menu.GetCircle("DrawAxe").Active)
                {
                    foreach (var Axe in AxeList.Where(x => !x.Axe.IsDead && x.Axe.IsValid))
                    {
                        Render.Circle.DrawCircle(Axe.Axe.Position, 120,
                            Menu.GetCircle("DrawAxe").Color, Menu.GetSlider("DrawThinkness"));
                    }
                }

                if (Menu.GetBool("DrawE") && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
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
