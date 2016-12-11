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

    internal class Ezreal : Champion
    {
        public Ezreal()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 1200);
            SpellManager.W = new Spell(SpellSlot.W, 1050);
            SpellManager.E = new Spell(SpellSlot.E, 475);
            SpellManager.R = new Spell(SpellSlot.R, 3000);

            SpellManager.Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            SpellManager.W.SetSkillshot(0.25f, 80f, 2000f, false, SkillshotType.SkillshotLine);
            SpellManager.E.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotCircle);
            SpellManager.R.SetSkillshot(0.99f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            var spellMenu = new Menu("SpellMenu", "SpellMenu");
            {
                var qMenu = new Menu("QMenu", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Max_Range", "Q Max Range", true).SetValue(new Slider(1050, 500, 1200)));
                    qMenu.AddItem(new MenuItem("Auto_Q_Slow", "Auto W Slow", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Auto_Q_Immobile", "Auto W Immobile", true).SetValue(true));
                    spellMenu.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("WMenu", "WMenu");
                {
                    wMenu.AddItem(
                        new MenuItem("W_Max_Range", "W Max Range Sliders", true).SetValue(new Slider(900, 500, 1050)));
                    spellMenu.AddSubMenu(wMenu);
                }

                var eMenu = new Menu("EMenu", "EMenu");
                {
                    eMenu.AddItem(new MenuItem("E_On_Killable", "E if enemy Killable", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("E_On_Safe", "E Safety check", true).SetValue(true));
                    spellMenu.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("RMenu", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("R_Nearest_Killable", "R Nearest Killable", true).SetValue(new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
                    rMenu.AddItem(new MenuItem("Force_R", "Force R Lowest", true).SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Press)));
                    rMenu.AddItem(new MenuItem("R_Min_Range", "R Min Range Sliders", true).SetValue(new Slider(300, 0, 1000)));
                    rMenu.AddItem(new MenuItem("R_Max_Range", "R Max Range Sliders", true).SetValue(new Slider(2000, 0, 4000)));

                    rMenu.AddSubMenu(new Menu("Don't use R on", "Dont_R"));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                        rMenu.SubMenu("Dont_R")
                            .AddItem(new MenuItem("Dont_R" + enemy.BaseSkinName, enemy.BaseSkinName, true).SetValue(false));

                    spellMenu.AddSubMenu(rMenu);
                }

                Menu.AddSubMenu(spellMenu);
            }

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(true));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 30);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 30);
                Menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddItem(new MenuItem("Misc_Use_WE", "Cast WE to mouse", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                miscMenu.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(false, true, false, true));
                Menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "Draw W", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R_Killable", "Draw R Mark on Killable", true).SetValue(true));

                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                var drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
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
                customMenu.AddItem(new MenuItem("custMenu", "Move Menu", true).SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));
                customMenu.AddItem(new MenuItem("enableCustMenu", "Enabled", true).SetValue(true));
                customMenu.AddItem(myCust.AddToMenu("Combo Active: ", "Orbwalk"));
                customMenu.AddItem(myCust.AddToMenu("Harass Active: ", "Farm"));
                customMenu.AddItem(myCust.AddToMenu("Harass(T) Active: ", "FarmT"));
                customMenu.AddItem(myCust.AddToMenu("Laneclear Active: ", "LaneClear"));
                customMenu.AddItem(myCust.AddToMenu("R Nearest Kill: ", "R_Nearest_Killable"));
                customMenu.AddItem(myCust.AddToMenu("Force R: ", "Force_R"));
                Menu.AddSubMenu(customMenu);
            }
        }
        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (W.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.W);

            if (E.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R);

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 2);
        }

        private void Combo()
        {
            if (Menu.Item("UseQCombo", true).GetValue<bool>())
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Physical, HitChance.High);

            if (Menu.Item("UseWCombo", true).GetValue<bool>())
                SpellCastManager.CastBasicSkillShot(W, W.Range, TargetSelector.DamageType.Magical, HitChance.High);

            var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

            if (itemTarget != null)
            {
                var dmg = GetComboDamage(itemTarget);

                ItemManager.Target = itemTarget;

                if (dmg > itemTarget.Health - 50)
                    ItemManager.KillableTarget = true;

                ItemManager.UseTargetted = true;
            }

            if (Menu.Item("UseECombo", true).GetValue<bool>())
                Cast_E();

            if (Menu.Item("UseRCombo", true).GetValue<bool>())
                Cast_R();
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            if (Menu.Item("UseQHarass", true).GetValue<bool>())
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Physical, HitChance.High);

            if (Menu.Item("UseWHarass", true).GetValue<bool>())
                SpellCastManager.CastBasicSkillShot(W, W.Range, TargetSelector.DamageType.Magical, HitChance.High);
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0)
                Q.Cast(allMinionsQ[0]);
        }

        private void Cast_E()
        {
            var target = TargetSelector.GetTarget(E.Range + 500, TargetSelector.DamageType.Magical);

            if (E.IsReady() && target != null && Menu.Item("E_On_Killable", true).GetValue<bool>())
            {
                if (Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 25)
                {
                    if (Menu.Item("E_On_Safe", true).GetValue<bool>())
                    {
                        var ePos = E.GetPrediction(target);

                        if (ePos.CastPosition.CountEnemiesInRange(500) < 2)
                            E.Cast(ePos.UnitPosition);
                    }
                    else
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        private void Cast_R()
        {
            if (!R.IsReady())
                return;

            var minRange = Menu.Item("R_Min_Range", true).GetValue<Slider>().Value;

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)))
            {
                if (Menu.Item("Dont_R" + target.BaseSkinName, true) != null)
                {
                    if (!Menu.Item("Dont_R" + target.BaseSkinName, true).GetValue<bool>())
                    {
                        if (Get_R_Dmg(target) > target.Health && Player.Distance(target.Position) > minRange)
                        {
                            R.Cast(target);
                            return;
                        }
                    }
                }
            }
        }

        private void Cast_R_Killable()
        {
            foreach (var unit in HeroManager.Enemies.Where(x => x.IsValidTarget(20000) && !x.IsDead).OrderBy(x => x.Health))
            {
                if (Menu.Item("Dont_R" + unit.BaseSkinName, true) != null)
                {
                    if (!Menu.Item("Dont_R" + unit.BaseSkinName, true).GetValue<bool>())
                    {
                        var health = unit.Health + unit.HPRegenRate * 3 + 25;
                        if (Get_R_Dmg(unit) > health)
                        {
                            R.Cast(unit);
                            return;
                        }
                    }
                }
            }
        }

        private float Get_R_Dmg(AIHeroClient target)
        {
            double dmg = 0;

            dmg += Player.GetSpellDamage(target, SpellSlot.R);

            R.Range = 3000;
            var rPred = R.GetPrediction(target);
            var collisionCount = rPred.CollisionObjects.Count;

            if (collisionCount >= 7)
                dmg = dmg * .3;
            else if (collisionCount != 0)
                dmg = dmg * ((10 - collisionCount) / 10);

            return (float)dmg;
        }

        private void Cast_WE()
        {
            if (W.IsReady() && E.IsReady())
            {
                var vec = Player.ServerPosition + Vector3.Normalize(Game.CursorPos - Player.ServerPosition) * E.Range;

                W.Cast(vec);
                E.Cast(vec);
            }
        }

        private void AutoQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                if (Q.GetPrediction(target).Hitchance >= HitChance.High && 
                    (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare)) &&
                    Menu.Item("Auto_Q_Immobile", true).GetValue<bool>())
                    Q.Cast(target);

                if (target.HasBuffOfType(BuffType.Slow) && Menu.Item("Auto_Q_Slow", true).GetValue<bool>())
                    Q.Cast(target);
            }
        }

        private void ForceR()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (target != null && R.GetPrediction(target).Hitchance >= HitChance.High)
                R.Cast(target);
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            if (Q.IsReady())
                Q.Range = Menu.Item("Q_Max_Range", true).GetValue<Slider>().Value;
            if (W.IsReady())
                W.Range = Menu.Item("W_Max_Range", true).GetValue<Slider>().Value;
            if (R.IsReady())
                R.Range = Menu.Item("R_Max_Range", true).GetValue<Slider>().Value;

            AutoQ();

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
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Farm();
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("R_Nearest_Killable", true).GetValue<KeyBind>().Active)
                        Cast_R_Killable();
                    if (Menu.Item("Force_R", true).GetValue<KeyBind>().Active)
                        ForceR();
                    if (Menu.Item("Misc_Use_WE", true).GetValue<KeyBind>().Active)
                        Cast_WE();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
                if (R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R_Killable", true).GetValue<bool>() && R.IsReady())
            {
                foreach (var unit in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(20000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
                {
                    var health = unit.Health + unit.HPRegenRate * 3 + 25;
                    if (Get_R_Dmg(unit) > health)
                    {
                        var wts = Drawing.WorldToScreen(unit.Position);
                        Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "KILL!!!");
                    }
                }
            }
        }
    }
}
