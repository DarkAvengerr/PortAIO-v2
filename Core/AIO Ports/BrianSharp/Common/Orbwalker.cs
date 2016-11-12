namespace BrianSharp.Common
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy;
    using System.Collections.Generic;

    internal class Orbwalker
    {
        #region Static Fields

        public static AIHeroClient ForcedTarget = null;

        private static Menu config;

        private static bool disableNextAttack, missileLaunched;

        private static int lastAttack, lastMove;

        private static AttackableUnit lastTarget;

        private static Obj_AI_Minion prevMinion;

        private static readonly Spell MovePrediction = new Spell(SpellSlot.Unknown, GetAutoAttackRange());

        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        #endregion

        #region Delegates

        /// <summary>
        ///     Delegate AfterAttackEvenH
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="target">The target.</param>
        public delegate void AfterAttackEvenH(AttackableUnit unit, AttackableUnit target);

        /// <summary>
        ///     Delegate BeforeAttackEvenH
        /// </summary>
        /// <param name="args">The <see cref="BeforeAttackEventArgs" /> instance containing the event data.</param>
        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);

        /// <summary>
        ///     Delegate OnAttackEvenH
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="target">The target.</param>
        public delegate void OnAttackEvenH(AttackableUnit unit, AttackableUnit target);

        /// <summary>
        ///     Delegate OnNonKillableMinionH
        /// </summary>
        /// <param name="minion">The minion.</param>
        public delegate void OnNonKillableMinionH(AttackableUnit minion);

        /// <summary>
        ///     Delegate OnTargetChangeH
        /// </summary>
        /// <param name="oldTarget">The old target.</param>
        /// <param name="newTarget">The new target.</param>
        public delegate void OnTargetChangeH(AttackableUnit oldTarget, AttackableUnit newTarget);

        #endregion

        #region Public Events

        public static event AfterAttackEvenH AfterAttack;

        public static event BeforeAttackEvenH BeforeAttack;

        public static event OnAttackEvenH OnAttack;

        public static event OnNonKillableMinionH OnNonKillableMinion;

        public static event OnTargetChangeH OnTargetChange;

        #endregion

        #region Enums

        public enum Mode
        {
            Combo,

            Harass,

            Clear,

            LastHit,

            Flee,

            None
        }

        #endregion

        #region Public Properties

        public static bool Attack { get; set; }

        public static bool CanAttack
        {
            get
            {
                if (Player.ChampionName == "Graves")
                {
                    var attackDelay = 1.0740296828d * 1000 * Player.AttackDelay - 716.2381256175d;
                    if (Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= lastAttack + attackDelay
                        && Player.HasBuff("GravesBasicAttackAmmo1"))
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

                return Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= lastAttack + Player.AttackDelay * 1000;
            }
        }

        public static Mode CurrentMode
        {
            get
            {
                return config.Item("OW_Combo_Key").IsActive()
                           ? Mode.Combo
                           : (config.Item("OW_Harass_Key").IsActive()
                                  ? Mode.Harass
                                  : (config.Item("OW_Clear_Key").IsActive()
                                         ? Mode.Clear
                                         : (config.Item("OW_LastHit_Key").IsActive()
                                                ? Mode.LastHit
                                                : (config.Item("OW_Flee_Key").IsActive() ? Mode.Flee : Mode.None))));
            }
        }

        public static AIHeroClient GetBestHeroTarget
        {
            get
            {
                AIHeroClient killableObj = null;
                var hitsToKill = double.MaxValue;
                foreach (var obj in HeroManager.Enemies.Where(i => InAutoAttackRange(i)))
                {
                    var killHits = obj.Health / Player.GetAutoAttackDamage(obj, true);
                    if (killableObj != null && (killHits >= hitsToKill || obj.HasBuffOfType(BuffType.Invulnerability)))
                    {
                        continue;
                    }
                    killableObj = obj;
                    hitsToKill = killHits;
                }
                return hitsToKill < 4 ? killableObj : TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
            }
        }

        public static float GetMyProjectileSpeed()
        {
            return IsMelee(Player) || ObjectManager.Player.ChampionName == "Azir" || ObjectManager.Player.ChampionName == "Velkoz"
                   || ObjectManager.Player.ChampionName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn")
                       ? float.MaxValue
                       : Player.BasicAttack.MissileSpeed;
        }

        public static bool IsMelee(Obj_AI_Base unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }

        public static AttackableUnit GetPossibleTarget
        {
            get
            {
                if (!config.Item("OW_Misc_PriorityFarm").IsActive()
                    && (CurrentMode == Mode.Harass || CurrentMode == Mode.Clear))
                {
                    var hero = GetBestHeroTarget;
                    if (hero.IsValidTarget() && hero != null && hero.IsHPBarRendered && hero.IsHPBarRendered && hero.IsTargetable)
                    {
                        Console.WriteLine("1");
                        return hero;
                    }
                }
                /*Killable Minion*/
                if (CurrentMode == Mode.Clear || CurrentMode == Mode.Harass || CurrentMode == Mode.LastHit)
                {
                    var MinionList =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(minionA => minionA.IsValidTarget() && InAutoAttackRange(minionA))
                            .OrderByDescending(minionA => minionA.CharData.BaseSkinName.Contains("Siege"))
                            .ThenBy(minionA => minionA.CharData.BaseSkinName.Contains("Super"))
                            .ThenBy(minionA => minionA.Health)
                            .ThenByDescending(minionA => minionA.MaxHealth);

                    foreach (var minionA in MinionList)
                    {
                        var t = (int)(ObjectManager.Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2
                                + 1000 * (int)Math.Max(0, ObjectManager.Player.Distance(minionA) - ObjectManager.Player.BoundingRadius)
                                / (int)GetMyProjectileSpeed();

                        var predHealth = HealthPrediction.GetHealthPrediction(minionA, t, 0);

                        if (minionA.Team != GameObjectTeam.Neutral && ShouldAttackMinion(minionA))
                        {
                            var damage = ObjectManager.Player.GetAutoAttackDamage(minionA, true);
                            var killable = predHealth <= damage;
                            if (predHealth <= 0)
                            {
                                FireOnNonKillableMinion(minionA);
                            }

                            if (killable)
                            {
                                return minionA;
                            }
                        }
                    }
                }
                if (InAutoAttackRange(ForcedTarget) && ForcedTarget != null && ForcedTarget.IsHPBarRendered && ForcedTarget.IsHPBarRendered)
                {
                    return ForcedTarget;
                }
                if (CurrentMode == Mode.Clear)
                {
                    foreach (var obj in ObjectManager.Get<Obj_AI_Turret>().Where(i => InAutoAttackRange(i)))
                    {
                        return obj;
                    }
                    foreach (var obj in ObjectManager.Get<Obj_BarracksDampener>().Where(i => InAutoAttackRange(i)))
                    {
                        return obj;
                    }
                    foreach (var obj in ObjectManager.Get<Obj_HQ>().Where(i => InAutoAttackRange(i)))
                    {
                        return obj;
                    }
                }
                if (CurrentMode != Mode.LastHit)
                {
                    var hero = GetBestHeroTarget;
                    if (hero.IsValidTarget() && hero != null && hero.IsHPBarRendered && hero.IsHPBarRendered && hero.IsTargetable)
                    {
                        return hero;
                    }
                }
                if (CurrentMode == Mode.Clear || CurrentMode == Mode.Harass)
                {
                    var mob =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                i =>
                                InAutoAttackRange(i) && i.Team == GameObjectTeam.Neutral
                                && i.CharData.BaseSkinName != "gangplankbarrel")
                            .MaxOrDefault(i => i.MaxHealth);
                    if (mob != null)
                    {
                        return mob;
                    }
                }
                if (CurrentMode != Mode.Clear || ShouldWait)
                {
                    return null;
                }

                if (prevMinion.IsValidTarget() && InAutoAttackRange(prevMinion))
                {
                    var predHealth = HealthPrediction.LaneClearHealthPrediction(
                        prevMinion,
                        (int)(ObjectManager.Player.AttackDelay * 1000 * 2f),
                        0);
                    if (predHealth >= 2 * ObjectManager.Player.GetAutoAttackDamage(prevMinion)
                        || Math.Abs(predHealth - prevMinion.Health) < float.Epsilon)
                    {
                        return prevMinion;
                    }
                }

                var minion = (from obj in
                                      ObjectManager.Get<Obj_AI_Minion>()
                                      .Where(
                                          x =>
                                          x.IsValidTarget() && InAutoAttackRange(x)
                                          && ShouldAttackMinion(x))
                              let predHealth =
                                  HealthPrediction.LaneClearHealthPrediction(
                                      obj,
                                      (int)(ObjectManager.Player.AttackDelay * 1000 * 2f), 0)
                              where
                                  predHealth >= 2 * ObjectManager.Player.GetAutoAttackDamage(obj)
                                  || Math.Abs(predHealth - obj.Health) < float.Epsilon
                              select obj).MaxOrDefault(
                                      m => !MinionManager.IsMinion(m, true) ? float.MaxValue : m.Health);
                if (minion != null)
                {
                    prevMinion = minion;
                }
                return minion;
            }
        }

        /// <summary>
        ///     Returns the auto-attack range of local player with respect to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.Single.</returns>
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

        /// <summary>
        ///     Returns true if the target is in auto-attack range.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.IsValidTarget())
            {
                return false;
            }
            var myRange = GetRealAutoAttackRange(target);
            return
                Vector2.DistanceSquared(
                    target is Obj_AI_Base ? ((Obj_AI_Base)target).ServerPosition.To2D() : target.Position.To2D(),
                    Player.ServerPosition.To2D()) <= myRange * myRange;
        }

        /// <summary>
        ///     Returns if a minion should be attacked
        /// </summary>
        /// <param name="minion">The <see cref="Obj_AI_Minion" /></param>
        /// <param name="includeBarrel">Include Gangplank Barrel</param>
        /// <returns><c>true</c> if the minion should be attacked; otherwise, <c>false</c>.</returns>
        private static bool ShouldAttackMinion(Obj_AI_Minion minion)
        {
            if (minion.Name == "WardCorpse" || minion.CharData.BaseSkinName == "jarvanivstandard")
            {
                return false;
            }

            if (MinionManager.IsWard(minion))
            {
                return true;
            }

            return (true || MinionManager.IsMinion(minion))
                   && minion.CharData.BaseSkinName != "gangplankbarrel";
        }

        public static bool Move { get; set; }

        #endregion

        #region Properties

        private static readonly string[] NoCancelChamps = { "Kalista" };

        private static bool CanMove
        {
            get
            {
                if (missileLaunched)
                {
                    return true;
                }

                var localExtraWindup = 0;
                if (ObjectManager.Player.ChampionName == "Rengar" && (Player.HasBuff("rengarqbase") || Player.HasBuff("rengarqemp")))
                {
                    localExtraWindup = 200;
                }

                return NoCancelChamps.Contains(ObjectManager.Player.ChampionName)
                       || (Utils.GameTimeTickCount + Game.Ping / 2
                           >= lastAttack + Player.AttackCastDelay * 1000 + GetCurrentWindupTime + localExtraWindup);
            }
        }

        private static int GetCurrentWindupTime
        {
            get
            {
                return config.Item("OW_Misc_ExtraWindUp").GetValue<Slider>().Value;
            }
        }

        private static bool IsAllowedToAttack
        {
            get
            {
                if (!Attack || config.Item("OW_Misc_AllAttackDisabled").IsActive())
                {
                    return false;
                }
                if ((CurrentMode == Mode.Combo || CurrentMode == Mode.Harass || CurrentMode == Mode.Clear)
                    && !config.Item("OW_" + CurrentMode + "_Attack").IsActive())
                {
                    return false;
                }
                return CurrentMode != Mode.LastHit || config.Item("OW_LastHit_Attack").IsActive();
            }
        }

        private static bool IsAllowedToMove
        {
            get
            {
                if (!Move || config.Item("OW_Misc_AllMovementDisabled").IsActive())
                {
                    return false;
                }
                if ((CurrentMode == Mode.Combo || CurrentMode == Mode.Harass || CurrentMode == Mode.Clear)
                    && !config.Item("OW_" + CurrentMode + "_Move").IsActive())
                {
                    return false;
                }
                return CurrentMode != Mode.LastHit || config.Item("OW_LastHit_Move").IsActive();
            }
        }

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        private static bool ShouldWait
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Any(
                            i =>
                            InAutoAttackRange(i) && i.Team != GameObjectTeam.Neutral
                            && HealthPrediction.GetHealthPrediction(i, (int)(Player.AttackDelay * 1000 * 2), 0)
                            <= Player.GetAutoAttackDamage(i, true));
            }
        }

        #endregion

        #region Public Methods and Operators

        public static float GetAutoAttackRange(AttackableUnit target = null)
        {
            return GetAutoAttackRange(Player, target);
        }

        public static bool InAutoAttackRange(AttackableUnit target, float extraRange = 0, Vector3 from = new Vector3())
        {
            return target.IsValidTarget(GetAutoAttackRange(target) + extraRange, true, from);
        }

        public static void Init(Menu mainMenu)
        {
            config = mainMenu;
            var owMenu = new Menu("Orbwalker", "OW");
            {
                var modeMenu = new Menu("Mode", "Mode");
                {
                    var comboMenu = new Menu("Combo", "OW_Combo");
                    {
                        comboMenu.AddItem(
                            new MenuItem("OW_Combo_Key", "Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                        comboMenu.AddItem(new MenuItem("OW_Combo_MeleeMagnet", "Melee Movement Magnet").SetValue(true));
                        comboMenu.AddItem(new MenuItem("OW_Combo_Move", "Movement").SetValue(true));
                        comboMenu.AddItem(new MenuItem("OW_Combo_Attack", "Attack").SetValue(true));
                        modeMenu.AddSubMenu(comboMenu);
                    }
                    var harassMenu = new Menu("Harass", "OW_Harass");
                    {
                        harassMenu.AddItem(
                            new MenuItem("OW_Harass_Key", "Key").SetValue(
                                new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                        harassMenu.AddItem(new MenuItem("OW_Harass_Move", "Movement").SetValue(true));
                        harassMenu.AddItem(new MenuItem("OW_Harass_Attack", "Attack").SetValue(true));
                        harassMenu.AddItem(new MenuItem("OW_Harass_LastHit", "Last Hit Minion").SetValue(true));
                        modeMenu.AddSubMenu(harassMenu);
                    }
                    var clearMenu = new Menu("Clear", "OW_Clear");
                    {
                        clearMenu.AddItem(
                            new MenuItem("OW_Clear_Key", "Key").SetValue(
                                new KeyBind("G".ToCharArray()[0], KeyBindType.Press)));
                        clearMenu.AddItem(new MenuItem("OW_Clear_Move", "Movement").SetValue(true));
                        clearMenu.AddItem(new MenuItem("OW_Clear_Attack", "Attack").SetValue(true));
                        modeMenu.AddSubMenu(clearMenu);
                    }
                    var lastHitMenu = new Menu("Last Hit", "OW_LastHit");
                    {
                        lastHitMenu.AddItem(
                            new MenuItem("OW_LastHit_Key", "Key").SetValue(new KeyBind(17, KeyBindType.Press)));
                        lastHitMenu.AddItem(new MenuItem("OW_LastHit_Move", "Movement").SetValue(true));
                        lastHitMenu.AddItem(new MenuItem("OW_LastHit_Attack", "Attack").SetValue(true));
                        modeMenu.AddSubMenu(lastHitMenu);
                    }
                    var fleeMenu = new Menu("Flee", "OW_Flee");
                    {
                        fleeMenu.AddItem(
                            new MenuItem("OW_Flee_Key", "Key").SetValue(
                                new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                        modeMenu.AddSubMenu(fleeMenu);
                    }
                    owMenu.AddSubMenu(modeMenu);
                }
                var miscMenu = new Menu("Misc", "Misc");
                {
                    miscMenu.AddItem(new MenuItem("OW_Misc_HoldZone", "Hold Zone").SetValue(new Slider(0, 0, 250)));
                    miscMenu.AddItem(
                        new MenuItem("OW_Misc_MoveDelay", "Movement Delay").SetValue(new Slider(30, 0, 250)));
                    miscMenu.AddItem(
                        new MenuItem("OW_Misc_ExtraWindUp", "Extra WindUp Time").SetValue(new Slider(80, 0, 200)));
                    miscMenu.AddItem(
                        new MenuItem("OW_Misc_PriorityFarm", "Priorize LastHit Over Harass").SetValue(true));
                    miscMenu.AddItem(
                        new MenuItem("OW_Misc_AllMovementDisabled", "Disable All Movement").SetValue(false));
                    miscMenu.AddItem(new MenuItem("OW_Misc_AllAttackDisabled", "Disable All Attack").SetValue(false));
                    owMenu.AddSubMenu(miscMenu);
                }
                var drawMenu = new Menu("Draw", "Draw");
                {
                    drawMenu.AddItem(
                        new MenuItem("OW_Draw_AARange", "Player AA Range").SetValue(
                            new Circle(false, Color.FloralWhite)));
                    drawMenu.AddItem(
                        new MenuItem("OW_Draw_AARangeEnemy", "Enemy AA Range").SetValue(new Circle(false, Color.Pink)));
                    drawMenu.AddItem(
                        new MenuItem("OW_Draw_HoldZone", "Hold Zone").SetValue(new Circle(false, Color.FloralWhite)));
                    owMenu.AddSubMenu(drawMenu);
                }
                config.AddSubMenu(owMenu);
            }
            MovePrediction.SetTargetted(Player.BasicAttack.SpellCastTime, Player.BasicAttack.MissileSpeed);
            Attack = true;
            Move = true;

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnBasicAttack += OnBasicAttack;
            Obj_AI_Base.OnSpellCast += new Obj_AI_BaseDoCastSpell(Obj_AI_Base_OnDoCast);

            Spellbook.OnStopCast += OnStopCast;
        }

        /// <summary>
        ///     Fired when an auto attack is fired.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                var ping = Game.Ping;
                if (ping <= 30) //First world problems kappa
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(30 - ping, () => Obj_AI_Base_OnDoCast_Delayed(sender, args));
                    return;
                }

                Obj_AI_Base_OnDoCast_Delayed(sender, args); //InvokeActionAfterAttackDelay()
            }
        }

        /// <summary>
        ///     Fired 30ms after an auto attack is launched.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void Obj_AI_Base_OnDoCast_Delayed(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (IsAutoAttackReset(args.SData.Name))
            {
                ResetAutoAttack();
            }

            if (IsAutoAttack(args.SData.Name))
            {
                FireAfterAttack(sender, args.Target as AttackableUnit);
                missileLaunched = true;
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
                        FireOnAttack(sender, lastTarget);
                    }
                }
            }

            if (sender.IsMe && (args.Target is Obj_AI_Base || args.Target is Obj_BarracksDampener || args.Target is Obj_HQ))
            {
                lastAttack = Utils.GameTimeTickCount - Game.Ping / 2;
                missileLaunched = false;
                lastMove = 0;

                if (args.Target is Obj_AI_Base)
                {
                    var target = (Obj_AI_Base)args.Target;
                    if (target.IsValid)
                    {
                        FireOnTargetSwitch(target);
                        lastTarget = target;
                    }
                }
            }

            if (sender is Obj_AI_Turret && args.Target is Obj_AI_Base)
            {
                LastTargetTurrets[sender.NetworkId] = (Obj_AI_Base)args.Target;
            }
        }

        internal static readonly Dictionary<int, Obj_AI_Base> LastTargetTurrets = new Dictionary<int, Obj_AI_Base>();

        public static void MoveTo(Vector3 pos)
        {
            if (Utils.GameTimeTickCount - lastMove < config.Item("OW_Misc_MoveDelay").GetValue<Slider>().Value)
            {
                return;
            }
            lastMove = Utils.GameTimeTickCount;
            if (Player.Distance(pos, true)
                < Math.Pow(Player.BoundingRadius + config.Item("OW_Misc_HoldZone").GetValue<Slider>().Value, 2))
            {
                return;
            }
            EloBuddy.Player.IssueOrder(
                GameObjectOrder.MoveTo,
                Player.ServerPosition.Extend(pos, (Random.NextFloat(0.6f, 1) + 0.2f) * 400));
        }

        public static void Orbwalk(AttackableUnit target)
        {
            if (target.IsValidTarget() && CanAttack && IsAllowedToAttack)
            {
                disableNextAttack = false;
                FireBeforeAttack(target);
                if (!disableNextAttack
                    && (CurrentMode != Mode.Harass || !target.IsValid<Obj_AI_Minion>()
                        || config.Item("OW_Harass_LastHit").IsActive()))
                {
                    lastAttack = Utils.GameTimeTickCount + Game.Ping + 100 - (int)(Player.AttackCastDelay * 1000);
                    missileLaunched = false;
                    if (Player.Distance(target, true) > Math.Pow(GetAutoAttackRange(target) - 65, 2) && !Player.IsMelee)
                    {
                        lastAttack = Utils.GameTimeTickCount + Game.Ping + 400 - (int)(Player.AttackCastDelay * 1000);
                    }
                    if (!EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                    {
                        //ResetAutoAttack();
                    }
                    lastTarget = target;
                    return;
                }
            }
            if (!CanMove || !IsAllowedToMove)
            {
                return;
            }
            if (config.Item("OW_Combo_MeleeMagnet").IsActive() && CurrentMode == Mode.Combo && Player.IsMelee
                && Player.AttackRange < 200 && InAutoAttackRange(target) && target.IsValid<AIHeroClient>()
                && ((AIHeroClient)target).Distance(Game.CursorPos) < 300)
            {
                MovePrediction.Delay = Player.BasicAttack.SpellCastTime;
                MovePrediction.Speed = Player.BasicAttack.MissileSpeed;
                MoveTo(MovePrediction.GetPrediction((AIHeroClient)target).UnitPosition);
            }
            else
            {
                MoveTo(Game.CursorPos);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        ///     Fires the after attack event.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="target">The target.</param>
        private static void FireAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (AfterAttack != null && target.IsValidTarget())
            {
                AfterAttack(unit, target);
            }
        }

        /// <summary>
        ///     Fires the before attack event.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void FireBeforeAttack(AttackableUnit target)
        {
            if (BeforeAttack != null)
            {
                BeforeAttack(new BeforeAttackEventArgs { Target = target });
            }
            else
            {
                disableNextAttack = false;
            }
        }

        /// <summary>
        ///     Fires the on attack event.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="target">The target.</param>
        private static void FireOnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (OnAttack != null)
            {
                OnAttack(unit, target);
            }
        }

        /// <summary>
        ///     Fires the on non killable minion event.
        /// </summary>
        /// <param name="minion">The minion.</param>
        private static void FireOnNonKillableMinion(AttackableUnit minion)
        {
            if (OnNonKillableMinion != null)
            {
                OnNonKillableMinion(minion);
            }
        }

        /// <summary>
        ///     Fires the on target switch event.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null && (!lastTarget.IsValidTarget() || lastTarget != newTarget))
            {
                OnTargetChange(lastTarget, newTarget);
            }
        }

        private static float GetAutoAttackRange(Obj_AI_Base source, AttackableUnit target)
        {
            return source.AttackRange + source.BoundingRadius + (target.IsValidTarget() ? target.BoundingRadius : 0);
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (config.Item("OW_Draw_AARange").IsActive())
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    GetAutoAttackRange(),
                    config.Item("OW_Draw_AARange").GetValue<Circle>().Color);
            }
            if (config.Item("OW_Draw_AARangeEnemy").IsActive())
            {
                foreach (var obj in HeroManager.Enemies.Where(i => i.IsValidTarget(1000)))
                {
                    Render.Circle.DrawCircle(
                        obj.Position,
                        GetAutoAttackRange(obj, Player),
                        config.Item("OW_Draw_AARangeEnemy").GetValue<Circle>().Color);
                }
            }
            if (config.Item("OW_Draw_HoldZone").IsActive())
            {
                Render.Circle.DrawCircle(
                    Player.Position,
                    config.Item("OW_Misc_HoldZone").GetValue<Slider>().Value,
                    config.Item("OW_Draw_HoldZone").GetValue<Circle>().Color);
            }
        }

        /// <summary>
        ///     Spells that reset the attack timer.
        /// </summary>
        private static readonly string[] AttackResets =
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

        /// <summary>
        ///     Spells that are attacks even if they dont have the "attack" word in their name.
        /// </summary>
        private static readonly string[] Attacks =
            {
                "caitlynheadshotmissile", "frostarrow", "garenslash2",
                "kennenmegaproc", "masteryidoublestrike", "quinnwenhanced",
                "renektonexecute", "renektonsuperexecute",
                "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust",
                "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff",
                "lucianpassiveshot"
            };

        /// <summary>
        ///     Spells that are not attacks even if they have the "attack" word in their name.
        /// </summary>
        private static readonly string[] NoAttacks =
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

        /// <summary>
        ///     Returns true if the spellname is an auto-attack.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the name is an auto attack; otherwise, <c>false</c>.</returns>
        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower()))
                   || Attacks.Contains(name.ToLower());
        }

        /// <summary>
        ///     Returns true if the spellname resets the attack timer.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is an auto attack reset; otherwise, <c>false</c>.</returns>
        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (IsAutoAttack(args.SData.Name))
                {
                    var target = args.Target as AttackableUnit;

                    if (target != null && target.IsValid)
                    {
                        FireOnAttack(sender, lastTarget);
                    }
                }
                if (IsAutoAttackReset(args.SData.Name) && Math.Abs(args.SData.CastTime) < 1.401298E-45f)
                {
                    ResetAutoAttack();
                }
            }
        }

        private static void OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsMe && EloBuddy.SDK.Orbwalker.IsRanged && (args.DestroyMissile || args.StopAnimation) && !EloBuddy.SDK.Orbwalker.CanBeAborted)
            {
                ResetAutoAttack();
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || CurrentMode == Mode.None || MenuGUI.IsChatOpen || Player.IsRecalling()
                || Player.IsCastingInterruptableSpell(true))
            {
                return;
            }
            Orbwalk(CurrentMode == Mode.Flee ? null : GetPossibleTarget);
        }

        private static void ResetAutoAttack()
        {
            lastAttack = 0;
        }

        #endregion

        public class BeforeAttackEventArgs
        {
            #region Fields

            public AttackableUnit Target;

            private bool process = true;

            #endregion

            #region Public Properties

            public bool Process
            {
                get
                {
                    return this.process;
                }
                set
                {
                    disableNextAttack = !value;
                    this.process = value;
                }
            }

            #endregion
        }
    }
}