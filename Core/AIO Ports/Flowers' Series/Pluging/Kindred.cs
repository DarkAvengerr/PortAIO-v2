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

    internal class Kindred : Program
    {
        private new readonly Menu Menu = Championmenu;

        public Kindred()
        {
            Q = new Spell(SpellSlot.Q, 340f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 550f);
            R = new Spell(SpellSlot.R, 550f);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboAQA", "Use AA-Q-AA Logic", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
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
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(20)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                FleeMenu.AddItem(new MenuItem("FleeQ", "Use Q", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var QMenu = MiscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    QMenu.AddItem(new MenuItem("QCheck", "Use Q|Safe Check?", true).SetValue(true));
                    QMenu.AddItem(new MenuItem("QTurret", "Use Q|Dont Cast To Turret", true).SetValue(true));
                    QMenu.AddItem(new MenuItem("QMelee", "Use Q|Anti Melee", true).SetValue(true));
                }

                var RMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    RMenu.AddItem(new MenuItem("AutoR", "Auto R Save Myself?", true).SetValue(true));
                    RMenu.AddItem(
                        new MenuItem("AutoRHp", "Auto R| When Player Health Percent <= x%", true).SetValue(new Slider(15)));
                    RMenu.AddItem(new MenuItem("AutoSave", "Auto R Save Ally?", true).SetValue(true));
                    RMenu.AddItem(
                        new MenuItem("AutoSaveHp", "Auto R| When Ally Health Percent <= x%", true).SetValue(new Slider(20)));
                }

                MiscMenu.AddItem(new MenuItem("Forcus", "Forcus Attack Passive Target", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("ForcusE", "Forcus Attack E Mark Target", true).SetValue(true));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += AfterAttack;
            Drawing.OnDraw += OnDraw;
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || !sender.IsEnemy || Args.Target == null || Me.IsDead || Me.InFountain())
            {
                return;
            }

            switch (sender.Type)
            {
                case GameObjectType.AIHeroClient:
                    if (Args.Target.IsMe)
                    {
                        if (sender.IsMelee && Q.IsReady() && Menu.Item("QMelee", true).GetValue<bool>())
                        {
                            Q.Cast(Me.Position.Extend(sender.Position, -Q.Range));
                        }

                        if (R.IsReady() && Menu.Item("AutoR", true).GetValue<bool>())
                        {
                            if (Me.HealthPercent <= Menu.Item("AutoRHp", true).GetValue<Slider>().Value)
                            {
                                R.Cast();
                            }

                            if (Orbwalking.IsAutoAttack(Args.SData.Name))
                            {
                                if (sender.GetAutoAttackDamage(Me, true) >= Me.Health)
                                {
                                    R.Cast();
                                }
                            }
                            else
                            {
                                var target = (AIHeroClient)Args.Target;

                                if (target.GetSpellSlot(Args.SData.Name) != SpellSlot.Unknown)
                                {
                                    if (target.GetSpellDamage(Me, Args.SData.Name) > Me.Health)
                                    {
                                        if (Args.End.DistanceToPlayer() < 150 + Me.BoundingRadius)
                                        {
                                            R.Cast();
                                        }

                                        if (target.DistanceToPlayer() < 150 + Me.BoundingRadius)
                                        {
                                            R.Cast();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (Args.Target.IsAlly && Args.Target.Type == GameObjectType.AIHeroClient && !Args.Target.IsDead)
                    {
                        var ally = (AIHeroClient) Args.Target;

                        if (R.IsReady() && Menu.Item("AutoSave", true).GetValue<bool>() && ally.DistanceToPlayer() <= R.Range)
                        {
                            if (ally.HealthPercent <= Menu.Item("AutoSaveHp", true).GetValue<Slider>().Value)
                            {
                                R.Cast();
                            }

                            if (Orbwalking.IsAutoAttack(Args.SData.Name))
                            {
                                if (sender.GetAutoAttackDamage(ally, true) >= ally.Health)
                                {
                                    R.Cast();
                                }
                            }
                            else
                            {
                                var target = (AIHeroClient)Args.Target;

                                if (target.GetSpellSlot(Args.SData.Name) != SpellSlot.Unknown)
                                {
                                    if (target.GetSpellDamage(Me, Args.SData.Name) > Me.Health)
                                    {
                                        if (Args.End.DistanceToPlayer() < 150 + ally.BoundingRadius)
                                        {
                                            R.Cast();
                                        }

                                        if (target.DistanceToPlayer() < 150 + ally.BoundingRadius)
                                        {
                                            R.Cast();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case GameObjectType.obj_AI_Turret:
                    if (Args.Target.IsMe)
                    {
                        if (sender.IsMelee && Q.IsReady() && Menu.Item("QMelee", true).GetValue<bool>())
                        {
                            Q.Cast(Me.Position.Extend(sender.Position, -Q.Range));
                        }

                        if (R.IsReady() && Menu.Item("AutoR", true).GetValue<bool>())
                        {
                            if (sender.TotalAttackDamage > Me.Health)
                            {
                                R.Cast();
                            }
                        }
                    }
                    else if (Args.Target.IsAlly && Args.Target.Type == GameObjectType.AIHeroClient && !Args.Target.IsDead)
                    {
                        var ally = (AIHeroClient)Args.Target;

                        if (R.IsReady() && Menu.Item("AutoSave", true).GetValue<bool>() && ally.DistanceToPlayer() <= R.Range)
                        {
                            if (sender.TotalAttackDamage > ally.Health)
                            {
                                R.Cast();
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

        private void KillSteal()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && CheckTargetSureCanKill(x)))
                {
                    if (!target.IsValidTarget(Q.Range) || !(target.Health < Q.GetDamage(target)))
                    {
                        continue;
                    }

                    QLogic(target);
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
            {
                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                {
                    QLogic(target);
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
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
                {
                    if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.Cast(target);
                    }

                    if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }

                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        QLogic(target);
                    }
                }
            }
        }

        private void LaneClear()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (minions.Any())
                    {
                        if (minions.Count >= Menu.Item("LaneClearQCount", true).GetValue<Slider>().Value)
                        {
                            Q.Cast(Game.CursorPos);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                    {
                        E.Cast(mob, true);
                    }
                    else if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady())
                    {
                        W.Cast(mob, true);
                    }
                    else if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        Q.Cast(mob, true);
                    }
                }
            }
        }

        private void Flee()
        {
            if (Menu.Item("FleeQ", true).GetValue<bool>() && Q.IsReady())
            {
                Q.Cast(Me.ServerPosition.Extend(Game.CursorPos, Q.Range), true);
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var ForcusETarget =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                            x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) &&
                            x.HasBuff("kindredecharge"));

                var ForcusTarget =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                            x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) &&
                            x.HasBuff("kindredhittracker"));

                if (CheckTarget(ForcusETarget, Orbwalking.GetRealAutoAttackRange(Me)) &&
                    Menu.Item("ForcusE", true).GetValue<bool>())
                {
                    Orbwalker.ForceTarget(ForcusETarget);
                }
                else if (Menu.Item("Forcus", true).GetValue<bool>() &&
                         CheckTarget(ForcusTarget, Orbwalking.GetRealAutoAttackRange(Me)))
                {
                    Orbwalker.ForceTarget(ForcusTarget);
                }
                else
                {
                    Orbwalker.ForceTarget(null);
                }
            }
        }

        private void AfterAttack(AttackableUnit sender, AttackableUnit t)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Menu.Item("ComboAQA", true).GetValue<bool>())
                {
                    var target = t as AIHeroClient;

                    if (target != null && !target.IsDead && !target.IsZombie && Q.IsReady())
                    {
                        QLogic(target);
                    }
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
                if (Menu.Item("DrawW", true).GetValue<bool>() && (W.IsReady() || Me.HasBuff("kindredwclonebuffvisible")))
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

        private void QLogic(Obj_AI_Base target)
        {
            if (!Q.IsReady())
            {
                return;
            }

            var qPosition = Me.ServerPosition.Extend(Game.CursorPos, Q.Range);
            var targetDisQ = target.ServerPosition.Distance(qPosition);
            var canQ = false;

            if (Menu.Item("QTurret", true).GetValue<bool>() && qPosition.UnderTurret(true))
            {
                canQ = false;
            }

            if (Menu.Item("QCheck", true).GetValue<bool>())
            {
                if (qPosition.CountEnemiesInRange(300f) >= 3)
                {
                    canQ = false;
                }

                //Catilyn W
                if (ObjectManager
                        .Get<Obj_GeneralParticleEmitter>()
                        .FirstOrDefault(
                            x =>
                                x != null && x.IsValid &&
                                x.Name.ToLower().Contains("yordletrap_idle_red.troy") &&
                                x.Position.Distance(qPosition) <= 100) != null)
                {
                    canQ = false;
                }

                //Jinx E
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "k" &&
                                             x.Position.Distance(qPosition) <= 100) != null)
                {
                    canQ = false;
                }

                //Teemo R
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "Noxious Trap" &&
                                             x.Position.Distance(qPosition) <= 100) != null)
                {
                    canQ = false;
                }
            }

            if (targetDisQ >= Q.Range && targetDisQ <= Q.Range * 2)
            {
                canQ = true;
            }

            if (canQ)
            {
                Q.Cast(qPosition, true);
                canQ = false;
            }
        }
    }
}
