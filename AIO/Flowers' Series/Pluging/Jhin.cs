using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Pluging
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;
    using static Common;

    internal class Jhin
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        public static int LastPingT;
        public static int LastECast;
        public static int LastShowNoit;
        public static bool IsAttack;

        public static Vector2 PingLocation;

        private static readonly Menu Menu = Program.Championmenu;
        private static readonly AIHeroClient Me = Program.Me;
        private static readonly Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        public Jhin()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 2500f);
            E = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R, 3500f);

            W.SetSkillshot(0.75f, 40, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.5f, 120, 1600, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.21f, 80, 5000, false, SkillshotType.SkillshotLine);

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
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(new MenuItem("AutoW", "Auto W| When target Cant Move", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("AutoE", "Auto E| When target Cant Move", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("GapW", "Anti GapCloser W| When target HavePassive", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("GapE", "Anti GapCloser E", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("PingKill", "Auto Ping Kill Target", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("NormalPingKill", "Normal Ping?", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("NotificationKill", "Notification Kill Target", true).SetValue(true));
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
            var target = gapcloser.Sender;

            if (target.IsValidTarget(E.Range) &&
                (gapcloser.End.DistanceToPlayer() <= 300 || target.DistanceToPlayer() <= 300))
            {
                if (Menu.Item("GapE", true).GetValue<bool>() && E.IsReady() && Utils.TickCount - LastECast > 2500 && !IsAttack)
                {
                    E.CastTo(target);
                }

                if (Menu.Item("GapW", true).GetValue<bool>() && W.IsReady() && HasPassive(target))
                {
                    W.CastTo(target);
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
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

                            if (Menu.Item("ComboW", true).GetValue<bool>() && Menu.Item("ComboWAA", true).GetValue<bool>() &&
                                W.IsReady() && target.IsValidTarget(W.Range) && target.HasBuff("jhinespotteddebuff"))
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

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            foreach (var enemy in HeroManager.Enemies.Where(h => R.IsReady() && h.IsValidTarget(R.Range) &&
            Me.GetSpellDamage(h, SpellSlot.R) * Menu.Item("RMenuKill", true).GetValue<Slider>().Value >
            h.Health + h.HPRegenRate * 3))
            {
                if (Menu.Item("PingKill", true).GetValue<bool>())
                {
                    Ping(enemy.Position.To2D());
                }

                if (Menu.Item("NotificationKill", true).GetValue<bool>() && Utils.TickCount - LastShowNoit > 10000)
                {
                    Notifications.AddNotification(new Notification("R Kill: " + enemy.ChampionName + "!", 3000, true).SetTextColor(Color.FromArgb(255, 0, 0)));
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
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        private void RLogic()
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
                        R.Cast(R.GetPrediction(target).UnitPosition, true);
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

                    if (target.Health > Me.GetSpellDamage(target, SpellSlot.R) * Menu.Item("RMenuKill", true).GetValue<Slider>().Value)
                    {
                        return;
                    }

                    R.Cast(R.GetPrediction(target).UnitPosition, true);
                }

                if (R.Instance.Name == "JhinRShot")
                {
                    foreach (var t in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && InRCone(x)))
                    {
                        if (!InRCone(t))
                        {
                            return;
                        }

                        if (Menu.Item("RMenuSemi", true).GetValue<KeyBind>().Active)
                        {
                            AutoUse(t);
                            R.Cast(R.GetPrediction(t).UnitPosition, true);
                        }

                        if (Menu.Item("ComboR", true).GetValue<bool>() &&
                            Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                        {
                            AutoUse(t);
                            R.Cast(R.GetPrediction(t).UnitPosition, true);
                        }

                        if (!Menu.Item("RMenuAuto", true).GetValue<bool>())
                        {
                            return;
                        }

                        AutoUse(t);
                        R.Cast(R.GetPrediction(t).UnitPosition, true);
                    }
                }
            }
        }

        private void KillSteal()
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

        private void Auto()
        {
            if (R.Instance.Name == "JhinRShot")
            {
                return;
            }

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

        private void Combo()
        {
            if (R.Instance.Name == "JhinRShot")
            {
                return;
            }

            var orbTarget = Orbwalker.GetTarget();

            if (CheckTarget((Obj_AI_Base)orbTarget, Orbwalking.GetRealAutoAttackRange(Me)))
            {
                if (Menu.Item("ComboCutlass", true).GetValue<bool>() && Items.HasItem(3144) && Items.CanUseItem(3144))
                {
                    Items.UseItem(3144, (Obj_AI_Base)orbTarget);
                }

                if (Menu.Item("ComboBotrk", true).GetValue<bool>() && Items.HasItem(3153) && Items.CanUseItem(3153) &&
                    (orbTarget.HealthPercent < 80 || Me.HealthPercent < 80))
                {
                    Items.UseItem(3153, (Obj_AI_Base)orbTarget);
                }
            }

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

            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() &&
                CheckTarget(qTarget, Q.Range) && !Orbwalking.CanAttack())
            {
                Q.CastOnUnit(qTarget, true);
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

        private void Harass()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
            {
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

        private void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                if (!minions.Any())
                {
                    return;
                }

                var minion = minions.MinOrDefault(x => x.Health);

                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady() && minion != null &&
                    minion.IsValidTarget(Q.Range) && minions.Count > 2)
                {
                    Q.Cast(minion, true);
                }

                if (Menu.Item("LaneClearW", true).GetValue<bool>() && W.IsReady() && minion != null)
                {
                    W.Cast(minion, true);
                }
            }
        }

        private void JungleClear()
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

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.Item("DrawW", true).GetValue<bool>() && W.IsReady())
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

        private void OnEndScene(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
#pragma warning disable 618
                if (Menu.Item("DrawRMin", true).GetValue<bool>() && R.IsReady())
                    LeagueSharp.Common.Utility.DrawCircle(Me.Position, R.Range, Color.FromArgb(14, 194, 255), 1, 30, true);
#pragma warning restore 618
            }
        }

        private void AutoUse(AIHeroClient target)
        {
            if (Items.HasItem(3363) && Items.CanUseItem(3363))
            {
                Items.UseItem(3363, target.Position);
            }
        }

        private bool HasPassive(AIHeroClient target)
        {
            return target.HasBuff("jhinespotteddebuff");
        }

        private bool InRCone(AIHeroClient target)
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
            TacticalMap.ShowPing(
                Menu.Item("NormalPingKill", true).GetValue<bool>() ? PingCategory.Normal : PingCategory.Fallback,
                PingLocation, true);
        }
    }
}
