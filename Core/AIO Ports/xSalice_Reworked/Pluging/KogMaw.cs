using EloBuddy; 
using LeagueSharp.Common; 
namespace xSaliceResurrected_Rework.Pluging
{
    using System;
    using System.Drawing;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Base;
    using Managers;
    using Utilities;
    using Orbwalking = Orbwalking;

    internal class KogMaw : Champion
    {
        public KogMaw()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 1000);
            SpellManager.W = new Spell(SpellSlot.W);
            SpellManager.E = new Spell(SpellSlot.E, 1280);
            SpellManager.R = new Spell(SpellSlot.R, 1800);

            SpellManager.Q.SetSkillshot(.25f, 70, 1650, true, SkillshotType.SkillshotLine);
            SpellManager.E.SetSkillshot(.25f, 70, 1650, false, SkillshotType.SkillshotLine);
            SpellManager.R.SetSkillshot(.9f, 150, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                combo.AddItem(new MenuItem("NoMovementDuringW", "Dont Move During W").SetValue(true));
                combo.AddItem(new MenuItem("ComboR_Limit", "Limit R Stack", true).SetValue(new Slider(7, 0, 7)));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(false));
                harass.AddItem(new MenuItem("UseRHarass", "Use R", true).SetValue(true));
                harass.AddItem(new MenuItem("HarassR_Limit", "Limit R Stack", true).SetValue(new Slider(3, 0, 7)));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 50);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "Use E", true).SetValue(false));
                farm.AddItem(new MenuItem("UseRFarm", "Use R", true).SetValue(true));
                farm.AddItem(new MenuItem("LaneClearR_Limit", "Limit R Stack", true).SetValue(new Slider(2, 0, 7)));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 50);
                Menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(false, false, true, true));
                miscMenu.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("UseGap", "Use E for GapCloser", true).SetValue(true));
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

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady())
                Cast_R("Combo");

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);

            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady())
                SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Physical, HitChance.VeryHigh);

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady())
            {
                var target = TargetSelector.GetTarget(Player.AttackRange + new[] { 130, 150, 170, 190, 210 }[W.Level - 1], TargetSelector.DamageType.Magical);

                if (target.IsValidTarget(Player.AttackRange + new[] { 90, 120, 150, 180, 210 }[W.Level - 1]))
                    W.Cast();
            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            if (Menu.Item("UseRHarass", true).GetValue<bool>() && R.IsReady())
                Cast_R("Harass");

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady())
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);

            if (Menu.Item("UseEHarass", true).GetValue<bool>() && E.IsReady())
                SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Physical, HitChance.VeryHigh);

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady())
            {
                var target = TargetSelector.GetTarget(Player.AttackRange + new[] { 130, 150, 170, 190, 210 }[W.Level - 1], TargetSelector.DamageType.Magical);

                if (target.IsValidTarget(Player.AttackRange + new[] { 90, 120, 150, 180, 210 }[W.Level - 1]))
                    W.Cast();
            }
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();
            var useR = Menu.Item("UseRFarm", true).GetValue<bool>();

            if (useQ)
            {
                var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly);

                if (minion.Count > 0)
                    Q.Cast(minion[0]);
            }

            if (useR)
                Cast_R("Farm");

            if (useE)
            {
                SpellCastManager.CastBasicFarm(E);
            }
        }

        private void Cast_R(string mode)
        {
            if (mode == "Combo" && RStacks <= Menu.Item("ComboR_Limit", true).GetValue<Slider>().Value)
                SpellCastManager.CastBasicSkillShot(R, new[] { 1200f, 1500f, 1800f }[R.Level - 1], TargetSelector.DamageType.Magical, HitChance.VeryHigh);
            else if (mode == "Harass" && RStacks <= Menu.Item("HarassR_Limit", true).GetValue<Slider>().Value)
                SpellCastManager.CastBasicSkillShot(R, new[] { 1200f, 1500f, 1800f }[R.Level - 1], TargetSelector.DamageType.Magical, HitChance.VeryHigh);
            else if (mode == "Farm" && RStacks <= Menu.Item("LaneClearR_Limit", true).GetValue<Slider>().Value)
                SpellCastManager.CastBasicFarm(R);
        }

        private int RStacks => RSpell.Ammo / 40;

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

                if (Player.ServerPosition.Distance(target.ServerPosition) <= E.Range &&
                    Player.GetSpellDamage(target, SpellSlot.E) > target.Health && E.IsReady())
                {
                    E.Cast(target);
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

            Orbwalker.SetMovement(Menu.Item("NoMovementDuringW").GetValue<bool>() && 
                Player.HasBuff("KogMawBioArcaneBarrage") && HeroManager.Enemies
                .Count(h=> !h.IsDead && h.IsVisible && h.CanAttack &&
                h.Distance(Player) < h.AttackRange) == 0);

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

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("UseGap", true).GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender);
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
                    Render.Circle.DrawCircle(Player.Position, Player.AttackRange + new[] { 130, 150, 170, 190, 210 }[W.Level - 1], W.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_E", true).GetValue<bool>())
                if (E.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R", true).GetValue<bool>())
                if (R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, new[] { 1200f, 1500f, 1800f }[R.Level - 1], R.IsReady() ? Color.Green : Color.Red);
        }
    }
}
