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
    internal class Katarina : Champion
    {
        public Katarina()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 675);
            SpellManager.W = new Spell(SpellSlot.W, 375);
            SpellManager.E = new Spell(SpellSlot.E, 700);
            SpellManager.R = new Spell(SpellSlot.R, 550);

            SpellManager.Q.SetTargetted(400, 1400);
            SpellManager.R.SetCharged("KatarinaR", "KatarinaR", 550, 550, 1.0f);

            SpellManager.SpellList.Add(Q);
            SpellManager.SpellList.Add(W);
            SpellManager.SpellList.Add(E);
            SpellManager.SpellList.Add(R);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("eDis", "E only if >", true).SetValue(new Slider(0, 0, 700)));
                combo.AddItem(new MenuItem("smartE", "Smart E with R CD ", true).SetValue(false));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                combo.AddItem(new MenuItem("comboMode", "Mode", true).SetValue(new StringList(new[] { "QEW", "EQW" })));
                combo.AddItem(new MenuItem("disableaa", "Disable AA").SetValue(true));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(false));
                harass.AddItem(new MenuItem("harassMode", "Mode", true).SetValue(new StringList(new[] { "QEW", "EQW", "QW" }, 2)));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("Farm", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q Farm", true).SetValue(true));
                farm.AddItem(new MenuItem("UseWFarm", "Use W Farm", true).SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "Use E Farm", true).SetValue(false));
                farm.AddItem(new MenuItem("UseQHit", "Use Q Last Hit", true).SetValue(true));
                farm.AddItem(new MenuItem("UseWHit", "Use W Last Hit", true).SetValue(false));
                Menu.AddSubMenu(farm);
            }

            var killSteal = new Menu("KillSteal", "KillSteal");
            {
                killSteal.AddItem(new MenuItem("smartKS", "Use Smart KS System", true).SetValue(true));
                killSteal.AddItem(new MenuItem("wardKs", "Use Jump KS", true).SetValue(true));
                killSteal.AddItem(new MenuItem("rKS", "Use R for KS", true).SetValue(true));
                killSteal.AddItem(new MenuItem("rCancel", "NO R Cancel for KS", true).SetValue(true));
                killSteal.AddItem(new MenuItem("KS_With_E", "Don't KS with E Toggle!", true).SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle)));
                Menu.AddSubMenu(killSteal);
            }

            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("waitQ", "Wait For Q Mark to W", true).SetValue(true));
                misc.AddItem(new MenuItem("autoWz", "Auto W Enemy", true).SetValue(true));
                misc.AddItem(new MenuItem("E_Delay_Slider", "Delay Between E(ms)", true).SetValue(new Slider(0, 0, 1000)));
                Menu.AddSubMenu(misc);
            }

            var drawing = new Menu("Drawings", "Drawings");
            {
                drawing.AddItem(new MenuItem("QRange", "Q range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("WRange", "W range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("ERange", "E range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("RRange", "R range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("Draw_Mode", "Draw E Mode", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));

                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                var drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawing.AddItem(drawComboDamageMenu);
                drawing.AddItem(drawFill);
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

                Menu.AddSubMenu(drawing);
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
                customMenu.AddItem(myCust.AddToMenu("LastHit Active: ", "LastHit"));
                customMenu.AddItem(myCust.AddToMenu("WardJump Active: ", "Flee"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            damage += MarkDmg(enemy);

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady() || (RSpell.State == SpellState.Surpressed && R.Level > 0))
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 8;

            damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            var mode = Menu.Item("comboMode", true).GetValue<StringList>().SelectedIndex;

            var eDis = Menu.Item("eDis", true).GetValue<Slider>().Value;

            if (!target.IsValidTarget(E.Range))
                return;

            if (!target.HasBuffOfType(BuffType.Invulnerability) && !target.IsZombie)
            {
                if (mode == 0)
                {
                    {
                        var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

                        if (itemTarget != null && E.IsReady())
                        {
                            var dmg = GetComboDamage(itemTarget);

                            ItemManager.Target = itemTarget;

                            if (dmg > itemTarget.Health - 50)
                                ItemManager.KillableTarget = true;

                            ItemManager.UseTargetted = true;
                        }


                        if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady() &&
                            target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target);
                        }

                        if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady() &&
                            target.IsValidTarget(E.Range) &&
                            Utils.TickCount - E.LastCastAttemptT > 0 &&
                            Player.Distance(target.Position) > eDis && !Q.IsReady())
                        {
                            if (Menu.Item("smartE", true).GetValue<bool>() &&
                                Player.CountEnemiesInRange(500) > 2 &&
                                (!R.IsReady() || !(RSpell.State == SpellState.Surpressed && R.Level > 0)))
                                return;

                            var delay = Menu.Item("E_Delay_Slider", true).GetValue<Slider>().Value;

                            Orbwalker.SetAttack(false);
                            Orbwalker.SetMovement(false);
                            E.Cast(target, true);
                            E.LastCastAttemptT = Utils.TickCount + delay;
                        }
                    }
                }
                else if (mode == 1)
                {
                    {
                        var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

                        if (itemTarget != null && E.IsReady())
                        {
                            var dmg = GetComboDamage(itemTarget);

                            ItemManager.Target = itemTarget;

                            if (dmg > itemTarget.Health - 50)
                                ItemManager.KillableTarget = true;

                            ItemManager.UseTargetted = true;
                        }

                        if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady() &&
                            target.IsValidTarget(E.Range) &&
                            Utils.TickCount - E.LastCastAttemptT > 0 &&
                            Player.Distance(target.Position) > eDis)
                        {
                            if (Menu.Item("smartE", true).GetValue<bool>() &&
                                Player.CountEnemiesInRange(500) > 2 &&
                                (!R.IsReady() || !(RSpell.State == SpellState.Surpressed && R.Level > 0)))
                                return;

                            var delay = Menu.Item("E_Delay_Slider", true).GetValue<Slider>().Value;

                            Orbwalker.SetAttack(false);
                            Orbwalker.SetMovement(false);
                            E.Cast(target, true);
                            E.LastCastAttemptT = Utils.TickCount + delay;
                        }

                        if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady() &&
                            target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target, true);
                        }
                    }
                }

                if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady() &&
                    target.IsValidTarget(W.Range) && QSuccessfullyCasted())
                {
                    W.Cast();
                }

                if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady() &&
                    Player.CountEnemiesInRange(R.Range) > 0)
                {
                    if (!Q.IsReady() && !E.IsReady() && !W.IsReady())
                    {
                        Orbwalker.SetAttack(false);
                        Orbwalker.SetMovement(false);
                        R.Cast();
                    }
                }
            }
        }

        private void Harass()
        {
            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var mode = Menu.Item("harassMode", true).GetValue<StringList>().SelectedIndex;

            if (mode == 0)
            {
                if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() && 
                    qTarget != null && qTarget.IsValidTarget(Q.Range))
                {
                    Q.Cast(qTarget, true);
                }

                if (Menu.Item("UseEHarass", true).GetValue<bool>() && eTarget != null && E.IsReady() &&
                    !Q.IsReady() && eTarget.IsValidTarget(E.Range))
                {
                    E.Cast(eTarget, true);
                }
            }
            else if (mode == 1)
            {
                if (Menu.Item("UseEHarass", true).GetValue<bool>() && eTarget != null &&
                    E.IsReady() && eTarget.IsValidTarget(E.Range))
                {
                    E.Cast(eTarget, true);
                }

                if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() && qTarget != null &&
                    qTarget.IsValidTarget(Q.Range))
                {
                    Q.Cast(qTarget, true);
                }
            }
            else if (mode == 2)
            {
                if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() && qTarget != null &&
                    qTarget.IsValidTarget(Q.Range))
                {
                    Q.Cast(qTarget, true);
                }
            }

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && wTarget != null && W.IsReady() &&
                wTarget.IsValidTarget(W.Range) && QSuccessfullyCasted())
            {
                W.Cast();
            }
        }

        private void LastHit()
        {
            var allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQHit", true).GetValue<bool>();
            var useW = Menu.Item("UseWHit", true).GetValue<bool>();

            if (Q.IsReady() && useQ)
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget(Q.Range) &&
                        HealthPrediction.GetHealthPrediction(
                            minion, (int)(Player.Distance(minion.Position) * 1000 / 1400), 200) <
                        Player.GetSpellDamage(minion, SpellSlot.Q))
                    {
                        Q.Cast(minion);
                        return;
                    }
                }
            }

            if (W.IsReady() && useW)
            {
                if (allMinions.Where(minion => minion.IsValidTarget(W.Range) && 
                minion.Health < Player.GetSpellDamage(minion, SpellSlot.W) + MarkDmg(minion) - 35)
                .Any(minion => Player.Distance(minion.ServerPosition) < W.Range))
                {
                    W.Cast();
                }
            }
        }

        private double MarkDmg(Obj_AI_Base target)
        {
            return target.HasBuff("katarinaqmark") ? Player.GetSpellDamage(target, SpellSlot.Q, 1) : 0;
        }

        private void Farm()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0 && Q.IsReady() && allMinionsQ[0].IsValidTarget(Q.Range))
            {
                Q.Cast(allMinionsQ[0]);
            }

            if (useE && allMinionsQ.Count > 0 && E.IsReady() && allMinionsQ[0].IsValidTarget(E.Range))
            {
                E.Cast(allMinionsE[0]);
            }

            if (useW && W.IsReady())
            {
                if (allMinionsW.Count > 0 && QSuccessfullyCasted())
                {
                    foreach (var minion in allMinionsW)
                    {
                        if (!Q.IsReady() || minion.HasBuff("katarinaqmark"))
                            W.Cast();
                    }
                }
                    
            }
        }
        private void JungleFarm()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.Neutral);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.Neutral);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0 && Q.IsReady() && allMinionsQ[0].IsValidTarget(Q.Range))
            {
                Q.Cast(allMinionsQ[0]);
            }

            if (useW && W.IsReady())
            {
                if (allMinionsW.Count > 0)
                    W.Cast();
            }
        }

        private void SmartKs()
        {
            if (!Menu.Item("smartKS", true).GetValue<bool>())
                return;

            if (Menu.Item("rCancel", true).GetValue<bool>() && Player.CountEnemiesInRange(570) > 1)
                return;

            foreach (var target in HeroManager.Enemies.Where(
                x => x.IsValidTarget(1375) && !x.HasBuffOfType(BuffType.Invulnerability))
                .OrderByDescending(GetComboDamage))
            {
                if (target != null)
                {
                    var delay = Menu.Item("E_Delay_Slider", true).GetValue<Slider>().Value;
                    var shouldE = !Menu.Item("KS_With_E", true).GetValue<KeyBind>().Active && Utils.TickCount - E.LastCastAttemptT > 0;

                    if (Player.Distance(target.ServerPosition) <= E.Range && shouldE &&
                        Player.GetSpellDamage(target, SpellSlot.E) + Player.GetSpellDamage(target, SpellSlot.Q) + MarkDmg(target) +
                        Player.GetSpellDamage(target, SpellSlot.W) > target.Health + 20)
                    {
                        if (E.IsReady() && Q.IsReady() && W.IsReady())
                        {
                            CancelUlt(target);
                            Q.Cast(target);
                            E.Cast(target);
                            E.LastCastAttemptT = Utils.TickCount + delay;
                            if (Player.Distance(target.ServerPosition) < W.Range)
                                W.Cast();
                            return;
                        }
                    }

                    if (Player.Distance(target.ServerPosition) <= E.Range && shouldE &&
                        Player.GetSpellDamage(target, SpellSlot.E) + Player.GetSpellDamage(target, SpellSlot.W) >
                        target.Health + 20)
                    {
                        if (E.IsReady() && W.IsReady())
                        {
                            CancelUlt(target);
                            E.Cast(target);
                            E.LastCastAttemptT = Utils.TickCount + delay;
                            if (Player.Distance(target.ServerPosition) < W.Range)
                                W.Cast();
                            return;
                        }
                    }

                    if (Player.Distance(target.ServerPosition) <= E.Range && shouldE &&
                        Player.GetSpellDamage(target, SpellSlot.E) + Player.GetSpellDamage(target, SpellSlot.Q) >
                        target.Health + 20)
                    {
                        if (E.IsReady() && Q.IsReady())
                        {
                            CancelUlt(target);
                            E.Cast(target);
                            E.LastCastAttemptT = Utils.TickCount + delay;
                            Q.Cast(target);
                            return;
                        }
                    }

                    if (Player.GetSpellDamage(target, SpellSlot.Q) > target.Health + 20)
                    {
                        if (Q.IsReady() && Player.Distance(target.ServerPosition) <= Q.Range)
                        {
                            CancelUlt(target);
                            Q.Cast(target);
                            return;
                        }
                        if (Q.IsReady() && E.IsReady() && Player.Distance(target.ServerPosition) <= 1375 &&
                            Menu.Item("wardKs", true).GetValue<bool>() &&
                            target.CountEnemiesInRange(500) < 3)
                        {
                            CancelUlt(target);
                            WardJumper.JumpKs(target);
                            return;
                        }
                    }

                    if (Player.Distance(target.ServerPosition) <= E.Range && shouldE &&
                        Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 20)
                    {
                        if (E.IsReady())
                        {
                            CancelUlt(target);
                            E.Cast(target);
                            E.LastCastAttemptT = Utils.TickCount + delay;
                            return;
                        }
                    }

                    if (Player.Distance(target.ServerPosition) <= E.Range &&
                        Player.GetSpellDamage(target, SpellSlot.R) * 5 > target.Health + 20 &&
                        Menu.Item("rKS", true).GetValue<bool>())
                    {
                        if (R.IsReady())
                        {
                            Orbwalker.SetAttack(false);
                            Orbwalker.SetMovement(false);
                            R.Cast();
                            return;
                        }
                    }
                }
            }
        }

        private void CancelUlt(AIHeroClient target)
        {
            if (Player.IsChannelingImportantSpell() || Player.HasBuff("katarinarsound"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.ServerPosition);
                R.LastCastAttemptT = 0;
            }
        }

        private void ShouldCancel()
        {
            if (Player.CountEnemiesInRange(500) < 1)
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                if (target == null)
                    return;

                R.LastCastAttemptT = 0;
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);
            }

        }

        private void AutoW()
        {
            if (!W.IsReady())
                return;

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
            {
                if (target != null && !target.IsDead)
                {
                    W.Cast();
                }
            }
        }

        private bool QSuccessfullyCasted()
        {
            return Utils.TickCount - Q.LastCastAttemptT > 350 || !Menu.Item("waitQ", true).GetValue<bool>();
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (!unit.IsMe)
                return;

            if (args.SData.Name == "KatarinaR")
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
            }

            var castedSlot = ObjectManager.Player.GetSpellSlot(args.SData.Name);

            if (castedSlot == SpellSlot.Q)
            {
                Q.LastCastAttemptT = Utils.TickCount;
            }

            if (castedSlot == SpellSlot.R)
            {
                R.LastCastAttemptT = Utils.TickCount;
            }
        }

        protected override void ObjAiHeroOnOnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (args.Order == GameObjectOrder.AttackUnit && Menu.Item("disableaa").GetValue<bool>() &&
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = false;
            }
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            SmartKs();

            if (Player.IsChannelingImportantSpell() || Player.HasBuff("KatarinaR"))
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
                ShouldCancel();
                return;
            }

            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);

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
                    JungleFarm();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("FarmT", true).GetValue<KeyBind>().Active)
                        Harass();

                    if (Menu.Item("autoWz", true).GetValue<bool>())
                        AutoW();
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Orbwalking.MoveTo(Game.CursorPos);
                    WardJumper.WardJump();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Menu.Item(spell.Slot + "Range", true).GetValue<Circle>();
                if (menuItem.Active)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, (spell.IsReady()) ? Color.Cyan : Color.DarkRed);
            }

            if (Menu.Item("Draw_Mode", true).GetValue<Circle>().Active)
            {
                var wts = Drawing.WorldToScreen(Player.Position);

                Drawing.DrawText(wts[0], wts[1], Color.White,
                    Menu.Item("KS_With_E", true).GetValue<KeyBind>().Active ? "Ks E Active" : "Ks E Off");
            }
        }

        protected override void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion))
                return;

            if (Utils.TickCount < WardJumper.LastPlaced + 300)
            {
                var ward = (Obj_AI_Minion)sender;
                if (ward.Name.ToLower().Contains("ward") && ward.Distance(WardJumper.LastWardPos) < 500 && E.IsReady())
                {
                    E.Cast(ward);
                }
            }
        }
    }
}
