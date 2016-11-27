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

    internal class Graves : Logic
    {
        private bool canE;
        private readonly float SearchERange;
        private readonly Menu Menu = Championmenu;

        public Graves()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1050f);

            Q.SetSkillshot(0.25f, 40f, 3000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);

            SearchERange = E.Range + Orbwalking.GetRealAutoAttackRange(Me) - 100;

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboEAA", "Use E| Reset Attack", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboECheck", "Use E| Safe Check", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(false));
                ComboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R| Min Hit Count >= x", true).SetValue(new Slider(4, 1, 5)));
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
                    new MenuItem("LaneClearQCount", "Use Q| Min Hit Count >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(20)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "KillSteal Q", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealW", "KillSteal W", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealR", "KillSteal R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    KillStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(),
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var BurstMenu = Menu.AddSubMenu(new Menu("Burst", "Burst"));
            {
                BurstMenu.AddItem(new MenuItem("BurstKeys", "Burst Key -> Please Check The Orbwalker Key!", true));
                BurstMenu.AddItem(new MenuItem("Bursttarget", "Burst Target -> Left Click to Lock!", true));
            }
            
            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(new MenuItem("GapW", "Use W| Anti GapCloser", true).SetValue(true));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawBurst", "Draw Burst Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Drawing.OnDraw += OnDraw;
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (Args.SData.Name)
            {
                case "GravesMove":
                    Orbwalking.ResetAutoAttackTimer();
                    canE = false;
                    break;
                case "GravesChargeShot":
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst &&
                        TargetSelector.GetSelectedTarget() != null && E.IsReady())
                    {
                        var target = TargetSelector.GetSelectedTarget();

                        E.Cast(target.Position, true);
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
                case Orbwalking.OrbwalkingMode.Burst:
                    Burst();
                    break;
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
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(Q.Range) && CheckTargetSureCanKill(x) && x.Health < Q.GetDamage(x)))
                {
                    Q.CastTo(target);
                }
            }

            if (Menu.Item("KillStealW", true).GetValue<bool>() && W.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(W.Range) && CheckTargetSureCanKill(x) && x.Health < W.GetDamage(x)))
                {
                    W.CastTo(target);
                }
            }

            if (Menu.Item("KillStealR", true).GetValue<bool>() && R.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(R.Range) && CheckTargetSureCanKill(x) && 
                        Menu.Item("KillStealR" + x.ChampionName.ToLower(), true).GetValue<bool>()
                        && x.Health < R.GetDamage(x) &&
                        x.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) + E.Range - 100))
                {
                    R.CastTo(target);
                }
            }
        }

        private void Burst()
        {
            var target = TargetSelector.GetSelectedTarget();

            if (CheckTarget(target, 700f))
            {
                if (R.IsReady() && E.IsReady())
                {
                    var rPred = R.GetPrediction(target, true);

                    if (rPred.Hitchance >= HitChance.High && target.IsValidTarget(R.Range))
                    {
                        if (R.Cast(rPred.CastPosition, true))
                        {
                            E.Cast(target.Position, true);
                            Orbwalking.ResetAutoAttackTimer();
                        }
                    }
                }
                else
                {
                    if (Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.CastTo(target);
                    }

                    if (W.IsReady() && target.IsValidTarget(W.Range) &&
                             (target.DistanceToPlayer() <= target.AttackRange + 70 ||
                              (target.DistanceToPlayer() >= Orbwalking.GetRealAutoAttackRange(Me) + 80)))
                    {
                        W.CastTo(target);
                    }

                    if (E.IsReady() && !R.IsReady())
                    {
                        ELogic(target);
                    }
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, R.Range))
            {
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.CastTo(target);
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(SearchERange))
                {
                    ELogic(target);
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range) &&
                    (target.DistanceToPlayer() <= target.AttackRange + 70 ||
                     (target.DistanceToPlayer() >= Orbwalking.GetRealAutoAttackRange(Me) + 80)))
                {
                    W.CastTo(target);
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() && !Q.IsReady() 
                    && target.IsValidTarget(R.Range))
                {
                    R.CastIfWillHit(target, Menu.Item("ComboRCount", true).GetValue<Slider>().Value);
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
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() && CheckTarget(target, Q.Range))
                {
                    Q.CastTo(target);
                }
            }
        }

        private void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady())
                {
                    var Minions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (Minions.Any())
                    {
                        var QFarm =
                            MinionManager.GetBestLineFarmLocation(Minions.Select(x => x.Position.To2D()).ToList(),
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
                    var mobs =
                        MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth).Where(x => !x.Name.ToLower().Contains("mini"));

                    if (mobs.Any())
                    {
                        var QFarm =
                            MinionManager.GetBestLineFarmLocation(mobs.Select(x => x.Position.To2D()).ToList(),
                                Q.Width, Q.Range);

                        if (QFarm.MinionsHit >= 1)
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var target = Args.Target as AIHeroClient;

                    if (target != null && !target.IsDead && !target.IsZombie)
                    {
                        if (Menu.Item("ComboEAA", true).GetValue<bool>() && E.IsReady())
                        {
                            ELogic(target);
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var target = Args.Target as Obj_AI_Minion;

                    if (target != null && !target.IsDead)
                    {
                        if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                        {
                            if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                            {
                                var mobs =
                                    MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All, MinionTeam.Neutral,
                                    MinionOrderTypes.MaxHealth).Where(x => !x.Name.ToLower().Contains("mini"));

                                if (mobs.FirstOrDefault() != null)
                                {
                                    if (!Me.Spellbook.IsCastingSpell)
                                    {
                                        ELogic(mobs.FirstOrDefault());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (Menu.Item("GapW", true).GetValue<bool>() && W.IsReady() && 
                Args.Sender.IsValidTarget(W.Range) && Args.End.DistanceToPlayer() <= 200)
            {
                W.Cast(Args.End);
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

                if (Menu.Item("DrawBurst", true).GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Me.Position, 700f, Color.FromArgb(90, 255, 255), 1);
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

        private void ELogic(Obj_AI_Base target)
        {
            if (!E.IsReady())
            {
                return;
            }

            var ePosition = Me.ServerPosition.Extend(Game.CursorPos, E.Range);
            var targetDisE = target.ServerPosition.Distance(ePosition);

            if (ePosition.UnderTurret(true) && Me.HealthPercent <= 60)
            {
                canE = false;
            }

            if (Menu.Item("ComboECheck", true).GetValue<bool>())
            {
                if (ePosition.CountEnemiesInRange(350f) >= 3)
                {
                    canE = false;
                }

                //Catilyn W
                if (ObjectManager
                        .Get<Obj_GeneralParticleEmitter>()
                        .FirstOrDefault(
                            x =>
                                x != null && x.IsValid &&
                                x.Name.ToLower().Contains("yordletrap_idle_red.troy") &&
                                x.Position.Distance(ePosition) <= 100) != null)
                {
                    canE = false;
                }

                //Jinx E
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "k" &&
                                             x.Position.Distance(ePosition) <= 100) != null)
                {
                    canE = false;
                }

                //Teemo R
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "Noxious Trap" &&
                                             x.Position.Distance(ePosition) <= 100) != null)
                {
                    canE = false;
                }

                if (ePosition.CountEnemiesInRange(350) >= 3)
                {
                    canE = false;
                }
            }

            if (target.Distance(ePosition) > Orbwalking.GetRealAutoAttackRange(Me))
            {
                canE = false;
            }

            if (target.Health < Me.GetAutoAttackDamage(target, true)*2 && 
                target.Distance(ePosition) <= Orbwalking.GetRealAutoAttackRange(Me) && Me.CanAttack)
            {
                canE = true;
            }

            if (!Me.HasBuff("GravesBasicAttackAmmo2") && Me.HasBuff("GravesBasicAttackAmmo1") &&
                target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) &&
                target.Distance(ePosition) <= Orbwalking.GetRealAutoAttackRange(Me))
            {
                canE = true;
            }

            if (!Me.HasBuff("GravesBasicAttackAmmo2") && !Me.HasBuff("GravesBasicAttackAmmo1") && 
                target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)))
            {
                canE = true;
            }

            if (canE)
            {
                if (E.Cast(ePosition, true))
                {
                    Orbwalking.ResetAutoAttackTimer();
                    canE = false;
                }
            }
        }
    }
}
