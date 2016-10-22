using EloBuddy; 
 using LeagueSharp.Common; 
 namespace xSaliceResurrected_Rework.Pluging
{
    using Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Managers;
    using Utilities;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;

    internal class Ekko : Champion
    {
        private Obj_AI_Base _ekkoPast;
        private readonly IDictionary<int, float> _pastStatus = new Dictionary<int, float>();

        public Ekko()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 800);
            SpellManager.Q2 = new Spell(SpellSlot.Q, 1050);
            SpellManager.W = new Spell(SpellSlot.W, 1600);
            SpellManager.E = new Spell(SpellSlot.E, 325);
            SpellManager.R = new Spell(SpellSlot.R);

            SpellManager.Q.SetSkillshot(.25f, 60f, 1700, false, SkillshotType.SkillshotLine);
            SpellManager.Q2.SetSkillshot(.5f, 120f, 1200, false, SkillshotType.SkillshotLine);
            SpellManager.W.SetSkillshot(.5f, 350f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.R.SetSkillshot(.1f, 400f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var spellMenu = new Menu("SpellMenu", "SpellMenu");
            {
                var qMenu = new Menu("QMenu", "QMenu");
                {
                    qMenu.AddItem(new MenuItem("Auto_Q_Slow", "Auto Q Slow", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Auto_Q_Dashing", "Auto Q Dashing", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Auto_Q_Immobile", "Auto Q Immobile", true).SetValue(true));
                    spellMenu.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("WMenu", "WMenu");
                {
                    wMenu.AddItem(new MenuItem("W_On_Cc", "W On top of Hard CC", true).SetValue(true));
                    spellMenu.AddSubMenu(wMenu);
                }

                var eMenu = new Menu("EMenu", "EMenu");
                {
                    eMenu.AddItem(new MenuItem("E_If_UnderTurret", "E Under Enemy Turret", true).SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle)));
                    eMenu.AddItem(new MenuItem("Do_Not_E", "Do not E if >= Enemies Around location", true).SetValue(new Slider(3, 1, 5)));
                    eMenu.AddItem(new MenuItem("Do_Not_E_HP", "Do not E if HP <= %", true).SetValue(new Slider(20)));
                    spellMenu.AddSubMenu(eMenu);
                }

                var rMenu = new Menu("RMenu", "RMenu");
                {
                    rMenu.AddItem(new MenuItem("R_Safe_Net", "R If Player Take % dmg > in Past 4 Seconds", true).SetValue(new Slider(60)));
                    rMenu.AddItem(new MenuItem("R_Safe_Net2", "R If Player HP <= %", true).SetValue(new Slider(10)));
                    rMenu.AddItem(new MenuItem("R_AOE", "AOE R If Will Hit Holding Combo", true).SetValue(new Slider(3, 1, 5)));
                    rMenu.AddItem(new MenuItem("R_AOE_Global", "AOE R If Will Hit Global check", true).SetValue(new Slider(4, 1, 5)));
                    rMenu.AddItem(new MenuItem("No_R_Aoe", "Do not AOE if old HP <= %", true).SetValue(new Slider(15)));
                    rMenu.AddItem(new MenuItem("R_On_Killable", "Ult Enemy If they are Killable with combo", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("R_KS", "Smart R KS", true).SetValue(true));
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
                farm.AddItem(new MenuItem("MinFarm", "Min Minion >= ", true).SetValue(new Slider(3, 1, 6)));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 50);
                Menu.AddSubMenu(farm);
            }

            var flee = new Menu("Flee", "Flee");
            {
                flee.AddItem(new MenuItem("UseQFlee", "Use Q", true).SetValue(true));
                flee.AddItem(new MenuItem("UseWFlee", "Use W", true).SetValue(true));
                flee.AddItem(new MenuItem("UseEFlee", "Use E", true).SetValue(true));
                Menu.AddSubMenu(flee);
            }

            var miscMenu = new Menu("Misc", "Misc");
            {
                miscMenu.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(true, true, false, false));
                miscMenu.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("UseInt", "Use W to Interrupt", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("UseGapQ", "Use Q for GapCloser", true).SetValue(true));
                miscMenu.AddItem(new MenuItem("UseGapW", "Use W for GapCloser", true).SetValue(true));
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
                customMenu.AddItem(myCust.AddToMenu("Flee Active: ", "Flee"));
                customMenu.AddItem(myCust.AddToMenu("E Turret Active: ", "E_If_UnderTurret"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private double PassiveDmg(Obj_AI_Base target)
        {
            return Player.CalcDamage(target, Damage.DamageType.Magical,
                15+ 12 * Player.Level + Player.TotalMagicalDamage * .7f);
        }

        private double TotalQDmg(Obj_AI_Base target)
        {
            if (Q.Level < 1)
                return 0;

            return Qdmg(target) + Q2Dmg(target);
        }

        private double Qdmg(Obj_AI_Base target)
        {
            if (Q.Level < 1)
                return 0;

            return Player.CalcDamage(target, Damage.DamageType.Magical,
                new double[] { 60, 75, 90, 105, 120 }[Q.Level - 1] + Player.TotalMagicalDamage * .2f);
        }

        private double Q2Dmg(Obj_AI_Base target)
        {
            if (Q.Level < 1)
                return 0;

            return Player.CalcDamage(target, Damage.DamageType.Magical,
                new double[] { 60, 85, 110, 135, 160 }[Q.Level - 1] + Player.TotalMagicalDamage * .6f);
        }

        private double Edmg(Obj_AI_Base target)
        {
            if (E.Level < 1)
                return 0;

            return Player.CalcDamage(target, Damage.DamageType.Magical,
                new double[] { 50, 80, 110, 140, 170 }[E.Level - 1] + Player.TotalMagicalDamage * .2f);
        }


        private double Rdmg(Obj_AI_Base target)
        {
            if (R.Level < 1)
                return 0;

            return Player.CalcDamage(target, Damage.DamageType.Magical,
                new double[] { 200, 350, 500 }[R.Level - 1] + Player.TotalMagicalDamage * 1.3f);
        }

        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            comboDamage += PassiveDmg(target);

            if(Q.IsReady())
                comboDamage += TotalQDmg(target);

            if(E.IsReady())
                comboDamage += Edmg(target);

            if (R.IsReady())
                comboDamage += Rdmg(target);

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target) * 2);
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (!target.IsValidTarget(Q.Range))
                return;

            var damg = GetComboDamage(target);

            ItemManager.Target = target;

            if (damg > target.Health - 50)
                ItemManager.KillableTarget = true;

            ItemManager.UseTargetted = true;

            if (Menu.Item("UseWCombo", true).GetValue<bool>() && W.IsReady())
            {
                var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                var pred = xSaliceResurrected_Rework.Prediction.CommonPredEx.GetP(Player.ServerPosition, W, wTarget, 2.5f, true);

                if (Menu.Item("W_On_Cc", true).GetValue<bool>())
                {
                    foreach (var enemies in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                    {
                        if (enemies.HasBuffOfType(BuffType.Snare) || enemies.HasBuffOfType(BuffType.Stun) ||
                            enemies.HasBuffOfType(BuffType.Fear) || enemies.HasBuffOfType(BuffType.Suppression))
                        {
                            W.Cast(enemies);
                            break;
                        }
                    }
                }

                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    W.Cast(pred.CastPosition);
                }
            }

            if (Menu.Item("UseECombo", true).GetValue<bool>() && E.IsReady())
            {
                var etarget = TargetSelector.GetTarget(E.Range + 425, TargetSelector.DamageType.Magical);

                if (etarget.IsValidTarget(E.Range + 425))
                {
                    var vec = Player.ServerPosition.Extend(etarget.ServerPosition, E.Range - 10);

                    if (vec.Distance(target.ServerPosition) < 425 && ShouldE(vec))
                    {
                        E.Cast(vec);
                        LeagueSharp.Common.Utility.DelayAction.Add((int)E.Delay * 1000 + Game.Ping, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, etarget));
                    }
                }
            }

            if (Menu.Item("UseQCombo", true).GetValue<bool>() && Q.IsReady())
            {
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical,
                    HitChance.VeryHigh);
                SpellCastManager.CastBasicSkillShot(Q2, Q2.Range, TargetSelector.DamageType.Magical,
                    HitChance.VeryHigh);
            }

            if (Menu.Item("UseRCombo", true).GetValue<bool>() && R.IsReady() && _ekkoPast != null)
            {
                if (Menu.Item("R_On_Killable", true).GetValue<bool>())
                {
                    if ((from enemie in HeroManager.Enemies.Where(x => x.IsValidTarget())
                         .Where(x => Prediction.GetPrediction(x, 25f).UnitPosition.Distance(_ekkoPast.ServerPosition) < 400)
                         let dmg = GetComboDamage(enemie)
                         where dmg > enemie.Health
                         select enemie).Any())
                    {
                        R.Cast();
                        return;
                    }
                }

                if (AoeR())
                    R.Cast();

            }
        }

        private void Harass()
        {
            if (!ManaManager.HasMana("Harass"))
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (!target.IsValidTarget(Q.Range))
                return;

            if (Menu.Item("UseWHarass", true).GetValue<bool>() && W.IsReady())
            {
                var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                var pred = xSaliceResurrected_Rework.Prediction.CommonPredEx.GetP(Player.ServerPosition, W, wTarget, 2.5f, true);

                if (Menu.Item("W_On_Cc", true).GetValue<bool>())
                {
                    foreach (var enemies in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                    {
                        if (enemies.HasBuffOfType(BuffType.Snare) || enemies.HasBuffOfType(BuffType.Stun) || 
                            enemies.HasBuffOfType(BuffType.Fear) || enemies.HasBuffOfType(BuffType.Suppression))
                        {
                            W.Cast(enemies);
                            break;
                        }
                    }
                }

                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    W.Cast(pred.CastPosition);
                }
            }

            if (Menu.Item("UseEHarass", true).GetValue<bool>() && E.IsReady())
            {
                var etarget = TargetSelector.GetTarget(E.Range + 425, TargetSelector.DamageType.Magical);

                if (etarget.IsValidTarget(E.Range + 425))
                {
                    var vec = Player.ServerPosition.Extend(etarget.ServerPosition, E.Range - 10);

                    if (vec.Distance(target.ServerPosition) < 425 && ShouldE(vec))
                    {
                        E.Cast(vec);
                        LeagueSharp.Common.Utility.DelayAction.Add((int)E.Delay * 1000 + Game.Ping, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, etarget));
                    }
                }
            }

            if (Menu.Item("UseQHarass", true).GetValue<bool>() && Q.IsReady())
            {
                SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical,
                    HitChance.VeryHigh);
                SpellCastManager.CastBasicSkillShot(Q2, Q2.Range, TargetSelector.DamageType.Magical,
                    HitChance.VeryHigh);
            }
        }

        private bool ShouldE(Vector3 vec)
        {
            var maxEnemies = Menu.Item("Do_Not_E", true).GetValue<Slider>().Value;

            if (!Menu.Item("E_If_UnderTurret", true).GetValue<KeyBind>().Active && vec.UnderTurret(true))
                return false;

            if (Player.HealthPercent <= Menu.Item("Do_Not_E_HP", true).GetValue<Slider>().Value)
                return false;

            return vec.CountEnemiesInRange(600) < maxEnemies;
        }

        private void Flee()
        {
            var useQ = Menu.Item("UseQFlee", true).GetValue<bool>();
            var useW = Menu.Item("UseWFlee", true).GetValue<bool>();
            var useE = Menu.Item("UseEFlee", true).GetValue<bool>();

            if (!useQ && !useW)
                return;

            if (useE)
            {
                var vec = Player.ServerPosition.Extend(Game.CursorPos, E.Range);
                E.Cast(vec);
            }

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
            {
                if (Q.IsReady() && useQ)
                    Q.Cast(target);

                if (W.IsReady() && useW)
                {
                    Player.ServerPosition.Extend(Game.CursorPos, 400);
                }
            }
        }

        private void SafetyR()
        {
            var burstHpAllowed = Menu.Item("R_Safe_Net", true).GetValue<Slider>().Value;
            
            if (_pastStatus.ContainsKey(Utils.TickCount - 3900))
            {
                var burst = _pastStatus[Utils.TickCount - 3900] - Player.HealthPercent;

                if (burst >= burstHpAllowed)
                {
                    R.Cast();
                }
            }
        }

        private bool AoeR(bool global = false)
        {
            if (!R.IsReady())
                return false;

            float minHp = Menu.Item("No_R_Aoe", true).GetValue<Slider>().Value;
            if (_pastStatus.Keys.ToList().Where(x => x == 3900).Any(value => value <= minHp))
            {
                return false;
            }

            var hit = TargetHitWithR();

            var min = global ? Menu.Item("R_AOE_Global", true).GetValue<Slider>().Value : Menu.Item("R_AOE", true).GetValue<Slider>().Value;
           
            return hit >= min;
        }

        private int TargetHitWithR()
        {
            if (!R.IsReady() || _ekkoPast == null)
                return 0;

            return HeroManager.Enemies.Where(x => x.IsValidTarget()).Count(x => _ekkoPast.Distance(Prediction.GetPrediction(x, .2f).UnitPosition) < 400);
        }

        private void AutoQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                if (Q.GetPrediction(target).Hitchance >= HitChance.High &&
                    (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare)) &&
                    Menu.Item("Auto_Q_Slow", true).GetValue<bool>())
                {
                    Q.Cast(target);
                }

                if (target.HasBuffOfType(BuffType.Slow) && Menu.Item("Auto_Q_Slow", true).GetValue<bool>())
                {
                    Q.Cast(target);
                }

                if (target.IsDashing() && Menu.Item("Auto_Q_Dashing", true).GetValue<bool>())
                {
                    Q.Cast(target);
                }
            }
        }

        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();

            var minion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            var min = Menu.Item("MinFarm", true).GetValue<Slider>().Value;

            if (useQ)
            {
                var pred = Q.GetLineFarmLocation(minion);

                if(pred.MinionsHit >= min)
                    Q.Cast(pred.Position);
            }
        }

        private void CheckKs()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q2.Range)).OrderByDescending(GetComboDamage))
            {
                if (Player.ServerPosition.Distance(target.ServerPosition) <= Q.Range &&
                    Qdmg(target) > target.Health && Q.IsReady())
                {
                    Q.Cast(target);
                    return;
                }

                if (Player.ServerPosition.Distance(target.ServerPosition) <= Q2.Range && 
                    TotalQDmg(target) > target.Health && Q.IsReady() && 
                    Q2.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                {
                    Q2.Cast(target);
                    return;
                }

                if (Player.ServerPosition.Distance(target.ServerPosition) <= E.Range + 475 && Edmg(target) > target.Health && E.IsReady())
                {
                    var vec = Player.ServerPosition.Extend(target.ServerPosition, E.Range - 10);
                    E.Cast(vec);
                    var target1 = target;
                    LeagueSharp.Common.Utility.DelayAction.Add((int)E.Delay * 1000 + Game.Ping, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target1));
                    return;
                }

                if (R.IsReady() && _ekkoPast != null)
                {
                    if (_ekkoPast.Distance(Prediction.GetPrediction(target, .2f).UnitPosition) <= R.Width && Rdmg(target) > target.Health)
                    {
                        R.Cast();
                        return;
                    }
                }
            }
        }

        private void UpdateOldStatus()
        {
            if (_pastStatus.Keys.ToList().All(x => x != Utils.TickCount))
            {
                _pastStatus.Add(Utils.TickCount, Player.HealthPercent);
            }

            foreach (var remove in _pastStatus.Keys.Where(x => Utils.TickCount - x > 4000).ToList())
            {
                _pastStatus.Remove(remove);
            }
        }

        protected override void ObjAiHeroOnOnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!sender.IsMe || !R.IsReady() || args.Damage > 45)
                return;

            var safeNet = Menu.Item("R_Safe_Net2", true).GetValue<Slider>().Value;

            if (Player.HealthPercent <= safeNet)
            {
                R.Cast();
            }
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            
            if(_ekkoPast == null && R.IsReady())
                _ekkoPast = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.Name == "Ekko" && x.IsAlly);
            
            UpdateOldStatus();
            SafetyR();
            AutoQ();
           
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
                    AoeR(true);
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Base) || !sender.IsAlly)
                return;

            if (sender.Name == "Ekko")
            {
                _ekkoPast = (Obj_AI_Base)sender;
            }
        }
        
        protected override void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Base) || !sender.IsAlly)
                return;

            if (sender.IsAlly && sender.Name == "Ekko")
            {
                _ekkoPast = null;
            }
        }

        protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("UseInt", true).GetValue<bool>()) return;

            if (Player.Distance(unit.Position) < W.Range && W.IsReady())
            {
                W.Cast(unit);
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("UseGapQ", true).GetValue<bool>())
            {
                if (Q.IsReady() && gapcloser.Sender.IsValidTarget(Q.Range))
                    Q.Cast(gapcloser.Sender);
            }

            if (Menu.Item("UseGapW", true).GetValue<bool>())
            {
                if (W.IsReady() && gapcloser.Sender.IsValidTarget(W.Range))
                    W.Cast(gapcloser.Sender);
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
                if (R.Level > 0 && _ekkoPast != null)
                    Render.Circle.DrawCircle(_ekkoPast.Position, R.Width, R.IsReady() ? Color.FromArgb(29, 238, 64) : Color.RoyalBlue);

            if (R.IsReady() && _ekkoPast != null)
            {
                var wts = Drawing.WorldToScreen(Player.Position);
               Drawing.DrawText(wts[0] - 20, wts[1], Color.White, "Enemies Hit with R: " + TargetHitWithR());
            }
        }
    }
}
