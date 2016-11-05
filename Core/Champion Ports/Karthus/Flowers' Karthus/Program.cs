using EloBuddy; 
 using LeagueSharp.Common; 
 using EloBuddy; 
 using LeagueSharp.Common; 
 using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Karthus
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public static class Program
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static int SkinID;
        public static int LastPingT;
        public static int LastShowNoit;
        public static int CastSpellFarmTime;
        public static Menu Menu;
        public static AIHeroClient Me;
        public static Vector2 PingLocation;
        public static Orbwalking.Orbwalker Orbwalker;
        public static HpBarDraw HpbarDraw = new HpBarDraw();

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "karthus")
            {
                return;
            }

            var enemyTracker = new EnemyTracker();

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 880f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 550f);
            R = new Spell(SpellSlot.R, 20000f);

            Q.SetSkillshot(0.95f, 145f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.5f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(1.0f, 505f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(3.0f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SkinID = Me.SkinId;

            Menu = new Menu("Flowers' Karthus", "Flowers' Karthus", true);

            var OrbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);
            }

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboEMana", "Use E|When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                ComboMenu.AddItem(new MenuItem("ComboDisable", "Disable Auto Attack?", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HaraassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HaraassQLH", "Use Q| Last Hit", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HaraassE", "Use E| Last Hit", true).SetValue(false));
                HarassMenu.AddItem(new MenuItem("AutoHarass", "Auto Harass?", true).SetValue(new KeyBind('T', KeyBindType.Toggle))).Permashow();
                HarassMenu.AddItem(new MenuItem("HaraassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearECount", "Use E| Min Hit Minions Count >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var LastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                LastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q", true).SetValue(true));
                LastHitMenu.AddItem(new MenuItem("LastHitMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var UltMenu = Menu.AddSubMenu(new Menu("Ult Settings", "Ult Settings"));
            {
                UltMenu.AddItem(new MenuItem("TeamFightR", "Use R TeamFight", true).SetValue(true));
                UltMenu.AddItem(new MenuItem("KillStealR", "Use R KillSteal", true).SetValue(true));
                UltMenu.AddItem(new MenuItem("KillStealRCount", "Use R KillSteal Count >=", true).SetValue(new Slider(2, 1, 5)));
                foreach (var target in HeroManager.Enemies)
                {
                    UltMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(), "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(new MenuItem("AutoZombie", "Auto Zombie Mode?", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("PingKill", "Auto Ping Kill Target", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("NormalPingKill", "Normal Ping?", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("NotificationKill", "Notification Kill Target", true).SetValue(true));
            }

            var SkinMenu = Menu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
            {
                SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged += EnbaleSkin;
                SkinMenu.AddItem(new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(new StringList(new[] { "Classic Karthus", "Phantom Karthus", "Statue of Karthus", "Grim Reaper Karthus", "Pentakill Karthus", "Fnatic Karthus" })));
            }

            var PredMenu = Menu.AddSubMenu(new Menu("Prediction", "Prediction"));
            {
                PredMenu.AddItem(new MenuItem("SelectPred", "Select Prediction: ", true).SetValue(new StringList(new[] { "Common Prediction", "OKTW Prediction", "SDK Prediction", "SPrediction(Need F5 Reload)", "xcsoft AIO Prediction" }, 1)));
                PredMenu.AddItem(new MenuItem("AboutCommonPred", "Common Prediction -> LeagueSharp.Commmon Prediction"));
                PredMenu.AddItem(new MenuItem("AboutOKTWPred", "OKTW Prediction -> Sebby' Prediction"));
                PredMenu.AddItem(new MenuItem("AboutSDKPred", "SDK Prediction -> LeagueSharp.SDKEx Prediction"));
                PredMenu.AddItem(new MenuItem("AboutSPred", "SPrediction -> Shine' Prediction"));
                PredMenu.AddItem(new MenuItem("AboutxcsoftAIOPred", "xcsoft AIO Prediction -> xcsoft ALL In One Prediction"));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("DrawKillSteal", "Draw KillSteal target", true).SetValue(true));
            }

            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();

            Chat.Print("Flowers' Karthus Load Succeed! Credit: NightMoon");

            if (Menu.Item("SelectPred", true).GetValue<StringList>().SelectedIndex == 3)
            {
                SPrediction.Prediction.Initialize(PredMenu);
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                var castedSlot = ObjectManager.Player.GetSpellSlot(Args.SData.Name);

                if (castedSlot == SpellSlot.Q || castedSlot == SpellSlot.E)
                {
                    CastSpellFarmTime = Utils.TickCount;
                }
            }
        }

        private static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                //////ObjectManager.//////Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            foreach (var enemy in HeroManager.Enemies.Where(h => R.IsReady() && h.IsValidTarget() && GetRDamage(h) > h.Health + h.MagicShield))
            {
                if (Menu.Item("PingKill", true).GetValue<bool>())
                {
                    Ping(enemy.Position.To2D());
                }

                if (Menu.Item("NotificationKill", true).GetValue<bool>() && Utils.TickCount - LastShowNoit > 10000)
                {
                    Notifications.AddNotification(new Notification("R Kill: " + enemy.ChampionName + "!", 3000, true).SetTextColor(System.Drawing.Color.FromArgb(255, 0, 0)));
                    LastShowNoit = Utils.TickCount;
                }
            }

            if (Me.IsRecalling())
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                ComboLogic();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || Menu.Item("AutoHarass", true).GetValue<KeyBind>().Active)
            {
                HarassLogic();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClearLogic();
                JungleClearLogic();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHitLogic();
            }

            AutoZombie();
            TeamFightUlt();
            KillStealLogic();

            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
                //////ObjectManager.//////Player.SetSkin(ObjectManager.Player.ChampionName, Menu.Item("SelectSkin", true).GetValue<StringList>().SelectedIndex);
            }

            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                Orbwalker.SetAttack(true);
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None && !Me.IsZombie && E.Instance.ToggleState == 2 &&
                !Me.InFountain())
            {
                E.Cast();
            }
        }

        private static void ComboLogic()
        {
            Orbwalker.SetAttack(!Menu.Item("ComboDisable", true).GetValue<bool>());

            if (Me.CountEnemiesInRange(E.Range) == 0 && E.Instance.ToggleState == 2)
            {
                E.Cast();
            }

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target.IsValidTarget(Q.Range) && !target.IsZombie)
            {
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.CastTo(target, true);
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (Me.ManaPercent >= Menu.Item("ComboEMana", true).GetValue<Slider>().Value)
                    {
                        if (E.Instance.ToggleState != 2)
                        {
                            E.Cast();
                        }
                    }
                    else if (E.Instance.ToggleState == 2)
                    {
                        E.Cast();
                    }
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.CastTo(target);
                }
            }
        }

        private static void HarassLogic()
        {
            if (Me.ManaPercent >= Menu.Item("HaraassMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("HaraassQLH", true).GetValue<bool>())
                {
                    var minions = MinionManager.GetMinions(Q.Range)
                        .Where(x => HealthPrediction.GetHealthPrediction(x, 950) * 0.9 < GetQDamage(x) &&
                         HealthPrediction.GetHealthPrediction(x, 950) * 0.9 > 0);

                    if (minions.Any())
                    {
                        var min = minions.FirstOrDefault();

                        if (Utils.TickCount - CastSpellFarmTime > 1500)
                        {
                            Q.Cast(min, true);
                        }
                    }
                }

                if (Menu.Item("HaraassQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget())
                    {
                        if (target.IsValidTarget(Q.Range))
                        {
                            Q.CastTo(target, true);
                        }
                    }
                }

                if (Menu.Item("HaraassE", true).GetValue<bool>() && E.IsReady())
                {
                    var minions =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                    x.IsEnemy && x.Name != "gangplankbarrel" && x.Name != "WardCorpse" &&
                                    x.Name != "jarvanivstandard" && !MinionManager.IsWard(x) && x.Distance(Me) < E.Range &&
                                    x.Health < GetEDamage(x)).ToList();

                    if (minions.Count(x => x.Distance(Me) < E.Range) > 0)
                    {
                        if (Utils.TickCount - CastSpellFarmTime > 1500)
                        {
                            if (E.Instance.ToggleState != 2)
                            {
                                E.Cast();
                            }
                        }
                    }
                    else
                    {
                        if (E.Instance.ToggleState == 2)
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }

        private static void LaneClearLogic()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value || Me.IsZombie)
            {
                var minions = MinionManager.GetMinions(Me.Position, 500);

                if (!minions.Any())
                {
                    return;
                }

                var minion = minions.Find(x => x.Health > Me.GetAutoAttackDamage(x));

                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady() && minion != null && minion.IsValidTarget(Q.Range))
                {
                    Q.Cast(minion, true);
                }

                if (Menu.Item("LaneClearE", true).GetValue<bool>() && E.IsReady() &&
                    minions.Count(x => x.Distance(Me) <= E.Range) >=
                    Menu.Item("LaneClearECount", true).GetValue<Slider>().Value && E.Instance.ToggleState != 2)
                {
                    E.Cast();
                }
                else if (minions.Count(x => x.Distance(Me) <= E.Range) == 0 && E.Instance.ToggleState == 2)
                {
                    E.Cast();
                }
            }
            else if (E.Instance.ToggleState == 2)
            {
                E.Cast();
            }
        }

        private static void JungleClearLogic()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value || Me.IsZombie)
            {
                var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                if (!mobs.Any())
                {
                    return;
                }

                var mob = mobs.FirstOrDefault();

                if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Q.CastTo(mob, true);
                }

                if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady() && E.Instance.ToggleState != 2 &&
                    mob.IsValidTarget(E.Range))
                {
                    E.Cast();
                }
                else if (mobs.Count(x => x.Distance(Me) <= E.Range) == 0 && E.Instance.ToggleState == 2)
                {
                    E.Cast();
                }
            }
            else if (E.Instance.ToggleState == 2)
            {
                E.Cast();
            }
        }

        private static void LastHitLogic()
        {
            if (Me.ManaPercent >= Menu.Item("LastHitMana", true).GetValue<Slider>().Value || Me.IsZombie)
            {
                if (Menu.Item("LastHitQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var minions = MinionManager.GetMinions(Q.Range)
                        .Where(x => HealthPrediction.GetHealthPrediction(x, 950) * 0.9 < GetQDamage(x) &&
                        HealthPrediction.GetHealthPrediction(x, 950) * 0.9 > 0);

                    if (minions.Any())
                    {
                        var min = minions.FirstOrDefault();

                        Q.Cast(min, true);
                    }
                }
            }
        }

        private static void AutoZombie()
        {
            if (Menu.Item("AutoZombie", true).GetValue<bool>() && Me.IsZombie)
            {
                if (E.Instance.ToggleState != 2)
                {
                    E.Cast();
                }

                if (Me.CountEnemiesInRange(Q.Range) > 0)
                {
                    ComboLogic();
                }
                else
                {
                    LaneClearLogic();
                    JungleClearLogic();
                }
            }
        }

        private static void TeamFightUlt()
        {
            var CanCastR = false;

            if (Menu.Item("TeamFightR", true).GetValue<bool>() && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies)
                {
                    if (!target.IsValidTarget())
                    {
                        continue;
                    }

                    if (target.IsDead)
                    {
                        continue;
                    }

                    if (target.IsZombie)
                    {
                        continue;
                    }

                    if (target.HasBuff("KindredRNoDeathBuff"))
                    {
                        continue;
                    }

                    if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                    {
                        continue;
                    }

                    if (target.HasBuff("JudicatorIntervention"))
                    {
                        continue;
                    }

                    if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                    {
                        continue;
                    }

                    if (target.HasBuff("FioraW"))
                    {
                        continue;
                    }

                    if (target.CountAlliesInRange(850) > 1 && target.CountEnemiesInRange(850) <= 2 &&
                        target.Health + target.MagicShield < GetRDamage(target) * 2 && Me.IsZombie)
                    {
                        CanCastR = true;
                    }

                    //if (Me.CountEnemiesInRange(850) > 0 && target.Health < GetRDamage(target) + GetQDamage(target) * 3)
                    //{
                    //    CanCastR = true;
                    //}

                    if (Me.CountEnemiesInRange(1000) >= 3 && Me.CountAlliesInRange(850) <= 3)
                    {
                        CanCastR = true;
                    }
                }

                if (Me.IsZombie)
                {
                    var passivetime = Me.GetBuff("KarthusDeathDefiedBuff").EndTime;

                    if (passivetime > 3 && passivetime < 4)
                    {
                        if (CanCastR)
                        {
                            R.Cast();
                        }
                    }
                }
                else if (!Me.IsZombie)
                {
                    if (CanCastR && Me.CountEnemiesInRange(800) == 0)
                    {
                        R.Cast();
                    }
                }
            }
            else
            {
                CanCastR = false;
            }
        }

        private static void KillStealLogic()
        {
            if (HeroManager.Enemies == null)
            {
                return;
            }

            KillStealRLogic();

            foreach (var target in HeroManager.Enemies)
            {
                if (target.IsDead || target.IsZombie)
                {
                    continue;
                }

                if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady() && target.Health + target.MagicShield < GetQDamage(target))
                {
                    Q.CastTo(target, true);
                }
            }
        }

        private static void KillStealRLogic()
        {
            if (Menu.Item("KillStealR", true).GetValue<bool>() && R.IsReady())
            {
                var targets = new List<AIHeroClient>();

                foreach (var ult in EnemyTracker.enemyInfo)
                {
                    if (ult.target.IsDead)
                    {
                        continue;
                    }

                    if (ult.target.IsZombie)
                    {
                        continue;
                    }

                    if (ult.target.HasBuff("KindredRNoDeathBuff"))
                    {
                        continue;
                    }

                    if (ult.target.HasBuff("UndyingRage") && ult.target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                    {
                        continue;
                    }

                    if (ult.target.HasBuff("JudicatorIntervention"))
                    {
                        continue;
                    }

                    if (ult.target.HasBuff("ChronoShift") && ult.target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                    {
                        continue;
                    }

                    if (ult.target.HasBuff("FioraW"))
                    {
                        continue;
                    }

                    if (!Menu.Item("KillStealR" + ult.target.ChampionName.ToLower(), true).GetValue<bool>())
                    {
                        continue;
                    }

                    if (ult.target.IsVisible &&
                        R.GetDamage(ult.target) >
                        ult.target.Health + ult.target.MagicShield + ult.target.HPRegenRate*2)
                    {
                        targets.Add(ult.target);
                    }

                    if (!ult.target.IsVisible && Utils.TickCount > ult.LastSeen + 5000 &&
                        R.GetDamage(ult.target) > EnemyTracker.GetTargetHealth(ult, R.Delay))
                    {
                        targets.Add(ult.target);
                    }

                    if (!ult.target.IsVisible && Utils.TickCount < ult.LastSeen + 5000 && targets.Contains(ult.target))
                    {
                        targets.Remove(ult.target);
                    }
                }

                if (targets.Count >= Menu.Item("KillStealRCount", true).GetValue<Slider>().Value)
                {
                    if (!Me.IsZombie && Me.CountEnemiesInRange(800) > 0)
                    {
                        return;
                    }

                    R.Cast();
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.Level > 0)
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.FromArgb(253, 164, 17), 1);
                }

                if (Menu.Item("DrawW", true).GetValue<bool>() && W.Level > 0)
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.FromArgb(9, 253, 242), 1);
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.Level > 0)
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.FromArgb(143, 16, 146), 1);
                }

                if (Menu.Item("DrawDamage", true).GetValue<bool>())
                {
                    foreach (
                        var x in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg(ComboDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
                }

                if (Menu.Item("DrawKillSteal", true).GetValue<bool>())
                {
                    Drawing.DrawText(Drawing.Width  - 150, Drawing.Height - 500, System.Drawing.Color.Yellow, "Ult Kill Target: ");

                    var targets = new List<AIHeroClient>();

                    foreach (var ult in EnemyTracker.enemyInfo)
                    {
                        if (ult.target.IsDead)
                        {
                            continue;
                        }

                        if (ult.target.IsZombie)
                        {
                            continue;
                        }

                        if (ult.target.HasBuff("KindredRNoDeathBuff"))
                        {
                            continue;
                        }

                        if (ult.target.HasBuff("UndyingRage") && ult.target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                        {
                            continue;
                        }

                        if (ult.target.HasBuff("JudicatorIntervention"))
                        {
                            continue;
                        }

                        if (ult.target.HasBuff("ChronoShift") && ult.target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                        {
                            continue;
                        }

                        if (ult.target.HasBuff("FioraW"))
                        {
                            continue;
                        }

                        if (!Menu.Item("KillStealR" + ult.target.ChampionName.ToLower(), true).GetValue<bool>())
                        {
                            continue;
                        }

                        if (ult.target.IsVisible &&
                            R.GetDamage(ult.target) >
                            ult.target.Health + ult.target.MagicShield + ult.target.HPRegenRate * 2)
                        {
                            targets.Add(ult.target);
                        }

                        if (!ult.target.IsVisible && Utils.TickCount > ult.LastSeen + 5000 &&
                            R.GetDamage(ult.target) > EnemyTracker.GetTargetHealth(ult, R.Delay))
                        {
                            targets.Add(ult.target);
                        }

                        if (!ult.target.IsVisible && Utils.TickCount < ult.LastSeen + 5000 && targets.Contains(ult.target))
                        {
                            targets.Remove(ult.target);
                        }
                    }

                    if (targets.Count > 0)
                    {
                        for (var i = 0; i <= targets.Count; i++)
                        {
                            Drawing.DrawText(Drawing.Width - 150, Drawing.Height - 470 + i * 30, System.Drawing.Color.Red, "   " + targets.ElementAt(i).ChampionName);
                        }
                    }
                }
            }
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

        private static float ComboDamage(AIHeroClient target)
        {
            var damage = GetQDamage(target) + GetWDamage(target) + GetEDamage(target) + GetRDamage(target);

            return (float)damage;
        }

        private static double GetQDamage(Obj_AI_Base target)
        {
            return !target.IsValidTarget() ? 0d : Q.GetDamage(target);
        }

        private static double GetWDamage(Obj_AI_Base target)
        {
            return !target.IsValidTarget() ? 0d : W.GetDamage(target);
        }

        private static double GetEDamage(Obj_AI_Base target)
        {
            return !target.IsValidTarget() ? 0d : E.GetDamage(target);
        }

        private static double GetRDamage(Obj_AI_Base target)
        {
            return !target.IsValidTarget() ? 0d : R.GetDamage(target);
        }
    }
}
