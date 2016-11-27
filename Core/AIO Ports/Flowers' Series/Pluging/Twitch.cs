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

    internal class Twitch : Logic
    {
        private bool PlayerIsKillTarget;
        private readonly Menu Menu = Championmenu;

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
                ComboMenu.AddItem(new MenuItem("ComboRYouMuu", "Use R| Auto Youmuu?", true).SetValue(true));
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

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearECount", "If E CanHit Counts >= x", true).SetValue(new Slider(3, 1, 5)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "If Player ManaPercent >= %", true).SetValue(new Slider(60)));
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

            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Game.OnNotify += OnNotify;
            Drawing.OnDraw += OnDraw;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.Item("ComboRYouMuu", true).GetValue<bool>() && Orbwalker.GetTarget() != null &&
                Orbwalker.GetTarget() is AIHeroClient && Me.HasBuff("TwitchFullAutomatic"))
            {
                if (Items.HasItem(3142))
                {
                    Items.UseItem(3142);
                }
            }
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
                    LaneClear();
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
                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() &&
                    target.DistanceToPlayer() <= Menu.Item("ComboQRange", true).GetValue<Slider>().Value &&
                    (Me.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(Me) + 150) >= 2 || Me.HealthPercent <= 50 ||
                     (target.HealthPercent <= 80 && target.HealthPercent >= 30)))
                {
                    Q.Cast();
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range) &&
                    Me.Mana >= W.Instance.SData.Mana + E.Instance.SData.Mana + R.Instance.SData.Mana)
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

        private void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("LaneClearE", true).GetValue<bool>() && E.IsReady())
                {
                    var eKillMinionsCount =
                        MinionManager.GetMinions(Me.Position, E.Range)
                            .Count(
                                x =>
                                    x.DistanceToPlayer() <= E.Range && x.HasBuff("TwitchDeadlyVenom") &&
                                    x.Health < E.GetDamage(x));

                    if (eKillMinionsCount >= Menu.Item("LaneClearECount", true).GetValue<Slider>().Value)
                    {
                        E.Cast();
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (Menu.Item("JungleStealE", true).GetValue<bool>() && E.IsReady())
            {
                var mobs = MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                foreach (
                    var mob in
                    mobs.Where(
                        x =>
                            !x.Name.ToLower().Contains("mini") && x.DistanceToPlayer() <= E.Range &&
                            x.HasBuff("TwitchDeadlyVenom")))
                {
                    if (mob.Health < E.GetDamage(mob))
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
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
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
                        var x in HeroManager.Enemies.Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
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
