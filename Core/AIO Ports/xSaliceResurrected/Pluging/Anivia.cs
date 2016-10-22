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
    using Color = System.Drawing.Color;
    using Managers;
    using Utilities;

    internal class Anivia : Champion
    {
        private GameObject _qMissle;
        private bool _eCasted;
        private GameObject _rObj;
        private bool _rFirstCreated;
        private bool _rByMe;

        public Anivia()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 1000f);
            SpellManager.W = new Spell(SpellSlot.W, 950f);
            SpellManager.E = new Spell(SpellSlot.E, 650f);
            SpellManager.R = new Spell(SpellSlot.R, 700f);

            SpellManager.Q.SetSkillshot(0.25f, 110f, 870f, false, SkillshotType.SkillshotLine);
            SpellManager.W.SetSkillshot(0.25f, 1.0f, float.MaxValue, false, SkillshotType.SkillshotLine);
            SpellManager.R.SetSkillshot(0.25f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellManager.SpellList.Add(Q);
            SpellManager.SpellList.Add(W);
            SpellManager.SpellList.Add(E);
            SpellManager.SpellList.Add(R);

            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("selected", "Focus Selected Target", true).SetValue(true));
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseEComboOnly", "Use E| Only Dounle Cast", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                Menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(new MenuItem("UseRHarass", "Use R", true).SetValue(true));
                harass.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                Menu.AddSubMenu(harass);
            }

            var farm = new Menu("Farm", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(false));
                farm.AddItem(new MenuItem("UseEFarm", "Use E", true).SetValue(false));
                farm.AddItem(new MenuItem("UseRFarm", "Use R", true).SetValue(false));
                Menu.AddSubMenu(farm);
            }

            var misc = new Menu("Misc", "Misc");
            {
                misc.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(true, false, false, false));
                misc.AddItem(new MenuItem("snipe", "W/Q Snipe", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                misc.AddItem(new MenuItem("UseInt", "Use Spells to Interrupt", true).SetValue(true));
                misc.AddItem(new MenuItem("detonateQ", "Auto Detonate Q", true).SetValue(true));
                misc.AddItem(new MenuItem("detonateQ2", "Pop Q Behind Enemy", true).SetValue(true));
                misc.AddItem(new MenuItem("wallKill", "Wall Enemy on killable", true).SetValue(true));
                misc.AddItem(new MenuItem("UseGap", "Use W for GapCloser", true).SetValue(true));
                misc.AddItem(new MenuItem("checkR", "Auto turn off R", true).SetValue(true));
                misc.AddItem(new MenuItem("smartKS", "Use Smart KS System", true).SetValue(true));
                Menu.AddSubMenu(misc);
            }

            var draw = new Menu("Drawings", "Drawings");
            {
                draw.AddItem(new MenuItem("QRange", "Q range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("WRange", "W range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("ERange", "E range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.AddItem(new MenuItem("RRange", "R range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));

                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                var drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                draw.AddItem(drawComboDamageMenu);
                draw.AddItem(drawFill);
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
                customMenu.AddItem(myCust.AddToMenu("Snipe Active: ", "snipe"));
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

            if (E.IsReady() & (Q.IsReady() || R.IsReady()))
                damage += Player.GetSpellDamage(enemy, SpellSlot.E) * 2;
            else if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 3;

            damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }

        private void Combo()
        {
            var range = Q.IsReady() ? Q.Range : E.Range;
            var focusSelected = Menu.Item("selected", true).GetValue<bool>();
            var target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Magical);

            if (TargetSelector.GetSelectedTarget() != null)
            {
                if (focusSelected && TargetSelector.GetSelectedTarget().Distance(Player.ServerPosition) < range)
                {
                    target = TargetSelector.GetSelectedTarget();
                }
            }

            if (target == null)
                return;

            var dmg = GetComboDamage(target);

            var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);

            if (itemTarget != null)
            {
                ItemManager.Target = itemTarget;

                if (dmg > itemTarget.Health - 50)
                    ItemManager.KillableTarget = true;

                ItemManager.UseTargetted = true;
            }

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady() &&
                target.IsValidTarget(R.Range))
            {
                if (ShouldR(true))
                {
                    R.Cast(target.Position, true);
                }
            }

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady() &&
                target.IsValidTarget(Q.Range) && ShouldQ())
            {
                var QPred = Q.GetPrediction(target);

                if (QPred.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(QPred.CastPosition, true);
                }
            }

            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady() &&
                target.IsValidTarget(E.Range) && ShouldE(target))
            {
                E.CastOnUnit(target, true);
            }

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady() &&
                target.IsValidTarget(W.Range) && ShouldUseW(target))
            {
                CastW(target);
            }
        }

        private void Harass()
        {
            var range = Q.IsReady() && Menu.Item("UseQHarass", true).GetValue<bool>() ? Q.Range : E.Range;
            var target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Magical);

            if (target.IsValidTarget(range))
            {
                if (Menu.Item("UseRHarass", true).GetValue<bool>() && R.IsReady() &&
                    target.IsValidTarget(R.Range))
                {
                    R.Cast(target.Position, true);
                }

                if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady() &&
                    target.IsValidTarget(Q.Range) && ShouldQ())
                {
                    var Qpred = Q.GetPrediction(target);

                    if (Qpred.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(Qpred.CastPosition, true);
                    }
                }

                if (Menu.Item("UseEHarass", true).GetValue<bool>() && E.IsReady() &&
                    target.IsValidTarget(E.Range) && target.HasBuff("chilled"))
                {
                    E.CastOnUnit(target, true);
                }
            }
        }

        private void CastR(AIHeroClient target)
        {
            var Rpred = Prediction.GetPrediction(new PredictionInput
            {
                Unit = target,
                Delay = R.Delay,
                Radius = R.Width,
                Speed = R.Speed,
                Range = R.Range,
                Collision = false,
                Type = SkillshotType.SkillshotCircle,
                Aoe = true,
            });

            if (Rpred.Hitchance >= HitChance.VeryHigh)
            {
                R.Cast(Rpred.CastPosition, true);
            }
        }

        private void SmartKs()
        {
            if (!Menu.Item("smartKS", true).GetValue<bool>())
                return;

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(1300)))
            {
                //ER
                if (Player.Distance(target.ServerPosition) <= R.Range && !_rFirstCreated &&
                    Player.GetSpellDamage(target, SpellSlot.R) + Player.GetSpellDamage(target, SpellSlot.E) * 2 >
                    target.Health + 50)
                {
                    if (R.IsReady() && E.IsReady())
                    {
                        E.CastOnUnit(target);
                        R.CastOnUnit(target);
                        return;
                    }
                }

                //QR
                if (Player.Distance(target.ServerPosition) <= R.Range && ShouldQ() &&
                    Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.R) >
                    target.Health + 30)
                {
                    if (W.IsReady() && R.IsReady())
                    {
                        W.Cast(target);
                        return;
                    }
                }

                //Q
                if (Player.Distance(target.ServerPosition) <= Q.Range && ShouldQ() &&
                    Player.GetSpellDamage(target, SpellSlot.Q) > target.Health + 30)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(target);
                        return;
                    }
                }

                //E
                if (Player.Distance(target.ServerPosition) <= E.Range &&
                    Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 30)
                {
                    if (E.IsReady())
                    {
                        E.CastOnUnit(target);
                        return;
                    }
                }
            }
        }

        private bool ShouldQ()
        {
            return Environment.TickCount - Q.LastCastAttemptT > 2000;
        }

        private bool ShouldR(bool isCombo)
        {
            if (_rFirstCreated)
            {
                return false;
            }
            if (_rByMe)
            {
                return false;
            }

            return _eCasted || isCombo;
        }

        private bool ShouldE(AIHeroClient target)
        {
            if (Menu.Item("UseEComboOnly", true).GetValue<bool>() && checkChilled(target))
                return true;

            if (Player.GetSpellDamage(target, SpellSlot.E) >= target.Health)
                return true;

            return !Menu.Item("UseEComboOnly", true).GetValue<bool>();
        }

        private bool ShouldUseW(AIHeroClient target)
        {
            if (GetComboDamage(target) >= target.Health - 20 && Menu.Item("wallKill", true).GetValue<bool>())
                return true;

            if (_rFirstCreated && _rObj != null)
            {
                if (_rObj.Position.Distance(target.ServerPosition) > 300)
                {
                    return true;
                }
            }

            return false;
        }

        private void CastW(AIHeroClient target)
        {
            var pred = W.GetPrediction(target);
            var vec = new Vector3(pred.CastPosition.X - Player.ServerPosition.X, 0,
                pred.CastPosition.Z - Player.ServerPosition.Z);

            var castBehind = pred.CastPosition + Vector3.Normalize(vec) * 125;

            if (W.IsReady())
                W.Cast(castBehind);
        }

        private void CastWEscape(AIHeroClient target)
        {
            var pred = W.GetPrediction(target);
            var vec = new Vector3(pred.CastPosition.X - Player.ServerPosition.X, 0,
                pred.CastPosition.Z - Player.ServerPosition.Z);

            var castBehind = pred.CastPosition - Vector3.Normalize(vec) * 125;

            if (W.IsReady())
                W.Cast(castBehind);
        }

        private bool checkChilled(AIHeroClient target)
        {
            return target.HasBuff("chilled");
        }

        private void DetonateQ()
        {
            if (_qMissle == null || !Q.IsReady())
                return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(1200) && x.Distance(_qMissle.Position) < 200).OrderByDescending(GetComboDamage))
            {
                if (ShouldDetonate(enemy) && Environment.TickCount - Q.LastCastAttemptT > Game.Ping)
                {
                    Q.Cast(_qMissle.Position);
                }
            }
        }

        private bool ShouldDetonate(AIHeroClient target)
        {
            if (Menu.Item("detonateQ2", true).GetValue<bool>())
            {
                if (target.Distance(_qMissle.Position) < Q.Width + target.BoundingRadius && checkChilled(target))
                    return true;
            }

            return target.Distance(_qMissle.Position) < Q.Width + target.BoundingRadius;
        }

        private void Snipe()
        {
            var range = Q.Range;
            var focusSelected = Menu.Item("selected", true).GetValue<bool>();
            var qTarget = TargetSelector.GetTarget(range, TargetSelector.DamageType.Magical);

            if (TargetSelector.GetSelectedTarget() != null)
                if (focusSelected && TargetSelector.GetSelectedTarget().Distance(Player.ServerPosition) < range)
                    qTarget = TargetSelector.GetSelectedTarget();

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (qTarget == null)
                return;

            if (W.IsReady() && Q.IsReady() && Player.Distance(qTarget.ServerPosition) < W.Range)
                CastW(qTarget);

            if (!W.IsReady() && Q.IsReady() && Player.Distance(qTarget.ServerPosition) < Q.Range &&
                Q.GetPrediction(qTarget).Hitchance >= HitChance.High && ShouldQ())
            {
                Q.Cast(Q.GetPrediction(qTarget).CastPosition);
            }
        }

        private void CheckR()
        {
            if (_rObj == null)
                return;

            var hit = ObjectManager.Get<AIHeroClient>().Count(x => _rObj.Position.Distance(x.ServerPosition) < 475 && x.IsValidTarget(R.Range + 500));

            if (hit < 1 && R.IsReady() && _rFirstCreated && R.IsReady())
            {
                R.Cast();
            }
        }

        private void Escape()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var enemy = (from champ in ObjectManager.Get<AIHeroClient>() where champ.IsValidTarget(1500) select champ).ToList();

            var hero = enemy.FirstOrDefault();

            if (hero != null && Q.IsReady() && Player.Distance(hero.Position) <= Q.Range && Q.GetPrediction(hero).Hitchance >= HitChance.High && ShouldQ())
            {
                Q.Cast(enemy.FirstOrDefault());
            }

            if (hero != null && (W.IsReady() && Player.Distance(hero.Position) <= W.Range))
            {
                CastWEscape(enemy.FirstOrDefault());
            }
        }

        private void Farm()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            var allMinionsR = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, R.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();
            var useR = Menu.Item("UseRFarm", true).GetValue<bool>();

            int hit = 0;

            if (useQ && Q.IsReady() && ShouldQ())
            {
                var qPos = Q.GetLineFarmLocation(allMinionsQ);
                if (qPos.MinionsHit >= 3)
                {
                    Q.Cast(qPos.Position);
                }
            }

            if (useR & R.IsReady() && !_rFirstCreated)
            {
                var rPos = R.GetCircularFarmLocation(allMinionsR);
                if (Player.Distance(rPos.Position) < R.Range)
                    R.Cast(rPos.Position);
            }

            if (!ShouldQ() && _qMissle != null)
            {
                if (useQ && Q.IsReady())
                {
                    hit += allMinionsQ.Count(enemy => enemy.Distance(_qMissle.Position) < 110);
                }

                if (hit >= 2 && Q.IsReady())
                    Q.Cast();
            }

            if (_rFirstCreated)
            {
                hit += allMinionsR.Count(enemy => enemy.Distance(_rObj.Position) < 400);

                if (hit < 2 && R.IsReady())
                    R.Cast();
            }

            if (useE && allMinionsE.Count > 0 && E.IsReady())
                E.Cast(allMinionsE[0]);
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead)
                return;

            //detonate Q check
            var detQ = Menu.Item("detonateQ", true).GetValue<bool>();
            if (detQ && !ShouldQ())
                DetonateQ();

            //checkR
            var rCheck = Menu.Item("checkR", true).GetValue<bool>();
            if (rCheck && _rFirstCreated && !Menu.Item("LaneClear", true).GetValue<KeyBind>().Active && _rByMe)
                CheckR();


            //check ks
            SmartKs();

            switch (Orbwalker.ActiveMode)
            {
                case xSaliceResurrected_Rework.Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case xSaliceResurrected_Rework.Orbwalking.OrbwalkingMode.Flee:
                    Escape();
                    break;
                case xSaliceResurrected_Rework.Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case xSaliceResurrected_Rework.Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case xSaliceResurrected_Rework.Orbwalking.OrbwalkingMode.LaneClear:
                    Farm();
                    break;
                case xSaliceResurrected_Rework.Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case xSaliceResurrected_Rework.Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case xSaliceResurrected_Rework.Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("FarmT", true).GetValue<KeyBind>().Active)
                        Harass();

                    if (Menu.Item("snipe", true).GetValue<KeyBind>().Active)
                        Snipe();
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
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs attack)
        {
            if (!unit.IsMe) return;

            var castedSlot = Player.GetSpellSlot(attack.SData.Name);

            if (castedSlot == SpellSlot.E)
            {
                _eCasted = true;
            }

            if (castedSlot == SpellSlot.Q && ShouldQ())
            {
                Q.LastCastAttemptT = Environment.TickCount;
            }

        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("UseGap", true).GetValue<bool>()) return;

            if (W.IsReady() && gapcloser.Sender.IsValidTarget(W.Range))
            {
                var vec = Player.ServerPosition -
                              Vector3.Normalize(Player.ServerPosition - gapcloser.Sender.ServerPosition) * 1;
                W.Cast(vec);
            }
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("UseInt", true).GetValue<bool>()) return;

            if (unit.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(unit);
            }

            if (unit.IsValidTarget(W.Range) && W.IsReady())
            {
                W.Cast(unit);
            }
        }

        protected override void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.Type != GameObjectType.obj_GeneralParticleEmitter || Player.Distance(obj.Position) > 1500)
                return;

            //Q
            if (obj.IsValid && obj.Name == "cryo_FlashFrost_Player_mis.troy")
            {
                _qMissle = obj;
            }

            //R
            if (obj.IsValid && obj.Name.Contains("cryo_storm"))
            {
                if (Menu.Item("Orbwalk", true).GetValue<KeyBind>().Active || Menu.Item("LaneClear", true).GetValue<KeyBind>().Active || Menu.Item("FarmT", true).GetValue<KeyBind>().Active)
                    _rByMe = true;

                _rObj = obj;
                _rFirstCreated = true;
            }
        }

        protected override void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.Type != GameObjectType.obj_GeneralParticleEmitter || Player.Distance(obj.Position) > 1500)
                return;

            //Q
            if (Player.Distance(obj.Position) < 1500)
            {
                if (obj.IsValid && obj.Name == "cryo_FlashFrost_Player_mis.troy")
                {
                    _qMissle = null;
                }

                //R
                if (obj.IsValid && obj.Name.Contains("cryo_storm"))
                {
                    _rObj = null;
                    _rFirstCreated = false;
                    _rByMe = false;
                }
            }
        }
    }
}