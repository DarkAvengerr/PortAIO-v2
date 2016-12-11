using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = ADCCOMMON.Orbwalking;

    internal class Jhin : Logic
    {
        private AIHeroClient rShotTarget;
        private int LastPingT;
        private int LastECast;
        private int LastShowNoit;
        private bool IsAttack;
        private Vector2 PingLocation;

        public Jhin()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 2500f);
            E = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R, 3500f);

            W.SetSkillshot(0.75f, 40, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 120, 1600, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.21f, 80, 5000, false, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboQMinion", "Use Q| Minion", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboWAA", "Use W| After Attack?", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboWOnly", "Use W| Only Use to MarkTarget?", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R| In Shot Mode", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboYoumuu", "Use Youmuu", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassWOnly", "Use W| Only Use to MarkTarget?", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var lastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                lastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q", true).SetValue(true));
                lastHitMenu.AddItem(
                    new MenuItem("LastHitMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealWInAttackRange", "Use W| Target In Attack Range", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var wMenu = miscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wMenu.AddItem(new MenuItem("AutoW", "Auto W| When target Cant Move", true).SetValue(true));
                    wMenu.AddItem(new MenuItem("GapW", "Anti GapCloser W| When target HavePassive", true).SetValue(true));
                }

                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(new MenuItem("AutoE", "Auto E| When target Cant Move", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("GapE", "Anti GapCloser E", true).SetValue(true));
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("rMenuAuto", "Auto R?", true).SetValue(true));
                    rMenu.AddItem(
                        new MenuItem("rMenuSemi", "Semi R Key(One Press One Shot)", true).SetValue(new KeyBind('T',
                            KeyBindType.Press)));
                    rMenu.AddItem(new MenuItem("rMenuCheck", "Use R| Check is Safe?", true).SetValue(true));
                    rMenu.AddItem(
                        new MenuItem("rMenuMin", "Use R| Min Range >= x", true).SetValue(new Slider(1000, 500, 2500)));
                    rMenu.AddItem(
                        new MenuItem("rMenuMax", "Use R| Man Range <= x", true).SetValue(new Slider(3000, 1500, 3500)));
                    rMenu.AddItem(
                        new MenuItem("rMenuKill", "Use R| Min Shot Can Kill >= x", true).SetValue(new Slider(3, 1, 4)));
                }

                miscMenu.AddItem(new MenuItem("PingKill", "Auto Ping Kill Target", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("NormalPingKill", "Normal Ping?", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("NotificationKill", "Notification Kill Target", true).SetValue(true));
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

                var itemsMenu = utilityMenu.AddSubMenu(new Menu("Items", "Items"));
                {
                    ItemsManager.AddToMenu(itemsMenu);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawRMin", "Draw R Range(MinMap)", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            var spellslot = Me.GetSpellSlot(Args.SData.Name);

            if (spellslot == SpellSlot.E)
            {
                LastECast = Utils.TickCount;
            }

            if (Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                IsAttack = true;
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => IsAttack = false);
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.Instance.Name == "JhinRShot")
            {
                return;
            }

            var target = gapcloser.Sender;

            if (target.IsValidTarget(E.Range) && (gapcloser.End.DistanceToPlayer() <= 300 || target.DistanceToPlayer() <= 300))
            {
                if (Menu.GetBool("GapE") && E.IsReady() && Utils.TickCount - LastECast > 2500 && !IsAttack)
                {
                    SpellManager.PredCast(E, target, true);
                }

                if (Menu.GetBool("GapW") && W.IsReady() && HasPassive(target))
                {
                    SpellManager.PredCast(W, target, true);
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            if (Args.Target is Obj_LampBulb)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    {
                        var target = Args.Target as AIHeroClient;

                        if (target != null && !target.IsDead && !target.IsZombie)
                        {
                            if (Menu.GetBool("ComboYoumuu") && Items.HasItem(3142) && Items.CanUseItem(3142))
                            {
                                Items.UseItem(3142);
                            }

                            if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                            {
                                Q.CastOnUnit(target, true);
                            }
                            else if (Menu.GetBool("ComboW") && Menu.GetBool("ComboWAA") && W.IsReady() &&
                                target.IsValidTarget(W.Range) && target.HasBuff("jhinespotteddebuff"))
                            {
                                SpellManager.PredCast(W, target, true);
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    {
                        if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
                        {
                            var hero = Args.Target as AIHeroClient;

                            if (hero != null && !hero.IsDead)
                            {
                                var target = hero;

                                if (Menu.GetBool("HarassQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                                {
                                    Q.CastOnUnit(target, true);
                                }
                                else if (Menu.GetBool("HarassW") && W.IsReady() && target.IsValidTarget(W.Range) &&
                                    target.HasBuff("jhinespotteddebuff"))
                                {
                                    SpellManager.PredCast(W, target, true);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            foreach (var enemy in HeroManager.Enemies.Where(h => R.IsReady() && h.IsValidTarget(R.Range) &&
                                                                 Me.GetSpellDamage(h, SpellSlot.R)*
                                                                 Menu.GetSlider("rMenuKill") > h.Health + h.HPRegenRate*3))
            {
                if (Menu.GetBool("PingKill"))
                {
                    Ping(enemy.Position.To2D());
                }

                if (Menu.GetBool("NotificationKill") && Utils.TickCount - LastShowNoit > 10000)
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
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }

        private void RLogic()
        {
            AIHeroClient target = null;

            if (TargetSelector.GetSelectedTarget() != null &&
                TargetSelector.GetSelectedTarget().DistanceToPlayer() <= Menu.GetSlider("rMenuMax"))
            {
                target = TargetSelector.GetSelectedTarget();
            }
            else
            {
                target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            }

            if (R.IsReady() && target.Check(R.Range))
            {
                switch (R.Instance.Name)
                {
                    case "JhinR":
                        if (Menu.GetKey("rMenuSemi"))
                        {
                            if (R.Cast(R.GetPrediction(target).UnitPosition))
                            {
                                rShotTarget = target;
                                return;
                            }
                        }

                        if (!Menu.GetBool("rMenuAuto"))
                        {
                            return;
                        }

                        if (Menu.GetBool("rMenuCheck") && Me.CountEnemiesInRange(800f) > 0)
                        {
                            return;
                        }

                        if (target.DistanceToPlayer() <= Menu.GetSlider("rMenuMin"))
                        {
                            return;
                        }

                        if (target.DistanceToPlayer() > Menu.GetSlider("rMenuMax"))
                        {
                            return;
                        }

                        if (target.Health >
                            Me.GetSpellDamage(target, SpellSlot.R) * Menu.GetSlider("rMenuKill"))
                        {
                            return;
                        }

                        if (IsSpellHeroCollision(target, R))
                        {
                            return;
                        }

                        if (R.Cast(R.GetPrediction(target).UnitPosition))
                        {
                            rShotTarget = target;
                        }
                        break;
                    case "JhinRShot":
                        var selectTarget = TargetSelector.GetSelectedTarget();

                        if (selectTarget != null && selectTarget.IsValidTarget(R.Range) && InRCone(selectTarget))
                        {
                            if (Menu.GetKey("rMenuSemi"))
                            {
                                AutoUse(rShotTarget);
                                SpellManager.PredCast(R, rShotTarget);
                                return;
                            }

                            if (Menu.GetBool("ComboR") && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                            {
                                AutoUse(rShotTarget);
                                SpellManager.PredCast(R, rShotTarget);
                                return;
                            }

                            if (!Menu.GetBool("rMenuAuto"))
                            {
                                return;
                            }

                            AutoUse(rShotTarget);
                            SpellManager.PredCast(R, rShotTarget);
                            return;
                        }

                        foreach (
                            var t in
                            HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && InRCone(x))
                                .OrderBy(x => x.Health))
                        {
                            if (Menu.GetKey("rMenuSemi"))
                            {
                                AutoUse(t);
                                SpellManager.PredCast(R, t);
                                return;
                            }

                            if (Menu.GetBool("ComboR") && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                            {
                                AutoUse(t);
                                SpellManager.PredCast(R, t);
                                return;
                            }

                            if (!Menu.GetBool("rMenuAuto"))
                            {
                                return;
                            }

                            AutoUse(t);
                            SpellManager.PredCast(R, t);
                            return;
                        }
                        break;
                }
            }
        }

        private void KillSteal()
        {
            if (R.Instance.Name == "JhinRShot")
            {
                return;
            }

            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(Q.Range) && x.Health < Me.GetSpellDamage(x, SpellSlot.Q)))
                {
                    if (target.Check(Q.Range))
                    {
                        Q.CastOnUnit(target, true);
                    }
                }
            }

            if (Menu.GetBool("KillStealW") && W.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(W.Range) && x.Health < Me.GetSpellDamage(x, SpellSlot.W)))
                {
                    if (target.Check(W.Range))
                    {
                        if (target.Health < Me.GetSpellDamage(target, SpellSlot.Q) && Q.IsReady() &&
                            target.IsValidTarget(Q.Range))
                        {
                            return;
                        }

                        if (Menu.GetBool("KillStealWInAttackRange") && Orbwalking.InAutoAttackRange(target))
                        {
                            SpellManager.PredCast(W, target, true);
                            return;
                        }

                        if (Orbwalking.InAutoAttackRange(target) && target.Health <= Me.GetAutoAttackDamage(target, true))
                        {
                            return;
                        }

                        SpellManager.PredCast(W, target, true);
                        return;
                    }
                }
            }
        }

        private void Auto()
        {
            if (R.Instance.Name == "JhinRShot")
            {
                return;
            }

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.CanMoveMent()))
            {
                if (Menu.Item("AutoW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    SpellManager.PredCast(W, target, true);
                }

                if (Menu.Item("AutoE", true).GetValue<bool>() && E.IsReady() &&
                    target.IsValidTarget(E.Range) && Utils.TickCount - LastECast > 2500 && !IsAttack)
                {
                    SpellManager.PredCast(E, target, true);
                }
            }
        }

        private void Combo()
        {
            if (R.Instance.Name == "JhinRShot")
            {
                return;
            }

            var orbTarget = Orbwalker.GetTarget();

            var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if (Menu.GetBool("ComboW") && W.IsReady() && wTarget.Check(W.Range))
            {
                if (Menu.GetBool("ComboWOnly"))
                {
                    if (HasPassive(wTarget))
                    {
                        SpellManager.PredCast(W, wTarget, true);
                    }
                }
                else
                {
                    SpellManager.PredCast(W, wTarget, true);
                }
            }

            if (Menu.GetBool("ComboQ") && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Physical);
                var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                if (qTarget.Check(Q.Range) && !Orbwalking.CanAttack())
                {
                    Q.CastOnUnit(qTarget, true);
                }
                else if (target.Check(Q.Range + 300) && Menu.GetBool("ComboQMinion"))
                {
                    if (Me.HasBuff("JhinPassiveReload") || (!Me.HasBuff("JhinPassiveReload") &&
                         Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me) + Me.BoundingRadius) == 0))
                    {
                        var qPred = Prediction.GetPrediction(target, 0.25f);
                        var bestQMinion =
                            MinionManager.GetMinions(qPred.CastPosition, 300)
                                .Where(x => x.IsValidTarget(Q.Range))
                                .OrderBy(x => x.Health)
                                .ThenBy(x => x.Distance(target))
                                .FirstOrDefault();

                        if (bestQMinion != null && bestQMinion.IsValidTarget(Q.Range))
                        {
                            Q.CastOnUnit(bestQMinion, true);
                        }
                    }
                }
            }

            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (Menu.GetBool("ComboE") && E.IsReady() && eTarget.Check(E.Range) && Utils.TickCount - LastECast > 2500 && !IsAttack)
            {
                if (!eTarget.CanMoveMent())
                {
                    SpellManager.PredCast(E, eTarget, true);
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

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassQ") && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Physical);

                    if (target.Check(Q.Range + 300))
                    {
                        if (Me.HasBuff("JhinPassiveReload") || (!Me.HasBuff("JhinPassiveReload") &&
                             Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me)) == 0))
                        {
                            var qPred = Prediction.GetPrediction(target, 0.25f);
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

                if (Menu.GetBool("HarassE") && E.IsReady() && eTarget.Check(E.Range) &&
                    Utils.TickCount - LastECast > 2500 && !IsAttack)
                {
                    SpellManager.PredCast(E, eTarget, true);
                }

                if (Menu.GetBool("HarassW") && W.IsReady())
                {
                    var target = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);

                    if (target.Check(W.Range))
                    {
                        if (Menu.GetBool("HarassWOnly") && !HasPassive(target))
                        {
                            return;
                        }

                        SpellManager.PredCast(W, target, true);
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

                if (!minions.Any())
                {
                    return;
                }

                var minion = minions.MinOrDefault(x => x.Health);

                if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                {
                    if (Me.CountEnemiesInRange(Q.Range + 300) > 0)
                    {
                        var target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Physical);

                        if (target.Check(Q.Range + 300))
                        {
                            if (Me.HasBuff("JhinPassiveReload") || (!Me.HasBuff("JhinPassiveReload") &&
                                 Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me)) == 0))
                            {
                                var qPred = Prediction.GetPrediction(target, 0.25f);
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

                if (Menu.GetBool("LaneClearW") && W.IsReady() && minion != null)
                {
                    W.Cast(minion, true);
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Me.Position, 700, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));

                    if (Menu.GetBool("JungleClearW") && W.IsReady())
                    {
                        W.Cast(mob ?? mobs.FirstOrDefault());
                    }

                    if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                    {
                        Q.CastOnUnit(mob ?? mobs.FirstOrDefault());
                    }

                    if (Menu.GetBool("JungleClearE") && E.IsReady() && mob.IsValidTarget(E.Range) && 
                        Utils.TickCount - LastECast > 2500 && !IsAttack)
                    {
                        E.Cast(mob ?? mobs.FirstOrDefault());
                    }
                }
            }
        }

        private void LastHit()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LastHitMana")))
            {
                if (Menu.GetBool("LastHitQ") && Q.IsReady())
                {
                    if (Me.CountEnemiesInRange(Q.Range + 300) > 0)
                    {
                        var target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Physical);

                        if (target.Check(Q.Range + 300))
                        {
                            if (Me.HasBuff("JhinPassiveReload") || (!Me.HasBuff("JhinPassiveReload") &&
                                 Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me)) == 0))
                            {
                                var qPred = Prediction.GetPrediction(target, 0.25f);
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

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetBool("DrawQ") && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.GetBool("DrawW") && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 1);
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

        private void OnEndScene(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
#pragma warning disable 618
                if (Menu.GetBool("DrawRMin") && R.IsReady())
                {
                    LeagueSharp.Common.Utility.DrawCircle(Me.Position, R.Range, Color.FromArgb(14, 194, 255), 1, 30, true);
                }
#pragma warning restore 618
            }
        }

        private void AutoUse(GameObject target)
        {
            if (Items.HasItem(3363) && Items.CanUseItem(3363))
            {
                Items.UseItem(3363, target.Position);
            }
        }

        private bool HasPassive(Obj_AI_Base target)
        {
            return target.HasBuff("jhinespotteddebuff");
        }

        private bool InRCone(GameObject target)
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

        private static bool IsSpellHeroCollision(AIHeroClient t, Spell QWER, int extraWith = 50)
        {
            foreach (
                var hero in
                HeroManager.Enemies.FindAll(
                    hero =>
                        hero.IsValidTarget(QWER.Range + QWER.Width, true, QWER.RangeCheckFrom) &&
                        t.NetworkId != hero.NetworkId))
            {
                var prediction = QWER.GetPrediction(hero);
                var powCalc = Math.Pow(QWER.Width + extraWith + hero.BoundingRadius, 2);

                if (
                    prediction.UnitPosition.To2D()
                        .Distance(QWER.From.To2D(), QWER.GetPrediction(t).CastPosition.To2D(), true, true) <= powCalc)
                {
                    return true;
                }

                if (prediction.UnitPosition.To2D().Distance(QWER.From.To2D(), t.ServerPosition.To2D(), true, true) <= powCalc)
                {
                    return true;
                }
            }

            return false;
        }

        private void Ping(Vector2 position)
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

        private void SimplePing()
        {
            TacticalMap.ShowPing(Menu.GetBool("NormalPingKill") ? PingCategory.Normal : PingCategory.Fallback, PingLocation, true);
        }
    }
}
