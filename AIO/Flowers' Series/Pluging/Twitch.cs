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

    internal class Twitch
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        private static bool PlayerIsKillTarget;

        private static readonly Menu Menu = Program.Championmenu;
        private static readonly AIHeroClient Me = Program.Me;
        private static readonly Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        public Twitch()
        {
            Q = new Spell(SpellSlot.Q, 0f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 1200f);
            R = new Spell(SpellSlot.R, 975f);

            W.SetSkillshot(0.25f, 100f, 1410f, false, SkillshotType.SkillshotCircle);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(false));
                ComboMenu.AddItem(new MenuItem("ComboQRange", "Use Q | Search Enemy Range", true).SetValue(new Slider(900, 0, 1800)));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboEStack", "Use E | Min E Stack Count(Leave E Range Auto E)", true).SetValue(new Slider(3, 1, 6)));
                ComboMenu.AddItem(new MenuItem("ComboEFull", "Use E | If enemy full stack", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRCount", "Use R | If enemies in counts >= x", true).SetValue(new Slider(3, 1, 5)));
                ComboMenu.AddItem(new MenuItem("ComboRRange", "Use R | Search Enemy Range", true).SetValue(new Slider(800, 0, 1500)));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassEStack", "Min E Stack Count", true).SetValue(new Slider(2, 1, 6)));
                HarassMenu.AddItem(new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var JungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleStealE", "Use E", true).SetValue(true));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(new MenuItem("AutoQ", "Auto Q?", true).SetValue(true));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Game.OnUpdate += OnUpdate;
            Game.OnNotify += OnNotify;
            Drawing.OnDraw += OnDraw;
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu.Item("AutoQ", true).GetValue<bool>())
            {
                if (PlayerIsKillTarget && Q.IsReady() && Me.CountEnemiesInRange(1000) >= 1)
                {
                    Q.Cast();
                }
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
                    JungleClear();
                    break;
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealE", true).GetValue<bool>() && E.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(E.Range) && CheckTargetSureCanKill(x) && x.Health < E.GetDamage(x) - 5)
                )
                {
                    if (CheckTarget(target, E.Range))
                    {
                        E.Cast();
                    }
                }
            }

            if (Menu.Item("KillStealW", true).GetValue<bool>() && W.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(W.Range) && CheckTargetSureCanKill(x) && x.Health < W.GetDamage(x))
                )
                {
                    if (CheckTarget(target, W.Range))
                    {
                        W.CastTo(target);
                    }
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, R.Range))
            {
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                {
                    if (target.DistanceToPlayer() <= Menu.Item("ComboQRange", true).GetValue<Slider>().Value)
                    {
                        Q.Cast();
                    }
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.CastTo(target);
                }

                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if ((target.DistanceToPlayer() >= E.Range * 0.7 && target.IsValidTarget(E.Range) &&
                        GetEStackCount(target) >= Menu.Item("ComboEStack", true).GetValue<Slider>().Value) ||
                        (Menu.Item("ComboEFull", true).GetValue<bool>() && GetEStackCount(target) >= 6))
                    {
                        E.Cast();
                    }
                }

                if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady())
                {
                    var count =
                        HeroManager.Enemies
                            .Count(x => x.DistanceToPlayer() <= Menu.Item("ComboRRange", true).GetValue<Slider>().Value);

                    if (count >= Menu.Item("ComboRCount", true).GetValue<Slider>().Value)
                    {
                        R.Cast();
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
                if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady())
                {
                    var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                    if (CheckTarget(wTarget, W.Range))
                    {
                        W.CastTo(wTarget);
                    }
                }

                if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady())
                {
                    var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                    if (CheckTarget(eTarget, E.Range))
                    {
                        if (eTarget.DistanceToPlayer() > E.Range * 0.8 && eTarget.IsValidTarget(E.Range) && 
                            GetEStackCount(eTarget) >= Menu.Item("HarassEStack", true).GetValue<Slider>().Value)
                        {
                            E.Cast();
                        }
                        else if (GetEStackCount(eTarget) >= 6)
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Menu.Item("JungleStealE", true).GetValue<bool>() && E.IsReady())
            {
                var mobs = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                foreach (var mob in mobs.Where(x => !x.Name.ToLower().Contains("mini") && x.DistanceToPlayer() <= E.Range))
                {
                    if (E.IsKillable(mob))
                    {
                        E.Cast();
                    }
                }
            }
        }

        private void OnNotify(GameNotifyEventArgs Args)
        {
            if (Me.IsDead)
            {
                PlayerIsKillTarget = false;
            }
            else if (!Me.IsDead)
            {
                if (Args.EventId == GameEventId.OnChampionDie && Args.NetworkId == Me.NetworkId)
                {
                    PlayerIsKillTarget = true;

                    LeagueSharp.Common.Utility.DelayAction.Add(8000, () =>
                    {
                        PlayerIsKillTarget = false;
                    });
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen)
            {
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
                        var x in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg((float)ComboDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
                }
            }
        }

        private int GetEStackCount(Obj_AI_Base target)
        {
            return target.HasBuff("TwitchDeadlyVenom") ? target.GetBuffCount("TwitchDeadlyVenom") : 0;
        }
    }
}
