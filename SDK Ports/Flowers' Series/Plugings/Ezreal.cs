using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Plugings
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Linq;
    using static Common.Manager;

    public static class Ezreal
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static Spell EQ;
        private static HpBarDraw HpBarDraw = new HpBarDraw();
        private static bool usee = false;

        private static Menu Menu => Program.Menu;
        private static AIHeroClient Me => Program.Me;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 475f);
            R = new Spell(SpellSlot.R, 10000f);
            EQ = new Spell(SpellSlot.Q, Q.Range + E.Range);
            EQ.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.Add(new Menu("Ezreal_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
            }

            var HarassMenu = Menu.Add(new Menu("Ezreal_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuSlider("QMana", "Use Q|When Player ManaPercent >= %", 60));
                HarassMenu.Add(new MenuBool("W", "Use W", true));
                HarassMenu.Add(new MenuSlider("WMana", "Use W|When Player ManaPercent >= %", 70));
            }

            var LaneClearMenu = Menu.Add(new Menu("Ezreal_LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuSlider("QMana", "Use Q|When Player ManaPercent >= %", 30));
                LaneClearMenu.Add(new MenuBool("W", "Use W (Push Tower)", true));
            }

            var JungleMenu = Menu.Add(new Menu("Ezreal_JungleClear", "Jungle Clear"));
            {
                JungleMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleMenu.Add(new MenuSlider("QMana", "Use Q|When Player ManaPercent >= %", 50));
            }

            var LastHitMenu = Menu.Add(new Menu("Ezreal_LastHit", "Last Hit"));
            {
                LastHitMenu.Add(new MenuBool("Q", "Use Q", true));
                LastHitMenu.Add(new MenuSlider("QMana", "Use Q|When Player ManaPercent >= %", 50));
            }

            var KillStealMenu = Menu.Add(new Menu("Ezreal_KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("W", "Use W", true));
            }

            var AutoMenu = Menu.Add(new Menu("Ezreal_Auto", "Auto"));
            {
                AutoMenu.Add(new MenuKeyBind("Q", "Use Q", System.Windows.Forms.Keys.H, KeyBindType.Toggle));
                AutoMenu.Add(new MenuKeyBind("W", "Use W", System.Windows.Forms.Keys.G, KeyBindType.Toggle));
            }

            var QList = Menu.Add(new Menu("Ezreal_QList", "Q Black List"));
            {
                QList.Add(new MenuSeparator("sfsd", "Only Work in Harass or Auto Mode"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => QList.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, !AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var WList = Menu.Add(new Menu("Ezreal_WList", "W Black List"));
            {
                WList.Add(new MenuSeparator("sfsd", "Only Work in Harass or Auto Mode"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => WList.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, !AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var MiscMenu = Menu.Add(new Menu("Ezreal_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("Gapcloser", "Use E |Anti GapCloser", true));
                MiscMenu.Add(new MenuBool("AntiMelee", "Use E |Anti Melee", true));
            }

            var RMenu = Menu.Add(new Menu("Ezreal_RMenu", "R Settings"));
            {
                RMenu.Add(new MenuBool("Auto", "Auto R", true));
                RMenu.Add(new MenuBool("Safe", "Safe R", true));
                RMenu.Add(new MenuSlider("RMin", "Min R Hit Enemies >= ", 2, 1, 5));
                RMenu.Add(new MenuBool("Steal", "Auto Steal Jungle", false));
                RMenu.Add(new MenuKeyBind("R", "R Key (Press)", System.Windows.Forms.Keys.T, KeyBindType.Press));
            }

            var Draw = Menu.Add(new Menu("Ezreal_Draw", "Draw"));
            {
                Draw.Add(new MenuBool("Q", "Q Range"));
                Draw.Add(new MenuBool("W", "W Range"));
                Draw.Add(new MenuBool("E", "E Range"));
                Draw.Add(new MenuBool("AutoQ", "Draw Auto Q States", true));
                Draw.Add(new MenuBool("AutoW", "Draw Auto W States", true));
                Draw.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Game.OnUpdate += OnUpdate;
            Variables.Orbwalker.OnAction += OnAction;
            Drawing.OnDraw += OnDraw;
            Events.OnGapCloser += OnGapCloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (InCombo)
            {
                ComboLogic();
            }

            if (InHarass)
            {
                HarassLogic();
            }

            if (InClear)
            {
                LaneLogic();
                JungleLogic();
            }

            LastHit();
            AutoQLogic();
            AutoWLogic();
            AutoRLogic();
            StealLogic();
            RKey();
            KillSteal();
        }

        private static void LastHit()
        {
            if ((InHarass || InLastHit) && !Me.IsUnderEnemyTurret() && Menu["Ezreal_LastHit"]["Q"])
            {
                var targets = GetMinions(Me.Position, Q.Range);

                foreach (var target in targets)
                {
                    if (target != null)
                    {
                        if (target.IsValidTarget(Q.Range) && !InAutoAttackRange(target) && Q.IsReady() &&
                            target.Health < Q.GetDamage(target) &&
                            Me.ManaPercent >= Menu["Ezreal_LastHit"]["QMana"].GetValue<MenuSlider>().Value)
                        {
                            Q.Cast(target);
                        }
                    }
                }
            }
        }

        private static void ComboLogic()
        {
            var target = GetTarget(EQ.Range, DamageType.Physical);

            if (CheckTarget(target))
            {
                if (Menu["Ezreal_Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }

                if (Menu["Ezreal_Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }

                if (Menu["Ezreal_Combo"]["E"] && E.IsReady() && !Me.IsUnderEnemyTurret())
                {
                    if (!InAutoAttackRange(target) && Me.CountEnemyHeroesInRange(1200) <= 2 && target.IsHPBarRendered)
                    {
                        if (target.Health < E.GetDamage(target) + Me.GetAutoAttackDamage(target) && 
                            target.Distance(Game.CursorPos) < Me.Distance(Game.CursorPos))
                        {
                            usee = true;
                        }

                        if (target.Health < E.GetDamage(target) + W.GetDamage(target) && W.IsReady() &&
                            target.Distance(Game.CursorPos) + 350 < Me.Distance(Game.CursorPos))
                        {
                            usee = true;
                        }

                        if (target.Health < E.GetDamage(target) + Q.GetDamage(target) && Q.IsReady() &&
                            target.Distance(Game.CursorPos) + 300 < Me.Distance(Game.CursorPos))
                        {
                            usee = true;
                        }
                    }

                    if (usee)
                    {
                        E.Cast(GameObjects.Player.Position.Extend(Game.CursorPos, E.Range));
                        usee = false;
                    }
                }
            }

            if (Menu["Ezreal_RMenu"]["Auto"] && R.IsReady())
            {
                var RTarget = GetTarget(R.Range);

                if (CheckTarget(RTarget))
                {
                    if (RTarget.Health + RTarget.HPRegenRate * 2 <= GetDamage(RTarget, false, false, false, false) &&
                        RTarget.IsValidTarget(1500))
                    {
                        R.Cast(RTarget);
                    }
                    else if (RTarget.IsValidTarget(Q.Range + E.Range - 200) &&
                        RTarget.Health + RTarget.MagicShield + RTarget.HPRegenRate * 2 <=
                        R.GetDamage(RTarget) + Q.GetDamage(RTarget) + W.GetDamage(RTarget) &&
                        Q.IsReady() && W.IsReady() &&
                        RTarget.CountAllyHeroesInRange(Q.Range + E.Range - 200) <= 1)
                    {
                        R.Cast(RTarget);
                    }
                    else if (RTarget.IsValidTarget())
                    {
                        R.CastIfWillHit(RTarget, Menu["Ezreal_RMenu"]["RMin"].GetValue<MenuSlider>().Value);
                    }
                }
            }
        }

        private static void HarassLogic()
        {
            if (Menu["Ezreal_Harass"]["Q"] && Q.IsReady())
            {
                foreach (var target in GetEnemies(Q.Range))
                {
                    if (!Menu["Ezreal_Harass"][target.ChampionName.ToLower()] &&
                        !Me.IsUnderEnemyTurret() && 
                        Me.ManaPercent >= Menu["Ezreal_Harass"]["QMana"].GetValue<MenuSlider>().Value)
                    {
                        Q.Cast(target);
                    }
                }
            }

            if (Menu["Ezreal_Harass"]["W"] && W.IsReady())
            {
                foreach (var target in GetEnemies(Q.Range))
                {
                    if (!Menu["Ezreal_WList"][target.ChampionName.ToLower()] && 
                        !Me.IsUnderEnemyTurret() &&
                        Me.ManaPercent >= Menu["Ezreal_Harass"]["WMana"].GetValue<MenuSlider>().Value)
                    {
                        W.Cast(target);
                    }
                }
            }
        }

        private static void LaneLogic()
        {
            if (Menu["Ezreal_LaneClear"]["Q"] && Me.ManaPercent >= Menu["Ezreal_LaneClear"]["QMana"].GetValue<MenuSlider>().Value)
            {
                if (Q.IsReady() && !Me.IsUnderEnemyTurret())
                {
                    var Minions = GetMinions(Me.Position, Q.Range);

                    if (Minions.Count() > 0)
                    {
                        foreach (var min in Minions.Where(x => x.Health < Q.GetDamage(x)))
                        {
                            if (min != null)
                            {
                                if (InAutoAttackRange(min) && min.Health > Me.GetAutoAttackDamage(min))
                                {
                                    Q.Cast(min);
                                }
                                else if (!InAutoAttackRange(min) && min.IsValidTarget(Q.Range))
                                {
                                    Q.Cast(min);
                                }
                            }
                            else if (min == null)
                            {
                                Q.Cast(Minions.FirstOrDefault());
                            }
                        }
                    }
                }
            }
        }

        private static void JungleLogic()
        {
            if (Menu["Ezreal_JungleClear"]["Q"] && Me.ManaPercent >= Menu["Ezreal_JungleClear"]["QMana"].GetValue<MenuSlider>().Value)
            {
                var Mobs = GetMobs(Me.Position, Q.Range);

                if (Mobs.Count() > 0)
                {
                    foreach (var mob in Mobs)
                    {
                        if (Q.IsReady())
                        {
                            Q.Cast(mob);
                        }
                    }
                }
            }
        }

        private static void AutoQLogic()
        {
            if (!Me.IsDead && !Me.IsRecalling() && !Me.IsDashing() && Menu["Ezreal_Auto"]["Q"].GetValue<MenuKeyBind>().Active && Q.IsReady() && !Me.HasBuff("Reacll") && !InCombo && !InHarass)
            {
                foreach (var target in GetEnemies(Q.Range))
                {
                    if (!Menu["Ezreal_QList"][target.ChampionName.ToLower()] && !Me.IsUnderEnemyTurret())
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        private static void AutoWLogic()
        {
            if (!Me.IsDead && !Me.IsRecalling() && !Me.IsDashing() && Menu["Ezreal_Auto"]["W"].GetValue<MenuKeyBind>().Active && W.IsReady() && !Me.HasBuff("Reacll") && !InCombo && !InHarass)
            {
                foreach (var target in GetEnemies(W.Range))
                {
                    if (!Menu["Ezreal_WList"][target.ChampionName.ToLower()] && !Me.IsUnderEnemyTurret())
                    {
                        W.Cast(target);
                    }
                }
            }
        }

        private static void AutoRLogic()
        {
            if (R.IsReady() && Menu["Ezreal_RMenu"]["Auto"])
            {
                if (Menu["Ezreal_RMenu"]["Safe"] && Me.CountEnemyHeroesInRange(900) != 0)
                {
                    return;
                }

                foreach (var t in GetEnemies(R.Range))
                {
                    var RDMG = R.GetDamage(t);
                    var CheckTargetHealth = t.Health + t.MagicShield + t.HPRegenRate * 2 < RDMG;

                    if (CheckTargetHealth && t.HasBuff("recall"))
                        R.Cast(t);

                    if (!CanMove(t) && t.Health < RDMG + Q.GetDamage(t) * 3 && t.IsValidTarget(Q.Range + E.Range - 200))
                    {
                        if (t.CountAllyHeroesInRange(400) >= 1)
                        {
                            R.CastIfWillHit(t, Menu["Ezreal_RMenu"]["RMin"].GetValue<MenuSlider>().Value);
                        }
                        else
                        {
                            R.Cast(t);
                        }
                    }
                }
            }
        }

        private static void StealLogic()
        {
            if (R.IsReady() && Menu["Ezreal_RMenu"]["Steal"])
            {
                foreach (var mob in GetMobs(Me.Position, 10000))
                {
                    if (mob != null && (mob.BaseSkinName == "SRU_Dragon" || mob.BaseSkinName == "SRU_Baron" || mob.BaseSkinName == "SRU_Red" || mob.BaseSkinName == "SRU_Blue") && Me.Distance(mob) >= Q.Range)
                    {
                        var time = (int)(Vector3.Distance(Me.ServerPosition, mob.ServerPosition) / R.Speed + R.Delay);
                        var Residual = R.GetDamage(mob) - (mob.Health + mob.HPRegenRate * 3);
                        var Enemy = GameObjects.EnemyHeroes.Find(e => e.Distance(mob) > 200 + mob.BoundingRadius);
                        var Ally = GameObjects.AllyHeroes.Find(e => e.Distance(mob) < 800 + mob.BoundingRadius);

                        if (Enemy != null && Residual >= 0 && Ally == null)
                        {
                            R.Cast(mob);
                        }
                    }
                }
            }
        }

        private static void RKey()
        {
            if (Menu["Ezreal_RMenu"]["R"].GetValue<MenuKeyBind>().Active && R.IsReady())
            {
                var t = GetTarget(2000, DamageType.Magical);

                if (t != null)
                {
                    if (t.Health + t.HPRegenRate * 2 < R.GetDamage(t))
                    {
                        R.Cast(t);
                    }
                    else
                    {
                        R.CastIfWillHit(t, Menu["Ezreal_RMenu"]["RMin"].GetValue<MenuSlider>().Value);
                    }
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var target in GetEnemies(Q.Range))
            {
                if (Q.GetDamage(target) > target.Health && Menu["Ezreal_KillSteal"]["Q"])
                {
                    if (target.IsValidTarget(Q.Range))
                        Q.Cast(target);
                }

                if (W.GetDamage(target) > target.Health && Menu["Ezreal_KillSteal"]["W"])
                {
                    if (target.IsValidTarget(W.Range))
                        W.Cast(target);
                }

                if (target.IsValidTarget(W.Range) && Menu["Ezreal_KillSteal"]["W"] && Menu["Ezreal_KillSteal"]["Q"])
                {
                    if (target.Health < Q.GetDamage(target) + W.GetDamage(target))
                    {
                        W.Cast(target);
                        Q.Cast(target);
                    }
                }
            }
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                if (Args.Target is AIHeroClient)
                {
                    if (InCombo)
                    {
                        if (Menu["Ezreal_Combo"]["Q"] && Q.IsReady())
                        {
                            var target = Args.Target as AIHeroClient;

                            if (target.IsValidTarget(Q.Range))
                            {
                                Q.Cast(target);
                            }
                        }

                        if (Menu["Ezreal_Combo"]["W"] && W.IsReady())
                        {
                            var target = Args.Target as AIHeroClient;

                            if (target.IsValidTarget(W.Range))
                            {
                                W.Cast(target);
                            }
                        }
                    }

                    if (InHarass && Menu["Ezreal_Harass"]["Q"] && !Me.IsUnderEnemyTurret())
                    {
                        var target = Args.Target as AIHeroClient;

                        if (target.IsValidTarget(Q.Range) && Q.IsReady() && Me.ManaPercent >= Menu["Ezreal_Harass"]["QMana"].GetValue<MenuSlider>().Value)
                        {
                            Q.Cast(target);
                        }
                    }
                }
            }

            if (Args.Type == OrbwalkingType.BeforeAttack)
            {
                if (Menu["Ezreal_LaneClear"]["W"] && W.IsReady() && Me.CountEnemyHeroesInRange(850) < 1)
                {
                    if (Args.Target is Obj_AI_Turret)
                    {
                        var target = Args.Target as Obj_AI_Turret;

                        if (W.IsReady() && Me.CountAllyHeroesInRange(W.Range) >= 1)
                        {
                            W.Cast(GameObjects.AllyHeroes.Find(x => W.IsInRange(x)));
                        }
                    }
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (Menu["Ezreal_Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Ezreal_Draw"]["W"] && W.IsReady())
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Ezreal_Draw"]["E"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Ezreal_Draw"]["AutoQ"])
            {
                var text = "";

                if (Menu["Ezreal_Auto"]["Q"].GetValue<MenuKeyBind>().Active)
                    text = "On";
                if (!Menu["Ezreal_Auto"]["Q"].GetValue<MenuKeyBind>().Active)
                    text = "Off";

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 60, System.Drawing.Color.Red, "Auto Q (" + Menu["Ezreal_Auto"]["Q"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 115, Me.HPBarPosition.Y - 60, System.Drawing.Color.Yellow, text);
            }

            if (Menu["Ezreal_Draw"]["AutoW"])
            {
                var text = "";

                if (Menu["Ezreal_Auto"]["W"].GetValue<MenuKeyBind>().Active)
                    text = "On";
                if (!Menu["Ezreal_Auto"]["W"].GetValue<MenuKeyBind>().Active)
                    text = "Off";

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, System.Drawing.Color.Red, "Auto W (" + Menu["Ezreal_Auto"]["W"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 115, Me.HPBarPosition.Y - 40, System.Drawing.Color.Yellow, text);
            }

            if (Menu["Ezreal_Draw"]["DrawDamage"])
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg((float)GetDamage(e), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Menu["Ezreal_Misc"]["Gapcloser"] && E.IsReady())
            {
                if (Args.IsDirectedToPlayer && Args.End.DistanceToPlayer() <= 200)
                {
                    E.Cast(Me.Position.Extend(Args.Sender.Position, -E.Range));
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu["Ezreal_Misc"]["AntiMelee"] && E.IsReady())
            {
                if (sender != null && sender.IsEnemy && Args.Target != null && Args.Target.IsMe)
                {
                    if (sender.Type == Me.Type && sender.IsMelee)
                    {
                        E.Cast(Me.Position.Extend(sender.Position, -E.Range));
                    }
                }
            }
        }
    }
}
