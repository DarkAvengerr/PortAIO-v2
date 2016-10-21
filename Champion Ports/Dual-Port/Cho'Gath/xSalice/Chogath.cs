namespace xSaliceResurrected_Rework.Pluging
{
    using Base;
    using System;
    using System.Drawing;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Managers;
    using Utilities;
    using Orbwalking = LeagueSharp.Common.Orbwalking;
    using EloBuddy;
    using Champion = Base.Champion;
    internal class Chogath : Champion
    {
        private int _lastFlash;

        public Chogath()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 950);
            SpellManager.W = new Spell(SpellSlot.W, 650);
            SpellManager.R = new Spell(SpellSlot.R, 175);

            SpellManager.Q.SetSkillshot(.625f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.W.SetSkillshot(.25f, (float)(30 * 0.5), float.MaxValue, false, SkillshotType.SkillshotCone);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(true));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 60);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("UseWFarm", "Use W", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(farm, "Farm", 50);
                Menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(true, true, false, false));
                miscMenu.AddItem(new MenuItem("Q_Gap_Closer", "Use Q On Gap Closer", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("UseInt", "Use Q/E to Interrupt", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("R_KS", "Use R to KS", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("R_KS2", "Use Flash R to KS", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("flashR", "Flash R", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                Menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "Draw W", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R", true).SetValue(true));

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
                customMenu.AddItem(myCust.AddToMenu("Flash R Active: ", "flashR"));
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

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R);

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target));
        }

        private void Combo()
        {
            var itemTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (itemTarget != null)
            {
                var dmg = GetComboDamage(itemTarget);
                ItemManager.Target = itemTarget;

                if (dmg > itemTarget.Health - 50)
                    ItemManager.KillableTarget = true;

                ItemManager.UseTargetted = true;
            }

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (target.IsValidTarget(Q.Range))
                {
                    var pred = Prediction.GetPrediction(target, 625);

                    if (pred.Hitchance >= HitChance.VeryHigh && target.IsMoving)
                        Q.Cast(pred.UnitPosition);
                }
            }

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady())
                SpellCastManager.CastBasicSkillShot(W, W.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(GetRealRRange(x))))
                {
                    var dmg = GetComboDamage(enemy);
                    if (dmg > enemy.Health)
                        R.Cast(enemy);
                }
            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget(Q.Range))
                {
                    var pred = Prediction.GetPrediction(target, 625);

                    if (pred.Hitchance >= HitChance.VeryHigh && target.IsMoving)
                        Q.Cast(pred.UnitPosition);
                }
            }

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady())
                SpellCastManager.CastBasicSkillShot(W, W.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("Farm"))
                return;

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();

            if (useQ)
                SpellCastManager.CastBasicFarm(Q);

            if (useW && W.IsReady())
                SpellCastManager.CastBasicFarm(W);
        }

        private void CheckKs()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)).OrderByDescending(GetComboDamage))
            {
                if (Player.Distance(target.ServerPosition) <= W.Range && 
                    Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.W) > target.Health && 
                    Q.IsReady() && W.IsReady())
                {
                    Q.Cast(target);
                    W.Cast(target);
                    return;
                }

                if (Player.Distance(target.ServerPosition) <= Q.Range && 
                    Player.GetSpellDamage(target, SpellSlot.Q) > target.Health && Q.IsReady())
                {
                    Q.Cast(target);
                    return;
                }

                if (Player.Distance(target.ServerPosition) <= W.Range &&
                    Player.GetSpellDamage(target, SpellSlot.W) > target.Health && W.IsReady())
                {
                    W.Cast(target);
                    return;
                }

                if (Player.Distance(target.ServerPosition) <= GetRealRRange(target) && 
                    Player.GetSpellDamage(target, SpellSlot.R) > target.Health && R.IsReady() &&
                    Menu.Item("R_KS", true).GetValue<bool>())
                {
                    R.Cast(target);
                    return;
                }

                if (Player.Distance(target.ServerPosition) <= GetRealRRange(target) + 375 && 
                    Player.Distance(target.ServerPosition) > GetRealRRange(target) + 25
                    && R.IsReady() && SummonerManager.Flash_Ready() && Menu.Item("R_KS2", true).GetValue<bool>())
                {
                    if (Player.GetSpellDamage(target, SpellSlot.R) +
                        (SummonerManager.Ignite_Ready()
                            ? ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) - 20
                            : 0) > target.Health)
                    {
                        CastFlashR(target);
                        _lastFlash = Utils.TickCount;
                        return;
                    }
                }
            }
        }

        private float GetRealRRange(AIHeroClient target)
        {
            return R.Range + Player.BoundingRadius + target.BoundingRadius;
        }

        private void CastFlashR(AIHeroClient target)
        {
            if(SummonerManager.Flash_Ready())
                SummonerManager.UseFlash(target.ServerPosition);

            var dmg = GetComboDamage(target);

            ItemManager.Target = target;

            if (dmg > target.Health - 50)
                ItemManager.KillableTarget = true;

            ItemManager.UseTargetted = true;

            Orbwalking.Orbwalk(target, Game.CursorPos);

            LeagueSharp.Common.Utility.DelayAction.Add(25, () => R.Cast(target));

            if(R.IsReady())
                R.Cast(target);
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Menu.Item("smartKS", true).GetValue<bool>())
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
                    if (Menu.Item("flashR", true).GetValue<KeyBind>().Active || Utils.TickCount - _lastFlash < 2500)
                    {
                        Orbwalking.MoveTo(Game.CursorPos);
                        var target = TargetSelector.GetSelectedTarget();

                        if (target != null)
                            if (target.IsValidTarget(R.Range + 425 + target.BoundingRadius))
                                CastFlashR(TargetSelector.GetSelectedTarget());
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("Q_Gap_Closer", true).GetValue<bool>()) return;

            if (Q.IsReady() && gapcloser.Sender.Distance(Player.Position) < 500)
                Q.Cast(gapcloser.Sender);
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("UseInt", true).GetValue<bool>()) return;

            if (Player.Distance(unit.Position) < W.Range && W.IsReady())
            {
                W.Cast(unit);
                return;
            }

            if (Player.Distance(unit.Position) < Q.Range && Q.IsReady())
            {
                Q.Cast(unit);
            }
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("Draw_Disabled", true).GetValue<bool>())
                return;

            if (Menu.Item("Draw_Q", true).GetValue<bool>())
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.MediumBlue : Color.Red);

            if (Menu.Item("Draw_W", true).GetValue<bool>())
                if (W.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.MediumBlue : Color.Red);

            if (Menu.Item("Draw_R", true).GetValue<bool>())
                if (R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.MediumBlue : Color.Red);
        }
    }
}
