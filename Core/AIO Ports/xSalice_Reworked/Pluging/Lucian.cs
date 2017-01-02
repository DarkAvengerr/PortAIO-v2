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
    using Geometry = LeagueSharp.Common.Geometry;
    using Orbwalking = Orbwalking;

    public class Lucian : Champion
    {
        private int CastSpellTime;

        public Lucian()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 650);
            SpellManager.Q.SetTargetted(0.25f, float.MaxValue);

            SpellManager.QExtend = new Spell(SpellSlot.Q, 900);
            SpellManager.QExtend.SetSkillshot(0.35f, 25f, float.MaxValue, false, SkillshotType.SkillshotLine);

            SpellManager.W = new Spell(SpellSlot.W, 1000);
            SpellManager.W.SetSkillshot(0.3f, 80f, 1600, true, SkillshotType.SkillshotLine);

            SpellManager.E = new Spell(SpellSlot.E, 425);
            SpellManager.E.SetSkillshot(0.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);

            SpellManager.R = new Spell(SpellSlot.R, 1200);
            SpellManager.R.SetSkillshot(0.2f, 110f, 2800, true, SkillshotType.SkillshotLine);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseQExtendCombo", "Use Q Extended", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseQExtendHarass", "Use Q Extended", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
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

            var jungle = new Menu("JungleClear", "JungleClear");
            {
                jungle.AddItem(new MenuItem("UseQJungle", "Use Q", true).SetValue(true));
                jungle.AddItem(new MenuItem("UseWJungle", "Use W", true).SetValue(true));
                jungle.AddItem(new MenuItem("UseEJungle", "Use E", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(jungle, "JungleClear", 30);
                Menu.AddSubMenu(jungle);
            }

            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("MovementCheck", "Only Cast Extended Q When Enemy is moving(More Accurate)", true).SetValue(false));
                misc.AddItem(new MenuItem("Anti", "Anti Gapcloser E", true).SetValue(true));
                misc.AddItem(new MenuItem("smartKS", "Use Smart KS System", true).SetValue(true));
                misc.AddItem(new MenuItem("CheckECast", "   Check E Cast", true));
                misc.AddItem(new MenuItem("underE", "Dont E to Enemy Turret", true).SetValue(true));
                misc.AddItem(new MenuItem("ECheck", "Check Wall/ Building", true).SetValue(true));
                misc.AddItem(new MenuItem("SafeCheck", "Safe Check", true).SetValue(true));
                Menu.AddSubMenu(misc);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_Q_Extended", "Draw Q Extended", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "Draw W", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_E", "Draw E", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R", true).SetValue(true));

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
                Menu.AddSubMenu(customMenu);
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("Anti", true).GetValue<bool>() && E.IsReady())
            {
                if (gapcloser.End.Distance(Player.Position) <= 200 || gapcloser.Sender.Distance(Player) < 250)
                {
                    E.Cast(Player.Position.Extend(gapcloser.Sender.Position, -E.Range));
                }
            }
        }

        protected override void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var target = args.Target as AIHeroClient;

                    if (target != null)
                    {
                        if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady())
                        {
                            Cast_E(target);
                        }
                        else if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
                        {
                            Q.Cast(target, true);
                        }
                        else if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady())
                        {
                            W.Cast(target.Position, true);
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (!ManaManager.HasMana("JungleClear"))
                        return;

                    var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);

                    if (mobs.Any())
                    {
                        var mob = mobs.FirstOrDefault();

                        if (Menu.Item("UseEJungle", true).GetValue<bool>() && E.IsReady())
                        {
                            var ex = Player.Position.Extend(Game.CursorPos, 150);

                            E.Cast(ex, true);
                        }
                        else if (Menu.Item("UseQJungle", true).GetValue<bool>() && Q.IsReady())
                        {
                            Q.Cast(mob, true);
                        }
                        else if (Menu.Item("UseWJungle", true).GetValue<bool>() && W.IsReady())
                        {
                            W.Cast(mob, true);
                        }
                    }
                }
            }
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Player.IsChannelingImportantSpell())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                return;
            }

            SmartKs();

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
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
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

            if (Menu.Item("UseQExtendCombo", true).GetValue<bool>() && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                if (!target.IsValidTarget(Q.Range) && target.IsValidTarget(QExtend.Range))
                {
                    var pred = QExtend.GetPrediction(target, true);
                    var collisions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                    if (!collisions.Any() || (!target.IsMoving && Menu.Item("MovementCheck", true).GetValue<bool>()))
                        return;

                    foreach (var minion in collisions)
                    {
                        var poly = new Geometry.Polygon.Rectangle(Player.ServerPosition, Player.ServerPosition.Extend(minion.ServerPosition, QExtend.Range), QExtend.Width);

                        if (poly.IsInside(pred.UnitPosition))
                        {
                            Q.Cast(minion);
                        }
                    }
                }
            }

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                if (target.IsValidTarget(R.Range) && !target.IsZombie && !target.IsDead &&
                    R.GetDamage(target)*GetShots() > target.Health &&
                    target.Distance(Player) > Orbwalking.GetAttackRange(Player) + 150)
                {
                    R.Cast(target);
                }
            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
            {
                return;
            }

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(QExtend.Range, TargetSelector.DamageType.Physical);

                if (target != null)
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }
                    else if (target.IsValidTarget(QExtend.Range) && Menu.Item("UseQExtendHarass", true).GetValue<bool>())
                    {
                        var pred = QExtend.GetPrediction(target, true);
                        var collisions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                        if (!collisions.Any() || (!target.IsMoving && Menu.Item("MovementCheck", true).GetValue<bool>()))
                            return;

                        foreach (var minion in collisions)
                        {
                            var poly = new Geometry.Polygon.Rectangle(Player.ServerPosition, Player.ServerPosition.Extend(minion.ServerPosition, QExtend.Range), QExtend.Width);

                            if (poly.IsInside(pred.UnitPosition))
                            {
                                Q.Cast(minion);
                            }
                        }
                    }
                }
            }

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady())
            {
                SpellCastManager.CastBasicSkillShot(W, W.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
            }
        }

        private void Cast_E(AIHeroClient target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var castpos = Player.ServerPosition.Extend(Game.CursorPos, 220);
            var maxepos = Player.ServerPosition.Extend(Game.CursorPos, E.Range);

            if (castpos.UnderTurret(true) && Menu.Item("underE", true).GetValue<bool>())
            {
                return;
            }

            if ((NavMesh.GetCollisionFlags(castpos).HasFlag(CollisionFlags.Wall) ||
                NavMesh.GetCollisionFlags(castpos).HasFlag(CollisionFlags.Building)) &&
                Menu.Item("ECheck", true).GetValue<bool>())
            {
                return;
            }

            if (castpos.CountEnemiesInRange(500) >= 3 && castpos.CountAlliesInRange(400) < 3 &&
                Menu.Item("SafeCheck", true).GetValue<bool>())
            {
                return;
            }

            if (Orbwalker.InAutoAttackRange(target) &&
                target.ServerPosition.Distance(castpos) < Orbwalking.GetAttackRange(Player))
            {
                E.Cast(castpos, true);
            }
            else if (!Orbwalker.InAutoAttackRange(target) && target.ServerPosition.Distance(castpos) > Orbwalking.GetAttackRange(Player))
            {
                E.Cast(castpos, true);
            }
            else if (!Orbwalker.InAutoAttackRange(target) &&
                     target.ServerPosition.Distance(castpos) > Orbwalking.GetAttackRange(Player) &&
                     target.ServerPosition.Distance(maxepos) < Orbwalking.GetAttackRange(Player))
            {
                E.Cast(maxepos, true);
            }
        }

        private void SmartKs()
        {
            if (!Menu.Item("smartKS", true).GetValue<bool>())
                return;

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(QExtend.Range) && !x.IsDead && !x.HasBuffOfType(BuffType.Invulnerability)))
            {
                if (Q.IsKillable(target) && Player.Distance(target.Position) < QExtend.Range && Q.IsReady())
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }
                    else
                    {
                        var pred = QExtend.GetPrediction(target, true);
                        var collisions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                        if (!collisions.Any() || (!target.IsMoving && Menu.Item("MovementCheck", true).GetValue<bool>()))
                            return;

                        foreach (var minion in collisions)
                        {
                            var poly = new Geometry.Polygon.Rectangle(Player.ServerPosition, Player.ServerPosition.Extend(minion.ServerPosition, QExtend.Range), QExtend.Width);

                            if (poly.IsInside(pred.UnitPosition))
                            {
                                if (Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                {
                                    Q.LastCastAttemptT = Utils.TickCount;
                                    return;
                                }
                            }
                        }
                    }
                }

                if (W.IsKillable(target) && Player.Distance(target.Position) < W.Range && W.IsReady())
                {
                    W.Cast(target);
                }
            }
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();

            if (useQ && Utils.TickCount - CastSpellTime >= 400)
            {
                var allMinions = MinionManager.GetMinions(Player.Position, Q.Range);

                if (allMinions.Any())
                {
                    var minion = allMinions.FirstOrDefault();

                    if (minion != null)
                    {
                        var qExminions = MinionManager.GetMinions(Player.Position, 900);

                        if (QExtend.CountHits(allMinions, Player.Position.Extend(minion.Position, 900)) >= 2)
                        {
                            Q.CastOnUnit(minion);
                        }
                    }
                }
            }
            if (useW && Utils.TickCount - CastSpellTime >= 400)
            {
                var allMinionE = MinionManager.GetMinions(Player.ServerPosition, W.Range);

                if (allMinionE.Count > 2)
                {
                    var pred = W.GetCircularFarmLocation(allMinionE);

                    W.Cast(pred.Position);
                }
            }
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (!unit.IsMe)
                return;

            var castedSlot = ObjectManager.Player.GetSpellSlot(args.SData.Name);

            if (castedSlot == SpellSlot.Q || castedSlot == SpellSlot.W || castedSlot == SpellSlot.E)
            {
                CastSpellTime = Utils.TickCount;
            }
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q) * 2;

            if (W.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.W);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R) * GetShots();

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 2);
        }

        private double GetShots()
        {
            double shots = 0;

            if (R.Level != 0)
                shots = 7.5 + 7.5 * Player.AttackSpeedMod;

            return shots;
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("Draw_Disabled", true).GetValue<bool>())
                return;

            if (Menu.Item("Draw_Q", true).GetValue<bool>())
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_Q_Extended", true).GetValue<bool>())
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, QExtend.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_W", true).GetValue<bool>())
                if (W.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_E", true).GetValue<bool>())
                if (E.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R", true).GetValue<bool>())
                if (R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
        }
    }
}