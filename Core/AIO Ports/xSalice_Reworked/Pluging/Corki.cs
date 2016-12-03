using EloBuddy; 
using LeagueSharp.Common; 
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
    using Orbwalking = Orbwalking;

    internal class Corki : Champion
    {
        public Corki()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 825);
            SpellManager.Q.SetSkillshot(.3f, 250, 1225, false, SkillshotType.SkillshotCircle);

            SpellManager.W = new Spell(SpellSlot.W, 800);

            SpellManager.E = new Spell(SpellSlot.E, 600);
            SpellManager.E.SetSkillshot(.1f, 70, 1500, false, SkillshotType.SkillshotCircle);

            SpellManager.R = new Spell(SpellSlot.R, 1500);
            SpellManager.R.SetSkillshot(.2f, 50, 1500, true, SkillshotType.SkillshotLine);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                combo.AddItem(new MenuItem("Always_Use", "Spell after AA in AA range", true).SetValue(true));
                combo.AddItem(new MenuItem("ComboR_Limit", "Save R Stack", true).SetValue(new Slider(0, 0, 7)));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(false));
                harass.AddItem(new MenuItem("UseRHarass", "Use R", true).SetValue(true));
                harass.AddItem(new MenuItem("HarassR_Limit", "Save R Stack", true).SetValue(new Slider(5, 0, 7)));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 50);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "Use E", true).SetValue(false));
                farm.AddItem(new MenuItem("UseRFarm", "Use R", true).SetValue(true));
                farm.AddItem(new MenuItem("LaneClearR_Limit", "Save R Stack", true).SetValue(new Slider(5, 0, 7)));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 50);
                Menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(true, false, false, true));
                miscMenu.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
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
            var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

            if (itemTarget != null)
            {
                var dmg = GetComboDamage(itemTarget);
                ItemManager.Target = itemTarget;

                if (dmg > itemTarget.Health - 50)
                    ItemManager.KillableTarget = true;

                ItemManager.UseTargetted = true;
            }

            var target = TargetSelector.GetTarget(550, TargetSelector.DamageType.Magical);

            if (target != null && Menu.Item("Always_Use", true).GetValue<bool>())
                return;

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady())
                Cast_R(1);

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);

            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady())
                SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Physical, HitChance.Low);
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            var target = TargetSelector.GetTarget(550, TargetSelector.DamageType.Magical);

            if (Menu.Item("UseRHarass", true).GetValue<bool>() && R.IsReady())
                Cast_R(2);

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady())
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);

            if (Menu.Item("UseEHarass", true).GetValue<bool>() && E.IsReady())
                SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Physical, HitChance.Low);
        }

        protected override void AfterAttack(AttackableUnit unit, AttackableUnit mytarget)
        {
            var target = (Obj_AI_Base)mytarget;

            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !unit.IsMe || !(target is AIHeroClient))
                return;

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady())
                R.Cast(target);

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
                Q.Cast(target);

            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady())
                E.Cast();
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();
            var useR = Menu.Item("UseRFarm", true).GetValue<bool>();

            if (useQ)
                SpellCastManager.CastBasicFarm(Q);

            if (useR)
                Cast_R(3);

            if (useE)
            {
                var allMinionECount = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly).Count;

                if (allMinionECount > 1)
                    E.Cast();
            }
        }

        private void Cast_R(int Index)
        {
            var range = Player.HasBuff("CorkiMissileBarrageCounterBig") ? 1500 : 1300;

            if (Index == 1 && Menu.Item("ComboR_Limit", true).GetValue<Slider>().Value < ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                SpellCastManager.CastBasicSkillShot(R, range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
            else if (Index == 2 && Menu.Item("HarassR_Limit", true).GetValue<Slider>().Value < ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                SpellCastManager.CastBasicSkillShot(R, range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
            else if (Index == 3 && Menu.Item("LaneClearR_Limit", true).GetValue<Slider>().Value < ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                SpellCastManager.CastBasicFarm(R);
        }

        private void CheckKs()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)).OrderByDescending(GetComboDamage))
            {
                if (Player.ServerPosition.Distance(target.ServerPosition) <= Q.Range &&
                    Player.GetSpellDamage(target, SpellSlot.Q) > target.Health && Q.IsReady())
                {
                    Q.Cast(target);
                    return;
                }

                if (Player.ServerPosition.Distance(target.ServerPosition) <= R.Range &&
                    Player.GetSpellDamage(target, SpellSlot.R) > target.Health && R.IsReady())
                {
                    R.Cast(target);
                    return;
                }
            }
        }


        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

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
                    Render.Circle.DrawCircle(Player.Position, Player.HasBuff("CorkiMissileBarrageCounterBig") ? R.Range : 1300, R.IsReady() ? Color.Green : Color.Red);
        }
    }
}
