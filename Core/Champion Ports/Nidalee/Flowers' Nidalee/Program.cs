using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Nidalee
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;
    using System.Linq;
    using System.Drawing;

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
        public static float Qcd, QcdEnd;
        public static float Q1Cd, Q1CdEnd;
        public static float Wcd, WcdEnd;
        public static float W1Cd, W1CdEnd;
        public static float Ecd, EcdEnd;
        public static float E1Cd, E1CdEnd;
        public static float UseWTime, ShouldRTime;
        public static Menu Menu;
        public static AIHeroClient Me;
        public static bool IsHumanizer = true;
        public static Orbwalking.Orbwalker Orbwalker;
        public static HpBarDraw HpbarDraw = new HpBarDraw();

        public static void Main(string[] Args)
        {
            OnGameLoad();
        }

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
            E1.SetSkillshot(0.25f, 260f, 1800f, false, SkillshotType.SkillshotCone);

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
                JungleClearMenu.AddItem(new MenuItem("JungleClearWHumanizer", "Use W", true).SetValue(true));
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
                HealMenu.AddItem(new MenuItem("AutoSwitch", "Auto Switch?", true).SetValue(true));
                HealMenu.AddItem(new MenuItem("HealMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(40)));
                HealMenu.AddItem(new MenuItem("HealHp", "When Player HealthPercent <= x%", true).SetValue(new Slider(40)));
                HealMenu.AddItem(new MenuItem("HealAlly", "Heal Ally?", true).SetValue(true));
                HealMenu.AddItem(new MenuItem("HealAllyHp", " When Ally HealthPercent <= x%", true).SetValue(new Slider(40)));
                foreach (var ally in HeroManager.Allies.Where(x => !x.IsMe))
                {
                    HealMenu.AddItem(new MenuItem("HealAllyName" + ally.ChampionName.ToLower(), ally.ChampionName, true).SetValue(true));
                }
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
                        new StringList(new[] { "VeryHigh", "High", "Medium", "Low" })));
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
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(new StringList(new [] {"Only Humanizer", "Only Cougar", "Now Status", "Both", "Off" }, 3)));
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
            if (sender.IsMe && !IsHumanizer && Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        var hero = (AIHeroClient)Args.Target;

                        if (hero != null)
                        {
                            if (Menu.Item("ComboECougar", true).GetValue<bool>() && E.Level > 0 && E1Cd == 0 && hero.IsValidTarget(E1.Range))
                            {
                                E1.Cast(hero.Position, true);
                            }
                            else if (Menu.Item("ComboQCougar", true).GetValue<bool>() && Q.Level > 0 && Q1Cd == 0 && hero.IsValidTarget(Q1.Range))
                            {
                                Q1.Cast(hero);
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        var mob = (Obj_AI_Base)Args.Target;
                        var mobs = MinionManager.GetMinions(Me.Position, Q1.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                        if (mob != null && mobs.Contains(mob))
                        {
                            if (Menu.Item("JungleClearECougar", true).GetValue<bool>() && E1Cd == 0 && mob.IsValidTarget(E1.Range))
                            {
                                E1.Cast(mob.Position, true);
                            }
                            else if (Menu.Item("JungleClearQCougar", true).GetValue<bool>() && Q.Level > 0 && Q1Cd == 0 && mob.IsValidTarget(Q1.Range))
                            {
                                Q1.Cast(mob);
                            }
                        }
                        break;
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
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            switch (Me.CharData.BaseSkinName.ToLower())
            {
                case "nidaleecougar":
                    IsHumanizer = false;
                    break;
                case "nidalee":
                    IsHumanizer = true;
                    break;
            }

            SetCoolDown();
            KillStealLogic();
            AutoHealLogic();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ComboLogic();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    HarassLogic();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClearLogic();
                    JungleClearLogic();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    FleeLogic();
                    break;
            }
        }

        private static void SetCoolDown()
        {
            if (IsHumanizer)
            {
                QcdEnd = Q.Instance.CooldownExpires;
                WcdEnd = W.Instance.CooldownExpires;
                EcdEnd = E.Instance.CooldownExpires;
            }
            else
            {
                Q1CdEnd = Q.Instance.CooldownExpires;
                W1CdEnd = W.Instance.CooldownExpires;
                E1CdEnd = E.Instance.CooldownExpires;
            }

            Qcd = Q.Level > 0 ? CheckCD(QcdEnd) : -1;
            Wcd = W.Level > 0 ? CheckCD(WcdEnd) : -1;
            Ecd = E.Level > 0 ? CheckCD(EcdEnd) : -1;
            Q1Cd = Q.Level > 0 ? CheckCD(Q1CdEnd) : -1;
            W1Cd = W.Level > 0 ? CheckCD(W1CdEnd) : -1;
            E1Cd = E.Level > 0 ? CheckCD(E1CdEnd) : -1;
        }

        private static void ComboLogic()
        {
            if (IsHumanizer)
            {
                if (Menu.Item("ComboQHumanizer", true).GetValue<bool>() && Q.Level > 0 && Qcd == 0)
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget(Q.Range) && !target.IsDead && !target.IsZombie)
                    {
                        Q.CastTo(target);
                    }
                }

                if (Menu.Item("ComboWHumanizer", true).GetValue<bool>() && W.Level > 0 && Wcd == 0)
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

                    if (!target.IsValidTarget() || target.IsDead || target.IsZombie || !target.IsValidTarget())
                    {
                        return;
                    }

                    if (HavePassive(target) && target.IsValidTarget(850f) && Q1Cd == 0 && W1Cd == 0 && E1Cd == 0)
                    {
                        R.Cast();
                        return;
                    }

                    if (!HavePassive(target) && target.IsValidTarget(W.Range) && Qcd > 2 && (!Menu.Item("ComboWHumanizer", true).GetValue<bool>() || Menu.Item("ComboWHumanizer", true).GetValue<bool>() && Wcd > 2 && Q1Cd == 0 && W1Cd == 0 && E1Cd == 0))
                    {
                        R.Cast();
                    }
                }
            }
            else if (!IsHumanizer)
            {
                if (Menu.Item("ComboECougar", true).GetValue<bool>() && E.Level > 0 && E1Cd == 0)
                {
                    var target = TargetSelector.GetTarget(E1.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget(E1.Range))
                    {
                        E1.Cast(target.Position, true);
                    }
                }

                if (Menu.Item("ComboQCougar", true).GetValue<bool>() && Q.Level > 0 && Q1Cd == 0 && E1Cd > 0)
                {
                    var target = TargetSelector.GetTarget(Q1.Range, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget(Q1.Range))
                    {
                        Q1.Cast(target);
                    }
                }

                if (Menu.Item("ComboWCougar", true).GetValue<bool>() && W.Level > 0 && W1Cd == 0)
                {
                    var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget(850f) && HavePassive(target) &&
                        (target.DistanceToPlayer() > 180 || target.Health + target.MagicShield < GetW1Damage(target)))
                    {
                        W2.Cast(target, true);
                    }
                    else
                    {
                        var otherTarget = TargetSelector.GetTarget(W1.Range, TargetSelector.DamageType.Magical);

                        if (target.IsValidTarget(W1.Range - 50) && !HavePassive(target) &&
                            (target.DistanceToPlayer() > 180 || target.Health + target.MagicShield < GetW1Damage(target)))
                        {
                            W1.Cast(target, true);
                        }
                    }
                }

                if (Menu.Item("ComboRCougar", true).GetValue<bool>() && R.IsReady() && Utils.TickCount - ShouldRTime > 500)
                {
                    var target = TargetSelector.GetTarget(1400f, TargetSelector.DamageType.Magical);

                    if (target.IsValidTarget() && !target.IsDead && !target.IsZombie && target.IsValidTarget())
                    {
                        if (Qcd == 0 && !HavePassive(target) && target.DistanceToPlayer() > 900)
                        {
                            R.Cast();
                            return;
                        }

                        if (Ecd == 0 && Q1Cd > 0 && W1Cd > 0 && E1Cd > 0 && Menu.Item("EnableHeal", true).GetValue<bool>() && Me.HealthPercent <= Menu.Item("HealHp", true).GetValue<Slider>().Value && Me.ManaPercent >= Menu.Item("HealMana", true).GetValue<Slider>().Value)
                        {
                            R.Cast();
                            return;
                        }

                        if (W1Cd == 0 && Q1Cd > 0 && E1Cd > 0 && !HavePassive(target))
                        {
                            R.Cast();
                            return;
                        }

                        if (Qcd == 0 && !HavePassive(target) && W1Cd != 0 && Q1Cd != 0 && E1Cd != 0)
                        {
                            R.Cast();
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
                        if (Menu.Item("HaraassQHumanizer", true).GetValue<bool>() && Qcd == 0 && target.IsValidTarget(Q.Range))
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
                var minions = MinionManager.GetMinions(Me.Position, 500);

                if (!minions.Any())
                {
                    return;
                }

                var minion = minions.Find(x => x.Health < GetQ1Damage(x));

                if (Menu.Item("LaneClearQCougar", true).GetValue<bool>() && Q1Cd == 0 && minion != null && minion.IsValidTarget(Q1.Range))
                {
                    Q1.Cast(minion);
                }

                if (Menu.Item("LaneClearWCougar", true).GetValue<bool>() && W1Cd == 0)
                {
                    var W1Farm = W1.GetCircularFarmLocation(minions, W1.Width);

                    if (W1Farm.MinionsHit >= Menu.Item("LaneClearWCougarCount", true).GetValue<Slider>().Value)
                    {
                        W1.Cast(W1Farm.Position);
                    }
                }

                if (Menu.Item("LaneClearECougar", true).GetValue<bool>() && E1Cd == 0)
                {
                    var E1Farm = E1.GetCircularFarmLocation(minions, E1.Width);

                    if (E1Farm.MinionsHit >= Menu.Item("LaneClearECougarCount", true).GetValue<Slider>().Value)
                    {
                        E1.Cast(E1Farm.Position);
                    }
                }
            }
        }

        private static void JungleClearLogic()
        {
            var mobs = MinionManager.GetMinions(Me.Position, 700, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (!mobs.Any())
            {
                return;
            }

            var bigmobs = mobs.Where(x => !x.CharData.BaseSkinName.ToLower().Contains("mini"));
            var mob = mobs.FirstOrDefault();
            var canChangeHumanizer = Menu.Item("JungleClearQHumanizer", true).GetValue<bool>() && Qcd == 0 && 
                Me.ManaPercent >= Menu.Item("JungleClearQMana", true).GetValue<Slider>().Value;

            if (IsHumanizer && Me.ManaPercent >= Menu.Item("JungleClearQMana", true).GetValue<Slider>().Value)
            {
                var bigmob = NewMethod(bigmobs);

                if (Menu.Item("JungleClearQHumanizer", true).GetValue<bool>() && Qcd == 0 && bigmob.Any())
                {
                    Q.CastTo(bigmob.FirstOrDefault(), true);
                }

                if (Menu.Item("JungleClearWHumanizer", true).GetValue<bool>() && Wcd == 0 && bigmob.Any())
                {
                    W.CastTo(bigmob.FirstOrDefault(), true);
                }

                if (Me.Level >= 3)
                {
                    if (!canChangeHumanizer && W1Cd == 0 && (Q1Cd == 0 || E1Cd == 0))
                    {
                        R.Cast();
                    }
                }
                else
                {
                    if (!canChangeHumanizer && (W1Cd == 0 || Q1Cd == 0 || E1Cd == 0))
                    {
                        R.Cast();
                    }
                }
            }
            else if (!IsHumanizer)
            {
                if (Menu.Item("JungleClearWCougar", true).GetValue<bool>() && W1Cd == 0 && mob != null)
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

                if (Menu.Item("JungleClearECougar", true).GetValue<bool>() && E1Cd == 0 && mob.IsValidTarget(E1.Range))
                {
                    if (mob != null)
                    {
                        E1.Cast(mob.Position, true);
                    }
                }

                if (Menu.Item("JungleClearQCougar", true).GetValue<bool>() && Q1Cd == 0 && mob.IsValidTarget(Q1.Range) && E1Cd != 0)
                {
                    Q1.Cast(mob, true);
                }

                if (Me.Level >= 3)
                {
                    if (canChangeHumanizer && bigmobs.Any() && W1Cd > 0 && (E1Cd > 0 || Q1Cd > 0))
                    {
                        R.Cast();
                    }
                }
                else
                {
                    if (canChangeHumanizer && bigmobs.Any() && (W1Cd > 0 || E1Cd > 0 || Q1Cd > 0))
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static Obj_AI_Base[] NewMethod(System.Collections.Generic.IEnumerable<Obj_AI_Base> bigmob)
        {
            return bigmob as Obj_AI_Base[] ?? bigmob.ToArray();
        }

        private static void FleeLogic()
        {
            if (IsHumanizer && R.IsReady())
            {
                R.Cast();
            }
            else if (!IsHumanizer)
            {
                if (W1Cd == 0 && Menu.Item("FleeWCougar", true).GetValue<bool>())
                {
                    W1.Cast(Game.CursorPos);
                }
            }
        }

        private static void KillStealLogic()
        {
            if (!Menu.Item("KillStealQHumanizer", true).GetValue<bool>() || Qcd != 0 || HeroManager.Enemies == null)
            {
                return;
            }

            foreach (var target in HeroManager.Enemies)
            {
                if (target.IsDead || target.IsZombie || !target.IsValidTarget(Q.Range) ||
                    !(target.Health + target.MagicShield < GetQDamage(target)))
                {
                    continue;
                }

                if (target.IsValidTarget(Q.Range))
                {
                    Q.CastTo(target);
                }
            }
        }

        private static void AutoHealLogic()
        {
            if (!Menu.Item("EnableHeal", true).GetValue<bool>())
            {
                return;
            }

            if (Me.ManaPercent < Menu.Item("HealMana", true).GetValue<Slider>().Value)
            {
                return;
            }

            if (Ecd != 0)
            {
                return;
            }

            if (Me.HealthPercent <= Menu.Item("HealHp", true).GetValue<Slider>().Value)
            {
                if (!IsHumanizer && Menu.Item("AutoSwitch", true).GetValue<bool>() && R.IsReady())
                {
                    R.Cast();
                }
                else if (IsHumanizer)
                {
                    E.CastOnUnit(Me, true);
                }
            }

            if (!Menu.Item("HealAlly", true).GetValue<bool>())
            {
                return;
            }

            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe && x.IsValidTarget(E.Range, false)))
            {
                if (!Menu.Item("HealAllyName" + ally.ChampionName.ToLower(), true).GetValue<bool>())
                {
                    continue;
                }

                if (!(ally.HealthPercent <= Menu.Item("HealAllyHp", true).GetValue<Slider>().Value))
                {
                    continue;
                }

                if (ally.CountEnemiesInRange(1000) <= 0 || !ally.IsValidTarget(E.Range, false))
                {
                    continue;
                }

                if (!IsHumanizer && Menu.Item("AutoSwitch", true).GetValue<bool>() && R.IsReady())
                {
                    R.Cast();
                }
                else if (IsHumanizer)
                {
                    E.CastOnUnit(ally, true);
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.Level > 0)
                {
                    Render.Circle.DrawCircle(Me.Position, IsHumanizer ? Q.Range : Q1.Range, Color.Green, 1);
                }

                if (Menu.Item("DrawW", true).GetValue<bool>() && W.Level > 0)
                {
                    if (Me.HasBuff("NidaleePassiveHunting") && !IsHumanizer)
                    {
                        Render.Circle.DrawCircle(Me.Position, W2.Range, Color.FromArgb(9, 253, 242), 1);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(Me.Position, IsHumanizer ? W.Range : W1.Range,
                            Color.FromArgb(9, 253, 242), 1);
                    }
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.Level > 0)
                {
                    Render.Circle.DrawCircle(Me.Position, IsHumanizer ? E.Range : E1.Range, Color.Green, 1);
                }

                if (Menu.Item("DrawCD", true).GetValue<bool>())
                {
                    string msg;
                    var QCoolDown = (int) Qcd == -1 ? 0 : (int) Qcd;
                    var WCoolDown = (int) Wcd == -1 ? 0 : (int) Wcd;
                    var ECoolDown = (int) Ecd == -1 ? 0 : (int) Ecd;
                    var Q1CoolDown = (int) Q1Cd == -1 ? 0 : (int) Q1Cd;
                    var W1CoolDown = (int) W1Cd == -1 ? 0 : (int) W1Cd;
                    var E1CoolDown = (int) E1Cd == -1 ? 0 : (int) E1Cd;

                    if (!IsHumanizer)
                    {
                        msg = "Q: " + QCoolDown + "   W: " + WCoolDown + "   E: " + ECoolDown;
                        Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 30, Color.Orange, msg);
                    }
                    else
                    {
                        msg = "Q: " + Q1CoolDown + "   W: " + W1CoolDown + "   E: " + E1CoolDown;
                        Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 30, Color.SkyBlue, msg);
                    }
                }

                if (Menu.Item("DrawTarget", true).GetValue<bool>())
                {
                    foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget() && HavePassive(x)))
                    {
                        if (target != null && target.IsValid)
                        {
                            Render.Circle.DrawCircle(target.Position, target.BoundingRadius + 100, Color.Red, 3);
                        }
                    }
                }

                if (Menu.Item("DrawDamage", true).GetValue<StringList>().SelectedIndex == 3)
                {
                    return;
                }

                foreach (
                    var x in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = x;
                    HpBarDraw.DrawDmg(ComboDamage(x), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        private static float ComboDamage(AIHeroClient target)
        {
            var humanizer = GetQDamage(target) + GetWDamage(target);
            var cougar = GetQ1Damage(target) + GetW1Damage(target) + GetE1Damage(target);

            switch (Menu.Item("DrawDamage", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return (float)humanizer;
                case 1:
                    return (float)cougar;
                case 2:
                    return IsHumanizer ? (float) humanizer : (float) cougar;
                case 3:
                    return (float) (humanizer + cougar);
            }

            return 0;
        }

        private static float CheckCD(float Expires)
        {
            var time = Expires - Game.Time;

            if (time < 0)
            {
                time = 0;

                return time;
            }

            return time;
        }

        private static bool HavePassive(Obj_AI_Base target)
        {
            return target.HasBuff("NidaleePassiveHunted");
        }

        private static double GetQDamage(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
            {
                return 0d;
            }

            return Q.Level > 0 && Qcd == 0
                ? Me.CalcDamage
                (target, Damage.DamageType.Magical,
                    new[] {60, 77.5, 95, 112.5, 130}[Q.Level - 1] +
                    0.4*Me.TotalMagicalDamage)
                : 0d;
        }

        private static double GetQ1Damage(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
            {
                return 0d;
            }

            if (Q.Level > 0 && Q1Cd == 0)
            {
                double dmg = (new double[] {4, 20, 50, 90}[R.Level - 1] + 0.36*Me.TotalMagicalDamage +
                           0.75*(Me.BaseAttackDamage + Me.FlatPhysicalDamageMod))*
                          ((target.MaxHealth - target.Health)/target.MaxHealth*1.5 + 1);

                dmg *= target.HasBuff("nidaleepassivehunted") ? 1.33 : 1.0;

                return dmg;
            }

            return 0d;
        }

        private static double GetWDamage(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
            {
                return 0d;
            }

            return W.Level > 0 && Wcd == 0
                ? Me.CalcDamage(target, Damage.DamageType.Magical,
                    new double[] {40, 80, 120, 160, 200}[W.Level - 1] + 0.2*Me.TotalMagicalDamage)
                : 0d;
        }

        private static double GetW1Damage(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
            {
                return 0d;
            }

            return W.Level > 0 && W1Cd == 0
                ? Me.CalcDamage(target, Damage.DamageType.Magical,
                    new double[] {60, 110, 160, 210}[R.Level - 1] + 0.3*Me.TotalMagicalDamage)
                : 0d;
        }

        private static double GetE1Damage(Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
            {
                return 0d;
            }

            return E.Level > 0 && E1Cd == 0
                ? Me.CalcDamage(target, Damage.DamageType.Magical,
                    new double[] {70, 130, 190, 250}[R.Level - 1] + 0.45*Me.TotalMagicalDamage)
                : 0d;
        }
    }
}
