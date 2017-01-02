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

    internal class Azir : Champion
    {
        private static AIHeroClient _insecTarget;
        private Vector3 InsecPosition;

        public Azir()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 1150);
            SpellManager.Q2 = new Spell(SpellSlot.Q, 875);
            SpellManager.W = new Spell(SpellSlot.W, 450);
            SpellManager.E = new Spell(SpellSlot.E, 1100);
            SpellManager.R = new Spell(SpellSlot.R, 450);
            SpellManager.R2 = new Spell(SpellSlot.R);

            SpellManager.Q.SetSkillshot(0, 80, 1600, false, SkillshotType.SkillshotCircle);
            SpellManager.Q2.SetSkillshot(0, 80, 1600, false, SkillshotType.SkillshotCircle);
            SpellManager.E.SetSkillshot(0.25f, 100, 1200, false, SkillshotType.SkillshotLine);
            SpellManager.R.SetSkillshot(0.5f, 700, 1400, false, SkillshotType.SkillshotLine);

            SpellManager.SpellList.Add(Q);
            SpellManager.SpellList.Add(W);
            SpellManager.SpellList.Add(E);
            SpellManager.SpellList.Add(R);

            var spell = new Menu("Spell", "Spell");
            {
                var qMenu = new Menu("QSpell", "QSpell");
                {
                    qMenu.AddItem(new MenuItem("qOutRange", "Only Use When target out of range", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("qMulti", "Q if 2+ Soldier", true).SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Toggle)));
                    spell.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("WSpell", "WSpell");
                {
                    wMenu.AddItem(new MenuItem("wAtk", "Always Atk Enemy", true).SetValue(true));
                    spell.AddSubMenu(wMenu);
                }

                var eMenu = new Menu("ESpell", "ESpell");
                {
                    eMenu.AddItem(new MenuItem("eKill", "If Killable Combo", true).SetValue(false));
                    eMenu.AddItem(new MenuItem("eKnock", "Always Knockup/DMG", true).SetValue(false));
                    eMenu.AddItem(new MenuItem("eHP", "if HP >", true).SetValue(new Slider(100)));
                    spell.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("RSpell", "RSpell");
                {
                    rMenu.AddItem(new MenuItem("rHP", "if HP <", true).SetValue(new Slider(20)));
                    rMenu.AddItem(new MenuItem("rWall", "R Enemy Into Wall", true).SetValue(true));
                    spell.AddSubMenu(rMenu);
                }

                Menu.AddSubMenu(spell);
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
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(false));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 60);
                Menu.AddSubMenu(harass);
            }

            var killSteal = new Menu("KillSteal", "KillSteal");
            {
                killSteal.AddItem(new MenuItem("smartKS", "Use Smart KS System", true).SetValue(true));
                killSteal.AddItem(new MenuItem("eKS", "Use E KS", true).SetValue(false));
                killSteal.AddItem(new MenuItem("wqKS", "Use WQ KS", true).SetValue(true));
                killSteal.AddItem(new MenuItem("qeKS", "Use WQE KS", true).SetValue(false));
                killSteal.AddItem(new MenuItem("rKS", "Use R KS", true).SetValue(true));
                Menu.AddSubMenu(killSteal);
            }

            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(false));
                farm.AddItem(new MenuItem("qFarm", "Only Q if > minion", true).SetValue(new Slider(3, 0, 5)));
                ManaManager.AddManaManagertoMenu(farm, "Farm", 50);
                Menu.AddSubMenu(farm);
            }

            var Jungle = new Menu("JungleClear", "JungleClear");
            {
                Jungle.AddItem(new MenuItem("UseQJungle", "Use Q", true).SetValue(true));
                Jungle.AddItem(new MenuItem("UseWJungle", "Use W", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(Jungle, "Jungle", 50);
                Menu.AddSubMenu(Jungle);
            }

            var misc = new Menu("Misc", "Misc");
            {
                misc.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(false, false, false, true));
                misc.AddItem(new MenuItem("qeCombo", "Q->E stun Nearest target", true).SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                misc.AddItem(new MenuItem("UseInt", "Use E to Interrupt", true).SetValue(true));
                misc.AddItem(new MenuItem("UseGap", "Use R for GapCloser", true).SetValue(true));
                misc.AddItem(new MenuItem("FleeDelay", "Escape Delay Decrease", true).SetValue(new Slider(0, 0, 300)));
                misc.AddItem(new MenuItem("insec", "Insec Selected target", true).SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Press)));
                misc.AddItem(new MenuItem("InsecMode", "Insec Mode: ", true).SetValue(new StringList(new[] { "Player Position", "Cursor" }, 1)));
                Menu.AddSubMenu(misc);
            }

            //Drawings menu:
            var draw = new Menu("Drawings", "Drawings");
            {
                draw.AddItem(new MenuItem("QRange", "Q range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("WRange", "W range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("ERange", "E range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("RRange", "R range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("InsecPos", "Insec Position", true).SetValue(true));
                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                var drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                draw.AddItem(drawComboDamageMenu);
                draw.AddItem(drawFill);
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

                Menu.AddSubMenu(draw);
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
                customMenu.AddItem(myCust.AddToMenu("Escape Active: ", "Flee"));
                customMenu.AddItem(myCust.AddToMenu("Insec Active: ", "insec"));
                customMenu.AddItem(myCust.AddToMenu("Q when 2+ Only: ", "qMulti"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            if (enemy == null)
                return 0;

            var damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (soldierCount() > 0 || W.IsReady())
            {
                damage += Orbwalker.GetAzirAaSandwarriorDamage(enemy);
            }

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

            damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }

        private void Combo()
        {
            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var soldierTarget = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Magical);
            var dmg = GetComboDamage(soldierTarget);

            if (soldierTarget == null || qTarget == null)
                return;

            ItemManager.Target = soldierTarget;

            if (dmg > soldierTarget.Health - 50)
                ItemManager.KillableTarget = true;

            ItemManager.UseTargetted = true;

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady() && ShouldR(qTarget) &&
                Player.Distance(qTarget.Position) < R.Range)
            {
                R.Cast(qTarget);
            }

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady() && Menu.Item("UseQCombo", true).GetValue<bool>())
            {
                CastW(qTarget);
            }

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
            {
                CastQ(qTarget);
                return;
            }

            if (Menu.Item("UseECombo", true).GetValue<bool>() && (E.IsReady() || ESpell.State == SpellState.Surpressed))
            {
                CastE(soldierTarget);
            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var soldierTarget = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Magical);
            var dmg = GetComboDamage(soldierTarget);

            if (soldierTarget == null || qTarget == null)
                return;

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady() && Menu.Item("UseQHarass", true).GetValue<bool>())
            {
                CastW(qTarget);
            }

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady())
            {
                CastQ(qTarget);
                return;
            }

            if (Menu.Item("UseEHarass", true).GetValue<bool>() && (E.IsReady() || ESpell.State == SpellState.Surpressed))
            {
                CastE(soldierTarget);
            }
        }

        private bool WallStun(Obj_AI_Base target)
        {
            var pushedPos = R.GetPrediction(target).UnitPosition;

            return Util.IsPassWall(Player.ServerPosition, pushedPos);
        }

        private void SmartKs()
        {
            if (!Menu.Item("smartKS", true).GetValue<bool>())
                return;

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(1200) && !x.HasBuffOfType(BuffType.Invulnerability)).OrderByDescending(GetComboDamage))
            {
                if (target != null)
                {
                    if (Player.GetSpellDamage(target, SpellSlot.R) > target.Health + 20 &&
                        Player.Distance(target.Position) < R.Range && Menu.Item("rKS", true).GetValue<bool>())
                    {
                        R.Cast(target);
                    }

                    if (soldierCount() < 1 && !W.IsReady())
                        return;

                    if (Player.GetSpellDamage(target, SpellSlot.Q) > target.Health + 20 &&
                        Menu.Item("wqKS", true).GetValue<bool>())
                    {
                        CastW(target);
                    }

                    if (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 20 &&
                        Player.Distance(target.Position) < Q.Range && Menu.Item("qeKS", true).GetValue<bool>())
                    {
                        CastQe(target, "Null");
                    }

                }
            }
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (!unit.IsMe)
                return;

            if (args.SData.Name == "AzirQ")
            {
                Q.LastCastAttemptT = Utils.TickCount + 250;
                InsecPosition = Player.Position;
            }

            if (args.SData.Name == "AzirE" && (Q.IsReady() || QSpell.State == SpellState.Surpressed))
            {
                if (Utils.TickCount - E.LastCastAttemptT < 0)
                    Q2.Cast(Game.CursorPos);
            }
        }

        private void Escape()
        {
            var wVec = Player.ServerPosition + Vector3.Normalize(Game.CursorPos - Player.ServerPosition) * W.Range;

            if (E.IsReady())
            {
                if (W.IsReady())
                {
                    W.Cast(wVec);
                    return;
                }

                if (Q.IsReady() && GetNearestSoldierToMouse().Position.Distance(Game.CursorPos) > 300)
                {
                    Q.Cast(Game.CursorPos);
                    return;
                }

                E.Cast();
            }
        }

        private static GameObject GetNearestSoldierToMouse()
        {
            var soldier = Orbwalking.AzirSoliders.ToList().OrderBy(x => Game.CursorPos.Distance(x.Position));

            return soldier.FirstOrDefault();
        }

        private void CastQe(AIHeroClient target, string source)
        {
            if (target == null)
                return;

            if (W.IsReady())
            {
                var wVec = Player.ServerPosition + Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * W.Range;

                var qPred = xSaliceResurrected_Rework.Prediction.CommonPredEx.GetP(wVec, Q, target, W.Delay + Q.Delay, true);

                if ((Q.IsReady() || QSpell.State == SpellState.Surpressed) && (E.IsReady() || ESpell.State == SpellState.Surpressed) &&
                    Player.Distance(target.Position) < Q.Range - 75 && qPred.Hitchance >= HitChance.VeryHigh)
                {
                    var vec = target.ServerPosition - Player.ServerPosition;
                    var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;

                    W.Cast(wVec);
                    LeagueSharp.Common.Utility.DelayAction.Add((int) W.Delay + 100, () => Q2.Cast(castBehind));
                    LeagueSharp.Common.Utility.DelayAction.Add((int)(W.Delay + Q.Delay) + 100, () => E.Cast(castBehind));
                }
            }
        }

        private void Insec()
        {
            var target = _insecTarget;

            if (target == null)
                return;

            switch (Menu.Item("InsecMode", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    InsecPosition = Player.Position;
                    break;
                case 1:
                    InsecPosition = Game.CursorPos;
                    break;
            }

            if (!InsecPosition.IsValid())
                return;

            if (soldierCount() > 0)
            {
                if ((Q.IsReady() || QSpell.State == SpellState.Surpressed) && E.IsReady())
                {
                    var slaves = Orbwalking.AzirSoliders.ToList();

                    foreach (var slave in slaves)
                    {
                        if (Player.Distance(target.Position) < 800)
                        {
                            var qPred = Prediction.GetPrediction(new PredictionInput
                            {
                                Unit = target,
                                Delay = Q.Delay,
                                Radius = Q.Width,
                                Speed = Q.Speed,
                                From = slave.Position,
                                Range = Q.Range,
                                Collision = Q.Collision,
                                Type = Q.Type,
                                RangeCheckFrom = Player.ServerPosition,
                                Aoe = true,
                            });

                            var vec = target.ServerPosition - Player.ServerPosition;
                            var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;

                            if (Q.IsReady() && (E.IsReady() || ESpell.State == SpellState.Surpressed) && R.IsReady() && qPred.Hitchance >= HitChance.VeryHigh)
                            {
                                Q.Cast(castBehind, true);
                                E.Cast(slave.Position, true);
                                E.LastCastAttemptT = Environment.TickCount;
                            }
                        }
                    }
                }
                if (R.IsReady())
                {
                    if (Player.Distance(target.Position) < 200 && Environment.TickCount - E.LastCastAttemptT > Game.Ping + 150)
                    {
                        R.Cast(InsecPosition, true);
                    }
                }
            }
            else if (W.IsReady())
            {
                var wVec = Player.ServerPosition + Vector3.Normalize(target.ServerPosition - Player.ServerPosition) * W.Range;

                var qPred = Prediction.GetPrediction(new PredictionInput
                {
                    Unit = target,
                    Delay = Q.Delay,
                    Radius = Q.Width,
                    Speed = Q.Speed,
                    From = wVec,
                    Range = Q.Range,
                    Collision = Q.Collision,
                    Type = Q.Type,
                    RangeCheckFrom = Player.ServerPosition,
                    Aoe = true,
                });

                if ((Q.IsReady() || QSpell.State == SpellState.Surpressed) && (E.IsReady() || ESpell.State == SpellState.Surpressed)
                    && R.IsReady() && Player.Distance(target.Position) < 800 && qPred.Hitchance >= HitChance.VeryHigh)
                {
                    var vec = target.ServerPosition - Player.ServerPosition;
                    var castBehind = qPred.CastPosition + Vector3.Normalize(vec) * 75;

                    W.Cast(wVec, true);
                    Q.Cast(castBehind, true);
                    E.Cast(Orbwalking.AzirSoliders.ToList().OrderBy(x => x.Position.Distance(target.Position)).FirstOrDefault(), true);
                }

                if (R.IsReady())
                {
                    if (Player.Distance(target.Position) < 200 && Environment.TickCount - E.LastCastAttemptT > Game.Ping + 150)
                    {
                        R.Cast(InsecPosition, true);
                    }
                }
            }
        }

        private void CastW(Obj_AI_Base target)
        {
            if (target == null)
                return;

            if (W.Instance.Ammo > 0)
            {
                if (target.IsValidTarget(W.Range) && Orbwalking.AzirSoliders.Count(x => x.Distance(target) < 250) < 0)
                {
                    W.Cast(Player.Position.Extend(target.Position, W.Range), true);
                }
                else if (target.IsValidTarget(W.Range + 200))
                {
                    W.Cast(target.Position, true);
                }
            }
        }

        private void CastQ(AIHeroClient target)
        {
            if (soldierCount() < 1)
                return;

            var slaves = Orbwalking.AzirSoliders.ToList();

            foreach (var slave in slaves)
            {
                if (Player.Distance(target.Position) < Q.Range && ShouldQ(target, slave))
                {
                    Q.UpdateSourcePosition(slave.Position, Player.ServerPosition);
                    var qPred = Q.GetPrediction(target, true);

                    if (Q.IsReady() && Player.Distance(target.Position) < Q.Range && qPred.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(qPred.CastPosition);
                        return;
                    }
                }
            }
        }

        private void CastE(AIHeroClient target)
        {
            if (soldierCount() < 1)
                return;

            var slaves = Orbwalking.AzirSoliders.ToList();

            foreach (var slave in slaves)
            {
                if (target != null && Player.Distance(slave.Position) < E.Range)
                {
                    var ePred = E.GetPrediction(target);
                    var obj = xSaliceResurrected_Rework.Prediction.CommonPredEx.
                        VectorPointProjectionOnLineSegment(Player.ServerPosition.To2D(), slave.Position.To2D(), ePred.UnitPosition.To2D());
                    var isOnseg = (bool)obj[2];
                    var pointLine = (Vector2)obj[1];

                    if (E.IsReady() && isOnseg && pointLine.Distance(ePred.UnitPosition.To2D()) < E.Width && ShouldE(target))
                    {
                        E.Cast(slave.Position);
                        return;
                    }
                }
            }
        }

        private bool ShouldQ(AIHeroClient target, GameObject slave)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (soldierCount() < 2 && Menu.Item("qMulti", true).GetValue<KeyBind>().Active)
                return false;

            if (!Menu.Item("qOutRange", true).GetValue<bool>())
                return true;

            if (!Orbwalker.InSoldierAttackRange(target))
                return true;

            return Player.GetSpellDamage(target, SpellSlot.Q) > target.Health + 10;
        }

        private bool ShouldE(AIHeroClient target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (Menu.Item("eKnock", true).GetValue<bool>() && GetNearestSoldierToMouse().Position.Distance(target.ServerPosition, true) < 40000)
                return true;

            if (Menu.Item("eKill", true).GetValue<bool>() && GetComboDamage(target) > target.Health + 15)
                return true;

            if (Menu.Item("eKS", true).GetValue<bool>() && Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 10)
                return true;

            var hp = Menu.Item("eHP", true).GetValue<Slider>().Value;
            var hpPercent = Player.Health / Player.MaxHealth * 100;

            return hpPercent > hp;
        }

        private bool ShouldR(AIHeroClient target)
        {
            if (Player.GetSpellDamage(target, SpellSlot.R) > target.Health - 150)
                return true;

            var hp = Menu.Item("rHP", true).GetValue<Slider>().Value;

            if (Player.HealthPercent < hp)
                return true;

            return WallStun(target) && GetComboDamage(target) > target.Health / 2 && Menu.Item("rWall", true).GetValue<bool>();
        }

        private void AutoAtk()
        {
            if (soldierCount() < 1)
                return;

            var soldierTarget = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);

            if (soldierTarget == null)
                return;

            AttackTarget(soldierTarget);
        }

        public int soldierCount()
        {
            return Orbwalking.AzirSoliders.Count;
        }

        private void AttackTarget(AIHeroClient target)
        {
            if (soldierCount() < 1)
                return;

            var tar = getNearestSoldierToEnemy(target);

            if (tar != null && Player.Distance(tar.Position) < 800)
            {
                if (target != null && target.Distance(tar.Position) <= 350)
                {
                    Orbwalking.Orbwalk(target, Game.CursorPos);
                }
            }
        }

        private GameObject getNearestSoldierToEnemy(Obj_AI_Base target)
        {
            var soldier = Orbwalking.AzirSoliders.ToList().OrderBy(x => x.Position.Distance(target.Position)).FirstOrDefault();
             
            return soldier;
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("Farm"))
                return;

            var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width);
            var allMinionsW = MinionManager.GetMinions(Player.ServerPosition, W.Range);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var min = Menu.Item("qFarm", true).GetValue<Slider>().Value;

            if (useQ && (Q.IsReady() || QSpell.State == SpellState.Surpressed))
            {
                int hit;

                if (soldierCount() > 0)
                {
                    var slaves = Orbwalking.AzirSoliders.ToList();

                    foreach (var slave in slaves)
                    {
                        foreach (var enemy in allMinionsQ)
                        {
                            hit = 0;
                            Q.UpdateSourcePosition(slave.Position, Player.ServerPosition);

                            var prediction = Q.GetPrediction(enemy);

                            if (Q.IsReady() && Player.Distance(enemy.Position) <= Q.Range)
                            {
                                hit += allMinionsQ.Count(enemy2 => enemy2.Distance(prediction.CastPosition) < 200 && Q.IsReady());
                                if (hit >= min)
                                {
                                    if (Q.IsReady())
                                    {
                                        Q.Cast(prediction.CastPosition);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                if (W.IsReady())
                {
                    if (allMinionsW.Count >= 3 && W.Instance.Ammo > 1)
                    {
                        var minw = allMinionsW.FirstOrDefault();

                        if (minw != null)
                            W.Cast(minw.Position);
                    }

                    foreach (var enemy in allMinionsQ)
                    {
                        hit = 0;
                        Q.UpdateSourcePosition(Player.Position, Player.ServerPosition);

                        var prediction = Q.GetPrediction(enemy);

                        if (Q.IsReady() && Player.Distance(enemy.Position) <= Q.Range)
                        {
                            hit += allMinionsQ.Count(enemy2 => enemy2.Distance(prediction.CastPosition) < 200 && Q.IsReady());
                            if (hit >= min)
                            {
                                if (Q.IsReady())
                                {
                                    Q.Cast(prediction.CastPosition);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            SmartKs();

            if (Menu.Item("FarmT", true).GetValue<KeyBind>().Active)
                Harass();

            if (Menu.Item("wAtk", true).GetValue<bool>())
                AutoAtk();

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
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Escape();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("insec", true).GetValue<KeyBind>().Active)
                    {
                        Orbwalking.MoveTo(Game.CursorPos);

                        _insecTarget = TargetSelector.GetSelectedTarget();

                        if (_insecTarget != null)
                        {
                            if (_insecTarget.HasBuffOfType(BuffType.Knockup) ||
                                _insecTarget.HasBuffOfType(BuffType.Knockback))
                                if (Player.ServerPosition.Distance(_insecTarget.ServerPosition) < 200)
                                    R2.Cast(InsecPosition);

                            Insec();
                        }
                    }
                    else
                    {
                        InsecPosition = new Vector3(0, 0, 0);
                    }

                    if (Menu.Item("qeCombo", true).GetValue<KeyBind>().Active)
                    {
                        var soldierTarget = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);

                        Orbwalking.MoveTo(Game.CursorPos);

                        CastQe(soldierTarget, "Null");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void JungleClear()
        {
            if (!ManaManager.HasMana("Jungle"))
                return;

            var JungleQ = MinionManager.GetMinions(Player.Position, 1100, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            var useQ = Menu.Item("UseQJungle", true).GetValue<bool>();
            var useW = Menu.Item("UseWJungle", true).GetValue<bool>();

            if (JungleQ.Any())
            {
                var o = JungleQ.FirstOrDefault();

                if (o == null)
                    return;

                if (soldierCount() > 0)
                {
                    if (useQ && Q.IsReady())
                    {
                        Q.Cast(o.Position);
                    }
                }
                else
                {
                    if (useW && W.IsReady() &&  JungleQ.Count(x => x.Distance(Player) <= W.Range) > 0)
                    {
                        W.Cast(o.Position);
                    }
                }
            }
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Menu.Item(spell.Slot + "Range", true).GetValue<Circle>();
                if (menuItem.Active)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }

            if (Menu.Item("InsecPos", true).GetValue<bool>() && InsecPosition != Vector3.Zero && Menu.Item("insec", true).GetValue<KeyBind>().Active)
            {
                Render.Circle.DrawCircle(InsecPosition, 200, Color.GreenYellow);
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("UseGap", true).GetValue<bool>()) return;

            if (R.IsReady() && gapcloser.Sender.IsValidTarget(R.Range))
                R.Cast(gapcloser.Sender);
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("UseInt", true).GetValue<bool>()) return;

            if (Player.Distance(unit.Position) < R.Range && R.IsReady())
            {
                R.Cast(unit);
            }
        }
    }
}
