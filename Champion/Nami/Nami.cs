namespace ElNamiBurrito
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

    internal class Nami
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 875) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 725) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 800) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 2750) }
                                                             };

        private static SpellSlot ignite;

        #endregion

        #region Properties

        private static AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        public static void Game_OnGameLoad()
        {
            if (!Player.ChampionName.Equals("Nami", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            spells[Spells.Q].SetSkillshot(1f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            spells[Spells.R].SetSkillshot(0.5f, 260f, 850f, false, SkillshotType.SkillshotLine);

            ignite = Player.GetSpellSlot("summonerdot");

            ElNamiMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        #endregion

        #region Methods

        private static void AllyHealing()
        {
            if (Player.IsRecalling() || Player.InFountain()
                || !ElNamiMenu.Menu.Item("ElNamiReborn.Heal.Ally.HP").GetValue<bool>())
            {
                return;
            }

            foreach (var hero in HeroManager.Allies.Where(h => !h.IsMe && (!h.IsRecalling() || !h.InFountain())))
            {
                if ((hero.Health / hero.MaxHealth) * 100
                    <= ElNamiMenu.Menu.Item("ElNamiReborn.Heal.Ally.HP.Percentage").GetValue<Slider>().Value
                    && spells[Spells.W].IsReady() && hero.Distance(Player.ServerPosition) <= spells[Spells.W].Range
                    && Player.ManaPercent >= ElNamiMenu.Menu.Item("ElNamiReborn.Heal.Mana").GetValue<Slider>().Value)
                {
                    spells[Spells.W].Cast(hero);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.IsValidTarget(spells[Spells.Q].Range))
            {
                return;
            }

            if (gapcloser.Sender.Distance(Player) > spells[Spells.Q].Range)
            {
                return;
            }

            if (gapcloser.Sender.IsValidTarget(spells[Spells.Q].Range))
            {
                if (ElNamiMenu.Menu.Item("ElNamiReborn.Interupt.Q").IsActive() && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast(gapcloser.Sender);
                }

                if (ElNamiMenu.Menu.Item("ElNamiReborn.Interupt.R").IsActive() && !spells[Spells.Q].IsReady()
                    && spells[Spells.R].IsReady())
                {
                    spells[Spells.R].Cast(gapcloser.Sender);
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (ElNamiMenu.Menu.Item("ElNamiReborn.Combo.Q").IsActive() && spells[Spells.Q].IsReady())
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    spells[Spells.Q].Cast(pred.CastPosition);
                }
            }

            if (ElNamiMenu.Menu.Item("ElNamiReborn.Combo.E").IsActive() && spells[Spells.E].IsReady())
            {
                if (Player.GetEnemiesInRange(spells[Spells.E].Range).Any())
                {
                    var closestEnemy =
                        Player.GetEnemiesInRange(spells[Spells.E].Range)
                            .OrderByDescending(h => (h.PhysicalDamageDealtPlayer + h.MagicDamageDealtPlayer))
                            .FirstOrDefault();

                    if (closestEnemy == null)
                    {
                        return;
                    }

                    if (closestEnemy.HasBuffOfType(BuffType.Stun))
                    {
                        return;
                    }

                    spells[Spells.E].Cast(closestEnemy);
                }

                if (Player.GetAlliesInRange(spells[Spells.E].Range).Any() && Player.GetEnemiesInRange(800f).Count >= 1)
                {
                    var closestToTarget =
                        Player.GetAlliesInRange(spells[Spells.E].Range)
                            .OrderByDescending(h => (h.PhysicalDamageDealtPlayer + h.MagicDamageDealtPlayer))
                            .FirstOrDefault();

                    spells[Spells.E].Cast(closestToTarget);
                }
            }

            if (ElNamiMenu.Menu.Item("ElNamiReborn.Combo.W").IsActive() && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(target);
            }

            if (ElNamiMenu.Menu.Item("ElNamiReborn.Combo.R").IsActive() && spells[Spells.R].IsReady())
            {
                foreach (var x in
                    HeroManager.Enemies.Where((hero => !hero.IsDead && hero.IsValidTarget(spells[Spells.R].Range))))
                {
                    var pred = spells[Spells.R].GetPrediction(x);
                    if (pred.AoeTargetsHitCount
                        >= ElNamiMenu.Menu.Item("ElNamiReborn.Combo.R.Count").GetValue<Slider>().Value)
                    {
                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            spells[Spells.R].Cast(pred.CastPosition);
                        }
                    }
                }
            }

            if (ElNamiMenu.Menu.Item("ElNamiReborn.Combo.Ignite").IsActive() && Player.Distance(target) <= 600
                && IgniteDamage(target) >= target.Health)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (Player.ManaPercent < ElNamiMenu.Menu.Item("ElNamiReborn.Harass.Mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (ElNamiMenu.Menu.Item("ElNamiReborn.Harass.Q").IsActive() && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }

            if (ElNamiMenu.Menu.Item("ElNamiReborn.Harass.E").IsActive() && spells[Spells.E].IsReady())
            {
                if (Player.GetAlliesInRange(spells[Spells.E].Range).Any())
                {
                    var closestToTarget =
                        Player.GetAlliesInRange(spells[Spells.E].Range)
                            .OrderByDescending(h => (h.PhysicalDamageDealtPlayer + h.MagicDamageDealtPlayer))
                            .First();

                    LeagueSharp.Common.Utility.DelayAction.Add(100, () => spells[Spells.E].Cast(closestToTarget));
                }
                else
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(100, () => spells[Spells.E].Cast(Player));
                }
            }

            if (ElNamiMenu.Menu.Item("ElNamiReborn.Harass.W").IsActive() && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(target);
            }
        }

        private static float IgniteDamage(Obj_AI_Base target)
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
            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.Distance(Player) > spells[Spells.Q].Range)
            {
                return;
            }

            if (sender.IsValidTarget(spells[Spells.Q].Range) && args.DangerLevel == Interrupter2.DangerLevel.High
                && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast(sender);
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
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }

            PlayerHealing();
            AllyHealing();
        }

        private static void PlayerHealing()
        {
            if (Player.IsRecalling() || Player.InFountain()
                || !ElNamiMenu.Menu.Item("ElNamiReborn.Heal.Activate").IsActive())
            {
                return;
            }

            if ((Player.Health / Player.MaxHealth) * 100
                <= ElNamiMenu.Menu.Item("ElNamiReborn.Heal.Player.HP").GetValue<Slider>().Value
                && spells[Spells.W].IsReady()
                && ObjectManager.Player.ManaPercent
                >= ElNamiMenu.Menu.Item("ElNamiReborn.Heal.Mana").GetValue<Slider>().Value)
            {
                spells[Spells.W].Cast();
            }
        }

        #endregion
    }
}