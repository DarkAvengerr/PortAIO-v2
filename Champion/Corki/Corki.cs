namespace ElCorki
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;

    internal enum Spells
    {
        Q,

        W,

        E,

        R1,

        R2
    }

    internal static class Corki
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 825) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 800) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 600) },
                                                                 { Spells.R1, new Spell(SpellSlot.R, 1300) },
                                                                 { Spells.R2, new Spell(SpellSlot.R, 1500) }
                                                             };

        private static SpellSlot _ignite;

        #endregion

        #region Public Properties

        public static string ScriptVersion
        {
            get
            {
                return typeof(Corki).Assembly.GetName().Version.ToString();
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

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Corki")
            {
                return;
            }

            Console.WriteLine("Injected");
            Notifications.AddNotification(string.Format("ElCorki by jQuery v{0}", ScriptVersion), 8000);

            spells[Spells.Q].SetSkillshot(0.35f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            spells[Spells.E].SetSkillshot(0f, (float)(45 * Math.PI / 180), 1500, false, SkillshotType.SkillshotCone);
            spells[Spells.R1].SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
            spells[Spells.R2].SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
            _ignite = Player.LSGetSpellSlot("summonerdot");

            ElCorkiMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
        }

        #endregion

        #region Methods

        private static void AutoHarassMode()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);
            var rTarget = TargetSelector.GetTarget(spells[Spells.R1].Range, TargetSelector.DamageType.Magical);

            if (target == null || !target.LSIsValidTarget() || rTarget == null || !rTarget.LSIsValidTarget())
            {
                return;
            }

            if (ElCorkiMenu._menu.Item("ElCorki.AutoHarass").GetValue<KeyBind>().Active)
            {
                var q = ElCorkiMenu._menu.Item("ElCorki.UseQAutoHarass").IsActive();
                var r = ElCorkiMenu._menu.Item("ElCorki.UseQAutoHarass").IsActive();
                var mana = ElCorkiMenu._menu.Item("ElCorki.harass.mana").GetValue<Slider>().Value;

                if (Player.ManaPercent < mana)
                {
                    return;
                }

                if (r && spells[Spells.R1].LSIsReady() && spells[Spells.R1].IsInRange(rTarget)
                    || spells[Spells.R2].IsInRange(rTarget))
                {
                    var bigR = HasBigRocket();

                    var _target = TargetSelector.GetTarget(
                        bigR ? spells[Spells.R2].Range : spells[Spells.R1].Range,
                        TargetSelector.DamageType.Magical);
                    if (_target != null)
                    {
                        if (bigR)
                        {
                            spells[Spells.R2].Cast(_target);
                        }
                        else
                        {
                            spells[Spells.R1].Cast(_target);
                        }
                    }
                }

                if (q && spells[Spells.Q].LSIsReady() && target.LSIsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast(target);
                }
            }
        }

        private static bool HasBigRocket()
        {
            return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName.ToLower() == "corkimissilebarragecounterbig");
        }

        private static void Combo(Obj_AI_Base target)
        {
            if (target == null || !target.LSIsValidTarget())
            {
                return;
            }

            var comboQ = ElCorkiMenu._menu.Item("ElCorki.Combo.Q").IsActive();
            var comboE = ElCorkiMenu._menu.Item("ElCorki.Combo.E").IsActive();
            var comboR = ElCorkiMenu._menu.Item("ElCorki.Combo.R").IsActive();
            var useIgnite = ElCorkiMenu._menu.Item("ElCorki.Combo.Ignite").IsActive();
            var rStacks = ElCorkiMenu._menu.Item("ElCorki.Combo.RStacks").GetValue<Slider>().Value;
            var rTarget = TargetSelector.GetTarget(spells[Spells.R1].Range, TargetSelector.DamageType.Magical);

            var bigR = HasBigRocket();

            var _target = TargetSelector.GetTarget(
                   bigR ? spells[Spells.R2].Range : spells[Spells.R1].Range,
                   TargetSelector.DamageType.Magical);

            Items(target);

            if (comboQ && spells[Spells.Q].LSIsReady())
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    spells[Spells.Q].Cast(pred.CastPosition);
                }
            }

            if (comboE && spells[Spells.E].LSIsReady())
            {
                spells[Spells.E].Cast(target);
            }

            if (comboR && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo > rStacks && _target != null)
            {
                if (!bigR)
                {
                    if (target.LSIsValidTarget(spells[Spells.R1].Range))
                    {
                        spells[Spells.R1].Cast(target);
                    }
                }
                else
                {
                    if (target.LSIsValidTarget(spells[Spells.R2].Range))
                    {
                        spells[Spells.R2].Cast(target);
                    }
                }
            }

            if (Player.LSDistance(target) <= 600 && IgniteDamage(target) >= target.Health && useIgnite)
            {
                Player.Spellbook.CastSpell(_ignite, target);
            }
        }

        private static HitChance GetHitchance()
        {
            switch (ElCorkiMenu._menu.Item("ElCorki.hitChance").GetValue<StringList>().SelectedIndex)
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
                    return HitChance.Medium;
            }
        }

        private static void Harass(Obj_AI_Base target)
        {
            if (target == null || !target.LSIsValidTarget())
            {
                return;
            }

            var harassQ = ElCorkiMenu._menu.Item("ElCorki.Harass.Q").IsActive();
            var harassE = ElCorkiMenu._menu.Item("ElCorki.Harass.E").IsActive();
            var harassR = ElCorkiMenu._menu.Item("ElCorki.Harass.R").IsActive();
            var minmana = ElCorkiMenu._menu.Item("ElCorki.harass.mana2").GetValue<Slider>().Value;
            var rStacks = ElCorkiMenu._menu.Item("ElCorki.Harass.RStacks").GetValue<Slider>().Value;

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            if (harassQ && spells[Spells.Q].LSIsReady())
            {
                spells[Spells.Q].Cast(target);
            }

            if (harassE && spells[Spells.E].LSIsReady())
            {
                spells[Spells.E].CastOnBestTarget();
            }

            if (harassR && spells[Spells.R1].LSIsReady()
                && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo > rStacks)
            {
                spells[Spells.R1].CastIfHitchanceEquals(target, CustomHitChance, true);
            }
        }

        private static float IgniteDamage(Obj_AI_Base target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Items(Obj_AI_Base target)
        {
            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            var useYoumuu = ElCorkiMenu._menu.Item("ElCorki.Items.Youmuu").IsActive();
            var useCutlass = ElCorkiMenu._menu.Item("ElCorki.Items.Cutlass").IsActive();
            var useBlade = ElCorkiMenu._menu.Item("ElCorki.Items.Blade").IsActive();

            var useBladeEhp = ElCorkiMenu._menu.Item("ElCorki.Items.Blade.EnemyEHP").GetValue<Slider>().Value;
            var useBladeMhp = ElCorkiMenu._menu.Item("ElCorki.Items.Blade.EnemyMHP").GetValue<Slider>().Value;

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && useBlade)
            {
                botrk.Cast(target);
            }

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
                && Player.HealthPercent <= useBladeMhp && useBlade)
            {
                botrk.Cast(target);
            }

            if (cutlass.IsReady() && cutlass.IsOwned(Player) && cutlass.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && useCutlass)
            {
                cutlass.Cast(target);
            }

            if (ghost.IsReady() && ghost.IsOwned(Player) && target.LSIsValidTarget(spells[Spells.Q].Range) && useYoumuu)
            {
                ghost.Cast();
            }
        }

        private static void JungleClear()
        {
            var useQ = ElCorkiMenu._menu.Item("useQFarmJungle").IsActive();
            var useE = ElCorkiMenu._menu.Item("useEFarmJungle").IsActive();
            var useR = ElCorkiMenu._menu.Item("useRFarmJungle").IsActive();
            var minmana = ElCorkiMenu._menu.Item("minmanaclear").GetValue<Slider>().Value;
            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                700,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            foreach (var minion in minions)
            {
                if (spells[Spells.Q].LSIsReady() && useQ)
                {
                    spells[Spells.Q].Cast(minion);
                }

                if (spells[Spells.E].LSIsReady() && useE)
                {
                    spells[Spells.E].Cast(minion);
                }

                if (spells[Spells.R1].LSIsReady() && useR)
                {
                    spells[Spells.R1].Cast(minion);
                }
            }
        }

        private static void JungleStealMode()
        {
            var useJsm = ElCorkiMenu._menu.Item("ElCorki.misc.junglesteal").IsActive();

            if (!useJsm)
            {
                return;
            }

            var jMob =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    spells[Spells.R1].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(x => x.Health + (x.HPRegenRate / 2) <= spells[Spells.R1].GetDamage(x));

            if (spells[Spells.R1].CanCast(jMob))
            {
                spells[Spells.R1].Cast(jMob);
            }

            var minion =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    spells[Spells.R1].Range,
                    MinionTypes.All,
                    MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth)
                    .FirstOrDefault(
                        x =>
                        x.Health <= spells[Spells.E].GetDamage(x)
                        && (x.BaseSkinName.ToLower().Contains("siege") || x.BaseSkinName.ToLower().Contains("super")));

            if (spells[Spells.R1].LSIsReady() && spells[Spells.R1].CanCast(minion))
            {
                spells[Spells.R1].Cast(minion);
            }
        }

        private static void KsMode()
        {
            var useKs = ElCorkiMenu._menu.Item("ElCorki.misc.ks").IsActive();
            if (!useKs)
            {
                return;
            }

            var target =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                    !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield)
                    && spells[Spells.R1].CanCast(x)
                    && (x.Health + (x.HPRegenRate / 2)) <= spells[Spells.R1].GetDamage(x));

            if (spells[Spells.R1].LSIsReady() && spells[Spells.R1].CanCast(target))
            {
                spells[Spells.R1].Cast(target);
            }
        }

        private static void LaneClear()
        {
            var useQ = ElCorkiMenu._menu.Item("useQFarm").IsActive();
            var useE = ElCorkiMenu._menu.Item("useEFarm").IsActive();
            var useR = ElCorkiMenu._menu.Item("useRFarm").IsActive();
            var countMinions = ElCorkiMenu._menu.Item("ElCorki.Count.Minions").GetValue<Slider>().Value;
            var countMinionsE = ElCorkiMenu._menu.Item("ElCorki.Count.Minions.E").GetValue<Slider>().Value;
            var countMinionsR = ElCorkiMenu._menu.Item("ElCorki.Count.Minions.R").GetValue<Slider>().Value;
            var minmana = ElCorkiMenu._menu.Item("minmanaclear").GetValue<Slider>().Value;

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);

            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.Q].LSIsReady() && useQ)
            {
                foreach (var minion in minions.Where(x => x.Health <= spells[Spells.Q].GetDamage(x)))
                {
                    var killcount = 0;

                    foreach (var cminion in minions)
                    {
                        if (cminion.Health <= spells[Spells.Q].GetDamage(cminion))
                        {
                            killcount++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (killcount >= countMinions)
                    {
                        spells[Spells.Q].Cast(minion);
                    }
                }
            }

            if (!useE || !spells[Spells.E].LSIsReady())
            {
                return;
            }

            var minionkillcount =
                minions.Count(x => spells[Spells.E].CanCast(x) && x.Health <= spells[Spells.E].GetDamage(x));

            if (minionkillcount >= countMinionsE)
            {
                foreach (var minion in minions.Where(x => x.Health <= spells[Spells.E].GetDamage(x)))
                {
                    spells[Spells.E].Cast();
                }
            }

            if (!useR || !spells[Spells.R1].LSIsReady())
            {
                return;
            }

            var rMinionkillcount =
                minions.Count(x => spells[Spells.R1].CanCast(x) && x.Health <= spells[Spells.R1].GetDamage(x));

            if (rMinionkillcount >= countMinionsR)
            {
                foreach (var minion in minions.Where(x => x.Health <= spells[Spells.R1].GetDamage(x)))
                {
                    spells[Spells.R1].Cast(minion);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(target);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass(target);
                    break;
            }

            KsMode();
            AutoHarassMode();
            JungleStealMode();

            spells[Spells.R1].Range = ObjectManager.Player.HasBuff("corkimissilebarragecounterbig")
                                          ? spells[Spells.R2].Range
                                          : spells[Spells.R1].Range;
        }

        #endregion
    }
}