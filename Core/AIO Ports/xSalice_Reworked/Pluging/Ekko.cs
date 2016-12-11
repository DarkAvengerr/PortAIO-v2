using EloBuddy; 
using LeagueSharp.Common; 
namespace xSaliceResurrected_Rework.Pluging
{
    using Base;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Managers;
    using Utilities;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;

    internal class Ekko : Champion
    {
        private GameObject rMissile;

        public Ekko()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 750f);
            SpellManager.W = new Spell(SpellSlot.W, 1600f);
            SpellManager.E = new Spell(SpellSlot.E, 325f);
            SpellManager.R = new Spell(SpellSlot.R, 375f);

            SpellManager.Q.SetSkillshot(0.25f, 60f, 1650f, false, SkillshotType.SkillshotLine);
            SpellManager.W.SetSkillshot(2.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.R.SetSkillshot(0.4f, 280f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                combo.AddItem(
                    new MenuItem("UseRComboKill", "Use R| Kill Target", true).SetValue(true));
                combo.AddItem(
                    new MenuItem("UseRComboSafe", "Use R| Safe Myself", true).SetValue(true));
                combo.AddItem(
                    new MenuItem("UseRComboSafeHp", "Use R| Safe MySelf Hp <= x%", true).SetValue(new Slider(20)));
                combo.AddItem(
                    new MenuItem("UseRComboHit", "Use R| Hit Enemies", true).SetValue(true));
                combo.AddItem(
                    new MenuItem("UseRComboHitCount", "Use R| Hit Enemies Count >= x", true).SetValue(new Slider(3, 1, 5)));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(
                    new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0],
                        KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 50);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("MinFarm", "Min Minion >= ", true).SetValue(new Slider(3, 1, 6)));
                farm.AddItem(new MenuItem("UseEFarm", "Use E", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 50);
                Menu.AddSubMenu(farm);
            }

            var jungle = new Menu("JungleClear", "JungleClear");
            {
                jungle.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                jungle.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                jungle.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(jungle, "JungleClear", 30);
                Menu.AddSubMenu(jungle);
            }

            var flee = new Menu("Flee", "Flee");
            {
                flee.AddItem(new MenuItem("UseEFlee", "Use E", true).SetValue(true));
                Menu.AddSubMenu(flee);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                var qMenu = new Menu("QMenu", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Auto_Q_Kill", "Auto Q KillSteal", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Auto_Q_Slow", "Auto Q Slow", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Auto_Q_Dashing", "Auto Q Dashing", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Auto_Q_Immobile", "Auto Q Immobile", true).SetValue(true));
                    miscMenu.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("WMenu", "WMenu");
                {
                    wMenu.AddItem(new MenuItem("W_On_Cc", "W On top of Hard CC", true).SetValue(true));
                    miscMenu.AddSubMenu(wMenu);
                }

                var eMenu = new Menu("EMenu", "EMenu");
                {
                    eMenu.AddItem(new MenuItem("Auto_E_Kill", "Auto E KillSteal", true).SetValue(true));
                    eMenu.AddItem(
                        new MenuItem("E_If_UnderTurret", "E Under Enemy Turret", true).SetValue(
                            new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle)));
                    eMenu.AddItem(
                        new MenuItem("Do_Not_E", "Do not E if >= Enemies Around location", true).SetValue(new Slider(3,
                            1, 5)));
                    eMenu.AddItem(new MenuItem("Do_Not_E_HP", "Do not E if HP <= %", true).SetValue(new Slider(20)));
                    miscMenu.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("RMenu", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("R_Safe_Net2", "R If Player HP <= %", true).SetValue(new Slider(10)));
                    miscMenu.AddSubMenu(rMenu);
                }

                Menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "Draw W", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R", true).SetValue(true));

                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                var drawFill =
                    new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true,
                        Color.FromArgb(90, 255, 169, 4)));
                drawMenu.AddItem(drawComboDamageMenu);
                drawMenu.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };

                Menu.AddSubMenu(drawMenu);
            }

            var customMenu = new Menu("Custom Perma Show", "Custom Perma Show");
            {
                var myCust = new CustomPermaMenu();
                customMenu.AddItem(
                    new MenuItem("custMenu", "Move Menu", true).SetValue(new KeyBind("L".ToCharArray()[0],
                        KeyBindType.Press)));
                customMenu.AddItem(new MenuItem("enableCustMenu", "Enabled", true).SetValue(true));
                customMenu.AddItem(myCust.AddToMenu("Combo Active: ", "Orbwalk"));
                customMenu.AddItem(myCust.AddToMenu("Harass Active: ", "Farm"));
                customMenu.AddItem(myCust.AddToMenu("Harass(T) Active: ", "FarmT"));
                customMenu.AddItem(myCust.AddToMenu("Laneclear Active: ", "LaneClear"));
                customMenu.AddItem(myCust.AddToMenu("Flee Active: ", "Flee"));
                customMenu.AddItem(myCust.AddToMenu("E Turret Active: ", "E_If_UnderTurret"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Q.GetDamage(target);

            if (E.IsReady())
                comboDamage += E.GetDamage(target);

            if (R.IsReady())
                comboDamage += R.GetDamage(target);

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 2);
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget(1500))
            {
                return;
            }

            var damg = GetComboDamage(target);

            ItemManager.Target = target;

            if (damg > target.Health - 50)
            {
                ItemManager.KillableTarget = true;
            }

            ItemManager.UseTargetted = true;

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                var qPred = Q.GetPrediction(target);

                if (qPred.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(qPred.CastPosition);
                }
            }

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
            {
                if (Menu.Item("W_On_Cc", true).GetValue<bool>())
                {
                    foreach (var enemies in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                    {
                        if (enemies.HasBuffOfType(BuffType.Snare) || enemies.HasBuffOfType(BuffType.Stun) ||
                            enemies.HasBuffOfType(BuffType.Fear) || enemies.HasBuffOfType(BuffType.Suppression))
                        {
                            W.Cast(enemies);
                            break;
                        }
                    }
                }

                var wPred = W.GetPrediction(target);

                if (wPred.Hitchance >= HitChance.VeryHigh)
                {
                    W.Cast(wPred.CastPosition);
                }
            }

            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(Q.Range))
            {
                var caseEPos = Player.Position.Extend(Game.CursorPos, E.Range);

                if (target.Distance(caseEPos) < 425f && ShouldE(caseEPos))
                {
                    E.Cast(caseEPos, true);
                }
            }

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady() && rMissile != null)
            {
                if (Menu.Item("UseRComboKill", true).GetValue<bool>() && rMissile.Position.CountEnemiesInRange(500) < 3)
                {
                    if (GetComboDamage(target) >= target.Health)
                    {
                        if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady())
                        {
                            if (Player.Distance(rMissile.Position) <= W.Range)
                            {
                                if (W.Cast(rMissile.Position))
                                {
                                    R.Cast();
                                }
                            }
                        }
                        else
                        {
                            R.Cast();
                        }
                    }
                }

                if (Menu.Item("UseRComboSafe", true).GetValue<bool>() &&
                    Player.HealthPercent <= Menu.Item("UseRComboSafeHp", true).GetValue<Slider>().Value &&
                    HeroManager.Enemies.Any(x => x.Distance(Player) <= 1000) &&
                    rMissile.Position.CountEnemiesInRange(600) <= 2)
                {
                    R.Cast();
                }

                if (Menu.Item("UseRComboHit", true).GetValue<bool>() && Menu.Item("UseWCombo", true).GetValue<bool>() &&
                    W.IsReady() &&
                    HeroManager.Enemies.Count(x => x.Distance(rMissile.Position) <= R.Range) >=
                    Menu.Item("UseRComboHitCount", true).GetValue<Slider>().Value &&
                    Player.Distance(rMissile.Position) <= W.Range)
                {
                    if (W.Cast(rMissile.Position))
                    {
                        R.Cast();
                    }
                }
            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget(1500))
            {
                return;
            }

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                var qPred = Q.GetPrediction(target);

                if (qPred.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(qPred.CastPosition);
                }
            }

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
            {
                if (Menu.Item("W_On_Cc", true).GetValue<bool>())
                {
                    foreach (var enemies in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                    {
                        if (enemies.HasBuffOfType(BuffType.Snare) || enemies.HasBuffOfType(BuffType.Stun) ||
                            enemies.HasBuffOfType(BuffType.Fear) || enemies.HasBuffOfType(BuffType.Suppression))
                        {
                            W.Cast(enemies);
                            break;
                        }
                    }
                }

                W.Cast(target.Position);
            }

            if (Menu.Item("UseEHarass", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(Q.Range))
            {
                var caseEPos = Player.Position.Extend(Game.CursorPos, E.Range);

                if (target.Distance(caseEPos) < 425f && ShouldE(caseEPos))
                {
                    E.Cast(caseEPos, true);
                }
            }
        }

        private bool ShouldE(Vector3 pos)
        {
            var maxEnemies = Menu.Item("Do_Not_E", true).GetValue<Slider>().Value;

            if (!Menu.Item("E_If_UnderTurret", true).GetValue<KeyBind>().Active && pos.UnderTurret(true))
            {
                return false;
            }

            if (Player.HealthPercent <= Menu.Item("Do_Not_E_HP", true).GetValue<Slider>().Value)
            {
                return false;
            }

            return pos.CountEnemiesInRange(600) < maxEnemies;
        }

        private void Flee()
        {
            if (Menu.Item("UseEFlee", true).GetValue<bool>() && E.IsReady())
            {
                E.Cast(Player.ServerPosition.Extend(Game.CursorPos, E.Range));
            }
        }

        private int TargetHitWithR()
        {
            if (!R.IsReady() || rMissile == null)
            {
                return 0;
            }

            return
                HeroManager.Enemies.Where(x => x.IsValidTarget())
                    .Count(x => rMissile.Position.Distance(Prediction.GetPrediction(x, .2f).UnitPosition) < 400);
        }

        private void AutoQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                if (Q.GetPrediction(target).Hitchance >= HitChance.High &&
                    (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare)) &&
                    Menu.Item("Auto_Q_Slow", true).GetValue<bool>())
                {
                    Q.Cast(target);
                }

                if (target.HasBuffOfType(BuffType.Slow) && Menu.Item("Auto_Q_Slow", true).GetValue<bool>())
                {
                    Q.Cast(target);
                }

                if (target.IsDashing() && Menu.Item("Auto_Q_Dashing", true).GetValue<bool>())
                {
                    Q.Cast(target);
                }
            }
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.Position, Q.Range);

            if (minions.Any())
            {
                if (Menu.Item("UseQFarm", true).GetValue<bool>() && Q.IsReady())
                {
                    var pred = MinionManager.GetBestLineFarmLocation(minions.Select(x => x.Position.To2D()).ToList(), 
                        Q.Width, Q.Range);

                    if (pred.MinionsHit >= Menu.Item("MinFarm", true).GetValue<Slider>().Value)
                    {
                        Q.Cast(pred.Position);
                    }
                }

                if (Menu.Item("UseEFarm", true).GetValue<bool>() && E.IsReady() &&
                    Player.CountEnemiesInRange(800) < 1)
                {
                    var eMinion =
                        minions.FirstOrDefault(
                            x => x.IsValidTarget(E.Range + Player.AttackRange) && x.Health < E.GetDamage(x));

                    if (eMinion != null)
                    {
                        E.Cast(eMinion.Position);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, eMinion);
                    }
                }
            }
        }

        private void JungleClear()
        {
            if (!ManaManager.HasMana("JungleClear"))
            {
                return;
            }

            var mobs = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (mobs.Any())
            {
                var mob = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));

                if (mob != null)
                {
                    if (Menu.Item("JungleClearW", true).GetValue<bool>() && W.IsReady() &&
                        mob.Distance(Player) <= W.Range)
                    {
                        W.Cast(mob.Position);
                    }

                    if (Menu.Item("JungleClearQ", true).GetValue<bool>() && Q.IsReady() &&
                        mob.Distance(Player) <= Q.Range)
                    {
                        Q.Cast(mob.Position);
                    }

                    if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady() &&
                        mob.Distance(Player) <= E.Range + Player.AttackRange)
                    {
                        E.Cast(mob.Position);
                    }
                }
            }
        }

        private void CheckKs()
        {
            foreach (
                var target in
                HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)).OrderByDescending(GetComboDamage))
            {
                if (Menu.Item("Auto_Q_Kill", true).GetValue<bool>() && Q.IsReady())
                {
                    if (Player.ServerPosition.Distance(target.ServerPosition) <= Q.Range &&
                        Q.GetDamage(target) > target.Health)
                    {
                        Q.Cast(target);
                        return;
                    }
                }

                if (Menu.Item("Auto_E_Kill", true).GetValue<bool>() && E.IsReady())
                {
                    if (Player.ServerPosition.Distance(target.ServerPosition) <= E.Range + 475 &&
                        E.GetDamage(target) > target.Health)
                    {
                        var vec = Player.ServerPosition.Extend(target.ServerPosition, E.Range - 10);

                        if (ShouldE(vec))
                        {
                            E.Cast(vec);
                            var target1 = target;
                            LeagueSharp.Common.Utility.DelayAction.Add((int)E.Delay * 1000 + Game.Ping,
                                () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target1));
                            return;
                        }
                    }
                }
            }
        }

        protected override void ObjAiHeroOnOnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!sender.IsMe || !R.IsReady() || args.Damage > 45)
            {
                return;
            }

            var safeNet = Menu.Item("R_Safe_Net2", true).GetValue<Slider>().Value;

            if (Player.HealthPercent <= safeNet)
            {
                R.Cast();
            }
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            AutoQ();
            CheckKs();

            if (Menu.Item("FarmT", true).GetValue<KeyBind>().Active)
                Harass();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Farm();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
                    break;
            }
        }

        protected override void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy)
            {
                return;
            }

            if (sender.IsValid)
            {
                if (sender.IsAlly && sender.Name.ToLower() == "ekko")
                {
                    rMissile = sender;
                }
            }
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("Draw_Disabled", true).GetValue<bool>())
                return;

            if (Menu.Item("Draw_Q", true).GetValue<bool>())
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_W", true).GetValue<bool>())
                if (W.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_E", true).GetValue<bool>())
                if (E.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);
            
            if (Menu.Item("Draw_R", true).GetValue<bool>())
                if (R.Level > 0 && rMissile != null && rMissile.IsValid)
                    Render.Circle.DrawCircle(rMissile.Position, R.Width,
                        R.IsReady() ? Color.FromArgb(29, 238, 64) : Color.RoyalBlue);

            if (R.IsReady() && rMissile != null)
            {
                var wts = Drawing.WorldToScreen(Player.Position);
               Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "Enemies Hit with R: " + TargetHitWithR());
            }
        }
    }
}
