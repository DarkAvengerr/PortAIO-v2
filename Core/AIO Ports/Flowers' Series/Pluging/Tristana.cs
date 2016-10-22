using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Pluging
{
    using Common;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;
    using static Common.Common;

    internal class Tristana : Program
    {
        private new readonly Menu Menu = Championmenu;

        public Tristana()
        {
            Q = new Spell(SpellSlot.Q, 0f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 700f);

            W.SetSkillshot(0.50f, 250f, 1400f, false, SkillshotType.SkillshotCircle);


            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboQOnlyPassive", "Use Q | Only target Have E Buff", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R| Save MySelf", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboRHp", "Use R| When Player HealthPercent <= x%", true).SetValue(new Slider(20)));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                HarassMenu.AddItem(new MenuItem("HarassEToMinion", "Use E| To Minion", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    KillStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(),
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                FleeMenu.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var EMenu = MiscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    EMenu.AddItem(
                        new MenuItem("SemiE", "Semi-manual E Key", true).SetValue(new KeyBind('E', KeyBindType.Press)));
                    foreach (var target in HeroManager.Enemies)
                    {
                        EMenu.AddItem(
                            new MenuItem("Semi" + target.ChampionName.ToLower(), "E target: " + target.ChampionName, true)
                                .SetValue(true));
                    }
                }

                var RMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    RMenu.AddItem(new MenuItem("InterruptR", "Use R Interrupt Spell", true).SetValue(true));
                    RMenu.AddItem(new MenuItem("AntiR", "Use R Anti Gapcloser", true).SetValue(false));
                    RMenu.AddItem(new MenuItem("AntiRengar", "Use R Anti Rengar", true).SetValue(true));
                    RMenu.AddItem(new MenuItem("AntiKhazix", "Use R Anti Khazix", true).SetValue(true));
                }

                MiscMenu.AddItem(new MenuItem("Forcustarget", "Forcus Attack Passive Target", true).SetValue(true));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += AfterAttack;
            Drawing.OnDraw += OnDraw;
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (E.Level > 0)
            {
                E.Range = 630 + 7 * (Me.Level - 1);
            }
            if (R.Level > 0)
            {
                R.Range = 630 + 7 * (Me.Level - 1);
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
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("SemiE", true).GetValue<KeyBind>().Active && E.IsReady())
                    {
                        OneKeyCastE();
                    }
                    break;
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealE", true).GetValue<bool>() && E.IsReady())
            {

                foreach (
                    var target in
                    HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && CheckTargetSureCanKill(x) && x.Health <
                                                   Me.GetSpellDamage(x, SpellSlot.E)*
                                                   (x.GetBuffCount("TristanaECharge")*0.30) +
                                                   Me.GetSpellDamage(x, SpellSlot.E)))
                {
                    E.CastOnUnit(target, true);
                }
            }

            if (Menu.Item("KillStealR", true).GetValue<bool>() && R.IsReady())
            {
                if (Menu.Item("KillStealE", true).GetValue<bool>() && E.IsReady())
                {
                    foreach (
                        var target in
                        from x in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && CheckTargetSureCanKill(x))
                        let etargetstacks = x.Buffs.Find(buff => buff.Name == "TristanaECharge")
                        where
                        R.GetDamage(x) + E.GetDamage(x) + etargetstacks?.Count*0.30*E.GetDamage(x) >=
                        x.Health
                        select x)
                    {
                        R.CastOnUnit(target);
                        return;
                    }
                }
                else
                {
                    var target = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && CheckTargetSureCanKill(x))
                        .OrderByDescending(x => x.Health).FirstOrDefault(x => x.Health < R.GetDamage(x));

                    if (target != null)
                    {
                        R.CastOnUnit(target);
                    }
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, E.Range))
            {
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                {
                    if (Menu.Item("ComboQOnlyPassive", true).GetValue<bool>())
                    {
                        if (!E.IsReady() && target.HasBuff("TristanaECharge"))
                        {
                            Q.Cast();
                        }
                        else if (!E.IsReady() && !target.HasBuff("TristanaECharge") && E.Cooldown > 4)
                        {
                            Q.Cast();
                        }
                    }
                    else
                    {
                        Q.Cast();
                    }
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(target, true);
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() &&
                    Me.HealthPercent <= Menu.Item("ComboRHp", true).GetValue<Slider>().Value)
                {
                    var dangerenemy = HeroManager.Enemies.Where(e => e.IsValidTarget(R.Range)).
                        OrderBy(enemy => enemy.Distance(Me)).FirstOrDefault();

                    if (dangerenemy != null)
                    {
                        R.CastOnUnit(dangerenemy, true);
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
                if (E.IsReady())
                {
                    if (Menu.Item("HarassE", true).GetValue<bool>())
                    {
                        foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range)))
                        {
                            E.CastOnUnit(target, true);
                        }
                    }

                    if (Menu.Item("HarassEToMinion", true).GetValue<bool>())
                    {
                        foreach (var minion in MinionManager.GetMinions(E.Range).Where(m =>
                        m.Health < Me.GetAutoAttackDamage(m) && m.CountEnemiesInRange(m.BoundingRadius + 150) >= 1))
                        {
                            var etarget = E.GetTarget();

                            if (etarget != null)
                            {
                                return;
                            }

                            E.CastOnUnit(minion, true);
                            Orbwalker.ForceTarget(minion);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs =
                    MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).Where(x => !x.Name.ToLower().Contains("mini"));

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        E.CastOnUnit(mob, true);
                    }

                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && !E.IsReady())
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private void Flee()
        {
            if (Menu.Item("FleeW", true).GetValue<bool>() && W.IsReady())
            {
                W.Cast(Game.CursorPos);
            }
        }

        private void OneKeyCastE()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && CheckTargetSureCanKill(x))
            )
            {
                if (target.Health <
                    Me.GetSpellDamage(target, SpellSlot.E) * (target.GetBuffCount("TristanaECharge") * 0.30) +
                    Me.GetSpellDamage(target, SpellSlot.E))
                {
                    E.CastOnUnit(target, true);
                }

                if (Me.CountEnemiesInRange(1200) == 1)
                {
                    if (Me.HealthPercent >= target.HealthPercent && Me.Level + 1 >= target.Level)
                    {
                        E.CastOnUnit(target);
                    }
                    else if (Me.HealthPercent + 20 >= target.HealthPercent &&
                        Me.HealthPercent >= 40 && Me.Level + 2 >= target.Level)
                    {
                        E.CastOnUnit(target);
                    }
                }

                if (Menu.Item("Semi" + target.ChampionName.ToLower(), true).GetValue<bool>())
                {
                    E.CastOnUnit(target, true);
                }
            }
        }

        private void OnCreate(GameObject sender, EventArgs Args)
        {
            if (R.IsReady())
            {
                var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));
                var Khazix = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Khazix"));

                if (Menu.Item("AntiRengar", true).GetValue<bool>() && Rengar != null)
                {
                    if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(Me.Position) < R.Range)
                    {
                        R.CastOnUnit(Rengar, true);
                    }
                }

                if (Menu.Item("AntiKhazix", true).GetValue<bool>() && Khazix != null)
                {
                    if (sender.Name == "Khazix_Base_E_Tar.troy" && sender.Position.Distance(Me.Position) <= 300)
                    {
                        R.CastOnUnit(Khazix, true);
                    }
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("AntiR", true).GetValue<bool>() && R.IsReady())
            {
                if (gapcloser.End.Distance(Me.Position) <= 200 && gapcloser.Sender.IsValidTarget(R.Range))
                {
                    R.CastOnUnit(gapcloser.Sender);
                }
            }
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.Item("InterruptR").GetValue<bool>() && R.IsReady())
            {
                if (Args.DangerLevel >= Interrupter2.DangerLevel.High && sender.IsValidTarget(R.Range))
                {
                    R.CastOnUnit(sender);
                }
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Menu.Item("Forcustarget", true).GetValue<bool>())
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                    Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    foreach (
                        var enemy in
                        HeroManager.Enemies.Where(
                            enemy => Orbwalking.InAutoAttackRange(enemy) && enemy.HasBuff("TristanaEChargeSound")))
                    {
                        Orbwalker.ForceTarget(enemy);
                    }
                }
            }

            if (Args.Unit.IsMe && Orbwalking.InAutoAttackRange(Args.Target))
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        {
                            if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                            {
                                if (Menu.Item("ComboQOnlyPassive", true).GetValue<bool>())
                                {
                                    var Target = Args.Target.Type == GameObjectType.AIHeroClient
                                        ? (AIHeroClient) Args.Target
                                        : null;

                                    if (Target != null &&
                                        (Target.HasBuff("TristanaEChargeSound") || Target.HasBuff("TristanaECharge")))
                                    {
                                        Q.Cast();
                                    }
                                }
                                else
                                {
                                    Q.Cast();
                                }
                            }
                            break;
                        }
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {
                            if (Menu.Item("JungleClearQ", true).GetValue<bool>() &&
                                Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                            {
                                var minion =
                                    MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player),
                                            MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                                if (minion.Any(x => x.NetworkId == Args.Target.NetworkId))
                                {
                                    Q.Cast();
                                }
                            }

                            break;
                        }
                }
            }
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear &&
                Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                if (unit.IsMe && target != null &&
                    (target.Type == GameObjectType.obj_AI_Turret || target.Type == GameObjectType.obj_Turret))
                {
                    if (Menu.Item("LaneClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        E.CastOnUnit(target as Obj_AI_Base, true);

                        if (!Me.Spellbook.IsAutoAttacking && Me.CountEnemiesInRange(1000) == 0 &&
                            Menu.Item("LaneClearQ", true).GetValue<bool>())
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
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
                        var x in HeroManager.Enemies.Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg((float)ComboDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
                }
            }
        }
    }
}
