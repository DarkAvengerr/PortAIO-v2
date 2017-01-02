namespace ElEasy.Plugins
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

    public class Katarina : IPlugin
    {
        #region Static Fields

        public static Vector2 JumpPos;

        private static readonly bool castWardAgain = true;

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           { Spells.Q, new Spell(SpellSlot.Q, 675) },
                                                                           { Spells.W, new Spell(SpellSlot.W, 375) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 700) },
                                                                           { Spells.R, new Spell(SpellSlot.R, 550) }
                                                                       };

        private static SpellSlot Ignite;

        private static bool isChanneling;

        private static long lastECast;

        private static int lastPlaced;

        private static Vector3 lastWardPos;

        private static Orbwalking.Orbwalker Orbwalker;

        private static bool reCheckWard = true;

        private static float rStart;

        private static float wcasttime;

        #endregion

        #region Enums

        public enum Spells
        {
            Q,

            W,

            E,

            R
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            this.Menu = new Menu("ElKatarina", "ElKatarina");
            {
                var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                this.Menu.AddSubMenu(orbwalkerMenu);

                var targetSelector = new Menu("Target Selector", "TargetSelector");
                TargetSelector.AddToMenu(targetSelector);
                this.Menu.AddSubMenu(targetSelector);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElEasy.Katarina.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Katarina.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Katarina.Combo.E", "Use E").SetValue(true));

                    comboMenu.SubMenu("E").AddItem(new MenuItem("ElEasy.Katarina.E.Legit", "Legit E").SetValue(false));
                    comboMenu.SubMenu("E")
                        .AddItem(new MenuItem("ElEasy.Katarina.E.Delay", "E Delay").SetValue(new Slider(1000, 0, 2000)));

                    comboMenu.SubMenu("R").AddItem(new MenuItem("ElEasy.Katarina.Combo.R", "Use R").SetValue(true));
                    comboMenu.SubMenu("R")
                        .AddItem(
                            new MenuItem("ElEasy.Katarina.Combo.Sort", "R:").SetValue(
                                new StringList(new[] { "Normal", "Smart" })));
                    comboMenu.SubMenu("R")
                        .AddItem(new MenuItem("ElEasy.Katarina.Combo.R.Force", "Force R").SetValue(false));
                    comboMenu.SubMenu("R")
                        .AddItem(
                            new MenuItem("ElEasy.Katarina.Combo.R.Force.Count", "Force R when in range:").SetValue(
                                new Slider(3, 0, 5)));
                    comboMenu.AddItem(new MenuItem("ElEasy.Katarina.Combo.Ignite", "Use Ignite").SetValue(true));
                }

                this.Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElEasy.Katarina.Harass.Q", "Use Q").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElEasy.Katarina.Harass.W", "Use W").SetValue(true));
                    harassMenu.AddItem(new MenuItem("ElEasy.Katarina.Harass.E", "Use E").SetValue(true));

                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(
                            new MenuItem("ElEasy.Katarina.AutoHarass.Activated", "Auto harass", true).SetValue(
                                new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle)));
                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Katarina.AutoHarass.Q", "Use Q").SetValue(true));
                    harassMenu.SubMenu("Harass")
                        .SubMenu("AutoHarass settings")
                        .AddItem(new MenuItem("ElEasy.Katarina.AutoHarass.W", "Use W").SetValue(true));

                    harassMenu.SubMenu("Harass")
                        .AddItem(
                            new MenuItem("ElEasy.Katarina.Harass.Mode", "Harass mode:").SetValue(
                                new StringList(new[] { "Q", "Q - W", "Q - E - W" })));
                }

                this.Menu.AddSubMenu(harassMenu);

                var clearMenu = new Menu("Clear", "Clear");
                {
                    clearMenu.SubMenu("Lasthit")
                        .AddItem(new MenuItem("ElEasy.Katarina.Lasthit.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Lasthit")
                        .AddItem(new MenuItem("ElEasy.Katarina.Lasthit.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Lasthit")
                        .AddItem(new MenuItem("ElEasy.Katarina.Lasthit.E", "Use E").SetValue(false));
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Katarina.LaneClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Katarina.LaneClear.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Katarina.LaneClear.E", "Use E").SetValue(false));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Katarina.JungleClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Katarina.JungleClear.W", "Use W").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Katarina.JungleClear.E", "Use E").SetValue(false));
                }

                this.Menu.AddSubMenu(clearMenu);

                var wardjumpMenu = new Menu("Wardjump", "Wardjump");
                {
                    wardjumpMenu.AddItem(
                        new MenuItem("ElEasy.Katarina.Wardjump", "Wardjump key").SetValue(
                            new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));

                    wardjumpMenu.AddItem(new MenuItem("ElEasy.Wardjump.Mouse", "Move to mouse").SetValue(true));
                    wardjumpMenu.AddItem(new MenuItem("ElEasy.Wardjump.Minions", "Jump to minions").SetValue(false));
                    wardjumpMenu.AddItem(new MenuItem("ElEasy.Wardjump.Champions", "Jump to champions").SetValue(false));
                }

                this.Menu.AddSubMenu(wardjumpMenu);

                var itemMenu = new Menu("Items", "Items");
                {
                    itemMenu.AddItem(
                        new MenuItem("ElEasy.Katarina.Items.hextech", "Use Hextech Gunblade").SetValue(true));
                }

                this.Menu.AddSubMenu(itemMenu);

                var killstealMenu = new Menu("Killsteal", "Killsteal");
                {
                    killstealMenu.AddItem(new MenuItem("ElEasy.Katarina.Killsteal", "Killsteal").SetValue(true));
                    killstealMenu.AddItem(
                        new MenuItem("ElEasy.Katarina.Killsteal.R", "Killsteal with R").SetValue(true));
                }

                this.Menu.AddSubMenu(killstealMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Katarina.Draw.off", "Turn drawings off").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Katarina.Draw.Q", "Draw Q").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Katarina.Draw.W", "Draw W").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Katarina.Draw.E", "Draw E").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Katarina.Draw.R", "Draw R").SetValue(new Circle()));

                    var dmgAfterE = new MenuItem("ElEasy.Katarina.DrawComboDamage", "Draw combo damage").SetValue(true);
                    var drawFill =
                        new MenuItem("ElEasy.Katarina.DrawColour", "Fill colour", true).SetValue(
                            new Circle(true, Color.FromArgb(204, 204, 0, 0)));
                    miscellaneousMenu.AddItem(drawFill);
                    miscellaneousMenu.AddItem(dmgAfterE);

                    //DrawDamage.DamageToUnit = this.GetComboDamage;
                    //DrawDamage.Enabled = dmgAfterE.IsActive();
                    //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
                    //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

                    dmgAfterE.ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                            {
                                //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                            };

                    drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                        {
                            //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                            //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                        };
                }

                this.Menu.AddSubMenu(miscellaneousMenu);
            }
            rootMenu.AddSubMenu(this.Menu);
        }

        public void Load()
        {
            try
            {
                Console.WriteLine("Loaded Katarina");
                Ignite = this.Player.GetSpellSlot("summonerdot");
                spells[Spells.R].SetCharged( 550, 550, 1.0f);

                Game.OnUpdate += this.OnUpdate;
                Drawing.OnDraw += this.OnDraw;
                EloBuddy.Player.OnIssueOrder += this.AIHeroClient_OnIssueOrder;
                GameObject.OnCreate += this.GameObject_OnCreate;
                Obj_AI_Base.OnSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
                Orbwalking.BeforeAttack += this.BeforeAttack;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Methods

        private static void CastEWard(Obj_AI_Base obj)
        {
            if (500 >= Environment.TickCount - wcasttime)
            {
                return;
            }

            spells[Spells.E].CastOnUnit(obj);
            wcasttime = Environment.TickCount;
        }

        private static bool KatarinaQ(Obj_AI_Base target)
        {
            return target.Buffs.Any(x => x.Name.Contains("katarinaqmark"));
        }

        private void CastE(Obj_AI_Base unit)
        {
            var playLegit = this.Menu.Item("ElEasy.Katarina.E.Legit").IsActive();
            var legitCastDelay = this.Menu.Item("ElEasy.Katarina.E.Delay").GetValue<Slider>().Value;

            if (playLegit)
            {
                if (Environment.TickCount > lastECast + legitCastDelay)
                {
                    spells[Spells.E].CastOnUnit(unit);
                    lastECast = Environment.TickCount;
                }
            }
            else
            {
                spells[Spells.E].CastOnUnit(unit);
                lastECast = Environment.TickCount;
            }
        }

        private InventorySlot FindBestWardItem()
        {
            var slot = Items.GetWardSlot();
            if (slot == default(InventorySlot))
            {
                return null;
            }

            var sdi = this.GetItemSpell(slot);

            if (sdi != default(SpellDataInst) && sdi.State == SpellState.Ready)
            {
                return slot;
            }
            return slot;
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!spells[Spells.E].IsReady() || !(sender is Obj_AI_Minion) || Environment.TickCount >= lastPlaced + 300)
            {
                return;
            }

            if (Environment.TickCount >= lastPlaced + 300)
            {
                return;
            }
            var ward = (Obj_AI_Minion)sender;

            if (ward.Name.ToLower().Contains("ward") && ward.Distance(lastWardPos) < 500)
            {
                spells[Spells.E].Cast(ward);
            }
        }

        private double QMarkDamage(Obj_AI_Base target)
        {
            return target.HasBuff("katarinaqmark") ? this.Player.GetSpellDamage(target, SpellSlot.Q, 1) : 0;
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (spells[Spells.Q].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            damage += this.QMarkDamage(enemy);

            if (spells[Spells.W].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (spells[Spells.E].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (spells[Spells.R].IsReady())
            {
                damage += this.Player.GetSpellDamage(enemy, SpellSlot.R) * 8;
            }

            return (float)damage;
        }

        private SpellDataInst GetItemSpell(InventorySlot invSlot)
        {
            return this.Player.Spellbook.Spells.FirstOrDefault(spell => (int)spell.Slot == invSlot.Slot + 4);
        }

        private bool HasRBuff()
        {
            return this.Player.HasBuff("KatarinaR") || this.Player.IsChannelingImportantSpell()
                   || this.Player.HasBuff("katarinarsound");
        }

        private float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || this.Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)this.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private void KillSteal()
        {
            foreach (
                var hero in
                    HeroManager.Enemies
                        .Where(hero => hero.IsValidTarget(spells[Spells.E].Range) && !hero.IsInvulnerable))
            {
                var qdmg = spells[Spells.Q].GetDamage(hero);
                var wdmg = spells[Spells.W].GetDamage(hero);
                var edmg = spells[Spells.E].GetDamage(hero);
                var markDmg = this.Player.CalcDamage(
                    hero,
                    Damage.DamageType.Magical,
                    this.Player.FlatMagicDamageMod * 0.15 + this.Player.Level * 15);
                float ignitedmg;

                if (Ignite != SpellSlot.Unknown)
                {
                    ignitedmg = (float)this.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
                }
                else
                {
                    ignitedmg = 0f;
                }

                if (hero.HasBuff("katarinaqmark") && hero.Health - wdmg - markDmg < 0 && spells[Spells.W].IsReady()
                    && hero.IsValidTarget(spells[Spells.W].Range))
                {
                    spells[Spells.W].Cast();
                }

                if (hero.Health - ignitedmg < 0 && Ignite.IsReady() && hero.IsValidTarget(600))
                {
                    this.Player.Spellbook.CastSpell(Ignite, hero);
                }

                if (hero.Health - edmg < 0 && spells[Spells.E].IsReady() && hero.IsValidTarget(spells[Spells.E].Range))
                {
                    spells[Spells.E].Cast(hero);
                }

                if (hero.Health - qdmg < 0 && spells[Spells.Q].IsReady() && spells[Spells.Q].IsInRange(hero))
                {
                    spells[Spells.Q].Cast(hero);
                }

                if (hero.Health - edmg - wdmg < 0 && spells[Spells.E].IsReady() && spells[Spells.W].IsReady()
                    && hero.IsValidTarget(spells[Spells.E].Range))
                {
                    this.CastE(hero);
                    if (spells[Spells.W].IsInRange(hero))
                    {
                        spells[Spells.W].Cast();
                    }
                }

                if (hero.Health - edmg - qdmg < 0 && spells[Spells.E].IsReady() && spells[Spells.Q].IsReady()
                    && hero.IsValidTarget(spells[Spells.E].Range))
                {
                    this.CastE(hero);
                    spells[Spells.Q].Cast(hero);
                }

                if (hero.Health - edmg - wdmg - qdmg < 0 && spells[Spells.E].IsReady() && spells[Spells.Q].IsReady()
                    && spells[Spells.W].IsReady() && hero.IsValidTarget(spells[Spells.E].Range))
                {
                    this.CastE(hero);
                    spells[Spells.Q].Cast(hero);
                    if (hero.IsValidTarget(spells[Spells.W].Range))
                    {
                        spells[Spells.W].Cast();
                    }
                }

                if (hero.Health - edmg - wdmg - qdmg - markDmg < 0 && spells[Spells.E].IsReady()
                    && spells[Spells.Q].IsReady() && spells[Spells.W].IsReady()
                    && hero.IsValidTarget(spells[Spells.E].Range))
                {
                    this.CastE(hero);
                    spells[Spells.Q].Cast(hero);
                    if (hero.IsValidTarget(spells[Spells.W].Range))
                    {
                        spells[Spells.W].Cast();
                    }
                }

                if (hero.Health - edmg - wdmg - qdmg - ignitedmg < 0 && spells[Spells.E].IsReady()
                    && spells[Spells.Q].IsReady() && spells[Spells.W].IsReady() && Ignite.IsReady()
                    && hero.IsValidTarget(spells[Spells.E].Range))
                {
                    this.CastE(hero);
                    spells[Spells.Q].Cast(hero);
                    if (hero.IsValidTarget(spells[Spells.W].Range))
                    {
                        spells[Spells.W].Cast();
                        this.Player.Spellbook.CastSpell(Ignite, hero);
                    }
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.SData.Name != "KatarinaR" || !this.Player.HasBuff("katarinarsound"))
            {
                return;
            }

            isChanneling = true;
            Orbwalker.SetMovement(false);
            Orbwalker.SetAttack(false);
            LeagueSharp.Common.Utility.DelayAction.Add(1, () => isChanneling = false);
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe)
            {
                args.Process = !this.Player.HasBuff("KatarinaR");
            }
        }

        private void AIHeroClient_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && Utils.GameTimeTickCount < rStart  && args.Order == GameObjectOrder.MoveTo)
            {
                args.Process = false;
            }
        }

        private void OnAutoHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget() || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Katarina.AutoHarass.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Katarina.AutoHarass.W").IsActive();

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget())
            {
                spells[Spells.Q].Cast(target);
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            this.UseItems(target);

            var useQ = this.Menu.Item("ElEasy.Katarina.Combo.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Katarina.Combo.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Katarina.Combo.E").IsActive();
            var useR = this.Menu.Item("ElEasy.Katarina.Combo.R").IsActive();
            var useI = this.Menu.Item("ElEasy.Katarina.Combo.Ignite").IsActive();
            var rSort = this.Menu.Item("ElEasy.Katarina.Combo.Sort").GetValue<StringList>();
            var forceR = this.Menu.Item("ElEasy.Katarina.Combo.R.Force").IsActive();
            var forceRCount = this.Menu.Item("ElEasy.Katarina.Combo.R.Force.Count").GetValue<Slider>().Value;

            if (useQ && spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast(target);
            }

            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
            {
                this.CastE(target);
            }

            if (useW && spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast();
            }
            if (useR && spells[Spells.R].IsReady())
            {
                switch (rSort.SelectedIndex)
                {
                    case 0:
                        if (this.Player.CountEnemiesInRange(spells[Spells.R].Range) > 0 && spells[Spells.R].IsReady())
                        {
                            spells[Spells.R].Cast();
                            Orbwalker.SetMovement(false);
                            Orbwalker.SetAttack(false);
                            rStart = Utils.GameTimeTickCount + 400;
                        }
                        break;

                    case 1:
                        if (!spells[Spells.E].IsReady()
                            || forceR && this.Player.CountEnemiesInRange(spells[Spells.R].Range) <= forceRCount)
                        {
                            spells[Spells.R].Cast();
                            Orbwalker.SetMovement(false);
                            Orbwalker.SetAttack(false);
                            rStart = Utils.GameTimeTickCount + 400;
                        }
                        break;
                }
            }

            if (target.IsValidTarget(600) && this.IgniteDamage(target) >= target.Health && useI)
            {
                this.Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = this.Menu.Item("ElEasy.Katarina.Draw.off").IsActive();
            var drawQ = this.Menu.Item("ElEasy.Katarina.Draw.Q").GetValue<Circle>();
            var drawW = this.Menu.Item("ElEasy.Katarina.Draw.W").GetValue<Circle>();
            var drawE = this.Menu.Item("ElEasy.Katarina.Draw.E").GetValue<Circle>();
            var drawR = this.Menu.Item("ElEasy.Katarina.Draw.R").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.Q].Range, Color.DeepPink);
                }
            }

            if (drawW.Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.W].Range, Color.DeepSkyBlue);
                }
            }

            if (drawE.Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR.Active)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }
        }

        private void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Katarina.Harass.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Katarina.Harass.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Katarina.Harass.E").IsActive();
            var hMode = this.Menu.Item("ElEasy.Katarina.Harass.Mode").GetValue<StringList>().SelectedIndex;

            switch (hMode)
            {
                case 0:
                    if (useQ && spells[Spells.Q].IsReady())
                    {
                        spells[Spells.Q].CastOnUnit(target);
                    }
                    break;

                case 1:
                    if (useQ && useW)
                    {
                        if (spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }

                        if (target.IsValidTarget(spells[Spells.W].Range) && spells[Spells.W].IsReady())
                        {
                            spells[Spells.W].Cast();
                        }
                    }
                    break;

                case 2:
                    if (useQ && useW && useE)
                    {
                        if (spells[Spells.Q].IsReady() && target.IsValidTarget(spells[Spells.Q].Range))
                        {
                            spells[Spells.Q].Cast(target);
                        }

                        if (spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                        {
                            this.CastE(target);
                        }

                        if (spells[Spells.W].IsReady() && target.IsValidTarget(spells[Spells.W].Range))
                        {
                            spells[Spells.W].Cast();
                        }
                    }
                    break;
            }
        }

        private void OnJungleclear()
        {
            var useQ = this.Menu.Item("ElEasy.Katarina.JungleClear.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Katarina.JungleClear.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Katarina.JungleClear.E").IsActive();

            var minions =
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.E].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minions == null)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast(minions);
            }

            if (useW && spells[Spells.W].IsReady() && spells[Spells.W].IsInRange(minions))
            {
                spells[Spells.W].Cast();
            }

            if (useE && spells[Spells.E].IsReady())
            {
                this.CastE(minions);
            }
        }

        private void OnLaneclear()
        {
            var useQ = this.Menu.Item("ElEasy.Katarina.LaneClear.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Katarina.LaneClear.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Katarina.LaneClear.E").IsActive();

            var minions =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.E].Range).FirstOrDefault();

            if (minions == null)
            {
                return;
            }

            if (spells[Spells.W].IsReady() && useW)
            {
                if (minions.Health < spells[Spells.W].GetDamage(minions))
                {
                    spells[Spells.W].Cast();
                }
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                if (minions.Health < spells[Spells.Q].GetDamage(minions))
                {
                    spells[Spells.Q].CastOnUnit(minions);
                }
            }

            if (spells[Spells.E].IsReady() && useE)
            {
                if (minions.Health < spells[Spells.E].GetDamage(minions))
                {
                    this.CastE(minions);
                }
            }
        }

        private void OnLasthit()
        {
            var useQ = this.Menu.Item("ElEasy.Katarina.Lasthit.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Katarina.Lasthit.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Katarina.Lasthit.E").IsActive();

            var minions =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.E].Range).FirstOrDefault();

            if (minions == null)
            {
                return;
            }

            if (spells[Spells.W].IsReady() && useW)
            {
                if (minions.Health < spells[Spells.W].GetDamage(minions))
                {
                    spells[Spells.W].Cast();
                }
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                if (minions.Health < spells[Spells.Q].GetDamage(minions))
                {
                    spells[Spells.Q].CastOnUnit(minions);
                }
            }

            if (spells[Spells.E].IsReady() && useE)
            {
                if (minions.Health < spells[Spells.E].GetDamage(minions))
                {
                    this.CastE(minions);
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead)
                {
                    return;
                }

                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        this.OnCombo();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        this.OnHarass();
                        break;

                    case Orbwalking.OrbwalkingMode.LaneClear:
                        this.OnLaneclear();
                        this.OnJungleclear();
                        break;

                    case Orbwalking.OrbwalkingMode.LastHit:
                        this.OnLasthit();
                        break;
                }

                if (this.Menu.Item("ElEasy.Katarina.Killsteal").IsActive())
                {
                    this.KillSteal();
                }

                if (this.Menu.Item("ElEasy.Katarina.AutoHarass.Activated", true).GetValue<KeyBind>().Active)
                {
                    this.OnAutoHarass();
                }

                if (this.Menu.Item("ElEasy.Katarina.Wardjump").GetValue<KeyBind>().Active)
                {
                    this.WardjumpToMouse();
                }

                if (this.HasRBuff())
                {
                    Orbwalker.SetAttack(false);
                    Orbwalker.SetMovement(false);
                }
                else
                {
                    Orbwalker.SetAttack(true);
                    Orbwalker.SetMovement(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Orbwalk(Vector3 pos, AIHeroClient target = null)
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos);
        }

        private void UseItems(Obj_AI_Base target)
        {
            if (this.Menu.Item("ElEasy.Katarina.Items.hextech").IsActive())
            {
                var cutlass = ItemData.Bilgewater_Cutlass.GetItem();
                var hextech = ItemData.Hextech_Gunblade.GetItem();

                if (cutlass.IsReady() && cutlass.IsOwned(this.Player) && cutlass.IsInRange(target))
                {
                    cutlass.Cast(target);
                }

                if (hextech.IsReady() && hextech.IsOwned(this.Player) && hextech.IsInRange(target))
                {
                    hextech.Cast(target);
                }
            }
        }

        private void WardJump(
            Vector3 pos,
            bool m2M = true,
            bool maxRange = false,
            bool reqinMaxRange = false,
            bool minions = true,
            bool champions = true)
        {
            if (!spells[Spells.E].IsReady())
            {
                return;
            }

            var basePos = this.Player.Position.To2D();
            var newPos = (pos.To2D() - this.Player.Position.To2D());

            if (JumpPos == new Vector2())
            {
                if (reqinMaxRange)
                {
                    JumpPos = pos.To2D();
                }
                else if (maxRange || this.Player.Distance(pos) > 590)
                {
                    JumpPos = basePos + (newPos.Normalized() * (590));
                }
                else
                {
                    JumpPos = basePos + (newPos.Normalized() * (this.Player.Distance(pos)));
                }
            }
            if (JumpPos != new Vector2() && reCheckWard)
            {
                reCheckWard = false;
                LeagueSharp.Common.Utility.DelayAction.Add(
                    20,
                    () =>
                        {
                            if (JumpPos != new Vector2())
                            {
                                JumpPos = new Vector2();
                                reCheckWard = true;
                            }
                        });
            }
            if (m2M)
            {
                this.Orbwalk(pos);
            }
            if (!spells[Spells.E].IsReady() || reqinMaxRange && this.Player.Distance(pos) > spells[Spells.E].Range)
            {
                return;
            }

            if (minions || champions)
            {
                if (champions)
                {
                    var champs = (from champ in ObjectManager.Get<AIHeroClient>()
                                  where
                                      champ.IsAlly && champ.Distance(this.Player) < spells[Spells.E].Range
                                      && champ.Distance(pos) < 200 && !champ.IsMe
                                  select champ).ToList();
                    if (champs.Count > 0 && spells[Spells.E].IsReady())
                    {
                        if (500 >= Environment.TickCount - wcasttime || !spells[Spells.E].IsReady())
                        {
                            return;
                        }

                        CastEWard(champs[0]);
                        return;
                    }
                }
                if (minions)
                {
                    var minion2 = (from minion in ObjectManager.Get<Obj_AI_Minion>()
                                   where
                                       minion.IsAlly && minion.Distance(this.Player) < spells[Spells.E].Range
                                       && minion.Distance(pos) < 200 && !minion.Name.ToLower().Contains("ward")
                                   select minion).ToList();
                    if (minion2.Count > 0)
                    {
                        if (500 >= Environment.TickCount - wcasttime || !spells[Spells.E].IsReady())
                        {
                            return;
                        }

                        CastEWard(minion2[0]);
                        return;
                    }
                }
            }

            var isWard = false;
            foreach (var ward in ObjectManager.Get<Obj_AI_Base>())
            {
                if (ward.IsAlly && ward.Name.ToLower().Contains("ward") && ward.Distance(JumpPos) < 200)
                {
                    isWard = true;
                    if (500 >= Environment.TickCount - wcasttime || !spells[Spells.E].IsReady())
                    {
                        return;
                    }

                    CastEWard(ward);
                    wcasttime = Environment.TickCount;
                }
            }

            if (!isWard && castWardAgain)
            {
                var ward = this.FindBestWardItem();
                if (ward == null || !spells[Spells.E].IsReady())
                {
                    return;
                }

                this.Player.Spellbook.CastSpell(ward.SpellSlot, JumpPos.To3D());
                lastWardPos = JumpPos.To3D();
            }
        }

        private void WardjumpToMouse()
        {
            this.WardJump(
                Game.CursorPos,
                this.Menu.Item("ElEasy.Wardjump.Mouse").IsActive(),
                false,
                false,
                this.Menu.Item("ElEasy.Wardjump.Minions").IsActive(),
                this.Menu.Item("ElEasy.Wardjump.Champions").IsActive());
        }

        #endregion
    }
}