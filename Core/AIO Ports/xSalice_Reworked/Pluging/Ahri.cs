using EloBuddy; 
using LeagueSharp.Common; 
 namespace xSaliceResurrected_Rework.Pluging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Base;
    using Managers;
    using Utilities;
    using Color = System.Drawing.Color;

    internal class Ahri : Champion
    {
        private static bool _rOn;
        private static int _rTimer;
        private static int _rTimeLeft;

        public Ahri()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 900f);
            SpellManager.W = new Spell(SpellSlot.W, 600f);
            SpellManager.E = new Spell(SpellSlot.E, 950f);
            SpellManager.R = new Spell(SpellSlot.R, 850f);

            SpellManager.Q.SetSkillshot(0.25f, 90f, 1550f, false, SkillshotType.SkillshotLine);
            SpellManager.E.SetSkillshot(0.25f, 60f, 1550f, true, SkillshotType.SkillshotLine);

            SpellManager.SpellList.Add(Q);
            SpellManager.SpellList.Add(W);
            SpellManager.SpellList.Add(E);
            SpellManager.SpellList.Add(R);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                combo.AddItem(new MenuItem("rSpeed", "Use All R fast Duel", true).SetValue(true));
                combo.AddItem(new MenuItem("charmCombo", "Q if Charmed in Combo", true).SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Toggle)));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(new MenuItem("longQ", "Cast Long range Q", true).SetValue(true));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 60);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("Farm", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(false));
                farm.AddItem(new MenuItem("UseWFarm", "Use W", true).SetValue(false));
                ManaManager.AddManaManagertoMenu(farm, "Farm", 50);
                Menu.AddSubMenu(farm);
            }

            var misc = new Menu("Misc", "Misc");
            {
                misc.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(true, false, false, false));
                misc.AddItem(new MenuItem("UseInt", "Use E to Interrupt", true).SetValue(true));
                misc.AddItem(new MenuItem("UseGap", "Use E for GapCloser", true).SetValue(true));
                misc.AddItem(new MenuItem("EQ", "Use Q onTop of E", true).SetValue(true));
                misc.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
                Menu.AddSubMenu(misc);
            }

            var drawing = new Menu("Drawings", "Drawings");
            {
                drawing.AddItem(
                        new MenuItem("QRange", "Q range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(
                        new MenuItem("WRange", "W range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(
                        new MenuItem("ERange", "E range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(
                        new MenuItem("RRange", "R range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(
                        new MenuItem("cursor", "Draw R Dash Range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));

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
                customMenu.AddItem(myCust.AddToMenu("Require Charm: ", "charmCombo"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            if (enemy == null)
                return 0;

            var damage = 0d;

            if (Q.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q, 1);
            }
            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (_rOn)
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * RCount();
            else if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 3;

            damage = ItemManager.CalcDamage(enemy, damage);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            return (float)damage;
        }

        private void Combo()
        {
            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var rETarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            var dmg = GetComboDamage(eTarget);

            if (eTarget == null)
                return;

            ItemManager.Target = eTarget;

            if (dmg > eTarget.Health - 50)
                ItemManager.KillableTarget = true;

            ItemManager.UseTargetted = true;

            //E
            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady() && Player.Distance(eTarget.Position) < E.Range)
            {
                SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
                if (Menu.Item("EQ", true).GetValue<bool>() && Q.IsReady() && !E.IsReady())
                {
                    SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
                }
            }

            //W
            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady() && Player.Distance(eTarget.Position) <= W.Range - 100 &&
                ShouldW(eTarget))
            {
                W.Cast();
            }

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady() && Player.Distance(eTarget.Position) <= Q.Range &&
                     ShouldQ(eTarget))
            {
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
            }

            //R
            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady() && Player.Distance(eTarget.Position) < R.Range)
            {
                if (E.IsReady())
                {
                    if (CheckReq(rETarget))
                        SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
                }
                if (ShouldR(eTarget, dmg) && R.IsReady())
                {
                    R.Cast(Game.CursorPos);
                    _rTimer = Utils.TickCount - 250;
                }
                if (_rTimeLeft > 9500 && _rOn && R.IsReady())
                {
                    R.Cast(Game.CursorPos);
                    _rTimer = Utils.TickCount - 250;
                }
            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var rETarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            var dmg = GetComboDamage(eTarget);

            if (eTarget == null)
                return;

            //E
            if (Menu.Item("UseEHarass", true).GetValue<bool>() && E.IsReady() && Player.Distance(eTarget.Position) < E.Range)
            {
                SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
                if (Menu.Item("EQ", true).GetValue<bool>() && Q.IsReady() && !E.IsReady())
                {
                    SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
                }
            }

            //W
            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady() && Player.Distance(eTarget.Position) <= W.Range - 100)
            {
                W.Cast();
            }

            if (Menu.Item("longQ", true).GetValue<bool>())
            {
                if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() && Player.Distance(eTarget.Position) <= Q.Range
                    && Player.Distance(eTarget.Position) > 600)
                {
                    SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
                }
            }
        }

        private void CheckKs()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(1300) && !x.IsDead).OrderByDescending(GetComboDamage))
            {
                if (target != null)
                {
                    if (Player.Distance(target.ServerPosition) <= W.Range &&
                        Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1) +
                        Player.GetSpellDamage(target, SpellSlot.W) > target.Health && Q.IsReady() && Q.IsReady())
                    {
                        Q.Cast(target);
                        return;
                    }

                    if (Player.Distance(target.ServerPosition) <= Q.Range &&
                        Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1) >
                        target.Health && Q.IsReady())
                    {
                        Q.Cast(target);
                        return;
                    }

                    if (Player.Distance(target.ServerPosition) <= E.Range &&
                        Player.GetSpellDamage(target, SpellSlot.E) > target.Health & E.IsReady())
                    {
                        E.Cast(target);
                        return;
                    }

                    if (Player.Distance(target.ServerPosition) <= W.Range &&
                        Player.GetSpellDamage(target, SpellSlot.W) > target.Health && W.IsReady())
                    {
                        W.Cast();
                        return;
                    }

                    var dashVector = Player.Position +
                                         Vector3.Normalize(target.ServerPosition - Player.Position) * 425;
                    if (Player.Distance(target.ServerPosition) <= R.Range &&
                        Player.GetSpellDamage(target, SpellSlot.R) > target.Health && R.IsReady() && _rOn &&
                        target.Distance(dashVector) < 425 && R.IsReady())
                    {
                        R.Cast(dashVector);
                    }
                }
            }
        }

        private bool ShouldQ(AIHeroClient target)
        {
            if (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.Q, 1) >
                target.Health)
                return true;

            if (_rOn)
                return true;

            if (!Menu.Item("charmCombo", true).GetValue<KeyBind>().Active)
                return true;

            return target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Taunt);
        }

        private bool ShouldW(AIHeroClient target)
        {
            if (Player.GetSpellDamage(target, SpellSlot.W) > target.Health)
                return true;

            if (_rOn)
                return true;

            if (Player.Mana > ESpell.SData.Mana + QSpell.SData.Mana)
                return true;

            if (!Menu.Item("charmCombo", true).GetValue<KeyBind>().Active)
                return true;

            return target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Taunt);
        }

        private bool ShouldR(AIHeroClient target, float dmg)
        {
            var dashVector = Player.ServerPosition + Vector3.Normalize(Game.CursorPos - Player.ServerPosition) * 425;

            if (Player.Distance(Game.CursorPos) < 475)
                dashVector = Game.CursorPos;

            if (target.Distance(dashVector) > 525)
                return false;

            if (Menu.Item("rSpeed", true).GetValue<bool>() && Game.CursorPos.CountEnemiesInRange(1500) < 3 && dmg > target.Health - 100)
                return true;

            if (Player.GetSpellDamage(target, SpellSlot.R) * RCount() > target.Health)
                return true;

            return _rOn && _rTimeLeft > 9500;
        }

        private bool CheckReq(AIHeroClient target)
        {
            if (Player.Distance(Game.CursorPos) < 75)
                return false;

            if (GetComboDamage(target) > target.Health && !_rOn && Game.CursorPos.CountEnemiesInRange(1500) < 3)
            {
                if (target.Distance(Game.CursorPos) <= E.Range && E.IsReady())
                {
                    var dashVector = Player.Position + Vector3.Normalize(Game.CursorPos - Player.Position) * 425;
                    var addedDelay = Player.Distance(dashVector) / 2200;
                    var pred = xSaliceResurrected_Rework.Prediction.CommonPredEx.GetP(Game.CursorPos, E, target, addedDelay, false);

                    if (pred.Hitchance >= HitChance.Medium && R.IsReady())
                    {
                        R.Cast(Game.CursorPos);
                        _rTimer = Utils.TickCount - 250;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsRActive()
        {
            return Player.HasBuff("AhriTumble");
        }

        private int RCount()
        {
            var buff = Player.Buffs.FirstOrDefault(x => x.Name == "AhriTumble");

            return buff?.Count ?? 0;
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("Farm"))
                return;

            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();

            if (useQ && Q.IsReady())
            {
                var qPos = Q.GetLineFarmLocation(allMinionsQ);
                if (qPos.MinionsHit >= 3)
                {
                    Q.Cast(qPos.Position);
                }
            }

            if (useW && allMinionsW.Count > 0 && W.IsReady())
                W.Cast();
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            _rOn = IsRActive();

            if (_rOn)
                _rTimeLeft = Utils.TickCount - _rTimer;

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

        protected override void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Menu.Item(spell.Slot + "Range", true).GetValue<Circle>();
                if (menuItem.Active)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }

            if (Menu.Item("cursor", true).GetValue<Circle>().Active)
                Render.Circle.DrawCircle(Player.Position, 475, Color.Aquamarine);
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("UseGap", true).GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range))
                E.Cast(gapcloser.Sender);
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("UseInt", true).GetValue<bool>()) return;

            if (Player.Distance(unit.Position) < E.Range)
            {
                if (E.GetPrediction(unit).Hitchance >= HitChance.Medium && E.IsReady())
                    E.Cast(unit);
            }
        }
    }
}
