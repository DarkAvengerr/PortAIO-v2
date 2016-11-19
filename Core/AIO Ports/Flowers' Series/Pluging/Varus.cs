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

    internal class Varus : Program
    {
        private float qRange = 1600f;
        private new readonly Menu Menu = Championmenu;

        public Varus()
        {
            Q = new Spell(SpellSlot.Q, 925f);
            W = new Spell(SpellSlot.Q, 0);
            E = new Spell(SpellSlot.E, 975f);
            R = new Spell(SpellSlot.R, 1050f);

            Q.SetSkillshot(0.25f, 70f, 1650f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.35f, 120f, 1500f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 120f, 1950f, false, SkillshotType.SkillshotLine);

            Q.SetCharged("VarusQ", "VarusQ", 925, 1600, 1.5f);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRSolo", "Use R|Logic?", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R|Counts Enemies In Range >= x", true).SetValue(new Slider(3, 1, 5)));
                ComboMenu.AddItem(
                    new MenuItem("ComboPassive", "Use Spell|When target Have x Passive", true).SetValue(new Slider(3, 0, 3)));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                HarassMenu.AddItem(
                    new MenuItem("AutoHarass", "Auto Harass?", true).SetValue(new KeyBind('G', KeyBindType.Toggle)));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearQCount", "If Q CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearECount", "If E CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(
                    new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Orbwalking.BeforeAttack += BeforeAttack;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            Args.Process = !Q.IsCharging;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            SemiRLogic();
            AutoHarass();
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

        private void SemiRLogic()
        {
            if (Menu.Item("SemiR", true).GetValue<KeyBind>().Active && R.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, R.Range))
                {
                    R.CastTo(target);
                }
            }
        }

        private void AutoHarass()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Menu.Item("AutoHarass", true).GetValue<KeyBind>().Active &&
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo &&
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed && 
                !Me.IsRecalling())
            {
                Harass();
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(qRange) && x.Health < Q.GetDamage(x)))
                {
                    CastQ(target);
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
                         TargetSelector.GetTarget(qRange, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, qRange))
            {
                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu.Item("ComboRSolo", true).GetValue<bool>() && Me.CountEnemiesInRange(1000) <= 2)
                    {
                        if (target.Health + target.HPRegenRate * 2 <
                            R.GetDamage(target) + W.GetDamage(target) + (E.IsReady() ? E.GetDamage(target) : 0) +
                            (Q.IsReady() ? Q.GetDamage(target) : 0) + Me.GetAutoAttackDamage(target) * 3)
                        {
                            R.CastTo(target);
                        }
                    }

                    var rPred = R.GetPrediction(target, true);

                    if (rPred.AoeTargetsHitCount >= Menu.Item("ComboRCount", true).GetValue<Slider>().Value ||
                        Me.CountEnemiesInRange(R.Range) >= Menu.Item("ComboRCount", true).GetValue<Slider>().Value)
                    {
                        R.CastTo(target);
                    }
                }

                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(qRange))
                {
                    if (target.DistanceToPlayer() >= Orbwalking.GetRealAutoAttackRange(Me) + 100 ||
                        GetPassiveCount(target) >= Menu.Item("ComboPassive", true).GetValue<Slider>().Value ||
                        W.Level == 0 || target.Health < Q.GetDamage(target))
                    {
                        CastQ(target);
                    }
                }

                if (Q.IsCharging)
                {
                    return;
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (GetPassiveCount(target) >= Menu.Item("ComboPassive", true).GetValue<Slider>().Value ||
                        W.Level == 0 || target.Health < E.GetDamage(target))
                    {
                        E.CastTo(target);
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
                var target = TargetSelector.GetTarget(qRange, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, qRange))
                {
                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(qRange))
                    {
                        CastQ(target);
                    }

                    if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.CastTo(target);
                    }
                }
            }
        }

        private void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            { 
                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var qMinions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (qMinions.Any())
                    {
                        var qFarm =
                            MinionManager.GetBestLineFarmLocation(qMinions.Select(x => x.Position.To2D()).ToList(),
                                Q.Width, Q.Range);

                        if (qFarm.MinionsHit >= Menu.Item("LaneClearQCount", true).GetValue<Slider>().Value)
                        {
                            CastQ(null, qFarm.Position.To3D());
                        }
                    }
                }

                if (Menu.Item("LaneClearE", true).GetValue<bool>() && E.IsReady())
                {
                    var eMinions = MinionManager.GetMinions(Me.Position, E.Range);

                    if (eMinions.Any())
                    {
                        var eFarm = 
                            MinionManager.GetBestCircularFarmLocation(eMinions.Select(x => x.Position.To2D()).ToList(),
                                E.Width, E.Range);

                        if (eFarm.MinionsHit >= Menu.Item("LaneClearECount", true).GetValue<Slider>().Value)
                        {
                            E.Cast(eFarm.Position);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Me.Position, qRange, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault(x => !x.Name.Contains("mini"));

                    if (mob != null)
                    {
                        if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && mob.IsValidTarget(qRange))
                        {
                            CastQ(mob);
                        }

                        if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady() && mob.IsValidTarget(E.Range))
                        {
                            E.Cast(mob.Position);
                        }
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

        private void CastQ(Obj_AI_Base target = null, Vector3 pos = new Vector3())
        {
            if (Q.IsCharging)
            {
                if (target != null)
                {
                    Q.CastTo(target);
                }
                else
                {
                    Q.Cast(pos, true);
                }
            }
            else
            {
                Q.StartCharging();
            }
        }

        private int GetPassiveCount(AIHeroClient target)
        {
            return target.HasBuff("varuswdebuff") ? target.GetBuffCount("varuswdebuff") : 0;
        }
    }
}
