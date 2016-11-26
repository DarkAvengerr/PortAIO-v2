using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Riven
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System;
    using System.Linq;
    using Color = System.Drawing.Color;

    internal class Program
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static SpellSlot Ignite = SpellSlot.Unknown;
        public static SpellSlot Flash = SpellSlot.Unknown;
        public static Menu Menu;
        public static AIHeroClient Me;
        public static bool CanQ;
        public static bool CanFlash;
        public static Vector3 FleePosition = Vector3.Zero;
        public static Vector3 TargetPosition = Vector3.Zero;
        public static int QStack;
        public static Orbwalking.Orbwalker Orbwalker;
        public static AttackableUnit QTarget;
        public static AIHeroClient BurstTarget;
        public static int SkinID;
        public static HpBarDraw DrawHpBar = new HpBarDraw();

        public static void Main()
        {
            OnLoad();
        }

        private static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Riven")
            {
                return;
            }

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W, 270f);
            E = new Spell(SpellSlot.E, 312f);
            R = new Spell(SpellSlot.R, 900f) { MinHitChance = HitChance.High };
            R.SetSkillshot(0.25f, 45f, 1600f, false, SkillshotType.SkillshotCone);

            Ignite = Me.GetSpellSlot("SummonerDot");
            Flash = Me.GetSpellSlot("SummonerFlash");

            SkinID = Me.SkinId;

            Menu = new Menu("Flowers' Riven", "Flowers' Riven", true);

            Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalker.Menu"));
            {
                Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker.Menu"));
            }

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("R1Combo", "Use R1", true).SetValue(new KeyBind('L', KeyBindType.Toggle, true)));
                ComboMenu.AddItem(
                    new MenuItem("R2Mode", "Use R2 Mode: ", true).SetValue(
                        new StringList(new[] { "Killable", "Max Damage", "First Cast", "Off" }, 1)));
                ComboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("Brust Setting", "Brust Setting"));
                ComboMenu.AddItem(new MenuItem("BurstFlash", "Use Flash", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("BurstIgnite", "Use Ignite", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(new MenuItem("Q Setting", "Q Setting"));
                MiscMenu.AddItem(new MenuItem("Q1Delay", "Q1 Delay: ", true).SetValue(new Slider(262, 200, 300)));
                MiscMenu.AddItem(new MenuItem("Q2Delay", "Q2 Delay: ", true).SetValue(new Slider(262, 200, 300)));
                MiscMenu.AddItem(new MenuItem("Q3Delay", "Q3 Delay: ", true).SetValue(new Slider(362, 300, 400)));
                MiscMenu.AddItem(new MenuItem("KeepQALive", "Keep Q alive", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("Dance", "Dance Emote in QA", true).SetValue(false));
                MiscMenu.AddItem(new MenuItem("W Setting", "W Setting"));
                MiscMenu.AddItem(new MenuItem("AntiGapCloserW", "AntiGapCloser", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("InterruptTargetW", "Interrupt Danger Spell", true).SetValue(true));
            }

            var EvadeMenu = Menu.AddSubMenu(new Menu("Evade", "Evade"));
            {
                Evade.Program.InjectEvade();
            }

            var SkinMenu = Menu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
            {
                SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged += EnbaleSkin;
                SkinMenu.AddItem(
                    new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(
                        new StringList(new[]
                        {
                            "Classic", "Redeemed Riven", "Crimson Elite Riven", "Battle Bunny Riven",
                            "Championship Riven", "Dragonblade Riven", "Arcade Riven"
                        })));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Draw", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("drawingW", "W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("BrustMinRange", "Burst Min Range", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("BrustMaxRange", "Burst Max Range", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("QuickHarassRange", "Quick Harass Range", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw Combo Damage", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("ShowR1", "Show R1 Status", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("ShowBurst", "Show Burst Status", true).SetValue(true));
            }

            Menu.AddItem(new MenuItem("Credit", "Credit : NightMoon"));

            Menu.AddToMainMenu();

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("AntiGapCloserW", true).GetValue<bool>() && W.IsReady())
            {
                if (gapcloser.Sender.IsValidTarget(W.Range) && Me.CountEnemiesInRange(1500) < 3)
                {
                    W.Cast();
                }
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.Item("InterruptTargetW", true).GetValue<bool>() && W.IsReady())
            {
                if (sender.IsValidTarget(W.Range) && !sender.ServerPosition.UnderTurret(true))
                {
                    W.Cast();
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (Args.SData.Name)
            {
                case "ItemTiamatCleave":
                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() &&
                                Me.CountEnemiesInRange(W.Range - 50) > 0)
                            {
                                W.Cast();
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            if (Me.CountEnemiesInRange(W.Range - 50) > 0)
                            {
                                W.Cast();
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Mixed:
                            if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady() &&
                                Me.CountEnemiesInRange(W.Range - 50) > 0)
                            {
                                W.Cast();
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.QuickHarass:
                            if (Me.CountEnemiesInRange(W.Range - 50) > 0)
                            {
                                W.Cast();
                            }
                            break;
                    }
                    break;
                case "RivenTriCleave":
                    CanQ = false;

                    if (Me.CountEnemiesInRange(400) == 0)
                    {
                        return;
                    }

                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            CastItem(true);

                            if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
                            {
                                var target = TargetSelector.GetSelectedTarget() ??
                                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                                if (target.IsValidTarget(R.Range))
                                {
                                    R2Logic(target);
                                }
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            CastItem(true);

                            if (R.IsReady())
                            {
                                if (BurstTarget != null && BurstTarget.IsValidTarget(R.Range))
                                {
                                    var rPred = R.GetPrediction(BurstTarget);

                                    if (rPred.Hitchance >= HitChance.High)
                                    {
                                        R.Cast(rPred.CastPosition);
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case "RivenTriCleaveBuffer":
                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
                            {
                                var target = TargetSelector.GetSelectedTarget() ??
                                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                                if (target.IsValidTarget(R.Range))
                                {
                                    R2Logic(target);
                                }
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            if (R.IsReady())
                            {
                                if (BurstTarget != null && BurstTarget.IsValidTarget(R.Range))
                                {
                                    var rPred = R.GetPrediction(BurstTarget);

                                    if (rPred.Hitchance >= HitChance.High)
                                    {
                                        R.Cast(rPred.CastPosition);
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case "RivenMartyr":
                    if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                        Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst) &&
                        Me.CountEnemiesInRange(400) > 0)
                    {
                        CastItem(true);
                    }
                    break;
                case "RivenFeint":
                    if (!R.IsReady())
                    {
                        return;
                    }

                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            var target = TargetSelector.GetSelectedTarget() ??
                                         TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                            if (target.IsValidTarget(R.Range))
                            {
                                switch (R.Instance.Name)
                                {
                                    case "RivenFengShuiEngine":
                                        if (Menu.Item("R1Combo", true).GetValue<KeyBind>().Active)
                                        {
                                            if (target.Distance(Me.ServerPosition) <
                                                E.Range + Me.AttackRange &&
                                                Me.CountEnemiesInRange(500) >= 1 &&
                                                !target.IsDead)
                                            {
                                                R.Cast();
                                            }
                                        }
                                        break;
                                    case "RivenIzunaBlade":
                                        R2Logic(target);
                                        break;
                                }
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            if (BurstTarget != null && BurstTarget.IsValidTarget(R.Range))
                            {
                                switch (R.Instance.Name)
                                {
                                    case "RivenFengShuiEngine":
                                        R.Cast();
                                        break;
                                    case "RivenIzunaBlade":
                                        var rPred = R.GetPrediction(BurstTarget);

                                        if (rPred.Hitchance >= HitChance.High)
                                        {
                                            R.Cast(rPred.CastPosition);
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                    break;
                case "RivenIzunaBlade":
                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            var target = TargetSelector.GetSelectedTarget() ??
                                         TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                            if (target != null && target.IsValidTarget())
                            {
                                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                                {
                                    CastQ(target);
                                }
                                else if (W.IsReady() && target.IsValidTarget(W.Range))
                                {
                                    W.Cast();
                                }
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            if (BurstTarget != null && BurstTarget.IsValidTarget())
                            {
                                if (BurstTarget.IsValidTarget(Q.Range))
                                {
                                    CastQ(BurstTarget);
                                }
                                else if (BurstTarget.IsValidTarget(W.Range) && W.IsReady())
                                {
                                    W.Cast();
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None ||
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.WallJump ||
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee)
            {
                return;
            }

            if (Args.Animation.Contains("Spell1a"))
            {
                QStack = 1;
                ResetQA(Menu.Item("Q1Delay", true).GetValue<Slider>().Value);
            }
            else if (Args.Animation.Contains("Spell1b"))
            {
                QStack = 2;
                ResetQA(Menu.Item("Q2Delay", true).GetValue<Slider>().Value);
            }
            else if (Args.Animation.Contains("Spell1c"))
            {
                QStack = 0;
                ResetQA(Menu.Item("Q3Delay", true).GetValue<Slider>().Value);
            }
        }

        private static void ResetQA(int time)
        {
            if (Menu.Item("Dance", true).GetValue<bool>())
            {
                EloBuddy.Player.DoEmote(Emote.Dance);
            }
            LeagueSharp.Common.Utility.DelayAction.Add(time, () =>
            {
                EloBuddy.Player.DoEmote(Emote.Dance);
                Orbwalking.ResetAutoAttackTimer();
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Me.Position.Extend(Game.CursorPos, +10));
            });
        }

        private static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                //ObjectManager.//Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            if (Args.Target == null)
            {
                return;
            }

            QTarget = (Obj_AI_Base)Args.Target;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = Args.Target as AIHeroClient;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    CastItem(true, true);

                    if (Q.IsReady())
                    {
                        CastQ(target);
                    }
                    else if (W.IsReady() && target.IsValidTarget(W.Range) && !target.HasBuffOfType(BuffType.SpellShield) &&
                             (target.IsMelee || target.IsFacing(Me) || !Q.IsReady() || Me.HasBuff("RivenFeint") ||
                              QStack != 0))
                    {
                        W.Cast();
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst)
            {
                var target = Args.Target as AIHeroClient;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    CastItem(true, true);

                    if (Q.IsReady())
                    {
                        CastQ(target);
                    }
                    else if (W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.QuickHarass)
            {
                var target = Args.Target as AIHeroClient;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    CastItem(true);

                    if (Q.IsReady() && QStack != 2 &&
                        Menu.Item("HarassQ", true).GetValue<bool>())
                    {
                        CastQ(target);
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                var target = Args.Target as AIHeroClient;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    CastItem(true);

                    if (Q.IsReady() && Menu.Item("HarassQ", true).GetValue<bool>())
                    {
                        CastQ(target);
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear(Args);
                JungleClear(Args);
            }
        }

        private static void LaneClear(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady())
            {
                if (Args.Target.Type == GameObjectType.obj_AI_Turret || Args.Target.Type == GameObjectType.obj_Turret)
                {
                    if (Q.IsReady() && !Args.Target.IsDead)
                    {
                        CastQ((Obj_AI_Base)Args.Target);
                    }
                }
                else
                {
                    var minion = Args.Target as Obj_AI_Minion;
                    var minions = MinionManager.GetMinions(E.Range + Me.AttackRange);

                    if (minion != null)
                    {
                        CastItem(true);

                        if (minions.Count >= 2)
                        {
                            CastQ(minion);
                        }
                    }
                }
            }
        }

        private static void JungleClear(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.Target is Obj_AI_Minion)
            {
                var mobs = MinionManager.GetMinions(E.Range + Me.AttackRange, MinionTypes.All,
                    MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                var mob = mobs.FirstOrDefault();

                if (mob != null)
                {
                    CastItem(true);

                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        CastQ(mob);
                    }
                    else if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady() &&
                             mob.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                    else if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        if (mob.HasBuffOfType(BuffType.Stun) && !W.IsReady())
                        {
                            E.Cast(mob.Position);
                        }
                        else if (!mob.HasBuffOfType(BuffType.Stun))
                        {
                            E.Cast(mob.Position);
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            Autobool();
            KeelQLogic();
            KillStealLogic();

            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Burst:
                    Brust();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.QuickHarass:
                    QuickHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    FleeLogic();
                    break;
                case Orbwalking.OrbwalkingMode.WallJump:
                    WallJump();
                    break;
            }
        }

        private static void Autobool()
        {
            if (QTarget != null)
            {
                if (CanQ)
                {
                    Q.Cast(((Obj_AI_Base)QTarget).Position);
                }
            }
        }

        private static void KeelQLogic()
        {
            if (Menu.Item("KeepQALive", true).GetValue<bool>() && !Me.UnderTurret(true) &&
                !Me.IsRecalling() && Me.HasBuff("RivenTriCleave"))
            {
                if (Me.GetBuff("RivenTriCleave").EndTime - Game.Time < 0.3)
                {
                    Q.Cast(Game.CursorPos);
                }
            }
        }

        private static void KillStealLogic()
        {
            foreach (var e in HeroManager.Enemies.Where(e => !e.IsZombie && !e.HasBuff("KindredrNoDeathBuff") &&
                                                             !e.HasBuff("Undying Rage") &&
                                                             !e.HasBuff("JudicatorIntervention") && e.IsValidTarget()))
            {
                if (W.IsReady() && Menu.Item("KillStealW", true).GetValue<bool>())
                {
                    if (e.IsValidTarget(W.Range) &&
                        Me.GetSpellDamage(e, SpellSlot.W) > e.Health + e.HPRegenRate)
                    {
                        W.Cast();
                    }
                }

                if (R.IsReady() && Menu.Item("KillStealR", true).GetValue<bool>())
                {
                    if (Me.HasBuff("RivenWindScarReady"))
                    {
                        if (E.IsReady() && Menu.Item("KillStealE", true).GetValue<bool>())
                        {
                            if (Me.ServerPosition.CountEnemiesInRange(R.Range + E.Range) < 3 &&
                                Me.HealthPercent > 50)
                            {
                                if (Me.GetSpellDamage(e, SpellSlot.R) > e.Health + e.HPRegenRate &&
                                    e.IsValidTarget(R.Range + E.Range - 100))
                                {
                                    if (E.IsReady())
                                    {
                                        E.Cast(e.Position);
                                    }
                                    else if (!E.IsReady())
                                    {
                                        R.CastIfHitchanceEquals(e, HitChance.High, true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Me.GetSpellDamage(e, SpellSlot.R) > e.Health + e.HPRegenRate &&
                                e.IsValidTarget(R.Range - 50))
                            {
                                R.CastIfHitchanceEquals(e, HitChance.High, true);
                            }
                        }
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget())
            {
                if (Menu.Item("ComboIgnite", true).GetValue<bool>() && Ignite != SpellSlot.Unknown && 
                    Ignite.IsReady() && target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() &&
                    target.IsValidTarget(W.Range) && !target.HasBuffOfType(BuffType.SpellShield) && 
                    (target.IsMelee || target.IsFacing(Me) || !Q.IsReady() || Me.HasBuff("RivenFeint") || QStack != 0))
                {
                    W.Cast();
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
                {
                    if (target.DistanceToPlayer() <= W.Range + E.Range &&
                        target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 100)
                    {
                        E.Cast(target.IsMelee ? Game.CursorPos : target.ServerPosition);
                    }
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
                {
                    switch (R.Instance.Name)
                    {
                        case "RivenFengShuiEngine":
                            if (Menu.Item("R1Combo", true).GetValue<KeyBind>().Active)
                            {
                                if (target.Distance(Me.ServerPosition) < E.Range + Me.AttackRange && 
                                    Me.CountEnemiesInRange(500) >= 1 && !target.IsDead)
                                {
                                    R.Cast();
                                }
                            }
                            break;
                        case "RivenIzunaBlade":
                            R2Logic(target);
                            break;
                    }
                }
            }
        }

        private static void R2Logic(AIHeroClient target)
        {
            if (target == null)
            {
                return; 
            }

            if (R.Instance.Name != "RivenIzunaBlade")
            {
                return;
            }

            if (target.IsValidTarget(850) && !target.IsDead)
            {
                switch (Menu.Item("R2Mode", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (R.GetDamage(target) > target.Health && target.IsValidTarget(R.Range) &&
                            target.Distance(Me.ServerPosition) < 600)
                        {
                            R.Cast(target);
                        }
                        break;
                    case 1:
                        if (target.HealthPercent < 30 &&
                            target.Health > R.GetDamage(target) + Me.GetAutoAttackDamage(target) * 2)
                        {
                            R.Cast(target);
                        }
                        break;
                    case 2:
                        if (target.IsValidTarget(R.Range) &&
                            target.Distance(Me.ServerPosition) < 600)
                        {
                            R.Cast(target);
                        }
                        break;
                }
            }
        }

        private static void Brust()
        {
            var target = TargetSelector.GetSelectedTarget();

            if (target != null && !target.IsDead && target.IsValidTarget() && !target.IsZombie)
            {
                BurstTarget = target;

                if (R.IsReady() && R.Instance.Name == "RivenFengShuiEngine")
                {
                    if (Q.IsReady() && E.IsReady() &&
                        W.IsReady() &&
                        target.Distance(Me.ServerPosition) < E.Range + Me.AttackRange + 100)
                    {
                        E.Cast(target.Position);
                    }

                    if (E.IsReady() &&
                        target.Distance(Me.ServerPosition) < Me.AttackRange + E.Range + 100)
                    {
                        R.Cast();
                        E.Cast(target.Position);
                    }
                }

                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if ((QStack == 1 || QStack == 2 || target.HealthPercent < 50) && R.Instance.Name == "RivenIzunaBlade")
                {
                    R.Cast(target.ServerPosition);
                }

                if (Menu.Item("BurstIgnite", true).GetValue<bool>() && Ignite != SpellSlot.Unknown && Ignite.IsReady())
                {
                    if (target.HealthPercent < 50)
                    {
                        Me.Spellbook.CastSpell(Ignite, target);
                    }
                }

                if (Menu.Item("BurstFlash", true).GetValue<bool>() && Flash != SpellSlot.Unknown)
                {
                    if (Flash.IsReady() && R.IsReady() &&
                        R.Instance.Name == "RivenFengShuiEngine" && E.IsReady() &&
                        W.IsReady() && target.Distance(Me.ServerPosition) <= 780 &&
                        target.Distance(Me.ServerPosition) >= E.Range + Me.AttackRange + 85)
                    {
                        R.Cast();
                        E.Cast(target.Position);
                        LeagueSharp.Common.Utility.DelayAction.Add(150,
                            () => { Me.Spellbook.CastSpell(Flash, target.Position); });
                    }
                }
            }
            else
            {
                BurstTarget = null;
            }
        }

        private static void Harass()
        {
            if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady())
            {
                if (HeroManager.Enemies.Find(
                        x => x.IsValidTarget(W.Range) && !x.HasBuffOfType(BuffType.SpellShield)) != null)
                {
                    W.Cast();
                }
            }
        }

        private static void QuickHarass()
        {
            var target = TargetSelector.GetSelectedTarget();

            if (target != null && target.IsValidTarget())
            {
                if (QStack == 2)
                {
                    if (E.IsReady())
                    {
                        E.Cast(Me.ServerPosition +
                                       (Me.ServerPosition - target.ServerPosition).Normalized() * E.Range);
                    }
                    else
                    {
                        Q.Cast(Me.ServerPosition +
                                       (Me.ServerPosition - target.ServerPosition).Normalized() * E.Range);
                    }
                }

                if (W.IsReady())
                {
                    if (target.IsValidTarget(W.Range) && QStack == 1)
                    {
                        W.Cast();
                    }
                }

                if (Q.IsReady())
                {
                    if (QStack == 0)
                    {
                        if (target.IsValidTarget(Me.AttackRange + Me.BoundingRadius + 150))
                        {
                            CastQ(target);
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (Menu.Item("LaneClearW", true).GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(Me.ServerPosition, W.Range);

                if (W.IsReady() && minions.Count >= 3 && (!Q.IsReady() || QStack == 0))
                {
                    W.Cast();
                }
            }
        }

        private static void FleeLogic()
        {
            var target =
                HeroManager.Enemies.FirstOrDefault(
                    enemy => enemy.IsValidTarget(W.Range) && !enemy.HasBuffOfType(BuffType.SpellShield));

            if (W.IsReady() && target != null && target.IsValidTarget(W.Range))
            {
                W.Cast();
            }

            if (E.IsReady() && !Me.IsDashing())
            {
                E.Cast(Me.Position.Extend(Game.CursorPos, 300));
            }
            else if (Q.IsReady() && !Me.IsDashing())
            {
                Q.Cast(Game.CursorPos);
            }
        }

        private static void WallJump()
        {
            if (QStack != 2 && Q.IsReady())
            {
                Q.Cast(Game.CursorPos);
            }
            else
            {
                var dashEndPos = Me.Position.Extend(Game.CursorPos, Q.Range);

                if (Common.Common.CanWallJump(dashEndPos, E.Range))
                {
                    Q.Cast(dashEndPos);
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Menu.Item("drawingW", true).GetValue<bool>() && W.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(3, 136, 253));
            }

            if (Menu.Item("BrustMaxRange", true).GetValue<bool>() && Me.Level >= 6 && R.IsReady())
            {
                if (E.IsReady() && Flash != SpellSlot.Unknown && Flash.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, 465 + E.Range, Color.FromArgb(253, 3, 3));
                }
            }

            if (Menu.Item("BrustMinRange", true).GetValue<bool>() && Me.Level >= 6 && R.IsReady())
            {
                if (E.IsReady() && Flash != SpellSlot.Unknown && Flash.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range + Me.BoundingRadius, Color.FromArgb(243, 253, 3));
                }
            }

            if (Menu.Item("DrawDamage", true).GetValue<bool>())
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsZombie))
                {
                    DrawHpBar.Unit = target;
                    DrawHpBar.DrawDmg((float)GetComboDamage(target), new ColorBGRA(255, 204, 0, 170));
                }
            }

            if (Menu.Item("QuickHarassRange", true).GetValue<bool>())
            {
                Render.Circle.DrawCircle(Me.Position, E.Range + Me.BoundingRadius, Color.FromArgb(237, 7, 246));
            }

            if (Menu.Item("ShowR1", true).GetValue<bool>())
            {
                var text = "";

                text = Menu.Item("R1Combo", true).GetValue<KeyBind>().Active ? "Enable" : "Off";

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, Color.Red, "Use R1: ");
                Drawing.DrawText(Me.HPBarPosition.X + 90, Me.HPBarPosition.Y - 40, Color.FromArgb(238, 242, 7), text);

                Menu.Item("R1Combo", true).Permashow();
            }

            if (Menu.Item("ShowBurst", true).GetValue<bool>())
            {
                foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget()))
                {
                    var text = "";
                    var text2 = "";
                    var Mepos = Drawing.WorldToScreen(Me.Position);
                    var target = TargetSelector.GetSelectedTarget();

                    if (target == null)
                    {
                        text = "Lock Target Is Null!";
                    }
                    else
                    {
                        text = "Lock Target is : " + target.ChampionName;
                        text2 = "Can Flash : " + CanFlash;
                    }

                    if (Menu.Item("BurstFlash", true).GetValue<bool>() && Flash != SpellSlot.Unknown &&
                        Flash.IsReady() && e.Distance(Me.ServerPosition) <= 800 &&
                        e.Distance(Me.ServerPosition) >= E.Range + Me.AttackRange + 85)
                    {
                        CanFlash = true;
                    }
                    else
                    {
                        CanFlash = false;
                    }

                    Drawing.DrawText(Mepos[0] - 20, Mepos[1], Color.Red, text);
                    Drawing.DrawText(Mepos[0] - 20, Mepos[1] + 14, Color.GreenYellow, text2);
                }
            }
        }

        private static double GetComboDamage(AIHeroClient target)
        {
            if (target == null)
            {
                return 0;
            }

            //Thanks Asuvril
            double passive = 0;

            if (Me.Level == 18)
            {
                passive = 0.5;
            }
            else if (Me.Level >= 15)
            {
                passive = 0.45;
            }
            else if (Me.Level >= 12)
            {
                passive = 0.4;
            }
            else if (Me.Level >= 9)
            {
                passive = 0.35;
            }
            else if (Me.Level >= 6)
            {
                passive = 0.3;
            }
            else if (Me.Level >= 3)
            {
                passive = 0.25;
            }
            else
            {
                passive = 0.2;
            }

            double damage = 0;

            if (Q.IsReady())
            {
                var qhan = 3 - QStack;

                damage += Q.GetDamage(target) * qhan + Me.GetAutoAttackDamage(target) * qhan * (1 + passive);
            }

            if (W.IsReady())
            {
                damage += W.GetDamage(target);
            }

            if (R.IsReady() && Me.HasBuff("RivenFengShuiEngine"))
            {
                damage += Me.CalcDamage(target, Damage.DamageType.Physical,
                    (new double[] { 80, 120, 160 }[R.Level - 1] +
                     0.6 * Me.FlatPhysicalDamageMod) *
                    (1 + (target.MaxHealth - target.Health) /
                     target.MaxHealth > 0.75
                        ? 0.75
                        : (target.MaxHealth - target.Health) / target.MaxHealth) * 8 / 3);
            }

            return damage;
        }

        private static void CastItem(bool tiamat = false, bool youmuu = false)
        {
            if (tiamat)
            {
                if (Items.HasItem(3077) && Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                }

                if (Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }

                if (Items.HasItem(3053) && Items.CanUseItem(3053))
                {
                    Items.UseItem(3053);
                }
            }

            if (youmuu)
            {
                if (Items.HasItem(3142) && Items.CanUseItem(3142))
                {
                    Items.UseItem(3142);
                }
            }
        }

        private static void CastQ(AttackableUnit target)
        {
            CanQ = true;
            QTarget = target;
        }
    }
}