using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
// ReSharper disable InconsistentNaming
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace yol0Riven
{
    internal class Program
    {
        private static readonly Spell _Q = new Spell(SpellSlot.Q, 260);
        private static readonly Spell _W = new Spell(SpellSlot.W, 250);
        private static readonly Spell _E = new Spell(SpellSlot.E, 325);
        private static readonly Spell _R = new Spell(SpellSlot.R, 900);
        private static readonly Items.Item _Tiamat = new Items.Item(3077, 400);
        private static readonly Items.Item _Hydra = new Items.Item(3074, 400);
        private static readonly Items.Item _Ghostblade = new Items.Item(3142, 600);
        private static Menu _menu;
        private static int qCount;
        private static int lastQCast;
        private static bool ultiOn;
        private static bool ultiReady;
        private static bool WaitForMove;
        private static Spell nextSpell;
        private static string lastSpellName = "";
        private static bool UseAttack;
        private static bool UseTiamat;
        private static AIHeroClient _target;
        private static int lastGapClose;
        private static Orbwalking.Orbwalker _orbwalker;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Riven")
                return;

            _menu = new Menu("yol0 Riven", "yol0Riven", true);
            _menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            _menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            _menu.AddSubMenu(new Menu("Combo", "Combo"));
            _menu.AddSubMenu(new Menu("Killsteal", "KS"));
            _menu.AddSubMenu(new Menu("Misc", "Misc"));
            _menu.AddSubMenu(new Menu("Drawing", "Draw"));

            _orbwalker = new Orbwalking.Orbwalker(_menu.SubMenu("Orbwalker"));
            TargetSelector.AddToMenu(_menu.SubMenu("Target Selector"));

            _menu.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q to gapclose").SetValue(true));
            _menu.SubMenu("Combo").AddItem(new MenuItem("useR", "Use Ultimate").SetValue(true));

            _menu.SubMenu("KS").AddItem(new MenuItem("ksQ", "KS with Q").SetValue(true));
            _menu.SubMenu("KS").AddItem(new MenuItem("ksW", "KS with W").SetValue(true));
            _menu.SubMenu("KS").AddItem(new MenuItem("ksT", "KS with Tiamat/Hydra").SetValue(true));
            _menu.SubMenu("KS").AddItem(new MenuItem("ksR", "KS with R2").SetValue(true));
            _menu.SubMenu("KS").AddItem(new MenuItem("ksRA", "Activate ult for KS").SetValue(false));
            _menu.SubMenu("KS").AddSubMenu(new Menu("Don't use R2 for KS", "noKS"));
            foreach (var enemy in HeroManager.Enemies)
            {
                _menu.SubMenu("KS")
                    .SubMenu("noKS")
                    .AddItem(new MenuItem(enemy.ChampionName, enemy.ChampionName).SetValue(false));
            }

            _menu.SubMenu("Misc")
                .AddItem(new MenuItem("Flee", "Flee Mode").SetValue(new KeyBind("T".ToArray()[0], KeyBindType.Press)));
            _menu.SubMenu("Misc").AddSubMenu(new Menu("Auto Stun", "Stun"));
            foreach (var enemy in HeroManager.Enemies)
            {
                _menu.SubMenu("Misc")
                    .SubMenu("Stun")
                    .AddItem(new MenuItem(enemy.ChampionName, "Stun " + enemy.ChampionName).SetValue(true));
            }

            _menu.SubMenu("Misc").AddItem(new MenuItem("gapclose", "Auto W Gapclosers").SetValue(true));
            _menu.SubMenu("Misc").AddItem(new MenuItem("interrupt", "Auto W Interruptible Spells").SetValue(true));
            _menu.SubMenu("Misc").AddItem(new MenuItem("keepalive", "Keep Q Alive").SetValue(true));

            _menu.SubMenu("Draw")
                .AddItem(new MenuItem("drawRange", "Draw Engage Range").SetValue(new Circle(true, Color.Green)));
            _menu.SubMenu("Draw")
                .AddItem(new MenuItem("drawTarget", "Draw Current Target").SetValue(new Circle(true, Color.Red)));
            _menu.SubMenu("Draw").AddItem(new MenuItem("drawDamage", "Draw Damage on Healthbar").SetValue(true));


            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = GetDamage;
            _menu.AddToMainMenu();

            _R.SetSkillshot(0.25f, 60f, 2200, false, SkillshotType.SkillshotCone);
            _E.SetSkillshot(0, 0, 1450, false, SkillshotType.SkillshotLine);

            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Orbwalking.AfterAttack += AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            AttackableUnit.OnDamage += OnDamage;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
        }

        private static void KillSecure()
        {
            foreach (var enemy in HeroManager.Enemies)
            {
                if (!enemy.IsDead && enemy.IsHPBarRendered)
                {
                    if (ultiReady && _menu.SubMenu("KS").Item("ksR").GetValue<bool>() && qCount == 2 && _Q.IsReady() &&
                        enemy.IsValidTarget(_Q.Range) && GetRDamage(enemy) + GetUltiQDamage(enemy) - 40 >= enemy.Health &&
                        !_menu.SubMenu("KS").SubMenu("noKS").Item(enemy.ChampionName).GetValue<bool>())
                    {
                        _R.Cast(enemy, aoe: true);
                    }
                    if (ultiReady && _menu.SubMenu("KS").Item("ksR").GetValue<bool>() &&
                        enemy.IsValidTarget(_R.Range - 30) && GetRDamage(enemy) - 20 >= enemy.Health &&
                        !_menu.SubMenu("KS").SubMenu("noKS").Item(enemy.ChampionName).GetValue<bool>())
                    {
                        _R.Cast(enemy, aoe: true);
                    }
                    else if (_menu.SubMenu("KS").Item("ksQ").GetValue<bool>() && _Q.IsReady() &&
                        enemy.IsValidTarget(_Q.Range) && (ultiOn ? GetUltiQDamage(enemy) : GetQDamage(enemy)) - 10 >= enemy.Health)
                    {
                        _Q.Cast(enemy.ServerPosition);
                    }
                    else if (_menu.SubMenu("KS").Item("ksW").GetValue<bool>() && _W.IsReady() &&
                             enemy.IsValidTarget(_W.Range) && GetWDamage(enemy) - 10 >= enemy.Health)
                    {
                        _Q.Cast(enemy.ServerPosition);
                    }
                    else if (_menu.SubMenu("KS").Item("ksT").GetValue<bool>() &&
                             (_Tiamat.IsReady() || _Hydra.IsReady()) && enemy.IsValidTarget(_Tiamat.Range) &&
                             Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat) - 10 >= enemy.Health)
                    {
                        if (_Tiamat.IsReady())
                            _Tiamat.Cast();
                        else if (_Hydra.IsReady())
                            _Hydra.Cast();
                    }
                    else if (!ultiReady && !ultiOn && _menu.SubMenu("KS").Item("ksR").GetValue<bool>() &&
                             _menu.SubMenu("KS").Item("ksRA").GetValue<bool>() &&
                             enemy.IsValidTarget(_R.Range - 30) && GetRDamage(enemy) - 20 >= enemy.Health &&
                             _orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                    {
                        _R.Cast();
                    }
                }
            }
        }

        private static void CancelAnimation()
        {
            if (WaitForMove)
                return;

            WaitForMove = true;
            var movePos = Game.CursorPos;
            if (Player.Distance(_target.Position) < 600)
            {
                movePos = Player.ServerPosition.Extend(_target.ServerPosition, 100);
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, movePos);
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && nextSpell == _Q)
            {
                _Q.Cast(_target.ServerPosition);
                nextSpell = null;
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = _menu.SubMenu("Draw").Item("drawDamage").GetValue<bool>();
            CheckBuffs();
            KillSecure();
            if (_orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                AutoStun();
            if (_menu.SubMenu("Misc").Item("Flee").GetValue<KeyBind>().Active)
                Flee();

            if (_target == null)
                _orbwalker.SetMovement(true);

            if (_target != null && _target.IsDead)
                _orbwalker.SetMovement(true);

            if (_orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                _orbwalker.SetMovement(true);

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (_target == null)
                    _target = TargetSelector.GetTarget(_E.Range + _Q.Range, TargetSelector.DamageType.Physical);

                if (_target != null &&
                    (_target.IsDead || !_target.IsHPBarRendered ||
                     !_target.IsValidTarget(_E.Range + _Q.Range + Player.AttackRange)))
                    _orbwalker.SetMovement(true);

                if (_target == null)
                    _orbwalker.SetMovement(true);
                else
                {
                    if (!_target.IsHPBarRendered)
                        _target = TargetSelector.GetTarget(_E.Range + _Q.Range, TargetSelector.DamageType.Physical);

                    if (_target.IsDead)
                        _target = TargetSelector.GetTarget(_E.Range + _Q.Range, TargetSelector.DamageType.Physical);

                    if (!_target.IsValidTarget(_E.Range + _Q.Range + Player.AttackRange))
                        _target = TargetSelector.GetTarget(_E.Range + _Q.Range, TargetSelector.DamageType.Physical);

                    if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget != _target && TargetSelector.SelectedTarget.IsHPBarRendered &&
                        TargetSelector.SelectedTarget is AIHeroClient)
                    {
                        var unit = (AIHeroClient)TargetSelector.SelectedTarget;
                        if (unit.IsValidTarget())
                            _target = (AIHeroClient)TargetSelector.SelectedTarget;
                    }

                    if (TargetSelector.GetSelectedTarget() != null && TargetSelector.GetSelectedTarget() != _target &&
                        TargetSelector.GetSelectedTarget().IsHPBarRendered &&
                        TargetSelector.GetSelectedTarget().IsValidTarget())
                    {
                        _target = TargetSelector.GetSelectedTarget();
                    }

                    if (_target != null && !_target.IsDead && _target.IsHPBarRendered)
                    {
                        GapClose(_target);
                        Combo(_target);
                    }
                    else
                    {
                        _orbwalker.SetMovement(true);
                    }
                }
            }
            else
            {
                _orbwalker.SetMovement(true);
                if (!Player.IsRecalling() && qCount != 0 && lastQCast + (3650 - Game.Ping/2) < Utils.TickCount &&
                    _menu.SubMenu("Misc").Item("keepalive").GetValue<bool>())
                {
                    _Q.Cast(Game.CursorPos);
                }
            }
        }

        private static void Combo(AIHeroClient target)
        {
            _orbwalker.SetMovement(false);
            var noRComboDmg = DamageCalcNoR(target);
            if (_R.IsReady() && !ultiReady && noRComboDmg < target.Health &&
                _menu.SubMenu("Combo").Item("useR").GetValue<bool>())
            {
                _R.Cast();
            }

            if (!(_Tiamat.IsReady() || _Hydra.IsReady()) && !_Q.IsReady() && _W.IsReady() &&
                target.IsValidTarget(_W.Range))
            {
                _W.Cast();
            }

            if (nextSpell == null && UseTiamat)
            {
                if (_Tiamat.IsReady())
                    _Tiamat.Cast();
                else if (_Hydra.IsReady())
                    _Hydra.Cast();

                UseTiamat = false;
                return;
            }

            if (nextSpell == null && UseAttack)
            {
                Orbwalking.LastAATick = Utils.TickCount + Game.Ping/2;
                _orbwalker.SetMovement(false);
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                return;
            }

            if (nextSpell == _Q)
            {
                if (lastSpellName.Contains("Attack") && Player.Spellbook.IsAutoAttacking)
                    return;

                _Q.Cast(target.Position);
                nextSpell = null;
            }

            if (nextSpell == _W)
            {
                _W.Cast();
                nextSpell = null;
            }

            if (nextSpell == _E)
            {
                _E.Cast(target.Position);
                nextSpell = null;
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (_W.IsReady() && sender.IsValidTarget(_W.Range) &&
                _menu.SubMenu("Misc").Item("interrupt").GetValue<bool>())
                _W.Cast();
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_W.IsReady() && gapcloser.Sender.IsValidTarget(_W.Range) &&
                _menu.SubMenu("Misc").Item("gapclose").GetValue<bool>())
            {
                _W.Cast();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (_menu.SubMenu("Draw").Item("drawRange").GetValue<Circle>().Active)
                Render.Circle.DrawCircle(Player.Position,
                    _menu.SubMenu("Combo").Item("useQ").GetValue<bool>() ? _Q.Range + _E.Range : _E.Range,
                    _menu.SubMenu("Draw").Item("drawRange").GetValue<Circle>().Color);

            if (_menu.SubMenu("Draw").Item("drawTarget").GetValue<Circle>().Active && _target != null &&
                _target.IsHPBarRendered)
            {
                Render.Circle.DrawCircle(_target.Position, _target.BoundingRadius + 10,
                    _menu.SubMenu("Draw").Item("drawTarget").GetValue<Circle>().Color);
                Render.Circle.DrawCircle(_target.Position, _target.BoundingRadius + 25,
                    _menu.SubMenu("Draw").Item("drawTarget").GetValue<Circle>().Color);
                Render.Circle.DrawCircle(_target.Position, _target.BoundingRadius + 45,
                    _menu.SubMenu("Draw").Item("drawTarget").GetValue<Circle>().Color);
            }
        }

        private static void OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (_target == null || args.Target.NetworkId != _target.NetworkId) return;
            if ((int) args.Type != 70) return;
            if (lastQCast != 0 && lastQCast + 100 > Utils.TickCount)
            {
                WaitForMove = true;
                CancelAnimation();
                Orbwalking.ResetAutoAttackTimer();
            }
            else if (lastSpellName.Contains("Attack"))
            {
                if (_Tiamat.IsReady())
                {
                    nextSpell = null;
                    UseTiamat = true;
                }
                else if (_Hydra.IsReady())
                {
                    nextSpell = null;
                    UseTiamat = true;
                }
                else if (_W.IsReady() && _target.IsValidTarget(_W.Range) && qCount != 0)
                {
                    UseTiamat = false;
                    nextSpell = _W;
                }
                else
                {
                    UseTiamat = false;
                    nextSpell = _Q;
                }
                UseAttack = false;
                _orbwalker.SetMovement(true);
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe || _orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;

            if (args.Animation.Contains("Spell1"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(125 + Game.Ping/2, CancelAnimation);
            }
            if (WaitForMove && args.Animation.Contains("Run") && _target != null)
            {
                WaitForMove = false;
                Orbwalking.LastAATick = Utils.TickCount + Game.Ping/2;
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, _target);
            }
            if (WaitForMove && args.Animation.Contains("Idle") && _target != null)
            {
                WaitForMove = false;
                Orbwalking.LastAATick = Utils.TickCount + Game.Ping/2;
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, _target);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            var spellname = args.SData.Name;

            if (spellname == "RivenTriCleave")
            {
                lastQCast = Utils.TickCount;
            }
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                lastSpellName = spellname;
                if (spellname.Contains("Attack"))
                {
                    if (_Tiamat.IsReady() && _target.IsValidTarget(_Tiamat.Range))
                    {
                        nextSpell = null;
                        UseTiamat = true;
                    }
                    else if (_Hydra.IsReady() && _target.IsValidTarget(_Hydra.Range))
                    {
                        nextSpell = null;
                        UseTiamat = true;
                    }
                }
                else if (spellname == "RivenTriCleave")
                {
                    nextSpell = null;
                    LeagueSharp.Common.Utility.DelayAction.Add(125 + Game.Ping/2, CancelAnimation);

                    if (_orbwalker.InAutoAttackRange(_target))
                    {
                        nextSpell = null;
                        UseAttack = true;
                        return;
                    }

                    if (_W.IsReady() && _target.IsValidTarget(_W.Range))
                    {
                        nextSpell = _W;
                    }
                    else
                    {
                        nextSpell = null;
                        UseAttack = true;
                    }
                }
                else if (spellname == "RivenMartyr")
                {
                    if (_Q.IsReady())
                    {
                        nextSpell = _Q;
                        UseAttack = false;
                        UseTiamat = false;
                        //LeagueSharp.Common.Utility.DelayAction.Add(175, delegate { nextSpell = _Q; });
                    }
                    else
                    {
                        nextSpell = null;
                        UseAttack = true;
                    }
                }
                else if (spellname == "ItemTiamatCleave")
                {
                    UseTiamat = false;
                    if (_W.IsReady() && _target.IsValidTarget(_W.Range))
                        nextSpell = _W;
                    else if (_Q.IsReady() && _target.IsValidTarget(_Q.Range))
                        nextSpell = _Q;
                }
                else if (spellname == "RivenFengShuiEngine")
                {
                    ultiOn = true;
                    if ((_Tiamat.IsReady() && _target.IsValidTarget(_Tiamat.Range)) ||
                        (_Hydra.IsReady() && _target.IsValidTarget(_Hydra.Range)))
                    {
                        nextSpell = null;
                        UseTiamat = true;
                    }
                    else if (_Q.IsReady() && _target.IsValidTarget(_Q.Range))
                    {
                        nextSpell = _Q;
                    }
                    else if (_E.IsReady())
                    {
                        nextSpell = _E;
                    }
                }
            }
        }

        private static void GapClose(AIHeroClient target)
        {
            var useE = _E.IsReady();
            var useQ = _Q.IsReady() && qCount < 2 && _menu.SubMenu("Combo").Item("useQ").GetValue<bool>();

            if (lastGapClose + 400 > Utils.TickCount && lastGapClose != 0)
                return;

            lastGapClose = Utils.TickCount;

            var aRange = Player.AttackRange + Player.BoundingRadius + target.BoundingRadius;
            var eRange = aRange + _E.Range;
            var qRange = aRange + _Q.Range;
            var eqRange = _Q.Range + _E.Range;
            var distance = Player.Distance(target.ServerPosition);
            if (distance < aRange)
                return;

            nextSpell = null;
            UseTiamat = false;
            UseAttack = true;
            if (_Ghostblade.IsReady())
                _Ghostblade.Cast();

            if (useQ && qRange > distance && !_E.IsReady())
            {
                var comboDmgNoR = DamageCalcNoR(target);
                if (_R.IsReady() && !ultiReady && comboDmgNoR < target.Health &&
                    _menu.SubMenu("Combo").Item("useR").GetValue<bool>())
                {
                    _R.Cast();
                }
                _Q.Cast(target.ServerPosition);
            }
            else if (useE && eRange > distance + aRange)
            {
                var pred = Prediction.GetPrediction(target, 0, 0, 1450);
                _E.Cast(pred.CastPosition);
            }
            else if (useQ && eqRange + aRange > distance)
            {
                var pred = Prediction.GetPrediction(target, 0, 0, 1450);
                _E.Cast(pred.CastPosition);
            }
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var t = args.Target as Obj_AI_Minion;
            if (t == null)
            {
                _orbwalker.SetMovement(false);
            }
        }

        private static void CheckBuffs()
        {
            var ulti = false;
            var ulti2 = false;
            var q = false;

            foreach (var buff in Player.Buffs)
            {
                if (buff.Name == "rivenwindslashready")
                {
                    ulti = true;
                    ultiReady = true;
                }

                if (buff.Name == "RivenTriCleave")
                {
                    q = true;
                    qCount = buff.Count;
                }

                if (buff.Name == "RivenFengShuiEngine")
                {
                    ulti2 = true;
                    ultiOn = true;
                }
            }

            if (!q)
                qCount = 0;

            if (!ulti)
            {
                ultiReady = false;
            }

            if (!ulti2)
                ultiOn = false;
        }

        private static void AutoStun()
        {
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                return;

            foreach (var enemy in HeroManager.Enemies)
            {
                if (_W.IsReady() && enemy.IsValidTarget(_W.Range) &&
                    _menu.SubMenu("Misc").SubMenu("Stun").Item(enemy.ChampionName).GetValue<bool>())
                {
                    _W.Cast();
                }
            }
        }

        private static void Flee()
        {
            _orbwalker.SetMovement(true);
            if (_Q.IsReady())
            {
                _Q.Cast(Game.CursorPos);
            }
            if (_E.IsReady())
            {
                _E.Cast(Game.CursorPos);
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }

        private static float GetDamage(AIHeroClient target)
        {
            if (_R.IsReady() || (ultiReady))
            {
                return (float) DamageCalcR(target);
            }
            return (float) DamageCalcNoR(target);
        }

        private static double GetRDamage(Obj_AI_Base target, float otherdmg = 0.0f)
        {
            if (_R.Level == 0)
                return 0.0;

            var minDmg = (80 + (40*(_R.Level - 1))) +
                            0.6*((0.2*(Player.BaseAttackDamage + Player.FlatPhysicalDamageMod)) + Player.FlatPhysicalDamageMod);

            var targetPercentHealthMissing = 100*(1 - (target.Health - otherdmg)/target.MaxHealth);
            double dmg;
            if (targetPercentHealthMissing > 75.0f)
            {
                dmg = minDmg*3;
            }
            else
            {
                dmg = minDmg + minDmg*(0.0267*targetPercentHealthMissing);
            }

            var realDmg = Player.CalcDamage(target, Damage.DamageType.Physical, dmg - 20);
            return realDmg;
        }

        private static double GetUltiQDamage(Obj_AI_Base target)
        {
            var dmg = 10 + ((_W.Level - 1)*20) + 0.6*(1.2*(Player.BaseAttackDamage + Player.FlatPhysicalDamageMod));
            return Player.CalcDamage(target, Damage.DamageType.Physical, dmg - 10);
        }

        private static double GetUltiWDamage(Obj_AI_Base target)
        {
            var totalAD = Player.FlatPhysicalDamageMod + Player.BaseAttackDamage;
            var dmg = 50 + ((_W.Level - 1)*30) + (0.2*totalAD + Player.FlatPhysicalDamageMod);
            return Player.CalcDamage(target, Damage.DamageType.Physical, dmg - 10);
        }

        private static double GetQDamage(Obj_AI_Base target)
        {
            var totalAD = Player.FlatPhysicalDamageMod + Player.BaseAttackDamage;
            var dmg = 10 + ((_Q.Level - 1)*20) + (0.35 + (Player.Level*0.05))*totalAD;
            return Player.CalcDamage(target, Damage.DamageType.Physical, dmg - 10);
        }

        private static double GetWDamage(Obj_AI_Base target)
        {
            var dmg = 50 + (_W.Level*30) + Player.FlatPhysicalDamageMod;
            return Player.CalcDamage(target, Damage.DamageType.Physical, dmg - 10);
        }

        private static double DamageCalcNoR(Obj_AI_Base target)
        {
            var qDamage = GetQDamage(target);
            var wDamage = GetWDamage(target);
            var tDamage = 0.0;
            var aDamage = Player.GetAutoAttackDamage(target);
            var pDmgMultiplier = 0.2 + (0.05*Math.Floor(Player.Level/3.0));
            
            var totalAD = Player.BaseAttackDamage + Player.FlatPhysicalDamageMod;
            var pDamage = Player.CalcDamage(target, Damage.DamageType.Physical, pDmgMultiplier*totalAD);

            if (_Tiamat.IsReady() || _Hydra.IsReady())
                tDamage = Player.GetItemDamage(target, Damage.DamageItems.Tiamat);

            if (!_Q.IsReady() && qCount == 0)
                qDamage = 0.0;

            if (!_W.IsReady())
                wDamage = 0.0;

            return wDamage + tDamage + (qDamage*(3 - qCount)) + (pDamage*(3 - qCount)) + aDamage*(3 - qCount);
        }

        public static double DamageCalcR(Obj_AI_Base target)
        {
            var qDamage = GetUltiQDamage(target);
            var wDamage = GetUltiWDamage(target);

            var tDamage = 0.0;
            var totalAD = Player.FlatPhysicalDamageMod + Player.BaseAttackDamage;


            var aDamage = Player.CalcDamage(target, Damage.DamageType.Physical, 0.2*totalAD + totalAD);
            var pDmgMultiplier = 0.2 + (0.05*Math.Floor(Player.Level/3.0));
            var pDamage = Player.CalcDamage(target, Damage.DamageType.Physical,
                pDmgMultiplier*(0.2*totalAD + totalAD));
            if (_Tiamat.IsReady() || _Hydra.IsReady())
                tDamage = Player.GetItemDamage(target, Damage.DamageItems.Tiamat);

            if (!_Q.IsReady() && qCount == 0)
                qDamage = 0.0;

            if (!_W.IsReady())
                wDamage = 0.0;


            var dmg = (pDamage*(3 - qCount)) + (aDamage*(3 - qCount)) + wDamage + tDamage +
                         (qDamage*(3 - qCount));

            var rDamage = GetRDamage(target, (float) dmg);

            if (_R.IsReady())
                rDamage = 0.0;

            return dmg + rDamage;
        }
    }
}