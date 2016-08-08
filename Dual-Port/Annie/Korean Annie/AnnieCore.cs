using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAnnie
{
    using System.Collections.Generic;
    using System.Linq;

    using KoreanAnnie.Common;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class AnnieCore : CommonOrbwalkImplementation
    {
        #region Fields

        private readonly Annie annie;

        private readonly CommonSpells spells;

        #endregion

        #region Constructors and Destructors

        public AnnieCore(Annie annie)
        {
            Champion = annie;
            this.annie = annie;
            spells = annie.Spells;

            Game.OnUpdate += UseSkills;
            Orbwalking.BeforeAttack += DisableAAToFarm;
        }

        #endregion

        #region Public Methods and Operators

        private bool CanAttack(AIHeroClient enemy)
        {
            if (enemy == null)
            {
                return false;
            }
            if (annie.GetParamBool("combo" + enemy.ChampionName.ToLowerInvariant()))
            {
                return true;
            }
            else if (enemy.HealthPercent < 30)
            {
                return true;
            }
            else if (annie.Player.LSGetEnemiesInRange(1100f).Count == 1)
            {
                return true;
            }
            else
            {
                return
                    !HeroManager.Enemies.Where(x => x.LSDistance(Champion.Player) < 1100f && !x.IsDead && !x.IsZombie)
                        .Any(
                            x =>
                                x.ChampionName != enemy.ChampionName &&
                                annie.GetParamBool("combo" + x.ChampionName.ToLowerInvariant()));
            }
        }

        public override void ComboMode()
        {
            AIHeroClient target = TargetSelector.GetTarget(spells.MaxRangeCombo, TargetSelector.DamageType.Magical);

            if ((target == null) || (!CanAttack(target)))
            {
                return;
            }

            if (annie.GetParamBool("usertocombo") && spells.R.LSIsReady() && spells.R.CanCast()
                && target.LSIsValidTarget(spells.R.Range) && !spells.CheckOverkill(target))
            {
                int minEnemiesToR = annie.GetParamSlider("minenemiestor");

                if (minEnemiesToR == 1 && annie.CheckStun())
                {
                    spells.R.Cast(target.Position);
                }
                else
                {
                    foreach (PredictionOutput pred in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(x => x.LSIsValidTarget(spells.R.Range))
                            .Select(x => spells.R.GetPrediction(x, true))
                            .Where(pred => pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= minEnemiesToR)
                        )
                    {
                        spells.R.Cast(pred.CastPosition);
                    }
                }
            }
            if (!annie.GetParamBool("supportmode") && spells.R.GetDamage(target) > target.Health + 50f
                && spells.R.LSIsReady() && spells.R.CanCast() && spells.R.CanCast(target) && !spells.CheckOverkill(target))
            {
                spells.R.Cast(target.Position);
            }
            if ((spells.W.LSIsReady()) && (annie.GetParamBool("usewtocombo")) && (target.LSIsValidTarget(spells.W.Range)))
            {
                spells.W.Cast(target.Position);
            }
            if ((spells.Q.LSIsReady()) && (annie.GetParamBool("useqtocombo")) && (target.LSIsValidTarget(spells.Q.Range)))
            {
                spells.Q.Cast(target);
            }
        }

        public override void Ultimate()
        {
            spells.R.CastOnBestTarget();
        }

        #endregion

        #region Methods

        protected override void HarasMode()
        {
            LastHitMode();
            Haras();
        }

        protected override void LaneClearMode()
        {
            if ((!annie.SaveStun()) && (annie.CanFarm()))
            {
                bool manaLimitReached = annie.Player.ManaPercent < annie.GetParamSlider("manalimittolaneclear");

                if ((annie.GetParamBool("useqtolaneclear")) && (spells.Q.LSIsReady()))
                {
                    if (annie.GetParamBool("saveqtofarm"))
                    {
                        QFarmLogic();
                    }
                    else if (!manaLimitReached)
                    {
                        List<Obj_AI_Base> minions = MinionManager.GetMinions(
                            annie.Player.Position,
                            spells.Q.Range,
                            MinionTypes.All,
                            MinionTeam.NotAlly,
                            MinionOrderTypes.MaxHealth);

                        if ((minions != null) && (minions.Count > 0))
                        {
                            spells.Q.Cast(minions[0]);
                        }
                    }
                }

                if (!manaLimitReached)
                {
                    if ((annie.GetParamBool("usewtolaneclear")) && (spells.W.LSIsReady()))
                    {
                        List<Obj_AI_Base> minions = MinionManager.GetMinions(annie.Player.Position, spells.W.Range);

                        MinionManager.FarmLocation wFarmLocation = spells.W.GetCircularFarmLocation(
                            minions,
                            spells.W.Width);

                        if (wFarmLocation.MinionsHit >= annie.GetParamSlider("minminionstow"))
                        {
                            spells.W.Cast(wFarmLocation.Position);
                        }
                    }
                }
            }

            if (annie.GetParamBool("harasonlaneclear"))
            {
                Haras();
            }
        }

        protected override void LastHitMode()
        {
            if (!annie.SaveStun())
            {
                QFarmLogic();
            }
        }

        private void DisableAAToFarm(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is Obj_AI_Minion
                && (Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                    || Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit
                    || Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                && !annie.SaveStun() 
                && annie.CanFarm() 
                && annie.GetParamBool("useqtofarm")
                && annie.Player.Mana > annie.Spells.Q.Instance.SData.Mana
                && Champion.Spells.Q.IsKillable((Obj_AI_Base)args.Target)
                && Champion.Spells.Q.Instance.CooldownExpires - Game.Time < 0.3f)
            {
                args.Process = false;
            }
        }

        private void Haras()
        {
            if (annie.Player.ManaPercent < annie.GetParamSlider("manalimittoharas"))
            {
                return;
            }

            AIHeroClient target = TargetSelector.GetTarget(spells.MaxRangeHaras, TargetSelector.DamageType.Magical);

            if (!CanAttack(target))
            {
                return;
            }

            if ((spells.Q.LSIsReady()) && (annie.GetParamBool("useqtoharas")) && (target.LSIsValidTarget(spells.Q.Range)))
            {
                spells.Q.Cast(target);
            }

            if ((spells.W.LSIsReady()) && (annie.GetParamBool("usewtoharas")) && (target.LSIsValidTarget(spells.W.Range)))
            {
                spells.W.Cast(target.Position);
            }
        }

        private void QFarmLogic()
        {
            if (annie.SaveStun() || !annie.CanFarm() || !spells.Q.LSIsReady() || !annie.GetParamBool("useqtofarm"))
            {
                return;
            }

            spells.Q.Cast(
                MinionManager.GetMinions(
                    annie.Player.Position,
                    spells.Q.Range,
                    MinionTypes.All,
                    MinionTeam.NotAlly,
                    MinionOrderTypes.MaxHealth).Where(x => spells.Q.IsKillable(x)).FirstOrDefault(
                        minion =>
                            spells.Q.GetDamage(minion) >
                            HealthPrediction.GetHealthPrediction(minion,
                                (int) (annie.Player.LSDistance(minion)/spells.Q.Speed)*1000, (int) spells.Q.Delay*1000))
                );
        }

        #endregion
    }
}