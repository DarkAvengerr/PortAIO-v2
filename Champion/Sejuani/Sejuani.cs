namespace ElSejuani
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

    internal static class Sejuani
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>()
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 650) },
                                                                 { Spells.W, new Spell(SpellSlot.W,  Orbwalking.GetRealAutoAttackRange(Player) + 100) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 1000) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 1175) }
                                                             };

        private static SpellSlot _ignite;

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

        public static BuffInstance GetFrost(Obj_AI_Base target)
        {
            return target.Buffs.FirstOrDefault(buff => buff.Name == "sejuanifrost");
        }

        public static void OnLoad()
        {
            if (Player.CharData.BaseSkinName != "Sejuani")
            {
                return;
            }

            Console.WriteLine("Injected");
            Notifications.AddNotification("ElSejuani by jQuery v1.0.0.0", 1000);

            spells[Spells.Q].SetSkillshot(0, 70, 1600, false, SkillshotType.SkillshotLine);
            spells[Spells.R].SetSkillshot(250, 110, 1600, false, SkillshotType.SkillshotLine);

            _ignite = Player.GetSpellSlot("summonerdot");

            ElSejuaniMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawings.OnDraw;

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        #endregion

        #region Methods

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

            var useQ = ElSejuaniMenu.Menu.Item("ElSejuani.Interupt.Q").IsActive();
            var useR = ElSejuaniMenu.Menu.Item("ElSejuani.Interupt.R").IsActive();

            if (gapcloser.Sender.IsValidTarget(spells[Spells.Q].Range))
            {
                if (useQ && spells[Spells.Q].IsReady())
                {
                    spells[Spells.Q].Cast(gapcloser.Sender);
                }

                if (useR && !spells[Spells.Q].IsReady() && spells[Spells.R].IsReady())
                {
                    spells[Spells.R].Cast(gapcloser.Sender);
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var comboQ = ElSejuaniMenu.Menu.Item("ElSejuani.Combo.Q").IsActive();
            var comboE = ElSejuaniMenu.Menu.Item("ElSejuani.Combo.E").IsActive();
            var comboW = ElSejuaniMenu.Menu.Item("ElSejuani.Combo.E").IsActive();
            var comboR = ElSejuaniMenu.Menu.Item("ElSejuani.Combo.R").IsActive();
            var countEnemyR = ElSejuaniMenu.Menu.Item("ElSejuani.Combo.R.Count").GetValue<Slider>().Value;
            var countEnemyE = ElSejuaniMenu.Menu.Item("ElSejuani.Combo.E.Count").GetValue<Slider>().Value;

            if (comboQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (comboW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (comboE && spells[Spells.E].IsReady() && IsFrozen(target) && target.IsValidTarget(spells[Spells.E].Range))
            {
                if (IsFrozen(target))
                {
                    spells[Spells.E].Cast();
                }

                if (IsFrozen(target)
                    && target.ServerPosition.Distance(Player.ServerPosition, true) <= spells[Spells.E].Range)
                {
                    spells[Spells.E].Cast();
                }
            }
            if (countEnemyR == 1)
            {
                if (comboR && spells[Spells.R].IsReady())
                {
                    var targ = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Physical);
                    var predA = spells[Spells.R].GetPrediction(targ);
                    if (targ != null && targ.IsValidTarget())
                    {
                        spells[Spells.R].Cast(predA.CastPosition);
                    }
                }
            }
            else
            {
                if (comboR && spells[Spells.R].IsReady())
                {
                    foreach (
                        var x in
                            HeroManager.Enemies.Where((hero => !hero.IsDead && hero.IsValidTarget(spells[Spells.R].Range))))
                    {
                        var pred = spells[Spells.R].GetPrediction(x);
                        if (pred.AoeTargetsHitCount >= countEnemyR)
                        {
                            spells[Spells.R].Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        private static HitChance GetHitchance()
        {
            switch (ElSejuaniMenu.Menu.Item("ElSejuani.hitChance").GetValue<StringList>().SelectedIndex)
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

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var harassQ = ElSejuaniMenu.Menu.Item("ElSejuani.Harass.Q").IsActive();
            var harassW = ElSejuaniMenu.Menu.Item("ElSejuani.Harass.W").IsActive();
            var harassE = ElSejuaniMenu.Menu.Item("ElSejuani.Harass.E").IsActive();
            var minmana = ElSejuaniMenu.Menu.Item("ElSejuani.harass.mana").GetValue<Slider>().Value;

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            if (harassQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (harassW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }

            if (harassE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                if (IsFrozen(target) && spells[Spells.E].GetDamage(target) > target.Health)
                {
                    spells[Spells.E].Cast();
                }

                if (IsFrozen(target)
                    && target.ServerPosition.Distance(Player.ServerPosition, true)
                    < Math.Pow(spells[Spells.E].Range * 0.8, 2))
                {
                    spells[Spells.E].Cast();
                }
            }
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

        private static bool IsFrozen(Obj_AI_Base target)
        {
            return target.HasBuff("SejuaniFrost");
        }

        private static void JungleClear()
        {
            var clearQ = ElSejuaniMenu.Menu.Item("ElSejuani.Clear.Q").IsActive();
            var clearW = ElSejuaniMenu.Menu.Item("ElSejuani.Clear.Q").IsActive();
            var clearE = ElSejuaniMenu.Menu.Item("ElSejuani.Clear.E").IsActive();
            var minmana = ElSejuaniMenu.Menu.Item("minmanaclear").GetValue<Slider>().Value;
            var minQ = ElSejuaniMenu.Menu.Item("ElSejuani.Clear.Q.Count").GetValue<Slider>().Value;

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.W].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minions.Count <= 0)
            {
                return;
            }

            foreach (var minion in minions)
            {
                if (spells[Spells.Q].IsReady() && clearQ)
                {
                    if (spells[Spells.Q].GetLineFarmLocation(minions).MinionsHit >= minQ)
                    {
                        spells[Spells.Q].Cast(spells[Spells.Q].GetLineFarmLocation(minions).Position);
                    }
                }

                if (spells[Spells.W].IsReady() && clearW
                    && minion.ServerPosition.Distance(Player.ServerPosition, true) <= spells[Spells.W].Range)
                {
                    spells[Spells.W].Cast();
                }

                if (spells[Spells.E].IsReady() && clearE
                    && minions[0].Health + (minions[0].HPRegenRate / 2) <= spells[Spells.E].GetDamage(minion)
                    && minion.HasBuff("sejuanifrost"))
                {
                    spells[Spells.E].Cast();
                }
            }
        }

        private static void LaneClear()
        {
            var clearQ = ElSejuaniMenu.Menu.Item("ElSejuani.Clear.Q").IsActive();
            var clearW = ElSejuaniMenu.Menu.Item("ElSejuani.Clear.Q").IsActive();
            var clearE = ElSejuaniMenu.Menu.Item("ElSejuani.Clear.E").IsActive();
            var minmana = ElSejuaniMenu.Menu.Item("minmanaclear").GetValue<Slider>().Value;
            var minQ = ElSejuaniMenu.Menu.Item("ElSejuani.Clear.Q.Count").GetValue<Slider>().Value;

            if (Player.ManaPercent < minmana)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.Q].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            foreach (var minion in minions)
            {
                if (spells[Spells.Q].IsReady() && clearQ)
                {
                    if (spells[Spells.Q].GetLineFarmLocation(minions).MinionsHit >= minQ)
                    {
                        spells[Spells.Q].Cast(spells[Spells.Q].GetLineFarmLocation(minions).Position);
                    }
                }

                if (spells[Spells.W].IsReady() && clearW
                    && minion.ServerPosition.Distance(Player.ServerPosition, true) >= spells[Spells.W].Range)
                {
                    spells[Spells.W].Cast();
                }

                if (spells[Spells.E].IsReady() && clearE
                    && minions[0].Health + (minions[0].HPRegenRate / 2) <= spells[Spells.E].GetDamage(minion)
                    && minion.HasBuff("sejuanifrost"))
                {
                    spells[Spells.E].Cast();
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

            if (ElSejuaniMenu.Menu.Item("ElSejuani.Combo.Semi.R").GetValue<KeyBind>().Active)
            {
                SemiR();
            }
        }


        private static void SemiR()
        {
            var target = TargetSelector.GetTarget(spells[Spells.R].Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (!spells[Spells.R].IsReady() || !target.IsValidTarget(spells[Spells.R].Range))
            {
                return;
            }

            spells[Spells.R].Cast(target);
        }


        #endregion
    }
}