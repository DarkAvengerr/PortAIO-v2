using EloBuddy; namespace ElVi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Vi
    {
        #region Static Fields

        public static readonly Dictionary<Spells, Spell> Spells = new Dictionary<Spells, Spell>
                                                                      {
                                                                          { ElVi.Spells.Q, new Spell(SpellSlot.Q, 800) },
                                                                          { ElVi.Spells.W, new Spell(SpellSlot.W) },
                                                                          { ElVi.Spells.E, new Spell(SpellSlot.E, 600) },
                                                                          { ElVi.Spells.R, new Spell(SpellSlot.R, 800) }
                                                                      };

        public static Orbwalking.Orbwalker Orbwalker;

        private static SpellSlot flash;

        private static SpellSlot ignite;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets Ravenous Hydra
        /// </summary>
        /// <value>
        ///     Ravenous Hydra
        /// </value>
        private static Items.Item Hydra => ItemData.Ravenous_Hydra_Melee_Only.GetItem();

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets Tiamat Item
        /// </summary>
        /// <value>
        ///     Tiamat Item
        /// </value>
        private static Items.Item Tiamat => ItemData.Tiamat_Melee_Only.GetItem();

        /// <summary>
        ///     Gets Titanic Hydra
        /// </summary>
        /// <value>
        ///     Titanic Hydra
        /// </value>
        private static Items.Item Titanic => ItemData.Titanic_Hydra_Melee_Only.GetItem();

        /// <summary>
        ///     Gets Youmuus Ghostblade
        /// </summary>
        /// <value>
        ///     Youmuus Ghostblade
        /// </value>
        private static Items.Item Youmuu => ItemData.Youmuus_Ghostblade.GetItem();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Cast items
        /// </summary>
        /// <param name="target"></param>
        /// <returns>true or false</returns>
        public static bool CastItems(Obj_AI_Base target)
        {
            if (Player.IsDashing() || Player.Spellbook.IsAutoAttacking)
            {
                return false;
            }

            var units =
                MinionManager.GetMinions(385, MinionTypes.All, MinionTeam.NotAlly).Count(o => !(o is Obj_AI_Turret));
            var heroes = Player.GetEnemiesInRange(385).Count;
            var count = units + heroes;

            var tiamat = Tiamat;
            if (tiamat.IsReady() && count > 0 && tiamat.Cast())
            {
                return true;
            }

            var hydra = Hydra;
            if (Hydra.IsReady() && count > 0 && hydra.Cast())
            {
                return true;
            }

            var youmuus = Youmuu;
            if (Youmuu.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && youmuus.Cast())
            {
                return true;
            }

            var titanic = Titanic;
            return titanic.IsReady() && count > 0 && titanic.Cast();
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Spells[ElVi.Spells.Q].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);
            }
            if (Spells[ElVi.Spells.E].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.E) * Spells[ElVi.Spells.E].Instance.Ammo
                          + (float)Player.GetAutoAttackDamage(enemy);
            }

            if (Spells[ElVi.Spells.R].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return (float)damage;
        }

        public static void OnLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Vi")
            {
                return;
            }

            ignite = Player.GetSpellSlot("summonerdot");
            flash = Player.GetSpellSlot("SummonerFlash");

            Spells[ElVi.Spells.Q].SetSkillshot(0.5f, 75f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Spells[ElVi.Spells.Q].SetCharged(100, 860, 1f);

            Spells[ElVi.Spells.E].SetSkillshot(0.15f, 150f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Spells[ElVi.Spells.R].SetTargetted(0.15f, 1500f);

            ElViMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            Orbwalking.AfterAttack += OrbwalkingAfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (ElViMenu._menu.Item("ElVi.misc.AntiGapCloser").GetValue<bool>())
            {
                if (Spells[ElVi.Spells.Q].IsReady() && gapcloser.Sender.Distance(Player) < Spells[ElVi.Spells.Q].Range)
                {
                    if (!Spells[ElVi.Spells.Q].IsCharging)
                    {
                        Spells[ElVi.Spells.Q].StartCharging();
                    }
                    else
                    {
                        Spells[ElVi.Spells.Q].Cast(gapcloser.Sender);
                    }
                }
            }
        }

        private static void FlashQ()
        {
            var target = TargetSelector.GetTarget(
                Spells[ElVi.Spells.Q].ChargedMaxRange,
                TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            var position = Spells[ElVi.Spells.Q].GetPrediction(target, true).CastPosition;

            if (Spells[ElVi.Spells.Q].IsReady())
            {
                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
                else
                {
                    ObjectManager.Player.Spellbook.CastSpell(flash, position);
                    Spells[ElVi.Spells.Q].Cast(target.ServerPosition);
                }
            }
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var useInterrupter = ElViMenu._menu.Item("ElVi.misc.Interrupter").GetValue<bool>();
            if (!useInterrupter)
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High
                || sender.Distance(Player) > Spells[ElVi.Spells.Q].Range)
            {
                return;
            }

            if (Spells[ElVi.Spells.Q].IsReady())
            {
                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
                else
                {
                    Spells[ElVi.Spells.Q].Cast(sender);
                }
            }

            if (Spells[ElVi.Spells.R].CanCast(sender) && args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                Spells[ElVi.Spells.R].Cast(sender);
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(
                Spells[ElVi.Spells.Q].ChargedMaxRange,
                TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (ElViMenu._menu.Item("ElVi.Combo.Q").GetValue<bool>() && Spells[ElVi.Spells.Q].IsReady())
            {
                if (Spells[ElVi.Spells.Q].IsCharging)
                {
                    if (Spells[ElVi.Spells.Q].IsInRange(target))
                    {
                        var prediction = Spells[ElVi.Spells.Q].GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            if (Spells[ElVi.Spells.Q].Range == Spells[ElVi.Spells.Q].ChargedMaxRange)
                            {
                                if (Spells[ElVi.Spells.Q].Cast(prediction.CastPosition))
                                {
                                    return;
                                }
                            }
                            else if (Spells[ElVi.Spells.Q].GetDamage(target) > target.Health)
                            {
                                var distance =
                                    Player.ServerPosition.Distance(
                                        prediction.UnitPosition
                                        * (prediction.UnitPosition - Player.ServerPosition).Normalized(),
                                        true);
                                if (distance < Spells[ElVi.Spells.Q].RangeSqr)
                                {
                                    if (Spells[ElVi.Spells.Q].Cast(prediction.CastPosition))
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
            }

            CastItems(target);

            if (ElViMenu._menu.Item("ElVi.Combo.R").GetValue<bool>() && Spells[ElVi.Spells.R].IsReady()
                && target.IsValidTarget(Spells[ElVi.Spells.R].Range))
            {
                var enemy =
                    HeroManager.Enemies.Where(
                        hero => ElViMenu._menu.Item("ElVi.Settings.R" + hero.CharData.BaseSkinName).GetValue<bool>())
                        .OrderByDescending(x => x.MaxHealth)
                        .FirstOrDefault();

                if (enemy.IsValidTarget() == false)
                {
                    return;
                }

                Spells[ElVi.Spells.R].CastOnUnit(enemy);
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health
                && ElViMenu._menu.Item("ElVi.Combo.I").GetValue<bool>())
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(
                Spells[ElVi.Spells.Q].ChargedMaxRange,
                TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (ElViMenu._menu.Item("ElVi.Harass.Q").GetValue<bool>() && Spells[ElVi.Spells.Q].IsReady())
            {
                if (Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].Cast(target);
                }
                else
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
            }

            if (ElViMenu._menu.Item("ElVi.Harass.E").GetValue<bool>() && Spells[ElVi.Spells.E].IsReady())
            {
                Spells[ElVi.Spells.E].Cast();
            }
        }

        private static void OnJungleClear()
        {
            var useQ = ElViMenu._menu.Item("ElVi.JungleClear.Q").GetValue<bool>();
            var useE = ElViMenu._menu.Item("ElVi.JungleClear.E").GetValue<bool>();
            var playerMana = ElViMenu._menu.Item("ElVi.Clear.Player.Mana").GetValue<Slider>().Value;

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                Spells[ElVi.Spells.E].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minions.Count <= 0)
            {
                return;
            }

            if (useQ && Spells[ElVi.Spells.Q].IsReady())
            {
                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
                else
                {
                    if (minions.Count == minions.Count(x => Player.Distance(x) < Spells[ElVi.Spells.Q].Range))
                    {
                        Spells[ElVi.Spells.Q].Cast(minions[0]);
                    }
                }
            }

            if (useE && Spells[ElVi.Spells.E].IsReady())
            {
                var bestFarmPos = Spells[ElVi.Spells.E].GetLineFarmLocation(minions);
                if (minions.Count == minions.Count(x => Player.Distance(x) < Spells[ElVi.Spells.E].Range)
                    && bestFarmPos.Position.IsValid() && bestFarmPos.MinionsHit > 1)
                {
                    Spells[ElVi.Spells.E].Cast();
                }
            }
        }

        private static void OnLaneClear()
        {
            var useQ = ElViMenu._menu.Item("ElVi.LaneClear.Q").GetValue<bool>();
            var useE = ElViMenu._menu.Item("ElVi.LaneClear.E").GetValue<bool>();
            var playerMana = ElViMenu._menu.Item("ElVi.Clear.Player.Mana").GetValue<Slider>().Value;

            if (Player.ManaPercent < playerMana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, Spells[ElVi.Spells.Q].Range);
            if (minions.Count <= 1)
            {
                return;
            }

            if (useQ && Spells[ElVi.Spells.Q].IsReady())
            {
                if (!Spells[ElVi.Spells.Q].IsCharging)
                {
                    Spells[ElVi.Spells.Q].StartCharging();
                }
                else
                {
                    var bestFarmPos = Spells[ElVi.Spells.Q].GetLineFarmLocation(minions);
                    if (minions.Count == minions.Count(x => Player.Distance(x) < Spells[ElVi.Spells.Q].Range)
                        && bestFarmPos.Position.IsValid() && bestFarmPos.MinionsHit > 2)
                    {
                        Spells[ElVi.Spells.Q].Cast(bestFarmPos.Position);
                    }
                }
            }

            if (useE && Spells[ElVi.Spells.E].IsReady())
            {
                if (minions.Count == minions.Count(x => Player.Distance(x) < Spells[ElVi.Spells.E].Range))
                {
                    Spells[ElVi.Spells.E].Cast();
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
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnLaneClear();
                    OnJungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
            }

            if (ElViMenu._menu.Item("ElVi.Combo.Flash").GetValue<KeyBind>().Active)
            {
                FlashQ();
            }
        }

        private static void OrbwalkingAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            if (unit.IsMe && ElViMenu._menu.Item("ElVi.Combo.E").GetValue<bool>())
            {
                Spells[ElVi.Spells.E].Cast();
            }

            Orbwalking.ResetAutoAttackTimer();
        }

        #endregion
    }
}