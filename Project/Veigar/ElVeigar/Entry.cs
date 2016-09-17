using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElVeigar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Collision = LeagueSharp.Common.Collision;
    using Color = System.Drawing.Color;

    internal static class Entry
    {
        #region Static Fields

        public static SpellSlot Ignite;

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 920f) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 900f) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 1050f) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 650f) }
                                                             };

        #endregion

        #region Enums

        internal enum Spells
        {
            Q,

            W,

            E,

            R
        }

        #endregion

        #region Public Properties

        public static AIHeroClient Player => ObjectManager.Player;

        public static string ScriptVersion => typeof(ElVeigar).Assembly.GetName().Version.ToString();

        #endregion

        #region Public Methods and Operators

        public static void CastE(Obj_AI_Base target)
        {
            var pred = Prediction.GetPrediction(target, spells[Spells.E].Delay);
            var castVec = pred.UnitPosition.To2D()
                          - Vector2.Normalize(pred.UnitPosition.To2D() - Player.Position.To2D())
                          * spells[Spells.E].Width;

            if (pred.Hitchance >= HitChance.VeryHigh && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(castVec);
            }
        }

        public static void CastE(AIHeroClient target)
        {
            var pred = Prediction.GetPrediction(target, spells[Spells.E].Delay);
            var castVec = pred.UnitPosition.To2D()
                          - Vector2.Normalize(pred.UnitPosition.To2D() - Player.Position.To2D())
                          * spells[Spells.E].Width;

            if (pred.Hitchance >= HitChance.VeryHigh && spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(castVec);
            }
        }

        public static void CastE(Vector3 pos)
        {
            var castVec = pos.To2D() - Vector2.Normalize(pos.To2D() - Player.Position.To2D()) * spells[Spells.E].Width;

            if (spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(castVec);
            }
        }

        public static void CastE(Vector2 pos)
        {
            var castVec = pos;

            if (spells[Spells.E].IsReady())
            {
                spells[Spells.E].Cast(castVec);
            }
        }

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

            if (spells[Spells.R].IsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }

        public static void OnLoad(EventArgs args)
        {
            try
            {
                if (ObjectManager.Player.ChampionName != "Veigar")
                {
                    return;
                }

                Ignite = Player.GetSpellSlot("summonerdot");
                MenuInit.Initialize();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                Interrupter2.OnInterruptableTarget += Interrupter_OnPosibleToInterrupt;

                spells[Spells.Q].SetSkillshot(0.25f, 70f, 2000f, true, SkillshotType.SkillshotLine);
                spells[Spells.W].SetSkillshot(1.35f, 225f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                spells[Spells.E].SetSkillshot(.8f, 350f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                spells[Spells.R].SetTargetted(0.25f, 1400);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private static void AutoW()
        {
            if (MenuInit.IsActive("ElVeigar.Combo.W.Stun"))
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            h =>
                            h.IsValidTarget(spells[Spells.W].Range) && h.GetStunDuration() >= spells[Spells.W].Delay);

                if (target.IsValidTarget() && target != null)
                {
                    spells[Spells.W].Cast(target.Position);
                }
            }
        }

        private static void DoCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range + 200, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget())
            {
                return;
            }


            if (spells[Spells.R].IsReady() && target.IsValidTarget(spells[Spells.R].Range) && MenuInit.IsActive("ElVeigar.Combo.R"))
            {
                if (spells[Spells.R].GetDamage(target) > target.Health)
                {
                    if (!IsInvulnerable(target))
                    {
                        spells[Spells.R].CastOnUnit(target);
                    }
                }
            }

            if (spells[Spells.E].IsReady() && MenuInit.IsActive("ElVeigar.Combo.E"))
            {
                if (Player.Distance(target.Position) <= spells[Spells.E].Range + 200)
                {
                    var predE = spells[Spells.E].GetPrediction(target);
                    if (predE.Hitchance == HitChance.VeryHigh)
                    {
                        if (!IsInvulnerable(target))
                        {
                            CastE(target);
                        }
                    }
                }
            }

            if (spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target)
                && MenuInit.IsActive("ElVeigar.Combo.Q"))
            {
                var pred = spells[Spells.Q].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh && pred.CollisionObjects.Count == 0)
                {
                    spells[Spells.Q].Cast(pred.CastPosition);
                }
            }

            var predictionW = spells[Spells.W].GetPrediction(target);

            if (spells[Spells.W].IsReady() && Player.Distance(target.Position) <= spells[Spells.W].Range - 80f
                && MenuInit.IsActive("ElVeigar.Combo.W"))
            {
                if (predictionW.Hitchance >= HitChance.VeryHigh)
                {
                    spells[Spells.W].Cast(predictionW.CastPosition);
                }
            }

            if (MenuInit.IsActive("ElVeigar.Combo.Use.Ignite") && Player.Distance(target) <= 600
                && IgniteDamage(target) >= target.Health)
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private static void DoHarass()
        {
            try
            {
                var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Magical);
                if (target == null || !target.IsValidTarget())
                {
                    return;
                }

                if (Player.ManaPercent < MenuInit.Menu.Item("ElVeigar.Harass.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                switch (MenuInit.IsListActive("Harass.Mode").SelectedIndex)
                {
                    case 0: // E - Q - W
                        if (spells[Spells.E].IsReady() && Player.Distance(target.Position) <= spells[Spells.E].Range)
                        {
                            var predE = spells[Spells.E].GetPrediction(target);
                            if (predE.Hitchance == HitChance.VeryHigh)
                            {
                                if (!IsInvulnerable(target))
                                {
                                    CastE(target);
                                }
                            }
                        }

                        if (spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
                        {
                            var prediction = spells[Spells.Q].GetPrediction(target);
                            if (prediction.Hitchance >= HitChance.High && prediction.CollisionObjects.Count == 0)
                            {
                                spells[Spells.Q].Cast(prediction.CastPosition);
                            }
                        }
                        break;

                    case 1: //E - W
                        if (spells[Spells.E].IsReady() && Player.Distance(target.Position) <= spells[Spells.E].Range)
                        {
                            var predE = spells[Spells.E].GetPrediction(target);
                            if (predE.Hitchance == HitChance.VeryHigh)
                            {
                                if (!IsInvulnerable(target))
                                {
                                    CastE(target);
                                }
                            }
                        }

                        break;

                    case 2: // Q

                        if (spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(target))
                        {
                            var predictionQ = spells[Spells.Q].GetPrediction(target);
                            if (predictionQ.Hitchance >= HitChance.High && predictionQ.CollisionObjects.Count == 0)
                            {
                                spells[Spells.Q].Cast(predictionQ.CastPosition);
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void DoJungleClear()
        {
            try
            {
                if (Player.ManaPercent < MenuInit.Menu.Item("ElVeigar.JungleClear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                var jungleWMinions = MinionManager.GetMinions(
                    Player.Position,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                var minionerinos2 = (from minions in jungleWMinions select minions.Position.To2D()).ToList();

                var minionPosition =
                    MinionManager.GetBestCircularFarmLocation(
                        minionerinos2,
                        spells[Spells.W].Width,
                        spells[Spells.W].Range).Position;

                if (minionPosition.Distance(Player.Position.To2D()) < spells[Spells.W].Range
                    && MinionManager.GetBestCircularFarmLocation(
                        minionerinos2,
                        spells[Spells.W].Width,
                        spells[Spells.W].Range).MinionsHit > 0)
                {
                    spells[Spells.W].Cast(minionPosition);
                }

                if (MenuInit.IsActive("ElVeigar.jungleclearMenu.Q"))
                {
                    if (!Player.Spellbook.IsAutoAttacking)
                    {
                        spells[Spells.Q].Cast(minionPosition);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void DoLaneClear()
        {
            try
            {
                if (Player.ManaPercent < MenuInit.Menu.Item("ElVeigar.LaneClear.Mana").GetValue<Slider>().Value)
                {
                    return;
                }

                var minions =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(spells[Spells.W].Range).Select(x => x.ServerPosition.To2D()).ToList(),
                        spells[Spells.W].Width,
                        spells[Spells.W].Range);

                if (MenuInit.IsActive("ElVeigar.LaneClear.W")
                    && minions.MinionsHit >= MenuInit.Menu.Item("ElVeigar.LaneClear.W.Minions").GetValue<Slider>().Value)
                {
                    spells[Spells.W].Cast(minions.Position);
                }

                if (MenuInit.IsActive("ElVeigar.LaneClear.Q"))
                {
                    QStack();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static float GetStunDuration(this Obj_AI_Base target)
        {
            return
                target.Buffs.Where(
                    b =>
                    b.IsActive && Game.Time < b.EndTime
                    && (b.Type == BuffType.Charm || b.Type == BuffType.Knockback || b.Type == BuffType.Stun
                        || b.Type == BuffType.Suppression || b.Type == BuffType.Snare))
                    .Aggregate(0f, (current, buff) => Math.Max(current, buff.EndTime)) - Game.Time;
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Interrupter_OnPosibleToInterrupt(
            AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!MenuInit.IsActive("Misc.Interrupt"))
            {
                return;
            }

            if (Player.Distance(unit.Position) < spells[Spells.E].Range && spells[Spells.E].IsReady())
            {
                if (!IsInvulnerable(unit))
                {
                    CastE(unit);
                }
            }
        }

        private static bool IsInvulnerable(this Obj_AI_Base target)
        {
            return Helpers.IgnoreList.Any(buff => target.HasBuff(buff.DisplayName) || target.HasBuff(buff.Name));
        }

        private static void KillstealHandler()
        {
            if (MenuInit.IsActive("ElVeigar.Combo.KS.R"))
            {
                var target =
                    HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(spells[Spells.R].Range) && spells[Spells.R].GetDamage(x) > x.Health);

                if (target != null)
                {
                    var getEnemies = MenuInit.Menu.Item("ElVeigar.KS.R.On" + target.CharData.BaseSkinName);
                    if (getEnemies != null && getEnemies.GetValue<bool>())
                    {
                        spells[Spells.R].CastOnUnit(target);
                    }
                }
            }

            if (MenuInit.IsActive("ElVeigar.Combo.KS.Q"))
            {
                var target =
                    HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(spells[Spells.Q].Range) && spells[Spells.Q].GetDamage(x) > x.Health);

                if (target != null)
                {
                    var predictionQ = spells[Spells.Q].GetPrediction(target);
                    if (predictionQ.Hitchance >= HitChance.High && predictionQ.CollisionObjects.Count == 0)
                    {
                        spells[Spells.Q].Cast(predictionQ.CastPosition);
                    }
                }
            }

            if (MenuInit.IsActive("ElVeigar.Combo.KS.W"))
            {
                var target =
                    HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(spells[Spells.W].Range) && spells[Spells.W].GetDamage(x) > x.Health);

                var predictionW = spells[Spells.W].GetPrediction(target);
                if (target != null)
                {
                    if (predictionW.Hitchance >= HitChance.High)
                    {
                        spells[Spells.W].Cast(predictionW.CastPosition);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (MenuInit.IsActive("Misc.Drawings.Off"))
            {
                return;
            }

            if (MenuInit.Menu.Item("Misc.Drawings.Q").GetValue<Circle>().Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (MenuInit.Menu.Item("Misc.Drawings.W").GetValue<Circle>().Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (MenuInit.Menu.Item("Misc.Drawings.E").GetValue<Circle>().Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (MenuInit.Menu.Item("Misc.Drawings.R").GetValue<Circle>().Active)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
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
                    DoCombo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    DoLaneClear();
                    DoJungleClear();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    DoHarass();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    QStack();
                    break;
            }

            KillstealHandler();
            AutoW();

            if (MenuInit.Menu.Item("ElVeigar.Combo.Multi").IsActive())
            {
                var target = spells[Spells.W].GetTarget();
                var pred = spells[Spells.W].GetPrediction(target);
                if (pred.AoeTargetsHitCount >= 2 && spells[Spells.W].IsReady())
                {
                    spells[Spells.W].Cast(pred.CastPosition);
                }
            }

            if (MenuInit.Menu.Item("ElVeigar.Stack.Q").GetValue<KeyBind>().Active)
            {
                QStack();
            }  
        }

        private static List<Obj_AI_Base> QGetCollisionMinions(AIHeroClient source, Vector3 targetposition)
        {
            var input = new PredictionInput
                            {
                                Unit = source, Radius = spells[Spells.Q].Width, Delay = spells[Spells.Q].Delay,
                                Speed = spells[Spells.Q].Speed
                            };

            input.CollisionObjects[0] = CollisionableObjects.Minions;

            return
                Collision.GetCollision(new List<Vector3> { targetposition }, input)
                    .OrderBy(obj => obj.Distance(source))
                    .ToList();
        }

        private static void QStack()
        {
            if (Player.IsRecalling() || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Player.ManaPercent < MenuInit.Menu.Item("ElVeigar.Stack.Mana").GetValue<Slider>().Value)
            {
                return;
            }

            var minions =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        m =>
                        m.IsMinion && !m.Name.Contains("ward") && m.IsEnemy && Player.Distance(m.Position) <= spells[Spells.Q].Range
                        && m.Health < ((Player.GetSpellDamage(m, SpellSlot.Q)))
                        && HealthPrediction.GetHealthPrediction(
                            m,
                            (int)(Player.Distance(m) / spells[Spells.Q].Speed),
                            (int)(spells[Spells.Q].Delay * 1000 + Game.Ping / 2)) > 0);

            foreach (var minion in minions.Where(x => x.Health <= spells[Spells.Q].GetDamage(x)))
            {
                var killcount = 0;

                foreach (var colminion in
                    QGetCollisionMinions(
                        Player,
                        Player.ServerPosition.Extend(minion.ServerPosition, spells[Spells.Q].Range)))
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

                if (killcount >= MenuInit.Menu.Item("ElVeigar.Stack.Q2").GetValue<Slider>().Value)
                {
                    if (!Player.Spellbook.IsAutoAttacking)
                    {
                        spells[Spells.Q].Cast(minion.ServerPosition);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}