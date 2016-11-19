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

    internal class Vayne : Program
    {
        private new readonly Menu Menu = Championmenu;

        public Vayne()
        {
            Q = new Spell(SpellSlot.Q, 300f);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R);

            E.SetTargetted(0.25f, 1600f);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R|When Enemies Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                ComboMenu.AddItem(
                    new MenuItem("ComboRHp", "Use R|Or Player HealthPercent <= x%", true).SetValue(new Slider(45)));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassQ2Passive", "Use Q|Only Target have 2 Passive", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E|Only Target have 2 Passive", true).SetValue(false));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearQTurret", "Use Q|Attack Tower", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= %", true).SetValue(new Slider(60)));
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
                var QMenu = MiscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    QMenu.AddItem(new MenuItem("AQALogic", "Use AA-Q-AA Logic", true).SetValue(true));
                    QMenu.AddItem(new MenuItem("QCheck", "Use Q|Safe Check?", true).SetValue(true));
                    QMenu.AddItem(new MenuItem("QTurret", "Use Q|Dont Cast To Turret", true).SetValue(true));
                    QMenu.AddItem(new MenuItem("QMelee", "Use Q|Anti Melee", true).SetValue(true));
                }

                var EMenu = MiscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    EMenu.AddItem(
                        new MenuItem("ComboEMode", "Use E Mode:", true).SetValue(
                            new StringList(new[] { "Default", "VHR", "Marksman", "SharpShooter", "OKTW" })));
                    EMenu.AddItem(new MenuItem("ComboEPush", "Use E|Push Tolerance", true).SetValue(new Slider(0, -100)));
                    EMenu.AddItem(new MenuItem("Interrupt", "Interrupt Danger Spells", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("AntiAlistar", "Anti Alistar", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("AntiRengar", "Anti Rengar", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("AntiKhazix", "Anti Khazix", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("Gapcloser", "Anti Gapcloser", true).SetValue(true));
                    foreach (var target in HeroManager.Enemies)
                    {
                        EMenu.AddItem(new MenuItem("AntiGapcloser" + target.ChampionName.ToLower(),
                            "AntiGapcloser: " + target.ChampionName, true).SetValue(true));
                    }
                    EMenu.AddItem(new MenuItem("AutoE", "Auto E?", true).SetValue(true));
                    foreach (var target in HeroManager.Enemies)
                    {
                        EMenu.AddItem(new MenuItem("AutoE" + target.ChampionName.ToLower(),
                            "AutoE: " + target.ChampionName, true).SetValue(true));
                    }
                }

                var RMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    RMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(true));
                    RMenu.AddItem(
                        new MenuItem("AutoRCount", "Auto R|When Enemies Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                    RMenu.AddItem(
                        new MenuItem("AutoRRange", "Auto R|Search Enemies Range", true).SetValue(new Slider(600, 500, 1200)));
                }

                MiscMenu.AddItem(new MenuItem("Forcus", "Force 2 Passive Target", true).SetValue(true));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += AfterAttack;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            GameObject.OnCreate += OnCreate;
            Drawing.OnDraw += OnDraw;
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsMelee || sender.Type != Me.Type || Args.Target == null)
            {
                return;
            }

            if (Args.Target.IsMe)
            {
                if (Menu.Item("QMelee", true).GetValue<bool>() && Q.IsReady())
                {
                    Q.Cast(Me.Position.Extend(sender.Position, -Q.Range));
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            AutoLogic();

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

        private void AutoLogic()
        {
            if (Menu.Item("AutoE", true).GetValue<bool>() && E.IsReady() && !Me.UnderTurret(true))
            {
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                {
                    foreach (
                        var target in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(E.Range) && !x.HasBuffOfType(BuffType.SpellShield) &&
                                Menu.Item("AutoE" + x.ChampionName.ToLower(), true).GetValue<bool>()))
                    {
                        if (CheckTarget(target, E.Range))
                        {
                            ELogic(target);
                        }
                    }
                }
            }

            if (Menu.Item("AutoR", true).GetValue<bool>() && R.IsReady() &&
                Me.CountEnemiesInRange(Menu.Item("AutoRRange", true).GetValue<Slider>().Value) >=
                Menu.Item("AutoRCount", true).GetValue<Slider>().Value)
            {
                R.Cast();
            }
        }

        private void Combo()
        {
            if (Me.Spellbook.IsAutoAttacking)
            {
                return;             
            }

            if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
            {
                if (Me.CountEnemiesInRange(800) >= Menu.Item("ComboRCount", true).GetValue<Slider>().Value)
                {
                    R.Cast();
                }

                if (Me.CountEnemiesInRange(600) >= 1 &&
                    Me.HealthPercent <= Menu.Item("ComboRHp", true).GetValue<Slider>().Value)
                {
                    R.Cast();
                }
            }

            if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, E.Range))
                {
                    ELogic(target);
                }
            }

            if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, 800f))
                {
                    if (Menu.Item("AQALogic", true).GetValue<bool>() && Orbwalking.InAutoAttackRange(target))
                    {
                        return;
                    }

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
                if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady())
                {
                    var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                    if (CheckTarget(target, E.Range) && target.HasBuff("VayneSilveredDebuff") &&
                        target.GetBuffCount("VayneSilveredDebuff") == 2)
                    {
                        E.CastOnUnit(target);
                    }
                }

                if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(800f, TargetSelector.DamageType.Magical);

                    if (CheckTarget(target, 800f))
                    {
                        QLogic(target);
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
                    var minions =
                        MinionManager.GetMinions(Me.Position, 700)
                            .Where(m => m.Health < Q.GetDamage(m) + Me.GetAutoAttackDamage(m));

                    var minion = minions.FirstOrDefault();

                    if (minion != null)
                    {
                        if (minion.Distance(Me.Position.Extend(Game.CursorPos, Q.Range)) <=
                            Orbwalking.GetRealAutoAttackRange(Me))
                        {
                            Q.Cast(Me.Position.Extend(Game.CursorPos, Q.Range));
                            Orbwalker.ForceTarget(minions.FirstOrDefault());
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                {
                    var mob =
                        MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                                MinionOrderTypes.MaxHealth)
                            .FirstOrDefault(
                                x =>
                                    !x.Name.ToLower().Contains("mini") && !x.Name.ToLower().Contains("baron") &&
                                    !x.Name.ToLower().Contains("dragon") && !x.Name.ToLower().Contains("crab") &&
                                    !x.Name.ToLower().Contains("herald"));

                    if (mob != null && mob.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(mob, true);
                    }
                }
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                var ForcusTarget =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                            x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) &&
                            x.HasBuff("VayneSilveredDebuff") && x.GetBuffCount("VayneSilveredDebuff") == 2);

                if (Menu.Item("Forcus", true).GetValue<bool>() && CheckTarget(ForcusTarget))
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
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Menu.Item("AQALogic", true).GetValue<bool>())
                {
                    var target = t as AIHeroClient;

                    if (target != null && !target.IsDead && !target.IsZombie && Q.IsReady())
                    {
                        QLogic(target);
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (t is Obj_AI_Turret)
                {
                    if (Menu.Item("LaneClearQ", true).GetValue<bool>() &&
                        Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value &&
                        Menu.Item("LaneClearQTurret", true).GetValue<bool>() &&
                        Me.CountEnemiesInRange(900) == 0 && Q.IsReady())
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
                else if (t is Obj_AI_Minion)
                {
                    if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                    {
                        if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            var mobs =
                                MinionManager.GetMinions(Me.Position, 700, MinionTypes.All, MinionTeam.Neutral,
                                    MinionOrderTypes.MaxHealth);

                            if (mobs.Any())
                            {
                                Q.Cast(mobs.FirstOrDefault());
                            }
                        }
                    }
                }
            }
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.Item("Interrupt", true).GetValue<bool>() && E.IsReady() && sender.IsEnemy &&
                sender.IsValidTarget(E.Range))
            {
                if (Args.DangerLevel >= Interrupter2.DangerLevel.High)
                {
                    E.CastOnUnit(sender, true);
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (E.IsReady())
            {
                if (Menu.Item("AntiAlistar", true).GetValue<bool>() && Args.Sender.ChampionName == "Alistar" &&
                    Args.SkillType == GapcloserType.Targeted)
                {
                    E.CastOnUnit(Args.Sender, true);
                }

                if (Menu.Item("Gapcloser", true).GetValue<bool>() &&
                    Menu.Item("AntiGapcloser" + Args.Sender.ChampionName.ToLower(), true).GetValue<bool>())
                {
                    if (Args.Sender.DistanceToPlayer() <= 200 && Args.Sender.IsValid)
                    {
                        E.CastOnUnit(Args.Sender, true);
                    }
                }
            }
        }

        private void OnCreate(GameObject sender, EventArgs Args)
        {
            var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));
            var Khazix = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Khazix"));

            if (Rengar != null && Menu.Item("AntiRengar", true).GetValue<bool>())
            {
                if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(Me.Position) < E.Range)
                {
                    E.CastOnUnit(Rengar);
                }
            }

            if (Khazix != null && Menu.Item("AntiKhazix", true).GetValue<bool>())
            {
                if (sender.Name == "Khazix_Base_E_Tar.troy" && sender.Position.Distance(Me.Position) <= 300)
                {
                    E.CastOnUnit(Khazix);
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
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

            if (targetDisQ >= Q.Range && targetDisQ <= Q.Range*2)
            {
                canQ = true;
            }

            if (canQ)
            {
                Q.Cast(qPosition, true);
                canQ = false;
            }
        }

        private void ELogic(Obj_AI_Base target)
        {
            if (target != null)
            {
                switch (Menu.Item("ComboEMode", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        {
                            var EPred = E.GetPrediction(target);
                            var PD = 425 + Menu.Item("ComboEPush", true).GetValue<Slider>().Value;
                            var PP = EPred.UnitPosition.Extend(Me.Position, -PD);

                            for (int i = 1; i < PD; i += (int)target.BoundingRadius)
                            {
                                var VL = EPred.UnitPosition.Extend(Me.Position, -i);
                                var J4 = ObjectManager.Get<Obj_AI_Base>()
                                    .Any(f => f.Distance(PP) <= target.BoundingRadius && f.Name.ToLower() == "beacon");
                                var CF = NavMesh.GetCollisionFlags(VL);

                                if (CF.HasFlag(CollisionFlags.Wall) || CF.HasFlag(CollisionFlags.Building) || J4)
                                {
                                    E.CastOnUnit(target);
                                    return;
                                }
                            }
                        }
                        break;
                    case 1:
                        {
                            var pushDistance = 425 + Menu.Item("ComboEPush", true).GetValue<Slider>().Value;
                            var Prediction = E.GetPrediction(target);
                            var endPosition = Prediction.UnitPosition.Extend
                                (Me.ServerPosition, -pushDistance);

                            if (Prediction.Hitchance >= HitChance.VeryHigh)
                            {
                                if (endPosition.IsWall())
                                {
                                    var condemnRectangle = new Geometry.Polygon.Rectangle(target.ServerPosition.To2D(),
                                        endPosition.To2D(), target.BoundingRadius);

                                    if (
                                        condemnRectangle.Points.Count(
                                            point =>
                                                NavMesh.GetCollisionFlags(point.X, point.Y)
                                                    .HasFlag(CollisionFlags.Wall)) >=
                                        condemnRectangle.Points.Count*(20/100f))
                                    {
                                        E.CastOnUnit(target);
                                    }
                                }
                                else
                                {
                                    var step = pushDistance / 5f;
                                    for (float i = 0; i < pushDistance; i += step)
                                    {
                                        var endPositionEx = Prediction.UnitPosition.Extend(Me.ServerPosition, -i);
                                        if (endPositionEx.IsWall())
                                        {
                                            var condemnRectangle =
                                                new Geometry.Polygon.Rectangle(target.ServerPosition.To2D(),
                                                    endPosition.To2D(), target.BoundingRadius);

                                            if (
                                                condemnRectangle.Points.Count(
                                                    point =>
                                                        NavMesh.GetCollisionFlags(point.X, point.Y)
                                                            .HasFlag(CollisionFlags.Wall)) >=
                                                condemnRectangle.Points.Count*(20/100f))
                                            {
                                                E.CastOnUnit(target);
                                            }
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            for (var i = 1; i < 8; i++)
                            {
                                var targetBehind = target.Position +
                                                   Vector3.Normalize(target.ServerPosition - Me.Position)*i*50;

                                if (targetBehind.IsWall() && target.IsValidTarget(E.Range))
                                {
                                    E.CastOnUnit(target);
                                    return;
                                }
                            }
                        }
                        break;
                    case 3:
                        {
                            var prediction = E.GetPrediction(target);

                            if (prediction.Hitchance >= HitChance.High)
                            {
                                var finalPosition = prediction.UnitPosition.Extend(Me.Position, -400);

                                if (finalPosition.IsWall())
                                {
                                    E.CastOnUnit(target);
                                    return;
                                }

                                for (var i = 1; i < 400; i += 50)
                                {
                                    var loc3 = prediction.UnitPosition.Extend(Me.Position, -i);

                                    if (loc3.IsWall())
                                    {
                                        E.CastOnUnit(target);
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    case 4:
                        {
                            var prepos = E.GetPrediction(target);
                            float pushDistance = 470;
                            var radius = 250;
                            var start2 = target.ServerPosition;
                            var end2 = prepos.CastPosition.Extend(Me.ServerPosition, -pushDistance);
                            var start = start2.To2D();
                            var end = end2.To2D();
                            var dir = (end - start).Normalized();
                            var pDir = dir.Perpendicular();
                            var rightEndPos = end + pDir * radius;
                            var leftEndPos = end - pDir * radius;
                            var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, Me.Position.Z);
                            var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, Me.Position.Z);
                            var step = start2.Distance(rEndPos) / 10;

                            for (var i = 0; i < 10; i++)
                            {
                                var pr = start2.Extend(rEndPos, step * i);
                                var pl = start2.Extend(lEndPos, step * i);

                                if (pr.IsWall() && pl.IsWall())
                                {
                                    E.CastOnUnit(target);
                                    return;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
