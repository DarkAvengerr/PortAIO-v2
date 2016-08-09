namespace ElAlistarReborn
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Alistar
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the E spell
        /// </summary>
        /// <value>
        ///     The E spell
        /// </value>
        private static Spell E { get; set; }

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
        /// <value>
        ///     The menu
        /// </value>
        private static Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the orbwalker
        /// </summary>
        /// <value>
        ///     The orbwalker
        /// </value>
        private static Orbwalking.Orbwalker Orbwalker { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets or sets the Q spell
        /// </summary>
        /// <value>
        ///     The Q spell
        /// </value>
        private static Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the R spell.
        /// </summary>
        /// <value>
        ///     The R spell
        /// </value>
        private static Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the W spell
        /// </summary>
        /// <value>
        ///     The W spell
        /// </value>
        private static Spell W { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The IgniteSpell
        /// </value>
        public static Spell IgniteSpell { get; set; }

        /// <summary>
        ///     FlashSlot
        /// </summary>
        public static SpellSlot FlashSlot;


        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Fired when the game loads.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void OnGameLoad()
        {
            try
            {
                if (Player.ChampionName != "Alistar")
                {
                    return;
                }

                var igniteSlot = Player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("summonerdot")
                                    ? SpellSlot.Summoner1
                                    : Player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("summonerdot")
                                          ? SpellSlot.Summoner2
                                          : SpellSlot.Unknown;

                if (igniteSlot != SpellSlot.Unknown)
                {
                    IgniteSpell = new Spell(igniteSlot, 600f);
                }

                FlashSlot = Player.GetSpellSlot("summonerflash");

                Q = new Spell(SpellSlot.Q, 365f);
                W = new Spell(SpellSlot.W, 650f);
                E = new Spell(SpellSlot.E, 575f);
                R = new Spell(SpellSlot.R);

                GenerateMenu();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                AttackableUnit.OnDamage += AttackableUnit_OnDamage;
                Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
                AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the menu
        /// </summary>
        /// <value>
        ///     Creates the menu
        /// </value>
        private static void GenerateMenu()
        {
            try
            {
                Menu = new Menu("ElAlistar", "ElAlistar", true);

                var targetselectorMenu = new Menu("Target Selector", "Target Selector");
                {
                    TargetSelector.AddToMenu(targetselectorMenu);
                }

                Menu.AddSubMenu(targetselectorMenu);

                var orbwalkMenu = new Menu("Orbwalker", "Orbwalker");
                {
                    Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
                }

                Menu.AddSubMenu(orbwalkMenu);

                var comboMenu = new Menu("Combo Settings", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElAlistar.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElAlistar.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElAlistar.Combo.R", "Use R").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElAlistar.Combo.RHeal.HP", "R on Health percentage").SetValue(new Slider(60, 1)));
                    comboMenu.AddItem(new MenuItem("ElAlistar.Combo.RHeal.Damage", "R on damage dealt %").SetValue(new Slider(60, 1)));
                }

                Menu.AddSubMenu(comboMenu);

                var flashMenu = new Menu("Flash Settings", "Flash");
                {
                    flashMenu.AddItem(new MenuItem("ElAlistar.Flash.Click", "Left Click [on] TS [off]").SetValue(true));
                    flashMenu.AddItem(
                    new MenuItem("ElAlistar.Combo.FlashQ", "Flash Q").SetValue(
                        new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
                }

                Menu.AddSubMenu(flashMenu);

                var healMenu = new Menu("Heal Settings", "Heal");
                {
                    healMenu.AddItem(new MenuItem("ElAlistar.Heal.E", "Use heal").SetValue(true));
                    healMenu.AddItem(new MenuItem("Heal.HP", "Health percentage").SetValue(new Slider(80, 1)));
                    healMenu.AddItem(new MenuItem("Heal.Damage", "Heal on damage dealt %").SetValue(new Slider(80, 1)));
                    healMenu.AddItem(
                            new MenuItem("ElAlistar.Heal.Mana", "Minimum mana").SetValue(
                                new Slider(20, 0, 100)));
                    healMenu.AddItem(new MenuItem("seperator21", ""));
                    foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
                    {
                        healMenu.AddItem(new MenuItem("healon" + x.ChampionName, "Use for " + x.ChampionName))
                            .SetValue(true);
                    }
                }

                Menu.AddSubMenu(healMenu);

                var interrupterMenu = new Menu("Interrupter Settings", "Interrupter");
                {
                    interrupterMenu.AddItem(new MenuItem("ElAlistar.Interrupter.Q", "Use Q").SetValue(true));
                    interrupterMenu.AddItem(new MenuItem("ElAlistar.Interrupter.W", "Use W").SetValue(true));
                    interrupterMenu.AddItem(new MenuItem("ElAlistar.GapCloser", "Anti gapcloser").SetValue(true));
                }

                Menu.AddSubMenu(interrupterMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Misc");
                {
                    miscellaneousMenu.AddItem(new MenuItem("ElAlistar.Ignite", "Use Ignite").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElAlistar.Drawings.W", "Draw W range").SetValue(true));

                }

                Menu.AddSubMenu(miscellaneousMenu);

                Menu.AddToMainMenu();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     Gets the active menu item
        /// </summary>
        /// <value>
        ///     The menu item
        /// </value>
        private static bool IsActive(string menuName)
        {
            return Menu.Item(menuName).IsActive();
        }
        /// <summary>
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnDraw(EventArgs args)
        {
            try
            {
                if (IsActive("ElAlistar.Drawings.W"))
                {
                    if (W.Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.DeepSkyBlue);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        /// <summary>
        ///     The ignite killsteal logic
        /// </summary>
        private static void HandleIgnite()
        {
            try
            {
                var kSableEnemy =
                    HeroManager.Enemies.FirstOrDefault(
                        hero =>
                        hero.IsValidTarget(550) && ShieldCheck(hero) && !hero.HasBuff("summonerdot") && !hero.IsZombie
                        && Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) >= hero.Health);

                if (kSableEnemy != null && IgniteSpell.Slot != SpellSlot.Unknown)
                {
                    Player.Spellbook.CastSpell(IgniteSpell.Slot, kSableEnemy);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        ///     The shield checker
        /// </summary>
        private static bool ShieldCheck(Obj_AI_Base hero)
        {
            try
            {
                return !hero.HasBuff("summonerbarrier") || !hero.HasBuff("BlackShield")
                       || !hero.HasBuff("SivirShield") || !hero.HasBuff("BansheesVeil")
                       || !hero.HasBuff("ShroudofDarkness");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return false;
        }


        /// <summary>
        ///     Returns the mana
        /// </summary>
        private static bool HasEnoughMana()
        {
            return Player.Mana
                   > Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana + Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
        }

        /// <summary>
        ///     Combo logic
        /// </summary>
        private static void OnCombo()
        {
            try
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                if (target == null)
                {
                    return;
                }

                if (IsActive("ElAlistar.Combo.Q") && IsActive("ElAlistar.Combo.W") && Q.IsReady() && W.IsReady())
                {
                    if (target.IsValidTarget(W.Range) && HasEnoughMana())
                    {
                        if (target.IsValidTarget(Q.Range))
                        {
                            Q.Cast();
                            return;
                        }

                        if (W.Cast(target).IsCasted())
                        {
                            var comboTime = Math.Max(0, Player.Distance(target) - 365) / 1.2f - 25;
                            LeagueSharp.Common.Utility.DelayAction.Add((int)comboTime, () => Q.Cast());
                        }
                    }
                }

                if (IsActive("ElAlistar.Combo.Q") && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                if (IsActive("ElAlistar.Combo.W"))
                {
                    if (target.IsValidTarget(W.Range) && W.GetDamage(target) > target.Health)
                    {
                        W.Cast(target);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.Distance(Player) > W.Range)
            {
                return;
            }

            if (sender.IsValidTarget(Q.Range) && Q.IsReady() && IsActive("ElAlistar.Interrupter.Q"))
            {
                Q.Cast();
            }

            if (sender.IsValidTarget(W.Range) && W.IsReady() && IsActive("ElAlistar.Interrupter.W"))
            {
                W.Cast(sender);
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (IsActive("ElAlistar.GapCloser"))
            {
                if (Q.IsReady()
                    && gapcloser.Sender.Distance(Player) < Q.Range)
                {
                    Q.Cast();
                }

                if (W.IsReady() && gapcloser.Sender.Distance(Player) < W.Range)
                {
                    W.Cast(gapcloser.Sender);
                }
            }
        }

        private static void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            var obj = ObjectManager.GetUnitByNetworkId<GameObject>((uint)args.Target.NetworkId);

            if (obj.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            var hero = (AIHeroClient)obj;

            if (hero.IsEnemy)
            {
                return;
            }

            if (Menu.Item("ElAlistar.Combo.R").IsActive())
            {
                if (ObjectManager.Get<AIHeroClient>()
                        .Any(
                            x =>
                            x.IsAlly && x.IsMe && !x.IsDead && ((int)(args.Damage / x.MaxHealth * 100)
                                > Menu.Item("ElAlistar.Combo.RHeal.Damage").GetValue<Slider>().Value
                                || x.HealthPercent < Menu.Item("ElAlistar.Combo.RHeal.HP").GetValue<Slider>().Value && x.CountEnemiesInRange(1000) >= 1)))
                {
                    R.Cast();
                }
            }

            if (Menu.Item("ElAlistar.Heal.E").IsActive() && Player.ManaPercent > Menu.Item("ElAlistar.Heal.Mana").GetValue<Slider>().Value)
            {
                if (
                    ObjectManager.Get<AIHeroClient>()
                        .Any(
                            x =>
                            x.IsAlly && !x.IsDead && Menu.Item(string.Format("healon{0}", x.ChampionName)).IsActive()
                            && ((int)(args.Damage / x.MaxHealth * 100)
                                > Menu.Item("Heal.Damage").GetValue<Slider>().Value
                                || x.HealthPercent < Menu.Item("Heal.HP").GetValue<Slider>().Value)
                            && x.Distance(Player) < E.Range && x.CountEnemiesInRange(1000) >= 1))
                {
                    E.Cast();
                }
            }
        }

        /// <summary>
        ///     Called when the game updates
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead || Player.IsRecalling() || Player.InFountain())
                {
                    return;
                }

                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        OnCombo();
                        break;
                }

                if (IsActive("ElAlistar.Ignite"))
                {
                    HandleIgnite();
                }

                if (Menu.Item("ElAlistar.Combo.FlashQ").GetValue<KeyBind>().Active && Q.IsReady())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                    var target = IsActive("ElAlistar.Flash.Click")
                                     ? TargetSelector.GetSelectedTarget()
                                     : TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                    if (!target.IsValidTarget(W.Range))
                    {
                        return;
                    }

                    Player.Spellbook.CastSpell(FlashSlot, target.ServerPosition);
                    LeagueSharp.Common.Utility.DelayAction.Add(50, () => Q.Cast());
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}