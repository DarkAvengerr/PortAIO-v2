using EloBuddy;
using LeagueSharp.Common;
namespace Flowers_Nidalee
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Color = System.Drawing.Color;

    public static class Orbwalking
    {
        public static int _delay;
        public static int LastAATick;
        public static int LastMoveCommandT;
        public static int LastAttackCommandT;
        public static int _autoattackCounter;
        public static bool Move = true;
        public static bool Attack = true;
        public static bool _missileLaunched;
        public static bool DisableNextAttack;
        public static float _minDistance = 400;
        public static AttackableUnit _lastTarget;
        public static Vector3 LastMoveCommandPosition = Vector3.Zero;
        public static readonly AIHeroClient Player;
        public static readonly string _championName;
        public static readonly string[] NoCancelChamps = { "Kalista" };
        public static readonly Random _random = new Random(DateTime.Now.Millisecond);

        public static readonly string[] AttackResets =
            {
                "dariusnoxiantacticsonh", "fiorae", "garenq", "gravesmove",
                "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge",
                "leonashieldofdaybreak", "luciane", "monkeykingdoubleattack",
                "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze",
                "netherblade", "gangplankqwrapper", "powerfist",
                "renektonpreexecute", "rengarq", "shyvanadoubleattack",
                "sivirw", "takedown", "talonnoxiandiplomacy",
                "trundletrollsmash", "vaynetumble", "vie", "volibearq",
                "xenzhaocombotarget", "yorickspectral", "reksaiq",
                "itemtitanichydracleave", "masochism", "illaoiw",
                "elisespiderw", "fiorae", "meditate", "sejuaninorthernwinds",
                "asheq"
            };
        public static readonly string[] Attacks =
            {
                "caitlynheadshotmissile", "frostarrow", "garenslash2",
                "kennenmegaproc", "masteryidoublestrike", "quinnwenhanced",
                "renektonexecute", "renektonsuperexecute",
                "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust",
                "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff",
                "lucianpassiveshot"
            };
        public static readonly string[] NoAttacks =
            {
                "volleyattack", "volleyattackwithsound",
                "jarvanivcataclysmattack", "monkeykingdoubleattack",
                "shyvanadoubleattack", "shyvanadoubleattackdragon",
                "zyragraspingplantattack", "zyragraspingplantattack2",
                "zyragraspingplantattackfire", "zyragraspingplantattack2fire",
                "viktorpowertransfer", "sivirwattackbounce", "asheqattacknoonhit",
                "elisespiderlingbasicattack", "heimertyellowbasicattack",
                "heimertyellowbasicattack2", "heimertbluebasicattack",
                "annietibbersbasicattack", "annietibbersbasicattack2",
                "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack",
                "yorickspectralghoulbasicattack", "malzaharvoidlingbasicattack",
                "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
                "kindredwolfbasicattack"
            };

        static Orbwalking()
        {
            Player = ObjectManager.Player;
            _championName = Player.ChampionName;

            Obj_AI_Base.OnSpellCast += new Obj_AI_BaseDoCastSpell(Obj_AI_Base_OnSpellCast);
            Obj_AI_Base.OnBasicAttack += new Obj_AI_BaseOnBasicAttack(OnBasicAttack);
            Obj_AI_Base.OnProcessSpellCast += new Obj_AI_ProcessSpellCast(OnProcessSpell);

            if (Player.ChampionName != "Jinx")
                Spellbook.OnStopCast += new SpellbookStopCast(SpellbookOnStopCast);

            if (_championName == "Rengar")
            {
                Obj_AI_Base.OnPlayAnimation += delegate (Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
                {
                    if (sender.IsMe && args.Animation == "Spell5")
                    {
                        var t = 0;

                        if (_lastTarget != null && _lastTarget.IsValid)
                        {
                            t += (int)Math.Min(ObjectManager.Player.Distance(_lastTarget) / 1.5f, 0.6f);
                        }

                        LastAATick = Utils.GameTimeTickCount - Game.Ping / 2 + t;
                    }
                };
            }
        }

        public delegate void AfterAttackEvenH(AttackableUnit unit, AttackableUnit target);
        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
        public delegate void OnAttackEvenH(AttackableUnit unit, AttackableUnit target);
        public delegate void OnNonKillableMinionH(AttackableUnit minion);
        public delegate void OnTargetChangeH(AttackableUnit oldTarget, AttackableUnit newTarget);

        public static event AfterAttackEvenH AfterAttack;
        public static event BeforeAttackEvenH BeforeAttack;
        public static event OnAttackEvenH OnAttack;
        public static event OnNonKillableMinionH OnNonKillableMinion;
        public static event OnTargetChangeH OnTargetChange;

        public enum OrbwalkingMode
        {
            LastHit,
            Mixed,
            LaneClear,
            Combo,
            Freeze,
            CustomMode,
            None,
            Flee
        }

        public static bool CanAttack()
        {
            if (Player.ChampionName == "Graves")
            {
                var attackDelay = 1.0740296828d * 1000 * Player.AttackDelay - 716.2381256175d;

                if (Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + attackDelay && Player.HasBuff("GravesBasicAttackAmmo1"))
                {
                    return true;
                }

                return false;
            }

            if (Player.ChampionName == "Jhin")
            {
                if (Player.HasBuff("JhinPassiveReload"))
                {
                    return false;
                }
            }

            if (Player.IsCastingInterruptableSpell())
            {
                return false;
            }

            return Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + Player.AttackDelay * 1000;
        }

        public static bool CanMove(float extraWindup, bool disableMissileCheck = false)
        {
            if (_missileLaunched && Orbwalker.MissileCheck && !disableMissileCheck)
            {
                return true;
            }

            var localExtraWindup = 0;

            if (_championName == "Rengar" && (Player.HasBuff("rengarqbase") || Player.HasBuff("rengarqemp")))
            {
                localExtraWindup = 200;
            }

            return NoCancelChamps.Contains(_championName) || (Utils.GameTimeTickCount + Game.Ping / 2 >= LastAATick + Player.AttackCastDelay * 1000 + extraWindup + localExtraWindup);
        }

        public static float GetAttackRange(AIHeroClient target)
        {
            var result = target.AttackRange + target.BoundingRadius;

            return result;
        }

        public static Vector3 GetLastMovePosition()
        {
            return LastMoveCommandPosition;
        }

        public static float GetLastMoveTime()
        {
            return LastMoveCommandT;
        }

        public static float GetMyProjectileSpeed()
        {
            return IsMelee(Player) || _championName == "Azir" || _championName == "Velkoz" || _championName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn") ? float.MaxValue : Player.BasicAttack.MissileSpeed;
        }

        public static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = Player.AttackRange + Player.BoundingRadius;

            if (target.IsValidTarget())
            {
                var aiBase = target as Obj_AI_Base;

                if (aiBase != null && Player.ChampionName == "Caitlyn")
                {
                    if (aiBase.HasBuff("caitlynyordletrapinternal"))
                    {
                        result += 650;
                    }
                }

                return result + target.BoundingRadius;
            }

            return result;
        }

        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.IsValidTarget())
            {
                return false;
            }

            var myRange = GetRealAutoAttackRange(target);

            return Vector2.DistanceSquared(target is Obj_AI_Base ? ((Obj_AI_Base)target).ServerPosition.To2D() : target.Position.To2D(), Player.ServerPosition.To2D()) <= myRange * myRange;
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) || Attacks.Contains(name.ToLower());
        }

        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static bool IsMelee(this Obj_AI_Base unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }

        public static void MoveTo(Vector3 position, float holdAreaRadius = 0, bool overrideTimer = false, bool useFixedDistance = true, bool randomizeMinDistance = true)
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
                point = playerPosition.Extend(position, randomizeMinDistance ? (_random.NextFloat(0.6f, 1) + 0.2f) * _minDistance : _minDistance);
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

            if (Utils.GameTimeTickCount - LastMoveCommandT < 70 + Math.Min(60, Game.Ping) && !overrideTimer && angle < 60)
            {
                return;
            }

            if (angle >= 60 && Utils.GameTimeTickCount - LastMoveCommandT < 60)
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, point);
            LastMoveCommandPosition = point;
            LastMoveCommandT = Utils.GameTimeTickCount;
        }

        public static void Orbwalk(AttackableUnit target, Vector3 position, float extraWindup = 90, float holdAreaRadius = 0, bool useFixedDistance = true, bool randomizeMinDistance = true)
        {
            if (Utils.GameTimeTickCount - LastAttackCommandT < 70 + Math.Min(60, Game.Ping))
            {
                return;
            }

            try
            {
                if (target.IsValidTarget() && CanAttack() && Attack)
                {
                    DisableNextAttack = false;
                    FireBeforeAttack(target);

                    if (!DisableNextAttack)
                    {
                        if (!NoCancelChamps.Contains(_championName))
                        {
                            _missileLaunched = false;
                        }

                        if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                        {
                            LastAttackCommandT = Utils.GameTimeTickCount;
                            _lastTarget = target;
                        }

                        return;
                    }
                }

                if (CanMove(extraWindup) && Move)
                {
                    if (Orbwalker.LimitAttackSpeed && (Player.AttackDelay < 1 / 2.6f) && _autoattackCounter % 3 != 0 && !CanMove(500, true))
                    {
                        return;
                    }

                    MoveTo(position, Math.Max(holdAreaRadius, 30), false, useFixedDistance, randomizeMinDistance);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ResetAutoAttackTimer()
        {
            LastAATick = 0;
        }

        public static void SetMinimumOrbwalkDistance(float d)
        {
            _minDistance = d;
        }

        public static void SetMovementDelay(int delay)
        {
            _delay = delay;
        }

        private static void FireAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (AfterAttack != null && target.IsValidTarget())
            {
                AfterAttack(unit, target);
            }
        }

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

        private static void FireOnNonKillableMinion(AttackableUnit minion)
        {
            if (OnNonKillableMinion != null)
            {
                OnNonKillableMinion(minion);
            }
        }

        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null && (!_lastTarget.IsValidTarget() || _lastTarget != newTarget))
            {
                OnTargetChange(_lastTarget, newTarget);
            }
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                var ping = Game.Ping;
                if (ping <= 30) //First world problems kappa
                {
                    Utility.DelayAction.Add(30 - ping, () => Obj_AI_Base_OnSpellCast_Delayed(sender, args));
                    return;
                }

                Obj_AI_Base_OnSpellCast_Delayed(sender, args); //InvokeActionAfterAttackDelay()
            }
        }

        private static void Obj_AI_Base_OnSpellCast_Delayed(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (IsAutoAttackReset(args.SData.Name))
            {
                ResetAutoAttackTimer();
            }

            if (IsAutoAttack(args.SData.Name))
            {
                FireAfterAttack(sender, args.Target as AttackableUnit);
                _missileLaunched = true;
            }
        }

        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (IsAutoAttack(args.SData.Name))
                {
                    var target = args.Target as AttackableUnit;

                    if (target != null && target.IsValid)
                    {
                        FireOnAttack(sender, _lastTarget);
                    }
                }
                if (IsAutoAttackReset(args.SData.Name) && Math.Abs(args.SData.CastTime) < 1.401298E-45f)
                {
                    ResetAutoAttackTimer();
                }
            }
        }

        private static void SpellbookOnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMe && EloBuddy.SDK.Orbwalker.IsRanged && (args.DestroyMissile && args.StopAnimation))
            {
                ResetAutoAttackTimer();
            }
        }

        private static void OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (IsAutoAttack(args.SData.Name))
                {
                    var target = args.Target as AttackableUnit;

                    if (target != null && target.IsValid)
                    {
                        FireOnAttack(sender, _lastTarget);
                    }
                }
            }

            if (sender.IsMe && (args.Target is Obj_AI_Base || args.Target is Obj_BarracksDampener || args.Target is Obj_HQ))
            {
                LastAATick = Utils.GameTimeTickCount - Game.Ping / 2;
                _missileLaunched = false;
                LastMoveCommandT = 0;
                _autoattackCounter++;

                if (args.Target is Obj_AI_Base)
                {
                    var target = (Obj_AI_Base)args.Target;
                    if (target.IsValid)
                    {
                        FireOnTargetSwitch(target);
                        _lastTarget = target;
                    }
                }
            }

            if (sender is Obj_AI_Turret && args.Target is Obj_AI_Base)
            {
                LastTargetTurrets[sender.NetworkId] = (Obj_AI_Base)args.Target;
            }
        }

        internal static readonly Dictionary<int, Obj_AI_Base> LastTargetTurrets = new Dictionary<int, Obj_AI_Base>();

        public class BeforeAttackEventArgs : EventArgs
        {
            public AttackableUnit Target;
            public Obj_AI_Base Unit = ObjectManager.Player;
            private bool _process = true;

            public bool Process
            {
                get
                {
                    return this._process;
                }
                set
                {
                    DisableNextAttack = !value;
                    this._process = value;
                }
            }
        }

        public class Orbwalker
        {
            private const float LaneClearWaitTimeMod = 2f;
            public static List<Orbwalker> Instances = new List<Orbwalker>();
            private static Menu Menu;
            private readonly AIHeroClient Player;
            private Obj_AI_Base _forcedTarget;
            private OrbwalkingMode _mode = OrbwalkingMode.None;
            private Vector3 _orbwalkingPoint;
            private Obj_AI_Minion _prevMinion;
            private string CustomModeName;

            public Orbwalker(Menu attachToMenu)
            {
                Menu = attachToMenu;

                var drawingsMenu = Menu.AddSubMenu(new Menu("Drawings", "drawings"));
                {
                    drawingsMenu.AddItem(new MenuItem("AACircle", "AACircle").SetValue(new Circle(true, Color.FromArgb(155, 255, 255, 0))));
                    drawingsMenu.AddItem(new MenuItem("AACircle2", "Enemy AA circle").SetValue(new Circle(false, Color.FromArgb(155, 255, 255, 0))));
                    drawingsMenu.AddItem(new MenuItem("AALineWidth", "Line Width")).SetValue(new Slider(2, 1, 6));
                }

                var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
                {
                    miscMenu.AddItem(new MenuItem("MissileCheck", "Use Missile Check").SetValue(true));
                    miscMenu.AddItem(new MenuItem("HoldPosRadius", "Hold Position Radius").SetValue(new Slider(0, 0, 250)));
                    miscMenu.AddItem(new MenuItem("LimitAttackSpeed", "Don't kite if Attack Speed > 2.5").SetValue(false));
                }

                var priorityMenu = Menu.AddSubMenu(new Menu("Priority", "Priority"));
                {
                    priorityMenu.AddItem(new MenuItem("PriorizeFarm", "Priorize farm over harass").SetValue(true));
                    priorityMenu.AddItem(new MenuItem("AttackWards", "Auto attack wards").SetValue(false));
                    priorityMenu.AddItem(new MenuItem("AttackPetsnTraps", "Auto attack pets & traps").SetValue(true));
                    priorityMenu.AddItem(new MenuItem("AttackGPBarrel", "Auto attack gangplank barrel").SetValue(new StringList(new[] { "Combo and Farming", "Farming", "No" }, 1)));
                    priorityMenu.AddItem(new MenuItem("Smallminionsprio", "Jungle clear small first").SetValue(false));
                    priorityMenu.AddItem(new MenuItem("FocusMinionsOverTurrets", "Focus minions over objectives").SetValue(new KeyBind('M', KeyBindType.Toggle)));
                }

                var delayMenu = Menu.AddSubMenu(new Menu("Delay", "Delay"));
                {
                    delayMenu.AddItem(new MenuItem("ExtraWindup", "Extra windup time").SetValue(new Slider(80, 0, 200)));
                    delayMenu.AddItem(new MenuItem("FarmDelay", "Farm delay").SetValue(new Slider(0, 0, 200)));
                }

                var orbwalkerKeyMenu = Menu.AddSubMenu(new Menu("Keys", "Keys"));
                {
                    orbwalkerKeyMenu.AddItem(new MenuItem("Flee", "Flee").SetValue(new KeyBind('Z', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(new MenuItem("LastHit", "Last hit").SetValue(new KeyBind('X', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(new MenuItem("Farm", "Mixed").SetValue(new KeyBind('C', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(new MenuItem("Freeze", "Freeze").SetValue(new KeyBind('N', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(new MenuItem("LaneClear", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(new MenuItem("Orbwalk", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                }

                this.Player = ObjectManager.Player;
                Game.OnUpdate += this.GameOnOnGameUpdate;
                Drawing.OnDraw += this.DrawingOnOnDraw;
                Instances.Add(this);
            }

            public static bool LimitAttackSpeed
            {
                get
                {
                    return Menu.Item("LimitAttackSpeed").GetValue<bool>();
                }
            }

            public static bool MissileCheck
            {
                get
                {
                    return Menu.Item("MissileCheck").GetValue<bool>();
                }
            }

            public OrbwalkingMode ActiveMode
            {
                get
                {
                    if (this._mode != OrbwalkingMode.None)
                    {
                        return this._mode;
                    }

                    if (Menu.Item("Orbwalk").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Combo;
                    }

                    if (Menu.Item("LaneClear").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LaneClear;
                    }

                    if (Menu.Item("Farm").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Mixed;
                    }

                    if (Menu.Item("Freeze").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Freeze;
                    }

                    if (Menu.Item("LastHit").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LastHit;
                    }

                    if (Menu.Item("Flee").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Flee;
                    }

                    if (Menu.Item(this.CustomModeName) != null && Menu.Item(this.CustomModeName).GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.CustomMode;
                    }

                    return OrbwalkingMode.None;
                }
                set
                {
                    this._mode = value;
                }
            }

            private int FarmDelay
            {
                get
                {
                    return Menu.Item("FarmDelay").GetValue<Slider>().Value;
                }
            }

            public void ForceTarget(Obj_AI_Base target)
            {
                this._forcedTarget = target;
            }

            public virtual AttackableUnit GetTarget()
            {
                AttackableUnit result = null;

                var mode = this.ActiveMode;

                if ((mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LaneClear) && !Menu.Item("PriorizeFarm").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);

                    if (target != null && this.InAutoAttackRange(target))
                    {
                        return target;
                    }
                }

                var attackGankPlankBarrels = Menu.Item("AttackGPBarrel").GetValue<StringList>().SelectedIndex;

                if (attackGankPlankBarrels != 2 && (attackGankPlankBarrels == 0 || (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)))
                {
                    var enemyGangPlank = HeroManager.Enemies.FirstOrDefault(e => e.ChampionName.Equals("gangplank", StringComparison.InvariantCultureIgnoreCase));

                    if (enemyGangPlank != null)
                    {
                        var barrels = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.Team == GameObjectTeam.Neutral && minion.CharData.BaseSkinName == "gangplankbarrel" && minion.IsHPBarRendered && minion.IsValidTarget() && this.InAutoAttackRange(minion));

                        foreach (var barrel in barrels)
                        {
                            if (barrel.Health <= 1f)
                            {
                                return barrel;
                            }

                            var t = (int)(this.Player.AttackCastDelay * 1000) + Game.Ping / 2 + 1000 * (int)Math.Max(0, this.Player.Distance(barrel) - this.Player.BoundingRadius) / (int)GetMyProjectileSpeed();
                            var barrelBuff = barrel.Buffs.FirstOrDefault(b => b.Name.Equals("gangplankebarrelactive", StringComparison.InvariantCultureIgnoreCase));

                            if (barrelBuff != null && barrel.Health <= 2f)
                            {
                                var healthDecayRate = enemyGangPlank.Level >= 13 ? 0.5f : (enemyGangPlank.Level >= 7 ? 1f : 2f);
                                var nextHealthDecayTime = Game.Time < barrelBuff.StartTime + healthDecayRate ? barrelBuff.StartTime + healthDecayRate : barrelBuff.StartTime + healthDecayRate * 2;

                                if (nextHealthDecayTime <= Game.Time + t / 1000f)
                                {
                                    return barrel;
                                }
                            }
                        }

                        if (barrels.Any())
                        {
                            return null;
                        }
                    }
                }

                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)
                {
                    var MinionList = ObjectManager.Get<Obj_AI_Minion>()
                            .Where(minion => minion.IsValidTarget() && this.InAutoAttackRange(minion))
                            .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                            .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                            .ThenBy(minion => minion.Health)
                            .ThenByDescending(minion => minion.MaxHealth);

                    foreach (var minion in MinionList)
                    {
                        var t = (int)(this.Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 + 1000 * (int)Math.Max(0, this.Player.Distance(minion) - this.Player.BoundingRadius) / (int)GetMyProjectileSpeed();

                        if (mode == OrbwalkingMode.Freeze)
                        {
                            t += 200 + Game.Ping / 2;
                        }

                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, this.FarmDelay);

                        if (minion.Team != GameObjectTeam.Neutral && this.ShouldAttackMinion(minion))
                        {
                            var damage = this.Player.GetAutoAttackDamage(minion, true);
                            var killable = predHealth <= damage;

                            if (mode == OrbwalkingMode.Freeze)
                            {
                                if (minion.Health < 50 || predHealth <= 50)
                                {
                                    return minion;
                                }
                            }
                            else
                            {
                                if (predHealth <= 0)
                                {
                                    FireOnNonKillableMinion(minion);
                                }

                                if (killable)
                                {
                                    return minion;
                                }
                            }
                        }
                    }
                }

                if (this._forcedTarget.IsValidTarget() && this.InAutoAttackRange(this._forcedTarget))
                {
                    return this._forcedTarget;
                }

                if (mode == OrbwalkingMode.LaneClear && (!Menu.Item("FocusMinionsOverTurrets").GetValue<KeyBind>().Active || !MinionManager.GetMinions(ObjectManager.Player.Position, GetRealAutoAttackRange(ObjectManager.Player)).Any()))
                {

                    foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && this.InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    foreach (var turret in ObjectManager.Get<Obj_BarracksDampener>().Where(t => t.IsValidTarget() && this.InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    foreach (var nexus in ObjectManager.Get<Obj_HQ>().Where(t => t.IsValidTarget() && this.InAutoAttackRange(t)))
                    {
                        return nexus;
                    }
                }

                if (mode != OrbwalkingMode.LastHit)
                {
                    if (mode != OrbwalkingMode.LaneClear || !this.ShouldWait())
                    {
                        var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);

                        if (target.IsValidTarget() && this.InAutoAttackRange(target))
                        {
                            return target;
                        }
                    }
                }

                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed)
                {
                    var jminions = ObjectManager.Get<Obj_AI_Minion>().Where(mob => mob.IsValidTarget() && mob.Team == GameObjectTeam.Neutral && this.InAutoAttackRange(mob) && mob.CharData.BaseSkinName != "gangplankbarrel" && mob.Name != "WardCorpse");

                    result = Menu.Item("Smallminionsprio").GetValue<bool>() ? jminions.MinOrDefault(mob => mob.MaxHealth) : jminions.MaxOrDefault(mob => mob.MaxHealth);

                    if (result != null)
                    {
                        return result;
                    }
                }

                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze)
                {
                    var closestTower = ObjectManager.Get<Obj_AI_Turret>().MinOrDefault(t => t.IsAlly && !t.IsDead ? this.Player.Distance(t, true) : float.MaxValue);

                    if (closestTower != null && this.Player.Distance(closestTower, true) < 1500 * 1500)
                    {
                        Obj_AI_Minion farmUnderTurretMinion = null;
                        Obj_AI_Minion noneKillableMinion = null;

                        var minions = MinionManager.GetMinions(this.Player.Position, this.Player.AttackRange + 200)
                                .Where(minion => this.InAutoAttackRange(minion) && closestTower.Distance(minion, true) < 900 * 900)
                                .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                                .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                                .ThenByDescending(minion => minion.MaxHealth)
                                .ThenByDescending(minion => minion.Health);

                        if (minions.Any())
                        {
                            var turretMinion = minions.FirstOrDefault(minion => minion is Obj_AI_Minion && HealthPrediction.HasTurretAggro(minion as Obj_AI_Minion));

                            if (turretMinion != null)
                            {
                                var hpLeftBeforeDie = 0;
                                var hpLeft = 0;
                                var turretAttackCount = 0;
                                var turretStarTick = HealthPrediction.TurretAggroStartTick(turretMinion as Obj_AI_Minion);
                                var turretLandTick = turretStarTick + (int)(closestTower.AttackCastDelay * 1000) + 1000 * Math.Max(0, (int)(turretMinion.Distance(closestTower) - closestTower.BoundingRadius)) / (int)(closestTower.BasicAttack.MissileSpeed + 70);

                                for (float i = turretLandTick + 50; i < turretLandTick + 10 * closestTower.AttackDelay * 1000 + 50; i = i + closestTower.AttackDelay * 1000)
                                {
                                    var time = (int)i - Utils.GameTimeTickCount + Game.Ping / 2;
                                    var predHP = (int)HealthPrediction.LaneClearHealthPrediction(turretMinion, time > 0 ? time : 0);

                                    if (predHP > 0)
                                    {
                                        hpLeft = predHP;
                                        turretAttackCount += 1;

                                        continue;
                                    }

                                    hpLeftBeforeDie = hpLeft;
                                    hpLeft = 0;

                                    break;
                                }

                                if (hpLeft == 0 && turretAttackCount != 0 && hpLeftBeforeDie != 0)
                                {
                                    var damage = (int)this.Player.GetAutoAttackDamage(turretMinion, true);
                                    var hits = hpLeftBeforeDie / damage;
                                    var timeBeforeDie = turretLandTick + (turretAttackCount + 1) * (int)(closestTower.AttackDelay * 1000) - Utils.GameTimeTickCount;
                                    var timeUntilAttackReady = LastAATick + (int)(this.Player.AttackDelay * 1000) > Utils.GameTimeTickCount + Game.Ping / 2 + 25 ? LastAATick + (int)(this.Player.AttackDelay * 1000) - (Utils.GameTimeTickCount + Game.Ping / 2 + 25) : 0;
                                    var timeToLandAttack = this.Player.IsMelee ? this.Player.AttackCastDelay * 1000 : this.Player.AttackCastDelay * 1000 + 1000 * Math.Max(0, turretMinion.Distance(this.Player) - this.Player.BoundingRadius) / this.Player.BasicAttack.MissileSpeed;

                                    if (hits >= 1 && hits * this.Player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack < timeBeforeDie)
                                    {
                                        farmUnderTurretMinion = turretMinion as Obj_AI_Minion;
                                    }
                                    else if (hits >= 1 && hits * this.Player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack > timeBeforeDie)
                                    {
                                        noneKillableMinion = turretMinion as Obj_AI_Minion;
                                    }
                                }
                                else if (hpLeft == 0 && turretAttackCount == 0 && hpLeftBeforeDie == 0)
                                {
                                    noneKillableMinion = turretMinion as Obj_AI_Minion;
                                }

                                if (this.ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }

                                if (farmUnderTurretMinion != null)
                                {
                                    return farmUnderTurretMinion;
                                }

                                foreach (var minion in minions.Where(x => x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion)))
                                {
                                    var playerDamage = (int)this.Player.GetAutoAttackDamage(minion);
                                    var turretDamage = (int)closestTower.GetAutoAttackDamage(minion, true);
                                    var leftHP = (int)minion.Health % turretDamage;

                                    if (leftHP > playerDamage)
                                    {
                                        return minion;
                                    }
                                }

                                var lastminion = minions.LastOrDefault(x => x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));

                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (1f / this.Player.AttackDelay >= 1f && (int)(turretAttackCount * closestTower.AttackDelay / this.Player.AttackDelay) * this.Player.GetAutoAttackDamage(lastminion) > lastminion.Health)
                                    {
                                        return lastminion;
                                    }

                                    if (minions.Count() >= 5 && 1f / this.Player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            else
                            {
                                if (this.ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }

                                foreach (var minion in minions.Where(x => x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion)))
                                {
                                    if (closestTower != null)
                                    {
                                        var playerDamage = (int)this.Player.GetAutoAttackDamage(minion);
                                        var turretDamage = (int)closestTower.GetAutoAttackDamage(minion, true);
                                        var leftHP = (int)minion.Health % turretDamage;
                                        if (leftHP > playerDamage)
                                        {
                                            return minion;
                                        }
                                    }
                                }

                                var lastminion = minions.LastOrDefault(x => x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));

                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (minions.Count() >= 5 && 1f / this.Player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            return null;
                        }
                    }
                }

                if (mode == OrbwalkingMode.LaneClear)
                {
                    if (!this.ShouldWait())
                    {
                        if (this._prevMinion.IsValidTarget() && this.InAutoAttackRange(this._prevMinion))
                        {
                            var predHealth = HealthPrediction.LaneClearHealthPrediction(this._prevMinion, (int)(this.Player.AttackDelay * 1000 * LaneClearWaitTimeMod), this.FarmDelay);

                            if (predHealth >= 2 * this.Player.GetAutoAttackDamage(this._prevMinion) || Math.Abs(predHealth - this._prevMinion.Health) < float.Epsilon)
                            {
                                return this._prevMinion;
                            }
                        }

                        result = (from minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && this.InAutoAttackRange(minion) && this.ShouldAttackMinion(minion))
                                  let predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)(this.Player.AttackDelay * 1000 * LaneClearWaitTimeMod), this.FarmDelay)
                                  where predHealth >= 2 * this.Player.GetAutoAttackDamage(minion) || Math.Abs(predHealth - minion.Health) < float.Epsilon
                                  select minion).MaxOrDefault(m => !MinionManager.IsMinion(m, true) ? float.MaxValue : m.Health);

                        if (result != null)
                        {
                            this._prevMinion = (Obj_AI_Minion)result;
                        }
                    }
                }

                return result;
            }

            public virtual bool InAutoAttackRange(AttackableUnit target)
            {
                return Orbwalking.InAutoAttackRange(target);
            }

            public virtual void RegisterCustomMode(string name, string displayname, uint key)
            {
                this.CustomModeName = name;

                if (Menu.Item(name) == null)
                {
                    Menu.AddItem(new MenuItem(name, displayname).SetValue(new KeyBind(key, KeyBindType.Press)));
                }
            }

            public void SetAttack(bool b)
            {
                Attack = b;
            }

            public void SetMovement(bool b)
            {
                Move = b;
            }

            public void SetOrbwalkingPoint(Vector3 point)
            {
                this._orbwalkingPoint = point;
            }

            public bool ShouldWait()
            {
                return ObjectManager.Get<Obj_AI_Minion>().Any(minion => minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral && this.InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false) && HealthPrediction.LaneClearHealthPrediction(minion, (int)(this.Player.AttackDelay * 1000 * LaneClearWaitTimeMod), this.FarmDelay) <= this.Player.GetAutoAttackDamage(minion));
            }

            private void DrawingOnOnDraw(EventArgs args)
            {
                if (ObjectManager.Player.IsDead || Shop.IsOpen || MenuGUI.IsChatOpen)
                {
                    return;
                }

                if (Menu.Item("AACircle").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(this.Player.Position, GetRealAutoAttackRange(null) + 65, Menu.Item("AACircle").GetValue<Circle>().Color, Menu.Item("AALineWidth").GetValue<Slider>().Value);
                }

                if (Menu.Item("AACircle2").GetValue<Circle>().Active)
                {
                    foreach (var target in HeroManager.Enemies.FindAll(target => target.IsValidTarget(1175)))
                    {
                        Render.Circle.DrawCircle(target.Position, GetAttackRange(target), Menu.Item("AACircle2").GetValue<Circle>().Color, Menu.Item("AALineWidth").GetValue<Slider>().Value);
                    }
                }
            }

            private void GameOnOnGameUpdate(EventArgs args)
            {
                try
                {
                    if (this.ActiveMode == OrbwalkingMode.None)
                    {
                        return;
                    }

                    var target = this.GetTarget();

                    Orbwalk(target, this._orbwalkingPoint.To2D().IsValid() ? this._orbwalkingPoint : Game.CursorPos, Menu.Item("ExtraWindup").GetValue<Slider>().Value, Math.Max(Menu.Item("HoldPosRadius").GetValue<Slider>().Value, 30));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            private bool ShouldAttackMinion(Obj_AI_Minion minion)
            {
                if (minion.Name == "WardCorpse" || minion.CharData.BaseSkinName == "jarvanivstandard")
                {
                    return false;
                }

                if (MinionManager.IsWard(minion))
                {
                    return Menu.Item("AttackWards").IsActive();
                }

                return (Menu.Item("AttackPetsnTraps").GetValue<bool>() || MinionManager.IsMinion(minion)) && minion.CharData.BaseSkinName != "gangplankbarrel";
            }

            private bool ShouldWaitUnderTurret(Obj_AI_Minion noneKillableMinion)
            {
                return ObjectManager.Get<Obj_AI_Minion>().Any(minion => (noneKillableMinion != null ? noneKillableMinion.NetworkId != minion.NetworkId : true) && minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral && this.InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false) && HealthPrediction.LaneClearHealthPrediction(minion, (int)(this.Player.AttackDelay * 1000 + (this.Player.IsMelee ? this.Player.AttackCastDelay * 1000 : this.Player.AttackCastDelay * 1000 + 1000 * (this.Player.AttackRange + 2 * this.Player.BoundingRadius) / this.Player.BasicAttack.MissileSpeed)), this.FarmDelay) <= this.Player.GetAutoAttackDamage(minion));
            }
        }
    }
}