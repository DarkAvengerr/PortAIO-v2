namespace Elvarus
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

        R
    }

    internal class Varus
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 925) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 0) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 925) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 1100) }
                                                             };

        #endregion

        #region Public Properties

        public static AIHeroClient Player
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
            if (ObjectManager.Player.ChampionName != "Varus")
            {
                return;
            }

            spells[Spells.Q].SetSkillshot(.25f, 70f, 1650f, false, SkillshotType.SkillshotLine);
            spells[Spells.E].SetSkillshot(0.35f, 120, 1500, false, SkillshotType.SkillshotCircle);
            spells[Spells.R].SetSkillshot(.25f, 120f, 1950f, false, SkillshotType.SkillshotLine);

            spells[Spells.Q].SetCharged(250, 1600, 1.2f);

            ElVarusMenu.Initialize();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
        }

        #endregion

        #region Methods

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(
                (spells[Spells.Q].ChargedMaxRange + spells[Spells.Q].Width) * 1.1f,
                TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            Items(target);

            if (spells[Spells.E].IsReady() && !spells[Spells.Q].IsCharging
                && ElVarusMenu.Menu.Item("ElVarus.Combo.E").IsActive())
            {
                if (spells[Spells.E].IsKillable(target) || GetWStacks(target) >= 1)
                {
                    var prediction = spells[Spells.E].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        spells[Spells.E].Cast(prediction.CastPosition);
                    }
                }
            }

            if (spells[Spells.Q].IsReady() && ElVarusMenu.Menu.Item("ElVarus.Combo.Q").IsActive())
            {
                if (spells[Spells.Q].IsCharging || ElVarusMenu.Menu.Item("ElVarus.combo.always.Q").IsActive()
                    || target.Distance(Player) > Orbwalking.GetRealAutoAttackRange(target) * 1.2f
                    || GetWStacks(target) >= ElVarusMenu.Menu.Item("ElVarus.Combo.Stack.Count").GetValue<Slider>().Value
                    || spells[Spells.Q].IsKillable(target))
                {
                    if (!spells[Spells.Q].IsCharging)
                    {
                        spells[Spells.Q].StartCharging();
                    }

                    if (spells[Spells.Q].IsCharging)
                    {
                        var prediction = spells[Spells.Q].GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.VeryHigh)
                        {
                            spells[Spells.Q].Cast(prediction.CastPosition);
                        }
                    }
                }
            }

            if (spells[Spells.R].IsReady() && !spells[Spells.Q].IsCharging
                && target.IsValidTarget(spells[Spells.R].Range) && ElVarusMenu.Menu.Item("ElVarus.Combo.R").IsActive())
            {
                var pred = spells[Spells.R].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    var ultimateHits = HeroManager.Enemies.Where(x => x.Distance(target) <= 450f).ToList();
                    if (ultimateHits.Count >= ElVarusMenu.Menu.Item("ElVarus.Combo.R.Count").GetValue<Slider>().Value)
                    {
                        spells[Spells.R].Cast(pred.CastPosition);
                    }
                }
            }
        }

        private static int GetWStacks(Obj_AI_Base target)
        {
            return target.GetBuffCount("varuswdebuff");
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].ChargedMaxRange, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (Player.ManaPercent > ElVarusMenu.Menu.Item("minmanaharass").GetValue<Slider>().Value)
            {
                if (ElVarusMenu.Menu.Item("ElVarus.Harass.E").IsActive() && spells[Spells.E].IsReady() && GetWStacks(target) >= 1)
                {
                    var prediction = spells[Spells.E].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        spells[Spells.E].Cast(prediction.CastPosition);
                    }
                }

                if (ElVarusMenu.Menu.Item("ElVarus.Harass.Q").IsActive() && spells[Spells.Q].IsReady())
                {
                    if (!spells[Spells.Q].IsCharging)
                    {
                        spells[Spells.Q].StartCharging();
                    }

                    if (spells[Spells.Q].IsCharging)
                    {
                        var prediction = spells[Spells.Q].GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.VeryHigh)
                        {
                            spells[Spells.Q].Cast(prediction.CastPosition);
                        }
                    }
                }
            }
        }

        private static void Items(Obj_AI_Base target)
        {
            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            var useYoumuu = ElVarusMenu.Menu.Item("ElVarus.Items.Youmuu").IsActive();
            var useCutlass = ElVarusMenu.Menu.Item("ElVarus.Items.Cutlass").IsActive();
            var useBlade = ElVarusMenu.Menu.Item("ElVarus.Items.Blade").IsActive();

            var useBladeEhp = ElVarusMenu.Menu.Item("ElVarus.Items.Blade.EnemyEHP").GetValue<Slider>().Value;
            var useBladeMhp = ElVarusMenu.Menu.Item("ElVarus.Items.Blade.EnemyMHP").GetValue<Slider>().Value;

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

            if (ghost.IsReady() && ghost.IsOwned(Player) && target.IsValidTarget(spells[Spells.Q].Range) && useYoumuu)
            {
                ghost.Cast();
            }
        }

        private static void JungleClear()
        {
            var useQ = ElVarusMenu.Menu.Item("useQFarmJungle").IsActive();
            var useE = ElVarusMenu.Menu.Item("useEFarmJungle").IsActive();
            var minmana = ElVarusMenu.Menu.Item("minmanaclear").GetValue<Slider>().Value;
            var minions = MinionManager.GetMinions(
                Player.ServerPosition,
                700,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (Player.ManaPercent >= minmana)
            {
                foreach (var minion in minions)
                {
                    if (spells[Spells.Q].IsReady() && useQ)
                    {
                        if (!spells[Spells.Q].IsCharging)
                        {
                            spells[Spells.Q].StartCharging();
                        }

                        if (spells[Spells.Q].IsCharging && spells[Spells.Q].Range >= spells[Spells.Q].ChargedMaxRange)
                        {
                            spells[Spells.Q].Cast(minion);
                        }
                    }

                    if (spells[Spells.E].IsReady() && useE)
                    {
                        spells[Spells.E].CastOnUnit(minion);
                    }
                }
            }
        }

        //Credits to God :cat_lazy:
        private static void Killsteal()
        {
            if (ElVarusMenu.Menu.Item("ElVarus.KSSS").IsActive() && spells[Spells.Q].IsReady())
            {
                foreach (var target in
                    HeroManager.Enemies.Where(
                        enemy =>
                        enemy.IsValidTarget() && spells[Spells.Q].IsKillable(enemy)
                        && Player.Distance(enemy.Position) <= spells[Spells.Q].ChargedMaxRange))
                {
                    if (!spells[Spells.Q].IsCharging)
                    {
                        spells[Spells.Q].StartCharging();
                    }

                    if (spells[Spells.Q].IsCharging)
                    {
                        var prediction = spells[Spells.Q].GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.VeryHigh)
                        {
                            spells[Spells.Q].Cast(prediction.CastPosition);
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (Player.ManaPercent < ElVarusMenu.Menu.Item("minmanaclear").GetValue<Slider>().Value)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && ElVarusMenu.Menu.Item("useQFarm").IsActive())
            {
                var allMinions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
                {
                    foreach (var minion in
                        allMinions.Where(minion => minion.Health <= Player.GetSpellDamage(minion, SpellSlot.Q)))
                    {
                        var killcount = 0;

                        foreach (var colminion in minions)
                        {
                            if (colminion.Health <= spells[Spells.Q].GetDamage(colminion))
                            {
                                killcount++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (killcount >= ElVarusMenu.Menu.Item("ElVarus.Count.Minions").GetValue<Slider>().Value)
                        {
                            if (minion.IsValidTarget())
                            {
                                spells[Spells.Q].Cast(minion);
                                return;
                            }
                        }
                    }
                }
            }

            if (!ElVarusMenu.Menu.Item("useQFarm").IsActive() || !spells[Spells.E].IsReady())
            {
                return;
            }

            var minionkillcount =
                minions.Count(x => spells[Spells.E].CanCast(x) && x.Health <= spells[Spells.E].GetDamage(x));

            if (minionkillcount >= ElVarusMenu.Menu.Item("ElVarus.Count.Minions.E").GetValue<Slider>().Value)
            {
                foreach (var minion in minions.Where(x => x.Health <= spells[Spells.E].GetDamage(x)))
                {
                    spells[Spells.E].Cast(minion);
                }
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
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

            Killsteal();

            var target = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Physical);
            if (spells[Spells.R].IsReady() && target.IsValidTarget()
                && ElVarusMenu.Menu.Item("ElVarus.SemiR").GetValue<KeyBind>().Active)
            {
                spells[Spells.R].CastOnUnit(target);
            }
        }

        #endregion
    }
}