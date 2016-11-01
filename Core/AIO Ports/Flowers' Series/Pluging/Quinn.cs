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

    internal class Quinn : Program
    {
        private new readonly Menu Menu = Championmenu;

        public Quinn()
        {
            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, 2000f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 550f);

            Q.SetSkillshot(0.25f, 90f, 1550f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 2000f, 1400f, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, 2000f);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
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
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "KillSteal Q", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var EMenu = MiscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    EMenu.AddItem(new MenuItem("Interrupt", "Interrupt Danger Spells", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("Gapcloser", "Anti Gapcloser", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("AntiAlistar", "Anti Alistar", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("AntiRengar", "Anti Rengar", true).SetValue(true));
                    EMenu.AddItem(new MenuItem("AntiKhazix", "Anti Khazix", true).SetValue(true));
                }

                var RMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    RMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(true));
                }

                MiscMenu.AddItem(new MenuItem("Forcus", "Forcus Attack Passive Target", true).SetValue(true));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            GameObject.OnCreate += OnCreate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
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

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Menu.Item("Forcus", true).GetValue<bool>())
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                    Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && HavePassive(x)))
                    {
                        Orbwalker.ForceTarget(enemy);
                    }
                }
                else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var all = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me),
                        MinionTypes.All, MinionTeam.NotAlly).Where(HavePassive);

                    if (all.Any())
                    {
                        Orbwalker.ForceTarget(all.FirstOrDefault());
                    }
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var target = Args.Target as AIHeroClient;

                    if (target != null && CheckTargetSureCanKill(target))
                    {
                        if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
                        {
                            E.CastOnUnit(target, true);
                        }
                        else if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && !Me.HasBuff("QuinnR"))
                        {
                            Q.CastTo(target);
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (Me.ManaPercent < Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                        return;

                    var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var mob = mobs.FirstOrDefault();

                        if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady() && mob.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(mob, true);
                        }
                        else if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() && mob.IsValidTarget(Q.Range))
                        {
                            Q.Cast(mob, true);
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
                if (sender.IsCastingInterruptableSpell())
                {
                    E.CastOnUnit(sender, true);
                }

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

                if (Menu.Item("Gapcloser", true).GetValue<bool>())
                {
                    if (Args.End.DistanceToPlayer() <= 250 && Args.Sender.IsValid)
                    {
                        E.CastOnUnit(Args.Sender, true);
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
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    AutoR();
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
                        continue;

                    Q.CastTo(target);
                    return;
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, Q.Range))
            {
                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && Me.HasBuff("QuinnR"))
                {
                    E.CastOnUnit(target);
                }

                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && !Me.HasBuff("QuinnR"))
                {
                    if (target.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me) && HavePassive(target))
                    {
                        return;
                    }

                    Q.CastTo(target);
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady())
                {
                    var WPred = W.GetPrediction(target);

                    if ((NavMesh.GetCollisionFlags(WPred.CastPosition) == CollisionFlags.Grass ||
                         NavMesh.IsWallOfGrass(target.ServerPosition, 20)) && !target.IsVisible)
                    {
                        W.Cast();
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
                if (Menu.Item("HarassQ", true).GetValue<bool>())
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
                        var QFarm = Q.GetCircularFarmLocation(minions, Q.Width);

                        if (QFarm.MinionsHit >= Menu.Item("LaneClearQCount", true).GetValue<Slider>().Value)
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }
                }
            }
        }

        private void AutoR()
        {
            if (Menu.Item("AutoR", true).GetValue<bool>() && R.IsReady() && R.Instance.Name == "QuinnR")
            {
                if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  &&
                    Me.InFountain())
                {
                    R.Cast();
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen )
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
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

        private bool HavePassive(Obj_AI_Base target)
        {
            return target.HasBuff("quinnw");
        }
    }
}
