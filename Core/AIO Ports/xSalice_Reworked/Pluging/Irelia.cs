using EloBuddy; 
using LeagueSharp.Common; 
namespace xSaliceResurrected_Rework.Pluging
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Base;
    using Managers;
    using Utilities;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;

    internal class Irelia : Champion
    {
        private int _lastNotification;

        public Irelia()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 650);
            SpellManager.W = new Spell(SpellSlot.W);
            SpellManager.E = new Spell(SpellSlot.E, 425);
            SpellManager.R = new Spell(SpellSlot.R, 1000);

            SpellManager.R.SetSkillshot(0, 80f, 1400f, false, SkillshotType.SkillshotLine);

            var spellMenu = new Menu("SpellMenu", "SpellMenu");
            {
                var qMenu = new Menu("QMenu", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Q_Min_Distance", "Min range to Q", true).SetValue(new Slider(300, 0, 600)));
                    qMenu.AddItem(new MenuItem("Q_Gap_Close", "Q Minion to Gap Close", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Q_Under_Tower", "Q Enemy Under Tower", true).SetValue(false));
                    spellMenu.AddSubMenu(qMenu);
                }

                var eMenu = new Menu("EMenu", "EMenu");
                {
                    eMenu.AddItem(new MenuItem("E_Only_Stun", "Save E to Stun", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("E_Running", "E On Running Enemy", true).SetValue(true));
                    spellMenu.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("RMenu", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("R_If_HP", "R If HP <=", true).SetValue(new Slider(20)));
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
                ManaManager.AddManaManagertoMenu(harass, "Harass", 30);
                Menu.AddSubMenu(harass);
            }

            var LastHit = new Menu("Lasthit", "Lasthit");
            {
                LastHit.AddItem(new MenuItem("UseQLastHit", "Use Q", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(LastHit, "Lasthit", 30);
                Menu.AddSubMenu(LastHit);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("UseQFarm_Tower", "Do not Q under Tower", true).SetValue(true));
                farm.AddItem(new MenuItem("UseWFarm", "Use W", true).SetValue(true));
                farm.AddItem(new MenuItem("UseRFarm", "Use R", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 0);
                Menu.AddSubMenu(farm);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddItem(new MenuItem("E_Gap_Closer", "Use E On Gap Closer", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("QE_Interrupt", "Use Q/E to interrupt", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
                Menu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R_Killable", "Draw R Mark on Killable", true).SetValue(true));

                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                var drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawMenu.AddItem(drawComboDamageMenu);
                drawMenu.AddItem(drawFill);
                //DamageIndicator.DamageToUnit = GetComboDamage;
                //DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                //DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                //DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        //DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        //DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        //DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
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
                customMenu.AddItem(myCust.AddToMenu("Laneclear Active: ", "LaneClear"));
                customMenu.AddItem(myCust.AddToMenu("LastHit Active: ", "LastHit"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (W.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.W) * 4;

            if (E.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R) * 4;

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 4);
        }

        private float GetComboDmgPercent(AIHeroClient target)
        {
            double comboDamage = GetComboDamage(target);

            var predHp = target.Health - comboDamage;
            var predHpPercent = predHp / target.MaxHealth * 100;

            return (float)predHpPercent;
        }

        private void Combo()
        {
            if (Menu.Item("UseQCombo", true).GetValue<bool>())
                Cast_Q();

            if (Menu.Item("UseWCombo", true).GetValue<bool>())
                Cast_W();

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
                Cast_Q();

            if (Menu.Item("UseWHarass", true).GetValue<bool>())
                Cast_W();

            if (Menu.Item("UseEHarass", true).GetValue<bool>())
                Cast_E();
        }

        private void Lasthit()
        {
            if (Menu.Item("UseQLastHit", true).GetValue<bool>() && ManaManager.HasMana("Lasthit"))
                Cast_Q_Last_Hit();
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 250,
                MinionTypes.All, MinionTeam.NotAlly);
            var allMinionR = MinionManager.GetMinions(Player.ServerPosition, R.Range, MinionTypes.All,
                        MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();
            var useR = Menu.Item("UseRFarm", true).GetValue<bool>();

            if (useQ)
                Cast_Q_Last_Hit();

            if (useW && allMinionsW.Count > 0 && W.IsReady())
                W.Cast();

            var rPred = R.GetLineFarmLocation(allMinionR);
            if (useR && rPred.MinionsHit > 0 && R.IsReady())
                R.Cast(rPred.Position);
        }

        private void Cast_Q()
        {
            var target = TargetSelector.GetTarget(Q.Range * 2, TargetSelector.DamageType.Physical);

            if (Q.IsReady() && target != null)
            {
                if (Q.IsKillable(target))
                    Q.Cast(target);

                if (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.E) > target.Health)
                    Q.Cast(target);

                var minDistance = Menu.Item("Q_Min_Distance", true).GetValue<Slider>().Value;

                if (!Menu.Item("Q_Under_Tower", true).GetValue<bool>())
                    if (target.UnderTurret(true))
                        return;

                if (Player.Distance(target.Position, true) > Q.RangeSqr / 2 && Menu.Item("Q_Gap_Close", true).GetValue<bool>())
                {
                    var allMinionQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                    var bestMinion = allMinionQ[0];

                    foreach (var minion in allMinionQ)
                    {
                        double dmg = 0;

                        dmg += Player.GetSpellDamage(minion, SpellSlot.Q);

                        if (W.IsReady() || Player.HasBuff("ireliahitenstylecharged"))
                            dmg += Player.GetSpellDamage(minion, SpellSlot.W);

                        if (target.Distance(minion.Position) < Q.Range && Player.Distance(minion.Position) < Q.Range &&
                            target.Distance(minion.Position) < target.Distance(Player.Position) && dmg > minion.Health + 40)
                            if (target.Distance(minion.Position) < target.Distance(bestMinion.Position))
                                bestMinion = minion;
                    }

                    if (bestMinion != null)
                    {
                        if (target.Distance(bestMinion.Position, true) < Q.RangeSqr && Player.Distance(bestMinion.Position, true) < Q.RangeSqr)
                        {
                            var dmg2 = Player.GetSpellDamage(bestMinion, SpellSlot.Q);

                            if (dmg2 > bestMinion.Health + 40)
                            {
                                Q.Cast(bestMinion);
                                return;
                            }

                            if (W.IsReady() || Player.HasBuff("ireliahitenstylecharged"))
                                dmg2 += Player.GetSpellDamage(bestMinion, SpellSlot.W);

                            if (dmg2 > bestMinion.Health)
                            {
                                W.Cast();
                                Q.Cast(bestMinion);
                                return;
                            }
                        }
                    }
                }

                if (Player.Distance(target.Position) > minDistance && Player.Distance(target.Position, true) < Q.RangeSqr)
                {
                    Q.Cast(target);
                }
            }
        }

        private void Cast_Q_Last_Hit()
        {
            var allMinionQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Player.BoundingRadius, MinionTypes.All, MinionTeam.NotAlly);

            if (allMinionQ.Count > 0 && Q.IsReady())
            {
                foreach (var minion in allMinionQ)
                {
                    var dmg = Player.GetSpellDamage(minion, SpellSlot.Q);

                    if (Player.HasBuff("ireliahitenstylecharged"))
                        dmg += Player.GetSpellDamage(minion, SpellSlot.W);

                    if (dmg > minion.Health + 35)
                    {
                        if (Menu.Item("UseQFarm_Tower", true).GetValue<bool>())
                        {
                            if (!minion.UnderTurret(true))
                            {
                                Q.Cast(minion);
                                return;
                            }
                        }
                        else
                            Q.Cast(minion);
                    }
                }
            }
        }

        private void Cast_W()
        {
            var target = TargetSelector.GetTarget(200, TargetSelector.DamageType.Physical);

            if (target != null && W.IsReady())
            {
                W.Cast();
            }
        }

        private void Cast_E()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (target != null && E.IsReady())
            {
                if (E.IsKillable(target))
                    E.Cast(target);

                if (Menu.Item("E_Only_Stun", true).GetValue<bool>())
                {
                    var targetHealthPercent = target.Health / target.MaxHealth * 100;

                    if (Player.HealthPercent < targetHealthPercent)
                    {
                        E.Cast(target);
                        return;
                    }
                }

                if (Menu.Item("E_Running", true).GetValue<bool>())
                {
                    var pred = Prediction.GetPrediction(target, 1f);

                    if (Player.Distance(target.Position) < Player.Distance(pred.UnitPosition) && Player.Distance(target.Position) > 200)
                        E.Cast(target);
                }
            }
        }

        private void Cast_R()
        {
            var target = TargetSelector.GetTarget(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).ToggleState == 1 ? Q.Range : R.Range,
                TargetSelector.DamageType.Physical);

            if (target != null && R.IsReady())
            {
                if (!Player.HasBuff("IreliaTranscendentBlades"))
                {
                    if (GetComboDmgPercent(target) < 25)
                        R.Cast(target);

                    var rHpValue = Menu.Item("R_If_HP", true).GetValue<Slider>().Value;
                    if (Player.HealthPercent <= rHpValue)
                        R.Cast(target);
                }
                else if (Player.HasBuff("IreliaTranscendentBlades"))
                {
                    R.Cast(target);
                }
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("E_Gap_Closer", true).GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender);
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.Medium || unit.IsAlly)
                return;

            if (Menu.Item("QE_Interrupt", true).GetValue<bool>())
            {
                var enemyHp = unit.Health / unit.MaxHealth * 100;
                if (Player.HealthPercent > enemyHp)
                    return;

                if (unit.IsValidTarget(E.Range))
                    E.Cast(unit);

                if (unit.IsValidTarget(Q.Range))
                {
                    Q.Cast(unit);
                    E.Cast(unit);
                }
            }
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

            if (Menu.Item("smartKS", true).GetValue<bool>())
                CheckKs();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Lasthit();
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

            if (Menu.Item("Draw_E", true).GetValue<bool>())
                if (E.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R", true).GetValue<bool>())
                if (R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R_Killable", true).GetValue<bool>())
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(5000) && !x.IsDead && x.IsEnemy).OrderBy(x => x.Health))
                {
                    var wts = Drawing.WorldToScreen(target.Position);
                    if (GetComboDmgPercent(target) < 30 && R.IsReady())
                    {
                        if (Utils.TickCount - _lastNotification > 0)
                        {
                            Notifications.AddNotification(target.BaseSkinName + " Is Killable!", 500);
                            _lastNotification = Utils.TickCount + 5000;
                        }
                    }

                    var enemyhp = target.Health / target.MaxHealth * 100;
                    if (Player.HealthPercent < enemyhp && E.IsReady())
                        Drawing.DrawText(wts[0] - 20, wts[1] - 30, Color.White, "Stunnable");
                }
            }
        }
    }
}
