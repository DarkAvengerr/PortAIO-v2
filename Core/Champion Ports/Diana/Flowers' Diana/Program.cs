using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Diana
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using static Common;

    internal class Program //TODO: flee mode
    {
        public static Menu Menu;
        public static AIHeroClient Me;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static SpellSlot Ignite = SpellSlot.Unknown;
        public static SpellSlot Flash = SpellSlot.Unknown;
        public static int SkinID;
        public static int Qcd;
        public static int LastRCast;
        public static Orbwalking.Orbwalker Orbwalker;
        public static HpBarDraw HpBarDraw = new HpBarDraw();

        public static void Main()
        {
            OnGameLoad();
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Diana")
            {
                return;
            }

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 250f);
            E = new Spell(SpellSlot.E, 450f);
            R = new Spell(SpellSlot.R, 825f);

            Q.SetSkillshot(0.25f, 150f, 1400f, false, SkillshotType.SkillshotCircle);

            Ignite = Me.GetSpellSlot("SummonerDot");
            Flash = Me.GetSpellSlot("SummonerFlash");

            SkinID = Me.SkinId;

            Menu = new Menu("Flowers' Diana", "Flowers' Diana", true);

            var OrbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(OrbMenu);
            }

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRUnder", "Use R| Under Turret", true).SetValue(false));
                ComboMenu.AddItem(new MenuItem("ComboSecondR", "Use Second R", true).SetValue(true)).SetTooltip("Only Can Kill Target");
                ComboMenu.AddItem(new MenuItem("ComboDot", "Use Ignite", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboMode", "Combo Mode?", true).SetValue(new StringList(new[] {"Q->R", "R->Q"})));
                ComboMenu.AddItem(
                    new MenuItem("MisayaRange", "Min RQ Range >= x", true).SetValue(new Slider(300, 150, 825)));// if target distance player < 150f, misaya combo dont work!
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("Harassmana", "When Player ManaPercent >= x%", true)
                    .SetValue(new Slider(50)));
            }

            var LaneMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneMenu.AddItem(new MenuItem("LaneClearQCount", "Use Q| Min Hit Count >= x", true)
                    .SetValue(new Slider(3, 1, 5)));
                LaneMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                LaneMenu.AddItem(new MenuItem("LaneClearWCount", "Use W| Min Hit Count >= x", true)
                    .SetValue(new Slider(3, 1, 5)));
                LaneMenu.AddItem(new MenuItem("LaneClearmana", "When Player ManaPercent >= x%", true)
                    .SetValue(new Slider(50)));
            }

            var JungleMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                JungleMenu.AddItem(new MenuItem("JungleClearR", "Use R", true).SetValue(true));
                JungleMenu.AddItem(new MenuItem("JungleClearmana", "When Player ManaPercent >= x%", true)
                    .SetValue(new Slider(30)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealRtarget", "Use R List", true));
                foreach (var target in HeroManager.Enemies)
                {
                    KillStealMenu.AddItem(
                        new MenuItem("KillStealR" + target.ChampionName.ToLower(), target.ChampionName, true).SetValue(
                            AutoEnableList.Contains(target.ChampionName)));
                }
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(new MenuItem("EGap", "Use E Anti Gapcloser", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("EInt", "Use E Interrupt Spell", true).SetValue(true));
                MiscMenu.AddItem(
                    new MenuItem("EFlash", "E Flash Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            }

            var SkinMenu = Menu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
            {
                SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged += EnbaleSkin;
                SkinMenu.AddItem(
                    new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(
                        new StringList(new[]
                        {
                            "Classic", "Dark Valkyrie Diana", "Lunar Goddess Diana", "Infernal Diana"
                        })));
            }

            var PredMenu = Menu.AddSubMenu(new Menu("Prediction", "Prediction"));
            {
                PredMenu.AddItem(new MenuItem("SelectPred", "Select Prediction: ", true).SetValue(new StringList(new[]
                {
                    "Common Prediction", "OKTW Prediction", "SDK Prediction", "SPrediction(Need F5 Reload)",
                    "xcsoft AIO Prediction"
                })));
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
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();

            if (Menu.Item("SelectPred", true).GetValue<StringList>().SelectedIndex == 3)
            {
                SPrediction.Prediction.Initialize(PredMenu);
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                Me.SetSkin(Me.ChampionName, SkinID);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Args.SData.Name.Contains("DianaTeleport"))
            {
                LastRCast = Utils.TickCount;
            }

            if (!Args.SData.Name.Contains("DianaTeleport") || !W.IsReady())
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
                            if (Menu.Item("ComboW", true).GetValue<bool>() && target.IsValidTarget(W.Range))
                            {
                                W.Cast(true);
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    {
                        if (Me.ManaPercent >= Menu.Item("JungleClearmana", true).GetValue<Slider>().Value)
                        {
                            var mob = (Obj_AI_Base)Args.Target;
                            var mobs = MinionManager.GetMinions(Me.Position, R.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                            if (mob != null && mobs.Contains(mob))
                            {
                                if (Menu.Item("JungleClearW", true).GetValue<bool>() && mob.IsValidTarget(W.Range))
                                {
                                    W.Cast(true);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("EGap", true).GetValue<bool>() && gapcloser.Sender.IsValidTarget(E.Range))
            {
                if (E.IsReady())
                {
                    E.Cast(true);
                }
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("EInt", true).GetValue<bool>() && sender.IsValidTarget(E.Range) &&
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                if (E.IsReady())
                {
                    E.Cast(true);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            Qcd = Q.Level > 0 ? (Q.Instance.CooldownExpires - Game.Time <= 0 ? 0 : (int)(Q.Instance.CooldownExpires - Game.Time)) : -1;

            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
                Me.SetSkin(Me.ChampionName,
                    Menu.Item("SelectSkin", true).GetValue<StringList>().SelectedIndex);
            }

            if (Menu.Item("EFlash", true).GetValue<KeyBind>().Active)
            {
                EFlash();
            }

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

        private static void EFlash()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (Flash != SpellSlot.Unknown && Flash.IsReady() && E.IsReady())
            {
                AIHeroClient target = null;

                target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(700f, TargetSelector.DamageType.Magical);

                if (CheckTarget(target, 700f) && target.DistanceToPlayer() > E.Range)
                {
                    var pos = Me.Position.Extend(target.Position, 425f);

                    if (target.Position.Distance(pos) < 350f && target.DistanceToPlayer() > E.Range)
                    {
                        E.Cast();
                        LeagueSharp.Common.Utility.DelayAction.Add(100 + Game.Ping*2, () =>
                        {
                            Me.Spellbook.CastSpell(Flash, target.Position);
                        });      
                    }
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var target in
                HeroManager.Enemies.Where(
                    e => e.IsValidTarget() && !e.IsDead && !e.IsZombie && CheckTargetSureCanKill(e)))
            {
                if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    Me.GetSpellDamage(target, SpellSlot.Q) > target.Health + target.MagicShield)
                {
                    Q.CastTo(target);
                }

                if (Menu.Item("KillStealR", true).GetValue<bool>() && Menu.Item("KillStealR" + target.ChampionName.ToLower(), true).GetValue<bool>() &&
                    R.IsReady() && target.IsValidTarget(R.Range) &&
                    Me.GetSpellDamage(target, SpellSlot.R) + Me.TotalAttackDamage > target.Health + target.MagicShield)
                {
                    R.CastOnUnit(target, true);
                }
            }
        }

        private static void Combo()
        {
            AIHeroClient target = null;

            target = TargetSelector.GetSelectedTarget() ??
                     TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (CheckTarget(target, Q.Range))
            {
                if (Menu.Item("ComboDot", true).GetValue<bool>() &&
                    Ignite != SpellSlot.Unknown && Ignite.IsReady())
                {
                    if (ComboDamage(target) > target.Health - 150 || target.HealthPercent < 20)
                    {
                        Me.Spellbook.CastSpell(Ignite, target);
                    }
                }

                if (Me.Level < 6)
                {
                    if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.CastTo(target);
                    }

                    if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.Cast(true);
                    }

                    if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() &&
                        target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 50 &&
                        target.IsValidTarget(E.Range - 30) && !target.HasBuffOfType(BuffType.SpellShield))
                    {
                        E.Cast(true);
                    }
                }
                else
                {
                    switch (Menu.Item("ComboMode", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() &&
                                Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(R.Range)
                                && target.DistanceToPlayer() >= 600)
                            {
                                Q.Cast(target, true);
                                R.Cast(target, true);
                            }

                            if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                            {
                                Q.CastTo(target);
                            }

                            if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() && target.IsValidTarget(R.Range))
                            {
                                LogicCast(target);
                            }

                            if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                            {
                                W.Cast(true);
                            }

                            if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() &&
                                target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 50 &&
                                target.IsValidTarget(E.Range - 30) && !target.HasBuffOfType(BuffType.SpellShield))
                            {
                                E.Cast(true);
                            }
                            break;
                        case 1:
                            if (target.IsValidTarget(R.Range))
                            {
                                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() &&
                                    Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(R.Range)
                                    && target.DistanceToPlayer() >= Menu.Item("MisayaRange", true).GetValue<Slider>().Value)
                                {
                                    R.Cast(target, true);
                                    Q.Cast(target, true);
                                }

                                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                                {
                                    Q.CastTo(target);
                                }

                                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() && target.IsValidTarget(R.Range))
                                {
                                    LogicCast(target);
                                }

                                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                                {
                                    W.Cast(true);
                                }

                                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() &&
                                    target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + 50 &&
                                    target.IsValidTarget(E.Range - 30) && !target.HasBuffOfType(BuffType.SpellShield))
                                {
                                    E.Cast(true);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private static void LogicCast(Obj_AI_Base target)
        {
            if (!R.IsReady() || !Menu.Item("ComboR", true).GetValue<bool>() || target.DistanceToPlayer() > R.Range)
            {
                return;
            }

            if (!Menu.Item("ComboRUnder", true).GetValue<bool>() && target.UnderTurret(true))
            {
                return;
            }

            if (HaveQPassive(target))
            {
                R.Cast(target, true);
            }

            if (Menu.Item("ComboSecondR", true).GetValue<bool>() && Utils.TickCount - LastRCast > 800 && Qcd >= 3)
            {
                if (target.Health + target.MagicShield + target.HPRegenRate < Me.GetSpellDamage(target, SpellSlot.R))
                {
                    R.Cast(target);
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu.Item("Harassmana", true).GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (CheckTarget(target, Q.Range))
                {
                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        Q.CastTo(target);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearmana", true).GetValue<Slider>().Value)
            {
                var qminions = MinionManager.GetMinions(Me.Position, Q.Range);

                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady() && qminions.Count > 0)
                {
                    var qfarm = Q.GetCircularFarmLocation(qminions);

                    if (qfarm.MinionsHit >= Menu.Item("LaneClearQCount", true).GetValue<Slider>().Value)
                    {
                        Q.Cast(qfarm.Position, true);
                    }
                }

                var wminions = MinionManager.GetMinions(Me.Position, W.Range);

                if (Menu.Item("LaneClearW", true).GetValue<bool>() && W.IsReady() && wminions.Count >= Menu.Item("LaneClearWCount", true).GetValue<Slider>().Value)
                {
                    W.Cast(true);
                }
            }
        }

        private static void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearmana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Count > 0)
                {
                    var bigmob = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));
                    var mob = mobs.FirstOrDefault();

                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        Q.Cast(mob, true);
                    }

                    if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        W.Cast(true);
                    }

                    if (Menu.Item("JungleClearR", true).GetValue<bool>() && R.IsReady() && bigmob.IsValidTarget(R.Range))
                    {
                        if (HaveQPassive(bigmob))
                        {
                            R.CastOnUnit(bigmob, true);
                        }
                        else if (bigmob != null && bigmob.Health < Me.GetSpellDamage(bigmob, SpellSlot.R) &&
                            bigmob.Health > Me.TotalAttackDamage * 2)
                        {
                            R.CastOnUnit(bigmob, true);
                        }
                    }
                }
            }
        }

        private static void Flee()
        {
            // wait to do ~
        }

        private static void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.FromArgb(253, 164, 17), 1);
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(143, 16, 146), 1);
                }

                if (Menu.Item("DrawR", true).GetValue<bool>() && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(25, 213, 255), 1);
                }

                if (Menu.Item("DrawDamage", true).GetValue<bool>())
                {
                    foreach (
                        var x in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg((float) ComboDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
                }
            }
        }

        private static bool HaveQPassive(Obj_AI_Base target)
        {
            return target.HasBuff("dianamoonlight");
        }
    }
}
