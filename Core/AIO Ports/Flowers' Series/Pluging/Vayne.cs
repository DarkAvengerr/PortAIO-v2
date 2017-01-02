using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    

    internal class Vayne : Logic
    {
        public Vayne()
        {
            Q = new Spell(SpellSlot.Q, 300f);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R);

            E.SetTargetted(0.25f, 1600f);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("AQALogic", "Use AA-Q-AA Logic", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R|When Enemies Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                comboMenu.AddItem(
                    new MenuItem("ComboRHp", "Use R|Or Player HealthPercent <= x%", true).SetValue(new Slider(45)));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassQ2Passive", "Use Q|Only Target have 2 Passive", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E|Only Target have 2 Passive", true).SetValue(false));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearQTurret", "Use Q|Attack Tower", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= %", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qMenu = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qMenu.AddItem(new MenuItem("QCheck", "Use Q|Safe Check?", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("QTurret", "Use Q|Dont Cast To Turret", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("QMelee", "Use Q|Anti Melee", true).SetValue(true));
                }

                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    var condemnMenu = eMenu.AddSubMenu(new Menu("Condemn Settings", "Condemn Settings"));
                    {
                        condemnMenu.AddItem(
                            new MenuItem("EMode", "Use E Mode:", true).SetValue(
                                new StringList(new[] {"Default", "VHR", "Marksman", "SharpShooter", "OKTW"})));
                        condemnMenu.AddItem(
                            new MenuItem("ComboEPush", "Use E|Push Tolerance", true).SetValue(new Slider(0, -50, 50)));
                    }

                    var interruptMenu = eMenu.AddSubMenu(new Menu("Interrupt Settings", "Interrupt Settings"));
                    {
                        interruptMenu.AddItem(new MenuItem("Interrupt", "Interrupt Danger Spells", true).SetValue(false));
                        interruptMenu.AddItem(new MenuItem("AntiAlistar", "Interrupt Alistar W", true).SetValue(true));
                        interruptMenu.AddItem(new MenuItem("AntiRengar", "Interrupt Rengar Jump", true).SetValue(true));
                        interruptMenu.AddItem(new MenuItem("AntiKhazix", "Interrupt Khazix R", true).SetValue(true));
                    }

                    var antigapcloserMenu =
                        eMenu.AddSubMenu(new Menu("AntiGapcloser Settings", "AntiGapcloser Settings"));
                    {
                        antigapcloserMenu.AddItem(new MenuItem("Gapcloser", "Anti Gapcloser", true).SetValue(false));
                        foreach (var target in HeroManager.Enemies)
                        {
                            antigapcloserMenu.AddItem(
                                new MenuItem("AntiGapcloser" + target.ChampionName.ToLower(), target.ChampionName, true)
                                    .SetValue(false));
                        }
                    }

                    var autocondemnMenu = eMenu.AddSubMenu(new Menu("AutoCondemn Settings", "AutoCondemn Settings"));
                    {
                        autocondemnMenu.AddItem(new MenuItem("AutoE", "Auto E?", true).SetValue(false));
                        foreach (var target in HeroManager.Enemies)
                        {
                            autocondemnMenu.AddItem(new MenuItem("AutoE" + target.ChampionName.ToLower(),
                                target.ChampionName, true).SetValue(true));
                        }
                    }
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(true));
                    rMenu.AddItem(
                        new MenuItem("AutoRCount", "Auto R|When Enemies Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                    rMenu.AddItem(
                        new MenuItem("AutoRRange", "Auto R|Search Enemies Range", true).SetValue(new Slider(600, 500, 1200)));
                }

                miscMenu.AddItem(new MenuItem("Forcus", "Force 2 Passive Target", true).SetValue(true));
            }

            var utilityMenu = Menu.AddSubMenu(new Menu("Utility", "Utility"));
            {
                var skinMenu = utilityMenu.AddSubMenu(new Menu("Skin Change", "Skin Change"));
                {
                    SkinManager.AddToMenu(skinMenu, 10);
                }

                var autoLevelMenu = utilityMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }

                var itemsMenu = utilityMenu.AddSubMenu(new Menu("Items", "Items"));
                {
                    ItemsManager.AddToMenu(itemsMenu);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                //DamageIndicator.AddToMenu(drawMenu);
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
                if (Menu.GetBool("QMelee") && Q.IsReady())
                {
                    Q.Cast(Me.Position.Extend(sender.Position, -Q.Range));
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
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
                    FarmHarass();
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        private void AutoLogic()
        {
            if (Menu.GetBool("AutoE") && E.IsReady() && !Me.UnderTurret(true))
            {
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                {
                    foreach (
                        var target in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(E.Range) && !x.HasBuffOfType(BuffType.SpellShield) &&
                                Menu.GetBool("AutoE" + x.ChampionName.ToLower())))
                    {
                        if (target.Check(E.Range))
                        {
                            ELogic(target);
                        }
                    }
                }
            }

            if (Menu.GetBool("AutoR") && R.IsReady() && 
                Me.CountEnemiesInRange(Menu.GetSlider("AutoRRange")) >= Menu.GetSlider("AutoRCount"))
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

            if (Menu.GetBool("ComboR") && R.IsReady())
            {
                if (Me.CountEnemiesInRange(800) >= Menu.GetSlider("ComboRCount"))
                {
                    R.Cast();
                }

                if (Me.CountEnemiesInRange(600) >= 1 && Me.HealthPercent <= Menu.GetSlider("ComboRHp"))
                {
                    R.Cast();
                }
            }

            if (Menu.GetBool("ComboE") && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (target.Check(E.Range))
                {
                    ELogic(target);
                }
            }

            if (Menu.GetBool("ComboQ") && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);

                if (target.Check(800f))
                {
                    if (Menu.GetBool("AQALogic") && Orbwalking.InAutoAttackRange(target))
                    {
                        return;
                    }

                    QLogic(target);
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassE") && E.IsReady())
                {
                    var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                    if (target.Check(E.Range) && target.HasBuff("VayneSilveredDebuff") &&
                        target.GetBuffCount("VayneSilveredDebuff") == 2)
                    {
                        E.CastOnUnit(target);
                    }
                }

                if (Menu.GetBool("HarassQ") && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(800f, TargetSelector.DamageType.Magical);

                    if (target.Check(800f))
                    {
                        QLogic(target);
                    }
                }
            }
        }

        private void FarmHarass()
        {
            if (ManaManager.SpellHarass)
            {
                Harass();
            }
        }

        private void LaneClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("LaneClearQ") && Q.IsReady())
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
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("JungleClearE") && E.IsReady())
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

                if (Menu.GetBool("Forcus") && ForcusTarget.Check())
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
                if (Menu.GetBool("AQALogic"))
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
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
                    {
                        if (Menu.GetBool("LaneClearQ") && Menu.GetBool("LaneClearQTurret") &&
                            Me.CountEnemiesInRange(900) == 0 && Q.IsReady())
                        {
                            Q.Cast(Game.CursorPos);
                        }
                    }
                }
                else if (t is Obj_AI_Minion)
                {
                    if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
                    {
                        if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                        {
                            var mobs =
                                MinionManager.GetMinions(Me.Position, 800, MinionTypes.All, MinionTeam.Neutral,
                                    MinionOrderTypes.MaxHealth);

                            if (mobs.Any())
                            {
                                Q.Cast(Game.CursorPos, true);
                            }
                        }
                    }
                }
            }
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.GetBool("Interrupt") && E.IsReady() && sender.IsEnemy && sender.IsValidTarget(E.Range))
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
                if (Menu.GetBool("AntiAlistar") && Args.Sender.ChampionName == "Alistar" && Args.SkillType == GapcloserType.Targeted)
                {
                    E.CastOnUnit(Args.Sender, true);
                }

                if (Menu.GetBool("Gapcloser") && Menu.GetBool("AntiGapcloser" + Args.Sender.ChampionName.ToLower()))
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

            if (Rengar != null && Menu.GetBool("AntiRengar"))
            {
                if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(Me.Position) < E.Range)
                {
                    E.CastOnUnit(Rengar);
                }
            }

            if (Khazix != null && Menu.GetBool("AntiKhazix"))
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
                if (Menu.GetBool("DrawE") && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
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

            if (Menu.GetBool("QTurret") && qPosition.UnderTurret(true))
            {
                canQ = false;
            }

            if (Menu.GetBool("QCheck"))
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
                Q.Cast(Game.CursorPos, true);
                canQ = false;
            }
        }

        private void ELogic(Obj_AI_Base target)
        {
            if (target != null)
            {
                switch (Menu.GetList("EMode"))
                {
                    case 0:
                        {
                            var EPred = E.GetPrediction(target);
                            var PD = 425 + Menu.GetSlider("ComboEPush");
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
                            var pushDistance = 425 + Menu.GetSlider("ComboEPush");
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
