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

    internal class Urgot : Program
    {
        private new readonly Menu Menu = Championmenu;

        public Urgot()
        {
            Q = new Spell(SpellSlot.Q, 1000f);
            QExtend = new Spell(SpellSlot.Q, 1200f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 550f);

            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            QExtend.SetSkillshot(0.25f, 60f, 1600f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 120f, 1500f, false, SkillshotType.SkillshotCircle);


            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboWAlways", "Use W| Always Use?", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboWBuff", "Use W| If target have E buff", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboWLowHp", "Use W| If Player HealthPercent <= x%", true).SetValue(new Slider(50)));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboFirstE", "First Cast E -> Q Combo", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearECount", "If E CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
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
                KillStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var rMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(
                        new MenuItem("RSwap", "If After Swap Enemies Count <= x", true).SetValue(new Slider(3, 1, 5)));
                    rMenu.AddItem(new MenuItem("RAlly", "If Target Under Ally Turret", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("RSafe", "Dont Cast In Enemy Turret", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("RKill", "If Target Can Kill", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("DontrList", "Dont R List: ", true));
                    foreach (var target in HeroManager.Enemies)
                    {
                        rMenu.AddItem(
                            new MenuItem("Dontr" + target.ChampionName.ToLower(), target.ChampionName, true).SetValue(true));
                    }
                }
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = (AIHeroClient)Args.Target;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() &&
                        (Q.IsReady() || target.Health < E.GetDamage(target)))
                    {
                        E.CastTo(target);
                    }

                    if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady())
                    {
                        if (target.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me))
                        {
                            if (Menu.Item("ComboWAlways", true).GetValue<bool>())
                            {
                                W.Cast();
                            }

                            if (Me.HealthPercent <= Menu.Item("ComboWLowHp", true).GetValue<Slider>().Value)
                            {
                                W.Cast();
                            }
                        }

                        if (Menu.Item("ComboWBuff", true).GetValue<bool>() && HaveEBuff(target) && Q.IsReady())
                        {
                            W.Cast();
                        }
                    }

                    if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                    {
                        if (!HaveEBuff(target) && target.IsValidTarget(Q.Range))
                        {
                            if (Menu.Item("ComboFirstE", true).GetValue<bool>() && E.IsReady() &&
                                Menu.Item("ComboE", true).GetValue<bool>() && target.IsValidTarget(E.Range))
                            {
                                E.CastTo(target);
                            }
                            else
                            {
                                Q.CastTo(target);
                            }
                        }
                        else if (target.IsValidTarget(QExtend.Range) && HaveEBuff(target))
                        {
                            QExtend.Cast(target);
                        }
                    }

                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                {
                    var mobs = MinionManager.GetMinions(Me.Position, R.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var mob = mobs.FirstOrDefault();
                        var bigmob = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));

                        if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady() && bigmob != null &&
                            bigmob.IsValidTarget(W.Range))
                        {
                            W.Cast();
                        }

                        if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && mob != null &&
                            mob.IsValidTarget(Q.Range))
                        {
                            Q.Cast(mob);
                        }
                    }
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (R.Level > 0)
            {
                R.Range = new[] { 550f, 700f, 850f }[R.Level - 1];
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
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    Q.CastTo(target);
                    return;
                }
            }

            if (Menu.Item("KillStealE", true).GetValue<bool>() && E.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x)))
                {
                    E.CastTo(target);
                    return;
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, QExtend.Range))
            {
                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu.Item("RSafe", true).GetValue<bool>() && !Me.UnderTurret(true))
                    {
                        foreach (
                            var rTarget in
                            HeroManager.Enemies.Where(
                                    x =>
                                        x.IsValidTarget(R.Range) &&
                                        !Menu.Item("Dontr" + target.ChampionName.ToLower(), true).GetValue<bool>())
                                .OrderByDescending(x => E.IsReady() ? E.GetDamage(x) : 0 + Q.GetDamage(x)*2))
                        {
                            if (rTarget.CountEnemiesInRange(R.Range) <= Menu.Item("RSwap", true).GetValue<Slider>().Value)
                            {
                                if (Menu.Item("RAlly", true).GetValue<bool>() && Me.UnderAllyTurret() &&
                                    rTarget.DistanceToPlayer() <= 350)
                                {
                                    R.CastOnUnit(rTarget);
                                }

                                if (Menu.Item("RKill", true).GetValue<bool>() && target.Health < ComboDamage(target))
                                {
                                    R.CastOnUnit(rTarget);
                                }
                            }
                        }
                    }
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range) &&
                    (Q.IsReady() || target.Health < E.GetDamage(target)))
                {
                    E.CastTo(target);
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady())
                {
                    if (target.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me))
                    {
                        if (Menu.Item("ComboWAlways", true).GetValue<bool>())
                        {
                            W.Cast();
                        }

                        if (Me.HealthPercent <= Menu.Item("ComboWLowHp", true).GetValue<Slider>().Value)
                        {
                            W.Cast();
                        }
                    }

                    if (Menu.Item("ComboWBuff", true).GetValue<bool>() && HaveEBuff(target) && Q.IsReady())
                    {
                        W.Cast();
                    }
                }

                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                {
                    if (!HaveEBuff(target) && target.IsValidTarget(Q.Range))
                    {
                        if (Menu.Item("ComboFirstE", true).GetValue<bool>() && E.IsReady() &&
                            Menu.Item("ComboE", true).GetValue<bool>() && target.IsValidTarget(E.Range))
                        {
                            E.CastTo(target);
                        }
                        else
                        {
                            Q.CastTo(target);
                        }
                    }
                    else if (target.IsValidTarget(QExtend.Range) && HaveEBuff(target))
                    {
                        QExtend.Cast(target);
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
                var target = TargetSelector.GetSelectedTarget() ??
                    TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, QExtend.Range))
                {
                    if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.CastTo(target);
                    }

                    if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady())
                    {
                        if (HaveEBuff(target) && Q.IsReady())
                        {
                            W.Cast();
                        }
                    }

                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(QExtend.Range))
                    {
                        if (!HaveEBuff(target) && target.IsValidTarget(Q.Range))
                        {
                            Q.CastTo(target);
                        }
                        else if (target.IsValidTarget(QExtend.Range) && HaveEBuff(target))
                        {
                            QExtend.Cast(target);
                        }
                    }
                }
            }
        }

        private void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                var minions = MinionManager.GetMinions(Me.Position, R.Range);

                if (minions.Any())
                {
                    if (Menu.Item("LaneClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        var eMinions = MinionManager.GetMinions(Me.Position, E.Range);
                        var eFarm =
                            MinionManager.GetBestLineFarmLocation(eMinions.Select(x => x.Position.To2D()).ToList(),
                                E.Width, E.Range);

                        if (eFarm.MinionsHit >= Menu.Item("LaneClearECount", true).GetValue<Slider>().Value)
                        {
                            E.Cast(eFarm.Position);
                        }
                    }

                    if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        var qMinion =
                            MinionManager
                                .GetMinions(
                                    Me.Position, Q.Range)
                                .FirstOrDefault(
                                    x =>
                                        x.Health < Q.GetDamage(x) &&
                                        HealthPrediction.GetHealthPrediction(x, 250) > 0 &&
                                        x.Health > Me.GetAutoAttackDamage(x));

                        if (qMinion != null)
                        {
                            Q.Cast(qMinion);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Me.Position, R.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();
                    var bigmob = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));

                    if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        if (bigmob != null && bigmob.IsValidTarget(E.Range))
                        {
                            E.Cast(bigmob);
                        }
                        else
                        {
                            var eMobs = MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                                MinionOrderTypes.MaxHealth);

                            var eFarm =
                                MinionManager.GetBestLineFarmLocation(eMobs.Select(x => x.Position.To2D()).ToList(),
                                    E.Width, E.Range);

                            if (eFarm.MinionsHit >= 1)
                            {
                                E.Cast(eFarm.Position);
                            }
                        }
                    }

                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && mob != null &&
                        mob.IsValidTarget(Q.Range))
                    {
                        Q.Cast(mob);
                    }
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
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

        private bool HaveEBuff(AIHeroClient target)
        {
            return target.HasBuff("urgotcorrosivedebuff");
        }
    }
}
