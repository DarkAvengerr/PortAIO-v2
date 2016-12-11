using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Talon
{
    using Common;
    using System;
    using System.Linq;
    using SharpDX;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    using static Common.Common;

    internal class Program
    {
        private static Spell Q;
        private static Spell Q1;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static Menu Menu;
        private static int SkinID;
        private static AIHeroClient Me;
        private static Orbwalking.Orbwalker Orbwalker;
        private static SpellSlot Ignite = SpellSlot.Unknown;
        private static HpBarDraw HpBarDraw = new HpBarDraw();

        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != "Talon")
            {
                return;
            }

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 170f);
            Q1 = new Spell(SpellSlot.Q, 550f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 500f);
            R = new Spell(SpellSlot.R, 650f);

            Ignite = Me.GetSpellSlot("SummonerDot");

            SkinID = Me.SkinId;

            Menu = new Menu("Flowers' Talon", "Flowers' Talon", true);

            xQxTargetSelector.Init(Menu);

            var orbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(orbMenu);
            }

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboQJump", "Use Q Jump to target", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRKillSteal", "Use R|KillSteal", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R|Counts Enemies In Range >= x(0 is off)", true).SetValue(
                        new Slider(3, 0, 6)));
                comboMenu.AddItem(
                    new MenuItem("ComboMode", "Mode: ", true).SetValue(
                        new StringList(new[] { "W -> Q -> R", "Q -> W -> R", "R -> Q -> W"})));
                comboMenu.AddItem(new MenuItem("ComboYoumuu", "Use Youmuu", true).SetValue(true));
				comboMenu.AddItem(new MenuItem("ComboTiamat", "Use Tiamat", true).SetValue(true));
				comboMenu.AddItem(new MenuItem("ComboHydra", "Use Hydra", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassQJump", "Use Q Jump to target", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassTiamat", "Use Tiamat", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassHydra", "Use Hydra", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var laneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                laneClearMenu.AddItem(
                    new MenuItem("LaneClearWCount", "Use W| Min Hit Count >= x", true).SetValue(new Slider(60)));
                laneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var jungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                jungleClearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(30)));
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeQ", "Use Q", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeE", "Use E", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeKey", "Flee Key", true).SetValue(new KeyBind('Z', KeyBindType.Press)));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qCheck = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qCheck.AddItem(new MenuItem("QCheck", "Enable Q Jump Check", true).SetValue(true));
                    qCheck.AddItem(
                        new MenuItem("QCheckcount", "If Cast Position Enemies Count >= x| Dont Cast", true).SetValue(
                            new Slider(3, 1, 5)));
                    qCheck.AddItem(
                        new MenuItem("TurretCheck", "If Under Turret or Cast Position Under Turret| Dont Cast", true)
                            .SetValue(true));
                    qCheck.AddItem(new MenuItem("SmartIgnore", "Smart Ignote Turret Check?", true).SetValue(true));
                    qCheck.AddItem(
                        new MenuItem("IgnoteCanKill", "Ignote Check: When Target Can Kill", true).SetValue(true));
                    qCheck.AddItem(
                        new MenuItem("IgnoteHp", "Or Player HealthPercent >= x", true).SetValue(new Slider(80)));
                }

                var eCheck = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eCheck.AddItem(new MenuItem("todo", "TODO!", true));
                }

                var skinMenu = miscMenu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
                {
                    skinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged +=
                        EnbaleSkin;
                    skinMenu.AddItem(
                        new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(
                            new StringList(new[]
                                {"Classic", "Renegade Talon", "Crimson Elite Talon", "Dragonblade Talon", "SSW Talon"})));
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawQMax", "Draw Q Max Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        private static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                //ObjectManager.//Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
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
                case Orbwalking.OrbwalkingMode.None:
                    Flee();
                    break;
            }
        }

        private static void KillSteal()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(170 + Me.BoundingRadius) && x.Health < GetQDamage(x)))
                {
                    if (CheckTarget(target, 170 + Me.BoundingRadius))
                    {
                        Q.Cast();
                        return;
                    }
                }
            }

            if (Menu.Item("KillStealW", true).GetValue<bool>() && W.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && x.Health < GetWDamage(x) + GetW1Damage(x)))
                {
                    if (CheckTarget(target, W.Range))
                    {
                        W.Cast(target.ServerPosition);
                        return;
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = xQxTargetSelector.GetTarget(W.Range);

            if (CheckTarget(target, W.Range))
            {
                if (target.Health*1.3 < ComboDamage(target) && Menu.Item("ComboYoumuu", true).GetValue<bool>() &&
                    Items.HasItem(3142) && Items.CanUseItem(3142) &&
                    target.DistanceToPlayer() > 500)
                {
                    Items.UseItem(3142);
                }

                if (target.Health < ComboDamage(target) && Menu.Item("ComboIgnite", true).GetValue<bool>() &&
                    Ignite != SpellSlot.Unknown && Ignite.IsReady())
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }

				ItemsUse(false, Menu.Item("ComboTiamat", true).GetValue<bool>(), 
				         Menu.Item("ComboHydra", true).GetValue<bool>());
				
                switch (Menu.Item("ComboMode", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() &&
                            target.IsValidTarget(W.Range - 120))
                        {
                            W.Cast(target.ServerPosition);
                        }
                        else if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            if (Menu.Item("ComboQJump", true).GetValue<bool>())
                            {
                                if (target.IsValidTarget(Q1.Range) && CheckQSafe(target))
                                {
                                    Q1.CastOnUnit(target);
                                }
                            }
                            else if (target.IsValidTarget(Q.Range))
                            {
                                Q.CastOnUnit(target);
                            }
                        }
                        else if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() &&
                                 ((target.Health <= ComboDamage(target) && !target.IsDead &&
                                   Menu.Item("ComboRKillSteal", true).GetValue<bool>()) ||
                                  (HeroManager.Enemies.Count(x => x.IsValidTarget(R.Range)) >=
                                   Menu.Item("ComboRCount", true).GetValue<Slider>().Value)))
                        {
                            R.Cast();
                        }
                        break;
                    case 1:
                        if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            if (Menu.Item("ComboQJump", true).GetValue<bool>())
                            {
                                if (target.IsValidTarget(Q1.Range) && CheckQSafe(target))
                                {
                                    Q1.CastOnUnit(target);
                                }
                            }
                            else if (target.IsValidTarget(Q.Range))
                            {
                                Q.CastOnUnit(target);
                            }
                        }
                        else if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() &&
                                 target.IsValidTarget(W.Range - 120))
                        {
                            W.Cast(target.ServerPosition);
                        }
                        else if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() &&
                                 ((target.Health <= ComboDamage(target) && !target.IsDead &&
                                   Menu.Item("ComboRKillSteal", true).GetValue<bool>()) ||
                                  (HeroManager.Enemies.Count(x => x.IsValidTarget(R.Range)) >=
                                   Menu.Item("ComboRCount", true).GetValue<Slider>().Value)))
                        {
                            R.Cast();
                        }
                        break;
                    case 2:
                        if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() &&
                            ((target.Health <= ComboDamage(target)*1.1 && !target.IsDead &&
                              Menu.Item("ComboRKillSteal", true).GetValue<bool>()) ||
                             (HeroManager.Enemies.Count(x => x.IsValidTarget(R.Range)) >=
                              Menu.Item("ComboRCount", true).GetValue<Slider>().Value)))
                        {
                            R.Cast();
                        }
                        if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            if (Menu.Item("ComboQJump", true).GetValue<bool>())
                            {
                                if (target.IsValidTarget(Q1.Range) && CheckQSafe(target))
                                {
                                    Q1.CastOnUnit(target);
                                }
                            }
                            else if (target.IsValidTarget(Q.Range))
                            {
                                Q.CastOnUnit(target);
                            }
                        }
                        else if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() &&
                                 target.IsValidTarget(W.Range - 120))
                        {
                            W.Cast(target.ServerPosition);
                        }
                        break;
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
            {
                var target = xQxTargetSelector.GetTarget(W.Range);

                if (CheckTarget(target, W.Range))
                {
                    ItemsUse(false, Menu.Item("HarassTiamat", true).GetValue<bool>(),
                             Menu.Item("HarassHydra", true).GetValue<bool>());

                    if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() && 
                        target.DistanceToPlayer() <= Q.Range + Me.BoundingRadius)
                    {
                        Q.CastOnUnit(target);
                    }

                    if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range - 120))
                    {
                        W.Cast(target.ServerPosition);
                    }

                    if (Menu.Item("HarassQJump", true).GetValue<bool>() && Q1.IsReady() &&
                        target.IsValidTarget(Q1.Range) && CheckQSafe(target))
                    {
                        Q1.CastOnUnit(target);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("LaneClearMana", true).GetValue<Slider>().Value)
            {
                var minions = MinionManager.GetMinions(Me.Position, W.Range);

                if (minions.Any())
                {
                    if (Menu.Item("LaneClearW", true).GetValue<bool>() && W.IsReady())
                    {
                        if (minions.Count >= Menu.Item("LaneClearWCount", true).GetValue<Slider>().Value)
                        {
                            var min = minions.MaxOrDefault(x => x.DistanceToPlayer());

                            if (min != null)
                            {
                                W.Cast(min.Position);
                            }
                        }
                    }

                    if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady() && Orbwalking.CanAttack())
                    {
                        var qminion =
                            minions.FirstOrDefault(
                                x => x.IsValidTarget(Q.Range + Me.BoundingRadius) && x.Health < GetQDamage(x));

                        if (qminion != null)
                        {
                            Q.CastOnUnit(qminion);
                        }
                    }
                }
            }
        }

        private static void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Any())
                {
                    if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady())
                    {
                        var wFarm =
                            MinionManager.GetBestCircularFarmLocation(mobs.Select(x => x.Position.To2D()).ToList(),
                                W.Width, W.Range);

                        if (wFarm.MinionsHit >= 1)
                        {
                            W.Cast(wFarm.Position);
                        }
                    }
                }
            }
        }

        private static void Flee()
        {
            if (Menu.Item("FleeKey", true).GetValue<KeyBind>().Active)
            {
                Orbwalking.MoveTo(Game.CursorPos);

                if (Menu.Item("FleeQ", true).GetValue<bool>() && Q1.IsReady())
                {
                    var minion =
                        MinionManager.GetMinions(Me.Position, 550f, MinionTypes.All, MinionTeam.NotAlly)
                            .Where(x => x.DistanceToPlayer() > Q.Range + Me.BoundingRadius)
                            .OrderBy(x => x.Distance(Game.CursorPos))
                            .FirstOrDefault();

                    if (minion != null)
                    {
                        Q1.CastOnUnit(minion);
                    }
                }

                if (Menu.Item("FleeE", true).GetValue<bool>() && E.IsReady())
                {
                    var dashPos = Me.Position.Extend(Game.CursorPos, E.Range);

                    if (CanWallJump(dashPos))
                    {
                        E.Cast(dashPos);
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Args.Slot == SpellSlot.Q)
            {
                ItemsUse(Menu.Item("ComboYoumuu", true).GetValue<bool>());
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            if (Args.Target is Obj_LampBulb)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    var ctarget = Args.Target as AIHeroClient;

                    if (ctarget != null && !ctarget.IsDead && !ctarget.IsZombie)
                    {
                        if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() &&
                            ctarget.DistanceToPlayer() <= Q.Range + Me.BoundingRadius)
                        {
                            Q.Cast();
                        }

                        ItemsUse(Menu.Item("ComboYoumuu", true).GetValue<bool>(),
                            Menu.Item("ComboTiamat", true).GetValue<bool>(),
                            Menu.Item("ComboHydra", true).GetValue<bool>());

                        if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && ctarget.IsValidTarget(500))
                        {
                            W.Cast(ctarget.ServerPosition);
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
                    {
                        var htarget = Args.Target as AIHeroClient;

                        if (htarget != null && !htarget.IsDead && !htarget.IsZombie)
                        {
                            if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() &&
                                htarget.DistanceToPlayer() <= Q.Range + Me.BoundingRadius)
                            {
                                Q.CastOnUnit(htarget);
                            }

                            ItemsUse(false,
                                Menu.Item("HarassTiamat", true).GetValue<bool>(),
                                Menu.Item("HarassHydra", true).GetValue<bool>());

                            if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady() && htarget.IsValidTarget(500))
                            {
                                W.Cast(htarget.ServerPosition);
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                    {
                        var mobs = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me), MinionTypes.All, MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth);

                        if (mobs.Any())
                        {
                            if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                            {
                                var mob = mobs.FirstOrDefault(x => x.DistanceToPlayer() <= Q.Range + Me.BoundingRadius);

                                if (mob != null)
                                {
                                    Q.CastOnUnit(mob);
                                }
                            }

                            ItemsUse(false, true, true);
                        }
                    }
                    break;
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q1.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range + Me.BoundingRadius, Color.FromArgb(188, 6, 248), 2);
                }

                if (Menu.Item("DrawQMax", true).GetValue<bool>() && Q1.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q1.Range, Color.FromArgb(154, 249, 39), 2);
                }

                if (Menu.Item("DrawW", true).GetValue<bool>() && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 2);
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

        private static void ItemsUse(bool UseYoumuu = false, bool UseTiamat = false, bool UseHydra = false)
        {
            if (UseYoumuu && Items.HasItem(3142) && Items.CanUseItem(3142) && Me.CountEnemiesInRange(W.Range) > 0)
            {
                Items.UseItem(3142);
            }

            if (UseTiamat && Me.CountEnemiesInRange(385f) > 0 && Items.HasItem(3077) && Items.CanUseItem(3077))
            {
                Items.UseItem(3077);
            }

            if (UseHydra && Me.CountEnemiesInRange(385f) > 0)
            {
                if (Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }
                else if(Items.HasItem(3748) && Items.CanUseItem(3748))
                {
                    Items.UseItem(3748);
                }
            }
        }

        private static bool CheckQSafe(AIHeroClient target)
        {
            if (!Q1.IsReady())
            {
                return false;
            }

            if (target.DistanceToPlayer() <= Q.Range + Me.BoundingRadius)
            {
                return false;
            }

            if (Menu.Item("QCheck", true).GetValue<bool>())
            {
                var pos = target.ServerPosition;

                if (pos.CountEnemiesInRange(500) >= Menu.Item("QCheckcount", true).GetValue<Slider>().Value)
                {
                    return false;
                }

                if (!Menu.Item("SmartIgnore", true).GetValue<bool>() && Menu.Item("TurretCheck", true).GetValue<bool>() &&
                    pos.UnderTurret(true))
                {
                    return false;
                }

                if (Menu.Item("SmartIgnore", true).GetValue<bool>() && pos.UnderTurret(true))
                {
                    if (Menu.Item("IgnoteCanKill", true).GetValue<bool>() &&
                        target.Health + target.HPRegenRate*2 > ComboDamage(target))
                    {
                        return false;
                    }

                    if (Me.HealthPercent <= Menu.Item("IgnoteHp", true).GetValue<Slider>().Value)
                    {
                        return false;
                    }

                    return true;
                }

                return true;
            }

            return true;
        }
    }
}
