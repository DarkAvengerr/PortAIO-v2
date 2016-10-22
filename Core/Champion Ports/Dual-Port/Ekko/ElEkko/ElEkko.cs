using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElEkko
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class ElEkko
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 950) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 1600) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 425) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 400) }
                                                             };

        private static readonly Dictionary<float, float> incomingDamage = new Dictionary<float, float>();

        private static readonly Dictionary<float, float> instantDamage = new Dictionary<float, float>();

        private static SpellSlot ignite;

        #endregion

        #region Public Properties

        public static Vector3 FleePosition { get; set; }

        public static float IncomingDamage
        {
            get
            {
                return incomingDamage.Sum(e => e.Value) + instantDamage.Sum(e => e.Value);
            }
        }

        public static bool IsJumpPossible { get; set; }

        public static string ScriptVersion => typeof(ElEkko).Assembly.GetName().Version.ToString();

        public static GameObject Troy { get; set; }

        #endregion

        #region Properties

        private static AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (spells[Spells.Q].IsReady())
            {
                damage += spells[Spells.Q].GetDamage(enemy);
            }

            if (spells[Spells.W].IsReady())
            {
                damage += spells[Spells.W].GetDamage(enemy);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return damage + 15 + (12 * Player.Level) + Player.FlatMagicDamageMod;
        }


        public static void OnLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Ekko")
            {
                return;
            }

            ignite = Player.GetSpellSlot("summonerdot");

            spells[Spells.Q].SetSkillshot(0.25f, 60, 1650f, false, SkillshotType.SkillshotLine);
            spells[Spells.W].SetSkillshot(2.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            ElEkkoMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.OnDraw;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            GameObject.OnDelete += Obj_AI_Base_OnDelete;
        }

        #endregion

        #region Methods

        private static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Player.IsRecalling())
            {
                return;
            }

            if (ElEkkoMenu._menu.Item("ElEkko.AutoHarass.Q").GetValue<KeyBind>().Active)
            {
                var mana = ElEkkoMenu._menu.Item("ElEkko.Harass.Q.Mana").GetValue<Slider>().Value;

                if (Player.ManaPercent < mana)
                {
                    return;
                }

                if (spells[Spells.Q].IsReady() && target.Distance(Player.Position) <= spells[Spells.Q].Range - 50
                    && !Player.IsDashing())
                {
                    spells[Spells.Q].Cast(target);
                }
            }
        }

        private static int CountPassive(Obj_AI_Base target)
        {
            var ekkoPassive = target.Buffs.FirstOrDefault(x => x.Name.Equals("EkkoStacks", StringComparison.InvariantCultureIgnoreCase));
            if (ekkoPassive != null)
            {
                return ekkoPassive.Count;
            }

            return 0;
        }

        private static void FleeMode()
        {
            var fleeActive = ElEkkoMenu._menu.Item("ElEkko.Flee.Key").GetValue<KeyBind>().Active;

            if (!fleeActive)
            {
                return;
            }

            var wallCheck = GetFirstWallPoint(Player.Position, Game.CursorPos);

            // Be more precise
            if (wallCheck != null)
            {
                wallCheck = GetFirstWallPoint((Vector3)wallCheck, Game.CursorPos, 5);
            }

            // Define more position point
            var movePosition = wallCheck != null ? (Vector3)wallCheck : Game.CursorPos;

            // Update fleeTargetPosition
            var tempGrid = NavMesh.WorldToGrid(movePosition.X, movePosition.Y);
            var fleeTargetPosition = NavMesh.GridToWorld((short)tempGrid.X, (short)tempGrid.Y);

            // Also check if we want to AA aswell
            Obj_AI_Base target = null;

            // Reset walljump indicators
            var wallJumpPossible = false;

            // Only calculate stuff when our Q is up and there is a wall inbetween
            if (spells[Spells.E].IsReady() && wallCheck != null)
            {
                // Get our wall position to calculate from
                var wallPosition = movePosition;

                // Check 300 units to the cursor position in a 160 degree cone for a valid non-wall spot
                var direction = (Game.CursorPos.To2D() - wallPosition.To2D()).Normalized();
                float maxAngle = 80;
                var step = maxAngle / 20;
                float currentAngle = 0;
                float currentStep = 0;
                var jumpTriggered = false;
                while (true)
                {
                    // Validate the counter, break if no valid spot was found in previous loops
                    if (currentStep > maxAngle && currentAngle < 0)
                    {
                        break;
                    }

                    // Check next angle
                    if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                    {
                        currentAngle = (currentStep) * (float)Math.PI / 180;
                        currentStep += step;
                    }

                    else if (currentAngle > 0)
                    {
                        currentAngle = -currentAngle;
                    }

                    Vector3 checkPoint;

                    // One time only check for direct line of sight without rotating
                    if (currentStep == 0)
                    {
                        currentStep = step;
                        checkPoint = wallPosition + spells[Spells.E].Range * direction.To3D();
                    }
                    // Rotated check
                    else
                    {
                        checkPoint = wallPosition + spells[Spells.E].Range * direction.Rotated(currentAngle).To3D();
                    }

                    // Check if the point is not a wall
                    if (!checkPoint.IsWall())
                    {
                        // Check if there is a wall between the checkPoint and wallPosition
                        wallCheck = GetFirstWallPoint(checkPoint, wallPosition);
                        if (wallCheck != null)
                        {
                            // There is a wall inbetween, get the closes point to the wall, as precise as possible
                            var wallPositionOpposite = (Vector3)GetFirstWallPoint((Vector3)wallCheck, wallPosition, 5);

                            // Check if it's worth to jump considering the path length
                            if (Player.GetPath(wallPositionOpposite).ToList().To2D().PathLength()
                                - Player.Distance(wallPositionOpposite) > 200)
                            {
                                // Check the distance to the opposite side of the wall
                                if (Player.Distance(wallPositionOpposite, true)
                                    < Math.Pow(spells[Spells.E].Range - Player.BoundingRadius / 2, 2))
                                {
                                    // Make the jump happen
                                    spells[Spells.E].Cast(wallPositionOpposite);

                                    // Update jumpTriggered value to not orbwalk now since we want to jump
                                    jumpTriggered = true;

                                    break;
                                }
                                wallJumpPossible = true;
                            }
                        }
                    }
                }

                // Check if the loop triggered the jump, if not just orbwalk
                if (!jumpTriggered)
                {
                    Orbwalking.Orbwalk(target, Game.CursorPos, 90f, 0f, false, false);
                }
            }
            else
            {
                Orbwalking.Orbwalk(target, Game.CursorPos, 90f, 0f, false, false);
                if (spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(Game.CursorPos);
                }
            }
        }

        private static Vector2? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25)
        {
            return GetFirstWallPoint(from.To2D(), to.To2D(), step);
        }

        private static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                {
                    return from + (d - step) * direction;
                }
            }

            return null;
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }


        private static void KillSteal()
        {
            var isActive = ElEkkoMenu._menu.Item("ElEkko.Killsteal.Active").GetValue<bool>();
            if (isActive)
            {
                foreach (var hero in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            hero =>
                            ObjectManager.Player.Distance(hero.ServerPosition) <= spells[Spells.Q].Range && !hero.IsMe
                            && hero.IsValidTarget() && hero.IsEnemy && !hero.IsInvulnerable))
                {
                    var qDamage = spells[Spells.Q].GetDamage(hero);
                    var useQ = ElEkkoMenu._menu.Item("ElEkko.Killsteal.Q").GetValue<bool>();
                    var useR = ElEkkoMenu._menu.Item("ElEkko.Killsteal.R").GetValue<bool>();
                    var useIgnite = ElEkkoMenu._menu.Item("ElEkko.Killsteal.Ignite").GetValue<bool>();

                    if (useQ && hero.Health - qDamage < 0 && spells[Spells.Q].IsReady()
                        && spells[Spells.Q].IsInRange(hero))
                    {
                        spells[Spells.Q].Cast(hero);
                    }

                    if (useR && spells[Spells.R].IsReady())
                    {
                        if (spells[Spells.R].GetDamage(hero) > hero.Health)
                        {
                            if (Troy != null)
                            {
                                if (hero.Distance(Troy.Position) <= spells[Spells.R].Range)
                                {
                                    spells[Spells.R].Cast();
                                }
                            }
                        }
                    }

                    if (useIgnite && Player.Distance(hero) <= 600 && IgniteDamage(hero) >= hero.Health)
                    {
                        Player.Spellbook.CastSpell(ignite, hero);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            var particle = obj as Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    Troy = particle;
                }
            }
        }

        private static void Obj_AI_Base_OnDelete(GameObject obj, EventArgs args)
        {
            var particle = obj as Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    Troy = null;
                }
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy)
            {
                if (Player != null && spells[Spells.R].IsReady())
                {
                    if ((!(sender is AIHeroClient) || args.SData.IsAutoAttack()) && args.Target != null
                        && args.Target.NetworkId == Player.NetworkId)
                    {
                        incomingDamage.Add(
                            Player.ServerPosition.Distance(sender.ServerPosition) / args.SData.MissileSpeed + Game.Time,
                            (float)sender.GetAutoAttackDamage(Player));
                    }
                    else if (sender is AIHeroClient)
                    {
                        var attacker = (AIHeroClient)sender;
                        var slot = attacker.GetSpellSlot(args.SData.Name);

                        if (slot != SpellSlot.Unknown)
                        {
                            if (slot == attacker.GetSpellSlot("SummonerDot") && args.Target != null
                                && args.Target.NetworkId == Player.NetworkId)
                            {
                                instantDamage.Add(
                                    Game.Time + 2,
                                    (float)attacker.GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite));
                            }
                            else if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R)
                                     && ((args.Target != null && args.Target.NetworkId == Player.NetworkId)
                                         || args.End.Distance(Player.ServerPosition) < Math.Pow(args.SData.LineWidth, 2)))
                            {
                                instantDamage.Add(Game.Time + 2, (float)attacker.GetSpellDamage(Player, slot));
                            }
                        }
                    }
                }
            }

            if (sender.IsMe)
            {
                if (args.SData.Name.Equals("EkkoE", StringComparison.InvariantCultureIgnoreCase))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, Orbwalking.ResetAutoAttackTimer);
                }
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = ElEkkoMenu._menu.Item("ElEkko.Combo.Q").GetValue<bool>();
            var useW = ElEkkoMenu._menu.Item("ElEkko.Combo.W").GetValue<bool>();
            var useE = ElEkkoMenu._menu.Item("ElEkko.Combo.E").GetValue<bool>();
            var useR = ElEkkoMenu._menu.Item("ElEkko.Combo.R").GetValue<bool>();
            var useRkill = ElEkkoMenu._menu.Item("ElEkko.Combo.R.Kill").GetValue<bool>();
            var useIgnite = ElEkkoMenu._menu.Item("ElEkko.Combo.Ignite").GetValue<bool>();

            var enemies = ElEkkoMenu._menu.Item("ElEkko.Combo.W.Count").GetValue<Slider>().Value;
            var enemiesRrange = ElEkkoMenu._menu.Item("ElEkko.Combo.R.Enemies").GetValue<Slider>().Value;

            if (useQ && spells[Spells.Q].IsReady() && target.Distance(Player.Position) <= spells[Spells.Q].Range - 50
                && !Player.IsDashing())
            {
                spells[Spells.Q].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady())
            {
                if (target.Distance(Player.Position) >= spells[Spells.E].Range)
                {
                    return;
                }

                if (Player.CountEnemiesInRange(spells[Spells.W].Range) >= enemies)
                {
                    var pred = spells[Spells.W].GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spells[Spells.W].Cast(pred.CastPosition);
                    }
                }
                else if (target.HasBuffOfType(BuffType.Slow) || target.HasBuffOfType(BuffType.Taunt)
                         || target.HasBuffOfType(BuffType.Stun)
                         || target.HasBuffOfType(BuffType.Snare)
                         && target.Distance(Player.Position) <= spells[Spells.E].Range)
                {
                    var pred = spells[Spells.W].GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spells[Spells.W].Cast(pred.CastPosition);
                    }
                }
                else
                {
                    if (target.ServerPosition.Distance(Player.Position)
                        > spells[Spells.E].Range * spells[Spells.E].Range)
                    {
                        if (spells[Spells.W].GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                        {
                            var pred = spells[Spells.W].GetPrediction(target);
                            if (pred.Hitchance >= HitChance.High)
                            {
                                spells[Spells.W].Cast(pred.CastPosition);
                            }
                        }
                    }
                }
            }

            if (useE && spells[Spells.E].IsReady() && !spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target)
                && !ObjectManager.Player.UnderTurret(true) && target.HasBuff("EkkoStacks"))
            {
                var eCast = ElEkkoMenu._menu.Item("ElEkko.Combo.E.Cast").GetValue<StringList>().SelectedIndex;
                switch (eCast)
                {
                    case 0:
                        spells[Spells.E].Cast(target.Position);
                        break;

                    case 1:
                        spells[Spells.E].Cast(Game.CursorPos);
                        break;
                }
            }

            if (useR && spells[Spells.R].IsReady())
            {
                if (target.Health < spells[Spells.R].GetDamage(target))
                {
                    if (Troy != null)
                    {
                        if (target.Distance(Troy.Position) <= spells[Spells.R].Range)
                        {
                            spells[Spells.R].Cast();
                        }
                    }
                }

                var enemyCount =
                    HeroManager.Enemies.Count(
                        h => h.IsValidTarget() && h.Distance(Troy.Position) < spells[Spells.R].Range);
                if (enemyCount >= enemiesRrange)
                {
                    if (Troy != null)
                    {
                        if (target.Distance(Troy.Position) <= spells[Spells.R].Range)
                        {
                            spells[Spells.R].Cast();
                        }
                    }
                }
            }

            if (useIgnite && Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = ElEkkoMenu._menu.Item("ElEkko.Harass.Q").GetValue<bool>();
            var useE = ElEkkoMenu._menu.Item("ElEkko.Harass.E").GetValue<bool>();
            var mana = ElEkkoMenu._menu.Item("ElEkko.Harass.Q.Mana").GetValue<Slider>().Value;

            if (Player.ManaPercent < mana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && target.Distance(Player.Position) <= spells[Spells.Q].Range
                && !Player.IsDashing())
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && !spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target)
                && !ObjectManager.Player.UnderTurret(true) && target.HasBuff("EkkoStacks"))
            {
                var eCast = ElEkkoMenu._menu.Item("ElEkko.Combo.E.Cast").GetValue<StringList>().SelectedIndex;
                switch (eCast)
                {
                    case 0:
                        spells[Spells.E].Cast(target.Position);
                        break;

                    case 1:
                        spells[Spells.E].Cast(Game.CursorPos);
                        break;
                }
            }
        }

        private static void OnJungleClear()
        {
            var useQ = ElEkkoMenu._menu.Item("ElEkko.JungleClear.Q").GetValue<bool>();
            var useW = ElEkkoMenu._menu.Item("ElEkko.JungleClear.W").GetValue<bool>();
            var mana = ElEkkoMenu._menu.Item("ElEkko.JungleClear.mana").GetValue<Slider>().Value;
            var qMinions = ElEkkoMenu._menu.Item("ElEkko.JungleClear.Minions").GetValue<Slider>().Value;

            if (Player.ManaPercent < mana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (minions.Count <= 0)
            {
                return;
            }

            var minionsInRange = minions.Where(x => spells[Spells.Q].IsInRange(x));
            var objAiBases = minionsInRange as IList<Obj_AI_Base> ?? minionsInRange.ToList();
            if (objAiBases.Count() >= qMinions && useQ)
            {
                var qKills = 0;
                foreach (var minion in objAiBases)
                {
                    if (spells[Spells.Q].GetDamage(minion) < minion.Health)
                    {
                        qKills++;

                        if (qKills >= qMinions)
                        {
                            var bestFarmPos = spells[Spells.Q].GetLineFarmLocation(minions);
                            spells[Spells.Q].Cast(bestFarmPos.Position);
                        }
                    }
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                var mobs = MinionManager.GetMinions(
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Count <= 0)
                {
                    return;
                }

                var bestFarmPos = spells[Spells.W].GetCircularFarmLocation(minions);
                spells[Spells.W].Cast(bestFarmPos.Position);
            }
        }

        private static void OnLaneClear()
        {
            var useQ = ElEkkoMenu._menu.Item("ElEkko.LaneClear.Q").GetValue<bool>();
            var mana = ElEkkoMenu._menu.Item("ElEkko.LaneClear.mana").GetValue<Slider>().Value;
            var qMinions = ElEkkoMenu._menu.Item("ElEkko.LaneClear.Minions").GetValue<Slider>().Value;

            if (Player.ManaPercent < mana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);
            if (minions.Count <= 0)
            {
                return;
            }

            var minionsInRange = minions.Where(x => spells[Spells.Q].IsInRange(x));
            var objAiBases = minionsInRange as IList<Obj_AI_Base> ?? minionsInRange.ToList();
            if (objAiBases.Count() >= qMinions && useQ)
            {
                var qKills = 0;
                foreach (var minion in objAiBases)
                {
                    if (spells[Spells.Q].GetDamage(minion) < minion.Health)
                    {
                        qKills++;

                        if (qKills >= qMinions)
                        {
                            var bestFarmPos = spells[Spells.Q].GetLineFarmLocation(minions);
                            spells[Spells.Q].Cast(bestFarmPos.Position);
                            return;
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnLaneClear();
                    OnJungleClear();
                    break;
            }

            FleeMode();

            var twoStacksQ = ElEkkoMenu._menu.Item("ElEkko.Combo.Auto.Q").GetValue<bool>();
            if (twoStacksQ)
            {
                var qtarget = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
                if (qtarget == null || !qtarget.IsValid || !Orbwalking.CanMove(1))
                {
                    return;
                }

                if (CountPassive(qtarget) == 2 && qtarget.Distance(Player.Position) <= spells[Spells.Q].Range)
                {
                    var pred = spells[Spells.Q].GetPrediction(qtarget);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spells[Spells.Q].Cast(pred.CastPosition);
                    }
                }
            }

            SaveMode();
            KillSteal();
            AutoHarass();

            var rtext = ElEkkoMenu._menu.Item("ElEkko.R.text").GetValue<bool>();
            if (Troy != null && rtext)
            {
                var enemyCount =
                    HeroManager.Enemies.Count(
                        h => h.IsValidTarget() && h.Distance(Troy.Position) < spells[Spells.R].Range);
                Drawing.DrawText(
                    Drawing.Width * 0.44f,
                    Drawing.Height * 0.80f,
                    Color.White,
                    "There are {0} in R range",
                    enemyCount);
            }
        }

        private static void SaveMode()
        {
            if (Player.IsRecalling() || Player.InFountain())
            {
                return;
            }

            var useR = ElEkkoMenu._menu.Item("ElEkko.Combo.R").GetValue<bool>();
            var playerHp = ElEkkoMenu._menu.Item("ElEkko.Combo.R.HP").GetValue<Slider>().Value;

            if (useR && spells[Spells.R].IsReady())
            {
                if (Player.HealthPercent < playerHp && Player.CountEnemiesInRange(600) > 0)
                {
                    spells[Spells.R].Cast();
                }
            }
        }

        #endregion
    }
}