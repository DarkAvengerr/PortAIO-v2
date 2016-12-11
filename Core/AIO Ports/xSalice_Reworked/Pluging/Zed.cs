using EloBuddy; 
using LeagueSharp.Common; 
namespace xSaliceResurrected_Rework.Pluging
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Base;
    using Managers;
    using Utilities;
    using Orbwalking = Orbwalking;
    using Color = System.Drawing.Color;

    internal class Zed : Champion
    {
        private int coaxDelay;
        private float qCooldown;
        private bool eCanHit;
        private Vector3 _predWq;
        private Vector3 wShadowPos;
        private Vector3 rShadowPos;

        public Zed()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 925f);
            SpellManager.W = new Spell(SpellSlot.W, 700f);
            SpellManager.E = new Spell(SpellSlot.E, 290f);
            SpellManager.R = new Spell(SpellSlot.R, 625f);

            SpellManager.Q.SetSkillshot(0.25f, 50f, 1700f, false, SkillshotType.SkillshotLine);
            SpellManager.W.SetSkillshot(0.25f, 0f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.E.SetSkillshot(0f, 220f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var combo = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("Prioritize_Q", "Prioritize Q over W->Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWFollowCombo", "Use W Follow", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(
                    new MenuItem("UseRCombo", "Use R", true).SetValue(new KeyBind('T', KeyBindType.Toggle, true)));
                combo.AddItem(
                    new MenuItem("R_Place_line", "R Range behind target in Line", true).SetValue(
                        new Slider(400, 250, 550)));
                combo.AddItem(
                    new MenuItem("Combo_mode", "Combo Mode", true).SetValue(
                        new StringList(new[] {"Normal", "Line Combo", "Coax"})));
            }

            var harass = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(new MenuItem("W_Require_QE", "Require both Q/E to hit on W Harass", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 30);
            }

            var farm = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "Use E", true).SetValue(true));
                farm.AddItem(
                    new MenuItem("LaneClear_useE_minHit", "Use E if min. hit", true).SetValue(new Slider(3, 1, 6)));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 30);
            }

            var jungle = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                jungle.AddItem(new MenuItem("UseQJungle", "Use Q", true).SetValue(true));
                jungle.AddItem(new MenuItem("UseEJungle", "Use E", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(jungle, "JungleClear", 30);
            }

            var lasthit = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                lasthit.AddItem(new MenuItem("UseQLastHit", "Use Q", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(lasthit, "LastHit", 30);
            }

            var flee = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                flee.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
            }

            var killSteal = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killSteal.AddItem(new MenuItem("smartKS", "Use Smart KS System", true).SetValue(true));
                killSteal.AddItem(new MenuItem("Use_W_KS", "Use W for KS", true).SetValue(true));
            }

            var misc = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var autoUse = misc.AddSubMenu(new Menu("Auto Use", "Auto Use"));
                {
                    autoUse.AddItem(
                        new MenuItem("AutoQ", "Auto Q Harass Enemy", true).SetValue(new KeyBind('G', KeyBindType.Press)));
                    autoUse.AddItem(
                        new MenuItem("AutoQMana", "Auto Q if Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                    autoUse.AddItem(
                        new MenuItem("AutoE", "Auto E Harass Enemy", true).SetValue(
                            new KeyBind('L', KeyBindType.Press, true)));
                    autoUse.AddItem(
                        new MenuItem("AutoEMana", "Auto E if Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var swapSettings = misc.AddSubMenu(new Menu("Swap Settings", "Swap Settings"));
                {
                    swapSettings.AddItem(new MenuItem("useWswap", "Use W swap", true).SetValue(true));
                    swapSettings.AddItem(
                        new MenuItem("useW_Health", "Use W swap if health below", true).SetValue(new Slider(25)));
                    swapSettings.AddItem(new MenuItem("useRswap", "Use R swap", true).SetValue(true));
                    swapSettings.AddItem(
                        new MenuItem("useR_Health", "Use R swap if health below", true).SetValue(new Slider(10)));
                    swapSettings.AddItem(new MenuItem("R_Back", "Use R Swap if Enemy Is dead", true).SetValue(true));
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "Draw W", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_WShadow", "Draw W Shadow", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_RShadow", "Draw R Shadow", true).SetValue(true));
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
            }

            var customMenu = Menu.AddSubMenu(new Menu("Custom Perma Show", "Custom Perma Show"));
            {
                var myCust = new CustomPermaMenu();
                customMenu.AddItem(
                    new MenuItem("custMenu", "Move Menu", true).SetValue(new KeyBind("L".ToCharArray()[0],
                        KeyBindType.Press)));
                customMenu.AddItem(new MenuItem("enableCustMenu", "Enabled", true).SetValue(true));
                customMenu.AddItem(myCust.AddToMenu("Combo Active: ", "Orbwalk"));
                customMenu.AddItem(myCust.AddToMenu("Harass Active: ", "Farm"));
                customMenu.AddItem(myCust.AddToMenu("Laneclear Active: ", "LaneClear"));
                customMenu.AddItem(myCust.AddToMenu("Flee Active: ", "Flee"));
                customMenu.AddItem(myCust.AddToMenu("Use R In Combo: ", "UseRCombo"));
                customMenu.AddItem(myCust.AddToMenu("Auto Q Harass: ", "AutoQ"));
                customMenu.AddItem(myCust.AddToMenu("Auto E Harass: ", "AutoE"));
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q)*2;

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E)*2;

            if (enemy.HealthPercent <= 50)
                damage += CalcPassive(enemy);

            if (enemy.HasBuff("zedulttargetmark"))
            {
                if (R.Level == 1)
                    damage += damage * 1.2;
                else if (R.Level == 2)
                    damage += damage * 1.35;
                else if (R.Level == 3)
                    damage += damage * 1.5;
            }

            if (R.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

                if (R.Level == 1)
                    damage += damage * 1.2;
                else if (R.Level == 2)
                    damage += damage * 1.35;
                else if (R.Level == 3)
                    damage += damage * 1.5;
            }

            damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }

        private double CalcPassive(Obj_AI_Base target)
        {
            double dmg = 0;

            if (Player.Level > 16)
            {
                double hp = target.MaxHealth * .1;
                dmg += Player.CalcDamage(target, Damage.DamageType.Magical, hp);
            }
            else if (Player.Level > 6)
            {
                double hp = target.MaxHealth * .08;
                dmg += Player.CalcDamage(target, Damage.DamageType.Magical, hp);
            }
            else
            {
                double hp = target.MaxHealth * .06;
                dmg += Player.CalcDamage(target, Damage.DamageType.Magical, hp);
            }

            return dmg;
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (!unit.IsMe)
            {
                return;
            }

            if (args.SData.Name == "ZedShuriken")
            {
                qCooldown = Game.Time + QSpell.Cooldown;
            }

            if (args.SData.Name == "ZedShadowDash")
            {
                if (W.LastCastAttemptT - Environment.TickCount > 0)
                {
                    if (_predWq != Vector3.Zero)
                    {
                        Q.Cast(_predWq, true);
                        Q.LastCastAttemptT = Environment.TickCount + 300;
                        _predWq = Vector3.Zero;
                    }

                    if (eCanHit)
                        E.Cast(true);
                }
            }

            if (args.SData.Name == "zedw2")
            {
                wShadowPos = Player.ServerPosition;
            }

            if (args.SData.Name == "zedult")
            {
                rShadowPos = Player.ServerPosition;
            }

            if (args.SData.Name == "ZedR2")
            {
                rShadowPos = Player.ServerPosition;
            }
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            AutoSwap();
            KillSteal();
            AutoUse();

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
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
                    break;
            }
        }

        private void AutoSwap()
        {
            if (RShadow != null)
            {
                if (Menu.Item("useRswap", true).GetValue<bool>() &&
                    Player.HealthPercent < Menu.Item("useR_Health", true).GetValue<Slider>().Value &&
                    RSpell.ToggleState == 2 && RShadow.ServerPosition.CountEnemiesInRange(400) < 1)
                {
                    R.Cast();
                    return;
                }
            }

            if (WShadow != null)
            {
                if (Menu.Item("useWswap", true).GetValue<bool>() &&
                    Player.HealthPercent < Menu.Item("useW_Health", true).GetValue<Slider>().Value &&
                    WSpell.ToggleState == 2 &&
                    WShadow.ServerPosition.CountEnemiesInRange(400) < 1)
                {
                    W.Cast();
                }
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("smartKS", true).GetValue<bool>())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.IsValidTarget(W.Range + Q.Range) && !x.IsDead &&
                            !x.HasBuffOfType(BuffType.Invulnerability)))
                {
                    //WQE
                    if (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.E) >
                        target.Health + 20 && W.IsReady() && Q.IsReady() && E.IsReady())
                    {
                        if (Menu.Item("Use_W_KS", true).GetValue<bool>())
                        {
                            Cast_W(true, true);
                        }
                        else
                        {
                            Cast_Q(target);
                            Cast_E(target);
                        }
                    }

                    //WQ
                    if (Q.IsKillable(target) && Player.Distance(target.Position) > Q.Range && Q.IsReady() && W.IsReady())
                    {
                        if (Menu.Item("Use_W_KS", true).GetValue<bool>())
                        {
                            Cast_W(true, true);
                        }
                        else
                        {
                            Cast_Q(target);
                        }
                    }

                    //WE
                    if (E.IsKillable(target) && Player.Distance(target.Position) > E.Range && E.IsReady() && W.IsReady())
                    {
                        if (Menu.Item("Use_W_KS", true).GetValue<bool>())
                        {
                            Cast_W(true, true);
                        }
                        else
                        {
                            Cast_E(target);
                        }
                    }

                    //Q
                    if (Q.IsKillable(target) && Player.Distance(target.Position) < Q.Range && Q.IsReady())
                    {
                        Cast_Q(target);
                    }

                    //E
                    if (E.IsKillable(target) && Player.Distance(target.Position) < E.Range && E.IsReady())
                    {
                        Cast_E(target);
                    }
                }
            }
        }

        private void AutoUse()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                return;
            }

            if (Menu.Item("AutoQ", true).GetValue<KeyBind>().Active &&
                Player.ManaPercent >= Menu.Item("AutoQMana", true).GetValue<Slider>().Value)
            {
                Cast_Q();
            }

            if (Menu.Item("AutoE", true).GetValue<KeyBind>().Active &&
                Player.ManaPercent >= Menu.Item("AutoEMana", true).GetValue<Slider>().Value)
            {
                Cast_E();
            }
        }

        private void Combo()
        {
            if (Menu.Item("Combo_mode", true).GetValue<StringList>().SelectedIndex == 0)
            {
                NormalCombo();
            }

            if (Menu.Item("Combo_mode", true).GetValue<StringList>().SelectedIndex == 1)
            {
                LineCombo();
            }

            if (Menu.Item("Combo_mode", true).GetValue<StringList>().SelectedIndex == 2)
            {
                CoaxCombo();
            }
        }

        private void ItemsUse()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

                if (Marked != null)
                {
                    itemTarget = Marked;
                }

                if (itemTarget != null)
                {
                    var dmg = GetComboDamage(itemTarget);

                    ItemManager.Target = itemTarget;

                    if (dmg > itemTarget.Health - 50)
                    {
                        ItemManager.KillableTarget = true;
                    }

                    ItemManager.UseTargetted = true;
                }
            }
        }

        private void NormalCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range + W.Range, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget(Q.Range + W.Range))
            {
                ItemsUse();

                var dmg2 = GetComboDamage(target);

                if (GetTargetFocus(Q.Range) != null)
                {
                    target = GetTargetFocus(Q.Range);
                }

                if (Menu.Item("UseRCombo", true).GetValue<KeyBind>().Active && R.IsReady() && dmg2 > target.Health + 50 &&
                    target.IsValidTarget(R.Range + 450f) && HasEnergy(true, true, false))
                {
                    if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady()
                        && GetComboDamage(target) > target.Health + 50 && !target.IsValidTarget(R.Range)
                        && WSpell.ToggleState == 0)
                    {
                        W.Cast(Player.Position.Extend(target.Position, W.Range));
                        W.LastCastAttemptT = Environment.TickCount + 500;
                    }
                    else if (target.IsValidTarget(R.Range))
                    {
                        R.CastOnUnit(target, true);
                    }
                }

                if (Menu.Item("Prioritize_Q", true).GetValue<bool>())
                {
                    if (Menu.Item("UseQCombo", true).GetValue<bool>())
                    {
                        Cast_Q();
                    }

                    if (HasEnergy(false, W.IsReady() && Menu.Item("UseWCombo", true).GetValue<bool>(),
                        E.IsReady() && Menu.Item("UseECombo", true).GetValue<bool>()))
                    {
                        if (Menu.Item("UseWCombo", true).GetValue<bool>())
                        {
                            Cast_W(false, Menu.Item("UseECombo", true).GetValue<bool>());
                        }
                    }
                }
                else
                {
                    if (HasEnergy(Q.IsReady() && Menu.Item("UseQCombo", true).GetValue<bool>(),
                        W.IsReady() && Menu.Item("UseWCombo", true).GetValue<bool>(),
                        E.IsReady() && Menu.Item("UseECombo", true).GetValue<bool>()))
                    {
                        if (Menu.Item("UseWCombo", true).GetValue<bool>())
                        {
                            Cast_W(Menu.Item("UseQCombo", true).GetValue<bool>(), 
                                Menu.Item("UseECombo", true).GetValue<bool>());
                        }
                    }
                    if (Menu.Item("UseQCombo", true).GetValue<bool>() && (!W.IsReady() || WSpell.ToggleState == 2))
                    {
                        Cast_Q();
                    }
                }

                if (Menu.Item("UseECombo", true).GetValue<bool>())
                {
                    Cast_E();
                }

                if (WShadow != null && Menu.Item("UseWFollowCombo", true).GetValue<bool>() && WSpell.ToggleState == 2 &&
                    Player.Distance(target.Position) > WShadow.Distance(target.Position) &&
                    (target.HasBuff("zedulttargetmark") || target.Health + 20 < GetComboDamage(target)))
                {
                    W.Cast(true);
                }
            }
        }

        private void LineCombo()
        {
            var target = TargetSelector.GetTarget(450f + R.Range, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget())
            {
                if (GetTargetFocus(450f + R.Range) != null)
                {
                    target = GetTargetFocus(450f + R.Range);
                }

                if (Marked != null)
                {
                    target = Marked;
                }

                ItemsUse();

                if (Menu.Item("UseRCombo", true).GetValue<KeyBind>().Active && R.IsReady())
                {
                    if (HasEnergy(Q.IsReady() && Menu.Item("UseQCombo", true).GetValue<bool>(), W.IsReady(),
                        E.IsReady() && Menu.Item("UseECombo", true).GetValue<bool>()))
                    {
                        var pred = Prediction.GetPrediction(target, 250f);

                        if (Environment.TickCount - R.LastCastAttemptT > Game.Ping && RSpell.ToggleState == 0 &&
                            W.IsReady())
                        {
                            R.Cast(target, true);
                            R.LastCastAttemptT = Environment.TickCount + 300;
                            return;
                        }

                        if (target.HasBuff("zedulttargetmark"))
                        {
                            if (WSpell.ToggleState == 0 && W.IsReady() && Environment.TickCount - R.LastCastAttemptT > 0 &&
                                Environment.TickCount - W.LastCastAttemptT > Game.Ping)
                            {
                                var dist = Menu.Item("R_Place_line", true).GetValue<Slider>().Value;
                                var behindVector = Player.ServerPosition -
                                                   Vector3.Normalize(target.ServerPosition - Player.ServerPosition)*dist;

                                if ((Menu.Item("UseECombo", true).GetValue<bool>() && pred.Hitchance >= HitChance.Medium) ||
                                    Q.GetPrediction(target).Hitchance >= HitChance.Medium)
                                {
                                    W.Cast(behindVector);
                                    W.LastCastAttemptT = Environment.TickCount + 300;

                                    _predWq = Menu.Item("UseQCombo", true).GetValue<bool>()
                                        ? Q.GetPrediction(target).UnitPosition
                                        : Vector3.Zero;

                                    eCanHit = Menu.Item("UseECombo", true).GetValue<bool>();
                                }
                            }
                        }
                    }
                }
                else
                {
                    var newtarget = TargetSelector.GetTarget(Q.Range + W.Range, TargetSelector.DamageType.Physical);

                    if (newtarget.IsValidTarget())
                    {
                        ItemsUse();

                        if (Menu.Item("Prioritize_Q", true).GetValue<bool>())
                        {
                            if (Menu.Item("UseQCombo", true).GetValue<bool>())
                            {
                                Cast_Q();
                            }

                            if (HasEnergy(false, W.IsReady() && Menu.Item("UseWCombo", true).GetValue<bool>(),
                                E.IsReady() && Menu.Item("UseECombo", true).GetValue<bool>()))
                            {
                                if (Menu.Item("UseWCombo", true).GetValue<bool>())
                                {
                                    Cast_W(false, Menu.Item("UseECombo", true).GetValue<bool>());
                                }
                            }
                        }
                        else
                        {
                            if (HasEnergy(Q.IsReady() && Menu.Item("UseQCombo", true).GetValue<bool>(),
                                W.IsReady() && Menu.Item("UseWCombo", true).GetValue<bool>(),
                                E.IsReady() && Menu.Item("UseECombo", true).GetValue<bool>()))
                            {
                                if (Menu.Item("UseWCombo", true).GetValue<bool>())
                                {
                                    Cast_W(Menu.Item("UseQCombo", true).GetValue<bool>(),
                                        Menu.Item("UseECombo", true).GetValue<bool>());
                                }
                            }
                            if (Menu.Item("UseQCombo", true).GetValue<bool>() && (!W.IsReady() || WSpell.ToggleState == 2))
                            {
                                Cast_Q();
                            }
                        }

                        if (Menu.Item("UseECombo", true).GetValue<bool>())
                        {
                            Cast_E();
                        }

                        if (WShadow != null && Menu.Item("UseWFollowCombo", true).GetValue<bool>() &&
                            WSpell.ToggleState == 2 &&
                            Player.Distance(newtarget.Position) > WShadow.Distance(newtarget.Position) &&
                            (newtarget.HasBuff("zedulttargetmark") || newtarget.Health + 20 < GetComboDamage(newtarget)))
                        {
                            W.Cast(true);
                        }
                    }
                }
            }
        }

        private void CoaxCombo()
        {
            var target = TargetSelector.GetTarget(W.Range + Q.Range, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget())
            {
                ItemsUse();

                if (Menu.Item("UseRCombo", true).GetValue<KeyBind>().Active && R.IsReady())
                {
                    if (GetTargetFocus(W.Range + Q.Range) != null)
                    {
                        target = GetTargetFocus(W.Range + Q.Range);
                    }

                    if (Marked != null)
                    {
                        target = Marked;
                    }

                    if (W.IsReady() && WSpell.ToggleState == 0)
                    {
                        Cast_W(Menu.Item("UseQCombo", true).GetValue<bool>(),
                            Menu.Item("UseECombo", true).GetValue<bool>(), true);
                        coaxDelay = Environment.TickCount + 500;
                        return;
                    }

                    if (WShadow != null && WShadow.Distance(target.Position) < R.Range - 100)
                    {
                        if ((Menu.Item("UseQCombo", true).GetValue<bool>() && qCooldown - Game.Time > QSpell.Cooldown/3) ||
                            (Menu.Item("UseECombo", true).GetValue<bool>() && !E.IsReady()))
                        {
                            return;
                        }

                        if (WShadow != null &&
                            HasEnergy(Q.IsReady() && Menu.Item("UseQCombo", true).GetValue<bool>(), false,
                                E.IsReady() && Menu.Item("UseECombo", true).GetValue<bool>()) &&
                            Environment.TickCount - coaxDelay > 0)
                        {
                            if (WSpell.ToggleState == 2 && WShadow.Distance(target.Position) < R.Range)
                            {
                                W.Cast(true);
                                LeagueSharp.Common.Utility.DelayAction.Add(50, () => R.Cast(target, true));
                            }
                        }
                    }
                }
                else
                {
                    if (Menu.Item("Prioritize_Q", true).GetValue<bool>())
                    {
                        if (Menu.Item("UseQCombo", true).GetValue<bool>())
                        {
                            Cast_Q();
                        }

                        if (HasEnergy(false, W.IsReady() && Menu.Item("UseWCombo", true).GetValue<bool>(),
                            E.IsReady() && Menu.Item("UseECombo", true).GetValue<bool>()))
                        {
                            if (Menu.Item("UseWCombo", true).GetValue<bool>())
                            {
                                Cast_W(false, Menu.Item("UseECombo", true).GetValue<bool>());
                            }
                        }
                    }
                    else
                    {
                        if (HasEnergy(Q.IsReady() && Menu.Item("UseQCombo", true).GetValue<bool>(),
                            W.IsReady() && Menu.Item("UseWCombo", true).GetValue<bool>(),
                            E.IsReady() && Menu.Item("UseECombo", true).GetValue<bool>()))
                        {
                            if (Menu.Item("UseWCombo", true).GetValue<bool>())
                            {
                                Cast_W(Menu.Item("UseQCombo", true).GetValue<bool>(),
                                    Menu.Item("UseECombo", true).GetValue<bool>());
                            }
                        }
                        if (Menu.Item("UseQCombo", true).GetValue<bool>() && (!W.IsReady() || WSpell.ToggleState == 2))
                        {
                            Cast_Q();
                        }
                    }

                    if (Menu.Item("UseECombo", true).GetValue<bool>())
                    {
                        Cast_E();
                    }

                    if (WShadow != null && Menu.Item("UseWFollowCombo", true).GetValue<bool>() &&
                        WSpell.ToggleState == 2 &&
                        Player.Distance(target.Position) > WShadow.Distance(target.Position) &&
                        (target.HasBuff("zedulttargetmark") || target.Health + 20 < GetComboDamage(target)))
                    {
                        W.Cast(true);
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

            var target = TargetSelector.GetTarget(Q.Range + W.Range, TargetSelector.DamageType.Physical);

            if (target.IsValidTarget(Q.Range + W.Range))
            {
                if (HasEnergy(Q.IsReady() && Menu.Item("UseQHarass", true).GetValue<bool>(),
                    W.IsReady() && Menu.Item("UseWHarass", true).GetValue<bool>(),
                    E.IsReady() && Menu.Item("UseEHarass", true).GetValue<bool>()))
                {
                    if (Menu.Item("UseWHarass", true).GetValue<bool>())
                    {
                        Cast_W(Menu.Item("UseQHarass", true).GetValue<bool>(),
                            Menu.Item("UseEHarass", true).GetValue<bool>(), false , true);

                        if (Menu.Item("UseQHarass", true).GetValue<bool>() && (!W.IsReady() || WSpell.ToggleState == 2))
                        {
                            Cast_Q();
                        }
                    }
                    else
                    {
                        if (Menu.Item("UseQHarass", true).GetValue<bool>())
                        {
                            Cast_Q();
                        }
                    }

                    if (Menu.Item("UseEHarass", true).GetValue<bool>())
                    {
                        Cast_E();
                    }
                }
            }
        }

        private void LaneClear()
        {
            if (!ManaManager.HasMana("LaneClear"))
            {
                return;
            }

            var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
            var allMinionsE = MinionManager.GetMinions(Player.ServerPosition, E.Range);

            if (Menu.Item("UseQFarm", true).GetValue<bool>() && Q.IsReady() && allMinionsQ.Any())
            {
                Q.UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                var pred = MinionManager.GetBestLineFarmLocation(allMinionsQ.Select(x => x.Position.To2D()).ToList(),
                    Q.Width, Q.Range);

                if (pred.MinionsHit > 2)
                {
                    Q.Cast(pred.Position);
                }
            }

            if (Menu.Item("UseEFarm", true).GetValue<bool>() && E.IsReady() && allMinionsE.Any())
            {
                E.UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                var pred = MinionManager.GetBestCircularFarmLocation(
                    allMinionsE.Select(x => x.Position.To2D()).ToList(), E.Width, E.Range);

                if (pred.MinionsHit >= Menu.Item("LaneClear_useE_minHit", true).GetValue<Slider>().Value)
                {
                    E.Cast();
                }
            }
        }

        private void JungleClear()
        {
            if (!ManaManager.HasMana("JungleClear"))
            {
                return;
            }

            var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var allMinionsE = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Menu.Item("UseQJungle", true).GetValue<bool>() && Q.IsReady() && allMinionsQ.Any())
            {
                Q.UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                var pred = MinionManager.GetBestLineFarmLocation(allMinionsQ.Select(x => x.Position.To2D()).ToList(),
                    Q.Width, Q.Range);

                if (pred.MinionsHit >= 1)
                {
                    Q.Cast(pred.Position);
                }
            }

            if (Menu.Item("UseEJungle", true).GetValue<bool>() && E.IsReady() && allMinionsE.Any())
            {
                E.UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                var pred = MinionManager.GetBestCircularFarmLocation(
                    allMinionsE.Select(x => x.Position.To2D()).ToList(), E.Width, E.Range);

                if (pred.MinionsHit >= 1)
                {
                    E.Cast();
                }
            }
        }

        private void LastHit()
        {
            if (!ManaManager.HasMana("LastHit"))
            {
                return;
            }

            var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

            if (Menu.Item("UseQLastHit", true).GetValue<bool>() && Q.IsReady() && allMinionsQ.Any())
            {
                var qMinion = allMinionsQ.FirstOrDefault(x => x.Health < Q.GetDamage(x));

                if (qMinion != null)
                {
                    Q.UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                    Q.Cast(qMinion);
                }
            }
        }

        private void Flee()
        {
            if (Menu.Item("FleeW", true).GetValue<bool>() && W.IsReady())
            {
                var pos = Player.Position.Extend(Game.CursorPos, W.Range);

                if (NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall) ||
                    NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Building))
                {
                    return;
                }

                W.Cast(pos);
            }
        }

        private void Cast_Q(AIHeroClient forceTarget = null)
        {
            var target = TargetSelector.GetTarget(Q.Range + W.Range, TargetSelector.DamageType.Physical);
            var qTarget = TargetSelector.GetTarget(Q.Range - 50, TargetSelector.DamageType.Physical);

            if (GetTargetFocus(W.Range + Q.Range) != null)
            {
                target = GetTargetFocus(W.Range + Q.Range);
            }

            if (Marked!= null)
            {
                target = Marked;
                qTarget = Marked;
            }

            if (forceTarget != null)
            {
                target = forceTarget;
                qTarget = forceTarget;
            }

            if (target != null && Q.IsReady())
            {
                if (qTarget != null)
                {
                    Q.UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                    Q.Cast(qTarget, true);
                    Q.LastCastAttemptT = Environment.TickCount + 300;
                    return;
                }

                if (WShadow != null && wShadowPos != Vector3.Zero)
                {
                    Q.UpdateSourcePosition(WShadow.ServerPosition, WShadow.ServerPosition);
                    Q.Cast(target, true);
                    return;
                }

                if (RShadow != null && rShadowPos != Vector3.Zero)
                {
                    Q.UpdateSourcePosition(RShadow.ServerPosition, RShadow.ServerPosition);
                    Q.Cast(target, true);
                }
            }
        }

        private void Cast_W(bool useQ, bool useE, bool isCoax = false, bool isHarass = false)
        {
            var target = TargetSelector.GetTarget(Q.Range + W.Range - 100, TargetSelector.DamageType.Physical);

            if (GetTargetFocus(Q.Range + W.Range - 100) != null)
            {
                target = GetTargetFocus(Q.Range + W.Range - 100);
            }

            if (target != null)
            {
                if (Marked != null)
                {
                    target = Marked;
                }

                if (E.Level < 1)
                {
                    useE = false;
                }

                if (Q.Level < 1)
                {
                    useQ = false;
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo 
                    && Menu.Item("UseRCombo", true).GetValue<KeyBind>().Active && R.IsReady()
                    && GetComboDamage(target) > target.Health + 50 && !target.IsValidTarget(R.Range)
                    && target.IsValidTarget(R.Range + 450f) && HasEnergy(true, true, false))
                    {
                    if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady()
                        && GetComboDamage(target) > target.Health + 50 && !target.IsValidTarget(R.Range)
                        && WSpell.ToggleState == 0)
                    {
                        W.Cast(Player.Position.Extend(target.Position, W.Range));
                        W.LastCastAttemptT = Environment.TickCount + 500;
                    }
                }

                if (WSpell.ToggleState == 0 && W.IsReady() && Environment.TickCount - W.LastCastAttemptT > 0)
                {
                    if (Player.Distance(target.Position) < W.Range + target.BoundingRadius)
                    {
                        if ((!useQ || Q.IsReady()) && (!useE || E.IsReady()) &&
                            Player.Distance(target.Position) < W.Range)
                        {
                            if (!IsPassWall(Player.ServerPosition, target.Position))
                            {
                                W.Cast(target);
                                W.LastCastAttemptT = Environment.TickCount + 500;

                                _predWq = useQ ? target.Position : Vector3.Zero;
                                eCanHit = useE;
                            }
                        }
                    }
                    else
                    {
                        var predE = Prediction.GetPrediction(target, .1f);
                        var vec = Player.ServerPosition +
                                  Vector3.Normalize(predE.CastPosition - Player.ServerPosition)*W.Range;

                        if (!IsPassWall(Player.ServerPosition, vec))
                        {
                            if ((!useQ || Q.IsReady()) && (!useE || E.IsReady()) && Player.Distance(vec) < W.Range)
                            {
                                if (useQ && useE)
                                {
                                    if ((Menu.Item("W_Require_QE", true).GetValue<bool>() && isHarass) || isCoax)
                                    {
                                        if (vec.Distance(target.ServerPosition) < E.Range)
                                        {
                                            W.Cast(vec);
                                            W.LastCastAttemptT = Environment.TickCount + 500;
                                        }
                                    }
                                    else
                                    {
                                        W.Cast(vec);
                                        W.LastCastAttemptT = Environment.TickCount + 500;
                                    }
                                }
                                else if (useE && vec.Distance(target.ServerPosition) < E.Range + target.BoundingRadius)
                                {
                                    W.Cast(vec);
                                    W.LastCastAttemptT = Environment.TickCount + 500;
                                }
                                else if (useQ)
                                {
                                    W.Cast(vec);
                                    W.LastCastAttemptT = Environment.TickCount + 500;
                                }

                                _predWq = useQ ? target.Position : Vector3.Zero;
                                eCanHit = useE && vec.Distance(target.ServerPosition) < E.Range;
                            }
                        }
                    }
                }
            }
        }

        private void Cast_E(AIHeroClient forceTarget = null)
        {
            var target = TargetSelector.GetTarget(E.Range + W.Range, TargetSelector.DamageType.Physical);

            if (GetTargetFocus(E.Range + W.Range) != null)
            {
                target = GetTargetFocus(E.Range + W.Range);
            }

            if (Marked!= null)
            {
                target = Marked;
            }

            if (forceTarget != null)
            {
                target = forceTarget;
            }

            if (target != null && E.IsReady())
            {
                if (WShadow != null && wShadowPos != Vector3.Zero)
                {
                    E.UpdateSourcePosition(WShadow.ServerPosition, WShadow.ServerPosition);
                    E.Cast(target, true);
                    return;
                }

                if (RShadow != null && rShadowPos != Vector3.Zero)
                {
                    E.UpdateSourcePosition(RShadow.ServerPosition, RShadow.ServerPosition);
                    E.Cast(target, true);
                    return;
                }

                E.UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                E.Cast(target, true);
                E.LastCastAttemptT = Environment.TickCount + 300;
            }
        }

        private bool HasEnergy(bool useQ, bool useW, bool useE)
        {
            var energy = Player.Mana;
            var totalEnergy = 0f;

            if (useQ)
            {
                totalEnergy += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
            }

            if (useW)
            {
                totalEnergy += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
            }

            if (useE)
            {
                totalEnergy += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
            }

            return energy >= totalEnergy;
        }

        private Obj_AI_Minion WShadow
        {
            get
            {
                if (wShadowPos == Vector3.Zero)
                {
                    return null;
                }

                return RShadow != null
                    ? ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            minion =>
                                minion.IsVisible && minion.IsAlly && minion.Name == "Shadow" && minion != RShadow &&
                                minion.ServerPosition != RShadow.ServerPosition)
                    : ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(minion => minion.IsVisible && minion.IsAlly && minion.Name == "Shadow");
            }
        }

        private Obj_AI_Minion RShadow
        {
            get
            {
                return rShadowPos == Vector3.Zero
                    ? null
                    : ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            minion =>
                                minion.IsVisible && minion.IsAlly && minion.Name == "Shadow" &&
                                minion.Distance(rShadowPos) < 200);
            }
        }

        private AIHeroClient Marked
        {
            get
            {
                return
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            x => x.IsValidTarget(W.Range + Q.Range) && x.HasBuff("zedulttargetmark") && x.IsVisible);
            }
        }

        private bool IsPassWall(Vector3 start, Vector3 end)
        {
            double count = Vector3.Distance(start, end);
            for (uint i = 0; i <= count; i += 25)
            {
                Vector2 pos = start.To2D().Extend(Player.ServerPosition.To2D(), -i);
                if (IsWall(pos))
                    return true;
            }
            return false;
        }

        private bool IsWall(Vector2 pos)
        {
            return NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Wall ||
                   NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Building;
        }

        private AIHeroClient GetTargetFocus(float range)
        {
            if (TargetSelector.GetSelectedTarget() != null)
            {
                if (TargetSelector.GetSelectedTarget().Distance(Player.ServerPosition) < range + 100 &&
                    TargetSelector.GetSelectedTarget().Type == GameObjectType.AIHeroClient)
                {
                    return TargetSelector.GetSelectedTarget();
                }
            }

            return null;
        }

        protected override void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmitter))
            {
                return;
            }

            if (sender.Name == "Zed_Base_W_cloneswap_buf.troy")
            {
                wShadowPos = sender.Position;
            }

            if (sender.Name == "Zed_Base_R_buf_tell.troy")
            {
                if (RSpell.ToggleState == 2 && RShadow != null && Menu.Item("R_Back", true).GetValue<bool>())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(500, () => R.Cast(true));
                }
            }
        }

        protected override void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmitter))
            {
                return;
            }

            if (sender.Name == "Zed_Clone_idle.troy" && wShadowPos != Vector3.Zero &&
                WShadow.Distance(sender.Position) < 100)
            {
                wShadowPos = Vector3.Zero;
            }


            if (RShadow != null && sender.Name == "Zed_Clone_idle.troy" && rShadowPos != Vector3.Zero &&
                RShadow.Distance(sender.Position) < 100)
            {
                rShadowPos = Vector3.Zero;
            }
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("Draw_Disabled", true).GetValue<bool>())
            {
                return;
            }

            if (Menu.Item("Draw_Q", true).GetValue<bool>() && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red, 3);
            }

            if (Menu.Item("Draw_W", true).GetValue<bool>() && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range - 2, W.IsReady() ? Color.Green : Color.Red, 3);
            }

            if (Menu.Item("Draw_E", true).GetValue<bool>() && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red, 3);
            }

            if (Menu.Item("Draw_R", true).GetValue<bool>() && R.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red, 3);
            }

            if (Menu.Item("Draw_WShadow", true).GetValue<bool>() && WShadow != null)
            {
                Render.Circle.DrawCircle(WShadow.Position, Player.BoundingRadius + 50, Color.Aqua, 3);
            }

            if (Menu.Item("Draw_RShadow", true).GetValue<bool>() && RShadow != null)
            {
                Render.Circle.DrawCircle(RShadow.Position, Player.BoundingRadius + 50, Color.Yellow, 3);
            }
        }
    }
}