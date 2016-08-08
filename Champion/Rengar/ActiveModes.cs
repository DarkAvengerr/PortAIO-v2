namespace ElRengarRevamped
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ActiveModes : Standards
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Handles combo
        /// </summary>
        public static void Combo()
        {
            var target = TargetSelector.GetSelectedTarget()
                             ?? TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
            if (target.LSIsValidTarget() == false)
            {
                return;
            }

            if (TargetSelector.GetSelectedTarget() != null)
            {
                Orbwalker.ForceTarget(target);
            }

            #region RengarR

            if (Ferocity <= 4)
            {
                if (spells[Spells.Q].LSIsReady() && IsActive("Combo.Use.Q")
                    && Player.LSCountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                {
                    spells[Spells.Q].Cast();
                }

                if (!RengarR)
                {
                    if (!HasPassive)
                    {
                        if (spells[Spells.E].LSIsReady() && IsActive("Combo.Use.E"))
                        {
                            CastE(target);
                        }
                    }
                    else
                    {
                        if (spells[Spells.E].LSIsReady() && IsActive("Combo.Use.E"))
                        {
                            if (Player.LSIsDashing())
                            {
                                CastE(target);
                            }
                        }
                    }
                }

                CastItems(target);

                if (spells[Spells.W].LSIsReady() && IsActive("Combo.Use.W"))
                {
                    CastW();
                }
            }

            if (Ferocity == 5)
            {
                switch (IsListActive("Combo.Prio").SelectedIndex)
                {
                    case 0:
                        if (!RengarR)
                        {
                            if (spells[Spells.E].LSIsReady() && !HasPassive)
                            {
                                CastE(target);

                                // switch combo mode
                                if (IsActive("Combo.Switch.E") && Utils.GameTimeTickCount - LastSwitch >= 350)
                                {
                                    MenuInit.Menu.Item("Combo.Prio")
                                        .SetValue(new StringList(new[] { "E", "W", "Q" }, 2));
                                    LastSwitch = Utils.GameTimeTickCount;
                                }
                            }
                        }
                        else
                        {
                            if (spells[Spells.E].LSIsReady() && IsActive("Combo.Use.E"))
                            {
                                if (Player.LSIsDashing())
                                {
                                    CastE(target);
                                }
                            }
                        }
                        break;
                    case 1:
                        if (IsActive("Combo.Use.W") && spells[Spells.W].LSIsReady())
                        {
                            CastW();
                        }
                        break;
                    case 2:
                        if (spells[Spells.Q].LSIsReady() && IsActive("Combo.Use.Q")
                            && Player.LSCountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                        {
                            spells[Spells.Q].Cast();
                        }
                        break;
                }
            }

            #region Summoner spells

            if (Youmuu.IsReady() && Youmuu.IsOwned() && target.LSIsValidTarget(spells[Spells.Q].Range))
            {
                Youmuu.Cast();
            }

            if (IsActive("Combo.Use.Ignite") && target.LSIsValidTarget(600f) && IgniteDamage(target) >= target.Health)
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Handles E cast
        /// </summary>
        /// <param name="target"></param>
        private static void CastE(Obj_AI_Base target)
        {
            if (!spells[Spells.E].LSIsReady() || !target.LSIsValidTarget(spells[Spells.E].Range))
            {
                return;
            }

            var pred = spells[Spells.E].GetPrediction(target);
            if (pred.Hitchance >= HitChance.High)
            {
                spells[Spells.E].Cast(pred.CastPosition);
            }
        }

        /// <summary>
        ///     Handles W casting
        /// </summary>
        private static void CastW()
        {
            if (!spells[Spells.W].LSIsReady())
            {
                return;
            }

            if (GetWHits().Item1 > 0)
            {
                spells[Spells.W].Cast();
            }
        }

        /// <summary>
        ///     Get W hits
        /// </summary>
        /// <returns></returns>
        private static Tuple<int, List<AIHeroClient>> GetWHits()
        {
            try
            {
                var hits =
                    HeroManager.Enemies.Where(
                        e =>
                        e.LSIsValidTarget() && e.LSDistance(Player) < 450f
                        || e.LSDistance(Player) < 450f).ToList();

                return new Tuple<int, List<AIHeroClient>>(hits.Count, hits);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new Tuple<int, List<AIHeroClient>>(0, null);
        }

        #endregion

        /// <summary>
        ///     Harass
        /// </summary>
        public static void Harass()
        {
            // ReSharper disable once ConvertConditionalTernaryToNullCoalescing
            var target = TargetSelector.GetSelectedTarget() != null
                             ? TargetSelector.GetSelectedTarget()
                             : TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);

            if (target.LSIsValidTarget() == false)
            {
                return;
            }

            #region RengarR

            if (Ferocity == 5)
            {
                switch (IsListActive("Harass.Prio").SelectedIndex)
                {
                    case 0:
                        if (!HasPassive && IsActive("Harass.Use.E") && spells[Spells.E].LSIsReady())
                        {
                            CastE(target);
                        }
                        break;

                    case 1:
                        if (IsActive("Harass.Use.Q") && target.LSIsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast();
                        }
                        break;
                }
            }

            if (Ferocity <= 4)
            {
                if (IsActive("Harass.Use.Q") && target.LSIsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast();
                }

                if (RengarR)
                {
                    return;
                }

                CastItems(target);

                if (!HasPassive && IsActive("Harass.Use.E") && spells[Spells.E].LSIsReady())
                {
                    CastE(target);
                }

                if (IsActive("Harass.Use.W"))
                {
                    CastW();
                }
            }
        }

        /// <summary>
        ///     Jungle clear
        /// </summary>
        public static void Jungleclear()
        {
            try
            {
                var minion =
                    MinionManager.GetMinions(
                        Player.ServerPosition,
                        spells[Spells.W].Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).FirstOrDefault();

                if (minion == null)
                {
                    return;
                }

                CastItems(minion);

                if (Ferocity == 5 && IsActive("Jungle.Save.Ferocity"))
                {
                    if (minion.LSIsValidTarget(spells[Spells.W].Range) && !HasPassive)
                    {
                        LaneItems(minion);
                    }
                    return;
                }

                if (IsActive("Jungle.Use.Q") && spells[Spells.Q].LSIsReady()
                    && minion.LSIsValidTarget(spells[Spells.Q].Range + 100))
                {
                    spells[Spells.Q].Cast();
                }

                LaneItems(minion);

                if (Ferocity == 5 && (Player.Health / Player.MaxHealth) * 100 <= 20)
                {
                    spells[Spells.W].Cast();
                }

                if (!HasPassive)
                {
                    if (IsActive("Jungle.Use.W") && spells[Spells.W].LSIsReady()
                        && minion.LSIsValidTarget(spells[Spells.W].Range))
                    {
                        if (Ferocity == 5 && spells[Spells.Q].LSIsReady())
                        {
                            return;
                        }
                        spells[Spells.W].Cast();
                    }
                }

                if (IsActive("Jungle.Use.E") && spells[Spells.E].LSIsReady()
                    && minion.LSIsValidTarget(spells[Spells.E].Range))
                {
                    if (Ferocity == 5)
                    {
                        return;
                    }

                    spells[Spells.E].Cast(minion.Position);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Lane clear
        /// </summary>
        public static void Laneclear()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            if (Player.Spellbook.IsAutoAttacking || Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Ferocity == 5 && IsActive("Clear.Save.Ferocity"))
            {
                if (minion.LSIsValidTarget(spells[Spells.W].Range))
                {
                    LaneItems(minion);
                }
                return;
            }

            if (IsActive("Clear.Use.Q") && spells[Spells.Q].LSIsReady()
                && minion.LSIsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast();
            }

            LaneItems(minion);

            if (IsActive("Clear.Use.W") && spells[Spells.W].LSIsReady()
                && minion.LSIsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (IsActive("Clear.Use.E") && spells[Spells.E].LSIsReady()
                && minion.LSIsValidTarget(spells[Spells.E].Range))
            {
                if (Ferocity == 5)
                {
                    return;
                }

                spells[Spells.E].Cast(minion.Position);
            }
        }

        /// <summary>
        ///     Gets Youmuus Ghostblade
        /// </summary>
        /// <value>
        ///     Youmuus Ghostblade
        /// </value>
        private static new Items.Item Youmuu => ItemData.Youmuus_Ghostblade.GetItem();

        /// <summary>
        ///     Gets Ravenous Hydra
        /// </summary>
        /// <value>
        ///     Ravenous Hydra
        /// </value>
        private static Items.Item Hydra => ItemData.Ravenous_Hydra_Melee_Only.GetItem();

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
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool LaneItems(Obj_AI_Base target)
        {
            var units =
                MinionManager.GetMinions(385, MinionTypes.All, MinionTeam.NotAlly).Count(o => !(o is Obj_AI_Turret));
            var count = units;
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

            return false;
        }

        /// <summary>
        ///     Cast items
        /// </summary>
        /// <param name="target"></param>
        /// <returns>true or false</returns>
        public static bool CastItems(Obj_AI_Base target)
        {
            if (Player.LSIsDashing() || Player.Spellbook.IsAutoAttacking || RengarR)
            {
                return false;
            }

            var heroes = Player.LSGetEnemiesInRange(385).Count;
            var count = heroes;

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

        #endregion
    }
}