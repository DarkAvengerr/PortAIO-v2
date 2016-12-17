using EloBuddy; 
using LeagueSharp.Common; 
namespace ElLeeSin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElLeeSin.Components;
    using ElLeeSin.Components.SpellManagers;
    using ElLeeSin.Components.Spells;
    using ElLeeSin.Kurisu;
    using ElLeeSin.Utilities;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class LeeSin
    {
        #region Constants

        /// <summary>
        ///     Lee Sin R kick distance
        /// </summary>
        private const float LeeSinRKickDistance = 700;

        /// <summary>
        ///     Lee Sin R kick width
        /// </summary>
        private const float LeeSinRKickWidth = 100;

        #endregion

        #region Static Fields

        public static bool CheckQ = true;

        public static SpellSlot flashSlot;

        public static bool isInQ2;

        public static int LastQ2;

        public static Orbwalking.Orbwalker Orbwalker;

        public static int PassiveStacks, LastW;

        public static Spell smite = null;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                     { Spells.Q, new Spell(SpellSlot.Q, 1100f) },
                                                                     { Spells.W, new Spell(SpellSlot.W, 700f) },
                                                                     { Spells.E, new Spell(SpellSlot.E, 425f) },
                                                                     { Spells.R, new Spell(SpellSlot.R, 375f) },
                                                                     { Spells.R2, new Spell(SpellSlot.R, 800f) }
                                                             };

        /// <summary>
        ///     The last spell casting.
        /// </summary>
        internal static int lastSpellCastTime;

        /// <summary>
        ///     The buffnames.
        /// </summary>
        private static readonly List<string> buffNames =
            new List<string>(
                new[]
                    {
                        "blindmonkqone", "blindmonkwone", "blindmonkeone", "blindmonkqtwo", "blindmonkwtwo",
                        "blindmonketwo", "blindmonkrkick"
                    });

        private static readonly int[] SmiteBlue = { 3706, 1403, 1402, 1401, 1400 };

        private static readonly int[] SmiteRed = { 3715, 1415, 1414, 1413, 1412 };

        private static bool castQAgain;

        private static int clickCount;

        private static float doubleClickReset;

        private static SpellSlot igniteSlot;

        private static bool lastClickBool;

        private static Vector3 lastClickPos;

        private static float lastPlaced;

        private static bool q2Done;

        private static float q2Timer;

        private static float resetTime;

        private static bool waitingForQ2;

        private static bool wardJumped;

        #endregion

        #region Enums

        internal enum Spells
        {
            Q,

            W,

            E,

            R,

            R2
        }

        #endregion

        #region Public Methods and Operators

        public static void CastQ(Obj_AI_Base target, bool smiteQ = false)
        {
            if (!spells[Spells.Q].IsReady())
            {
                return;
            }

            var prediction = spells[Spells.Q].GetPrediction(target);
            if (prediction.Hitchance >= HitChance.High)
            {
                spells[Spells.Q].Cast(target);
            }

            if (smiteQ && Misc.GetMenuItem("ElLeeSin.Smite.Q"))
            {
                if (target.IsValidTarget(spells[Spells.Q].Range)
                    && (prediction.CollisionObjects.Count(a => (a.NetworkId != target.NetworkId) && a.IsMinion) == 1)
                    && ObjectManager.Player.GetSpellSlot(SmiteSpellName()).IsReady())
                {
                    ObjectManager.Player.Spellbook.CastSpell(
                        ObjectManager.Player.GetSpellSlot(SmiteSpellName()),
                        prediction.CollisionObjects.Where(a => (a.NetworkId != target.NetworkId) && a.IsMinion).ToList()
                            [0]);

                    spells[Spells.Q].Cast(prediction.CastPosition);
                }
            }
        }

        public static void CastW(Obj_AI_Base obj)
        {
            if ((500 >= Utils.TickCount - Wardmanager.Wcasttime) || (Misc.WStage != Wardmanager.WCastStage.First))
            {
                return;
            }

            spells[Spells.W].CastOnUnit(obj);
            Wardmanager.Wcasttime = Utils.TickCount;
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }

            if (Misc.GetMenuItem("ElLeeSin.Combo.Q") && spells[Spells.Q].IsReady() && Misc.IsQOne)
            {
                CastQ(target, Misc.GetMenuItem("ElLeeSin.Smite.Q"));
            }

            if (spells[Spells.R].IsReady() && Misc.GetMenuItem("ElLeeSin.Combo.R")
                && Misc.GetMenuItem("ElLeeSin.Combo.Q") && Misc.GetMenuItem("ElLeeSin.Combo.Q2"))
            {
                var qTarget = HeroManager.Enemies.FirstOrDefault(x => x.HasBuff("BlindMonkSonicWave"));
                if (qTarget != null)
                {
                    if ((target.Health + target.AttackShield
                         > spells[Spells.Q].GetDamage(target, 1) + ObjectManager.Player.GetAutoAttackDamage(target))
                        && (target.Health + target.AttackShield
                            <= Misc.Q2Damage(target, spells[Spells.R].GetDamage(target))
                            + ObjectManager.Player.GetAutoAttackDamage(target)))
                    {
                        if (spells[Spells.R].CastOnUnit(target))
                        {
                            return;
                        }

                        if (Misc.GetMenuItem("ElLeeSin.Combo.StarKill1") && !spells[Spells.R].IsInRange(target)
                            && (target.Distance(ObjectManager.Player) < 600 + spells[Spells.R].Range - 50)
                            && (ObjectManager.Player.Mana >= 80) && !ObjectManager.Player.IsDashing())
                        {
                            Wardmanager.WardJump(target.Position, false, true);
                        }
                    }
                }
            }


            if (Misc.GetMenuItem("ElLeeSin.Combo.Q2") && !ObjectManager.Player.IsDashing() && target.HasQBuff()
                && target.IsValidTarget(1300f) && !isInQ2)
            {
                if (castQAgain || (spells[Spells.Q].GetDamage(target, 1) > target.Health + target.AttackShield)
                    || ((Misc.ReturnQBuff()?.Distance(target) < ObjectManager.Player.Distance(target))
                        && !target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player))))
                {
                    spells[Spells.Q].Cast();
                }
            }

            if ((spells[Spells.R].GetDamage(target) > target.Health) && Misc.GetMenuItem("ElLeeSin.Combo.KS.R")
                && target.IsValidTarget(spells[Spells.R].Range))
            {
                spells[Spells.R].CastOnUnit(target);
            }

            if (Misc.GetMenuItem("ElLeeSin.Combo.AAStacks")
                && (PassiveStacks > MyMenu.Menu.Item("ElLeeSin.Combo.PassiveStacks").GetValue<Slider>().Value)
                && (Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) > ObjectManager.Player.Distance(target)))
            {
                return;
            }

            if (Misc.GetMenuItem("ElLeeSin.Combo.W"))
            {
                if (Misc.GetMenuItem("ElLeeSin.Combo.Mode.WW")
                    && (target.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)))
                {
                    Wardmanager.WardJump(target.Position, false, true);
                }

                if (!Misc.GetMenuItem("ElLeeSin.Combo.Mode.WW")
                    && (target.Distance(ObjectManager.Player) > spells[Spells.Q].Range))
                {
                    Wardmanager.WardJump(target.Position, false, true);
                }
            }

            if (Misc.GetMenuItem("ElLeeSin.Combo.E"))
            {
                SpellE.CastE();
            }

            if (spells[Spells.W].IsReady() && Misc.GetMenuItem("ElLeeSin.Combo.W2")
                && (!Misc.GetMenuItem("ElLeeSin.Combo.W") || !Misc.GetMenuItem("ElLeeSin.Combo.Mode.WW")))
            {
                if (Environment.TickCount - lastSpellCastTime <= 500)
                {
                    return;
                }

                if (Misc.IsWOne)
                {
                    if (target.IsValidTarget(ObjectManager.Player.AttackRange + 50))
                    {
                        spells[Spells.W].Cast();
                        lastSpellCastTime = Environment.TickCount;
                    }
                }
                else
                {
                    if ((PassiveStacks == 0) || (Environment.TickCount - LastW >= 2500))
                    {
                        spells[Spells.W].Cast();
                        lastSpellCastTime = Environment.TickCount;
                    }
                }
            }
        }

        public static void KnockUp()
        {
            if (ObjectManager.Player.IsDashing())
            {
                return;
            }

            var minREnemies = MyMenu.Menu.Item("ElLeeSin.Combo.R.Count").GetValue<Slider>().Value;
            foreach (var enemy in HeroManager.Enemies.Where(e => e.IsValidTarget(1300f)))
            {
                var startPos = enemy.ServerPosition;
                var endPos = ObjectManager.Player.ServerPosition.Extend(
                    startPos,
                    ObjectManager.Player.Distance(enemy) + LeeSinRKickDistance);

                var rectangle = new Geometry.Polygon.Rectangle(startPos, endPos, LeeSinRKickWidth);
                if (HeroManager.Enemies.Count(x => rectangle.IsInside(x)) >= minREnemies)
                {
                    spells[Spells.R].Cast(enemy);
                }
            }
        }

        public static void KnockUpKill()
        {
            if (ObjectManager.Player.IsDashing())
            {
                return;
            }

            foreach (var enemy in HeroManager.Enemies.Where(e => e.IsValidTarget(1300f)))
            {
                var startPos = enemy.ServerPosition;
                var endPos = ObjectManager.Player.ServerPosition.Extend(
                    startPos,
                    ObjectManager.Player.Distance(enemy) + LeeSinRKickDistance);

                var rectangle = new Geometry.Polygon.Rectangle(startPos, endPos, LeeSinRKickWidth);
                if (
                    HeroManager.Enemies.Where(x => rectangle.IsInside(x) && !x.IsDead)
                        .Any(i => spells[Spells.R].IsKillable(i)) && spells[Spells.R].CastOnUnit(enemy))
                {
                    break;
                }
            }
        }

        public static void OnGameLoad(EventArgs args)
        {
            try
            {
                if (ObjectManager.Player.ChampionName != "LeeSin")
                {
                    return;
                }

                igniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");
                flashSlot = ObjectManager.Player.GetSpellSlot("summonerflash");

                spells[Spells.Q].SetSkillshot(0.25f, 60f, 1800f, true, SkillshotType.SkillshotLine);
                spells[Spells.E].SetTargetted(0.25f, float.MaxValue);
                spells[Spells.R2].SetSkillshot(0.25f, 100, 1500, false, SkillshotType.SkillshotLine);

                JumpHandler.Load();
                MyMenu.Initialize();

                GameObject.OnCreate += OnCreate;
                Game.OnWndProc += Game_OnWndProc;
                Drawing.OnDraw += Drawings.OnDraw;
                //Drawing.OnDraw += BubbaSharp.Drawing_OnDraw;
                Game.OnUpdate += Game_OnGameUpdate;
                Orbwalking.AfterAttack += AfterAttack;
                Orbwalking.BeforeAttack += BeforeAttack;
                GameObject.OnDelete += GameObject_OnDelete;
                Obj_AI_Base.OnBuffGain += PassiveManager.OnBuffAdd;
                Obj_AI_Base.OnBuffLose += PassiveManager.OnBuffLose;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var targ = target as Obj_AI_Base;
            if (!unit.IsMe || (targ == null))
            {
                return;
            }

            if (PassiveStacks > 0)
            {
                PassiveStacks = PassiveStacks - 1;
            }

            Orbwalker.SetOrbwalkingPoint(Vector3.Zero);
            var mode = Orbwalker.ActiveMode;
            if (mode.Equals(Orbwalking.OrbwalkingMode.None) || mode.Equals(Orbwalking.OrbwalkingMode.LastHit)
                || mode.Equals(Orbwalking.OrbwalkingMode.LaneClear))
            {
                return;
            }

            if (spells[Spells.E].IsReady() && (target.Type == GameObjectType.AIHeroClient) && Misc.IsEOne)
            {
                spells[Spells.E].Cast();
            }

            if (ItemManager.IsActive())
            {
                ItemManager.CastItems(targ);
            }
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var targ = args.Target as AIHeroClient;
            if (!args.Unit.IsMe || (targ == null))
            {
                return;
            }

            if (!spells[Spells.E].IsReady() || !Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.Combo))
            {
                return;
            }

            if (targ.Distance(ObjectManager.Player) <= Orbwalking.GetRealAutoAttackRange(ObjectManager.Player))
            {
                if (ItemManager.IsActive())
                {
                    ItemManager.CastItems(targ);
                }
            }
        }

        // credits brian
        private static bool CanCastQ2(Obj_AI_Base target)
        {
            var buff = target.GetBuff("BlindMonkSonicWave");
            return (buff != null) && (buff.EndTime - Game.Time <= 0.3 * (buff.EndTime - buff.StartTime));
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                if (ObjectManager.Player.IsDead || ObjectManager.Player.IsRecalling())
                {
                    return;
                }

                /*if (MyMenu.Menu.Item("bSharpOn").GetValue<KeyBind>().Active)
                  {
                      Misc.Orbwalk(Game.CursorPos);
  
                      var t = TargetSelector.GetTarget(1000 + spells[Spells.W].Range, TargetSelector.DamageType.Physical);
                      if (t != null)
                      {
                         BubbaSharp.BubbKushGo(t);
                      }
                }*/

                if ((doubleClickReset <= Environment.TickCount) && (clickCount != 0))
                {
                    doubleClickReset = float.MaxValue;
                    clickCount = 0;
                }

                if (clickCount >= 2)
                {
                    resetTime = Environment.TickCount + 3000;
                    InsecManager.ClicksecEnabled = true;
                    InsecManager.InsecClickPos = Game.CursorPos;
                    clickCount = 0;
                }

                if ((resetTime <= Environment.TickCount) && !MyMenu.Menu.Item("InsecEnabled").GetValue<KeyBind>().Active
                    && InsecManager.ClicksecEnabled)
                {
                    InsecManager.ClicksecEnabled = false;
                }

                if (q2Timer <= Environment.TickCount)
                {
                    q2Done = false;
                }

                if ((Misc.GetMenuItem("insecMode")
                         ? TargetSelector.GetSelectedTarget()
                         : TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical)) == null)
                {
                    InsecManager.insecComboStep = InsecManager.InsecComboStepSelect.None;
                }

                if (MyMenu.Menu.Item("starCombo").GetValue<KeyBind>().Active)
                {
                    StarCombo();
                }

                if (MyMenu.Menu.Item("ElLeeSin.Wardjump").GetValue<KeyBind>().Active)
                {
                    Wardmanager.WardjumpToMouse();
                }

                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        LaneClear();
                        JungleClear();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        Harass();
                        break;
                }

                if (Misc.GetMenuItem("ElLeeSin.Ignite.KS") && igniteSlot != SpellSlot.Unknown)
                {
                    var newTarget = TargetSelector.GetTarget(600f, TargetSelector.DamageType.True);
                    if (newTarget != null && ((ObjectManager.Player.Spellbook.CanUseSpell(igniteSlot) == SpellState.Ready)
                                              && (ObjectManager.Player.GetSummonerSpellDamage(newTarget, Damage.SummonerSpell.Ignite)
                                                  > newTarget.Health)))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(igniteSlot, newTarget);
                    }
                }

                if (MyMenu.Menu.Item("InsecEnabled").GetValue<KeyBind>().Active)
                {
                    if (Misc.GetMenuItem("insecOrbwalk"))
                    {
                        Misc.Orbwalk(Game.CursorPos);
                    }

                    var newTarget = Misc.GetMenuItem("insecMode")
                                        ? (TargetSelector.GetSelectedTarget()
                                           ?? TargetSelector.GetTarget(
                                               spells[Spells.Q].Range,
                                               TargetSelector.DamageType.Physical))
                                        : TargetSelector.GetTarget(
                                            spells[Spells.Q].Range,
                                            TargetSelector.DamageType.Physical);

                    if (newTarget != null)
                    {
                        InsecManager.InsecCombo(newTarget);
                    }
                }
                else
                {
                    InsecManager.isNullInsecPos = true;
                    wardJumped = false;
                }

                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                {
                    InsecManager.insecComboStep = InsecManager.InsecComboStepSelect.None;
                }

                if (MyMenu.Menu.Item("ElLeeSin.Insec.UseInstaFlash").GetValue<KeyBind>().Active)
                {
                    var target = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Physical);
                    if (target == null)
                    {
                        return;
                    }

                    if (spells[Spells.R].IsReady() && !target.IsZombie
                        && (ObjectManager.Player.Spellbook.CanUseSpell(flashSlot) == SpellState.Ready)
                        && target.IsValidTarget(spells[Spells.R].Range))
                    {
                        spells[Spells.R].CastOnUnit(target);
                    }
                }

                if (spells[Spells.R].IsReady() && Misc.GetMenuItem("ElLeeSin.Combo.New")
                    && !MyMenu.Menu.Item("InsecEnabled").GetValue<KeyBind>().Active
                    && !MyMenu.Menu.Item("ElLeeSin.Wardjump").GetValue<KeyBind>().Active
                    && !MyMenu.Menu.Item("ElLeeSin.Insec.UseInstaFlash").GetValue<KeyBind>().Active)
                {
                    KnockUp();
                }

                if (spells[Spells.R].IsReady() && Misc.GetMenuItem("ElLeeSin.Combo.New.R.Kill")
                    && !MyMenu.Menu.Item("InsecEnabled").GetValue<KeyBind>().Active
                    && !MyMenu.Menu.Item("ElLeeSin.Wardjump").GetValue<KeyBind>().Active
                    && !MyMenu.Menu.Item("ElLeeSin.Insec.UseInstaFlash").GetValue<KeyBind>().Active)
                {
                    KnockUpKill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }

            var asec = HeroManager.Enemies.Where(a => (a.Distance(Game.CursorPos) < 200) && a.IsValid && !a.IsDead);
            if (asec.Any())
            {
                return;
            }

            if (!lastClickBool || (clickCount == 0))
            {
                clickCount++;
                lastClickPos = Game.CursorPos;
                lastClickBool = true;
                doubleClickReset = Environment.TickCount + 600;
                return;
            }

            if (lastClickBool && (lastClickPos.Distance(Game.CursorPos) < 200))
            {
                clickCount++;
                lastClickBool = false;
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmitter))
            {
                return;
            }

            if (sender.Name.Contains("blindmonk_q_resonatingstrike") && waitingForQ2)
            {
                waitingForQ2 = false;
                q2Done = true;
                q2Timer = Environment.TickCount + 800;
            }
        }

        private static void Harass()
        {
            try
            {
                var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);
                if (target == null)
                {
                    return;
                }

                if (!Misc.IsQOne && Misc.GetMenuItem("ElLeeSin.Harass.Q1") && !Misc.IsQOne && spells[Spells.Q].IsReady()
                    && target.HasQBuff()
                    && ((spells[Spells.Q].GetDamage(target, 1) > target.Health)
                        || (target.Distance(ObjectManager.Player)
                            > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 50)))
                {
                    spells[Spells.Q].Cast();
                }

                if (Misc.GetMenuItem("ElLeeSin.Combo.AAStacks")
                    && (PassiveStacks > MyMenu.Menu.Item("ElLeeSin.Harass.PassiveStacks").GetValue<Slider>().Value)
                    && (Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) > ObjectManager.Player.Distance(target)))
                {
                    return;
                }

                if (spells[Spells.Q].IsReady() && Misc.GetMenuItem("ElLeeSin.Harass.Q1"))
                {
                    if (Misc.IsQOne && (target.Distance(ObjectManager.Player) < spells[Spells.Q].Range))
                    {
                        CastQ(target, Misc.GetMenuItem("ElLeeSin.Smite.Q"));
                    }
                }

                if (spells[Spells.E].IsReady() && Misc.GetMenuItem("ElLeeSin.Harass.E1"))
                {
                    if (Misc.IsEOne && (target.Distance(ObjectManager.Player) < spells[Spells.E].Range))
                    {
                        spells[Spells.E].Cast();
                        return;
                    }

                    if (!Misc.IsEOne
                        && (target.Distance(ObjectManager.Player)
                            > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 50))
                    {
                        spells[Spells.E].Cast();
                    }
                }

                if (Misc.GetMenuItem("ElLeeSin.Harass.Wardjump") && (ObjectManager.Player.Distance(target) < 50)
                    && !target.HasQBuff()
                    && (Misc.IsEOne || (!spells[Spells.E].IsReady() && Misc.GetMenuItem("ElLeeSin.Harass.E1")))
                    && (Misc.IsQOne || (!spells[Spells.Q].IsReady() && Misc.GetMenuItem("ElLeeSin.Harass.Q1"))))
                {
                    var min =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(a => a.IsAlly && (a.Distance(ObjectManager.Player) <= spells[Spells.W].Range))
                            .OrderByDescending(a => a.Distance(target))
                            .FirstOrDefault();

                    spells[Spells.W].CastOnUnit(min);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void JungleClear()
        {
            try
            {
                var minion =
                    MinionManager.GetMinions(
                        spells[Spells.Q].Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                if (Misc.IsEOne)
                {
                    if (spells[Spells.E].IsReady() && Misc.GetMenuItem("ElLeeSin.Jungle.E"))
                    {
                        if (PassiveStacks > 0)
                        {
                            return;
                        }

                        if (minion.Distance(ObjectManager.Player) < spells[Spells.E].Range + spells[Spells.E].Width)
                        {
                            spells[Spells.E].Cast();
                            lastSpellCastTime = Environment.TickCount;
                        }
                    }
                }
                else
                {
                    if ((PassiveStacks == 0) && (minion.Distance(ObjectManager.Player) < spells[Spells.E].Range)
                        && Misc.HasBlindMonkTempest(minion))
                    {
                        if (Environment.TickCount - lastSpellCastTime <= 500)
                        {
                            return;
                        }

                        spells[Spells.E].Cast();
                    }
                }

                if (ItemManager.IsActive())
                {
                    ItemManager.CastItems(minion);
                }

                if (Misc.IsQOne)
                {
                    if (spells[Spells.Q].IsReady() && Misc.GetMenuItem("ElLeeSin.Jungle.Q"))
                    {
                        if (PassiveStacks == 2)
                        {
                            return;
                        }

                        if (minion.Distance(ObjectManager.Player) < spells[Spells.Q].Range)
                        {
                            spells[Spells.Q].Cast(minion);
                            lastSpellCastTime = Environment.TickCount;
                        }
                    }
                }
                else
                {
                    if (CanCastQ2(minion) || (spells[Spells.Q].GetDamage(minion, 1) > minion.Health)
                        || (PassiveStacks == 0)
                        || (minion.Distance(ObjectManager.Player)
                            > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 50))
                    {
                        spells[Spells.Q].Cast();
                        lastSpellCastTime = Environment.TickCount;
                    }
                }

                if (Misc.IsWOne)
                {
                    if (spells[Spells.W].IsReady() && Misc.GetMenuItem("ElLeeSin.Jungle.W"))
                    {
                        if (PassiveStacks == 2)
                        {
                            return;
                        }

                        if (Misc.IsWOne && minion.IsValidTarget(ObjectManager.Player.AttackRange + 50))
                        {
                            spells[Spells.W].Cast();
                            lastSpellCastTime = Environment.TickCount;
                        }
                    }
                }
                else
                {
                    spells[Spells.W].Cast();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void LaneClear()
        {
            try
            {
                var minions = MinionManager.GetMinions(spells[Spells.Q].Range).FirstOrDefault();
                if (minions == null)
                {
                    return;
                }

                if (Environment.TickCount - lastSpellCastTime <= 500)
                {
                    return;
                }

                if (spells[Spells.Q].IsReady() && Misc.GetMenuItem("ElLeeSin.Lane.Q"))
                {
                    if (Misc.IsQOne)
                    {
                        if (minions.Distance(ObjectManager.Player) < spells[Spells.Q].Range)
                        {
                            spells[Spells.Q].Cast(minions);
                            lastSpellCastTime = Environment.TickCount;
                        }
                    }
                    else if (Misc.GetMenuItem("ElLeeSin.Lane.Q") && spells[Spells.Q].IsReady() && minions.HasQBuff()
                             && ((spells[Spells.Q].GetDamage(minions, 1) > minions.Health)
                                 || (minions.Distance(ObjectManager.Player)
                                     > Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 50)))
                    {
                        spells[Spells.Q].Cast();
                        lastSpellCastTime = Environment.TickCount;
                    }
                }

                if (Misc.GetMenuItem("ElLeeSin.Combo.AAStacks")
                    && (PassiveStacks > MyMenu.Menu.Item("ElLeeSin.Combo.PassiveStacks").GetValue<Slider>().Value)
                    && (Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) > ObjectManager.Player.Distance(minions)))
                {
                    return;
                }

                if (spells[Spells.E].IsReady() && Misc.GetMenuItem("ElLeeSin.Lane.E"))
                {
                    if (Misc.IsEOne)
                    {
                        if (PassiveStacks > 0)
                        {
                            return;
                        }

                        if (spells[Spells.E].IsInRange(minions))
                        {
                            spells[Spells.E].Cast();
                            lastSpellCastTime = Environment.TickCount;
                        }
                    }
                    else
                    {
                        if ((PassiveStacks == 0) && (minions.Distance(ObjectManager.Player) < spells[Spells.E].Range)
                            && Misc.HasBlindMonkTempest(minions))
                        {
                            if (Environment.TickCount - lastSpellCastTime <= 500)
                            {
                                return;
                            }

                            spells[Spells.E].Cast();
                        }
                    }
                }

                if (ItemManager.IsActive())
                {
                    ItemManager.CastItems(minions);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!sender.IsMe)
                {
                    return;
                }

                switch (args.Slot)
                {
                    case SpellSlot.Q:
                        if (!args.SData.Name.ToLower().Contains("one"))
                        {
                            isInQ2 = true;
                        }
                        break;

                    case SpellSlot.W:
                        if (args.SData.Name.ToLower().Contains("one"))
                        {
                            LastW = Environment.TickCount;
                        }
                        break;
                }

                switch (args.SData.Name.ToLower())
                {
                    case "blindmonkqone":
                        castQAgain = false;
                        LeagueSharp.Common.Utility.DelayAction.Add(2900, () => { castQAgain = true; });
                        break;

                    case "blindmonkqtwo":
                        waitingForQ2 = true;
                        LastQ2 = Environment.TickCount;
                        LeagueSharp.Common.Utility.DelayAction.Add(2900, () => { CheckQ = true; });
                        LeagueSharp.Common.Utility.DelayAction.Add(3000, () => { waitingForQ2 = false; });
                        break;

                    case "blindmonkrkick":
                        InsecManager.insecComboStep = InsecManager.InsecComboStepSelect.None;
                        break;
                }

                if (spells[Spells.R].IsReady()
                    && (ObjectManager.Player.Spellbook.CanUseSpell(flashSlot) == SpellState.Ready))
                {
                    var target = Misc.GetMenuItem("insecMode")
                                     ? TargetSelector.GetSelectedTarget()
                                     : TargetSelector.GetTarget(
                                         spells[Spells.R].Range,
                                         TargetSelector.DamageType.Physical);

                    if (target == null)
                    {
                        return;
                    }

                    if (args.SData.Name.Equals("BlindMonkRKick", StringComparison.InvariantCultureIgnoreCase)
                        && MyMenu.Menu.Item("ElLeeSin.Insec.UseInstaFlash").GetValue<KeyBind>().Active)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            80,
                            () => ObjectManager.Player.Spellbook.CastSpell(flashSlot, InsecManager.GetInsecPos(target)));
                    }
                }

                if (args.SData.Name.Equals("summonerflash", StringComparison.InvariantCultureIgnoreCase)
                    && (InsecManager.insecComboStep != InsecManager.InsecComboStepSelect.None))
                {
                    var target = Misc.GetMenuItem("insecMode")
                                     ? TargetSelector.GetSelectedTarget()
                                     : TargetSelector.GetTarget(
                                         spells[Spells.Q].Range,
                                         TargetSelector.DamageType.Physical);

                    if (target == null)
                    {
                        return;
                    }

                    InsecManager.insecComboStep = InsecManager.InsecComboStepSelect.Pressr;

                    LeagueSharp.Common.Utility.DelayAction.Add(80, () => spells[Spells.R].CastOnUnit(target));
                }

                foreach (var buff in buffNames)
                {
                    if (buff.Equals(args.SData.Name.ToLower(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        PassiveStacks = 2;
                        lastSpellCastTime = Environment.TickCount;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if ((Environment.TickCount < lastPlaced + 300) && spells[Spells.W].IsReady()
                && sender.Name.ToLower().Contains("ward"))
            {
                var ward = (Obj_AI_Base)sender;
                if (ward.Distance(Wardmanager.lastWardPos) < 500)
                {
                    spells[Spells.W].Cast(ward);
                }
            }
        }

        private static string SmiteSpellName()
        {
            if (SmiteBlue.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteplayerganker";
            }

            if (SmiteRed.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteduel";
            }

            return "summonersmite";
        }

        private static void StarCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(
                    spells[Spells.W].Range + spells[Spells.R].Range,
                    TargetSelector.DamageType.Physical);

                Orbwalking.Orbwalk(
                    target ?? null,
                    Game.CursorPos,
                    MyMenu.Menu.Item("ExtraWindup").GetValue<Slider>().Value,
                    MyMenu.Menu.Item("HoldPosRadius").GetValue<Slider>().Value);

                if (target == null)
                {
                    return;
                }

                if (ItemManager.IsActive())
                {
                    ItemManager.CastItems(target);
                }

                if (spells[Spells.Q].IsReady())
                {
                    if (Misc.IsQOne)
                    {
                        if (spells[Spells.R].IsReady() && spells[Spells.Q].Cast(target).IsCasted())
                        {
                            return;
                        }
                    }
                    else if (target.HasQBuff()
                             && (spells[Spells.Q].IsKillable(target, 1)
                                 || (!spells[Spells.R].IsReady()
                                     && (Utils.TickCount - spells[Spells.R].LastCastAttemptT > 300)
                                     && (Utils.TickCount - spells[Spells.R].LastCastAttemptT < 1500)
                                     && spells[Spells.Q].Cast())))
                    {
                        return;
                    }
                }

                if (spells[Spells.E].IsReady())
                {
                    if (Misc.IsEOne)
                    {
                        if (spells[Spells.E].IsInRange(target) && target.HasQBuff() && !spells[Spells.R].IsReady()
                            && (Utils.TickCount - spells[Spells.R].LastCastAttemptT < 1500)
                            && (ObjectManager.Player.Mana >= 80) && spells[Spells.E].Cast())
                        {
                            return;
                        }
                    }
                }

                if (!spells[Spells.Q].IsReady() || !spells[Spells.R].IsReady() || Misc.IsQOne || !target.HasQBuff())
                {
                    return;
                }

                if (spells[Spells.R].IsInRange(target))
                {
                    spells[Spells.R].CastOnUnit(target);
                }
                else if (spells[Spells.W].IsReady())
                {
                    if ((target.Distance(ObjectManager.Player) > spells[Spells.R].Range)
                        && (target.Distance(ObjectManager.Player) < spells[Spells.R].Range + 580) && target.HasQBuff())
                    {
                        Wardmanager.WardJump(target.Position, false);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}