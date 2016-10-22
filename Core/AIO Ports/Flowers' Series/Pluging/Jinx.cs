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

    internal class Jinx : Program
    {
        private float bigGunRange;
        private float rCoolDown;
        private new readonly Menu Menu = Championmenu;

        public Jinx()
        {
            Q = new Spell(SpellSlot.Q, 525f);
            W = new Spell(SpellSlot.W, 1500f);
            E = new Spell(SpellSlot.E, 920f);
            R = new Spell(SpellSlot.R, 3000f);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 100f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRTeam", "Use R|Team Fight", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRSolo", "Use R|Solo Mode", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearQCount", "If Q CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    KillStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(),
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var WMenu = MiscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    WMenu.AddItem(new MenuItem("AutoW", "Auto W| When target Cant Move", true).SetValue(true));
                }

                var EMenu = MiscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    EMenu.AddItem(new MenuItem("AutoE", "Auto E| When target Cant Move", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("AutoETP", "Auto E| Teleport", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("GapE", "Anti GapCloser E", true).SetValue(true));
                }

                var RMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    RMenu.AddItem(
                        new MenuItem("RMenuSemi", "Semi R Key", true).SetValue(
                            new KeyBind('T', KeyBindType.Press)));
                    RMenu.AddItem(
                        new MenuItem("RMenuMin", "Use R| Min Range >= x", true).SetValue(new Slider(1000, 500, 2500)));
                    RMenu.AddItem(
                        new MenuItem("RMenuMax", "Use R| Man Range <= x", true).SetValue(new Slider(3000, 1500, 3500)));
                }
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Orbwalking.BeforeAttack += BeforeAttack;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;

            if (target.IsValidTarget(E.Range) &&
                (gapcloser.End.DistanceToPlayer() <= 300 || target.DistanceToPlayer() <= 300))
            {
                if (Menu.Item("GapE", true).GetValue<bool>() && E.IsReady())
                {
                    E.CastTo(target);
                }
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (!Q.IsReady())
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (Menu.Item("ComboQ", true).GetValue<bool>())
                    {
                        var target = Args.Target as AIHeroClient;

                        if (target != null && !target.IsDead && !target.IsZombie)
                        {
                            if (Me.HasBuff("JinxQ"))
                            {
                                if (target.Health < Me.GetAutoAttackDamage(target) * 3 &&
                                    target.DistanceToPlayer() <= Q.Range + 60)
                                {
                                    Q.Cast();
                                }
                                else if (Me.Mana < (rCoolDown == -1 ? 100 : (rCoolDown > 10 ? 130 : 150)))
                                {
                                    Q.Cast();
                                }
                                else if (target.IsValidTarget(Q.Range))
                                {
                                    Q.Cast();
                                }
                            }
                            else
                            {
                                if (target.CountEnemiesInRange(150) >= 2 &&
                                    Me.Mana > R.ManaCost + Q.ManaCost * 2 + W.ManaCost && target.DistanceToPlayer() > Q.Range)
                                {
                                    Q.Cast();
                                }
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (Me.UnderTurret(true))
                    {
                        return;
                    }

                    if (Menu.Item("HarassQ", true).GetValue<bool>())
                    {
                        var target = Args.Target as AIHeroClient;

                        if (target != null && !target.IsDead && !target.IsZombie)
                        {
                            if (Me.HasBuff("JinxQ"))
                            {
                                if (target.DistanceToPlayer() >= bigGunRange)
                                {
                                    Q.Cast();
                                }
                            }
                            else
                            {
                                if (target.CountEnemiesInRange(150) >= 2 &&
                                    Me.Mana > R.ManaCost + Q.ManaCost * 2 + W.ManaCost && target.DistanceToPlayer() > Q.Range)
                                {
                                    Q.Cast();
                                }
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (Me.UnderTurret(true))
                    {
                        return;
                    }

                    if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
                    {
                        if (Menu.Item("LaneClearQ", true).GetValue<bool>())
                        {
                            var min = Args.Target as Obj_AI_Base;
                            var minions = MinionManager.GetMinions(Me.Position, bigGunRange);

                            if (minions.Any() && min != null)
                            {
                                foreach (var minion in minions.Where(x => x.NetworkId != min.NetworkId))
                                {
                                    var count = ObjectManager.Get<Obj_AI_Minion>().Count(x => x.Distance(minion) <= 150);

                                    if (minion.DistanceToPlayer() <= bigGunRange)
                                    {
                                        if (Me.HasBuff("JinxQ"))
                                        {
                                            if (Menu.Item("LaneClearQCount", true).GetValue<Slider>().Value > count)
                                            {
                                                Q.Cast();
                                            }
                                            else if (min.Health > Me.GetAutoAttackDamage(min) * 1.1f)
                                            {
                                                Q.Cast();
                                            }
                                        }
                                        else if (!Me.HasBuff("JinxQ"))
                                        {
                                            if (Menu.Item("LaneClearQCount", true).GetValue<Slider>().Value <= count)
                                            {
                                                Q.Cast();
                                            }
                                            else if (min.Health < Me.GetAutoAttackDamage(min)*1.1f &&
                                                     min.DistanceToPlayer() > Q.Range)
                                            {
                                                Q.Cast();
                                            }
                                        }
                                    }
                                }

                                if (minions.Count <= 2 && Me.HasBuff("JinxQ"))
                                {
                                    Q.Cast();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Me.HasBuff("JinxQ") && Q.IsReady())
                        {
                            Q.Cast();
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

            if (Q.Level > 0)
            {
                bigGunRange = Q.Range + new[] {75, 100, 125, 150, 175}[Q.Level - 1];
            }

            if (R.Level > 0)
            {
                R.Range = Menu.Item("RMenuMax", true).GetValue<Slider>().Value;
            }

            rCoolDown = R.Level > 0
                ? (R.Instance.CooldownExpires - Game.Time < 0 ? 0 : R.Instance.CooldownExpires - Game.Time)
                : -1;

            AutoLogic();
            SemiRLogic();
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
            }
        }

        private void AutoLogic()
        {
            if (Menu.Item("AutoW", true).GetValue<bool>() && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && !x.CanMove()))
                {
                    W.Cast(target);
                }
            }

            if (Menu.Item("AutoE", true).GetValue<bool>() && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.CanMove()))
                {
                    E.Cast(target);
                }
            }

            if (Menu.Item("AutoETP", true).GetValue<bool>() && E.IsReady())
            {
                foreach (
                    var obj in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            x =>
                                x.IsEnemy && x.DistanceToPlayer() < E.Range &&
                                (x.HasBuff("teleport_target") || x.HasBuff("Pantheon_GrandSkyfall_Jump"))))
                {
                    E.Cast(obj.Position);
                }
            }
        }

        private void SemiRLogic()
        {
            if (Menu.Item("RMenuSemi", true).GetValue<KeyBind>().Active && R.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, R.Range))
                {
                    R.CastTo(target);
                }
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealW", true).GetValue<bool>() && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && x.Health < W.GetDamage(x)))
                {
                    if (Orbwalker.InAutoAttackRange(target) && target.Health <= Me.GetAutoAttackDamage(target, true))
                    {
                        continue;
                    }

                    W.CastTo(target);
                    return;
                }
            }

            if (Menu.Item("KillStealR", true).GetValue<bool>() && R.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.IsValidTarget(R.Range) &&
                            x.DistanceToPlayer() > Menu.Item("RMenuMin", true).GetValue<Slider>().Value &&
                            Menu.Item("KillStealR" + x.ChampionName.ToLower(), true).GetValue<bool>() &&
                            x.Health < R.GetDamage(x)))
                {
                    R.CastTo(target);
                    return;
                }
            }
        }

        private void Combo()
        {
            if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, W.Range) && target.DistanceToPlayer() > Q.Range)
                {
                    W.OktwCast(target);
                }
            }

            if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, E.Range))
                {
                    if (!target.CanMove())
                    {
                        E.Cast(target);
                    }
                    else
                    {
                        if (E.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                        {
                            E.Cast(E.GetPrediction(target).UnitPosition);
                        }
                    }
                }
            }

            if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(bigGunRange, TargetSelector.DamageType.Physical);

                if (Me.HasBuff("JinxQ"))
                {
                    if (Me.Mana < (rCoolDown == -1 ? 100 : (rCoolDown > 10 ? 130 : 150)))
                    {
                        Q.Cast();
                    }

                    if (Me.CountEnemiesInRange(1500) == 0)
                    {
                        Q.Cast();
                    }

                    if (target == null)
                    {
                        Q.Cast();
                    }
                    else if (CheckTarget(target, bigGunRange))
                    {
                        if (target.Health < Me.GetAutoAttackDamage(target) * 3 &&
                            target.DistanceToPlayer() <= Q.Range + 60)
                        {
                            Q.Cast();
                        }
                    }
                }
                else
                {
                    if (CheckTarget(target, bigGunRange))
                    {
                        if (Me.CountEnemiesInRange(Q.Range) == 0 && Me.CountEnemiesInRange(bigGunRange) > 0 &&
                            Me.Mana > R.ManaCost + W.ManaCost + Q.ManaCost*2)
                        {
                            Q.Cast();
                        }

                        if (target.CountEnemiesInRange(150) >= 2 &&
                            Me.Mana > R.ManaCost + Q.ManaCost * 2 + W.ManaCost && target.DistanceToPlayer() > Q.Range)
                        {
                            Q.Cast();
                        }
                    }
                }
            }

            if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(1200) && CheckTargetSureCanKill(x)))
                {
                    if (Menu.Item("ComboRTeam", true).GetValue<bool>() && target.IsValidTarget(600) &&
                        Me.CountEnemiesInRange(600) >= 2 &&
                        target.CountAlliesInRange(200) <= 3 && target.HealthPercent < 50)
                    {
                        R.CastTo(target);
                    }

                    if (Menu.Item("ComboRSolo", true).GetValue<bool>() && Me.CountEnemiesInRange(1500) <= 2 &&
                        target.DistanceToPlayer() > Q.Range &&
                        target.DistanceToPlayer() < bigGunRange &&
                        target.Health > Me.GetAutoAttackDamage(target) &&
                        target.Health < R.GetDamage(target) + Me.GetAutoAttackDamage(target)*3)
                    {
                        R.CastTo(target);
                    }
                }
            }
        }

        private void Harass()
        {
            if (Me.UnderTurret(true) || Me.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady())
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                    if (CheckTarget(target, W.Range) && target.DistanceToPlayer() > Q.Range)
                    {
                        W.CastTo(target);
                    }
                }

                if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(bigGunRange, TargetSelector.DamageType.Physical);

                    if (target.IsValidTarget(bigGunRange) && Orbwalking.CanAttack())
                    {
                        if (target.CountEnemiesInRange(150) >= 2 &&
                            Me.Mana > R.ManaCost + Q.ManaCost * 2 + W.ManaCost && target.DistanceToPlayer() > Q.Range)
                        {
                            Q.Cast();
                        }

                        if (target.DistanceToPlayer() > Q.Range && Me.Mana > R.ManaCost + Q.ManaCost*2 + W.ManaCost)
                        {
                            Q.Cast();
                        }
                    }
                    else
                    {
                        if (Me.HasBuff("JinxQ") && Q.IsReady())
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Me.Position, bigGunRange, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady())
                    {
                        W.Cast(mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini")));
                    }

                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        if (Me.HasBuff("JinxQ"))
                        {
                            foreach (var mob in mobs)
                            {
                                var count = ObjectManager.Get<Obj_AI_Minion>().Count(x => x.Distance(mob) <= 150);

                                if (mob.DistanceToPlayer() <= bigGunRange)
                                {
                                    if (count < 2)
                                    {
                                        Q.Cast();
                                    }
                                    else if (mob.Health > Me.GetAutoAttackDamage(mob) * 1.1f)
                                    {
                                        Q.Cast();
                                    }
                                }
                            }

                            if (mobs.Count < 2)
                            {
                                Q.Cast();
                            }
                        }
                        else
                        {
                            foreach (var mob in mobs)
                            {
                                var count = ObjectManager.Get<Obj_AI_Minion>().Count(x => x.Distance(mob) <= 150);

                                if (mob.DistanceToPlayer() <= bigGunRange)
                                {
                                    if (count >= 2)
                                    {
                                        Q.Cast();
                                    }
                                    else if (mob.Health < Me.GetAutoAttackDamage(mob) * 1.1f &&
                                             mob.DistanceToPlayer() > Q.Range)
                                    {
                                        Q.Cast();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Me.HasBuff("JinxQ") && Q.IsReady())
                    {
                        Q.Cast();
                    }
                }
            }
            else
            {
                if (Me.HasBuff("JinxQ") && Q.IsReady())
                {
                    Q.Cast();
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
