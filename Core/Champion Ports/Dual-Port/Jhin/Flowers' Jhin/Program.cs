using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Jhin
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;
    using System.Linq;
    using SharpDX;
    using Color = System.Drawing.Color;

    internal class Program
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Menu Menu;
        public static AIHeroClient Me;
        public static AIHeroClient rShotTarget;
        public static int SkinID;
        public static int LastPingT;
        public static int LastECast;
        public static int LastShowNoit;
        public static bool IsAttack;
        public static Vector2 PingLocation;
        public static Orbwalking.Orbwalker Orbwalker;
        public static HpBarDraw HpbarDraw = new HpBarDraw();

        public static void Main()
        {
            OnGameLoad();
        }

        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "jhin")
            {
                return;
            }

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 2500f);
            E = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R, 3500f);

            W.SetSkillshot(0.75f, 40, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 120, 1600, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.21f, 80, 5000, false, SkillshotType.SkillshotLine);

            SkinID = Me.SkinId;

            Menu = new Menu("Flowers' Jhin", "Flowers' Jhin", true);

            var OrbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);
            }

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboWAA", "Use W| After Attack?", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboWOnly", "Use W| Only Use to MarkTarget?", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R| In Shot Mode", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboItem", "Items Setting", true));
                ComboMenu.AddItem(new MenuItem("ComboYoumuu", "Use Youmuu", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboCutlass", "Use Cutlass", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboBotrk", "Use Botrk", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassWOnly", "Use W| Only Use to MarkTarget?", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var LastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                LastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q", true).SetValue(true));
                LastHitMenu.AddItem(
                    new MenuItem("LastHitMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
            }

            var RMenu = Menu.AddSubMenu(new Menu("R Menu", "RMenu"));
            {
                RMenu.AddItem(new MenuItem("RMenuAuto", "Auto R?", true).SetValue(true));
                RMenu.AddItem(
                    new MenuItem("RMenuSemi", "Semi R Key(One Press One Shot)", true).SetValue(new KeyBind('T',
                        KeyBindType.Press)));
                RMenu.AddItem(new MenuItem("RMenuCheck", "Use R| Check is Safe?", true).SetValue(true));
                RMenu.AddItem(
                    new MenuItem("RMenuMin", "Use R| Min Range >= x", true).SetValue(new Slider(1000, 500, 2500)));
                RMenu.AddItem(
                    new MenuItem("RMenuMax", "Use R| Man Range <= x", true).SetValue(new Slider(3000, 1500, 3500)));
                RMenu.AddItem(
                    new MenuItem("RMenuKill", "Use R| Min Shot Can Kill >= x", true).SetValue(new Slider(3, 1, 4)));
                RMenu.AddItem(new MenuItem("PingKill", "Auto Ping Kill Target", true).SetValue(true));
                RMenu.AddItem(new MenuItem("NormalPingKill", "Normal Ping?", true).SetValue(true));
                RMenu.AddItem(new MenuItem("NotificationKill", "Notification Kill Target", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(new MenuItem("AutoW", "Auto W| When target Cant Move", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("AutoE", "Auto E| When target Cant Move", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("GapW", "Anti GapCloser W| When target HavePassive", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("GapE", "Anti GapCloser E", true).SetValue(true));
            }

            var PredMenu = Menu.AddSubMenu(new Menu("Prediction", "Prediction"));
            {
                PredMenu.AddItem(new MenuItem("SelectPred", "Select Prediction: ", true).SetValue(new StringList(new[]
                {
                    "Common Prediction", "OKTW Prediction", "SDK Prediction", "SPrediction(Need F5 Reload)",
                    "xcsoft AIO Prediction"
                }, 1)));
                PredMenu.AddItem(
                    new MenuItem("SetHitchance", "HitChance: ", true).SetValue(
                        new StringList(new[] {"VeryHigh", "High", "Medium", "Low"})));
            }

            var SkinMenu = Menu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
            {
                SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged += EnbaleSkin;
                SkinMenu.AddItem(
                    new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(
                        new StringList(new[] {"Classic", "High Noon Jhin"})));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawRMin", "Draw R Range(MinMap)", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();

            Chat.Print("Flowers' Jhin Load Succeed! Credit: NightMoon");

            if (Menu.Item("SelectPred", true).GetValue<StringList>().SelectedIndex == 3)
            {
                SPrediction.Prediction.Initialize(PredMenu);
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Me.GetSpellSlot(Args.SData.Name) == SpellSlot.E)
            {
                LastECast = Utils.TickCount;
            }

            if (Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                IsAttack = true;
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => IsAttack = false);
            }
        }

        private static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                //ObjectManager.//Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;

            if (target.IsValidTarget(E.Range) &&
                (gapcloser.End.DistanceToPlayer() <= 300 || target.DistanceToPlayer() <= 300))
            {
                if (Menu.Item("GapE", true).GetValue<bool>() && E.IsReady() && Utils.TickCount - LastECast > 2500 &&
                    !IsAttack)
                {
                    E.CastTo(target);
                }

                if (Menu.Item("GapW", true).GetValue<bool>() && W.IsReady() && HasPassive(target))
                {
                    W.CastTo(target);
                }
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    {
                        var target = (AIHeroClient)Args.Target;

                        if (target != null && !target.IsDead && !target.IsZombie)
                        {
                            if (Menu.Item("ComboYoumuu", true).GetValue<bool>() &&
                                Items.HasItem(3142) && Items.CanUseItem(3142))
                            {
                                Items.UseItem(3142);
                            }

                            if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() &&
                                target.IsValidTarget(Q.Range))
                            {
                                Q.CastOnUnit(target, true);
                            }

                            if (Menu.Item("ComboW", true).GetValue<bool>() 
                                && Menu.Item("ComboWAA", true).GetValue<bool>() && W.IsReady() && 
                                target.IsValidTarget(W.Range) && target.HasBuff("jhinespotteddebuff"))
                            {
                                W.CastTo(target);
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    {
                        if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
                        {
                            var hero = Args.Target as AIHeroClient;

                            if (hero != null && !hero.IsDead)
                            {
                                var target = hero;

                                if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() &&
                                    target.IsValidTarget(Q.Range))
                                {
                                    Q.CastOnUnit(target, true);
                                }

                                if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady()
                                    && target.IsValidTarget(W.Range) && target.HasBuff("jhinespotteddebuff"))
                                {
                                    W.CastTo(target);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            foreach (var enemy in HeroManager.Enemies.Where(h => R.IsReady() && h.IsValidTarget(R.Range) &&
                                                                 Me.GetSpellDamage(h, SpellSlot.R)*
                                                                 Menu.Item("RMenuKill", true).GetValue<Slider>().Value >
                                                                 h.Health + h.HPRegenRate*3))
            {
                if (Menu.Item("PingKill", true).GetValue<bool>())
                {
                    Ping(enemy.Position.To2D());
                }

                if (Menu.Item("NotificationKill", true).GetValue<bool>() && Utils.TickCount - LastShowNoit > 10000)
                {
                    Notifications.AddNotification(
                        new Notification("R Kill: " + enemy.ChampionName + "!", 3000, true).SetTextColor(
                            Color.FromArgb(255, 0, 0)));
                    LastShowNoit = Utils.TickCount;
                }
            }

            RLogic();

            if (R.Instance.Name == "JhinRShot")
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                return;
            }

            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);

            KillSteal();
            Auto();

            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
            }

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
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }

        private static void LastHit()
        {
            if (Me.ManaPercent >= Menu.Item("LastHitMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("LastHitQ", true).GetValue<bool>() && Q.IsReady())
                {
                    if (Me.CountEnemiesInRange(Q.Range + 300) > 0)
                    {
                        var target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Physical);

                        if (CheckTarget(target, Q.Range + 300))
                        {
                            if (Me.HasBuff("JhinPassiveReload") ||
                                (!Me.HasBuff("JhinPassiveReload") &&
                                 Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me)) == 0))
                            {
                                var qPred = LeagueSharp.Common.Prediction.GetPrediction(target, 0.25f);
                                var bestQMinion =
                                    MinionManager.GetMinions(qPred.CastPosition, 300)
                                        .Where(x => x.IsValidTarget(Q.Range))
                                        .OrderBy(x => x.Distance(target))
                                        .ThenBy(x => x.Health)
                                        .FirstOrDefault();

                                if (bestQMinion != null)
                                {
                                    Q.CastOnUnit(bestQMinion, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        var minion =
                            MinionManager.GetMinions(Me.Position, Q.Range)
                                .Where(x => x.IsValidTarget(Q.Range) && HealthPrediction.GetHealthPrediction(x, 250) > 0)
                                .OrderBy(x => x.Health)
                                .FirstOrDefault(x => x.Health < Q.GetDamage(x));

                        if (minion != null)
                        {
                            Q.CastOnUnit(minion, true);
                        }
                    }
                }
            }
        }

        private static void RLogic()
        {
            AIHeroClient target = null;

            if (TargetSelector.GetSelectedTarget() != null &&
                TargetSelector.GetSelectedTarget().DistanceToPlayer() <=
                Menu.Item("RMenuMax", true).GetValue<Slider>().Value)
            {
                target = TargetSelector.GetSelectedTarget();
            }
            else
            {
                target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            }

            if (R.IsReady() && CheckTarget(target, R.Range))
            {
                if (R.Instance.Name == "JhinR")
                {
                    if (Menu.Item("RMenuSemi", true).GetValue<KeyBind>().Active)
                    {
                        if (R.Cast(R.GetPrediction(target).UnitPosition))
                        {
                            rShotTarget = target;
                            return;
                        }
                    }

                    if (!Menu.Item("RMenuAuto", true).GetValue<bool>())
                    {
                        return;
                    }

                    if (Menu.Item("RMenuCheck", true).GetValue<bool>() && Me.CountEnemiesInRange(800f) > 0)
                    {
                        return;
                    }

                    if (target.DistanceToPlayer() <= Menu.Item("RMenuMin", true).GetValue<Slider>().Value)
                    {
                        return;
                    }

                    if (target.DistanceToPlayer() > Menu.Item("RMenuMax", true).GetValue<Slider>().Value)
                    {
                        return;
                    }

                    if (target.Health >
                        Me.GetSpellDamage(target, SpellSlot.R)*Menu.Item("RMenuKill", true).GetValue<Slider>().Value)
                    {
                        return;
                    }

                    if (SebbyLib.OktwCommon.IsSpellHeroCollision(target, R))
                    {
                        return;
                    }

                    if (R.Cast(R.GetPrediction(target).UnitPosition))
                    {
                        rShotTarget = target;
                        return;
                    }
                }

                if (R.Instance.Name == "JhinRShot")
                {
                    if (rShotTarget != null && rShotTarget.IsValidTarget(R.Range))
                    {
                        if (!InRCone(rShotTarget))
                        {
                            return;
                        }

                        if (Menu.Item("RMenuSemi", true).GetValue<KeyBind>().Active)
                        {
                            AutoUse(rShotTarget);
                            R.CastTo(rShotTarget);
                            return;
                        }

                        if (Menu.Item("ComboR", true).GetValue<bool>() &&
                            Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                        {
                            AutoUse(rShotTarget);
                            R.CastTo(rShotTarget);
                            return;
                        }

                        if (!Menu.Item("RMenuAuto", true).GetValue<bool>())
                        {
                            return;
                        }

                        AutoUse(rShotTarget);
                        R.CastTo(rShotTarget);

                    }
                    else
                    {
                        foreach (
                            var t in
                            HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && InRCone(x))
                                .OrderBy(x => x.Health))
                        {
                            if (!InRCone(t))
                            {
                                return;
                            }

                            if (Menu.Item("RMenuSemi", true).GetValue<KeyBind>().Active)
                            {
                                AutoUse(t);
                                R.Cast(R.GetPrediction(t).UnitPosition, true);
                                return;
                            }

                            if (Menu.Item("ComboR", true).GetValue<bool>() &&
                                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                            {
                                AutoUse(t);
                                R.Cast(R.GetPrediction(t).UnitPosition, true);
                                return;
                            }

                            if (!Menu.Item("RMenuAuto", true).GetValue<bool>())
                            {
                                return;
                            }

                            AutoUse(t);
                            R.Cast(R.GetPrediction(t).UnitPosition, true);
                            return;
                        }
                    }
                }
            }
        }

        private static void KillSteal()
        {
            if (R.Instance.Name == "JhinRShot")
                return;

            var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if (Menu.Item("KillStealW", true).GetValue<bool>() && CheckTarget(wTarget, Q.Range) && W.IsReady() && 
                wTarget.Health < Me.GetSpellDamage(wTarget, SpellSlot.W) &&
                !(Q.IsReady() && wTarget.IsValidTarget(Q.Range) &&
                wTarget.Health < Me.GetSpellDamage(wTarget, SpellSlot.Q)))
            {
                W.CastTo(wTarget);
                return;
            }

            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (Menu.Item("KillStealQ", true).GetValue<bool>() && CheckTarget(qTarget, Q.Range) &&
                Q.IsReady() && qTarget.Health < Me.GetSpellDamage(qTarget, SpellSlot.Q))
            {
                Q.CastOnUnit(qTarget, true);
            }
        }

        private static void Auto()
        {
            if (R.Instance.Name == "JhinRShot")
                return;

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.CanMove()))
            {
                if (Menu.Item("AutoW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.CastTo(target);
                }

                if (Menu.Item("AutoE", true).GetValue<bool>() && E.IsReady() && 
                    target.IsValidTarget(E.Range) && Utils.TickCount - LastECast > 2500 && !IsAttack)
                {
                    E.CastTo(target);
                }
            }
        }

        private static void Combo()
        {
            if (R.Instance.Name == "JhinRShot")
            {
                return;
            }

            var orbTarget = Orbwalker.GetTarget();

            var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && CheckTarget(wTarget, W.Range))
            {
                if (Menu.Item("ComboWOnly", true).GetValue<bool>())
                {
                    if (HasPassive(wTarget))
                    {
                        W.CastTo(wTarget);
                    }
                }
                else
                {
                    W.CastTo(wTarget);
                }
            }

            if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Physical);
                var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(qTarget, Q.Range) && !Orbwalking.CanAttack())
                {
                    Q.CastOnUnit(qTarget, true);
                }
                else if (CheckTarget(target, Q.Range + 300))
                {
                    if (Me.HasBuff("JhinPassiveReload") ||
                        (!Me.HasBuff("JhinPassiveReload") &&
                         Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me)) == 0))
                    {
                        var qPred = LeagueSharp.Common.Prediction.GetPrediction(target, 0.25f);
                        var bestQMinion =
                            MinionManager.GetMinions(qPred.CastPosition, 300)
                                .Where(x => x.IsValidTarget(Q.Range))
                                .OrderBy(x => x.Distance(target))
                                .ThenBy(x => x.Health)
                                .FirstOrDefault();

                        if (bestQMinion != null)
                        {
                            Q.CastOnUnit(bestQMinion, true);
                        }
                    }
                }
            }

            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady()
                && CheckTarget(eTarget, E.Range) && Utils.TickCount - LastECast > 2500 && !IsAttack)
            {
                if (!eTarget.CanMove())
                {
                    E.CastTo(eTarget);
                }
                else
                {
                    if (E.GetPrediction(eTarget).Hitchance >= HitChance.High)
                    {
                        E.Cast(E.GetPrediction(eTarget).UnitPosition);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Physical);

                    if (CheckTarget(target, Q.Range + 300))
                    {
                        if (Me.HasBuff("JhinPassiveReload") ||
                            (!Me.HasBuff("JhinPassiveReload") &&
                             Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me)) == 0))
                        {
                            var qPred = LeagueSharp.Common.Prediction.GetPrediction(target, 0.25f);
                            var bestQMinion =
                                MinionManager.GetMinions(qPred.CastPosition, 300)
                                    .Where(x => x.IsValidTarget(Q.Range))
                                    .OrderBy(x => x.Distance(target))
                                    .ThenBy(x => x.Health)
                                    .FirstOrDefault();

                            if (bestQMinion != null)
                            {
                                Q.CastOnUnit(bestQMinion, true);
                            }
                        }
                    }
                }

                var wTarget = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);

                if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady() && CheckTarget(wTarget, W.Range))
                {
                    if (Menu.Item("HarassWOnly", true).GetValue<bool>() && !HasPassive(wTarget))
                    {
                        return;
                    }

                    W.CastTo(wTarget);
                }

                var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady() 
                    && CheckTarget(eTarget, E.Range) && Utils.TickCount - LastECast > 2500 && !IsAttack)
                {
                    E.CastTo(eTarget, true);
                }
            }
        }

        private static void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                if (!minions.Any())
                {
                    return;
                }

                var minion = minions.MinOrDefault(x => x.Health);


                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady())
                {
                    if (Me.CountEnemiesInRange(Q.Range + 300) > 0)
                    {
                        var target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Physical);

                        if (CheckTarget(target, Q.Range + 300))
                        {
                            if (Me.HasBuff("JhinPassiveReload") ||
                                (!Me.HasBuff("JhinPassiveReload") &&
                                 Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me)) == 0))
                            {
                                var qPred = LeagueSharp.Common.Prediction.GetPrediction(target, 0.25f);
                                var bestQMinion =
                                    MinionManager.GetMinions(qPred.CastPosition, 300)
                                        .Where(x => x.IsValidTarget(Q.Range))
                                        .OrderBy(x => x.Distance(target))
                                        .ThenBy(x => x.Health)
                                        .FirstOrDefault();

                                if (bestQMinion != null)
                                {
                                    Q.CastOnUnit(bestQMinion, true);
                                }
                            }
                        }
                    }
                    else if (minion != null && minion.IsValidTarget(Q.Range) && minions.Count > 2)
                    {
                        Q.Cast(minion, true);
                    }
                }

                if (Menu.Item("LaneClearW", true).GetValue<bool>() && W.IsReady() && minion != null)
                {
                    W.Cast(minion, true);
                }
            }
        }

        private static void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Me.Position, 700, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                if (!mobs.Any())
                {
                    return;
                }

                var mob = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));

                if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady())
                {
                    W.CastTo(mob ?? mobs.FirstOrDefault());
                }

                if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Q.CastOnUnit(mob ?? mobs.FirstOrDefault());
                }

                if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady() &&
                    mob.IsValidTarget(E.Range) && Utils.TickCount - LastECast > 2500 && !IsAttack)
                {
                    E.CastTo(mob ?? mobs.FirstOrDefault());
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.Item("DrawW", true).GetValue<bool>() && W.Level > 0)
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 1);
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }

                if (Menu.Item("DrawR", true).GetValue<bool>() && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(19, 130, 234), 1);
                }

                if (Menu.Item("DrawDamage", true).GetValue<bool>())
                {
                    foreach (
                        var x in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg((float)ComboDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
                }
            }
        }

        private static void OnEndScene(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
#pragma warning disable 618
                if (Menu.Item("DrawRMin", true).GetValue<bool>() && R.IsReady())
                    LeagueSharp.Common.Utility.DrawCircle(Me.Position, R.Range, Color.FromArgb(14, 194, 255), 1, 30, true);
#pragma warning restore 618
            }
        }

        private static double ComboDamage(AIHeroClient target)
        {
            if (target != null && target.IsValidTarget())
            {
                var Damage = 0d;

                Damage += Me.GetAutoAttackDamage(target);

                if (Q.IsReady())
                {
                    Damage += Me.Spellbook.GetSpell(SpellSlot.Q).IsReady() ? Me.GetSpellDamage(target, SpellSlot.Q) : 0d;
                }

                if (W.IsReady())
                {
                    Damage += Me.Spellbook.GetSpell(SpellSlot.W).IsReady() ? Me.GetSpellDamage(target, SpellSlot.W) : 0d;
                }

                if (E.IsReady())
                {
                    Damage += Me.Spellbook.GetSpell(SpellSlot.E).IsReady() ? Me.GetSpellDamage(target, SpellSlot.E) : 0d;
                }

                if (R.IsReady())
                {
                    Damage += Me.Spellbook.GetSpell(SpellSlot.R).IsReady() ? Me.GetSpellDamage(target, SpellSlot.R) * Menu.Item("RMenuKill", true).GetValue<Slider>().Value : 0d;
                }

                if (Me.GetSpellSlot("SummonerDot") != SpellSlot.Unknown && Me.GetSpellSlot("SummonerDot").IsReady())
                {
                    Damage += 50 + 20 * Me.Level - target.HPRegenRate / 5 * 3;
                }

                if (target.ChampionName == "Moredkaiser")
                    Damage -= target.Mana;

                // exhaust
                if (Me.HasBuff("SummonerExhaust"))
                    Damage = Damage * 0.6f;

                // blitzcrank passive
                if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
                    Damage -= target.Mana / 2f;

                // kindred r
                if (target.HasBuff("KindredRNoDeathBuff"))
                    Damage = 0;

                // tryndamere r
                if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                    Damage = 0;

                // kayle r
                if (target.HasBuff("JudicatorIntervention"))
                    Damage = 0;

                // zilean r
                if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                    Damage = 0;

                // fiora w
                if (target.HasBuff("FioraW"))
                    Damage = 0;

                return Damage;
            }
            return 0d;
        }

        private static void AutoUse(AIHeroClient target)
        {
            if (Items.HasItem(3363) && Items.CanUseItem(3363))
            {
                Items.UseItem(3363, target.Position);
            }
        }

        private static bool HasPassive(AIHeroClient target)
        {
            return target.HasBuff("jhinespotteddebuff");
        }

        private static bool CheckTarget(Obj_AI_Base target, float Range)
        {
            return target.IsValidTarget(Range) && !target.IsDead && !target.IsZombie && !DontCast(target);
        }

        private static bool InRCone(AIHeroClient target) 
        {
            // Asuvril
            // https://github.com/VivianGit/LeagueSharp/blob/master/Jhin%20As%20The%20Virtuoso/Jhin%20As%20The%20Virtuoso/Extensions.cs#L67-L79
            var range = R.Range;
            const float angle = 70f * (float)Math.PI / 180;
            var end2 = target.Position.To2D() - Me.Position.To2D();
            var edge1 = end2.Rotated(-angle / 2);
            var edge2 = edge1.Rotated(angle);

            var point = target.Position.To2D() - Me.Position.To2D();

            return point.Distance(new Vector2(), true) < range * range && 
                edge1.CrossProduct(point) > 0 && point.CrossProduct(edge2) > 0;
        }

        private static bool DontCast(Obj_AI_Base target)
        {
            // kindred r
            if (target.HasBuff("KindredRNoDeathBuff"))
                return true;

            // tryndamere r
            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                return true;

            // kayle r
            if (target.HasBuff("JudicatorIntervention"))
                return true;

            // zilean r
            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                return true;

            // fiora w
            if (target.HasBuff("FioraW"))
                return true;

            return false;
        }

        private static void Ping(Vector2 position)
        {
            if (Utils.TickCount - LastPingT < 30 * 1000)
            {
                return;
            }

            LastPingT = Utils.TickCount;
            PingLocation = position;
            SimplePing();

            LeagueSharp.Common.Utility.DelayAction.Add(150, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(300, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(400, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(800, SimplePing);
        }

        private static void SimplePing()
        {
            TacticalMap.ShowPing(
                Menu.Item("NormalPingKill", true).GetValue<bool>() ? PingCategory.Normal : PingCategory.Fallback,
                PingLocation, true);
        }
    }
}
