using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ADCPackage.Plugins;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; namespace ADCPackage
{
    public static class CustomOrbwalker
    {
        public delegate void AfterAttackEvenH(AttackableUnit unit, AttackableUnit target);

        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);

        public delegate void OnAttackEvenH(AttackableUnit unit, AttackableUnit target);

        public delegate void OnNonKillableMinionH(AttackableUnit minion);

        public delegate void OnTargetChangeH(AttackableUnit oldTarget, AttackableUnit newTarget);

        public enum OrbwalkingMode
        {
            LastHit,
            Mixed,
            LaneClear,
            Combo,
            CustomMode,
            None
        }

        //Spells that reset the attack timer.
        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fioraflurry", "garenq",
            "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane", "lucianq",
            "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade",
            "parley", "poppydevastatingblow", "powerfist", "renektonpreexecute", "rengarq", "shyvanadoubleattack",
            "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble", "vie", "volibearq",
            "xenzhaocombotarget", "yorickspectral", "reksaiq", "itemtitanichydracleave"
        };

        //Spells that are not attacks even if they have the "attack" word in their name.
        private static readonly string[] NoAttacks =
        {
            "volleyattack", "volleyattackwithsound", "jarvanivcataclysmattack",
            "monkeykingdoubleattack", "shyvanadoubleattack",
            "shyvanadoubleattackdragon", "zyragraspingplantattack",
            "zyragraspingplantattack2", "zyragraspingplantattackfire",
            "zyragraspingplantattack2fire", "viktorpowertransfer",
            "sivirwattackbounce", "asheqattacknoonhit",
            "elisespiderlingbasicattack", "heimertyellowbasicattack",
            "heimertyellowbasicattack2", "heimertbluebasicattack",
            "annietibbersbasicattack", "annietibbersbasicattack2",
            "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack",
            "yorickspectralghoulbasicattack", "malzaharvoidlingbasicattack",
            "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
            "kindredwolfBasicAttack", "kindredbasicattackoverridelightbombfinal"
        };

        //Spells that are attacks even if they dont have the "attack" word in their name.
        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "lucianpassiveattack", "masteryidoublestrike", "quinnwenhanced", "renektonexecute",
            "renektonsuperexecute", "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "xenzhaothrust2",
            "xenzhaothrust3", "viktorqbuff"
        };

        // Champs whose auto attacks can't be cancelled
        private static readonly string[] NoCancelChamps = { "Kalista" };
        public static int LastAATick;
        public static bool Attack = true;
        public static bool DisableNextAttack;
        public static bool Move = true;
        public static int LastMoveCommandT;
        public static Vector3 LastMoveCommandPosition = Vector3.Zero;
        private static AttackableUnit _lastTarget;
        private static readonly AIHeroClient Player;
        private static int _delay;
        private static MenuItem _stringList;

        private static float _fix;
        private static float _minDistance = 400;
        private static bool _missileLaunched = false;
        private static string _championName;
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        static CustomOrbwalker()
        {
            Player = ObjectManager.Player;
            _championName = Player.ChampionName;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Spellbook.OnStopCast += SpellbookOnStopCast;
        }

        /// <summary>
        ///     This event is fired before the player auto attacks.
        /// </summary>
        public static event BeforeAttackEvenH BeforeAttack;

        /// <summary>
        ///     This event is fired when a unit is about to auto-attack another unit.
        /// </summary>
        public static event OnAttackEvenH OnAttack;

        /// <summary>
        ///     This event is fired after a unit finishes auto-attacking another unit (Only works with player for now).
        /// </summary>
        public static event AfterAttackEvenH AfterAttack;

        /// <summary>
        ///     Gets called on target changes
        /// </summary>
        public static event OnTargetChangeH OnTargetChange;

        //  <summary>
        //      Gets called if you can't kill a minion with auto attacks
        //  </summary>
        public static event OnNonKillableMinionH OnNonKillableMinion;

        private static void FireBeforeAttack(AttackableUnit target)
        {
            if (BeforeAttack != null)
            {
                BeforeAttack(new BeforeAttackEventArgs { Target = target });
            }
            else
            {
                DisableNextAttack = false;
            }
        }

        private static void FireOnAttack(AttackableUnit unit, AttackableUnit target)
        {
            OnAttack?.Invoke(unit, target);
        }

        private static void FireAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (AfterAttack != null && target.IsValidTarget())
            {
                AfterAttack(unit, target);
            }
        }

        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null && (!_lastTarget.IsValidTarget() || _lastTarget != newTarget))
            {
                OnTargetChange(_lastTarget, newTarget);
            }
        }

        private static void FireOnNonKillableMinion(AttackableUnit minion)
        {
            if (OnNonKillableMinion != null)
            {
                OnNonKillableMinion(minion);
            }
        }

        /// <summary>
        ///     Returns true if the spellname resets the attack timer.
        /// </summary>
        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        /// <summary>
        ///     Returns true if the unit is melee
        /// </summary>
        public static bool IsMelee(this Obj_AI_Base unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }

        /// <summary>
        ///     Returns true if the spellname is an auto-attack.
        /// </summary>
        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) ||
                   Attacks.Contains(name.ToLower());
        }

        /// <summary>
        ///     Returns the auto-attack range of local player with respect to the target.
        /// </summary>
        public static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = Player.AttackRange + Player.BoundingRadius;
            if (target.IsValidTarget())
            {
                return result + target.BoundingRadius;
            }
            return result;
        }

        /// <summary>
        ///     Returns the auto-attack range of the target.
        /// </summary>
        public static float GetAttackRange(AIHeroClient target)
        {
            var result = target.AttackRange + target.BoundingRadius;
            return result;
        }

        /// <summary>
        ///     Returns true if the target is in auto-attack range.
        /// </summary>
        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.IsValidTarget())
            {
                return false;
            }
            var myRange = GetRealAutoAttackRange(target);
            return
                Vector2.DistanceSquared(
                    (target is Obj_AI_Base) ? ((Obj_AI_Base)target).ServerPosition.To2D() : target.Position.To2D(),
                    Player.ServerPosition.To2D()) <= myRange * myRange;
        }

        /// <summary>
        ///     Returns player auto-attack missile speed.
        /// </summary>
        public static float GetMyProjectileSpeed()
        {
            return IsMelee(Player) || _championName == "Azir" || _championName == "Velkoz" || _championName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn") ? float.MaxValue : Player.BasicAttack.MissileSpeed;
        }

        /// <summary>
        ///     Returns if the player's auto-attack is ready.
        /// </summary>
        public static bool CanAttack()
        {
            return !_missileLaunched;
        }

        public static void SetMovementDelay(int delay)
        {
            _delay = delay;
        }

        public static void SetMinimumOrbwalkDistance(float d)
        {
            _minDistance = d;
        }

        public static float GetLastMoveTime()
        {
            return LastMoveCommandT;
        }

        public static Vector3 GetLastMovePosition()
        {
            return LastMoveCommandPosition;
        }

        public static void MoveTo(Vector3 position,
            float holdAreaRadius = 0,
            bool overrideTimer = false,
            bool useFixedDistance = true,
            bool randomizeMinDistance = true)
        {
            var playerPosition = Player.ServerPosition;

            if (playerPosition.Distance(position, true) < holdAreaRadius * holdAreaRadius)
            {
                if (Player.Path.Length > 0)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, playerPosition);
                    LastMoveCommandPosition = playerPosition;
                    LastMoveCommandT = Utils.GameTimeTickCount - 70;
                }
                return;
            }

            var point = position;

            if (Player.Distance(point, true) < 150 * 150)
            {
                point = playerPosition.Extend(position, (randomizeMinDistance ? (_random.NextFloat(0.6f, 1) + 0.2f) * _minDistance : _minDistance));
            }
            var angle = 0f;
            var currentPath = Player.GetWaypoints();
            if (currentPath.Count > 1 && currentPath.PathLength() > 100)
            {
                var movePath = Player.GetPath(point);

                if (movePath.Length > 1)
                {
                    var v1 = currentPath[1] - currentPath[0];
                    var v2 = movePath[1] - movePath[0];
                    angle = v1.AngleBetween(v2.To2D());
                    var distance = movePath.Last().To2D().Distance(currentPath.Last(), true);

                    if ((angle < 10 && distance < 500 * 500) || distance < 50 * 50)
                    {
                        return;
                    }
                }
            }

            if (Utils.GameTimeTickCount - LastMoveCommandT < (90 + Math.Min(80, Game.Ping)) && !overrideTimer && angle < 60)
            {
                return;
            }

            if (angle >= 60 && Utils.GameTimeTickCount - LastMoveCommandT < 160)
            {
                return;
            }

            if (angle >= 150 && Utils.GameTimeTickCount - LastMoveCommandT < (106 + Math.Min(63, Game.Ping)) && !overrideTimer)
            {
                return;
            }


            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, point);
            LastMoveCommandPosition = point;
            LastMoveCommandT = Utils.GameTimeTickCount;
        }

        /// <summary>
        ///     Orbwalk a target while moving to Position.
        /// </summary>
        public static void Orbwalk(AttackableUnit target,
            Vector3 position,
            float extraWindup = 200,
            float holdAreaRadius = 0,
            bool useFixedDistance = true,
            bool randomizeMinDistance = true)
        {
            try
            {
                if (target.IsValidTarget() && EloBuddy.SDK.Orbwalker.CanAutoAttack)
                {
                    DisableNextAttack = false;
                    FireBeforeAttack(target);

                    if (!DisableNextAttack)
                    {
                        if (!NoCancelChamps.Contains(_championName))
                        {
                            LastAATick = Utils.GameTimeTickCount + Game.Ping + 100 - (int)(ObjectManager.Player.AttackCastDelay * 1000f);

                            var d = GetRealAutoAttackRange(target) - 65;
                            if (Player.Distance(target, true) > d * d && !Player.IsMelee)
                            {
                                LastAATick = Utils.GameTimeTickCount + Game.Ping + 400 - (int)(ObjectManager.Player.AttackCastDelay * 1000f);
                            }
                        }

                        if (!EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                        {
                            ResetAutoAttackTimer();
                        }

                        LastMoveCommandT = 0;
                        _lastTarget = target;
                        return;
                    }
                }

                if (EloBuddy.SDK.Orbwalker.CanMove)
                {
                    MoveTo(position, holdAreaRadius, false, useFixedDistance, randomizeMinDistance);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        ///     Resets the Auto-Attack timer.
        /// </summary>
        public static void ResetAutoAttackTimer()
        {
            LastAATick = 0;
        }

        private static void SpellbookOnStopCast(Obj_AI_Base spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.IsValid && spellbook.IsMe && args.DestroyMissile && args.StopAnimation)
            {
                ResetAutoAttackTimer();
            }
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs Spell)
        {
            // todo: MOVED ALL THE BELOW FROM ONPROCESS TO THIS. CHANGED MY 700 TO SOMETHING ELSE. MOVES IF (SPELL.TARGET) TO ABOVE LASTAATICK.
            try // 705 used to be #
            {
                var spellName = Spell.SData.Name;
                var attackmode = Orbwalker._config.Item("CanAttackMode").GetValue<StringList>().SelectedIndex;

                if (IsAutoAttackReset(spellName) && unit.IsMe)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, ResetAutoAttackTimer);
                }

                if (!IsAutoAttack(spellName))
                {
                    return;
                }

                if (unit.IsMe &&
                    (Spell.Target is Obj_AI_Base || Spell.Target is Obj_BarracksDampener || Spell.Target is Obj_HQ))
                {
                    LastAATick = Utils.GameTimeTickCount - Game.Ping / 2;
                    _missileLaunched = true;

                    if (Spell.Target is Obj_AI_Base)
                    {
                        var target = (Obj_AI_Base)Spell.Target;
                        if (target.IsValid)
                        {
                            FireOnTargetSwitch(target);
                            _lastTarget = target;
                        }
                    }

                    if (attackmode == 0)
                    {
                        if (Orbwalker._config.Item("AttackModeSwitch").GetValue<bool>())
                        {
                            if (Player.HealthPercent <= 25)
                            {
                                _stringList.SetValue(new StringList(new string[] {"Damage", "Distance"}, 1));
                                _fix = Player.AttackDelay * 905 - Game.Ping;
                                LeagueSharp.Common.Utility.DelayAction.Add((int)_fix, () => _missileLaunched = false);
                                FireOnAttack(unit, _lastTarget);
                                FireAfterAttack(unit, Spell.Target as AttackableUnit);
                                return;
                            }
                        }
                        if (_championName == "Jinx")
                        {
                            var jinxqattackspeed = ((((20 + ((Jinx.Q.Level - 1 * 10))) + new float[] { 0, 2, 4, 6, 8, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70 }[Player.Level - 1]) * Player.AttackDelay) + ((Player.AttackDelay * 750)));
                            var buff = Player.GetBuff("jinxqramp");

                            _fix = (_championName == "Jinx" && buff != null && buff.Count == 3 &&
                                    buff.EndTime - Game.Time > 1.75 && Jinx.FishBones
                                ? jinxqattackspeed - Game.Ping
                                : Player.AttackDelay*750 - Game.Ping);
                        }
                        else
                        {
                            _fix = (Player.AttackDelay*750) /* - ((1000*Player.Distance(Spell.Target.Position))/GetMyProjectileSpeed() + Player.AttackCastDelay)*/ - Game.Ping; //todo: had * 705 works perfectly // 800 = good on common 800 is actually bad now? try 750, not sure if distance to target impacts anything, but the - is wrong
                            _fix = (Player.AttackDelay*750) /* - ((1000*Player.Distance(Spell.Target.Position))/GetMyProjectileSpeed() + Player.AttackCastDelay)*/ - Game.Ping; //todo: had * 705 works perfectly // 800 = good on common 800 is actually bad now? try 750, not sure if distance to target impacts anything, but the - is wrong
                            //Console.WriteLine((Player.AttackDelay * 1000));
                            //Console.WriteLine(((1000 * Player.Distance(Spell.Target.Position)) / GetMyProjectileSpeed() + Player.AttackCastDelay));
                        }
                    }
                    else
                    {
                        if (Orbwalker._config.Item("AttackModeSwitch").GetValue<bool>())
                        {
                            if (Player.HealthPercent > 25)
                            {
                                _stringList.SetValue(new StringList(new string[] { "Damage", "Distance" }, 0));
                                _fix = Player.AttackDelay * 750 - Game.Ping;
                                LeagueSharp.Common.Utility.DelayAction.Add((int)_fix, () => _missileLaunched = false);
                                FireOnAttack(unit, _lastTarget);
                                FireAfterAttack(unit, Spell.Target as AttackableUnit);
                                return;
                            }
                        }
                        _fix = Player.AttackDelay * 905 - Game.Ping;
                    }
                    LeagueSharp.Common.Utility.DelayAction.Add((int)_fix, () => _missileLaunched = false);

                }
                FireOnAttack(unit, _lastTarget);
                FireAfterAttack(unit, Spell.Target as AttackableUnit);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //if (unit.IsMe && IsAutoAttack(Spell.SData.Name))
            //{
            //    _missileLaunched = true;
            //    FireAfterAttack(unit, Spell.Target as AttackableUnit);

            //    var delay = Player.AttackDelay * 700;
            //    LeagueSharp.Common.Utility.DelayAction.Add((int)delay, () => _missileLaunched = false);
            //}
        }

        public class BeforeAttackEventArgs
        {
            private bool _process = true;
            public AttackableUnit Target;
            public Obj_AI_Base Unit = ObjectManager.Player;

            public bool Process
            {
                get { return _process; }
                set
                {
                    DisableNextAttack = !value;
                    _process = value;
                }
            }
        }

        /// <summary>
        ///     This class allows you to add an instance of "Orbwalker" to your assembly in order to control the orbwalking in an
        ///     easy way.
        /// </summary>
        public class Orbwalker
        {
            private const float LaneClearWaitTimeMod = 2f;
            public static LeagueSharp.Common.Menu _config;
            private readonly AIHeroClient Player;
            private Obj_AI_Base _forcedTarget;
            private OrbwalkingMode _mode = OrbwalkingMode.None;
            private Vector3 _orbwalkingPoint;
            private Obj_AI_Minion _prevMinion;
            public static List<Orbwalker> Instances = new List<Orbwalker>();

            public Orbwalker(LeagueSharp.Common.Menu attachToMenu)
            {
                _config = attachToMenu;
                /* Drawings submenu */

                var drawings = new LeagueSharp.Common.Menu("Drawings", "drawings");
                drawings.AddItem(
                    new MenuItem("AACircle", "AACircle").SetShared()
                        .SetValue(new Circle(true, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(
                    new MenuItem("AACircle2", "Enemy AA circle").SetShared()
                        .SetValue(new Circle(false, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(
                    new MenuItem("HoldZone", "HoldZone").SetShared()
                        .SetValue(new Circle(false, Color.FromArgb(155, 255, 255, 0))));
                drawings.AddItem(
                    new MenuItem("AALineWidth", "Line Width")).SetShared()
                        .SetValue(new Slider(2, 1, 6));
                _config.AddSubMenu(drawings);

                /* Misc options */
                var misc = new LeagueSharp.Common.Menu("Misc", "Misc");
                misc.AddItem(
                    new MenuItem("HoldPosRadius", "Hold Position Radius").SetShared().SetValue(new Slider(0, 0, 250)));
                misc.AddItem(new MenuItem("PriorizeFarm", "Priorize farm over harass").SetShared().SetValue(true));
                misc.AddItem(new MenuItem("AttackWards", "Auto attack wards").SetShared().SetValue(false));
                misc.AddItem(new MenuItem("AttackPetsnTraps", "Auto attack pets & traps").SetShared().SetValue(true));
                misc.AddItem(new MenuItem("Smallminionsprio", "Jungle clear small first").SetShared().SetValue(false));

                _config.AddSubMenu(misc);

                /* Delay sliders */
                _config.AddItem(
                    new MenuItem("CanAttackMode", "Attacking mode").SetShared()
                        .SetValue(new StringList(new[] {"Damage", "Distance"})));
                _config.AddItem(
                    new MenuItem("AttackModeSwitch", "Switch to Distance when HP% < 25%").SetShared()
                        .SetValue(false));
                _config.AddItem(new MenuItem("info1", "What is Attacking Mode?").SetFontStyle(FontStyle.Regular, SharpDX.Color.DarkTurquoise)).SetTooltip(
                        "Damage moves less between attacks to attack more, distance moves more but attacks less.",
                        SharpDX.Color.Yellow);
                _config.AddItem(new MenuItem("FarmDelay", "Farm delay").SetShared().SetValue(new Slider(30, 0, 200)));

                /*Load the menu*/
                _config.AddItem(
                    new MenuItem("LastHit", "Last hit").SetShared().SetValue(new KeyBind('X', KeyBindType.Press)));

                _config.AddItem(new MenuItem("Farm", "Mixed").SetShared().SetValue(new KeyBind('C', KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("LaneClear", "LaneClear").SetShared().SetValue(new KeyBind('V', KeyBindType.Press)));

                _config.AddItem(
                    new MenuItem("Orbwalk", "Combo").SetShared().SetValue(new KeyBind(32, KeyBindType.Press)));


                _stringList = _config.Item("CanAttackMode");
                Player = ObjectManager.Player;
                Game.OnUpdate += GameOnOnGameUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Instances.Add(this);
            }

            public virtual bool InAutoAttackRange(AttackableUnit target)
            {
                return Orbwalking.InAutoAttackRange(target);
            }

            private int FarmDelay
            {
                get { return _config.Item("FarmDelay").GetValue<Slider>().Value; }
            }

            /// <summary>
            /// Registers the Custom Mode of the Orbwalker. Useful for adding a flee mode and such.
            /// </summary>
            /// <param name="name">The name of the mode in the menu. Ex. Flee</param>
            /// <param name="key">The default key for this mode.</param>
            public virtual void RegisterCustomMode(string name, uint key)
            {
                if (_config.Item("CustomMode") == null)
                {
                    _config.AddItem(
                        new MenuItem("CustomMode", name).SetShared().SetValue(new KeyBind(key, KeyBindType.Press)));
                }
            }

            public OrbwalkingMode ActiveMode
            {
                get
                {
                    if (_mode != OrbwalkingMode.None)
                    {
                        return _mode;
                    }

                    if (_config.Item("Orbwalk").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Combo;
                    }

                    if (_config.Item("LaneClear").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LaneClear;
                    }

                    if (_config.Item("Farm").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Mixed;
                    }

                    if (_config.Item("LastHit").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LastHit;
                    }

                    if (_config.Item("CustomMode") != null && _config.Item("CustomMode").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.CustomMode;
                    }

                    return OrbwalkingMode.None;
                }
                set { _mode = value; }
            }

            /// <summary>
            ///     Enables or disables the auto-attacks.
            /// </summary>
            public void SetAttack(bool b)
            {
                Attack = b;
            }

            /// <summary>
            ///     Enables or disables the movement.
            /// </summary>
            public void SetMovement(bool b)
            {
                Move = b;
            }

            /// <summary>
            ///     Forces the orbwalker to attack the set target if valid and in range.
            /// </summary>
            public void ForceTarget(Obj_AI_Base target)
            {
                _forcedTarget = target;
            }

            /// <summary>
            ///     Forces the orbwalker to move to that point while orbwalking (Game.CursorPos by default).
            /// </summary>
            public void SetOrbwalkingPoint(Vector3 point)
            {
                _orbwalkingPoint = point;
            }

            private bool ShouldWait()
            {
                return Player.Spellbook.IsAutoAttacking ||
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Any(
                            minion =>
                                minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                                InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false) &&
                                HealthPrediction.LaneClearHealthPrediction(
                                    minion, (int)((Player.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay) <=
                                Player.GetAutoAttackDamage(minion));
            }

            public virtual AttackableUnit GetTarget()
            {
                AttackableUnit result = null;

                if ((ActiveMode == OrbwalkingMode.Mixed || ActiveMode == OrbwalkingMode.LaneClear) &&
                    !_config.Item("PriorizeFarm").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
                    if (target != null && InAutoAttackRange(target))
                    {
                        return target;
                    }
                }

                /*Killable Minion*/
                if (ActiveMode == OrbwalkingMode.LaneClear || ActiveMode == OrbwalkingMode.Mixed ||
                    ActiveMode == OrbwalkingMode.LastHit)
                {
                    var MinionList =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                minion =>
                                    minion.IsValidTarget() && InAutoAttackRange(minion) && !minion.IsDead && minion.IsTargetable && minion.IsHPBarRendered && minion.IsHPBarRendered)
                                    .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                                    .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                                    .ThenBy(minion => minion.Health)
                                    .ThenByDescending(minion => minion.MaxHealth);

                    foreach (var minion in MinionList)
                    {
                        var t = (int)(Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 +
                                1000 * (int)Math.Max(0, Player.Distance(minion) - Player.BoundingRadius) / (int)GetMyProjectileSpeed();
                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay);

                        if (minion.Team != GameObjectTeam.Neutral && (_config.Item("AttackPetsnTraps").GetValue<bool>() && minion.BaseSkinName != "jarvanivstandard" || MinionManager.IsMinion(minion, _config.Item("AttackWards").GetValue<bool>())))
                        {
                            if (predHealth <= 0)
                            {
                                FireOnNonKillableMinion(minion);
                            }

                            if (predHealth > 0 && predHealth <= Player.GetAutoAttackDamage(minion, true))
                            {
                                return minion;
                            }
                        }
                    }
                }

                //Forced target
                if (_forcedTarget.IsValidTarget() && InAutoAttackRange(_forcedTarget) && _forcedTarget.IsHPBarRendered && !_forcedTarget.IsDead && _forcedTarget.IsHPBarRendered && _forcedTarget.IsTargetable)
                {
                    return _forcedTarget;
                }

                /* turrets / inhibitors / nexus */
                if (ActiveMode == OrbwalkingMode.LaneClear)
                {
                    /* turrets */
                    foreach (var turret in
                        ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    /* inhibitor */
                    foreach (var turret in
                        ObjectManager.Get<Obj_BarracksDampener>().Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    /* nexus */
                    foreach (var nexus in
                        ObjectManager.Get<Obj_HQ>().Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return nexus;
                    }
                }

                /*Champions*/
                if (ActiveMode != OrbwalkingMode.LastHit)
                {
                    var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget() && InAutoAttackRange(target) && !target.IsDead && target.IsHPBarRendered && target.IsHPBarRendered && target.IsTargetable)
                    {
                        return target;
                    }
                }

                /*Jungle minions*/
                if (ActiveMode == OrbwalkingMode.LaneClear || ActiveMode == OrbwalkingMode.Mixed)
                {
                    var jminions =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                mob =>
                                    mob.IsValidTarget() && mob.Team == GameObjectTeam.Neutral && InAutoAttackRange(mob) &&
                                    mob.CharData.BaseSkinName != "gangplankbarrel");

                    result = _config.Item("Smallminionsprio").GetValue<bool>() ? jminions.MinOrDefault(mob => mob.MaxHealth) : jminions.MaxOrDefault(mob => mob.MaxHealth);

                    if (result != null)
                    {
                        return result;
                    }
                }

                /*Lane Clear minions*/
                if (ActiveMode == OrbwalkingMode.LaneClear)
                {
                    if (!ShouldWait())
                    {
                        if (_prevMinion.IsValidTarget() && InAutoAttackRange(_prevMinion))
                        {
                            var predHealth = HealthPrediction.LaneClearHealthPrediction(
                                _prevMinion, (int)((Player.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay);
                            if (predHealth >= 2 * Player.GetAutoAttackDamage(_prevMinion) ||
                                Math.Abs(predHealth - _prevMinion.Health) < float.Epsilon)
                            {
                                return _prevMinion;
                            }
                        }

                        result = (from minion in
                                      ObjectManager.Get<Obj_AI_Minion>()
                                          .Where(minion => minion.IsValidTarget() && InAutoAttackRange(minion) && minion.IsHPBarRendered && !minion.IsDead && minion.IsHPBarRendered && minion.IsTargetable &&
                                          (_config.Item("AttackWards").GetValue<bool>() || !MinionManager.IsWard(minion.CharData.BaseSkinName.ToLower())) &&
                                          (_config.Item("AttackPetsnTraps").GetValue<bool>() && minion.CharData.BaseSkinName != "jarvanivstandard" || MinionManager.IsMinion(minion, _config.Item("AttackWards").GetValue<bool>())) &&
                                          minion.CharData.BaseSkinName != "gangplankbarrel")
                                  let predHealth =
                                      HealthPrediction.LaneClearHealthPrediction(
                                          minion, (int)((Player.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay)
                                  where
                                      predHealth >= 2 * Player.GetAutoAttackDamage(minion) ||
                                      Math.Abs(predHealth - minion.Health) < float.Epsilon
                                  select minion).MaxOrDefault(m => !MinionManager.IsMinion(m, true) ? float.MaxValue : m.Health);

                        if (result != null)
                        {
                            _prevMinion = (Obj_AI_Minion)result;
                        }
                    }
                }
                return result;
            }

            private void GameOnOnGameUpdate(EventArgs args)
            {
                try
                {
                    if (ActiveMode == OrbwalkingMode.None)
                    {
                        return;
                    }

                    //Prevent canceling important spells
                    if (Player.IsCastingInterruptableSpell(true))
                    {
                        return;
                    }

                    var target = GetTarget();
                    Orbwalk(
                        target, (_orbwalkingPoint.To2D().IsValid()) ? _orbwalkingPoint : Game.CursorPos,
                        200,
                        _config.Item("HoldPosRadius").GetValue<Slider>().Value);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            private void DrawingOnOnDraw(EventArgs args)
            {
                if (_config.Item("AACircle").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(
                        Player.Position, GetRealAutoAttackRange(null) + 65,
                        _config.Item("AACircle").GetValue<Circle>().Color,
                        _config.Item("AALineWidth").GetValue<Slider>().Value);
                }

                if (_config.Item("AACircle2").GetValue<Circle>().Active)
                {
                    foreach (var target in
                        HeroManager.Enemies.FindAll(target => target.IsValidTarget(1175)))
                    {
                        Render.Circle.DrawCircle(
                            target.Position, GetAttackRange(target),
                            _config.Item("AACircle2").GetValue<Circle>().Color,
                            _config.Item("AALineWidth").GetValue<Slider>().Value);
                    }
                }

                if (_config.Item("HoldZone").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(
                        Player.Position, _config.Item("HoldPosRadius").GetValue<Slider>().Value,
                        _config.Item("HoldZone").GetValue<Circle>().Color,
                        _config.Item("AALineWidth").GetValue<Slider>().Value, true);
                }

            }
        }
    }
}
