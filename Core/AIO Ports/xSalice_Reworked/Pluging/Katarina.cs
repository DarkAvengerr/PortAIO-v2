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

    internal class Katarina : Champion
    {
        private int LastCastW;

        public Katarina()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 625f);
            SpellManager.E = new Spell(SpellSlot.E, 720f);
            SpellManager.R = new Spell(SpellSlot.R, 550f);

            SpellManager.Q.SetTargetted(400f, 1400f);
            SpellManager.R.SetCharged(550, 550, 1.0f);

            SpellManager.SpellList.Add(Q);
            SpellManager.SpellList.Add(E);
            SpellManager.SpellList.Add(R);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseEDaggerCombo", "Use E to Dagger", true).SetValue(true));
                combo.AddItem(new MenuItem("eDis", "E only if >", true).SetValue(new Slider(0, 0, 700)));
                combo.AddItem(new MenuItem("smartE", "Smart E with R CD ", true).SetValue(false));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                combo.AddItem(
                    new MenuItem("comboMode", "Mode", true).SetValue(new StringList(new[] {"Beta(QWEE)", "QEW", "EQW"}, 1)));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEDaggerHarass", "Use E to Dagger", true).SetValue(true));
                harass.AddItem(
                    new MenuItem("harassMode", "Mode", true).SetValue(
                        new StringList(new[] {"Beta(QWEE)", "QEW", "EQW", "Q"}, 3)));
                harass.AddItem(
                    new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0],
                        KeyBindType.Toggle)));
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("Farm", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q Farm", true).SetValue(true));
                farm.AddItem(new MenuItem("UseWFarm", "Use W Farm", true).SetValue(true));
                farm.AddItem(new MenuItem("UseEFarm", "Use E Farm", true).SetValue(false));
                farm.AddItem(new MenuItem("UseEDaggerFarm", "Use E Farm(Jump to Dagger)", true).SetValue(false));
                farm.AddItem(new MenuItem("UseQHit", "Use Q Last Hit", true).SetValue(true));
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
                misc.AddItem(new MenuItem("E_Delay_Slider", "Delay Between E(ms)", true).SetValue(new Slider(0, 0, 1000)));
                Menu.AddSubMenu(misc);
            }

            var drawing = new Menu("Drawings", "Drawings");
            {
                drawing.AddItem(new MenuItem("QRange", "Q range", true).SetValue(false));
                drawing.AddItem(new MenuItem("ERange", "E range", true).SetValue(false));
                drawing.AddItem(new MenuItem("RRange", "R range", true).SetValue(false));
                drawing.AddItem(new MenuItem("Draw_Mode", "Draw E Mode", true).SetValue(true));
                
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
                customMenu.AddItem(myCust.AddToMenu("Jump Active: ", "Flee"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += GetQDamage(enemy);

            if (E.IsReady())
                damage += GetEDamage(enemy);

            if (R.IsReady() || (RSpell.State == SpellState.Surpressed && R.Level > 0))
                damage += GetRDamage(enemy) * 10;

            damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }

        public double GetQDamage(Obj_AI_Base target)
        {
            if (ObjectManager.Player.GetSpell(SpellSlot.Q).Level == 0)
            {
                return 0d;
            }

            return ObjectManager.Player.CalcDamage
            (target, Damage.DamageType.Magical,
                (float) new double[] {75, 105, 135, 165, 195}[ObjectManager.Player.GetSpell(SpellSlot.Q).Level - 1] +
                0.3f*ObjectManager.Player.TotalMagicalDamage);
        }

        public double GetEDamage(Obj_AI_Base target)
        {
            if (ObjectManager.Player.GetSpell(SpellSlot.E).Level == 0)
            {
                return 0d;
            }

            return ObjectManager.Player.CalcDamage
            (target, Damage.DamageType.Magical,
                (float) new double[] {30, 45, 60, 75, 90}[ObjectManager.Player.GetSpell(SpellSlot.E).Level - 1] +
                0.65f*ObjectManager.Player.TotalAttackDamage +
                0.25f*ObjectManager.Player.TotalMagicalDamage);
        }

        public double GetRDamage(Obj_AI_Base target)
        {
            if (ObjectManager.Player.GetSpell(SpellSlot.R).Level == 0)
            {
                return 0d;
            }

            return ObjectManager.Player.CalcDamage
            (target, Damage.DamageType.Magical,
                (float) new[] {25, 37.5, 50}[ObjectManager.Player.GetSpell(SpellSlot.R).Level - 1] +
                0.65f*ObjectManager.Player.TotalAttackDamage +
                0.25f*ObjectManager.Player.TotalMagicalDamage);
        }

        private void Combo()
        {
            if (ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("katarinar")) ||
                Player.IsChannelingImportantSpell())
            {
                return;
            }

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget(E.Range))
            {
                return;
            }

            if (!target.HasBuffOfType(BuffType.Invulnerability) && !target.IsZombie)
            {
                switch (Menu.Item("comboMode", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
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

                            var targetDagger =
                                ObjectManager
                                    .Get<Obj_AI_Minion>()
                                    .FirstOrDefault(
                                        x => x.CharData.BaseSkinName == "testcuberender" && x.Health > 1
                                        && x.IsValid && x.Distance(Player) <= E.Range
                                        && x.Distance(target) <= 400);

                            if (Utils.TickCount - Q.LastCastAttemptT > 800 && targetDagger != null)
                            {
                                if (Menu.Item("UseWCombo", true).GetValue<bool>() &&
                                    Player.GetSpell(SpellSlot.W).IsReady())
                                {
                                    if (Player.Spellbook.CastSpell(SpellSlot.W))
                                    {
                                        LastCastW = Utils.TickCount;
                                    }
                                }

                                if (Utils.TickCount - LastCastW > 1000)
                                {
                                    if (Menu.Item("UseEDaggerCombo", true).GetValue<bool>()
                                        && targetDagger.Distance(Player) <= E.Range)
                                    {
                                        if (Menu.Item("smartE", true).GetValue<bool>() &&
                                            Player.CountEnemiesInRange(500) > 2 &&
                                            (!R.IsReady() || !(RSpell.State == SpellState.Surpressed && R.Level > 0)))
                                        {
                                            return;
                                        }

                                        var delay = Menu.Item("E_Delay_Slider", true).GetValue<Slider>().Value;

                                        if (targetDagger.Position.CountEnemiesInRange(400) > 0)
                                        {
                                            Orbwalker.SetAttack(false);
                                            Orbwalker.SetMovement(false);
                                            E.Cast(targetDagger, true);
                                            E.LastCastAttemptT = Utils.TickCount + delay;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Menu.Item("UseWCombo", true).GetValue<bool>() && 
                                    Player.GetSpell(SpellSlot.W).IsReady())
                                {
                                    if (Player.Spellbook.CastSpell(SpellSlot.W))
                                    {
                                        LastCastW = Utils.TickCount;
                                    }
                                }

                                if (Utils.TickCount - LastCastW > 1000)
                                {
                                    if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady() &&
                                        target.IsValidTarget(E.Range) &&
                                        Utils.TickCount - E.LastCastAttemptT > 0 &&
                                        Player.Distance(target.Position) > Menu.Item("eDis", true).GetValue<Slider>().Value
                                        && !Q.IsReady())
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
                        }
                        break;
                    case 1:
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

                            if (Menu.Item("UseEDaggerCombo", true).GetValue<bool>())
                            {
                                var Dagger =
                                    ObjectManager.Get<Obj_AI_Minion>()
                                        .FirstOrDefault(
                                            x =>
                                                x.CharData.BaseSkinName == "testcuberender" && x.Health > 1 &&
                                                x.IsValid);

                                if (Dagger != null && Dagger.Distance(Player) <= E.Range)
                                {
                                    if (Menu.Item("smartE", true).GetValue<bool>() &&
                                        Player.CountEnemiesInRange(500) > 2 &&
                                        (!R.IsReady() || !(RSpell.State == SpellState.Surpressed && R.Level > 0)))
                                    {
                                        return;
                                    }

                                    var delay = Menu.Item("E_Delay_Slider", true).GetValue<Slider>().Value;

                                    if (Dagger.Position.CountEnemiesInRange(400) > 0)
                                    {
                                        Orbwalker.SetAttack(false);
                                        Orbwalker.SetMovement(false);
                                        E.Cast(Dagger, true);
                                        E.LastCastAttemptT = Utils.TickCount + delay;
                                    }
                                }
                            }

                            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady() &&
                                target.IsValidTarget(E.Range) &&
                                Utils.TickCount - E.LastCastAttemptT > 0 &&
                                Player.Distance(target.Position) > Menu.Item("eDis", true).GetValue<Slider>().Value
                                && !Q.IsReady())
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
                        break;
                    case 2:
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

                            if (Menu.Item("UseEDaggerCombo", true).GetValue<bool>())
                            {
                                var Dagger =
                                    ObjectManager.Get<Obj_AI_Minion>()
                                        .FirstOrDefault(
                                            x =>
                                                x.CharData.BaseSkinName == "testcuberender" && x.Health > 1 &&
                                                x.IsValid);

                                if (Dagger != null && Dagger.Distance(Player) <= E.Range)
                                {
                                    if (Menu.Item("smartE", true).GetValue<bool>() &&
                                        Player.CountEnemiesInRange(500) > 2 &&
                                        (!R.IsReady() || !(RSpell.State == SpellState.Surpressed && R.Level > 0)))
                                    {
                                        return;
                                    }

                                    var delay = Menu.Item("E_Delay_Slider", true).GetValue<Slider>().Value;

                                    if (Dagger.Position.CountEnemiesInRange(400) > 0)
                                    {
                                        Orbwalker.SetAttack(false);
                                        Orbwalker.SetMovement(false);
                                        E.Cast(Dagger, true);
                                        E.LastCastAttemptT = Utils.TickCount + delay;
                                    }
                                }
                            }

                            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady() &&
                                target.IsValidTarget(E.Range) &&
                                Utils.TickCount - E.LastCastAttemptT > 0 &&
                                Player.Distance(target.Position) > Menu.Item("eDis", true).GetValue<Slider>().Value
                                && !Q.IsReady())
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
                        break;
                }

                if (Menu.Item("comboMode", true).GetValue<StringList>().SelectedIndex != 0 &&
                    Menu.Item("UseWCombo", true).GetValue<bool>() && Player.GetSpell(SpellSlot.W).IsReady() &&
                    (target.IsValidTarget(300) ||
                     (!E.IsReady() && target.IsValidTarget(E.Range))))

                {
                    if (Player.Spellbook.CastSpell(SpellSlot.W))
                    {
                        LastCastW = Utils.TickCount;
                    }
                }

                if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady() &&
                    Player.CountEnemiesInRange(R.Range) > 0)
                {
                    if (!Q.IsReady() && !E.IsReady())
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
            if (ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("katarinar")) ||
                Player.IsChannelingImportantSpell())
            {
                return;
            }

            var Dagger =
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(x => x.CharData.BaseSkinName == "testcuberender" && x.Health > 1 && x.IsValid);
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            switch (Menu.Item("harassMode", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    {
                        if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() &&
                            target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target);
                        }

                        var targetDagger =
                            ObjectManager
                                .Get<Obj_AI_Minion>()
                                .FirstOrDefault(
                                    x => x.CharData.BaseSkinName == "testcuberender" && x.Health > 1
                                    && x.IsValid && x.Distance(Player) <= E.Range
                                    && x.Distance(target) <= 400);

                        if (Utils.TickCount - Q.LastCastAttemptT > 800 && targetDagger != null)
                        {
                            if (Menu.Item("UseWHarass", true).GetValue<bool>() &&
                                Player.GetSpell(SpellSlot.W).IsReady())
                            {
                                if (Player.Spellbook.CastSpell(SpellSlot.W))
                                {
                                    LastCastW = Utils.TickCount;
                                }
                            }

                            if (Utils.TickCount - LastCastW > 1000)
                            {
                                if (Menu.Item("UseEDaggerHarass", true).GetValue<bool>()
                                    && targetDagger.Distance(Player) <= E.Range)
                                {
                                    if (targetDagger.Position.CountEnemiesInRange(400) > 0)
                                    {
                                        Orbwalker.SetAttack(false);
                                        Orbwalker.SetMovement(false);
                                        E.Cast(targetDagger, true);
                                        E.LastCastAttemptT = Utils.TickCount;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Menu.Item("UseWHarass", true).GetValue<bool>() &&
                                Player.GetSpell(SpellSlot.W).IsReady())
                            {
                                if (Player.Spellbook.CastSpell(SpellSlot.W))
                                {
                                    LastCastW = Utils.TickCount;
                                }
                            }

                            if (Utils.TickCount - LastCastW > 1000)
                            {
                                if (Menu.Item("UseEHarass", true).GetValue<bool>() && E.IsReady() &&
                                    target.IsValidTarget(E.Range) &&
                                    Utils.TickCount - E.LastCastAttemptT > 0 &&
                                    Player.Distance(target.Position) > 0 && !Q.IsReady())
                                {
                                    Orbwalker.SetAttack(false);
                                    Orbwalker.SetMovement(false);
                                    E.Cast(target, true);
                                    E.LastCastAttemptT = Utils.TickCount;
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    {
                        if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() &&
                            target != null && target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target, true);
                        }

                        if (Menu.Item("UseEHarass", true).GetValue<bool>() && target != null && E.IsReady() &&
                            !Q.IsReady() && target.IsValidTarget(E.Range))
                        {
                            if (Menu.Item("UseEDaggerHarass", true).GetValue<bool>() && Dagger != null
                                && target.Distance(Dagger) <= 400)
                            {
                                E.Cast(Dagger, true);
                            }
                            else
                            {
                                E.Cast(target, true);
                            }
                        }

                        if (Menu.Item("UseWHarass", true).GetValue<bool>() && target != null
                            && Player.GetSpell(SpellSlot.W).IsReady() &&
                            target.IsValidTarget(300))
                        {
                            Player.Spellbook.CastSpell(SpellSlot.W);
                        }
                    }
                    break;
                case 2:
                    {
                        if (Menu.Item("UseEHarass", true).GetValue<bool>() && target != null &&
                            E.IsReady() && target.IsValidTarget(E.Range))
                        {
                            if (Menu.Item("UseEDaggerHarass", true).GetValue<bool>() && Dagger != null
                                && target.Distance(Dagger) <= 400)
                            {
                                E.Cast(Dagger, true);
                            }
                            else
                            {
                                E.Cast(target, true);
                            }
                        }

                        if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() && target != null &&
                            target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target, true);
                        }

                        if (Menu.Item("UseWHarass", true).GetValue<bool>() && target != null
                            && Player.GetSpell(SpellSlot.W).IsReady() &&
                            target.IsValidTarget(300))
                        {
                            Player.Spellbook.CastSpell(SpellSlot.W);
                        }
                    }
                    break;
                case 3:
                    {
                        if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() && target != null &&
                            target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target, true);
                        }
                    }
                    break;
            }
        }

        private void LastHit()
        {
            if (ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("katarinar")) ||
                Player.IsChannelingImportantSpell())
            {
                return;
            }
            var allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQHit", true).GetValue<bool>();

            if (Q.IsReady() && useQ)
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget(Q.Range) &&
                        HealthPrediction.GetHealthPrediction(
                            minion, (int)(Player.Distance(minion.Position) * 1000 / 1400), 200) <
                        GetQDamage(minion))
                    {
                        Q.Cast(minion);
                        return;
                    }
                }
            }
        }

        private void Farm()
        {
            if (ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("katarinar")) ||
                Player.IsChannelingImportantSpell())
            {
                return;
            }

            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 400);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();
            var useEDagger = Menu.Item("UseEDaggerFarm", true).GetValue<bool>();

            var Dagger =
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(x => x.CharData.BaseSkinName == "testcuberender" && x.Health > 1 && x.IsValid);

            if (useQ && allMinionsQ.Count > 0 && Q.IsReady() && allMinionsQ[0].IsValidTarget(Q.Range))
            {
                Q.Cast(allMinionsQ[0]);
            }

            if (Dagger != null && Dagger.Distance(Player) <= E.Range)
            {
                var daggerMinions = MinionManager.GetMinions(Dagger.Position, 400);

                if (daggerMinions.Count >= 2)
                {
                    if (useW && Player.GetSpell(SpellSlot.W).IsReady())
                    {
                        if (allMinionsW.Count >= 2)
                        {
                            foreach (var minion in allMinionsW)
                            {
                                if (Player.Spellbook.CastSpell(SpellSlot.W))
                                {
                                    LastCastW = Utils.TickCount;
                                }
                            }
                        }
                    }

                    if (useE && Utils.TickCount - LastCastW > 1000 && E.IsReady() && Dagger.Distance(Player) <= E.Range)
                    {
                        E.CastOnUnit(Dagger);
                    }
                }
            }
            else
            {
                if (useW && Player.GetSpell(SpellSlot.W).IsReady())
                {
                    if (allMinionsW.Count >= 2)
                    {
                        foreach (var minion in allMinionsW)
                        {
                            if (Player.Spellbook.CastSpell(SpellSlot.W))
                            {
                                LastCastW = Utils.TickCount;
                            }
                        }
                    }
                }
            }
        }

        private void JungleFarm()
        {
            if (ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("katarinar")) ||
                Player.IsChannelingImportantSpell())
            {
                return;
            }
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.Neutral);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 400,
                MinionTypes.All, MinionTeam.Neutral);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();
            var useEDagger = Menu.Item("UseEDaggerFarm", true).GetValue<bool>();

            var Dagger =
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(x => x.CharData.BaseSkinName == "testcuberender" && x.Health > 1 && x.IsValid);

            if (useQ && allMinionsQ.Count > 0 && Q.IsReady() && allMinionsQ[0].IsValidTarget(Q.Range))
            {
                Q.Cast(allMinionsQ[0]);
            }

            if (Dagger != null && Dagger.Distance(Player) <= E.Range)
            {
                var daggerMinions = MinionManager.GetMinions(Dagger.Position, 400, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (daggerMinions.Count >= 1)
                {
                    if (useW && Player.GetSpell(SpellSlot.W).IsReady())
                    {
                        if (Player.Spellbook.CastSpell(SpellSlot.W))
                        {
                            LastCastW = Utils.TickCount;
                        }
                    }

                    if (useEDagger && E.IsReady() && Utils.TickCount - LastCastW > 1000)
                    {
                        if (daggerMinions.Count >= 1)
                        {
                            E.CastOnUnit(Dagger);
                        }
                    }
                }
            }
            else
            {
                if (useE && allMinionsQ.Count > 0 && E.IsReady() && allMinionsQ[0].IsValidTarget(E.Range))
                {
                    E.Cast(allMinionsE[0]);
                }

                if (useW && Player.GetSpell(SpellSlot.W).IsReady())
                {
                    if (allMinionsW.Count > 0)
                    {
                        if (Player.Spellbook.CastSpell(SpellSlot.W))
                        {
                            LastCastW = Utils.TickCount;
                        }
                    }
                }
            }
        }

        private void SmartKs()
        {
            if (!Menu.Item("smartKS", true).GetValue<bool>())
                return;

            if (Menu.Item("rCancel", true).GetValue<bool>() &&
                HeroManager.Enemies.Any(x => !x.IsDead && !x.IsZombie && x.IsValidTarget(500)))
            {
                return;
            }

            foreach (var target in HeroManager.Enemies.Where(
                x => x.IsValidTarget(1375) && !x.HasBuffOfType(BuffType.Invulnerability))
                .OrderByDescending(GetComboDamage))
            {
                if (target != null)
                {
                    var delay = Menu.Item("E_Delay_Slider", true).GetValue<Slider>().Value;
                    var shouldE = !Menu.Item("KS_With_E", true).GetValue<KeyBind>().Active && Utils.TickCount - E.LastCastAttemptT > 0;

                    if (Player.Distance(target.ServerPosition) <= E.Range && shouldE &&
                        GetEDamage(target) + GetQDamage(target) > target.Health + 20)
                    {
                        if (E.IsReady() && Q.IsReady())
                        {
                            CancelUlt(target);
                            Q.Cast(target);
                            E.Cast(target);
                            E.LastCastAttemptT = Utils.TickCount + delay;
                            return;
                        }
                    }

                    if (GetQDamage(target) > target.Health + 20)
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
                        GetEDamage(target) > target.Health + 20)
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
                        GetRDamage(target) * 5 > target.Health + 20 &&
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
            if (ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("katarinar")) ||
                Player.IsChannelingImportantSpell())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.ServerPosition);
            }
        }

        private void ShouldCancel()
        {
            if (HeroManager.Enemies.Any(x => !x.IsDead && !x.IsZombie && x.IsValidTarget(500)))
            {
                return;
            }

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget())
                return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);
        }

        private bool QSuccessfullyCasted()
        {
            return Utils.TickCount - Q.LastCastAttemptT > 350;
        }

        protected override void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!args.Unit.IsMe)
                args.Process = !ObjectManager.Player.HasBuff("KatarinaR");
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (!unit.IsMe)
                return;

            var castedSlot = ObjectManager.Player.GetSpellSlot(args.SData.Name);

            if (castedSlot == SpellSlot.Q)
            {
                Q.LastCastAttemptT = Utils.TickCount;
            }
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            SmartKs();

            if (ObjectManager.Player.Buffs.Any(x => x.Name.ToLower().Contains("katarinar")) ||
                Player.IsChannelingImportantSpell())
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
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    WardJumper.WardJump();
                    break;
            }
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("QRange", true).GetValue<bool>() && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, (Q.IsReady()) ? Color.Cyan : Color.DarkRed);
            }

            if (Menu.Item("ERange", true).GetValue<bool>() && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, (E.IsReady()) ? Color.Cyan : Color.DarkRed);
            }

            if (Menu.Item("RRange", true).GetValue<bool>() && R.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, (R.IsReady()) ? Color.Cyan : Color.DarkRed);
            }

            if (Menu.Item("Draw_Mode", true).GetValue<bool>())
            {
                var wts = Drawing.WorldToScreen(Player.Position);

                Drawing.DrawText(wts[0], wts[1], Color.White,
                    Menu.Item("KS_With_E", true).GetValue<KeyBind>().Active ? "Ks E Active" : "Ks E Off");
            }
        }
    }
}
