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
    using System;
    using System.Linq;
    using static Common.Manager;

    public static class Tristana
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static HpBarDraw HpBarDraw = new HpBarDraw();

        private static Menu Menu => Program.Menu;
        private static AIHeroClient Me => Program.Me;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 700);
             W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 700);

            W.SetSkillshot(0.50f, 250f, 1400f, false, SkillshotType.SkillshotCircle);

            var ComboMenu = Menu.Add(new Menu("Tristana_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("alwaysQ", "Always Q"));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuSliderButton("Self", "Use R|When Player HealthPercent <=", 20, 0, 100, true));
            }

            var HarassMenu = Menu.Add(new Menu("Tristana_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("E", "Use E to Minion To Quick Harass", true));
            }

            var LaneClearMenu = Menu.Add(new Menu("Tristana_LaneClear", "LaneClear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q To Turret", true));
                LaneClearMenu.Add(new MenuBool("E", "Use E To Turret", true));
            }

            var JungleClearMenu = Menu.Add(new Menu("Tristana_JungleClear", "JungleClear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "When Player ManaPercent >= %", 30));
            }

            var KillStealMenu = Menu.Add(new Menu("Tristana_KillSteal", "KillSteal"));
            {
                KillStealMenu.Add(new MenuBool("R", "Use R", true));
                KillStealMenu.Add(new MenuBool("RE", "Use R + E", true));
            }

            var EListMenu = Menu.Add(new Menu("Tristana_EList", "E BlackList"));
            {
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => EListMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, false)));
                }
            }

            var MiscMenu = Menu.Add(new Menu("Tristana_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuKeyBind("Key", "Key", System.Windows.Forms.Keys.E, KeyBindType.Press));
                MiscMenu.Add(new MenuBool("Interrupt", "Interrupt Danger Spells", true));
                MiscMenu.Add(new MenuBool("GapCloser", "Anti GapCloser", true));
                MiscMenu.Add(new MenuBool("Khazix", "Anti Khazix", true));
                MiscMenu.Add(new MenuBool("Rengar", "Anti Rengar", true));
                MiscMenu.Add(new MenuBool("Forcus1", "Forcus Attack Have E Target", true));
            }

            var Draw = Menu.Add(new Menu("Tristana_Draw", "Draw"));
            {
                Draw.Add(new MenuBool("E", "Draw E Range"));
                Draw.Add(new MenuBool("EKS", "Draw E Kill"));
                Draw.Add(new MenuBool("RKS", "Draw R Kill"));
                Draw.Add(new MenuBool("Damage", "Draw Combo Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            Obj_AI_Base.OnLevelUp += OnLevelUp;
            Events.OnGapCloser += OnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Variables.Orbwalker.OnAction += OnAction;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (InCombo)
            {
                var target = GetTarget(E.Range);

                if (CheckTarget(target))
                {
                    if (Menu["Tristana_Combo"]["E"] && target.IsValidTarget(E.Range) && !Menu["Tristana_EList"][target.ChampionName.ToLower()] && E.IsReady())
                    {
                        E.CastOnUnit(target);
                    }

                    if (Q.IsReady() && Menu["Tristana_Combo"]["Q"] && !E.IsReady() && target.HasBuff("TristanaECharge"))
                    {
                        Q.Cast();
                    }

                    if (Menu["Tristana_Combo"]["Self"].GetValue<MenuSliderButton>().BValue && Me.HealthPercent <= Menu["Tristana_Combo"]["Self"].GetValue<MenuSliderButton>().Value)
                    {
                        var dangerenemy = GameObjects.EnemyHeroes.Where(e => R.CanCast(e)).OrderBy(enemy => enemy.DistanceToPlayer()).FirstOrDefault();

                        if (dangerenemy != null)
                            if (R.IsReady())
                                R.CastOnUnit(dangerenemy);
                    }
                }
            }

            if (InHarass)
            {
                if (Menu["Tristana_Harass"]["E"] && E.IsReady())
                {
                    foreach (var min in GameObjects.EnemyMinions.Where(m => m.IsValidTarget(E.Range) && m.Health < Me.GetAutoAttackDamage(m)
                    && m.CountEnemyHeroesInRange(m.BoundingRadius + 150) >= 1).ToList())
                    {
                        if (min != null)
                        {
                            E.CastOnUnit(min);
                            Variables.Orbwalker.Attack(min);
                            return;
                        }
                    }
                }
            }

            if (InClear)
            {
                if (Menu["Tristana_JungleClear"]["E"] && E.IsReady() && Me.ManaPercent >= Menu["Tristana_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    var mob = GetMobs(Me.Position, E.Range, true).Find(x => x.Health >= Me.GetAutoAttackDamage(x) * 5);

                    if (mob != null && mob.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(mob);
                    }
                }
            }

            if (Menu["Tristana_Misc"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                var target = Variables.TargetSelector.GetTarget(E.Range, DamageType.Physical);

                if (target == null || !target.IsValidTarget())
                    return;

                if (E.IsReady())
                {
                    if (target.Health < GetEDamage(target))
                        E.CastOnUnit(target);

                    if (Me.CountEnemyHeroesInRange(1200) == 1)
                    {
                        if (Me.HealthPercent >= target.HealthPercent && Me.Level + 1 >= target.Level)
                        {
                            E.CastOnUnit(target);
                        }
                        else if (Me.HealthPercent + 20 >= target.HealthPercent && Me.HealthPercent >= 40 && Me.Level + 2 >= target.Level)
                        {
                            E.CastOnUnit(target);
                        }
                    }

                    if (E.IsInRange(target) && !Menu["Tristana_EList"][target.ChampionName.ToLower()])
                    {
                        E.CastOnUnit(target);
                    }
                }
            }

            if (Menu["Tristana_KillSteal"]["RE"] && R.IsReady())
            {
                foreach (var enemy in from enemy in GameObjects.EnemyHeroes.Where(e => R.CanCast(e))
                                      let etargetstacks = enemy.Buffs.Find(buff => buff.Name == "TristanaECharge")
                                      where R.GetDamage(enemy) + E.GetDamage(enemy) + etargetstacks?.Count * 0.30 * E.GetDamage(enemy) >= enemy.Health
                                      select enemy)
                {
                    if (R.IsReady())
                    {
                        R.CastOnUnit(enemy);
                        return;
                    }
                }
            }

            if (Menu["Tristana_KillSteal"]["R"] && R.IsReady())
            {
                var Target = GameObjects.EnemyHeroes.OrderByDescending(x => x.Health).FirstOrDefault(x => x.IsValidTarget(R.Range) && x.Health < R.GetDamage(x));

                if (Target != null)
                    if (R.IsReady())
                        R.CastOnUnit(Target);
            }
        }

        private static void OnCreate(GameObject sender, EventArgs Args)
        {
            var Rengar = GameObjects.EnemyHeroes.Find(heros => heros.ChampionName.Equals("Rengar"));
            var Khazix = GameObjects.EnemyHeroes.Find(heros => heros.ChampionName.Equals("Khazix"));

            if (Menu["Tristana_Misc"]["Rengar"] && Rengar != null)
            {
                if (sender.Name == ("Rengar_LeapSound.troy") && sender.Position.DistanceToPlayer() < R.Range)
                {
                    R.Cast(Rengar);
                }
            }

            if (Menu["Tristana_Misc"]["Khazix"] && Khazix != null)
            {
                if (sender.Name == ("Khazix_Base_E_Tar.troy") && sender.Position.DistanceToPlayer() <= 300)
                {
                    R.Cast(Khazix);
                }
            }
        }

        private static void OnLevelUp(Obj_AI_Base sender, EventArgs Args)
        {
            if (sender.IsMe)
            {
                Q.Range = 600 + 5 * (Me.Level - 1);
                E.Range = 630 + 7 * (Me.Level - 1);
                R.Range = 630 + 7 * (Me.Level - 1);
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Args.IsDirectedToPlayer)
            {
                if (Menu["Tristana_Misc"]["GapCloser"] && R.IsReady() && Args.End.DistanceToPlayer() <= 200 && Args.Sender.IsValidTarget(R.Range))
                {
                    R.CastOnUnit(Args.Sender);
                }
            }
        }

        private static void OnInterruptableTarget(object obj, Events.InterruptableTargetEventArgs Args)
        {
            var target = Args.Sender;

            if (target.IsEnemy && target.IsCastingInterruptableSpell() && Menu["Tristana_Misc"]["Interrupt"] && R.IsReady() && target.IsValidTarget(R.Range))
            {
                R.CastOnUnit(target);
            }
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack)
            {
                if (Menu["Tristana_Misc"]["Forcus1"] && (InCombo || InHarass))
                {
                    foreach (var e in GetEnemies(GetAttackRange(Me) + 300))
                    {
                        if (e != null && e.HasBuff("TristanaEChargeSound") && e.IsValidTarget(GetAttackRange(Me) + 300))
                        {
                            Variables.TargetSelector.SetTarget(e);
                        }
                    }
                }

                if (InAutoAttackRange(Args.Target) && Q.IsReady())
                {
                    if (InCombo && Args.Target is AIHeroClient)
                    {
                        var Target = Args.Target.Type == GameObjectType.AIHeroClient ? (AIHeroClient)Args.Target : null;

                        if (Target != null && !E.IsReady() && (Target.HasBuff("TristanaEChargeSound") || Target.HasBuff("TristanaECharge")) && Menu["Tristana_Combo"]["Q"])
                        {
                            Q.Cast();
                        }
                        else if (Target != null && !E.IsReady() && !(Target.HasBuff("TristanaEChargeSound") || Target.HasBuff("TristanaECharge")) && Menu["Tristana_Combo"]["Q"])
                        {
                            Q.Cast();
                        }
                        else if (Menu["Tristana_Combo"]["alwaysQ"])
                        {
                            Q.Cast();
                        }
                    }

                    if (InClear && Args.Target is Obj_AI_Minion)
                    {
                        var mobs = GetMobs(Me.Position, 800);
                        var target = (Obj_AI_Minion)Args.Target;
                        if (mobs.Count() > 0)
                        {
                            foreach (var mob in mobs)
                            {
                                if (mob.CharData.BaseSkinName == target.CharData.BaseSkinName && Menu["Tristana_JungleClear"]["Q"] &&
                                    Me.ManaPercent >= Menu["Tristana_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                                {
                                    Q.Cast();
                                }
                            }
                        }
                    }
                }
            }

            if (Args.Type == OrbwalkingType.AfterAttack && InClear && (Args.Target is Obj_AI_Turret || Args.Target.Type == GameObjectType.obj_AI_Turret))
            {
                if (Menu["Tristana_LaneClear"]["E"] && E.IsReady())
                {
                    E.CastOnUnit(Args.Target as Obj_AI_Base);
                }

                var t = Args.Target as Obj_AI_Turret;

                if (Q.IsReady() && Menu["Tristana_LaneClear"]["Q"] && Me.CountEnemyHeroesInRange(1000) < 1 && (t.HasBuff("TristanaEChargeSound") || t.HasBuff("TristanaECharge")))
                {
                    Q.Cast();
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (Menu["Tristana_Draw"]["E"])
            {
                var ETurret = ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(t => !t.IsDead && t.HasBuff("TristanaECharge"));
                var ETarget = GameObjects.EnemyHeroes.FirstOrDefault(e => !e.IsDead && e.HasBuff("TristanaECharge"));

                if (ETurret != null)
                {
                    var eturretstacks = ETurret.Buffs.Find(buff => buff.Name == "TristanaECharge").Count;

                    if (ETurret.Health < (E.GetDamage(ETurret) + (((eturretstacks * 0.30)) * E.GetDamage(ETurret))))
                    {
                        Drawing.DrawCircle(ETurret.Position, 300 + ETurret.BoundingRadius, System.Drawing.Color.Red);
                    }
                    else if (ETurret.Health > (E.GetDamage(ETurret) + (((eturretstacks * 0.30)) * E.GetDamage(ETurret))))
                    {
                        Drawing.DrawCircle(ETurret.Position, 300 + ETurret.BoundingRadius, System.Drawing.Color.Orange);
                    }
                }

                if (ETarget != null)
                {
                    var etargetstacks = ETarget.Buffs.Find(buff => buff.Name == "TristanaECharge").Count;

                    if (ETarget.Health < (E.GetDamage(ETarget) + (((etargetstacks * 0.30)) * E.GetDamage(ETarget))))
                    {
                        Drawing.DrawCircle(ETarget.Position, 150 + ETarget.BoundingRadius, System.Drawing.Color.Red);
                    }
                    else if (ETarget.Health > (E.GetDamage(ETarget) + (((etargetstacks * 0.30)) * E.GetDamage(ETarget))))
                    {
                        Drawing.DrawCircle(ETarget.Position, 150 + ETarget.BoundingRadius, System.Drawing.Color.Orange);
                    }
                }
            }

            if (Menu["Tristana_Draw"]["EKS"])
            {
                foreach (var Target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && x.Health < GetEDamage(x)))
                {
                    var TargetPos = Drawing.WorldToScreen(Target.Position);
                    Render.Circle.DrawCircle(Target.Position, Target.BoundingRadius, System.Drawing.Color.Orange);
                    Drawing.DrawText(TargetPos.X, TargetPos.Y - 50, System.Drawing.Color.Orange, "Kill For E");
                }
            }

            if (Menu["Tristana_Draw"]["RKS"] && R.IsReady())
            {
                foreach (var Target in GameObjects.EnemyHeroes.Where(x => x.Health < R.GetDamage(x) && !x.IsZombie))
                {
                    var TargetPos = Drawing.WorldToScreen(Target.Position);
                    Render.Circle.DrawCircle(Target.Position, Target.BoundingRadius, System.Drawing.Color.Orange);
                    Drawing.DrawText(TargetPos.X, TargetPos.Y - 20, System.Drawing.Color.Orange, "Kill For R");
                }
            }

            if (Menu["Tristana_Draw"]["Damage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && !x.IsDead && !x.IsZombie))
                {
                    if (target != null)
                    {
                        HpBarDraw.Unit = target;

                        HpBarDraw.DrawDmg((float)GetDamage(target), new SharpDX.ColorBGRA(255, 200, 0, 170));
                    }
                }
            }
        }

        private static double GetEDamage(Obj_AI_Base target)
        {
            return Damage.GetSpellDamage(Me, target, SpellSlot.E) * (target.GetBuffCount("TristanaECharge") * 0.30) + Damage.GetSpellDamage(Me, target, SpellSlot.E);
        }
    }
}
