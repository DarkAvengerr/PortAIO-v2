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

    internal class Ezreal
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static Spell EQ;

        private static readonly Menu Menu = Program.Championmenu;
        private static readonly AIHeroClient Me = Program.Me;
        private static readonly Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        public Ezreal()
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

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRCheck", "Use R |Safe Check", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboRRange", "Use R |Min Cast Range >= x", true).SetValue(new Slider(800, 0, 1500)));
                ComboMenu.AddItem(
                 new MenuItem("ComboRMin", "Use R| Min Hit Enemies >= x", true).SetValue(new Slider(2, 1, 5)));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                HarassMenu.AddItem(new MenuItem("Harasstarget", "Harass List:", true));
                foreach (var target in HeroManager.Enemies)
                {
                    HarassMenu.AddItem(new MenuItem("Harasstarget" + target.ChampionName.ToLower(),
                        target.ChampionName, true).SetValue(true));
                }
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearQOut", "Use Q| Out of Attack Range Farm", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(20)));
            }

            var LastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                LastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q", true).SetValue(true));
                LastHitMenu.AddItem(
                    new MenuItem("LastHitMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(20)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "KillSteal Q", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealW", "KillSteal W", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(new MenuItem("Gapcloser", "Use E |Anti GapCloser", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("AntiMelee", "Use E |Anti Melee", true).SetValue(true));
                MiscMenu.AddItem(
                    new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
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
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("SemiR", true).GetValue<KeyBind>().Active)
                    {
                        OneKeyCastR();
                    }
                    break;
            }
        }

        private void KillSteal()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && CheckTargetSureCanKill(x)))
            {
                if (Q.GetDamage(target) > target.Health && Menu.Item("KillStealQ", true).GetValue<bool>())
                {
                    if (target.IsValidTarget(Q.Range))
                        Q.CastTo(target);
                }

                if (W.GetDamage(target) > target.Health && Menu.Item("KillStealW", true).GetValue<bool>())
                {
                    if (target.IsValidTarget(W.Range))
                        W.CastTo(target);
                }

                if (target.IsValidTarget(W.Range) && Menu.Item("KillStealQ", true).GetValue<bool>() &&
                     Menu.Item("KillStealW", true).GetValue<bool>())
                {
                    if (target.Health < Q.GetDamage(target) + W.GetDamage(target))
                    {
                        W.CastTo(target);
                        Q.CastTo(target);
                    }
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(EQ.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, EQ.Range))
            {
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.CastTo(target);
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.CastTo(target, true);
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(EQ.Range) && 
                    !Me.UnderTurret(true))
                {
                    var usee = false;

                    if (target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                        Me.CountEnemiesInRange(1200) <= 2 && CheckTargetSureCanKill(target))
                    {
                        if (HealthPrediction.GetHealthPrediction(target, 250) > 0)
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
                        else
                        {
                            usee = false;
                        }
                    }

                    if (usee)
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range));
                        usee = false;
                    }
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
                {
                    var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                    if (CheckTarget(rTarget))
                    {
                        if (Menu.Item("ComboRCheck", true).GetValue<bool>() &&
                            (Me.UnderTurret(true) || Me.CountEnemiesInRange(600) > 1))
                        {
                            return;
                        }

                        if (rTarget.DistanceToPlayer() < Menu.Item("ComboRRange", true).GetValue<Slider>().Value)
                        {
                            return;
                        }

                        if (rTarget.Health + rTarget.HPRegenRate * 2 <= R.GetDamage(target) &&
                            rTarget.IsValidTarget(1500))
                        {
                            R.CastTo(rTarget);
                        }
                        else if (rTarget.IsValidTarget(Q.Range + E.Range - 200) &&
                            rTarget.Health + rTarget.MagicShield + rTarget.HPRegenRate * 2 <=
                            R.GetDamage(rTarget) + Q.GetDamage(rTarget) + W.GetDamage(rTarget) &&
                            Q.IsReady() && W.IsReady() &&
                            rTarget.CountAlliesInRange(Q.Range + E.Range - 200) <= 1)
                        {
                            R.CastTo(rTarget);
                        }
                        else if (rTarget.IsValidTarget())
                        {
                            R.CastIfWillHit(rTarget, Menu.Item("ComboRMin", true).GetValue<Slider>().Value);
                        }
                    }
                }
            }
        }

        private void Harass()
        {
            if (!Me.UnderTurret(true))
            {
                if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
                {
                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        foreach (
                            var target in
                            HeroManager.Enemies.Where(
                                x =>
                                    x.IsValidTarget(Q.Range) &&
                                    Menu.Item("Harasstarget" + x.ChampionName.ToLower(), true).GetValue<bool>()))
                        {
                            Q.CastTo(target);
                        }
                    }

                    if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady())
                    {
                        foreach (
                            var target in
                            HeroManager.Enemies.Where(
                                x =>
                                    x.IsValidTarget(W.Range) &&
                                    Menu.Item("Harasstarget" + x.ChampionName.ToLower(), true).GetValue<bool>()))
                        {
                            W.CastTo(target, true);
                        }
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
                    var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (minions.Any())
                    {
                        if (Menu.Item("LaneClearQOut", true).GetValue<bool>())
                        {
                            var mins =
                                minions.Where(
                                    x =>
                                        x.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                                        x.Health < Q.GetDamage(x) &&
                                        HealthPrediction.GetHealthPrediction(x, 250) > 0);

                            Q.Cast(mins.Any() ? mins.FirstOrDefault() : minions.FirstOrDefault(), true);
                        }
                        else
                            Q.Cast(minions.FirstOrDefault(), true);
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        Q.Cast(mobs.FirstOrDefault(), true);
                    }
                }
            }
        }

        private void LastHit()
        {
            if (!Me.UnderTurret(true))
            {
                if (Me.ManaPercent >= Menu.Item("LastHitMana", true).GetValue<Slider>().Value)
                {
                    if (Menu.Item("LastHitQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        var minions =
                            MinionManager.GetMinions(Me.Position, Q.Range)
                                .Where(
                                    x =>
                                        x.DistanceToPlayer() <= Q.Range &&
                                        x.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                                        x.Health < Q.GetDamage(x));

                        if (minions.Any())
                        {
                            Q.Cast(minions.FirstOrDefault(), true);
                        }
                    }
                }
            }
        }

        private void OneKeyCastR()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(3000, TargetSelector.DamageType.Magical);

            if (CheckTarget(target, 3000f))
            {
                R.CastTo(target, true);
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("LaneClearW", true).GetValue<bool>() && W.IsReady() && Me.CountEnemiesInRange(850) == 0)
                {
                    var turret = Args.Target as Obj_AI_Turret;

                    if (turret != null)
                    {
                        if (W.IsReady() && Me.CountAlliesInRange(W.Range) >= 1)
                        {
                            W.Cast(HeroManager.Allies.Find(x => x.DistanceToPlayer() <= W.Range));
                        }
                    }
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                var t = (Obj_AI_Base)Args.Target;

                if (t != null && !t.IsDead && !t.IsZombie)
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        var target = (AIHeroClient) Args.Target;

                        if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && t.IsValidTarget(Q.Range))
                        {
                            Q.CastTo(target);
                        }

                        if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && t.IsValidTarget(W.Range))
                        {
                            W.CastTo(target);
                        }
                    }
                    else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    {
                        if (t is AIHeroClient)
                        {
                            var target = (AIHeroClient)Args.Target;

                            if (Menu.Item("Harasstarget" + target.ChampionName.ToLower(), true).GetValue<bool>() &&
                                Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
                            {
                                if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() && t.IsValidTarget(Q.Range))
                                {
                                    Q.CastTo(t);
                                }

                                if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady() && t.IsValidTarget(W.Range))
                                {
                                    W.CastTo(t);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (Menu.Item("Gapcloser", true).GetValue<bool>() && E.IsReady())
            {
                if (Args.End.DistanceToPlayer() <= 200)
                {
                    E.Cast(Me.Position.Extend(Args.Sender.Position, -E.Range));
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.Item("AntiMelee", true).GetValue<bool>() && E.IsReady())
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
    }
}
