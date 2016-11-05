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
            OnGameLoad();
        }

        private static void OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Talon")
            {
                return;
            }

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 650f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 650f);
            W.SetSkillshot(0.25f, 65f, 2300f, false, SkillshotType.SkillshotCone);

            Ignite = Me.GetSpellSlot("SummonerDot");

            SkinID = Me.SkinId;

            Menu = new Menu("Flowers' Talon", "Flowers' Talon", true);

            xQxTargetSelector.Init(Menu);

            var OrbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(OrbMenu);
            }

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboRKillSteal", "Use R|KillSteal", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R|Counts Enemies In Range >= x(0 is off)", true).SetValue(
                        new Slider(3, 0, 6)));
                ComboMenu.AddItem(
                    new MenuItem("ComboMode", "Mode: ", true).SetValue(
                        new StringList(new[] {"E -> Q -> W -> R", "R -> E -> Q -> W"})));
                ComboMenu.AddItem(new MenuItem("ComboYoumuu", "Use Youmuu", true).SetValue(true));
				ComboMenu.AddItem(new MenuItem("ComboTiamat", "Use Tiamat", true).SetValue(true));
				ComboMenu.AddItem(new MenuItem("ComboHydra", "Use Hydra", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                HarassMenu.AddItem(new MenuItem("HarassTiamat", "Use Tiamat", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassHydra", "Use Hydra", true).SetValue(true));
                HarassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var LaneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearWCount", "Use W| Min Hit Count >= x", true).SetValue(new Slider(60)));
                LaneClearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
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
                KillStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
            }

            var FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                FleeMenu.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
                FleeMenu.AddItem(new MenuItem("FleeE", "Use E", true).SetValue(true));
                FleeMenu.AddItem(new MenuItem("FleeKey", "Flee Key", true).SetValue(new KeyBind('Z', KeyBindType.Press)));
            }

            var SkinMenu = Menu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
            {
                SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged += EnbaleSkin;
                SkinMenu.AddItem(
                    new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(
                        new StringList(new[]
                            {"Classic", "Renegade Talon", "Crimson Elite Talon", "Dragonblade Talon", "SSW Talon"})));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
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
                        x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) && x.Health < Q.GetDamage(x)))
                {
                    if (CheckTarget(target, Orbwalking.GetRealAutoAttackRange(Me)))
                    {
                        Q.Cast();
                        return;
                    }
                }
            }

            if (Menu.Item("KillStealW", true).GetValue<bool>() && W.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range) && x.Health < W.GetDamage(x)))
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
                if (target.Health*1.2 < ComboDamage(target) && Menu.Item("ComboYoumuu", true).GetValue<bool>() &&
                    Items.HasItem(3142) && Items.CanUseItem(3142) &&
                    target.DistanceToPlayer() > 400)
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
                        if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(target);
                        }
                        else if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() &&
                            target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)))
                        {
                            Q.Cast();
                        }
                        else if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
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
                    case 1:
                        if (Menu.Item("ComboR", true).GetValue<bool>() && R.IsReady() &&
                            ((target.Health <= ComboDamage(target) && !target.IsDead &&
                              Menu.Item("ComboRKillSteal", true).GetValue<bool>()) ||
                             (HeroManager.Enemies.Count(x => x.IsValidTarget(R.Range)) >=
                              Menu.Item("ComboRCount", true).GetValue<Slider>().Value)))
                        {
                            R.Cast();
                        }
                        else if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(target);
                        }
                        else if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() &&
                            target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)))
                        {
                            Q.Cast();
                        }
                        else if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
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

                    if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(target);
                    }
                    else if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady() &&
                        target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)))
                    {
                        Q.Cast();
                    }
                    else if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.Cast(target.ServerPosition);
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
                        var wFarm =
                            MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.To2D()).ToList(),
                                W.Width, W.Range);

                        if (wFarm.MinionsHit >= Menu.Item("LaneClearWCount", true).GetValue<Slider>().Value)
                        {
                            W.Cast(wFarm.Position);
                        }
                    }

                    if (Menu.Item("LaneClearQ", true).GetValue<bool>() && Q.IsReady() && Orbwalking.CanAttack())
                    {
                        var qminion =
                            minions.FirstOrDefault(
                                x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) && x.Health < Q.GetDamage(x));

                        if (qminion != null)
                        {
                            Q.Cast();
                            Orbwalker.ForceTarget(qminion);
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

                if (Menu.Item("FleeE", true).GetValue<bool>() && E.IsReady())
                {
                    var minion =
                        MinionManager.GetMinions(Me.Position, E.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .Where(x => x.IsValidTarget(E.Range) && E.CanCast(x))
                            .OrderBy(x => x.Position.Distance(Game.CursorPos))
                            .FirstOrDefault();

                    if (minion != null)
                    {
                        E.CastOnUnit(minion);
                    }
                }

                if (Menu.Item("FleeW", true).GetValue<bool>() && W.IsReady())
                {
                    var target =
                        HeroManager.Enemies.FirstOrDefault(
                            x =>
                                x.IsValidTarget(W.Range) && !x.HasBuffOfType(BuffType.SpellShield) &&
                                x.DistanceToPlayer() > 300);

                    if (target != null)
                    {
                        W.Cast(target.ServerPosition);
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

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Args.Slot == SpellSlot.E)
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

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = Args.Target as AIHeroClient;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    ItemsUse(Menu.Item("ComboYoumuu", true).GetValue<bool>(), 
					         Menu.Item("ComboTiamat", true).GetValue<bool>(), 
							 Menu.Item("ComboHydra", true).GetValue<bool>());

                    if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady())
                    {
                        Q.Cast();
                    }
                    else if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady())
                    {
                        W.Cast(target.ServerPosition);
                    }
                }
            }
            else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
                {
                    var target = Args.Target as AIHeroClient;

                    if (target != null && !target.IsDead && !target.IsZombie)
                    {
                        ItemsUse(false,
                                 Menu.Item("HarassTiamat", true).GetValue<bool>(),
                                 Menu.Item("HarassHydra", true).GetValue<bool>());

                        if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            Q.Cast();
                        }
                        else if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady())
                        {
                            W.Cast(target.ServerPosition);
                        }
                    }
                }
            }
            else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                {
                    var mobs = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me), MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady())
                        {
                            var mob = mobs.FirstOrDefault(x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)));

                            if (mob != null)
                            {
                                Q.Cast();
                            }
                        }

                        ItemsUse(false, true, true);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs Args)
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
    }
}
