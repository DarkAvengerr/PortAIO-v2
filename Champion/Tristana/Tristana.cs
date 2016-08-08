namespace ElTristana
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;

    #region

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    #endregion

    internal static class Tristana
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>()
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 550) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 900) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 625) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 700) },
                                                             };

        #endregion

        #region Public Properties

        public static String ScriptVersion => typeof(Tristana).Assembly.GetName().Version.ToString();

        #endregion

        #region Properties

        private static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets or sets the random.
        /// </summary>
        /// <value>
        ///     The random.
        /// </value>
        private static Random Random { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets or sets the last harass position
        /// </summary>
        /// <value>
        ///     The last harass position
        /// </value>
        private static Vector3? LastHarassPos { get; set; }

        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Tristana")
            {
                return;
            }

            try
            {
                Chat.Print(
                    "[00:00] <font color='#f9eb0b'>HEEEEEEY!</font> Use ElUtilitySuite for optimal results! xo jQuery");

                spells[Spells.W].SetSkillshot(0.35f, 250f, 1400f, false, SkillshotType.SkillshotCircle);

                MenuInit.Initialize();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
                Orbwalking.BeforeAttack += OrbwalkingBeforeAttack;
                Orbwalking.AfterAttack += OrbwalkingAfterAttack;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods


        private static void OrbwalkingAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (!(target is AIHeroClient))
                {
                    return;
                }

                var targetQ = (AIHeroClient)target;
                if (targetQ.LSIsValidTarget() && IsActive("ElTristana.Combo.Q"))
                {
                    spells[Spells.Q].Cast();
                }
            }
        }

        private static void OrbwalkingBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            try
            {
                if (!args.Unit.IsMe)
                {
                    return;
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                    || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (!(args.Target is AIHeroClient))
                    {
                        return;
                    }

                    var target = HeroManager.Enemies.Find(x => x.HasBuff("TristanaECharge") && x.LSIsValidTarget(spells[Spells.E].Range));
                    if (target == null)
                    {
                        return;
                    }
                    if (Orbwalking.InAutoAttackRange(target))
                    {
                        Orbwalker.ForceTarget(target);
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit
                    || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var minion = args.Target as Obj_AI_Minion;
                    if (minion != null && minion.HasBuff("TristanaECharge"))
                    {
                        Orbwalker.ForceTarget(minion);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (IsActive("ElTristana.Antigapcloser"))
            {
                if (spells[Spells.R].LSIsReady())
                {
                    spells[Spells.R].Cast(gapcloser.Sender);
                }
            }
        }

        private static BuffInstance GetECharge(this Obj_AI_Base target)
        {
            return target.Buffs.Find(x => x.DisplayName == "TristanaECharge");
        }


        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!IsActive("ElTristana.Interrupter"))
            {
                return;
            }

            if (sender.LSDistance(Player) <= spells[Spells.R].Range)
            {
                spells[Spells.R].Cast(sender);
            }
        }

        private static bool IsActive(string menuItem)
        {
            return MenuInit.Menu.Item(menuItem).GetValue<bool>();
        }

        private static bool IsECharged(this Obj_AI_Base target)
        {
            return target.GetECharge() != null;
        }

        private static void OnCombo()
        {
            var eTarget =
                HeroManager.Enemies.Find(
                    x => x.HasBuff("TristanaECharge") && x.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)));
            var target = eTarget ?? TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);

            if (!target.LSIsValidTarget())
            {
                return;
            }


            if (spells[Spells.Q].LSIsReady() && IsActive("ElTristana.Combo.Q"))
            {
                QLogic();
            }

            if (IsActive("ElTristana.Combo.Focus.E"))
            {
                var passiveTarget = HeroManager.Enemies.Find(x => x.HasBuff("TristanaECharge") && x.LSIsValidTarget(spells[Spells.E].Range));
                if (passiveTarget != null)
                {
                    Orbwalker.ForceTarget(passiveTarget);
                }
                else
                {
                    Orbwalker.ForceTarget(null);
                }
            }

            if (spells[Spells.E].LSIsReady() && IsActive("ElTristana.Combo.E")
                && Player.ManaPercent > MenuInit.Menu.Item("ElTristana.Combo.E.Mana").GetValue<Slider>().Value)
            {
                foreach (var hero in HeroManager.Enemies.OrderBy(x => x.Health))
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = MenuInit.Menu.Item("ElTristana.E.On" + hero.ChampionName);
                        if (getEnemies != null && getEnemies.GetValue<bool>())
                        {
                            spells[Spells.E].Cast(hero);
                        }

                        if (getEnemies != null && !getEnemies.GetValue<bool>() && Player.LSCountEnemiesInRange(1500) == 1)
                        {
                            spells[Spells.E].Cast(hero);
                        }
                    }
                }
            }

            UseItems(target);

            if (spells[Spells.R].LSIsReady() && IsActive("ElTristana.Combo.R"))
            {
                if (spells[Spells.R].GetDamage(target) > target.Health)
                {
                    spells[Spells.R].Cast(target);
                }
            }

            if (IsECharged(target) && IsActive("ElTristana.Combo.Always.RE"))
            {
                if (spells[Spells.R].GetDamage(target)
                    + spells[Spells.E].GetDamage(target) * ((0.3 * target.GetBuffCount("TristanaECharge") + 1))
                    > target.Health + target.AttackShield + 75)
                {
                    spells[Spells.R].Cast(target);
                }
            }
        }

        private static void QLogic()
        {
            var qRange = Player.AttackRange + Player.BoundingRadius + 60 + 25 * spells[Spells.Q].Level;
            if (HeroManager.Enemies.Any(e => e.LSDistance(Player) < qRange + e.BoundingRadius))
            {
                spells[Spells.Q].Cast();
            }
        }

        private static bool HasEBuff(this Obj_AI_Base target)
        {
            return target.HasBuff("TristanaECharge");
        }

        private static void OnDraw(EventArgs args)
        {

            if (IsActive("ElTristana.Draw.off")) // I moved it up because the method was breaking before it got to the drawings - now the drawings will work.
            {
                return;
            }

            if (MenuInit.Menu.Item("ElTristana.Draw.Q").GetValue<Circle>().Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (MenuInit.Menu.Item("ElTristana.Draw.E").GetValue<Circle>().Active)
            {
                var cir = MenuInit.Menu.Item("ElTristana.Draw.E").GetValue<Circle>();
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, cir.Color);
                }
            }

            if (MenuInit.Menu.Item("ElTristana.Draw.R").GetValue<Circle>().Active)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }

            var target = HeroManager.Enemies.Find(x => x.HasBuff("TristanaECharge") && x.LSIsValidTarget(2000));
            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (IsActive("ElTristana.DrawStacks"))
            {

                if (LastHarassPos == null)
                {
                    LastHarassPos = Player.ServerPosition;
                }

                var x = target.HPBarPosition.X + 45;
                var y = target.HPBarPosition.Y - 25;

                if (spells[Spells.E].Level > 0)
                {
                    if (HasEBuff(target)) //Credits to lizzaran 
                    {
                        int stacks = target.GetBuffCount("TristanaECharge");
                        if (stacks > -1)
                        {
                            for (var i = 0; 4 > i; i++)
                            {
                                Drawing.DrawLine( x + i * 20, y, x + i * 20 + 10, y, 10, i > stacks ? Color.DarkGray : Color.OrangeRed);
                            }
                        }

                        if (stacks == 3)
                        {
                            if (IsActive("ElTristana.W"))
                            {
                                if (IsActive("ElTristana.W.Jump"))
                                {
                                    if (IsActive("ElTristana.W.Jump.kill"))
                                    {
                                        if (target.LSIsValidTarget(spells[Spells.W].Range) && Player.LSCountEnemiesInRange(MenuInit.Menu.Item("ElTristana.W.Enemies.Range").GetValue<Slider>().Value) <= MenuInit.Menu.Item("ElTristana.W.Enemies").GetValue<Slider>().Value)
                                        {
                                            if (IsActive("ElTristana.W.Jump.tower"))
                                            {
                                                bool underTower = target.LSUnderTurret();
                                                if (underTower)
                                                {
                                                    return;
                                                }
                                            }

                                            if (spells[Spells.W].GetDamage(target) > target.Health + 15)
                                            {
                                                var prediction = spells[Spells.W].GetPrediction(target);
                                                if (prediction.Hitchance >= HitChance.High)
                                                {
                                                    spells[Spells.W].Cast(prediction.CastPosition);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (target.LSIsValidTarget(spells[Spells.W].Range) && Player.LSCountEnemiesInRange(MenuInit.Menu.Item("ElTristana.W.Enemies.Range").GetValue<Slider>().Value) <= MenuInit.Menu.Item("ElTristana.W.Enemies").GetValue<Slider>().Value)
                                        {
                                            if (IsActive("ElTristana.W.Jump.tower"))
                                            {
                                                bool underTower = target.LSUnderTurret();
                                                if (underTower)
                                                {
                                                    return;
                                                }
                                            }
                                            spells[Spells.W].Cast(target.ServerPosition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.LSIsValidTarget())
            {
                return;
            }

            if (spells[Spells.E].LSIsReady() && IsActive("ElTristana.Harass.E")
                && Player.ManaPercent > MenuInit.Menu.Item("ElTristana.Harass.E.Mana").GetValue<Slider>().Value)
            {
                foreach (var hero in HeroManager.Enemies.OrderByDescending(x => x.Health))
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = MenuInit.Menu.Item("ElTristana.E.On.Harass" + hero.CharData.BaseSkinName);
                        if (getEnemies != null && getEnemies.GetValue<bool>())
                        {
                            spells[Spells.E].Cast(hero);
                            Orbwalker.ForceTarget(hero);
                        }
                    }
                }
            }

            if (spells[Spells.Q].LSIsReady() && IsActive("ElTristana.Harass.Q")
                && target.LSIsValidTarget(spells[Spells.E].Range))
            {
                if (IsECharged(target) && IsActive("ElTristana.Harass.QE"))
                {
                    spells[Spells.Q].Cast();
                }
                else if (!IsActive("ElTristana.Harass.QE"))
                {
                    spells[Spells.Q].Cast();
                }
            }
        }

        private static void OnJungleClear()
        {
            var minions =
                MinionManager.GetMinions(
                    spells[Spells.Q].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (!minions.LSIsValidTarget() || minions == null)
            {
                return;
            }

            if (spells[Spells.E].LSIsReady() && IsActive("ElTristana.JungleClear.E")
                && Player.ManaPercent > MenuInit.Menu.Item("ElTristana.JungleClear.E.Mana").GetValue<Slider>().Value)
            {
                spells[Spells.E].CastOnUnit(minions);
            }

            if (spells[Spells.Q].LSIsReady() && IsActive("ElTristana.JungleClear.Q"))
            {
                spells[Spells.Q].Cast();
            }
        }

        private static void OnLaneClear()
        {
            if (IsActive("ElTristana.LaneClear.Tower"))
            {
                foreach (var tower in ObjectManager.Get<Obj_AI_Turret>())
                {
                    if (!tower.IsDead && tower.Health > 100 && tower.IsEnemy && tower.LSIsValidTarget()
                        && Player.ServerPosition.LSDistance(tower.ServerPosition)
                        < Orbwalking.GetRealAutoAttackRange(Player))
                    {
                        spells[Spells.E].Cast(tower);
                    }
                }
            }

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.E].Range,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);

            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.E].LSIsReady() && IsActive("ElTristana.LaneClear.E") && minions.Count > 2
                && Player.ManaPercent > MenuInit.Menu.Item("ElTristana.LaneClear.E.Mana").GetValue<Slider>().Value)
            {
                foreach (var minion in
                    ObjectManager.Get<Obj_AI_Minion>().OrderByDescending(m => m.Health))
                {
                    spells[Spells.E].Cast(minion);
                    Orbwalker.ForceTarget(minion);
                }
            }

            var eminion =
                minions.Find(x => x.HasBuff("TristanaECharge") && x.LSIsValidTarget(1000));

            if (eminion != null)
            {
                Orbwalker.ForceTarget(eminion);
            }

            if (spells[Spells.Q].LSIsReady() && IsActive("ElTristana.LaneClear.Q"))
            {
                var eMob = minions.FindAll(x => x.LSIsValidTarget() && x.HasBuff("TristanaECharge")).FirstOrDefault();
                if (eMob != null)
                {
                    Orbwalker.ForceTarget(eMob);
                    spells[Spells.Q].Cast();
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (IsActive("ElTristana.Combo.Focus.E"))
            {
                var passiveTarget = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget() && x.HasBuff("TristanaECharge") 
                || x.HasBuff("tristanaechargesound") && Orbwalker.InAutoAttackRange(x));

                if (passiveTarget != null)
                {
                    Orbwalker.ForceTarget(passiveTarget);
                }
            }

            try
            {
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

                if (IsActive("ElTristana.killsteal.Active"))
                {
                    DoKillsteal();
                }

                spells[Spells.Q].Range = 550 + 9 * (Player.Level - 1);
                spells[Spells.E].Range = 625 + 9 * (Player.Level - 1);
                spells[Spells.R].Range = 517 + 9 * (Player.Level - 1);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (!Player.Spellbook.IsAutoAttacking)
            {
                damage += (float)ObjectManager.Player.LSGetAutoAttackDamage(enemy, true);
            }

            if (enemy.HasBuff("tristanaecharge"))
            {
                damage += (float)(spells[Spells.E].GetDamage(enemy) * (enemy.GetBuffCount("tristanaecharge") * 0.30)) +
                          spells[Spells.E].GetDamage(enemy);
            }

            if (spells[Spells.R].LSIsReady())
            {
                damage += spells[Spells.R].GetDamage(enemy);
            }

            return damage;
        }

        private static void DoKillsteal()
        {
            var enemy =
                HeroManager.Enemies
                     .FirstOrDefault(e => e.LSIsValidTarget(spells[Spells.R].Range) && spells[Spells.R].IsKillable(e));

            if (enemy != null)
            {
                spells[Spells.R].Cast(enemy);
            }
        }

        private static void UseItems(Obj_AI_Base target)
        {
            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            var useBladeEhp = MenuInit.Menu.Item("ElTristana.Items.Blade.EnemyEHP").GetValue<Slider>().Value;
            var useBladeMhp = MenuInit.Menu.Item("ElTristana.Items.Blade.EnemyMHP").GetValue<Slider>().Value;

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && IsActive("ElTristana.Items.Blade"))
            {
                botrk.Cast(target);
            }

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
                && Player.HealthPercent <= useBladeMhp && IsActive("ElTristana.Items.Blade"))
            {
                botrk.Cast(target);
            }

            if (cutlass.IsReady() && cutlass.IsOwned(Player) && cutlass.IsInRange(target)
                && target.HealthPercent <= useBladeEhp && IsActive("ElTristana.Items.Blade"))
            {
                cutlass.Cast(target);
            }

            if (ghost.IsReady() && ghost.IsOwned(Player) && target.LSIsValidTarget(spells[Spells.Q].Range)
                && IsActive("ElTristana.Items.Youmuu"))
            {
                ghost.Cast();
            }
        }

        #endregion
    }
}
 