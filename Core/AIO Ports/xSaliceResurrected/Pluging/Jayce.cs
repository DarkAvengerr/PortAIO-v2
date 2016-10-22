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

    internal class Jayce : Champion
    {
        public static float Qcd, QcdEnd;
        public static float Q1Cd, Q1CdEnd;
        public static float Wcd, WcdEnd;
        public static float W1Cd, W1CdEnd;
        public static float Ecd, EcdEnd;
        public static float E1Cd, E1CdEnd;

        public static bool IsMelee => !ObjectManager.Player.HasBuff("jaycestancegun");

        public Jayce()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 1050f);
            SpellManager.QExtend = new Spell(SpellSlot.Q, 1650f);
            SpellManager.Q2 = new Spell(SpellSlot.Q, 600f);
            SpellManager.W = new Spell(SpellSlot.W);
            SpellManager.W2 = new Spell(SpellSlot.W, 350f);
            SpellManager.E = new Spell(SpellSlot.E, 650f);
            SpellManager.E2 = new Spell(SpellSlot.E, 240f);
            SpellManager.R = new Spell(SpellSlot.R);

            SpellManager.Q.SetSkillshot(0.25f, 79f, 1200f, true, SkillshotType.SkillshotLine);
            SpellManager.QExtend.SetSkillshot(0.35f, 98f, 1900f, true, SkillshotType.SkillshotLine);
            SpellManager.Q2.SetTargetted(0.25f, float.MaxValue);
            SpellManager.E.SetSkillshot(0.1f, 120, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.E2.SetTargetted(.25f, float.MaxValue);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Cannon Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use Cannon W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use Cannon E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseQComboHam", "Use Hammer Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWComboHam", "Use Hammer W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseEComboHam", "Use Hammer E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R to Switch", true).SetValue(true));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(new MenuItem("UseQHarassHam", "Use Q Hammer", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarassHam", "Use W Hammer", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarassHam", "Use E Hammer", true).SetValue(true));
                harass.AddItem(new MenuItem("UseRHarass", "Use R to switch", true).SetValue(true));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 60);
                Menu.AddSubMenu(harass);
            }

            var misc = new Menu("Misc", "Misc");
            {
                misc.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(false, false, false, false, true));
                misc.AddItem(new MenuItem("shoottheQE", "Shoot QE", true).SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press)));
                misc.AddItem(
                    new MenuItem("shootmode", "Shoot QE Mode", true).SetValue(new StringList(new[] {"Target", "Mouse"})));
                misc.AddItem(new MenuItem("UseInt", "Use E to Interrupt", true).SetValue(true));
                misc.AddItem(new MenuItem("UseGap", "Use E for GapCloser", true).SetValue(true));
                misc.AddItem(new MenuItem("forceGate", "Force Gate After Q", true).SetValue(false));
                misc.AddItem(new MenuItem("gatePlace", "Gate Distance", true).SetValue(new Slider(50, 50, 600)));
                misc.AddItem(new MenuItem("UseQAlways", "Use Q When E onCD", true).SetValue(true));
                misc.AddItem(new MenuItem("autoE", "EPushInCombo HP < %", true).SetValue(new Slider(20)));
                misc.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
                Menu.AddSubMenu(misc);
            }

            var drawMenu = new Menu("Drawings", "Drawings");
            {
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_QExtend", "Draw Q Cannon Extended", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("drawcds", "Draw Cooldowns", true).SetValue(false));

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
                customMenu.AddItem(myCust.AddToMenu("Shoot QE ", "shoottheQE"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            if (enemy == null)
                return 0;

            var damage = 0d;

            if (Qcd == 0 && Ecd == 0 && Q.Level > 0 && E.Level > 0)
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q) * 1.4;

            else if (Qcd == 0 && Q.Level > 0)
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (Q1Cd == 0 && Q.Level > 0)
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q, 1);

            if (W1Cd == 0 && W.Level > 0)
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E1Cd == 0 && E.Level > 0)
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            damage = ItemManager.CalcDamage(enemy, damage);

            damage += Player.GetAutoAttackDamage(enemy) * 3;
            return (float)damage;
        }

        private void Combo()
        {
            var qTarget = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);
            var q2Target = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Physical);
            var e2Target = TargetSelector.GetTarget(E2.Range, TargetSelector.DamageType.Physical);

            if (qTarget != null)
            {
                if (Menu.Item("UseQCombo", true).GetValue<bool>() && Qcd == 0 &&
                    Player.Distance(qTarget.Position) <= QExtend.Range && !IsMelee)
                {
                    CastQCannon(qTarget, Menu.Item("UseECombo", true).GetValue<bool>());
                    return;
                }
            }

            if (IsMelee)
            {
                if (q2Target != null)
                {
                    if (Menu.Item("UseWHarassHam", true).GetValue<bool>() && Player.Distance(q2Target.Position) <= 300 &&
                        W.IsReady())
                    {
                        W.Cast();
                    }

                    if (Menu.Item("UseQComboHam", true).GetValue<bool>() &&
                        Player.Distance(q2Target.Position) <= Q2.Range + q2Target.BoundingRadius && Q2.IsReady())
                    {
                        Q2.Cast(q2Target);
                    }
                }
                if (e2Target != null)
                {
                    if (Menu.Item("UseEComboHam", true).GetValue<bool>() &&
                        ECheck(e2Target, Menu.Item("UseQCombo", true).GetValue<bool>(),
                        Menu.Item("UseWCombo", true).GetValue<bool>()) &&
                        Player.Distance(e2Target.Position) <= E2.Range + e2Target.BoundingRadius && E2.IsReady())
                    {
                        E2.Cast(q2Target);
                    }
                }
            }

            var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

            if (itemTarget != null)
            {
                var dmg = GetComboDamage(itemTarget);
                ItemManager.Target = itemTarget;

                if (dmg > itemTarget.Health - 50)
                    ItemManager.KillableTarget = true;

                ItemManager.UseTargetted = true;
            }

            if (Menu.Item("UseRCombo", true).GetValue<bool>())
            {
                SwitchFormCheck(q2Target, Menu.Item("UseQCombo", true).GetValue<bool>(),
                    Menu.Item("UseWCombo", true).GetValue<bool>(), Menu.Item("UseQComboHam", true).GetValue<bool>(), 
                    Menu.Item("UseWComboHam", true).GetValue<bool>(), Menu.Item("UseEComboHam", true).GetValue<bool>());
            }

        }
        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            var qTarget = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);
            var q2Target = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Physical);
            var e2Target = TargetSelector.GetTarget(E2.Range, TargetSelector.DamageType.Physical);


            if (qTarget != null)
            {
                if (Menu.Item("UseQHarass", true).GetValue<bool>() && Qcd == 0 &&
                    Player.Distance(qTarget.Position) <= QExtend.Range && !IsMelee)
                {
                    CastQCannon(qTarget, Menu.Item("UseEHarass", true).GetValue<bool>());
                    return;
                }
            }
            if (IsMelee)
            {
                if (q2Target != null)
                {
                    if (Menu.Item("UseWHarassHam", true).GetValue<bool>() && Player.Distance(q2Target.Position) <= 300 &&
                        W.IsReady())
                    {
                        W.Cast();
                    }

                    if (Menu.Item("UseQHarassHam", true).GetValue<bool>() &&
                        Player.Distance(q2Target.Position) <= Q2.Range + q2Target.BoundingRadius && Q2.IsReady())
                    {
                        Q2.Cast(q2Target);
                    }     
                }

                if (q2Target != null)
                {
                    if (Menu.Item("UseEHarassHam", true).GetValue<bool>() &&
                        Player.Distance(q2Target.Position) <= E2.Range + e2Target.BoundingRadius && E2.IsReady())
                    {
                        E2.Cast(q2Target);
                    }
                }
            }

            if (Menu.Item("UseRHarass", true).GetValue<bool>() && q2Target != null)
            {
                SwitchFormCheck(q2Target, Menu.Item("UseQHarass", true).GetValue<bool>(),
                    Menu.Item("UseWHarass", true).GetValue<bool>(), Menu.Item("UseQHarassHam", true).GetValue<bool>(), 
                    Menu.Item("UseWHarassHam", true).GetValue<bool>(),
                    Menu.Item("UseEHarassHam", true).GetValue<bool>());
            }
        }

        private bool ECheck(AIHeroClient target, bool useQ, bool useW)
        {
            if (Player.GetSpellDamage(target, SpellSlot.E) >= target.Health)
            {
                return true;
            }
            if (((Qcd == 0 && useQ) || (Wcd == 0 && useW)) && Q1Cd != 0 && W1Cd != 0)
            {
                return true;
            }
            if (WallStun(target))
            {
                return true;
            }

            var hp = Menu.Item("autoE", true).GetValue<Slider>().Value;
            if (Player.HealthPercent <= hp)
            {
                return true;
            }

            return false;
        }

        private bool WallStun(AIHeroClient target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var pred = E2.GetPrediction(target);

            var pushedPos = pred.CastPosition + Vector3.Normalize(pred.CastPosition - Player.ServerPosition) * 350;

            return Util.IsPassWall(target.ServerPosition, pushedPos);
        }

        private void KsCheck()
        {
            foreach (
                var enemy in
                ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.IsValidTarget(QExtend.Range) && x.IsEnemy && !x.IsDead)
                    .OrderByDescending(GetComboDamage))
            {
                //Q
                if (Player.GetSpellDamage(enemy, SpellSlot.Q) - 20 > enemy.Health && Qcd == 0 &&
                    Player.Distance(enemy.ServerPosition) <= Q.Range)
                {
                    if (IsMelee && R.IsReady())
                    {
                        R.Cast();
                    }

                    if (!IsMelee && Q.IsReady())
                        Q.Cast(enemy);
                }

                //QE
                if (Player.GetSpellDamage(enemy, SpellSlot.Q)*1.4 - 20 > enemy.Health && Qcd == 0 && Ecd == 0 &&
                    Player.Distance(enemy.ServerPosition) <= QExtend.Range)
                {
                    if (IsMelee && R.IsReady())
                    {
                        R.Cast();
                    }

                    if (!IsMelee)
                        CastQCannon(enemy, true);
                }

                //Hammer QE
                if (Player.GetSpellDamage(enemy, SpellSlot.E) + Player.GetSpellDamage(enemy, SpellSlot.Q, 1) - 20 > enemy.Health
                    && E1Cd == 0 && Q1Cd == 0 && Player.Distance(enemy.ServerPosition) <= Q2.Range + enemy.BoundingRadius)
                {
                    if (!IsMelee && R.IsReady())
                    {
                        R.Cast();
                    }

                    if (IsMelee && Q2.IsReady() && E2.IsReady())
                    {
                        Q2.Cast(enemy);
                        E2.Cast(enemy);
                        return;
                    }
                }

                //Hammer Q
                if (Player.GetSpellDamage(enemy, SpellSlot.Q, 1) - 20 > enemy.Health && Q1Cd == 0 &&
                    Player.Distance(enemy.ServerPosition) <= Q2.Range + enemy.BoundingRadius)
                {
                    if (!IsMelee && R.IsReady())
                    {
                        R.Cast();
                    }

                    if (IsMelee && Q2.IsReady())
                    {
                        Q2.Cast(enemy);
                        return;
                    }
                }

                //Hammer E
                if (Player.GetSpellDamage(enemy, SpellSlot.E) - 20 > enemy.Health && E1Cd == 0 &&
                    Player.Distance(enemy.ServerPosition) <= E2.Range + enemy.BoundingRadius)
                {
                    if (!IsMelee && R.IsReady() && enemy.Health > 80)
                    {
                        R.Cast();
                    }

                    if (IsMelee && E2.IsReady())
                    {
                        E2.Cast(enemy);
                        return;
                    }
                }
            }
        }

        private void SwitchFormCheck(AIHeroClient target, bool useQ, bool useW, bool useQ2, bool useW2, bool useE2)
        {
            if (target == null)
                return;

            if (target.Health > 80)
            {
                //switch to hammer
                if ((Qcd != 0 || !useQ) && (Wcd != 0 && !HyperCharged() || !useW) && R.IsReady() && HammerAllReady() &&
                    !IsMelee && Player.Distance(target.ServerPosition) < 650 && (useQ2 || useW2 || useE2))
                {
                    R.Cast();
                    return;
                }
            }

            //switch to cannon
            if (((Qcd == 0 && useQ) || Wcd == 0 && useW && R.IsReady()) && IsMelee)
            {
                R.Cast();
                return;
            }

            if (Q1Cd != 0 && W1Cd != 0 && E1Cd != 0 && IsMelee && R.IsReady())
            {
                R.Cast();
            }
        }

        private bool HyperCharged()
        {
            return Player.Buffs.Any(buffs => buffs.Name == "jaycehypercharge");
        }

        private bool HammerAllReady()
        {
            return Q1Cd == 0 && W1Cd == 0 && E1Cd == 0;
        }

        private void CastQCannon(AIHeroClient target, bool useE)
        {
            var gateDis = Menu.Item("gatePlace", true).GetValue<Slider>().Value;

            var tarPred = QExtend.GetPrediction(target, true);

            if (tarPred.Hitchance >= HitChance.VeryHigh && Qcd == 0 && Ecd == 0 && useE)
            {
                var gateVector = Player.Position + Vector3.Normalize(target.ServerPosition - Player.Position) * gateDis;

                if (Player.Distance(tarPred.CastPosition) < QExtend.Range + 100)
                {
                    if (E.IsReady() && QExtend.IsReady())
                    {
                        E.Cast(gateVector);
                        QExtend.Cast(tarPred.CastPosition);
                        return;
                    }
                }
            }

            if ((Menu.Item("UseQAlways", true).GetValue<bool>() || !useE) &&
                Qcd == 0 && Q.GetPrediction(target, true).Hitchance >= HitChance.VeryHigh &&
                Player.Distance(target.ServerPosition) <= Q.Range && Q.IsReady() && Ecd != 0)
            {
                Q.Cast(target);
            }
        }

        private void CastQCannonMouse()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (IsMelee && !R.IsReady())
                return;

            if (IsMelee && R.IsReady())
            {
                R.Cast();
                return;
            }

            if (Ecd == 0 && Qcd== 0 && !IsMelee)
            {
                if (Menu.Item("shootmode", true).GetValue<StringList>().SelectedIndex == 1)
                {
                    var gateDis = Menu.Item("gatePlace", true).GetValue<Slider>().Value;
                    var gateVector = Player.ServerPosition +
                                     Vector3.Normalize(Game.CursorPos - Player.ServerPosition)*gateDis;

                    if (E.IsReady() && Q.IsReady())
                    {
                        E.Cast(gateVector);
                        Q.Cast(Game.CursorPos);
                    }
                }
                else
                {
                    var qTarget = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                    if (qTarget != null)
                    {
                        if (Qcd == 0 && Player.Distance(qTarget.Position) <= QExtend.Range)
                        {
                            CastQCannon(qTarget, true);
                        }
                    }
                }
            }
        }

        private void ProcessCooldowns()
        {
            if (!IsMelee)
            {
                QcdEnd = Q.Instance.CooldownExpires;
                WcdEnd = W.Instance.CooldownExpires;
                EcdEnd = E.Instance.CooldownExpires;
            }
            else
            {
                Q1CdEnd = Q2.Instance.CooldownExpires;
                W1CdEnd = W2.Instance.CooldownExpires;
                E1CdEnd = E2.Instance.CooldownExpires;
            }

            Qcd = Q.Level > 0 ? CheckCD(QcdEnd) : -1;
            Wcd = W.Level > 0 ? CheckCD(WcdEnd) : -1;
            Ecd = E.Level > 0 ? CheckCD(EcdEnd) : -1;
            Q1Cd = Q2.Level > 0 ? CheckCD(Q1CdEnd) : -1;
            W1Cd = W2.Level > 0 ? CheckCD(W1CdEnd) : -1;
            E1Cd = E2.Level > 0 ? CheckCD(E1CdEnd) : -1;
        }

        private static float CheckCD(float Expires)
        {
            var time = Expires - Game.Time;

            if (time < 0)
            {
                time = 0;

                return time;
            }

            return time;
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            //cd check
            ProcessCooldowns();

            //ks check
            if (Menu.Item("smartKS", true).GetValue<bool>())
                KsCheck();

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
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("shoottheQE", true).GetValue<KeyBind>().Active)
                    {
                        CastQCannonMouse();
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Flee()
        {
            if (IsMelee)
            {
                if (Q1Cd == 0)
                {
                    var fleehero =
                        HeroManager.Enemies
                            .Where(x => x.IsValidTarget(Q2.Range))
                            .OrderBy(x => x.Distance(Game.CursorPos))
                            .FirstOrDefault(x => x.Distance(Game.CursorPos) < Player.Distance(Game.CursorPos) - 200);

                    if (fleehero != null)
                    {
                        Q2.Cast(fleehero);
                    }
                    else
                    {
                        var fleetarget = MinionManager.GetMinions(Player.Position, Q2.Range, MinionTypes.All,
                                MinionTeam.NotAlly)
                            .OrderBy(x => x.Distance(Game.CursorPos))
                            .FirstOrDefault(x => x.Distance(Game.CursorPos) < Player.Distance(Game.CursorPos) - 200);

                        if (fleetarget != null)
                        {
                            Q2.Cast(fleetarget);
                        }
                    }
                }

                if (R.IsReady() && Ecd == 0)
                {
                    R.Cast();
                }
            }
            else
            {
                if (Ecd == 0)
                {
                    var pos = Player.Position + Vector3.Normalize(Game.CursorPos - Player.Position)*80;

                    E.Cast(pos);
                }

                if (R.IsReady() && Q1Cd == 0)
                {
                    R.Cast();
                }
            }
        }

        protected override void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var useWCombo = Menu.Item("UseWCombo", true).GetValue<bool>();
            var useWHarass = Menu.Item("UseWHarass", true).GetValue<bool>();

            if (unit.IsMe && !IsMelee)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (Wcd == 0 && Player.Distance(target.Position) < 600 && !IsMelee && W.Level > 0 && W.IsReady())
                    {
                        if (useWCombo)
                        {
                            Orbwalking.ResetAutoAttackTimer();
                            W.Cast();
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || Menu.Item("FarmT", true).GetValue<KeyBind>().Active)
                {
                    if (Wcd == 0 && Player.Distance(target.Position) < 600 && !IsMelee && W.Level > 0 && W.IsReady() &&
                        target is AIHeroClient)
                    {
                        if (useWHarass)
                        {
                            Orbwalking.ResetAutoAttackTimer();
                            W.Cast();
                        }
                    }
                }
            }
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("Draw_Q", true).GetValue<bool>() && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, IsMelee? Q2.Range : Q.Range, Q.IsReady() ? Color.Green : Color.Red);
            }

            if (Menu.Item("Draw_QExtend", true).GetValue<bool>() && Q.Level > 0 && !IsMelee)
            {
                Render.Circle.DrawCircle(Player.Position, QExtend.Range, Q.IsReady() ? Color.Green : Color.Red);
            }

            if (Menu.Item("Draw_E", true).GetValue<bool>() && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, IsMelee ? E2.Range : E.Range, E.IsReady() ? Color.Green : Color.Red);
            }

            if (Menu.Item("drawcds", true).GetValue<bool>())
            {
                string msg;
                var QCoolDown = (int)Qcd == -1 ? 0 : (int)Qcd;
                var WCoolDown = (int)Wcd == -1 ? 0 : (int)Wcd;
                var ECoolDown = (int)Ecd == -1 ? 0 : (int)Ecd;
                var Q1CoolDown = (int)Q1Cd == -1 ? 0 : (int)Q1Cd;
                var W1CoolDown = (int)W1Cd == -1 ? 0 : (int)W1Cd;
                var E1CoolDown = (int)E1Cd == -1 ? 0 : (int)E1Cd;

                if (IsMelee)
                {
                    msg = "Q: " + QCoolDown + "   W: " + WCoolDown + "   E: " + ECoolDown;
                    Drawing.DrawText(Player.HPBarPosition.X + 30, Player.HPBarPosition.Y - 30, Color.Orange, msg);
                }
                else
                {
                    msg = "Q: " + Q1CoolDown + "   W: " + W1CoolDown + "   E: " + E1CoolDown;
                    Drawing.DrawText(Player.HPBarPosition.X + 30, Player.HPBarPosition.Y - 30, Color.SkyBlue, msg);
                }
            }
        }

        protected override void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is MissileClient))
                return;

            var spell = (MissileClient)sender;
            var unit = spell.SpellCaster.Name;
            var name = spell.SData.Name;

            if (unit == null)
                return;

            if (unit == Player.Name && name == "JayceShockBlastMis")
            {
                if (Menu.Item("forceGate", true).GetValue<bool>() && Ecd == 0 && E.IsReady())
                {
                    var vec = spell.Position - Vector3.Normalize(Player.ServerPosition - spell.Position) * 100;
                    E.Cast(vec);
                }
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("UseGap", true).GetValue<bool>()) return;

            if (E1Cd == 0 && gapcloser.Sender.IsValidTarget(E2.Range + gapcloser.Sender.BoundingRadius))
            {
                if (!IsMelee && R.IsReady())
                    R.Cast();

                if (E2.IsReady())
                    E2.Cast(gapcloser.Sender);
            }
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("UseInt", true).GetValue<bool>()) return;

            if (unit != null && Player.Distance(unit.Position) < Q2.Range + unit.BoundingRadius && Q1Cd == 0 && E1Cd == 0)
            {
                if (!IsMelee && R.IsReady())
                    R.Cast();

                if (Q2.IsReady())
                    Q2.Cast(unit);
            }

            if (unit != null && Player.Distance(unit.Position) < E2.Range + unit.BoundingRadius && E1Cd == 0)
            {
                if (!IsMelee && R.IsReady())
                    R.Cast();

                if (E2.IsReady())
                    E2.Cast(unit);
            }
        }
    }
}
