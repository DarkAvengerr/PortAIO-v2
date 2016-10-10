using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Color = System.Drawing.Color;
    using static EloBuddy.ObjectManager;

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
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Spellbook.OnStopCast += SpellbookOnStopCast;

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
            Combo,
            Mixed,
            LaneClear,
            LastHit,
            Freeze,
            Flee,
            CustomMode,
            None
        }

        public static bool CanAttack()
        {
            if (Player.ChampionName == "Graves")
            {
                var attackDelay = 1.0740296828d * 1000 * Player.AttackDelay - 716.2381256175d;

                return Utils.GameTimeTickCount + Game.Ping/2 + 25 >= LastAATick + attackDelay &&
                       Player.HasBuff("GravesBasicAttackAmmo1");
            }

            if (Player.ChampionName == "Jhin")
            {
                return !Player.HasBuff("JhinPassiveReload");
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

            return NoCancelChamps.Contains(_championName) ||
                   (Utils.GameTimeTickCount + Game.Ping / 2 >=
                    LastAATick + Player.AttackCastDelay * 1000 + extraWindup + localExtraWindup);
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
            return IsMelee(Player) || _championName == "Azir" || _championName == "Velkoz" ||
                   _championName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn")
                ? float.MaxValue
                : Player.BasicAttack.MissileSpeed;
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

            var @base = target as Obj_AI_Base;

            return
                Vector2.DistanceSquared(@base?.ServerPosition.To2D() ?? target.Position.To2D(),
                    Player.ServerPosition.To2D()) <= myRange * myRange;
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) ||
                   Attacks.Contains(name.ToLower());
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
                point = playerPosition.Extend(position,
                    randomizeMinDistance ? (_random.NextFloat(0.6f, 1) + 0.2f) * _minDistance : _minDistance);
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
                if (Orbwalker.LimitAttackSpeed && (Player.AttackDelay < 1 / 2.6f) && _autoattackCounter % 3 != 0 &&
                    !CanMove(500, true))
                {
                    return;
                }

                MoveTo(position, Math.Max(holdAreaRadius, 30), false, useFixedDistance, randomizeMinDistance);
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
            OnNonKillableMinion?.Invoke(minion);
        }

        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null && (!_lastTarget.IsValidTarget() || _lastTarget != newTarget))
            {
                OnTargetChange(_lastTarget, newTarget);
            }
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                var ping = Game.Ping;

                if (ping <= 30)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(30 - ping,
                        () => Obj_AI_Base_OnSpellCast_Delayed(sender, Args));

                    return;
                }

                Obj_AI_Base_OnSpellCast_Delayed(sender, Args);
            }
        }

        private static void Obj_AI_Base_OnSpellCast_Delayed(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (IsAutoAttackReset(Args.SData.Name))
            {
                ResetAutoAttackTimer();
            }

            if (IsAutoAttack(Args.SData.Name))
            {
                FireAfterAttack(sender, Args.Target as AttackableUnit);
                _missileLaunched = true;
            }
        }

        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Spell)
        {
            var spellName = Spell.SData.Name;

            if (sender.IsMe && IsAutoAttackReset(spellName) && Spell.SData.SpellCastTime == 0)
            {
                ResetAutoAttackTimer();
            }

            if (!IsAutoAttack(spellName))
            {
                return;
            }

            if (sender.IsMe &&
                (Spell.Target is Obj_AI_Base || Spell.Target is Obj_BarracksDampener || Spell.Target is Obj_HQ))
            {
                LastAATick = Utils.GameTimeTickCount - Game.Ping / 2;
                _missileLaunched = false;
                LastMoveCommandT = 0;
                _autoattackCounter++;

                var @base = Spell.Target as Obj_AI_Base;

                if (@base != null)
                {
                    var target = @base;

                    if (target.IsValid)
                    {
                        FireOnTargetSwitch(target);
                        _lastTarget = target;
                    }
                }
            }

            FireOnAttack(sender, _lastTarget);
        }

        private static void SpellbookOnStopCast(Obj_AI_Base spellbook, SpellbookStopCastEventArgs Args)
        {
            if (spellbook.IsValid && spellbook.IsMe && Args.DestroyMissile && Args.StopAnimation)
            {
                ResetAutoAttackTimer();
            }
        }

        public class BeforeAttackEventArgs : EventArgs
        {
            public AttackableUnit Target;
            public Obj_AI_Base Unit = ObjectManager.Player;
            private bool _process = true;

            public bool Process
            {
                get
                {
                    return _process;
                }
                set
                {
                    DisableNextAttack = !value;
                    _process = value;
                }
            }
        }

        public class Orbwalker
        {
            private const float LaneClearWaitTimeMod = 2f;
            public static List<Orbwalker> Instances = new List<Orbwalker>();
            private static Menu Menu;
            private readonly AIHeroClient player;
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
                    drawingsMenu.AddItem(
                        new MenuItem("AACircle", "AACircle").SetValue(new Circle(true, Color.FromArgb(155, 255, 255, 0))));
                    drawingsMenu.AddItem(
                        new MenuItem("AACircle2", "Enemy AA circle").SetValue(new Circle(false,
                            Color.FromArgb(155, 255, 255, 0))));
                    drawingsMenu.AddItem(new MenuItem("AALineWidth", "Line Width")).SetValue(new Slider(2, 1, 6));
                }

                var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
                {
                    miscMenu.AddItem(new MenuItem("MissileCheck", "Use Missile Check").SetValue(true));
                    miscMenu.AddItem(
                        new MenuItem("HoldPosRadius", "Hold Position Radius").SetValue(new Slider(50, 0, 250)));
                    miscMenu.AddItem(new MenuItem("LimitAttackSpeed", "Don't kite if Attack Speed > 2.5").SetValue(false));
                }

                var priorityMenu = Menu.AddSubMenu(new Menu("Priority", "Priority"));
                {
                    priorityMenu.AddItem(new MenuItem("PriorizeFarm", "Priorize farm over harass").SetValue(true));
                    priorityMenu.AddItem(new MenuItem("AttackWards", "Auto attack wards").SetValue(false));
                    priorityMenu.AddItem(new MenuItem("AttackPetsnTraps", "Auto attack pets & traps").SetValue(true));
                    priorityMenu.AddItem(
                        new MenuItem("AttackGPBarrel", "Auto attack gangplank barrel").SetValue(
                            new StringList(new[] { "Combo and Farming", "Farming", "No" }, 1)));
                    priorityMenu.AddItem(new MenuItem("Smallminionsprio", "Jungle clear small first").SetValue(false));
                    priorityMenu.AddItem(
                        new MenuItem("FocusMinionsOverTurrets", "Focus minions over objectives").SetValue(
                            new KeyBind('M', KeyBindType.Toggle)));
                }

                var delayMenu = Menu.AddSubMenu(new Menu("Delay", "Delay"));
                {
                    delayMenu.AddItem(new MenuItem("ExtraWindup", "Extra windup time").SetValue(new Slider(80, 0, 200)));
                    delayMenu.AddItem(new MenuItem("FarmDelay", "Farm delay").SetValue(new Slider(0, 0, 200)));
                }

                var orbwalkerKeyMenu = Menu.AddSubMenu(new Menu("Keys", "Keys"));
                {
                    orbwalkerKeyMenu.AddItem(new MenuItem("Flee", "Flee").SetValue(new KeyBind('Z', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(
                        new MenuItem("LastHit", "Last hit").SetValue(new KeyBind('X', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(new MenuItem("Farm", "Mixed").SetValue(new KeyBind('C', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(
                        new MenuItem("Freeze", "Freeze").SetValue(new KeyBind('N', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(
                        new MenuItem("LaneClear", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
                    orbwalkerKeyMenu.AddItem(
                        new MenuItem("Orbwalk", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                }

                Menu.AddItem(new MenuItem("EnableOrbwalker", "Enable Orbwalker").SetValue(true));

                player = ObjectManager.Player;
                Game.OnUpdate += GameOnOnGameUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Instances.Add(this);
            }

            public static bool LimitAttackSpeed => Menu.Item("LimitAttackSpeed").GetValue<bool>();

            public static bool MissileCheck => Menu.Item("MissileCheck").GetValue<bool>();

            public OrbwalkingMode ActiveMode
            {
                get
                {
                    if (_mode != OrbwalkingMode.None)
                    {
                        return _mode;
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

                    if (Menu.Item(CustomModeName) != null && Menu.Item(CustomModeName).GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.CustomMode;
                    }

                    return OrbwalkingMode.None;
                }
                set
                {
                    _mode = value;
                }
            }

            private int FarmDelay => Menu.Item("FarmDelay").GetValue<Slider>().Value;

            public void ForceTarget(Obj_AI_Base target)
            {
                _forcedTarget = target;
            }

            public virtual AttackableUnit GetTarget()
            {
                AttackableUnit result = null;

                var mode = ActiveMode;

                if ((mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LaneClear) &&
                    !Menu.Item("PriorizeFarm").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);

                    if (target != null && InAutoAttackRange(target))
                    {
                        return target;
                    }
                }

                var attackGankPlankBarrels = Menu.Item("AttackGPBarrel").GetValue<StringList>().SelectedIndex;

                if (attackGankPlankBarrels != 2 &&
                    (attackGankPlankBarrels == 0 || mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed ||
                     mode == OrbwalkingMode.LastHit || mode == OrbwalkingMode.Freeze))
                {
                    var enemyGangPlank =
                        HeroManager.Enemies.FirstOrDefault(
                            e => e.ChampionName.Equals("gangplank", StringComparison.InvariantCultureIgnoreCase));

                    if (enemyGangPlank != null)
                    {
                        var barrels =
                            Get<Obj_AI_Minion>()
                                .Where(
                                    minion =>
                                        minion.Team == GameObjectTeam.Neutral &&
                                        minion.CharData.BaseSkinName == "gangplankbarrel" && minion.IsHPBarRendered &&
                                        minion.IsValidTarget() && InAutoAttackRange(minion));
                        var objAiMinions = barrels as Obj_AI_Minion[] ?? barrels.ToArray();

                        foreach (var barrel in objAiMinions)
                        {
                            if (barrel.Health <= 1f)
                            {
                                return barrel;
                            }

                            var t = (int)(player.AttackCastDelay * 1000) + Game.Ping / 2 +
                                    1000 * (int)Math.Max(0, player.Distance(barrel) - player.BoundingRadius) /
                                    (int)GetMyProjectileSpeed();
                            var barrelBuff =
                                barrel.Buffs.FirstOrDefault(
                                    b =>
                                        b.Name.Equals("gangplankebarrelactive",
                                            StringComparison.InvariantCultureIgnoreCase));

                            if (barrelBuff != null && barrel.Health <= 2f)
                            {
                                var healthDecayRate = enemyGangPlank.Level >= 13
                                    ? 0.5f
                                    : (enemyGangPlank.Level >= 7 ? 1f : 2f);
                                var nextHealthDecayTime = Game.Time < barrelBuff.StartTime + healthDecayRate
                                    ? barrelBuff.StartTime + healthDecayRate
                                    : barrelBuff.StartTime + healthDecayRate * 2;

                                if (nextHealthDecayTime <= Game.Time + t / 1000f)
                                {
                                    return barrel;
                                }
                            }
                        }

                        if (objAiMinions.Any())
                        {
                            return null;
                        }
                    }
                }

                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit ||
                    mode == OrbwalkingMode.Freeze)
                {
                    var MinionList = Get<Obj_AI_Minion>()
                        .Where(minion => minion.IsValidTarget() && InAutoAttackRange(minion))
                        .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                        .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                        .ThenBy(minion => minion.Health)
                        .ThenByDescending(minion => minion.MaxHealth);

                    foreach (var minion in MinionList)
                    {
                        var t = (int)(player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 +
                                1000 * (int)Math.Max(0, player.Distance(minion) - player.BoundingRadius) /
                                (int)GetMyProjectileSpeed();

                        if (mode == OrbwalkingMode.Freeze)
                        {
                            t += 200 + Game.Ping / 2;
                        }

                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay);

                        if (minion.Team != GameObjectTeam.Neutral && ShouldAttackMinion(minion))
                        {
                            var damage = player.GetAutoAttackDamage(minion, true);
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

                if (_forcedTarget.IsValidTarget() && InAutoAttackRange(_forcedTarget))
                {
                    return _forcedTarget;
                }

                if (mode == OrbwalkingMode.LaneClear &&
                    (!Menu.Item("FocusMinionsOverTurrets").GetValue<KeyBind>().Active ||
                     !MinionManager.GetMinions(ObjectManager.Player.Position,
                         GetRealAutoAttackRange(ObjectManager.Player)).Any()))
                {
                    foreach (var turret in Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    foreach (var turret in Get<Obj_BarracksDampener>().Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return turret;
                    }

                    foreach (var nexus in Get<Obj_HQ>().Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    {
                        return nexus;
                    }
                }

                if (mode != OrbwalkingMode.LastHit)
                {
                    if (mode != OrbwalkingMode.LaneClear || !ShouldWait())
                    {
                        var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);

                        if (target.IsValidTarget() && InAutoAttackRange(target))
                        {
                            return target;
                        }
                    }
                }

                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed)
                {
                    var jminions =
                        Get<Obj_AI_Minion>()
                            .Where(
                                mob =>
                                    mob.IsValidTarget() && mob.Team == GameObjectTeam.Neutral &&
                                    InAutoAttackRange(mob) && mob.CharData.BaseSkinName != "gangplankbarrel" &&
                                    mob.Name != "WardCorpse");

                    result = Menu.Item("Smallminionsprio").GetValue<bool>()
                        ? jminions.MinOrDefault(mob => mob.MaxHealth)
                        : jminions.MaxOrDefault(mob => mob.MaxHealth);

                    if (result != null)
                    {
                        return result;
                    }
                }

                if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Mixed || mode == OrbwalkingMode.LastHit ||
                    mode == OrbwalkingMode.Freeze)
                {
                    var closestTower =
                        Get<Obj_AI_Turret>()
                            .MinOrDefault(t => t.IsAlly && !t.IsDead ? player.Distance(t, true) : float.MaxValue);

                    if (closestTower != null && player.Distance(closestTower, true) < 1500 * 1500)
                    {
                        Obj_AI_Minion farmUnderTurretMinion = null;
                        Obj_AI_Minion noneKillableMinion = null;

                        var minions = MinionManager.GetMinions(player.Position, player.AttackRange + 200)
                            .Where(minion => InAutoAttackRange(minion) && closestTower.Distance(minion, true) < 900 * 900)
                            .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                            .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                            .ThenByDescending(minion => minion.MaxHealth)
                            .ThenByDescending(minion => minion.Health);

                        if (minions.Any())
                        {
                            var turretMinion =
                                minions.FirstOrDefault(
                                    minion =>
                                        minion is Obj_AI_Minion &&
                                        HealthPrediction.HasTurretAggro((Obj_AI_Minion)minion));

                            if (turretMinion != null)
                            {
                                var hpLeftBeforeDie = 0;
                                var hpLeft = 0;
                                var turretAttackCount = 0;
                                var turretStarTick = HealthPrediction.TurretAggroStartTick(turretMinion as Obj_AI_Minion);
                                var turretLandTick = turretStarTick + (int)(closestTower.AttackCastDelay * 1000) +
                                                     1000 *
                                                     Math.Max(0,
                                                         (int)
                                                         (turretMinion.Distance(closestTower) -
                                                          closestTower.BoundingRadius)) /
                                                     (int)(closestTower.BasicAttack.MissileSpeed + 70);

                                for (float i = turretLandTick + 50;
                                    i < turretLandTick + 10 * closestTower.AttackDelay * 1000 + 50;
                                    i = i + closestTower.AttackDelay * 1000)
                                {
                                    var time = (int)i - Utils.GameTimeTickCount + Game.Ping / 2;
                                    var predHP =
                                        (int)
                                        HealthPrediction.LaneClearHealthPrediction(turretMinion, time > 0 ? time : 0);

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
                                    var damage = (int)player.GetAutoAttackDamage(turretMinion, true);
                                    var hits = hpLeftBeforeDie / damage;
                                    var timeBeforeDie = turretLandTick +
                                                        (turretAttackCount + 1) * (int)(closestTower.AttackDelay * 1000) -
                                                        Utils.GameTimeTickCount;
                                    var timeUntilAttackReady = LastAATick + (int)(player.AttackDelay * 1000) >
                                                               Utils.GameTimeTickCount + Game.Ping / 2 + 25
                                        ? LastAATick + (int)(player.AttackDelay * 1000) -
                                          (Utils.GameTimeTickCount + Game.Ping / 2 + 25)
                                        : 0;
                                    var timeToLandAttack = player.IsMelee
                                        ? player.AttackCastDelay * 1000
                                        : player.AttackCastDelay * 1000 +
                                          1000 * Math.Max(0, turretMinion.Distance(player) - player.BoundingRadius) /
                                          player.BasicAttack.MissileSpeed;

                                    if (hits >= 1 &&
                                        hits * player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack <
                                        timeBeforeDie)
                                    {
                                        farmUnderTurretMinion = turretMinion as Obj_AI_Minion;
                                    }
                                    else if (hits >= 1 &&
                                             hits * player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack >
                                             timeBeforeDie)
                                    {
                                        noneKillableMinion = turretMinion as Obj_AI_Minion;
                                    }
                                }
                                else if (hpLeft == 0 && turretAttackCount == 0 && hpLeftBeforeDie == 0)
                                {
                                    noneKillableMinion = turretMinion as Obj_AI_Minion;
                                }

                                if (ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }

                                if (farmUnderTurretMinion != null)
                                {
                                    return farmUnderTurretMinion;
                                }

                                foreach (
                                    var minion in
                                    minions.Where(
                                        x =>
                                            x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion &&
                                            !HealthPrediction.HasMinionAggro((Obj_AI_Minion)x)))
                                {
                                    var playerDamage = (int)player.GetAutoAttackDamage(minion);
                                    var turretDamage = (int)closestTower.GetAutoAttackDamage(minion, true);
                                    var leftHP = (int)minion.Health % turretDamage;

                                    if (leftHP > playerDamage)
                                    {
                                        return minion;
                                    }
                                }

                                var lastminion =
                                    minions.LastOrDefault(
                                        x =>
                                            x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion &&
                                            !HealthPrediction.HasMinionAggro((Obj_AI_Minion)x));

                                if (lastminion != null && minions.Count() >= 2)
                                {
                                    if (1f / player.AttackDelay >= 1f &&
                                        (int)(turretAttackCount * closestTower.AttackDelay / player.AttackDelay) *
                                        player.GetAutoAttackDamage(lastminion) > lastminion.Health)
                                    {
                                        return lastminion;
                                    }

                                    if (minions.Count() >= 5 && 1f / player.AttackDelay >= 1.2)
                                    {
                                        return lastminion;
                                    }
                                }
                            }
                            else
                            {
                                if (ShouldWaitUnderTurret(noneKillableMinion))
                                {
                                    return null;
                                }

                                foreach (
                                    var minion in
                                    minions.Where(
                                        x =>
                                            x is Obj_AI_Minion &&
                                            !HealthPrediction.HasMinionAggro((Obj_AI_Minion)x)))
                                {
                                    {
                                        var playerDamage = (int)player.GetAutoAttackDamage(minion);
                                        var turretDamage = (int)closestTower.GetAutoAttackDamage(minion, true);
                                        var leftHP = (int)minion.Health % turretDamage;
                                        if (leftHP > playerDamage)
                                        {
                                            return minion;
                                        }
                                    }
                                }

                                var lastminion =
                                    minions.LastOrDefault(
                                        x =>
                                            x is Obj_AI_Minion &&
                                            !HealthPrediction.HasMinionAggro((Obj_AI_Minion)x));

                                if (lastminion == null || minions.Count() < 2)
                                {
                                    return null;
                                }

                                if (minions.Count() >= 5 && 1f / player.AttackDelay >= 1.2)
                                {
                                    return lastminion;
                                }
                            }

                            return null;
                        }
                    }
                }

                if (mode == OrbwalkingMode.LaneClear)
                {
                    if (!ShouldWait())
                    {
                        if (_prevMinion.IsValidTarget() && InAutoAttackRange(_prevMinion))
                        {
                            var predHealth = HealthPrediction.LaneClearHealthPrediction(_prevMinion,
                                (int)(player.AttackDelay * 1000 * LaneClearWaitTimeMod), FarmDelay);

                            if (predHealth >= 2 * player.GetAutoAttackDamage(_prevMinion) ||
                                Math.Abs(predHealth - _prevMinion.Health) < float.Epsilon)
                            {
                                return _prevMinion;
                            }
                        }

                        result = Get<Obj_AI_Minion>()
                            .Where(
                                minion =>
                                    minion.IsValidTarget() && InAutoAttackRange(minion) &&
                                    ShouldAttackMinion(minion))
                            .Select(
                                minion =>
                                    new
                                    {
                                        minion,
                                        predHealth =
                                        HealthPrediction.LaneClearHealthPrediction(minion,
                                            (int)(player.AttackDelay * 1000 * LaneClearWaitTimeMod), FarmDelay)
                                    })
                            .Where(
                                t =>
                                    t.predHealth >= 2 * player.GetAutoAttackDamage(t.minion) ||
                                    Math.Abs(t.predHealth - t.minion.Health) < float.Epsilon)
                            .Select(t => t.minion)
                            .MaxOrDefault(m => !MinionManager.IsMinion(m, true) ? float.MaxValue : m.Health);

                        if (result != null)
                        {
                            _prevMinion = (Obj_AI_Minion)result;
                        }
                    }
                }

                return result;
            }

            protected bool InAutoAttackRange(AttackableUnit target) => Orbwalking.InAutoAttackRange(target);

            public virtual void RegisterCustomMode(string name, string displayname, uint key)
            {
                CustomModeName = name;

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
                _orbwalkingPoint = point;
            }

            public bool ShouldWait()
            {
                return
                    Get<Obj_AI_Minion>()
                        .Any(
                            minion =>
                                minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                                InAutoAttackRange(minion) && MinionManager.IsMinion(minion) &&
                                HealthPrediction.LaneClearHealthPrediction(minion,
                                    (int)(player.AttackDelay * 1000 * LaneClearWaitTimeMod), FarmDelay) <=
                                player.GetAutoAttackDamage(minion));
            }

            private void DrawingOnOnDraw(EventArgs args)
            {
                if (ObjectManager.Player.IsDead || Shop.IsOpen || MenuGUI.IsChatOpen )
                {
                    return;
                }

                if (Menu.Item("AACircle").GetValue<Circle>().Active)
                {
                    Render.Circle.DrawCircle(player.Position, GetRealAutoAttackRange(null) + 65,
                        Menu.Item("AACircle").GetValue<Circle>().Color,
                        Menu.Item("AALineWidth").GetValue<Slider>().Value);
                }

                if (Menu.Item("AACircle2").GetValue<Circle>().Active)
                {
                    foreach (var target in HeroManager.Enemies.FindAll(target => target.IsValidTarget(1175)))
                    {
                        Render.Circle.DrawCircle(target.Position, GetAttackRange(target),
                            Menu.Item("AACircle2").GetValue<Circle>().Color,
                            Menu.Item("AALineWidth").GetValue<Slider>().Value);
                    }
                }
            }

            private void GameOnOnGameUpdate(EventArgs args)
            {
                if (ActiveMode == OrbwalkingMode.None)
                {
                    return;
                }

                Move = Menu.Item("EnableOrbwalker").GetValue<bool>();

                var target = GetTarget();

                Orbwalk(target, _orbwalkingPoint.To2D().IsValid() ? _orbwalkingPoint : Game.CursorPos,
                    Menu.Item("ExtraWindup").GetValue<Slider>().Value,
                    Math.Max(Menu.Item("HoldPosRadius").GetValue<Slider>().Value, 30));
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

                return (Menu.Item("AttackPetsnTraps").GetValue<bool>() || MinionManager.IsMinion(minion)) &&
                       minion.CharData.BaseSkinName != "gangplankbarrel";
            }

            private bool ShouldWaitUnderTurret(Obj_AI_Minion noneKillableMinion)
            {
                return Get<Obj_AI_Minion>().Any(minion =>
                {
                    if (minion == null)
                    {
                        throw new ArgumentNullException(nameof(minion));
                    }

                    return (noneKillableMinion == null || noneKillableMinion.NetworkId != minion.NetworkId) &&
                           minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                           InAutoAttackRange(minion) && MinionManager.IsMinion(minion) &&
                           HealthPrediction.LaneClearHealthPrediction(minion,
                               (int)
                               (player.AttackDelay * 1000 +
                                (player.IsMelee
                                    ? player.AttackCastDelay * 1000
                                    : player.AttackCastDelay * 1000 +
                                      1000 * (player.AttackRange + 2 * player.BoundingRadius) /
                                      player.BasicAttack.MissileSpeed)), FarmDelay) <=
                           player.GetAutoAttackDamage(minion);
                });
            }
        }
    }
}