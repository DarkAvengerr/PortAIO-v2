using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Pluging
{
    using Common;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;
    using static Common.Common;

    internal class Caitlyn : Logic
    {
        private int LastQTime;
        private int LastWTime;
        private readonly Menu Menu = Championmenu;

        public Caitlyn()
        {
            Q = new Spell(SpellSlot.Q, 1250f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R, 2000f);

            Q.SetSkillshot(0.50f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboQCount", "Use Q| Min Hit Count >= x", true).SetValue(new Slider(2, 1, 5)));
                ComboMenu.AddItem(
                    new MenuItem("ComboQRange", "Use Q| Min Range >= x", true).SetValue(new Slider(750, 500, 1000)));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboWCount", "Use W| Min Count >= x", true).SetValue(new Slider(1, 1, 3)));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRSafe", "Use R| Safe Check?", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboRRange", "Use R| Min Range >= x", true).SetValue(new Slider(900, 500, 1500)));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearQCount", "If Q CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= %", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                FleeMenu.AddItem(new MenuItem("FleeE", "Use E", true).SetValue(true));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qSettings = MiscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qSettings.AddItem(new MenuItem("AutoQ", "Auto Q?", true).SetValue(true));
                }

                var wSettings = MiscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wSettings.AddItem(new MenuItem("AutoWCC", "Auto W|CC", true).SetValue(true));
                    wSettings.AddItem(new MenuItem("AutoWTP", "Auto W|Teleport", true).SetValue(true));
                }

                var eSettings = MiscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    var interruptMenu = eSettings.AddSubMenu(new Menu("Interrupt Settings", "Interrupt Settings"));
                    {
                        interruptMenu.AddItem(new MenuItem("Interrupt", "Interrupt Danger Spells", true).SetValue(true));
                        interruptMenu.AddItem(new MenuItem("AntiAlistar", "Interrupt Alistar W", true).SetValue(true));
                        interruptMenu.AddItem(new MenuItem("AntiRengar", "Interrupt Rengar Jump", true).SetValue(true));
                        interruptMenu.AddItem(new MenuItem("AntiKhazix", "Interrupt Khazix R", true).SetValue(true));
                    }

                    var antigapcloserMenu =
                        eSettings.AddSubMenu(new Menu("AntiGapcloser Settings", "AntiGapcloser Settings"));
                    {
                        antigapcloserMenu.AddItem(new MenuItem("Gapcloser", "Anti Gapcloser", true).SetValue(false));
                        foreach (var target in HeroManager.Enemies)
                        {
                            antigapcloserMenu.AddItem(
                                new MenuItem("AntiGapcloser" + target.ChampionName.ToLower(), target.ChampionName, true)
                                    .SetValue(false));
                        }
                    }
                }

                var rSettings = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rSettings.AddItem(
                        new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
                }

                MiscMenu.AddItem(
                    new MenuItem("EQKey", "One Key EQ target", true).SetValue(new KeyBind('G', KeyBindType.Press)));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawRMin", "Draw R Range(MinMap)", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }


            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
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

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Args.Slot == SpellSlot.Q)
            {
                LastQTime = Utils.TickCount;
            }

            if (Args.Slot == SpellSlot.W)
            {
                LastWTime = Utils.TickCount;
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            R.Range = 500 * R.Level + 1500;

            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            Auto();
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
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("EQKey", true).GetValue<KeyBind>().Active)
                    {
                        OneKeyEQ();
                    }
                    if (Menu.Item("SemiR", true).GetValue<KeyBind>().Active && R.IsReady())
                    {
                        OneKeyCastR();
                    }
                    break;
            }
        }

        private void OneKeyCastR()
        {
            var select = TargetSelector.GetSelectedTarget();
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (select != null && target.IsValidTarget(R.Range))
            {
                R.CastOnUnit(select);
            }
            else if (select == null && target != null && target.IsValidTarget(R.Range))
            {
                R.CastOnUnit(target);
            }
        }

        private void Auto()
        {
            if (Menu.Item("AutoQ", true).GetValue<bool>() && Q.IsReady() && 
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && 
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
            {
                var target = TargetSelector.GetTarget(Q.Range - 30, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, Q.Range) && !target.CanMove())
                {
                    Q.CastTo(target);
                }
            }

            if (W.IsReady())
            {
                if (Menu.Item("AutoWCC", true).GetValue<bool>())
                {
                    foreach (
                        var target in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(W.Range) && !x.CanMove() && !x.HasBuff("caitlynyordletrapinternal")))
                    {
                        if (Utils.TickCount - LastWTime > 1500)
                        {
                            W.Cast(target.Position, true);
                        }
                    }
                }

                if (Menu.Item("AutoWTP", true).GetValue<bool>())
                {
                    var obj =
                        ObjectManager
                            .Get<Obj_AI_Base>()
                            .FirstOrDefault(x => !x.IsAlly && !x.IsMe && x.DistanceToPlayer() <= W.Range &&
                                                 x.Buffs.Any(
                                                     a =>
                                                         a.Name.ToLower().Contains("teleport") || // tp
                                                         a.Name.ToLower().Contains("gate")) && // tf r
                                                 !ObjectManager.Get<Obj_AI_Base>()
                                                     .Any(b => b.Name.ToLower().Contains("trap") && b.Distance(x) <= 150));

                    if (obj != null)
                    {
                        if (Utils.TickCount - LastWTime > 1500)
                        {
                            W.Cast(obj.Position, true);
                        }
                    }
                }
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(Q.Range) && CheckTargetSureCanKill(x) && x.Health < Q.GetDamage(x)))
                {
                    if (Orbwalker.InAutoAttackRange(target) && target.Health <= Me.GetAutoAttackDamage(target, true))
                    {
                        continue;
                    }

                    Q.CastTo(target);
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, R.Range))
            {
                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(700))
                {
                    var ePred = E.GetPrediction(target);

                    if (ePred.CollisionObjects.Count == 0 || ePred.Hitchance >= HitChance.VeryHigh)
                    {
                        if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            if (E.Cast(target).IsCasted())
                            {
                                Q.Cast(target);
                            }
                        }
                        else
                        {
                            E.Cast(target);
                        }
                    }
                    else
                    {
                        if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                        {
                            if (target.DistanceToPlayer() >= Menu.Item("ComboQRange", true).GetValue<Slider>().Value)
                            {
                                Q.CastTo(target);

                                if (Me.CountEnemiesInRange(Q.Range) >=
                                    Menu.Item("ComboQCount", true).GetValue<Slider>().Value)
                                {
                                    Q.CastIfWillHit(target, Menu.Item("ComboQCount", true).GetValue<Slider>().Value, true);
                                }
                            }
                        }
                    }
                }

                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && !E.IsReady() &&
                    target.IsValidTarget(Q.Range) &&
                    target.DistanceToPlayer() >= Menu.Item("ComboQRange", true).GetValue<Slider>().Value)
                {
                    if (target.DistanceToPlayer() >= Menu.Item("ComboQRange", true).GetValue<Slider>().Value)
                    {
                        Q.CastTo(target);

                        if (Me.CountEnemiesInRange(Q.Range) >=
                            Menu.Item("ComboQCount", true).GetValue<Slider>().Value)
                        {
                            Q.CastIfWillHit(target, Menu.Item("ComboQCount", true).GetValue<Slider>().Value, true);
                        }
                    }
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range) &&
                    W.Instance.Ammo >= Menu.Item("ComboWCount", true).GetValue<Slider>().Value)
                {
                    if (Utils.TickCount - LastWTime > 1500)
                    {
                        if (target.IsFacing(Me))
                        {
                            if (target.IsMelee && target.DistanceToPlayer() < target.AttackRange + 100)
                            {
                                W.Cast(Me.Position);
                            }
                            else
                            {
                                var wPred = W.GetPrediction(target);

                                if (wPred.Hitchance >= HitChance.VeryHigh && target.IsValidTarget(W.Range))
                                {
                                    W.Cast(wPred.CastPosition);
                                }
                            }
                        }
                        else
                        {
                            var wPred = W.GetPrediction(target);

                            if (wPred.Hitchance >= HitChance.VeryHigh && target.IsValidTarget(W.Range))
                            {
                                W.Cast(wPred.CastPosition +
                                       Vector3.Normalize(target.ServerPosition - Me.ServerPosition)*100);
                            }
                        }
                    }
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() && Utils.TickCount - LastQTime > 2500)
                {
                    if (Menu.Item("ComboRSafe", true).GetValue<bool>() &&
                        (Me.UnderTurret(true) || Me.CountEnemiesInRange(1000) > 2))
                    {
                        return;
                    }

                    if (!target.IsValidTarget(R.Range))
                    {
                        return;
                    }

                    if (target.DistanceToPlayer() < Menu.Item("ComboRRange", true).GetValue<Slider>().Value)
                    {
                        return;
                    }

                    if (target.Health + target.HPRegenRate * 3 > R.GetDamage(target))
                    {
                        return;
                    }

                    var RCollision =
                        LeagueSharp.Common.Collision
                            .GetCollision(new List<Vector3> {target.ServerPosition},
                                new PredictionInput
                                {
                                    Delay = R.Delay,
                                    Radius = R.Width,
                                    Speed = R.Speed,
                                    Unit = Me,
                                    UseBoundingRadius = true,
                                    Collision = true,
                                    CollisionObjects = new[] {CollisionableObjects.Heroes, CollisionableObjects.YasuoWall}
                                })
                            .Any(x => x.NetworkId != target.NetworkId);

                    if (RCollision)
                    {
                        return;
                    }

                    R.CastOnUnit(target, true);
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
                if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                    if (CheckTarget(target, Q.Range))
                    {
                        Q.CastTo(target);
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
                        var QFarm =
                            MinionManager.GetBestLineFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
                                Q.Width, Q.Range);

                        if (QFarm.MinionsHit >= Menu.Item("LaneClearQCount", true).GetValue<Slider>().Value)
                        {
                            Q.Cast(QFarm.Position);
                        }
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

        private void Flee()
        {
            if (Menu.Item("FleeE", true).GetValue<bool>() && E.IsReady())
            {
                E.Cast(Me.Position - (Game.CursorPos - Me.Position));
            }
        }

        private void OneKeyEQ()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (E.IsReady() && Q.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                    TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, E.Range))
                {
                    if (E.GetPrediction(target).CollisionObjects.Count == 0 && E.CanCast(target))
                    {
                        E.Cast(target);
                        Q.CastTo(target);
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

        private void OnEndScene(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
#pragma warning disable 618
                if (Menu.Item("DrawRMin", true).GetValue<bool>() && R.IsReady())
                    LeagueSharp.Common.Utility.DrawCircle(Me.Position, R.Range, Color.FromArgb(14, 194, 255), 1, 30, true);
#pragma warning restore 618
            }
        }
    }
}
