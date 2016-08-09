namespace ElDiana
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Diana
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 895) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 240) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 450) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 825) }
                                                             };

        private static SpellSlot ignite;

        private static int lastNotification;

        #endregion

        #region Public Properties

        public static string ScriptVersion
        {
            get
            {
                return typeof(Diana).Assembly.GetName().Version.ToString();
            }
        }

        #endregion

        #region Properties

        private static HitChance CustomHitChance
        {
            get
            {
                return GetHitchance();
            }
        }

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

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

            return damage;
        }

        public static void OnLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Diana")
            {
                return;
            }

            spells[Spells.Q].SetSkillshot(0.25f, 150f, 1400f, false, SkillshotType.SkillshotCircle);
            ignite = Player.GetSpellSlot("summonerdot");

            ElDianaMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;

            Interrupter2.OnInterruptableTarget += (source, eventArgs) =>
                {
                    var eSlot = spells[Spells.E];
                    if (ElDianaMenu._menu.Item("ElDiana.Interrupt.UseEInterrupt").GetValue<bool>() && eSlot.IsReady()
                        && eSlot.Range >= Player.Distance(source, false))
                    {
                        eSlot.Cast();
                    }
                };

            CustomEvents.Unit.OnDash += (source, eventArgs) =>
                {
                    if (!source.IsEnemy)
                    {
                        return;
                    }

                    var eSlot = spells[Spells.E];
                    var dis = Player.Distance(source);
                    if (!eventArgs.IsBlink && ElDianaMenu._menu.Item("ElDiana.Interrupt.UseEDashes").GetValue<bool>()
                        && eSlot.IsReady() && eSlot.Range >= dis)
                    {
                        eSlot.Cast();
                    }
                };
        }

        #endregion

        #region Methods

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var useQ = ElDianaMenu._menu.Item("ElDiana.Combo.Q").GetValue<bool>();
            var useW = ElDianaMenu._menu.Item("ElDiana.Combo.W").GetValue<bool>();
            var useE = ElDianaMenu._menu.Item("ElDiana.Combo.E").GetValue<bool>();
            var useR = ElDianaMenu._menu.Item("ElDiana.Combo.R").GetValue<bool>();
            var useIgnite = ElDianaMenu._menu.Item("ElDiana.Combo.Ignite").GetValue<bool>();
            var secondR = ElDianaMenu._menu.Item("ElDiana.Combo.Secure").GetValue<bool>();
            var useSecondRLimitation =
                ElDianaMenu._menu.Item("ElDiana.Combo.UseSecondRLimitation").GetValue<Slider>().Value;
            var minHpToDive = ElDianaMenu._menu.Item("ElDiana.Combo.R.PreventUnderTower").GetValue<Slider>().Value;

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    spells[Spells.Q].Cast(pred.CastPosition);
                }
            }

            if (useR && spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range)
                && target.HasBuff("dianamoonlight")
                && (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent)))
            {
                spells[Spells.R].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady()
                && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(400f))
            {
                spells[Spells.E].Cast();
            }

            if (secondR && (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent)))
            {
                var closeEnemies = Player.GetEnemiesInRange(spells[Spells.R].Range * 2).Count;

                if (closeEnemies <= useSecondRLimitation && useR && !spells[Spells.Q].IsReady()
                    && spells[Spells.R].IsReady())
                {
                    if (target.Health < spells[Spells.R].GetDamage(target)
                        && (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent)))
                    {
                        spells[Spells.R].Cast(target);
                    }
                }

                if (closeEnemies <= useSecondRLimitation && spells[Spells.R].IsReady())
                {
                    if (target.Health < spells[Spells.R].GetDamage(target))
                    {
                        spells[Spells.R].Cast(target);
                    }
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health && useIgnite)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static HitChance GetHitchance()
        {
            switch (ElDianaMenu._menu.Item("ElDiana.hitChance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.VeryHigh;
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useQ = ElDianaMenu._menu.Item("ElDiana.Harass.Q").GetValue<bool>();
            var useW = ElDianaMenu._menu.Item("ElDiana.Harass.W").GetValue<bool>();
            var useE = ElDianaMenu._menu.Item("ElDiana.Harass.E").GetValue<bool>();
            var checkMana = ElDianaMenu._menu.Item("ElDiana.Harass.Mana").GetValue<Slider>().Value;

            if (Player.ManaPercent < checkMana)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= CustomHitChance)
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(target))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && Player.Distance(target) <= spells[Spells.E].Range)
            {
                spells[Spells.E].Cast();
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

        private static void JungleClear()
        {
            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            var useQ = ElDianaMenu._menu.Item("ElDiana.JungleClear.Q").GetValue<bool>();
            var useW = ElDianaMenu._menu.Item("ElDiana.JungleClear.W").GetValue<bool>();
            var useE = ElDianaMenu._menu.Item("ElDiana.JungleClear.E").GetValue<bool>();
            var useR = ElDianaMenu._menu.Item("ElDiana.JungleClear.R").GetValue<bool>();

            var qMinions = minions.FindAll(minion => minion.IsValidTarget(spells[Spells.Q].Range));
            var qMinion = qMinions.FirstOrDefault();

            if (qMinion == null)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                if (qMinion.IsValidTarget())
                {
                    spells[Spells.Q].Cast(qMinion);
                }
            }

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady()
                && qMinions.Count(m => Player.Distance(m, false) < spells[Spells.W].Range) < 1)
            {
                spells[Spells.E].Cast();
            }

            if (useR && spells[Spells.R].IsReady())
            {
                //find Mob with moonlight buff
                var moonlightMob =
                    minions.FindAll(minion => minion.HasBuff("dianamoonlight")).OrderBy(minion => minion.HealthPercent);
                if (moonlightMob.Any())
                {
                    //only cast when killable
                    var canBeKilled = moonlightMob.Find(minion => minion.Health < spells[Spells.R].GetDamage(minion));

                    //cast R on mob that can be killed
                    if (canBeKilled.IsValidTarget())
                    {
                        spells[Spells.R].Cast(canBeKilled);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var minion =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.Q].Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var useQ = ElDianaMenu._menu.Item("ElDiana.LaneClear.Q").GetValue<bool>();
            var useW = ElDianaMenu._menu.Item("ElDiana.LaneClear.W").GetValue<bool>();
            var useE = ElDianaMenu._menu.Item("ElDiana.LaneClear.E").GetValue<bool>();
            var useR = ElDianaMenu._menu.Item("ElDiana.LaneClear.R").GetValue<bool>();

            var countQ = ElDianaMenu._menu.Item("ElDiana.LaneClear.Count.Minions.Q").GetValue<Slider>().Value;
            var countW = ElDianaMenu._menu.Item("ElDiana.LaneClear.Count.Minions.W").GetValue<Slider>().Value;
            var countE = ElDianaMenu._menu.Item("ElDiana.LaneClear.Count.Minions.E").GetValue<Slider>().Value;

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.NotAlly);

            var qMinions = minions.FindAll(minionQ => minion.IsValidTarget(spells[Spells.Q].Range));
            var qMinion = qMinions.Find(minionQ => minionQ.IsValidTarget());

            if (useQ && spells[Spells.Q].IsReady()
                && spells[Spells.Q].GetCircularFarmLocation(minions).MinionsHit >= countQ)
            {
                spells[Spells.Q].Cast(qMinion);
            }

            if (useW && spells[Spells.W].IsReady()
                && spells[Spells.W].GetCircularFarmLocation(minions).MinionsHit >= countW)
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && Player.Distance(qMinion, false) < 200
                && spells[Spells.E].GetCircularFarmLocation(minions).MinionsHit >= countE)
            {
                spells[Spells.E].Cast();
            }

            var minionsR = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.Q].Range,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);

            if (useR && spells[Spells.R].IsReady())
            {
                //find Mob with moonlight buff
                var moonlightMob = minionsR.FindAll(x => x.HasBuff("dianamoonlight")).OrderBy(x => minion.HealthPercent);
                if (moonlightMob.Any())
                {
                    //only cast when killable
                    var canBeKilled = moonlightMob.Find(x => minion.Health < spells[Spells.R].GetDamage(minion));

                    //cast R on mob that can be killed
                    if (canBeKilled.IsValidTarget())
                    {
                        spells[Spells.R].Cast(canBeKilled);
                    }
                }
            }
        }

        private static void MisayaCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var minHpToDive = ElDianaMenu._menu.Item("ElDiana.Combo.R.PreventUnderTower").GetValue<Slider>().Value;

            var useQ = ElDianaMenu._menu.Item("ElDiana.Combo.Q").GetValue<bool>();
            var useW = ElDianaMenu._menu.Item("ElDiana.Combo.W").GetValue<bool>();
            var useE = ElDianaMenu._menu.Item("ElDiana.Combo.E").GetValue<bool>();
            var useR = ElDianaMenu._menu.Item("ElDiana.Combo.R").GetValue<bool>()
                       && (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent));
            var useIgnite = ElDianaMenu._menu.Item("ElDiana.Combo.Ignite").GetValue<bool>();
            var secondR = ElDianaMenu._menu.Item("ElDiana.Combo.Secure").GetValue<bool>()
                          && (!target.UnderTurret(true) || (minHpToDive <= Player.HealthPercent));
            var distToTarget = Player.Distance(target, false);
            var misayaMinRange = ElDianaMenu._menu.Item("ElDiana.Combo.R.MisayaMinRange").GetValue<Slider>().Value;
            var useSecondRLimitation =
                ElDianaMenu._menu.Item("ElDiana.Combo.UseSecondRLimitation").GetValue<Slider>().Value;

            // Can use R, R is ready but player too far from the target => do nothing
            if (useR && spells[Spells.R].IsReady() && distToTarget > spells[Spells.R].Range)
            {
                return;
            }

            // Prerequisites for Misaya Combo : If target is too close, won't work
            if (useQ && useR && spells[Spells.Q].IsReady() && spells[Spells.R].IsReady()
                && distToTarget >= misayaMinRange)
            {
                spells[Spells.R].Cast(target);
                // No need to check the hitchance since R is a targeted dash.
                spells[Spells.Q].Cast(target);
            }

            // Misaya Combo is not possible, classic mode then
            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    spells[Spells.Q].Cast(pred.CastPosition);
                }
            }

            if (useR && spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range)
                && target.HasBuff("dianamoonlight"))
            {
                spells[Spells.R].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady()
                && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(400f))
            {
                spells[Spells.E].Cast();
            }

            if (secondR)
            {
                var closeEnemies = Player.GetEnemiesInRange(spells[Spells.R].Range * 2).Count;

                if (closeEnemies <= useSecondRLimitation && useR && !spells[Spells.Q].IsReady()
                    && spells[Spells.R].IsReady())
                {
                    if (target.Health < spells[Spells.R].GetDamage(target))
                    {
                        spells[Spells.R].Cast(target);
                    }
                }

                if (closeEnemies <= useSecondRLimitation && spells[Spells.R].IsReady())
                {
                    if (target.Health < spells[Spells.R].GetDamage(target))
                    {
                        spells[Spells.R].Cast(target);
                    }
                }
            }

            if (Player.Distance(target, false) <= 600 && IgniteDamage(target) >= target.Health && useIgnite)
            {
                Player.Spellbook.CastSpell(ignite, target);
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
                    var ultType = ElDianaMenu._menu.Item("ElDiana.Combo.R.Mode").GetValue<StringList>().SelectedIndex;
                    if (ElDianaMenu._menu.Item("ElDiana.Hotkey.ToggleComboMode").GetValue<KeyBind>().Active)
                    {
                        ultType = (ultType + 1) % 2;
                    }
                    switch (ultType)
                    {
                        case 0:
                            Combo();
                            break;

                        case 1:
                            MisayaCombo();
                            break;
                    }

                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
        }

        #endregion
    }
}