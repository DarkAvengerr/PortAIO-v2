using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_ADC_Series.Pluging
{
    using ADCCOMMON;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    using Orbwalking = ADCCOMMON.Orbwalking;

    internal class Ezreal : Logic
    {
        public Ezreal()
        {
            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 475f);
            R = new Spell(SpellSlot.R, 10000f);
            EQ = new Spell(SpellSlot.Q, 1150f + 475f);

            EQ.SetSkillshot(0.25f + 0.65f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboECheck", "Use E |Safe Check", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboEWall", "Use E |Wall Check", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                harassMenu.AddItem(new MenuItem("Harasstarget", "Harass List:", true));
                foreach (var target in HeroManager.Enemies)
                {
                    harassMenu.AddItem(new MenuItem("Harasstarget" + target.ChampionName.ToLower(),
                        target.ChampionName, true).SetValue(true));
                }
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearQOut", "Use Q| Out of Attack Range Farm", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(20)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var lastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                lastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q", true).SetValue(true));
                lastHitMenu.AddItem(
                    new MenuItem("LastHitMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(20)));
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "KillSteal Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealW", "KillSteal W", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(new MenuItem("Gapcloser", "Anti GapCloser", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("AntiMelee", "Anti Melee", true).SetValue(true));
                    eMenu.AddItem(
                        new MenuItem("AntiMeleeHp", "Anti Melee|When Player HealthPercent <= x%", true).SetValue(
                            new Slider(50)));
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rMenu.AddItem(new MenuItem("AutoR", "Auto R?", true).SetValue(true));
                    rMenu.AddItem(
                        new MenuItem("RRange", "Use R |Min Cast Range >= x", true).SetValue(new Slider(800, 0, 1500)));
                    rMenu.AddItem(
                        new MenuItem("RMaxRange", "Use R |Max Cast Range >= x", true).SetValue(new Slider(3000, 1500, 5000)));
                    rMenu.AddItem(
                        new MenuItem("RMinCast", "Use R| Min Hit Enemies >= x", true).SetValue(new Slider(2, 1, 6)));
                    rMenu.AddItem(
                        new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
                }

                miscMenu.AddItem(new MenuItem("PlayMode", "Play Mode: ", true).SetValue(new StringList(new[] {"AD", "AP"})));
            }

            var utilityMenu = Menu.AddSubMenu(new Menu("Utility", "Utility"));
            {
                var skinMenu = utilityMenu.AddSubMenu(new Menu("Skin Change", "Skin Change"));
                {
                    SkinManager.AddToMenu(skinMenu);
                }

                var autoLevelMenu = utilityMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }

                var stackMenu = utilityMenu.AddSubMenu(new Menu("Auto Stack", "Auto Stack"));
                {
                    StackManager.AddToMenu(stackMenu, true, true, false);
                }

                var itemsMenu = utilityMenu.AddSubMenu(new Menu("Items", "Items"));
                {
                    ItemsManager.AddToMenu(itemsMenu);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu);
            }

            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        private bool isADMode => Menu.GetList("PlayMode") == 0;

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (R.Level > 0)
            {
                R.Range = Menu.GetSlider("RMaxRange");
            }

            if (Menu.GetKey("SemiR") && R.IsReady())
            {
                OneKeyCastR();
            }

            KillSteal();
            AutoRLogic();

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
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }

        private void AutoRLogic()
        {
            if (Menu.GetBool("AutoR") && R.IsReady() && Me.CountEnemiesInRange(1000) == 0)
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.Check(R.Range) && x.DistanceToPlayer() >= Menu.GetSlider("RRange")))
                {
                    if (!target.CanMoveMent() && target.IsValidTarget(EQ.Range) &&
                        R.GetDamage(target) + Q.GetDamage(target)*3 >= target.Health + target.HPRegenRate*2)
                    {
                        R.Cast(target, true);
                    }

                    if (R.GetDamage(target) > target.Health + target.HPRegenRate*2 && target.Path.Length < 2 &&
                        R.GetPrediction(target, true).Hitchance >= HitChance.High)
                    {
                        R.Cast(target, true);
                    }

                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Me.CountEnemiesInRange(800) == 0)
                    {
                        R.CastIfWillHit(target, Menu.GetSlider("RMinCast"), true);
                    }
                }
            }
        }

        private void KillSteal()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.Check(Q.Range)))
            {
                if (Menu.GetBool("KillStealQ") && Q.GetDamage(target) > target.Health && target.IsValidTarget(Q.Range))
                {
                    SpellManager.PredCast(Q, target);
                    return;
                }

                if (Menu.GetBool("KillStealW") && W.GetDamage(target) > target.Health && target.IsValidTarget(W.Range))
                {
                    SpellManager.PredCast(W, target, true);
                    return;
                }

                if (Menu.GetBool("KillStealQ") && Menu.GetBool("KillStealW") &&
                    target.Health < Q.GetDamage(target) + W.GetDamage(target) && target.IsValidTarget(W.Range))
                {
                    SpellManager.PredCast(W, target, true);
                    SpellManager.PredCast(Q, target);
                    return;
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         (isADMode
                             ? TargetSelector.GetTarget(EQ.Range, TargetSelector.DamageType.Physical)
                             : TargetSelector.GetTarget(EQ.Range, TargetSelector.DamageType.Magical));

            if (target.Check(EQ.Range))
            {
                if (Menu.GetBool("ComboQ") && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    SpellManager.PredCast(Q, target);
                }

                if (Menu.GetBool("ComboW") && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    SpellManager.PredCast(W, target, true);
                }

                if (Menu.GetBool("ComboE") && E.IsReady() && target.IsValidTarget(EQ.Range))
                {
                    if (Menu.GetBool("ComboECheck") && !Me.UnderTurret(true) &&
                        Me.CountEnemiesInRange(1200) <= 2)
                    {
                        var useECombo = false;

                        if (target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                            target.Check() && HealthPrediction.GetHealthPrediction(target, 750) > 0)
                        {
                            if (target.Health < E.GetDamage(target) + Me.GetAutoAttackDamage(target) &&
                                target.Distance(Game.CursorPos) < Me.Distance(Game.CursorPos))
                            {
                                useECombo = true;
                            }

                            if (target.Health < E.GetDamage(target) + W.GetDamage(target) && W.IsReady() &&
                                target.Distance(Game.CursorPos) + 350 < Me.Distance(Game.CursorPos))
                            {
                                useECombo = true;
                            }

                            if (target.Health < E.GetDamage(target) + Q.GetDamage(target) && Q.IsReady() &&
                                target.Distance(Game.CursorPos) + 300 < Me.Distance(Game.CursorPos))
                            {
                                useECombo = true;
                            }
                        }

                        if (useECombo)
                        {
                            var CastEPos = Me.Position.Extend(target.Position, 475f);

                            if (Menu.GetBool("ComboEWall"))
                            {
                                if (NavMesh.GetCollisionFlags(CastEPos) != CollisionFlags.Wall &&
                                    NavMesh.GetCollisionFlags(CastEPos) != CollisionFlags.Building &&
                                    NavMesh.GetCollisionFlags(CastEPos) != CollisionFlags.Prop)
                                {
                                    E.Cast(CastEPos);
                                    useECombo = false;
                                }
                            }
                            else
                            {
                                E.Cast(CastEPos);
                                useECombo = false;
                            }
                        }
                    }
                }

                if (Menu.GetBool("ComboR") && R.IsReady())
                {
                    if (Me.UnderTurret(true) || Me.CountEnemiesInRange(800) > 1)
                    {
                        return;
                    }

                    foreach (
                        var rTarget in
                        HeroManager.Enemies.Where(
                            x =>
                                x.Check(R.Range) &&
                                target.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                                HealthPrediction.GetHealthPrediction(x, 3000) > 0))
                    {
                        if (target.Health < R.GetDamage(rTarget) && R.GetPrediction(rTarget).Hitchance >= HitChance.High &&
                            target.DistanceToPlayer() > Q.Range + E.Range/2)
                        {
                            R.Cast(rTarget, true);
                        }

                        if (rTarget.IsValidTarget(Q.Range + E.Range) &&
                            R.GetDamage(rTarget) + (Q.IsReady() ? Q.GetDamage(rTarget) : 0) +
                            (W.IsReady() ? W.GetDamage(rTarget) : 0) > rTarget.Health + rTarget.HPRegenRate*2)
                        {
                            R.Cast(rTarget, true);
                        }
                    }
                }
            }
        }

        private void Harass()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.Item("HarassQ", true).GetValue<bool>() && Q.IsReady())
                {
                    foreach (
                        var target in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(Q.Range) &&
                                Menu.GetBool("Harasstarget" + x.ChampionName.ToLower())))
                    {
                        if (target.Check(Q.Range))
                        {
                            SpellManager.PredCast(Q, target);
                        }
                    }
                }

                if (Menu.Item("HarassW", true).GetValue<bool>() && W.IsReady())
                {
                    foreach (
                        var target in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget(W.Range) &&
                                Menu.GetBool("Harasstarget" + x.ChampionName.ToLower())))
                    {
                        if (target.Check(W.Range))
                        {
                            SpellManager.PredCast(W, target, true);
                        }
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
                    var minions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (minions.Any())
                    {
                        if (Menu.GetBool("LaneClearQOut"))
                        {
                            var mins =
                                minions.Where(
                                    x =>
                                        x.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                                        x.Health < Q.GetDamage(x) &&
                                        HealthPrediction.GetHealthPrediction(x, 250) > 0);

                            Q.Cast(mins.Any() ? mins.FirstOrDefault() : minions.FirstOrDefault(), true);
                        }
                        else
                        {
                            Q.Cast(minions.FirstOrDefault(), true);
                        }
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("JungleClearQ") && Q.IsReady())
                {
                    var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral);

                    if (mobs.Any())
                    {
                        Q.Cast(mobs.FirstOrDefault(), true);
                    }
                }
            }
        }

        private void LastHit()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LastHitMana")))
            {
                if (Menu.GetBool("LastHitQ") && Q.IsReady())
                {
                    var minions =
                        MinionManager.GetMinions(Me.Position, Q.Range)
                            .Where(
                                x =>
                                    x.DistanceToPlayer() <= Q.Range &&
                                    x.DistanceToPlayer() > Orbwalking.GetRealAutoAttackRange(Me) &&
                                    x.Health < Q.GetDamage(x));

                    if (minions.Any())
                    {
                        Q.Cast(minions.FirstOrDefault(), true);
                    }
                }
            }
        }

        private void OneKeyCastR()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(3000, TargetSelector.DamageType.Magical);

            if (target.Check(3000f))
            {
                SpellManager.PredCast(R, target, true);
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
                {
                    if (Menu.GetBool("LaneClearW") && W.IsReady() && Me.CountEnemiesInRange(850) == 0)
                    {
                        var turret = Args.Target as Obj_AI_Turret;

                        if (turret != null)
                        {
                            if (W.IsReady() && Me.CountAlliesInRange(W.Range) >= 1)
                            {
                                W.Cast(HeroManager.Allies.Find(x => x.DistanceToPlayer() <= W.Range));
                            }
                        }
                    }
                }
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                var t = (Obj_AI_Base)Args.Target;

                if (t != null && !t.IsDead && !t.IsZombie)
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        if (t is AIHeroClient)
                        {
                            var target = (AIHeroClient)Args.Target;

                            if (Menu.GetBool("ComboQ") && Q.IsReady() && t.IsValidTarget(Q.Range))
                            {
                                SpellManager.PredCast(Q, target);
                            }

                            if (Menu.GetBool("ComboW") && W.IsReady() && t.IsValidTarget(W.Range))
                            {
                                SpellManager.PredCast(W, target, true);
                            }
                        }
                    }

                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    {
                        if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
                        {
                            if (t is AIHeroClient)
                            {
                                var target = (AIHeroClient)Args.Target;

                                if (Menu.GetBool("Harasstarget" + target.ChampionName.ToLower()))
                                {
                                    if (Menu.GetBool("HarassQ") && Q.IsReady() && t.IsValidTarget(Q.Range))
                                    {
                                        SpellManager.PredCast(Q, target);
                                    }

                                    if (Menu.GetBool("HarassW") && W.IsReady() && t.IsValidTarget(W.Range))
                                    {
                                        SpellManager.PredCast(W, target, true);
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
            if (Menu.GetBool("Gapcloser") && E.IsReady())
            {
                if (Args.End.DistanceToPlayer() <= 200)
                {
                    E.Cast(Me.Position.Extend(Args.Sender.Position, -E.Range));
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.GetBool("AntiMelee") && E.IsReady() && Me.HealthPercent <= Menu.GetSlider("AntiMeleeHp"))
            {
                if (sender != null && sender.IsEnemy && Args.Target != null && Args.Target.IsMe)
                {
                    if (sender.Type == Me.Type && sender.IsMelee)
                    {
                        E.Cast(Me.Position.Extend(sender.Position, -E.Range));
                    }
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (Menu.GetBool("DrawQ") && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.GetBool("DrawW") && W.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 1);
                }

                if (Menu.GetBool("DrawE") && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }
            }
        }
    }
}
