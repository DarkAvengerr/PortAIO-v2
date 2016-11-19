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

    internal class Ashe : Program
    {
        private new readonly Menu Menu = Championmenu;

        public Ashe()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1255f);
            E = new Spell(SpellSlot.E, 5000f);
            R = new Spell(SpellSlot.R, 2000f);

            W.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotCone);
            E.SetSkillshot(0.25f, 300f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 130f, 1600f, true, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboSaveMana", "Save Mana To Cast Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassWMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearWCount", "If W CanHit Counts >= ", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearWMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                JungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealW", "KillSteal W", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealR", "KillSteal R", true).SetValue(true));
                foreach (var target in HeroManager.Enemies)
                {
                    KillStealMenu.AddItem(new MenuItem("KillStealR" + target.ChampionName.ToLower(), 
                        "Kill: " + target.ChampionName, true).SetValue(true));
                }
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var RMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    RMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(true));
                    RMenu.AddItem(new MenuItem("Interrupt", "Interrupt Danger Spells", true).SetValue(true));
                    RMenu.AddItem(
                        new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
                    RMenu.AddItem(new MenuItem("AntiGapCloser", "Anti GapCloser", true).SetValue(true));
                    RMenu.AddItem(
                        new MenuItem("AntiGapCloserHp", "AntiGapCloser |When Player HealthPercent <= x%", true).SetValue(
                            new Slider(30)));
                    RMenu.AddItem(new MenuItem("AntiGapCloserRList", "AntiGapCloser R List:"));
                    foreach (var target in HeroManager.Enemies)
                    {
                        RMenu.AddItem(new MenuItem("AntiGapCloserR" + target.ChampionName.ToLower(),
                            "GapCloser: " + target.ChampionName, true).SetValue(true));
                    }
                }
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawRMin", "Draw R Range(MinMap)", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private void OnEnemyGapcloser(ActiveGapcloser Args)
        {
            if (!Args.Sender.IsEnemy || !R.IsReady() || Menu.Item("AntiGapCloser", true).GetValue<bool>() ||
                Me.HealthPercent > Menu.Item("AntiGapCloserHp", true).GetValue<Slider>().Value)
            {
                return;
            }

            if (Menu.Item("AntiGapCloserR" + Args.Sender.ChampionName.ToLower(), true).GetValue<bool>() &&
                Args.End.DistanceToPlayer() <= 300 &&
                Args.Sender.IsValidTarget(R.Range))
            {
                R.CastTo(Args.Sender);
            }
        }

        private void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.Item("Interrupt", true).GetValue<bool>() && R.IsReady())
            {
                if (!sender.IsEnemy || Args.DangerLevel < Interrupter2.DangerLevel.High ||
                    !sender.IsValidTarget(R.Range))
                {
                    return;
                }

                R.CastTo(sender);
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (Menu.Item("ComboQ", true).GetValue<bool>())
                    {
                        var target = (AIHeroClient)Args.Target;

                        if (target != null && !target.IsDead && !target.IsZombie)
                        {
                            if (Me.HasBuff("asheqcastready"))
                            {
                                Q.Cast();
                                Orbwalking.ResetAutoAttackTimer();
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() &&
                        Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value &&
                        Args.Target is Obj_AI_Minion)
                    {
                        var Mobs = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me),
                            MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                        foreach (var mob in Mobs)
                        {
                            if (mob == null)
                            {
                                continue;
                            }

                            if (!mob.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) ||
                                !(mob.Health > Me.GetAutoAttackDamage(mob)*2))
                            {
                                continue;
                            }

                            if (Me.HasBuff("asheqcastready"))
                            {
                                Q.Cast();
                                Orbwalking.ResetAutoAttackTimer();
                            }
                        }
                    }
                    break;
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu.Item("SemiR", true).GetValue<KeyBind>().Active)
            {
                OneKeyR();
            }

            AutoRLogic();
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

        private void AutoRLogic()
        {
            if (Menu.Item("AutoR", true).GetValue<bool>() && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && CheckTargetSureCanKill(x)))
                {
                    if (!(target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me)) ||
                        !(target.DistanceToPlayer() <= 700) ||
                        !(target.Health > Me.GetAutoAttackDamage(target)) ||
                        !(target.Health < R.GetDamage(target) + Me.GetAutoAttackDamage(target)*3) ||
                        target.HasBuffOfType(BuffType.SpellShield))
                    {
                        continue;
                    }

                    R.CastTo(target);
                    return;
                }
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealW", true).GetValue<bool>() && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && CheckTargetSureCanKill(x)))
                {
                    if (!target.IsValidTarget(W.Range) || !(target.Health < W.GetDamage(target)))
                        continue;

                    if (target.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me) && 
                        Me.HasBuff("AsheQAttack"))
                    {
                        continue;
                    }

                    W.CastTo(target);
                    return;
                }
            }

            if (!Menu.Item("KillStealR", true).GetValue<bool>() || !R.IsReady())
            {
                return;
            }

            foreach (
                var target in
                HeroManager.Enemies.Where(
                    x =>
                        x.IsValidTarget(2000) &&
                        Menu.Item("KillStealR" + x.ChampionName.ToLower(), true).GetValue<bool>()))
            {
                if (!(target.DistanceToPlayer() > 800) || !(target.Health < R.GetDamage(target)) ||
                    target.HasBuffOfType(BuffType.SpellShield))
                {
                    continue;
                }

                R.CastTo(target);
                return;
            }
        }

        private void Combo()
        {
            if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(1200) && CheckTargetSureCanKill(x)))
                {
                    if (target.IsValidTarget(600) && Me.CountEnemiesInRange(600) >= 3 && target.CountAlliesInRange(200) <= 2)
                    {
                        R.CastTo(target);
                    }

                    if (Me.CountEnemiesInRange(800) == 1 &&
                        target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                        target.DistanceToPlayer() <= 700 &&
                        target.Health > Me.GetAutoAttackDamage(target) &&
                        target.Health < R.GetDamage(target) + Me.GetAutoAttackDamage(target)*3 &&
                        !target.HasBuffOfType(BuffType.SpellShield))
                    {
                        R.CastTo(target);
                    }

                    if (target.DistanceToPlayer() <= 1000 &&
                        (!target.CanMove || target.HasBuffOfType(BuffType.Stun) ||
                        R.GetPrediction(target).Hitchance == HitChance.Immobile))
                    {
                        R.CastTo(target);
                    }
                }
            }

            if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && !Me.HasBuff("AsheQAttack"))
            {
                if ((Menu.Item("ComboSaveMana", true).GetValue<bool>() &&
                     Me.Mana > (R.IsReady() ? R.Instance.SData.Mana : 0) + W.Instance.SData.Mana + Q.Instance.SData.Mana) ||
                    !Menu.Item("ComboSaveMana", true).GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                    if (CheckTarget(target, W.Range))
                    {
                        W.CastTo(target);
                    }
                }
            }

            if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
            {
                var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, 1000))
                {
                    var EPred = E.GetPrediction(target);

                    if ((NavMesh.GetCollisionFlags(EPred.CastPosition) == CollisionFlags.Grass ||
                         NavMesh.IsWallOfGrass(target.ServerPosition, 20)) && !target.IsVisible)
                    {
                        E.Cast(EPred.CastPosition);
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

            if (Me.ManaPercent >= Menu.Item("HarassWMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady() && !Me.HasBuff("AsheQAttack"))
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                    if (CheckTarget(target, W.Range))
                    {
                        W.CastTo(target);
                    }
                }
            }
        }

        private void LaneClear()
        {
            var Minions = MinionManager.GetMinions(Me.Position, W.Range);

            if (Minions.Count <= 0)
            {
                return;
            }

            if (!Menu.Item("LaneClearW", true).GetValue<bool>() || !W.IsReady() ||
                !(Menu.Item("LaneClearWMana", true).GetValue<Slider>().Value <= Me.ManaPercent))
            {
                return;
            }

            var WFarm = MinionManager.GetBestCircularFarmLocation(Minions.Select(x => x.Position.To2D()).ToList(),
                W.Width, W.Range);

            if (WFarm.MinionsHit >= Menu.Item("LaneClearWCount", true).GetValue<Slider>().Value)
            {
                W.Cast(WFarm.Position);
            }
        }

        private void JungleClear()
        {
            var Mobs = MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Mobs.Count <= 0)
            {
                return;
            }

            if (Menu.Item("JungleClearW", true).GetValue<bool>() &&
                Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value &&
                !Me.HasBuff("AsheQAttack"))
            {
                var mob = Mobs.FirstOrDefault();

                if (mob != null)
                {
                    W.Cast(mob.Position);
                }
            }
        }

        private void OneKeyR()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (!R.IsReady())
                return;

            var select = TargetSelector.GetSelectedTarget();
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (select != null && !target.HasBuffOfType(BuffType.SpellShield) && target.IsValidTarget(R.Range))
            {
                R.CastTo(select);
            }
            else if (select == null && target != null && !target.HasBuffOfType(BuffType.SpellShield) && 
                target.IsValidTarget(R.Range))
            {
                R.CastTo(target);
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.Item("DrawW", true).GetValue<bool>() && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 1);
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
