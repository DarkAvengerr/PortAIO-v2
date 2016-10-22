using EloBuddy; 
 using LeagueSharp.Common; 
 namespace xSaliceResurrected_Rework.Pluging
{
    using Base;
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Managers;
    using Utilities;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;

    internal class Ashe : Champion
    {
        public Ashe()
        {
            SpellManager.Q = new Spell(SpellSlot.Q);
            SpellManager.W = new Spell(SpellSlot.W, 1200);
            SpellManager.E = new Spell(SpellSlot.E);
            SpellManager.R = new Spell(SpellSlot.R, 20000);

            SpellManager.W.SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotCone);
            SpellManager.R.SetSkillshot(250f, 130f, 1600f, false, SkillshotType.SkillshotLine);

            var spellMenu = new Menu("SpellMenu", "SpellMenu");
            {
                var qMenu = new Menu("QMenu", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Min_Stack", "Require Q Min Stacks", true).SetValue(new Slider(5, 0, 5)));
                    spellMenu.AddSubMenu(qMenu);
                }

                var rMenu = new Menu("RMenu", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("Force_R", "Force R Lowest", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                    rMenu.AddItem(new MenuItem("R_Min_Range", "R Min Range Sliders", true).SetValue(new Slider(300, 0, 1000)));
                    rMenu.AddItem(new MenuItem("R_Max_Range", "R Max Range Sliders", true).SetValue(new Slider(2000, 0, 4000)));
                    rMenu.AddSubMenu(new Menu("Don't use R on", "Dont_R"));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                    {
                        rMenu.SubMenu("Dont_R")
                            .AddItem(new MenuItem("Dont_R" + enemy.CharData.BaseSkinName, enemy.CharData.BaseSkinName, true).SetValue(false));
                    }

                    spellMenu.AddSubMenu(rMenu);
                }

                Menu.AddSubMenu(spellMenu);
            }

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
                ManaManager.AddManaManagertoMenu(harass, "Harass", 30);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("UseWFarm", "Use W", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 30);
                Menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(false, true, false, true));
                miscMenu.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("ksR", "KS with R", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("UseInt", "Use R to Interrupt", true).SetValue(true));
                Menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
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

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R);

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 2);
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget(R.Range))
            {
                var dmg = GetComboDamage(target);

                if (Menu.Item("UseRCombo", true).GetValue<bool>() && dmg > target.Health && Player.ServerPosition.Distance(target.ServerPosition) > Menu.Item("R_Min_Range", true).GetValue<Slider>().Value)
                    SpellCastManager.CastBasicSkillShot(R, R.Range, TargetSelector.DamageType.Physical, HitChance.VeryHigh);

                if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady() && 
                    Player.ServerPosition.Distance(target.ServerPosition) < 550)
                {
                    var qMin = Menu.Item("Q_Min_Stack", true).GetValue<Slider>().Value;

                    if (qMin <= QStacks)
                        Q.Cast();
                }
            }

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady())
                SpellCastManager.CastBasicSkillShot(W, W.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);

            var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

            if (itemTarget != null)
            {
                var dmg = GetComboDamage(itemTarget);
                ItemManager.Target = itemTarget;

                if (dmg > itemTarget.Health - 50)
                    ItemManager.KillableTarget = true;

                ItemManager.UseTargetted = true;
            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            var target = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(Player), TargetSelector.DamageType.Physical);

            if (target.IsValidTarget())
            {
                if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() && 
                    Player.ServerPosition.Distance(target.ServerPosition) < 550)
                {
                    var qMin = Menu.Item("Q_Min_Stack", true).GetValue<Slider>().Value;

                    if (qMin <= QStacks)
                        Q.Cast();
                }
            }

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady())
                SpellCastManager.CastBasicSkillShot(W, W.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
        }

        protected override void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if ((Menu.Item("UseQCombo", true).GetValue<bool>() && Menu.Item("Orbwalk", true).GetValue<KeyBind>().Active) || 
                (Menu.Item("Farm", true).GetValue<KeyBind>().Active && Menu.Item("UseQHarass", true).GetValue<bool>()))
            {
                if (Q.IsReady())
                {
                    var qMin = Menu.Item("Q_Min_Stack", true).GetValue<Slider>().Value;

                    if (qMin <= QStacks)
                        Q.Cast();
                }
            }
        }

        private int QStacks => (from buff in Player.Buffs
            where buff.Name == "asheqcastready" || buff.Name == "AsheQ"
            select buff.Count).FirstOrDefault();

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0)
            {
                var qMin = Menu.Item("Q_Min_Stack", true).GetValue<Slider>().Value;

                if (qMin <= QStacks)
                    Q.Cast();
            }

            if(useW)
                SpellCastManager.CastBasicFarm(W);
        }

        private void ForceR()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (target != null)
                R.Cast(target);
        }

        private void CheckKs()
        {
            foreach (AIHeroClient target in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(R.Range))
                .OrderByDescending(GetComboDamage))
            {
                //W
                if (Player.ServerPosition.Distance(target.ServerPosition) <= W.Range &&
                    Player.GetSpellDamage(target, SpellSlot.W) > target.Health && W.IsReady())
                {
                    W.Cast(target);
                    return;
                }

                //R
                if (Player.ServerPosition.Distance(target.ServerPosition) <= R.Range &&
                    Player.GetSpellDamage(target, SpellSlot.R) > target.Health && R.IsReady() &&
                    Menu.Item("ksR", true).GetValue<bool>())
                {
                    R.Cast(target);
                    return;
                }
            }
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            if(R.IsReady())
                R.Range = Menu.Item("R_Max_Range", true).GetValue<Slider>().Value;

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
                    if (Menu.Item("Force_R", true).GetValue<KeyBind>().Active)
                    {
                        Orbwalking.MoveTo(Game.CursorPos);
                        ForceR();
                    }
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

            if (Menu.Item("Draw_W", true).GetValue<bool>())
                if (W.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R", true).GetValue<bool>())
                if (R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("UseInt", true).GetValue<bool>()) return;

            if (Player.Distance(unit.Position) < R.Range && spell.DangerLevel >= Interrupter2.DangerLevel.Medium)
            {
                if (R.GetPrediction(unit).Hitchance >= HitChance.Medium && R.IsReady())
                    R.Cast(unit);
            }
        }
    }
}
