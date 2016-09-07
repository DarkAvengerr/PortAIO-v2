using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Nidalee
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;
    using System.Linq;

    public static class Program
    {
        public static Spell Q;
        public static Spell Q1;
        public static Spell W;
        public static Spell W1;
        public static Spell W2;
        public static Spell E;
        public static Spell E1;
        public static Spell R;
        public static int SkinID;
        public static float QCD, QCDEnd;
        public static float Q1CD, Q1CDEnd;
        public static float WCD, WCDEnd;
        public static float W1CD, W1CDEnd;
        public static float ECD, ECDEnd;
        public static float E1CD, E1CDEnd;
        public static float UseWTime, ShouldRTime;
        public static Menu Menu;
        public static AIHeroClient Me;
        public static bool IsHumanizer = true;
        public static Orbwalking.Orbwalker Orbwalker;
        public static HpBarDraw HpbarDraw = new HpBarDraw();

        public static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName.ToLower() != "nidalee")
            {
                return;
            }

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 1400f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R);
            Q1 = new Spell(SpellSlot.Q, 325f);
            W1 = new Spell(SpellSlot.W, 475f);
            E1 = new Spell(SpellSlot.E, 300f);
            W2 = new Spell(SpellSlot.W, 750f);
            Q.SetSkillshot(0.25f, 37.5f, 1325f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 80f, 1450f, false, SkillshotType.SkillshotCircle);
            W1.SetSkillshot(0.25f, 300f, 1500f, false, SkillshotType.SkillshotLine);
            W2.SetSkillshot(0.25f, 300f, 1800f, false, SkillshotType.SkillshotLine);
            E1.SetSkillshot(0.25f, 260f, 1800f, false, SkillshotType.SkillshotCircle);

            SkinID = Me.SkinId;

            Menu = new Menu("Flowers' Nidalee", "Flowers' Nidalee", true);

            var OrbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);
            }

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboHumanizer", "-- Humanizer"));
                ComboMenu.AddItem(new MenuItem("ComboQHumanizer", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboWHumanizer", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRHumanizer", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboCougar", "-- Cougar"));
                ComboMenu.AddItem(new MenuItem("ComboQCougar", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboWCougar", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboECougar", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRCougar", "Use R", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassHumanizer", "-- Humanizer"));
                HarassMenu.AddItem(new MenuItem("HaraassQHumanizer", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HaraassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearCougar", "-- Cougar"));
                LaneClearMenu.AddItem(new MenuItem("LaneClearQCougar", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearWCougar", "Use W", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearWCougarCount", "Use W| Min Hit Minions Count >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(new MenuItem("LaneClearECougar", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearECougarCount", "Use E| Min Hit Minions Count >= x", true).SetValue(new Slider(3, 1, 5)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearHumanizer", "-- Humanizer"));
                JungleClearMenu.AddItem(new MenuItem("JungleClearQHumanizer", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearQMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                JungleClearMenu.AddItem(new MenuItem("JungleClearCougar", "-- Cougar"));
                JungleClearMenu.AddItem(new MenuItem("JungleClearQCougar", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearWCougar", "Use W", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearECougar", "Use E", true).SetValue(true));
            }

            var FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                FleeMenu.AddItem(new MenuItem("FleeWCougar", "Use W(Cougar)", true).SetValue(true));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealHumanizer", "-- Humanizer"));
                KillStealMenu.AddItem(new MenuItem("KillStealQHumanizer", "Use Q", true).SetValue(true));
            }

            var HealMenu = Menu.AddSubMenu(new Menu("Heal", "Heal"));
            {
                HealMenu.AddItem(new MenuItem("EnableHeal", "Use E to Heal", true).SetValue(true));
                HealMenu.AddItem(new MenuItem("HealMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(40)));
                HealMenu.AddItem(new MenuItem("HealHp", "When Player HealthPercent <= x%", true).SetValue(new Slider(40)));
                HealMenu.AddItem(new MenuItem("HealAlly", "Heal Ally?", true).SetValue(true));
                HealMenu.AddItem(new MenuItem("HealAllyHp", " When Ally HealthPercent <= x%", true).SetValue(new Slider(40)));
                foreach (var ally in HeroManager.Allies.Where(x => !x.IsMe))
                {
                    HealMenu.AddItem(new MenuItem("Heal" + ally.ChampionName, ally.ChampionName, true).SetValue(true));
                }
            }

            //var SkinMenu = Menu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
            //{
            //    SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false));
            //    SkinMenu.AddItem(new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(new StringList(new string[] { "Classic", "Snow Bunny Nidalee", "Leopard Nidalee", "French Maid Nidalee", "Pharaoh Nidalee", "Bewitching Nidalee", "Headhunter Nidalee", "Warring Kingdoms Nidalee", "Challenger Nidalee" })));
            //}

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
                DrawMenu.AddItem(new MenuItem("DrawCD", "Draw CoolDown", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("DrawTarget", "Draw Have Passive Target", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();

            Chat.Print("Flowers' Nidalee Load Succeed! Credit: NightMoon");

            if (Menu.Item("SelectPred", true).GetValue<StringList>().SelectedIndex == 3)
            {
                SPrediction.Prediction.Initialize(PredMenu);
            }

            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && !IsHumanizer)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (Menu.Item("ComboQCougar", true).GetValue<bool>() && Q.Level > 0 && Q1CD == 0 && (E1CD > 0 || E.Level == 0))
                    {
                        var target = (AIHeroClient)Args.Target;

                        if (target != null && target.IsValidTarget(Q1.Range))
                        {
                            Q1.Cast(target);
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (Menu.Item("JungleClearQCougar", true).GetValue<bool>() && Q.Level > 0 && Q1CD == 0 && (E1CD > 0 || E.Level == 0))
                    {
                        var mob = (Obj_AI_Base)Args.Target;
                        var mobs = MinionManager.GetMinions(Me.Position, Q1.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                        if (mob != null && mobs.Contains(mob) && mob.IsValidTarget(Q.Range))
                        {
                            Q1.Cast(mob);
                        }
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                switch (Args.SData.Name)
                {
                    case "JavelinToss":
                        ShouldRTime = Utils.TickCount;
                        break;
                    case "Bushwhack":
                        UseWTime = Utils.TickCount;
                        ShouldRTime = Utils.TickCount;
                        break;
                    case "PrimalSurge":
                        ShouldRTime = Utils.TickCount;
                        break;
                    case "Takedown":
                        Orbwalking.ResetAutoAttackTimer();
                        ShouldRTime = Utils.TickCount;
                        break;
                    case "Pounce":
                        ShouldRTime = Utils.TickCount;
                        break;
                    case "Swipe":
                        ShouldRTime = Utils.TickCount;
                        break;
                    case "AspectOfTheCougar":
                        Orbwalking.ResetAutoAttackTimer();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            CheckNidStatus();
            SetCoolDown();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                ComboLogic();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                HarassLogic();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClearLogic();
                JungleClearLogic();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee)
            {
                FleeLogic();
            }

            KillStealLogic();
            AutoHealLogic();
            //SkinChanceLogic();
        }

        private static void AutoHealLogic()
        {
            if (Menu.Item("EnableHeal", true).GetValue<bool>() && Me.ManaPercent >= Menu.Item("HealMana", true).GetValue<Slider>().Value && ECD == 0 && IsHumanizer)
            {
                if (Me.HealthPercent <= Menu.Item("HealHp", true).GetValue<Slider>().Value)
                {
                    E.CastOnUnit(Me, true);
                }

                if (Menu.Item("HealAlly", true).GetValue<bool>())
                {
                    foreach (var ally in HeroManager.Allies.Where(x => x.IsAlly && !x.IsMe && x.IsValidTarget(E.Range) && Menu.Item("Heal" + x.ChampionName, true).GetValue<bool>()))
                    {
                        if (ally != null && ally.HealthPercent <= Menu.Item("HealAllyHp", true).GetValue<Slider>().Value)
                        {
                            if (ally.CountEnemiesInRange(1000) > 0 && ally.IsValidTarget(E.Range))
                            {
                                E.CastOnUnit(ally, true);
                            }
                        }
                    }
                }
            }
        }

        private static void CheckNidStatus()
        {
            if (Me.CharData.BaseSkinName.ToLower() == "nidaleecougar")
            {
                IsHumanizer = false;
            }
            else if (Me.CharData.BaseSkinName.ToLower() == "nidalee")
            {
                IsHumanizer = true;
            }
        }

        private static void SetCoolDown()
        {
            if (IsHumanizer)
            {
                QCDEnd = Q.Instance.CooldownExpires;
                WCDEnd = W.Instance.CooldownExpires;
                ECDEnd = E.Instance.CooldownExpires;
            }
            else
            {
                Q1CDEnd = Q.Instance.CooldownExpires;
                W1CDEnd = W.Instance.CooldownExpires;
                E1CDEnd = E.Instance.CooldownExpires;
            }

            QCD = Q.Level > 0 ? CheckCD(QCDEnd) : -1;
            WCD = W.Level > 0 ? CheckCD(WCDEnd) : -1;
            ECD = E.Level > 0 ? CheckCD(ECDEnd) : -1;
            Q1CD = Q.Level > 0 ? CheckCD(Q1CDEnd) : -1;
            W1CD = W.Level > 0 ? CheckCD(W1CDEnd) : -1;
            E1CD = E.Level > 0 ? CheckCD(E1CDEnd) : -1;
        }

        private static void ComboLogic()
        {
            if (IsHumanizer)
            {
                if (Menu.Item("ComboQHumanizer", true).GetValue<bool>() && Q.Level > 0 && QCD == 0)
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget(Q.Range))
                    {
                        Q.CastTo(target);
                    }
                }

                if (Menu.Item("ComboWHumanizer", true).GetValue<bool>() && W.Level > 0 && WCD == 0)
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget(W.Range))
                    {
                        if (target.IsMelee && target.IsFacing(Me) && target.DistanceToPlayer() < 275 && Utils.TickCount - UseWTime > 1500)
                        {
                            W.Cast(Me.ServerPosition);
                        }
                        else if (target.IsFacing(Me) && Environment.TickCount - UseWTime > 2000)
                        {
                            W.CastTo(target);
                        }
                    }
                }

                if (Menu.Item("ComboRHumanizer", true).GetValue<bool>() && R.IsReady() && Utils.TickCount - ShouldRTime > 500)
                {
                    var target = TargetSelector.GetTarget(1400f, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget())
                    {
                        if (HavePassive(target) && target.IsValidTarget(850f) && Q1CD == 0 && W1CD == 0 && E1CD == 0)
                        {
                            R.Cast();
                            return;
                        }

                        if (!HavePassive(target) && target.IsValidTarget(W.Range) && QCD > 2 && (!Menu.Item("ComboWHumanizer", true).GetValue<bool>() || (Menu.Item("ComboWHumanizer", true).GetValue<bool>() && WCD > 2) && Q1CD == 0 && W1CD == 0 && E1CD == 0))
                        {
                            R.Cast();
                            return;
                        }
                    }
                }
            }
            else if (!IsHumanizer)
            {
                if (Menu.Item("ComboECougar", true).GetValue<bool>() && E.Level > 0 && E1CD == 0)
                {
                    var target = TargetSelector.GetTarget(E1.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget(E1.Range))
                    {
                        E1.Cast(target, true, true);
                    }
                }

                if (Menu.Item("ComboQCougar", true).GetValue<bool>() && Q.Level > 0 && Q1CD == 0 && E1CD > 0)
                {
                    var target = TargetSelector.GetTarget(Q1.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget(Q1.Range))
                    {
                        Q1.Cast(target);
                    }
                }

                if (Menu.Item("ComboWCougar", true).GetValue<bool>() && W.Level > 0 && W1CD == 0)
                {
                    var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget(900) && (target.DistanceToPlayer() > 180 || target.Health + target.MagicShield < GetW1Damage(target)))
                    {
                        if (HavePassive(target) && target.IsValidTarget(W2.Range + 50))
                        {
                            W2.Cast(target.Position, true);
                        }
                        else if (!HavePassive(target) && target.IsValidTarget(W1.Range + 50))
                        {
                            W1.Cast(target.Position, true);
                        }
                    }
                }

                if (Menu.Item("ComboRCougar", true).GetValue<bool>() && R.IsReady() && Utils.TickCount - ShouldRTime > 500)
                {
                    var target = TargetSelector.GetTarget(1400f, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget())
                    {
                        if (QCD == 0 && !HavePassive(target) && target.DistanceToPlayer() > 900)
                        {
                            R.Cast();
                            return;
                        }

                        if (ECD == 0 && Me.HealthPercent <= Me.MaxHealth * 0.45)
                        {
                            R.Cast();
                            return;
                        }

                        if (W1CD == 0 && Q1CD > 0 && E1CD > 0 && !HavePassive(target))
                        {
                            R.Cast();
                            return;
                        }

                        if (QCD == 0 && HavePassive(target))
                        {
                            R.Cast();
                            return;
                        }
                    }
                }
            }
        }

        private static void HarassLogic()
        {
            if (Me.ManaPercent >= Menu.Item("HaraassMana", true).GetValue<Slider>().Value)
            {
                if (IsHumanizer)
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget())
                    {
                        if (Menu.Item("HaraassQHumanizer", true).GetValue<bool>() && QCD == 0 && target.IsValidTarget(Q.Range))
                        {
                            Q.CastTo(target);
                        }
                    }
                }
            }
        }

        private static void LaneClearLogic()
        {
            if (!IsHumanizer)
            {
                var minions = MinionManager.GetMinions(Me.Position, 500, MinionTypes.All, MinionTeam.Enemy);

                if (minions.Count() > 0)
                {
                    var minion = minions.Find(x => x.Health < GetQ1Damage(x));

                    if (Menu.Item("LaneClearQCougar", true).GetValue<bool>() && Q1CD == 0 && minion != null && minion.IsValidTarget(Q1.Range))
                    {
                        Q1.Cast(minion);
                    }

                    if (Menu.Item("LaneClearWCougar", true).GetValue<bool>() && W1CD == 0)
                    {
                        var W1Farm = W1.GetCircularFarmLocation(minions, W1.Width);

                        if (W1Farm.MinionsHit >= Menu.Item("LaneClearWCougarCount", true).GetValue<Slider>().Value)
                        {
                            W1.Cast(W1Farm.Position);
                        }
                    }

                    if (Menu.Item("LaneClearECougar", true).GetValue<bool>() && E1CD == 0)
                    {
                        var E1Farm = E1.GetCircularFarmLocation(minions, E1.Width);

                        if (E1Farm.MinionsHit >= Menu.Item("LaneClearECougarCount", true).GetValue<Slider>().Value)
                        {
                            W1.Cast(E1Farm.Position);
                        }
                    }
                }
            }
        }

        private static void JungleClearLogic()
        {
            var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count() > 0)
            {
                var bigmob = mobs.Where(x => !x.CharData.BaseSkinName.ToLower().Contains("mini"));
                var mob = mobs.FirstOrDefault();
                var CanCastHumanizerQ = Menu.Item("JungleClearQHumanizer", true).GetValue<bool>() && QCD == 0 && Me.ManaPercent >= Menu.Item("JungleClearQMana", true).GetValue<Slider>().Value;

                if (IsHumanizer)
                {
                    if (CanCastHumanizerQ)
                    {
                        if (bigmob.Count() > 0)
                        {
                            Q.CastTo(bigmob.FirstOrDefault(), true);
                        }
                        else
                        {
                            Q.CastTo(mob, true);
                        }
                    }

                    if (!CanCastHumanizerQ)
                    {
                        R.Cast();
                    }
                }
                else if (!IsHumanizer)
                {
                    if (Menu.Item("JungleClearWCougar", true).GetValue<bool>() && W1CD == 0)
                    {
                        if (HavePassive(mob) && mob.IsValidTarget(W2.Range))
                        {
                            W2.Cast(mob.Position, true);
                        }
                        else if (!HavePassive(mob) && mob.IsValidTarget(W1.Range))
                        {
                            W1.Cast(mob.Position, true);
                        }
                    }

                    if (Menu.Item("JungleClearECougar", true).GetValue<bool>() && E1CD == 0 && mob.IsValidTarget(E1.Range))
                    {
                        E1.Cast(mob, true);
                    }

                    if (Menu.Item("JungleClearQCougar", true).GetValue<bool>() && Q1CD == 0 && mob.IsValidTarget(Q1.Range))
                    {
                        Q1.Cast(mob, true);
                    }

                    if (CanCastHumanizerQ)
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void FleeLogic()
        {
            if (IsHumanizer && R.IsReady())
            {
                R.Cast();
            }
            else if (!IsHumanizer)
            {
                if (W1CD == 0 && Menu.Item("FleeWCougar", true).GetValue<bool>())
                {
                    W1.Cast(Game.CursorPos);
                }
            }
        }

        private static void KillStealLogic()
        {
            if (Menu.Item("KillStealQHumanizer", true).GetValue<bool>() && QCD == 0)
            {
                foreach (var target in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.IsValidTarget(Q.Range) && x.Health + x.MagicShield < GetQDamage(x)))
                {
                    if (target != null && target.IsValidTarget(Q.Range))
                    {
                        Q.CastTo(target);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead || Shop.IsOpen || MenuGUI.IsChatOpen)
            {
                return;
            }

            if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Me.Position, IsHumanizer ? Q.Range : Q1.Range, System.Drawing.Color.Green, 1);
            }

            if (Menu.Item("DrawW", true).GetValue<bool>() && W.Level > 0)
            {
                if (Me.HasBuff("NidaleePassiveHunting") && !IsHumanizer)
                {
                    Render.Circle.DrawCircle(Me.Position, W2.Range, System.Drawing.Color.Green, 1);
                }
                else
                    Render.Circle.DrawCircle(Me.Position, IsHumanizer ? W.Range : W1.Range, System.Drawing.Color.Green, 1);
            }

            if (Menu.Item("DrawE", true).GetValue<bool>() && E.Level > 0)
            {
                Render.Circle.DrawCircle(Me.Position, IsHumanizer ? E.Range : E1.Range, System.Drawing.Color.Green, 1);
            }

            if (Menu.Item("DrawCD", true).GetValue<bool>())
            {
                string msg = " ";
                int QCoolDown = (int)QCD == -1 ? 0 : (int)QCD;
                int WCoolDown = (int)WCD == -1 ? 0 : (int)WCD;
                int ECoolDown = (int)ECD == -1 ? 0 : (int)ECD;
                int Q1CoolDown = (int)Q1CD == -1 ? 0 : (int)Q1CD;
                int W1CoolDown = (int)W1CD == -1 ? 0 : (int)W1CD;
                int E1CoolDown = (int)E1CD == -1 ? 0 : (int)E1CD;

                if (!IsHumanizer)
                {
                    msg = "Q: " + QCoolDown + "   W: " + WCoolDown + "   E: " + ECoolDown;
                    Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 30, System.Drawing.Color.Orange, msg);
                }
                else
                {
                    msg = "Q: " + Q1CoolDown + "   W: " + W1CoolDown + "   E: " + E1CoolDown;
                    Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 30, System.Drawing.Color.SkyBlue, msg);
                }
            }

            if (Menu.Item("DrawTarget", true).GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget() && HavePassive(x)))
                {
                    if (target != null && target.IsValid)
                    {
                        Render.Circle.DrawCircle(target.Position, target.BoundingRadius + 100, System.Drawing.Color.Red, 3);
                    }
                }
            }

            if (Menu.Item("DrawDamage", true).GetValue<bool>())
            {
                foreach (var x in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = x;
                    HpBarDraw.DrawDmg(ComboDamage(x), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        private static float ComboDamage(AIHeroClient target)
        {
            double damage = GetQDamage(target) + GetQ1Damage(target) + GetWDamage(target) + GetW1Damage(target) + GetE1Damage(target) + Me.GetAutoAttackDamage(target);

            return (float)damage;
        }

        private static float CheckCD(float Expires)
        {
            var time = Expires - Game.Time;

            if (time < 0)
            {
                time = 0;
            }

            return time;
        }

        private static bool HavePassive(Obj_AI_Base target)
        {
            return target.HasBuff("NidaleePassiveHunted");
        }

        private static double GetQDamage(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                if (Q.Level > 0 && QCD == 0)
                {
                    return Me.CalcDamage
                        (target, Damage.DamageType.Magical,
                        new double[] { 60, 77.5, 95, 112.5, 130 }[Q.Level - 1] + 
                        0.4 * Me.TotalMagicalDamage);
                }
                else
                {
                    return 0d;
                }
            }
            else
            {
                return 0d;
            }
        }

        private static double GetQ1Damage(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                if (Q.Level > 0 && Q1CD == 0)
                {
                    var dmg = (new double[] { 4, 20, 50, 90 }[R.Level - 1] + 0.36 * Me.TotalMagicalDamage + 0.75 * (Me.BaseAttackDamage + Me.FlatPhysicalDamageMod)) * ((target.MaxHealth - target.Health) / target.MaxHealth * 1.5 + 1);

                    dmg *= target.HasBuff("nidaleepassivehunted") ? 1.33 : 1.0;

                    return dmg;
                }
                else
                {
                    return 0d;
                }
            }
            else
            {
                return 0d;
            }
        }

        private static double GetWDamage(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                if (W.Level > 0 && WCD == 0)
                {
                    return Me.CalcDamage(target, Damage.DamageType.Magical, new double[] { 40, 80, 120, 160, 200 }[W.Level - 1] + 0.2 * Me.TotalMagicalDamage);
                }
                else
                {
                    return 0d;
                }
            }
            else
            {
                return 0d;
            }
        }

        private static double GetW1Damage(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                if (W.Level > 0 && W1CD == 0)
                {
                    return Me.CalcDamage(target, Damage.DamageType.Magical, new double[] { 60, 110, 160, 210 }[R.Level - 1] + 0.3 * Me.TotalMagicalDamage);
                }
                else
                {
                    return 0d;
                }
            }
            else
            {
                return 0d;
            }
        }

        private static double GetE1Damage(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                if (E.Level > 0 && E1CD == 0)
                {
                    return Me.CalcDamage(target, Damage.DamageType.Magical, new double[] { 70, 130, 190, 250 }[R.Level - 1] + 0.45 * Me.TotalMagicalDamage);
                }
                else
                {
                    return 0d;
                }
            }
            else
            {
                return 0d;
            }
        }
    }
}
