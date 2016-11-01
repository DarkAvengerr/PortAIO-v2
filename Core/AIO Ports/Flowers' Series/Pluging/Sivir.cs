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

    internal class Sivir : Program
    {
        private new readonly Menu Menu = Championmenu;

        public Sivir()
        {
            Q = new Spell(SpellSlot.Q, 1200f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R| When Enemies Counts >= x", true).SetValue(new Slider(3, 1, 5)));
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
                LaneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearWTurret", "Use W| Attack Turret", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
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
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var QMenu = MiscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    QMenu.AddItem(new MenuItem("AutoQ", "Auto Q?", true).SetValue(true));
                }

                var EMenu = MiscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    EMenu.AddItem(new MenuItem("AutoE", "Auto E?", true).SetValue(true));
                    EMenu.AddItem(
                        new MenuItem("AutoEHp", "Auto E| When Player HealthPercent <= x%", true).SetValue(new Slider(80)));
                }

                var RMenu = MiscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    RMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(false));
                }
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
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
            }
        }

        private void Auto()
        {
            if (Me.UnderTurret(true))
            {
                return;
            }

            if (Menu.Item("AutoQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.CanMove()))
                {
                    if (CheckTarget(target, Q.Range))
                    {
                        Q.CastTo(target);
                    }
                }
            }

            if (Menu.Item("AutoR", true).GetValue<bool>() && R.IsReady() && Me.CountEnemiesInRange(850) >= 3)
            {
                R.Cast();
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x) && CheckTargetSureCanKill(x)))
                {
                    if (CheckTarget(target, Q.Range))
                    {
                        Q.CastTo(target, true);
                    }
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, 1500f))
            {
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    !Me.IsDashing())
                {
                    Q.CastTo(target);
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() &&
                    Me.CountEnemiesInRange(850) >= Menu.Item("ComboRCount", true).GetValue<Slider>().Value &&
                    ((target.Health <= Me.GetAutoAttackDamage(target)*3 && !Q.IsReady()) ||
                     (target.Health <= Me.GetAutoAttackDamage(target)*3 + Q.GetDamage(target))))
                {
                    R.Cast();
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
                    Q.CastTo(target, true);
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
                        var QFarm = Q.GetLineFarmLocation(Minions);

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
                        var mob = mobs.FirstOrDefault();

                        Q.Cast(mob, true);
                    }
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var hero = Args.Target as AIHeroClient;

                if (hero != null && Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady())
                {
                    var WTarget = hero;

                    if (WTarget.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) && Me.CanAttack)
                    {
                        W.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
            }
            else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (Args.Target is Obj_AI_Minion)
                {
                    LaneClearW();
                    JungleClearW();
                }
                else if (Args.Target is Obj_AI_Turret || Args.Target.Type == GameObjectType.obj_AI_Turret)
                {
                    if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
                    {
                        if (Menu.Item("LaneClearWTurret", true).GetValue<bool>() && W.IsReady() && 
                            Me.CountEnemiesInRange(1000) == 0)
                        {
                            W.Cast();
                            Orbwalking.ResetAutoAttackTimer();
                        }
                    }
                }
            }
        }

        private void LaneClearW()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("LaneClearW", true).GetValue<bool>() && W.IsReady())
                {
                    var minions = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me));

                    if (minions.Count >= 3)
                    {
                        W.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
            }
        }

        private void JungleClearW()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady())
                {
                    var Mobs = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me),
                        MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                    if (Mobs.Any())
                    {
                        W.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.Item("AutoE", true).GetValue<bool>() && E.IsReady() &&
                Me.HealthPercent <= Menu.Item("AutoEHp", true).GetValue<Slider>().Value)
            {
                if (sender != null && sender.IsEnemy && sender is AIHeroClient)
                {
                    var e = (AIHeroClient)sender;

                    if (Args.Target != null)
                    {
                        if (Args.Target.IsMe)
                        {
                            if (CanE(e, Args))
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(120, () => E.Cast());
                            }
                        }
                    }
                }
            }
        }

        private bool CanE(AIHeroClient e, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                switch (e.ChampionName)
                {
                    case "TwistedFate":
                        if (Args.SData.Name == "GoldCardLock" || Args.SData.Name == "RedCardLock" || Args.SData.Name == "BlueCardLock")
                        {
                            return true;
                        }
                        break;
                    case "Leona":
                        if (Args.SData.Name == "LeonaQ")
                        {
                            return true;
                        }
                        break;
                    default:
                        return false;
                }
            }
            else
            {
                return !Args.SData.Name.ToLower().Contains("summoner");
            }

            return false;
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen )
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
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
