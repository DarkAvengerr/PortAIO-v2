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

    internal class Ezreal : Program
    {
        private float lastSpellCast;
        private new readonly Menu Menu = Championmenu;

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
                ComboMenu.AddItem(new MenuItem("ComboECheck", "Use E |Safe Check", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboEWall", "Use E |Wall Check", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
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
                var EMenu = MiscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    EMenu.AddItem(new MenuItem("Gapcloser", "Anti GapCloser", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("AntiMelee", "Anti Melee", true).SetValue(true));
                    EMenu.AddItem(
                        new MenuItem("AntiMeleeHp", "Anti Melee|When Player HealthPercent <= x%", true).SetValue(
                            new Slider(50)));
                }

                var RMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    RMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(true));
                    RMenu.AddItem(
                        new MenuItem("RRange", "Use R |Min Cast Range >= x", true).SetValue(new Slider(800, 0, 1500)));
                    RMenu.AddItem(
                        new MenuItem("RMaxRange", "Use R |Max Cast Range >= x", true).SetValue(new Slider(3000, 1500, 5000)));
                    RMenu.AddItem(
                        new MenuItem("RMinCast", "Use R| Min Hit Enemies >= x", true).SetValue(new Slider(2, 1, 6)));
                    RMenu.AddItem(
                        new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
                }

                var stackMenu = MiscMenu.AddSubMenu(new Menu("Auto Stack", "Auto Stack"));
                {
                    stackMenu.AddItem(new MenuItem("AutoStack", "Auto Stack?", true).SetValue(true));
                    stackMenu.AddItem(new MenuItem("AutoStackQ", "Use Q", true).SetValue(true));
                    stackMenu.AddItem(new MenuItem("AutoStackW", "Use W", true).SetValue(true));
                    stackMenu.AddItem(
                        new MenuItem("AutoStackMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(80)));
                }
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

            if (R.Level > 0)
            {
                R.Range = Menu.Item("RMaxRange", true).GetValue<Slider>().Value;
            }

            if (Menu.Item("SemiR", true).GetValue<KeyBind>().Active && R.IsReady())
            {
                OneKeyCastR();
            }

            AutoStackLogic();
            KillSteal();
            AutoRLogic();

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
            }
        }

        private void AutoStackLogic()
        {
            if (Me.IsRecalling())
            {
                return;
            }

            if (Menu.Item("AutoStack", true).GetValue<bool>() &&
                Me.ManaPercent >= Menu.Item("AutoStackMana", true).GetValue<Slider>().Value)
            {
                if (Utils.TickCount - lastSpellCast < 4100)
                {
                    return;
                }
             
                if (!Items.HasItem(3003) && !Items.HasItem(3004) && !Items.HasItem(3070))
                {
                    return;
                }

                if (Me.CountEnemiesInRange(Q.Range + E.Range) == 0 &&
                    !MinionManager.GetMinions(Me.Position, Q.Range + 200, MinionTypes.All, MinionTeam.NotAlly).Any())
                {
                    if (Menu.Item("AutoStackQ", true).GetValue<bool>() && Q.IsReady() &&
                        Utils.TickCount - lastSpellCast > 4100)
                    {
                        Q.Cast(Game.CursorPos);
                        lastSpellCast = Utils.TickCount;
                    }
                    else if (Menu.Item("AutoStackW", true).GetValue<bool>() && W.IsReady() &&
                             Utils.TickCount - lastSpellCast > 4100)
                    {
                        W.Cast(Game.CursorPos);
                        lastSpellCast = Utils.TickCount;
                    }
                }
            }
        }

        private void AutoRLogic()
        {
            if (Menu.Item("AutoR", true).GetValue<bool>() && R.IsReady() && Me.CountEnemiesInRange(1000) == 0)
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.IsValidTarget(R.Range) && CheckTargetSureCanKill(x) &&
                            x.DistanceToPlayer() >= Menu.Item("RRange", true).GetValue<Slider>().Value))
                {
                    if (!target.CanMove() && target.IsValidTarget(EQ.Range) &&
                        R.GetDamage(target) + Q.GetDamage(target)*3 >= target.Health + target.HPRegenRate*2)
                    {
                        R.Cast(target, true);
                    }

                    if (R.GetDamage(target) > target.Health + target.HPRegenRate*2 && target.Path.Length < 2 &&
                        R.GetPrediction(target, true).Hitchance >= HitChance.High)
                    {
                        R.Cast(target, true);
                    }

                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Me.CountEnemiesInRange(800) == 0)
                    {
                        R.CastIfWillHit(target, Menu.Item("RMinCast", true).GetValue<Slider>().Value, true);
                    }
                }
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

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(EQ.Range))
                {
                    if (Menu.Item("ComboECheck", true).GetValue<bool>() && !Me.UnderTurret(true) &&
                        Me.CountEnemiesInRange(1200) <= 2)
                    {
                        var useECombo = false;

                        if (target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                            CheckTargetSureCanKill(target) && HealthPrediction.GetHealthPrediction(target, 750) > 0)
                        {
                            if (target.Health < E.GetDamage(target) + Me.GetAutoAttackDamage(target) &&
                                target.Distance(Game.CursorPos) < Me.Distance(Game.CursorPos))
                            {
                                useECombo = true;
                            }

                            if (target.Health < E.GetDamage(target) + W.GetDamage(target) && W.IsReady() &&
                                target.Distance(Game.CursorPos) + 350 < Me.Distance(Game.CursorPos))
                            {
                                useECombo = true;
                            }

                            if (target.Health < E.GetDamage(target) + Q.GetDamage(target) && Q.IsReady() &&
                                target.Distance(Game.CursorPos) + 300 < Me.Distance(Game.CursorPos))
                            {
                                useECombo = true;
                            }
                        }

                        if (useECombo)
                        {
                            var CastEPos = Me.Position.Extend(target.Position, 475f);

                            if (Menu.Item("ComboEWall", true).GetValue<bool>())
                            {
                                if (NavMesh.GetCollisionFlags(CastEPos) != CollisionFlags.Wall &&
                                    NavMesh.GetCollisionFlags(CastEPos) != CollisionFlags.Building &&
                                    NavMesh.GetCollisionFlags(CastEPos) != CollisionFlags.Prop)
                                {
                                    E.Cast(CastEPos);
                                    useECombo = false;
                                }
                            }
                            else
                            {
                                E.Cast(CastEPos);
                                useECombo = false;
                            }
                        }
                    }
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
                {
                    if (Me.UnderTurret(true) || Me.CountEnemiesInRange(800) > 1)
                    {
                        return;
                    }

                    foreach (
                        var rTarget in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(R.Range) &&
                                target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                                CheckTargetSureCanKill(x) &&
                                HealthPrediction.GetHealthPrediction(x, 3000) > 0))
                    {
                        if (target.Health < R.GetDamage(rTarget) && R.GetPrediction(rTarget).Hitchance >= HitChance.High &&
                            target.DistanceToPlayer() > Q.Range + E.Range/2)
                        {
                            R.Cast(rTarget, true);
                        }

                        if (rTarget.IsValidTarget(Q.Range + E.Range) &&
                            R.GetDamage(rTarget) + (Q.IsReady() ? Q.GetDamage(rTarget) : 0) +
                            (W.IsReady() ? W.GetDamage(rTarget) : 0) > rTarget.Health + rTarget.HPRegenRate*2)
                        {
                            R.Cast(rTarget, true);
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
                        if (t is AIHeroClient)
                        {
                            var target = (AIHeroClient)Args.Target;

                            if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && t.IsValidTarget(Q.Range))
                            {
                                Q.CastTo(target);
                            }

                            if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && t.IsValidTarget(W.Range))
                            {
                                W.CastTo(target);
                            }
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
            if (Menu.Item("AntiMelee", true).GetValue<bool>() && E.IsReady() &&
                Me.HealthPercent <= Menu.Item("AntiMeleeHp", true).GetValue<Slider>().Value)
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
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
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
