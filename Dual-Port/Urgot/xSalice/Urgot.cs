namespace xSaliceResurrected_Rework.Pluging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Base;
    using Managers;
    using Utilities;
    using Orbwalking = LeagueSharp.Common.Orbwalking;
    using EloBuddy;
    using Champion = Base.Champion;
    using Utility = LeagueSharp.Common.Utility;
    internal class Urgot : Champion
    {
        private readonly List<AIHeroClient> _poisonTargets = new List<AIHeroClient>();

        public Urgot ()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 1000);
            SpellManager.Q2 = new Spell(SpellSlot.Q, 1300);
            SpellManager.W = new Spell(SpellSlot.W);
            SpellManager.E = new Spell(SpellSlot.E, 850);
            SpellManager.R = new Spell(SpellSlot.R, 550);

            SpellManager.Q.SetSkillshot(0.2667f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            SpellManager.Q2.SetSkillshot(0.3f, 60f, 1800f, false, SkillshotType.SkillshotLine);
            SpellManager.E.SetSkillshot(0.2658f, 120f, 1500f, false, SkillshotType.SkillshotCircle);


            var spellMenu = new Menu("SpellMenu", "SpellMenu");
            {
                var qMenu = new Menu("QMenu", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Poison", "Auto Q Poison Target", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("ForceE", "Require to use E first if Enemy is in E range", true).SetValue(false));
                    spellMenu.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("WMenu", "WMenu");
                {
                    wMenu.AddItem(new MenuItem("W_If_HP", "W If HP <= ", true).SetValue(new Slider(50)));
                    wMenu.AddItem(new MenuItem("W_Always", "Always W At start Of Combo", true).SetValue(false));
                    spellMenu.AddSubMenu(wMenu);
                }

                var rMenu = new Menu("RMenu", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("R_Safe_Net", "Do not ult into >= enemies after swap", true).SetValue(new Slider(2, 0, 5)));
                    rMenu.AddItem(new MenuItem("R_If_UnderTurret", "Ult Enemy If they are under ally Turret", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("R_On_Killable", "Ult Enemy If they are Killable", true).SetValue(true));
                    rMenu.AddSubMenu(new Menu("Don't use R on", "Dont_R"));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                        rMenu.SubMenu("Dont_R")
                            .AddItem(new MenuItem("Dont_R" + enemy.CharData.BaseSkinName, enemy.CharData.BaseSkinName, true).SetValue(false));

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
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 50);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "Use E", true).SetValue(false));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 50);
                Menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(false, false, true, false));
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
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q) * 2;

            if (E.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.E);

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 3);
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Physical);

            if (!target.IsValidTarget(Q2.Range))
                return;

            var dmg = GetComboDamage(target);

            ItemManager.Target = target;

            if (dmg > target.Health - 50)
                ItemManager.KillableTarget = true;

            ItemManager.UseTargetted = true;

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady())
                Cast_R();

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady())
                Cast_W(target);

            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady())
                SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Physical, HitChance.VeryHigh);

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
            {
                if (Player.ServerPosition.Distance(target.ServerPosition) > E.Range || !E.IsReady() || !Menu.Item("ForceE", true).GetValue<bool>())
                    Cast_Q(target);
            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            var target = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget(Q2.Range))
                return;

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady())
                Cast_W(target);

            if (Menu.Item("UseEHarass", true).GetValue<bool>() && E.IsReady())
                SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Physical, HitChance.VeryHigh);

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady())
            {
                if (Player.ServerPosition.Distance(target.ServerPosition) > E.Range || !E.IsReady() || !Menu.Item("ForceE", true).GetValue<bool>())
                    Cast_Q(target);
            }
        }

        protected override void ObjAiBaseOnOnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe || !(sender is AIHeroClient) || !sender.IsEnemy)
                return;

            if (args.Buff.Name == "urgotcorrosivedebuff")
            {
                _poisonTargets.Add((AIHeroClient)sender);
                Console.WriteLine("Added: " + _poisonTargets.Count);
            }
        }

        protected override void AfterAttack(AttackableUnit unit, AttackableUnit mytarget)
        {
            var target = (Obj_AI_Base)mytarget;

            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !unit.IsMe || !(target is AIHeroClient))
                return;

            if (Menu.Item("UseWCombo", true).GetValue<bool>())
                W.Cast();

            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady())
                E.Cast(target);

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
                Q.Cast(target);
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();

            var minion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (useQ && minion.Count > 0)
                Q.Cast(minion[0]);

            if (useE)
            {
                var allMinionECount = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
                var pred = E.GetCircularFarmLocation(allMinionECount);
                if (pred.MinionsHit > 1)
                    E.Cast(pred.Position);
            }
        }

        private void Cast_R()
        {
            switch (R.Instance.Level)
            {
                case 1:
                    R.Range = 550;
                    break;
                case 2:
                    R.Range = 700;
                    break;
                case 3:
                    R.Range = 850;
                    break;
            }

            var safeNet = Menu.Item("R_Safe_Net", true).GetValue<Slider>().Value;

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)).OrderByDescending(GetComboDamage))
            {
                if (Menu.Item("Dont_R" + target.CharData.BaseSkinName, true) != null)
                {
                    if (!Menu.Item("Dont_R" + target.CharData.BaseSkinName, true).GetValue<bool>())
                    {
                        if (!(target.CountEnemiesInRange(1000) >= safeNet))
                        {
                            if (Menu.Item("R_On_Killable", true).GetValue<bool>())
                            {
                                if (GetComboDamage(target) > target.Health && Player.Distance(target.Position) < R.Range)
                                {
                                    R.Cast(target);
                                    return;
                                }
                            }

                            if (Menu.Item("R_If_UnderTurret", true).GetValue<bool>())
                            {
                                if (Util.UnderAllyTurret() && Player.ServerPosition.Distance(target.ServerPosition) > 300f)
                                {
                                    R.Cast(target);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Cast_W(AIHeroClient target)
        {
            if (Menu.Item("W_Always", true).GetValue<bool>() && Player.ServerPosition.Distance(target.ServerPosition) < Q.Range)
                W.Cast();

            if (target.HasBuff("urgotcorrosivedebuff"))
                W.Cast();

            var hp = Menu.Item("W_If_HP", true).GetValue<Slider>().Value;

            if (Player.HealthPercent <= hp)
                W.Cast();
        }

        private void Cast_Q(AIHeroClient target)
        {
            if (target.HasBuff("urgotcorrosivedebuff"))
                Q2.Cast(target);
            else 
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Physical, HitChance.VeryHigh);
        }

        private void CheckKs()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q2.Range)).OrderByDescending(GetComboDamage))
            {
                if (Player.ServerPosition.Distance(target.ServerPosition) <= Q.Range && Player.GetSpellDamage(target, SpellSlot.Q) > target.Health && Q.IsReady())
                {
                    Q.Cast(target);
                    return;
                }

                if (Player.ServerPosition.Distance(target.ServerPosition) <= E.Range && Player.GetSpellDamage(target, SpellSlot.E) > target.Health && E.IsReady())
                {
                    E.Cast(target);
                    return;
                }
            }
        }


        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Player.IsChannelingImportantSpell())
                return;

            if (Menu.Item("Q_Poison", true).GetValue<bool>() && _poisonTargets.Count > 0)
            {
                var target = _poisonTargets.OrderByDescending(GetComboDamage).FirstOrDefault();

                if (target.IsValidTarget(Q2.Range))
                    Q2.Cast(target);
            }

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
                    Render.Circle.DrawCircle(Player.Position,R.Range, R.IsReady() ? Color.Green : Color.Red);
        }
    }
}
