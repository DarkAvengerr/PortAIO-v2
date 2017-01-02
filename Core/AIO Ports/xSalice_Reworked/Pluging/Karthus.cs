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
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;

    internal class Karthus : Champion
    {
        private const int QWidth = 200;
        private static int _lastNotification;
        private int _lastE;

        public Karthus()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 875);
            SpellManager.W = new Spell(SpellSlot.W, 1000);
            SpellManager.E = new Spell(SpellSlot.E, 520);
            SpellManager.R = new Spell(SpellSlot.R);

            SpellManager.Q.SetSkillshot(.5f, 190f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.W.SetSkillshot(0.25f, 50f, 1600f, false, SkillshotType.SkillshotCircle);
            SpellManager.R.SetSkillshot(3f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellList.Add(SpellManager.Q);
            SpellList.Add(SpellManager.W);
            SpellList.Add(SpellManager.E);
            SpellList.Add(SpellManager.R);

            var spells = new Menu("Spell", "Spell");
            {
                var qMenu = new Menu("QSpell", "QSpell");
                {
                    qMenu.AddItem(new MenuItem("qImmo", "Auto Q Immobile", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("qDash", "Auto Q Dashing", true).SetValue(true));
                    spells.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("WSpell", "WSpell");
                {
                    wMenu.AddItem(new MenuItem("wTower", "Auto W Enemy in Tower", true).SetValue(true));
                    ManaManager.AddManaManagertoMenu(wMenu, "WMana", 30);
                    spells.AddSubMenu(wMenu);
                }

                var eMenu = new Menu("ESpell", "ESpell");
                {
                    eMenu.AddItem(new MenuItem("eManaCombo", "Min Mana Combo", true).SetValue(new Slider(10)));
                    eMenu.AddItem(new MenuItem("eManaHarass", "Min Mana Harass", true).SetValue(new Slider(70)));
                    eMenu.AddItem(new MenuItem("EDelay", "E Delay Before Turning Off (Milliseconds)", true).SetValue(new Slider(100, 0, 2000)));
                    spells.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("RSpell", "RSpell");
                {
                    rMenu.AddItem(new MenuItem("rPing", "Ping if Enemy Is Killable", true).SetValue(true));
                    spells.AddSubMenu(rMenu);
                }

                Menu.AddSubMenu(spells);
            }

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 30);
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("Farm", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(false));
                farm.AddItem(new MenuItem("UseEFarm", "Use E", true).SetValue(false));
                ManaManager.AddManaManagertoMenu(harass, "Farm", 30);
                Menu.AddSubMenu(farm);
            }

            var misc = new Menu("Misc", "Misc");
            {
                misc.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(true, false, false, false));
                misc.AddItem(new MenuItem("wTar", "Cast W On Selected", true).SetValue(new KeyBind("W".ToCharArray()[0], KeyBindType.Press)));
                misc.AddItem(new MenuItem("UseGap", "Use W for GapCloser", true).SetValue(true));
                misc.AddItem(new MenuItem("smartKS", "Use Smart KS System", true).SetValue(true));
                Menu.AddSubMenu(misc);
            }

            var drawMenu = new Menu("Drawings", "Drawings");
            {
                drawMenu.AddItem(new MenuItem("QRange", "Q range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawMenu.AddItem(new MenuItem("WRange", "W range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawMenu.AddItem(new MenuItem("ERange", "E range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawMenu.AddItem(new MenuItem("RRange", "R range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawMenu.AddItem(new MenuItem("drawUlt", "Killable With ult", true).SetValue(true));

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
                customMenu.AddItem(myCust.AddToMenu("W Target Active: ", "wTar"));
                customMenu.AddItem(myCust.AddToMenu("Lasthit Q Active: ", "LastHit"));
                Menu.AddSubMenu(customMenu);
            }
        }


        private float GetComboDamage(Obj_AI_Base enemy)
        {
            if (!enemy.IsValidTarget())
                return 0;

            var damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q) * 2;

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E) * 2;

            if (R.IsReady())
                damage += GetUltDmg((AIHeroClient)enemy);

            damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }

        private void Combo()
        {
            var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

            if (itemTarget != null)
            {
                ItemManager.Target = itemTarget;

                var dmg = GetComboDamage(itemTarget);

                if (dmg > itemTarget.Health - 50)
                    ItemManager.KillableTarget = true;

                ItemManager.UseTargetted = true;
            }

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady())
            {
                if (ShouldW())
                    SpellCastManager.CastBasicSkillShot(W, W.Range, TargetSelector.DamageType.Magical,
                        HitChance.VeryHigh);
            }

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
            {
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical,
                    HitChance.VeryHigh);
            }

            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady() && ESpell.ToggleState == 1 && HasManaForE("Combo") &&
                Utils.TickCount - E.LastCastAttemptT > 500)
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                if (!target.IsValidTarget(E.Range))
                    return;

                E.Cast();
            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady())
            {
                if (ShouldW())
                    SpellCastManager.CastBasicSkillShot(W, W.Range, TargetSelector.DamageType.Magical,
                        HitChance.VeryHigh);
            }

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady())
            {
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical,
                    HitChance.VeryHigh);
            }

            if (Menu.Item("UseEHarass", true).GetValue<bool>() && E.IsReady() && ESpell.ToggleState == 1 && HasManaForE("Harass") &&
                Utils.TickCount - E.LastCastAttemptT > 500)
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                if (!target.IsValidTarget(E.Range))
                    return;

                E.Cast();
            }
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (!unit.IsMe)
                return;

            var castedSlot = ObjectManager.Player.GetSpellSlot(args.SData.Name);

            if (castedSlot == SpellSlot.E)
            {
                E.LastCastAttemptT = Utils.TickCount;
            }
        }

        private bool ShouldW()
        {
            return ManaManager.HasMana("WMana");
        }

        private float GetUltDmg(AIHeroClient target)
        {
            double dmg = 0;

            dmg += Player.GetSpellDamage(target, SpellSlot.R);

            dmg -= target.HPRegenRate * 3.25;

            if (Items.HasItem(3155, target))
            {
                dmg = dmg - 250;
            }

            if (Items.HasItem(3156, target))
            {
                dmg = dmg - 400;
            }

            return (float)dmg;
        }

        private void DrawEnemyKillable()
        {
            var kill = 0;

            foreach (
                var enemy in
                    HeroManager.Enemies.Where(
                            x => x.IsValidTarget()))
            {
                if (GetUltDmg(enemy) > enemy.Health - 30)
                {
                    if (Menu.Item("rPing", true).GetValue<bool>() && Utils.TickCount - _lastNotification > 5000)
                    {
                        if (Utils.TickCount - _lastNotification > 0)
                        {
                            Notifications.AddNotification(enemy.BaseSkinName + " Is Killable!", 500);
                            _lastNotification = Utils.TickCount + 5000;
                        }

                    }
                    kill++;
                }
            }

            if (kill > 0)
            {
                var wts = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(wts[0] - 100, wts[1], Color.Red, "Killable with R: " + kill);
            }
            else
            {
                var wts = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(wts[0] - 100, wts[1], Color.White, "Killable with R: " + kill);
            }
        }


        private bool HasManaForE(string source)
        {
            var eManaCombo = Menu.Item("eManaCombo", true).GetValue<Slider>().Value;
            var eManaHarass = Menu.Item("eManaHarass", true).GetValue<Slider>().Value;

            if (source == "Combo" && Player.ManaPercent > eManaCombo)
                return true;

            return source == "Harass" && Player.ManaPercent > eManaHarass;
        }

        private void AutoQ()
        {
            var qDashing = Menu.Item("qImmo", true).GetValue<bool>();
            var qImmo = Menu.Item("qDash", true).GetValue<bool>();

            if (!Q.IsReady())
                return;

            if (!qDashing && !qImmo)
                return;

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(Q.Range)))
            {
                if ((Q.GetPrediction(target).Hitchance == HitChance.Immobile || IsStunned(target)) && qImmo && Player.Distance(target.Position) < Q.Range)
                {
                    SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
                    return;
                }

                if (Q.GetPrediction(target).Hitchance == HitChance.Dashing && qDashing && Player.Distance(target.Position) < Q.Range)
                {
                    SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
                }
            }
        }

        private bool IsStunned(Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                   target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt);
        }

        private void CheckUnderTower()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => Player.Distance(x.Position) < W.Range && x.IsValidTarget(W.Range) && !x.IsDead && x.IsVisible))
            {
                if (ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret != null && turret.IsValid &&
                turret.IsAlly && turret.Health > 0).Any(turret => Vector2.Distance(enemy.Position.To2D(), turret.Position.To2D()) < 750 && W.IsReady()))
                {
                    var vec = enemy.ServerPosition +
                              Vector3.Normalize(enemy.ServerPosition - Player.ServerPosition) * 100;

                    W.Cast(vec);
                    return;
                }
            }
        }

        private void SmartKs()
        {
            if (!Menu.Item("smartKS", true).GetValue<bool>())
                return;

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.HasBuffOfType(BuffType.Invulnerability))
                .OrderByDescending(GetComboDamage))
            {
                if (Player.Distance(target.ServerPosition) <= Q.Range &&
                    Player.GetSpellDamage(target, SpellSlot.Q) > target.Health + 30)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(target);
                        return;
                    }
                }

                if (Player.Distance(target.ServerPosition) <= E.Range && ESpell.ToggleState == 1 &&
                    Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 30)
                {
                    if (E.IsReady())
                    {
                        E.Cast();
                        return;
                    }
                }
            }
        }

        private void CheckEState()
        {
            if (ESpell.ToggleState == 1 || Utils.TickCount - _lastE < Menu.Item("EDelay", true).GetValue<Slider>().Value)
                return;

            var target = ObjectManager.Get<AIHeroClient>().Count(x => x.IsValidTarget(E.Range));

            if (target > 0)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.NotAlly);

                if (allMinionsE.Count > 0)
                    return;
            }

            if (Utils.TickCount - (_lastE + 250) < Menu.Item("EDelay", true).GetValue<Slider>().Value)
                E.Cast();

            if (E.IsReady() && ESpell.ToggleState != 1)
            {
                _lastE = Utils.TickCount;
            }
        }


        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsChannelingImportantSpell())
                return;

            SmartKs();

            AutoQ();

            CheckEState();

            if (Menu.Item("wTower", true).GetValue<bool>())
                CheckUnderTower();

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
                    LastHitQ();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Farm();
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("wTar", true).GetValue<KeyBind>().Active)
                    {
                        AIHeroClient target = null;

                        if (TargetSelector.GetSelectedTarget() != null)
                            target = TargetSelector.GetSelectedTarget();

                        if (target != null && target.IsEnemy && target.Type == GameObjectType.AIHeroClient)
                        {
                            if (W.GetPrediction(target).Hitchance >= HitChance.High)
                                W.Cast(target);
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LastHitQ()
        {
            if (!Q.IsReady())
                return;

            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (allMinionsQ.Count > 0)
            {
                foreach (var minion in allMinionsQ)
                {
                    var health = HealthPrediction.GetHealthPrediction(minion, 700);

                    var qPred = Q.GetCircularFarmLocation(allMinionsQ, 210);

                    if (qPred.MinionsHit == 1)
                    {
                        if (Player.GetSpellDamage(minion, SpellSlot.Q) - 15 > health)
                            Q.Cast(minion);
                    }
                    else
                    {
                        if (Player.GetSpellDamage(minion, SpellSlot.Q, 1) - 15 > health)
                            Q.Cast(minion);
                    }
                }
            }
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("Farm"))
                return;

            var allMinionsE = MinionManager.GetMinions(Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();

            if (useQ && Q.IsReady())
            {
                SpellCastManager.CastBasicFarm(Q);
            }

            if (useE && allMinionsE.Count > 0 && E.IsReady() && ESpell.ToggleState == 1)
            {
                var ePos = E.GetCircularFarmLocation(allMinionsE);

                if (ePos.MinionsHit > 1)
                    E.Cast();
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

            if (R.IsReady() && Menu.Item("drawUlt", true).GetValue<bool>())
                DrawEnemyKillable();
        }


        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("UseGap", true).GetValue<bool>()) return;

            if (W.IsReady() && gapcloser.Sender.IsValidTarget(W.Range))
                W.Cast(gapcloser.Sender);
        }
    }
}
