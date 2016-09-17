using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElTalon
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Talon
    {
        #region Static Fields

        public static Menu Menu;

        private static SpellSlot ignite;

        private static Orbwalking.Orbwalker orbwalker;

        private static Spell Q, W, E, R;

        private static List<Spell> SpellList;

        private static Items.Item Tiamat, Hydra, Youmuu;

        #endregion

        #region Properties

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Talon")
            {
                return;
            }

            #region Spell Data

            // set spells
            Q = new Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(Player) + 100);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 650);

            W.SetSkillshot(0.25f, 75, 2300, false, SkillshotType.SkillshotLine);

            SpellList = new List<Spell> { Q, E, W, R };

            ignite = Player.GetSpellSlot("summonerdot");

            Tiamat = new Items.Item(3077, 400f);
            Youmuu = new Items.Item(3142, 0f);
            Hydra = new Items.Item(3074, 400f);

            InitializeMenu();

            #endregion

            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += AfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            new AssassinManager();
        }

        #endregion

        #region Methods

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (unit.IsMe && Q.IsReady() && target is AIHeroClient)
                    {
                        Q.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                    break;
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var antiGapActive = Menu.Item("Antigap").IsActive();
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }

            if (antiGapActive && E.IsReady() && gapcloser.Sender.Distance(Player) < 700)
            {
                E.Cast(target);
            }
        }

        private static void Combo()
        {
            var target = GetEnemy(W.Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var useW = Menu.Item("WCombo").IsActive();
            var useE = Menu.Item("ECombo").IsActive();
            var rCombo = Menu.Item("RCombo").IsActive();
            var onlyKill = Menu.Item("RWhenKill").IsActive();
            var smartUlt = Menu.Item("SmartUlt").IsActive();
            var youmuuitem = Menu.Item("UseYoumuu").IsActive();
            var ultCount = Menu.Item("rcount").GetValue<Slider>().Value;

            var comboDamage = GetComboDamage(target);
            var getUltComboDamage = GetUltComboDamage(target);

            var ultType = Menu.Item("ElTalon.Combo.Mode").GetValue<StringList>().SelectedIndex;

            switch (ultType)
            {
                case 0:

                    if (useE && E.IsReady())
                    {
                        E.Cast(target);
                    }
                    FightItems();
                    if (target.IsValidTarget(Q.Range))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }

                    if (Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }

                    if (useW && W.IsReady())
                    {
                        W.Cast(target);
                    }

                    if (onlyKill && E.IsReady() && rCombo && Q.IsReady()
                        && ObjectManager.Get<AIHeroClient>().Count(aiHero => aiHero.IsValidTarget(R.Range)) >= ultCount)
                    {
                        if (comboDamage >= target.Health)
                        {
                            R.Cast();
                        }
                    }

                    if (onlyKill && R.IsReady() && smartUlt)
                    {
                        if (getUltComboDamage >= target.Health)
                        {
                            R.Cast();
                        }
                    }

                    if (!onlyKill && E.IsReady() && rCombo && Q.IsReady()
                        && ObjectManager.Get<AIHeroClient>().Count(aiHero => aiHero.IsValidTarget(R.Range)) >= ultCount)
                    {
                        R.Cast();
                    }

                    break;

                case 1:

                    if (R.IsReady() && R.IsInRange(target))
                    {
                        R.Cast();
                    }

                    if (useE && E.IsReady())
                    {
                        E.Cast(target);
                    }

                    if (useW && W.IsReady())
                    {
                        W.Cast(target);
                    }

                    FightItems();
                    break;

                case 2:

                    if (useE && E.IsReady())
                    {
                        E.Cast(target);
                    }

                    if (R.IsReady() && R.IsInRange(target))
                    {
                        R.Cast();
                    }

                    if (useW && W.IsReady())
                    {
                        W.Cast(target);
                    }

                    FightItems();
                    break;
            }

            if (youmuuitem && Player.Distance(target) <= 400f && Youmuu.IsReady())
            {
                Youmuu.Cast();
            }

            if (target.IsValidTarget(600f) && IgniteDamage(target) >= target.Health && Menu.Item("UseIgnite").IsActive())
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawOff = Menu.Item("ElTalon.Drawingsoff").IsActive();
            var drawE = Menu.Item("ElTalon.DrawE").GetValue<Circle>();
            var drawW = Menu.Item("ElTalon.DrawW").GetValue<Circle>();
            var drawR = Menu.Item("ElTalon.DrawR").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawW.Active)
            {
                if (W.Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        W.Range,
                        W.IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawE.Active)
            {
                if (E.Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        E.Range,
                        E.IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawR.Active)
            {
                if (R.Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        R.Range,
                        R.IsReady() ? Color.Green : Color.Red);
                }
            }
        }

        private static void FightItems()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            var tiamatItem = Menu.Item("UseTiamat").IsActive();
            var hydraItem = Menu.Item("UseHydra").IsActive();

            if (Items.CanUseItem(3074) && hydraItem && Player.Distance(target) <= 400f)
            {
                Items.UseItem(3074);
            }

            if (Items.CanUseItem(3077) && tiamatItem && Player.Distance(target) <= 400f)
            {
                Items.UseItem(3077);
            }
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (W.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (E.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (R.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return (float)damage;
        }

        private static AIHeroClient GetEnemy(
            float vDefaultRange = 0,
            TargetSelector.DamageType vDefaultDamageType = TargetSelector.DamageType.Physical)
        {
            if (Math.Abs(vDefaultRange) < 0.00001)
            {
                vDefaultRange = Q.Range;
            }

            if (!Menu.Item("AssassinActive").GetValue<bool>())
            {
                return TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType);
            }

            var assassinRange = Menu.Item("AssassinSearchRange").GetValue<Slider>().Value;

            var vEnemy =
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        enemy =>
                        enemy.Team != ObjectManager.Player.Team && !enemy.IsDead && enemy.IsVisible
                        && Menu.Item("Assassin" + enemy.ChampionName) != null
                        && Menu.Item("Assassin" + enemy.ChampionName).GetValue<bool>()
                        && ObjectManager.Player.Distance(enemy) < assassinRange);

            if (Menu.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex == 1)
            {
                vEnemy = (from vEn in vEnemy select vEn).OrderByDescending(vEn => vEn.MaxHealth);
            }

            var objAiHeroes = vEnemy as AIHeroClient[] ?? vEnemy.ToArray();

            var t = !objAiHeroes.Any() ? TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType) : objAiHeroes[0];

            return t;
        }

        private static float GetUltComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (R.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return (float)damage;
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValid)
            {
                return;
            }

            var qHarass = Menu.Item("HarassQ").IsActive();
            var wHarass = Menu.Item("HarassW").IsActive();
            var eHarass = Menu.Item("HarassE").IsActive();

            foreach (var spell in SpellList.Where(y => y.IsReady()))
            {
                if (Player.ManaPercent >= Menu.Item("HarassMana").GetValue<Slider>().Value)
                {
                    if (spell.Slot == SpellSlot.Q && qHarass && Q.IsReady() && target.IsValidTarget(Q.Range)
                        && Q.IsReady())
                    {
                        Q.Cast();
                    }

                    if (spell.Slot == SpellSlot.W && wHarass && W.IsReady())
                    {
                        W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    }

                    if (spell.Slot == SpellSlot.E && eHarass && E.IsReady())
                    {
                        E.Cast(target);
                    }
                }
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

        private static void InitializeMenu()
        {
            Menu = new Menu("ElTalon", "Talon", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("fsfsafsaasffsadddd111dsasd", ""));
            comboMenu.AddItem(new MenuItem("QCombo", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("WCombo", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("ECombo", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("RCombo", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("fsfsafsaasffsa", ""));
            comboMenu.AddItem(new MenuItem("RWhenKill", "Use R only when killable").SetValue(true));
            comboMenu.AddItem(new MenuItem("SmartUlt", "Use smart ult").SetValue(true));
            comboMenu.AddItem(new MenuItem("rcount", "Min target to R >= ")).SetValue(new Slider(1, 1, 5));
            comboMenu.AddItem(new MenuItem("UseIgnite", "Use Ignite in combo when killable").SetValue(true));

            comboMenu.SubMenu("Combo mode")
                .AddItem(
                    new MenuItem("ElTalon.Combo.Mode", "Mode").SetValue(
                        new StringList(new[] { "Default [E->AA->Q->W->R]", "R->E->W->Q", "E->R->W->Q" })));

            comboMenu.AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            comboMenu.SubMenu("Items").AddItem(new MenuItem("UseTiamat", "Use Tiamat").SetValue(true));
            comboMenu.SubMenu("Items").AddItem(new MenuItem("UseHydra", "Use Hydra").SetValue(true));
            comboMenu.SubMenu("Items").AddItem(new MenuItem("UseYoumuu", "Use Youmuu").SetValue(true));

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "H"));
            harassMenu.AddItem(new MenuItem("fsfsafsaasffsadddd", ""));
            harassMenu.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("HarassW", "Use W").SetValue(true));
            harassMenu.AddItem(new MenuItem("HarassE", "Use E").SetValue(false));

            harassMenu.SubMenu("HarassMana")
                .AddItem(new MenuItem("HarassMana", "[Harass] Minimum Mana").SetValue(new Slider(30, 0, 100)));
            harassMenu.AddItem(
                new MenuItem("HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            var waveClearMenu = Menu.AddSubMenu(new Menu("WaveClear", "waveclear"));
            waveClearMenu.AddItem(new MenuItem("fsfsafsaasffsadddd111", ""));
            waveClearMenu.AddItem(new MenuItem("WaveClearQ", "Use Q").SetValue(true));
            waveClearMenu.AddItem(new MenuItem("WaveClearW", "Use W").SetValue(true));
            waveClearMenu.AddItem(new MenuItem("WaveClearE", "Use E").SetValue(false));
            waveClearMenu.SubMenu("Mana")
                .AddItem(new MenuItem("LaneClearMana", "[WaveClear] Minimum Mana").SetValue(new Slider(30, 0, 100)));
            waveClearMenu.AddItem(new MenuItem("fsfsafsaasffsadddd11sss1", ""));

            var settingsMenu = Menu.AddSubMenu(new Menu("Misc", "SuperSecretSettings"));
            settingsMenu.AddItem(new MenuItem("Antigap", "[BETA] Use E for gapclosers").SetValue(false));

            waveClearMenu.SubMenu("Items").AddItem(new MenuItem("HydraClear", "Use hydra").SetValue(true));
            waveClearMenu.SubMenu("Items").AddItem(new MenuItem("TiamatClear", "Use tiamat").SetValue(true));
            waveClearMenu.AddItem(
                new MenuItem("WaveClearActive", "WaveClear!").SetValue(
                    new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            var miscMenu = Menu.AddSubMenu(new Menu("Drawings", "Misc"));
            miscMenu.AddItem(new MenuItem("ElTalon.Drawingsoff", "Drawings off").SetValue(false));
            miscMenu.AddItem(new MenuItem("ElTalon.DrawW", "Draw W").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElTalon.DrawE", "Draw E").SetValue(new Circle()));
            miscMenu.AddItem(new MenuItem("ElTalon.DrawR", "Draw R").SetValue(new Circle()));

            var dmgAfterComboItem = new MenuItem("ElTalon.DrawComboDamage", "Draw combo damage").SetValue(true);
            miscMenu.AddItem(dmgAfterComboItem);

            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };

            var credits = Menu.AddSubMenu(new Menu("Credits", "jQuery"));
            credits.AddItem(new MenuItem("Paypal", "if you would like to donate via paypal:"));
            credits.AddItem(new MenuItem("Email", "info@zavox.nl"));

            Menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            Menu.AddItem(new MenuItem("422442fsaafsf", "Version: 2.0.0.1"));
            Menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By jQuery"));

            Menu.AddToMainMenu();
        }

        private static void JungleClear()
        {
            var qWaveClear = Menu.Item("WaveClearQ").IsActive();
            var wWaveClear = Menu.Item("WaveClearW").IsActive();
            var eWaveClear = Menu.Item("WaveClearE").IsActive();
            var hydraClear = Menu.Item("HydraClear").IsActive();
            var tiamatClear = Menu.Item("TiamatClear").IsActive();

            var target =
                MinionManager.GetMinions(
                    Player.Position,
                    700,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (Player.ManaPercent >= Menu.Item("LaneClearMana").GetValue<Slider>().Value)
            {
                if (qWaveClear && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                if (wWaveClear && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }

                if (eWaveClear && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(target);
                }
            }

            if (Items.CanUseItem(3074) && hydraClear && target.IsValidTarget(Hydra.Range))
            {
                Items.UseItem(3074);
            }

            if (Items.CanUseItem(3077) && tiamatClear && target.IsValidTarget(Tiamat.Range))
            {
                Items.UseItem(3077);
            }
        }

        private static void LaneClear()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, W.Range).FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            var qWaveClear = Menu.Item("WaveClearQ").IsActive();
            var wWaveClear = Menu.Item("WaveClearW").IsActive();
            var eWaveClear = Menu.Item("WaveClearE").IsActive();
            var hydraClear = Menu.Item("HydraClear").IsActive();
            var tiamatClear = Menu.Item("TiamatClear").IsActive();
            var minions = MinionManager.GetMinions(Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (Player.ManaPercent >= Menu.Item("LaneClearMana").GetValue<Slider>().Value)
            {
                if (qWaveClear && Q.IsReady() && minion.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                if (wWaveClear && W.IsReady() && minion.IsValidTarget(W.Range))
                {
                    if (minions.Count > 2)
                    {
                        var farmLocation = W.GetCircularFarmLocation(minions);
                        W.Cast(farmLocation.Position);
                    }
                }

                if (eWaveClear && E.IsReady() && minion.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(minion);
                }
            }

            if (Items.CanUseItem(3074) && hydraClear && minion.IsValidTarget(Hydra.Range) && minions.Count() > 1)
            {
                Items.UseItem(3074);
            }

            if (Items.CanUseItem(3077) && tiamatClear && minion.IsValidTarget(Tiamat.Range) && minions.Count() > 1)
            {
                Items.UseItem(3077);
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }
        }

        #endregion
    }
}