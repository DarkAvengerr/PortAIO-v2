namespace ElEasy.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using EloBuddy;

    internal class Nasus : IPlugin
    {
        #region Static Fields

        public static int Sheen = 3057, Iceborn = 3025;

        private static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                       {
                                                                           {
                                                                               Spells.Q,
                                                                               new Spell(
                                                                               SpellSlot.Q,
                                                                               Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                                                                           },
                                                                           { Spells.W, new Spell(SpellSlot.W, 600) },
                                                                           { Spells.E, new Spell(SpellSlot.E, 650) },
                                                                           { Spells.R, new Spell(SpellSlot.R) }
                                                                       };

        private static SpellSlot Ignite;

        private static Orbwalking.Orbwalker Orbwalker;

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
            this.Menu = new Menu("ElNasus", "ElNasus");
            {
                var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                this.Menu.AddSubMenu(orbwalkerMenu);

                var targetSelector = new Menu("Target Selector", "TargetSelector");
                TargetSelector.AddToMenu(targetSelector);
                this.Menu.AddSubMenu(targetSelector);

                var comboMenu = new Menu("Combo", "Combo");
                {
                    comboMenu.AddItem(new MenuItem("ElEasy.Nasus.Combo.Q", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Nasus.Combo.W", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Nasus.Combo.E", "Use E").SetValue(true));
                    comboMenu.AddItem(new MenuItem("ElEasy.Nasus.Combo.R", "Use R").SetValue(true));
                    comboMenu.AddItem(
                        new MenuItem("ElEasy.Nasus.Combo.Count.R", "Minimum champions in range for R").SetValue(
                            new Slider(2, 1, 5)));
                    comboMenu.AddItem(
                        new MenuItem("ElEasy.Nasus.Combo.HP", "Minimum HP for R").SetValue(new Slider(55)));
                    comboMenu.AddItem(new MenuItem("ElEasy.Nasus.Combo.Ignite", "Use Ignite").SetValue(true));
                }

                this.Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "Harass");
                {
                    harassMenu.AddItem(new MenuItem("ElEasy.Nasus.Harass.E", "Use E").SetValue(true));
                    harassMenu.AddItem(
                        new MenuItem("ElEasy.Nasus.Harass.Player.Mana", "Minimum Mana").SetValue(new Slider(55)));
                }

                this.Menu.AddSubMenu(harassMenu);

                var clearMenu = new Menu("Clear", "Clear");
                {
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Nasus.LaneClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Laneclear")
                        .AddItem(new MenuItem("ElEasy.Nasus.LaneClear.E", "Use E").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Nasus.JungleClear.Q", "Use Q").SetValue(true));
                    clearMenu.SubMenu("Jungleclear")
                        .AddItem(new MenuItem("ElEasy.Nasus.JungleClear.E", "Use E").SetValue(true));
                }

                this.Menu.AddSubMenu(clearMenu);

                var lasthitMenu = new Menu("Lasthit", "Lasthit");
                {
                    lasthitMenu.AddItem(
                        new MenuItem("ElEasy.Nasus.Lasthit.Activated", "Auto Lasthit").SetValue(
                            new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle)));
                    lasthitMenu.AddItem(
                        new MenuItem("ElEasy.Nasus.Lasthitrange", "Auto last hit range").SetValue(new Slider(100, 100, 500)));
                }

               
                this.Menu.AddSubMenu(lasthitMenu);

                var miscellaneousMenu = new Menu("Miscellaneous", "Miscellaneous");
                {
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Nasus.Draw.off", "Turn drawings off").SetValue(true));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Nasus.Draw.LastHitRange", "Draw Last hit range").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Nasus.Draw.W", "Draw W").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Nasus.Draw.E", "Draw E").SetValue(new Circle()));
                    miscellaneousMenu.AddItem(new MenuItem("ElEasy.Nasus.Draw.Text", "Draw text").SetValue(true));
                    miscellaneousMenu.AddItem(
                        new MenuItem("ElEasy.Nasus.Draw.MinionHelper", "Draw killable minions").SetValue(true));
                }

                this.Menu.AddSubMenu(miscellaneousMenu);
            }
            rootMenu.AddSubMenu(this.Menu);
        }

        public void Load()
        {
            Console.WriteLine("Loaded Nasus");
            Ignite = this.Player.GetSpellSlot("summonerdot");
            spells[Spells.E].SetSkillshot(
                spells[Spells.E].Instance.SData.SpellCastTime,
                spells[Spells.E].Instance.SData.LineWidth,
                spells[Spells.E].Instance.SData.MissileSpeed,
                false,
                SkillshotType.SkillshotCircle);

            Game.OnUpdate += this.OnUpdate;
            Drawing.OnDraw += this.OnDraw;
            Obj_AI_Base.OnSpellCast += this.Obj_AI_Base_OnProcessSpellCast;
        }

        #endregion

        #region Methods

        private void AutoLastHit()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || this.Player.IsRecalling() || !spells[Spells.Q].IsReady())
            {
                return;
            }

            var minions = MinionManager.GetMinions(
                this.Player.Position,
                spells[Spells.E].Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth).OrderByDescending(m => this.GetBonusDmg(m) > m.Health);

            foreach (var minion in minions)
            {
                if (this.GetBonusDmg(minion) > minion.Health
                    && Vector3.Distance(ObjectManager.Player.ServerPosition, minion.Position)
                    < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + this.Menu.Item("ElEasy.Nasus.Lasthitrange").GetValue<Slider>().Value)
                {
                    Orbwalker.SetAttack(false);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                    Orbwalker.SetAttack(true);
                    break;
                }
            }
        }

        private double GetBonusDmg(Obj_AI_Base target)
        {
            double dmgItem = 0;
            if (Items.HasItem(Sheen) && (Items.CanUseItem(Sheen) || this.Player.HasBuff("sheen"))
                && this.Player.BaseAttackDamage > dmgItem)
            {
                dmgItem = this.Player.GetAutoAttackDamage(target);
            }

            if (Items.HasItem(Iceborn) && (Items.CanUseItem(Iceborn) || this.Player.HasBuff("itemfrozenfist"))
                && this.Player.BaseAttackDamage * 1.25 > dmgItem)
            {
                dmgItem = this.Player.GetAutoAttackDamage(target) * 1.25;
            }

            return spells[Spells.Q].GetDamage(target) + this.Player.GetAutoAttackDamage(target) + dmgItem;
        }

        private float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || this.Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)this.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private void Jungleclear()
        {
            var useQ = this.Menu.Item("ElEasy.Nasus.JungleClear.Q").IsActive();
            var useE = this.Menu.Item("ElEasy.Nasus.JungleClear.E").IsActive();

            var minions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                spells[Spells.E].Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minions.Count <= 0)
            {
                return;
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                if (minions.Find(x => x.Health >= spells[Spells.Q].GetDamage(x) && x.IsValidTarget()) != null)
                {
                    spells[Spells.Q].Cast();
                }
            }

            if (useE && spells[Spells.E].IsReady())
            {
                if (minions.Count > 1)
                {
                    var farmLocation = spells[Spells.E].GetCircularFarmLocation(minions);
                    spells[Spells.E].Cast(farmLocation.Position);
                }
            }
        }

        private void Laneclear()
        {
            var useQ = this.Menu.Item("ElEasy.Nasus.LaneClear.Q").IsActive();
            var useE = this.Menu.Item("ElEasy.Nasus.LaneClear.E").IsActive();

            var minions = MinionManager.GetMinions(this.Player.ServerPosition, spells[Spells.E].Range);
            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spells[Spells.E].Range);
                {
                    foreach (var minion in
                        allMinions.Where(
                            minion => minion.Health <= ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q)))
                    {
                        if (minion.IsValidTarget())
                        {
                            spells[Spells.Q].CastOnUnit(minion);
                            return;
                        }
                    }
                }
            }

            if (useE && spells[Spells.E].IsReady())
            {
                if (minions.Count > 1)
                {
                    var farmLocation = spells[Spells.E].GetCircularFarmLocation(minions);
                    spells[Spells.E].Cast(farmLocation.Position);
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!args.SData.Name.ToLower().Contains("attack") || !sender.IsMe)
            {
                return;
            }

            var unit = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)args.Target.NetworkId);
            if ((this.GetBonusDmg(unit) > unit.Health))
            {
                spells[Spells.Q].Cast();
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.W].Range, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            var useQ = this.Menu.Item("ElEasy.Nasus.Combo.Q").IsActive();
            var useW = this.Menu.Item("ElEasy.Nasus.Combo.W").IsActive();
            var useE = this.Menu.Item("ElEasy.Nasus.Combo.E").IsActive();
            var useR = this.Menu.Item("ElEasy.Nasus.Combo.R").IsActive();
            var useI = this.Menu.Item("ElEasy.Nasus.Combo.Ignite").IsActive();
            var countEnemies = this.Menu.Item("ElEasy.Nasus.Combo.Count.R").GetValue<Slider>().Value;
            var playerHp = this.Menu.Item("ElEasy.Nasus.Combo.HP").GetValue<Slider>().Value;

            if (useW && spells[Spells.W].IsReady())
            {
                spells[Spells.W].Cast(target);
            }

            if (useQ && spells[Spells.Q].IsReady())
            {
                spells[Spells.Q].Cast();
            }


            if (useE && spells[Spells.E].IsReady() && target.IsValidTarget())
            {
                var pred = spells[Spells.E].GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {
                    spells[Spells.E].Cast(pred.CastPosition);
                }
            }

            if (useR && spells[Spells.R].IsReady()
                && this.Player.CountEnemiesInRange(spells[Spells.W].Range) >= countEnemies
                || (this.Player.Health / this.Player.MaxHealth) * 100 <= playerHp)
            {
                spells[Spells.R].CastOnUnit(this.Player);
            }

            if (this.Player.Distance(target) <= 600 && this.IgniteDamage(target) >= target.Health && useI)
            {
                this.Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private void OnDraw(EventArgs args)
        {
            var drawOff = this.Menu.Item("ElEasy.Nasus.Draw.off").IsActive();
            var drawW = this.Menu.Item("ElEasy.Nasus.Draw.W").GetValue<Circle>();
            var drawE = this.Menu.Item("ElEasy.Nasus.Draw.E").GetValue<Circle>();
            var drawText = this.Menu.Item("ElEasy.Nasus.Draw.Text").IsActive();
            var rBool = this.Menu.Item("ElEasy.Nasus.Lasthit.Activated").GetValue<KeyBind>().Active;
            var helper = this.Menu.Item("ElEasy.Nasus.Draw.MinionHelper").IsActive();
            var drawLastHit = this.Menu.Item("ElEasy.Nasus.Draw.LastHitRange").GetValue<Circle>();


            

            var playerPos = Drawing.WorldToScreen(this.Player.Position);

            if (drawOff)
            {
                return;
            }

            if (drawLastHit.Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + this.Menu.Item("ElEasy.Nasus.Lasthitrange").GetValue<Slider>().Value, Color.Orange);
                }
            }

            if (drawE.Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawW.Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(this.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawText)
            {
                Drawing.DrawText(
                    playerPos.X - 70,
                    playerPos.Y + 40,
                    (rBool ? Color.Green : Color.Red),
                    (rBool ? "Auto lasthit enabled" : "Auto lasthit disabled"));
            }

            if (helper)
            {
                var minions = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    spells[Spells.E].Range,
                    MinionTypes.All,
                    MinionTeam.NotAlly);
                foreach (var minion in minions)
                {
                    if (minion != null)
                    {
                        if ((this.GetBonusDmg(minion) > minion.Health))
                        {
                            Render.Circle.DrawCircle(minion.ServerPosition, minion.BoundingRadius, Color.Green);
                        }
                    }
                }
            }
        }

        private void OnHarass()
        {
            var eTarget = TargetSelector.GetTarget(
                spells[Spells.E].Range + spells[Spells.E].Width,
                TargetSelector.DamageType.Magical);
            if (eTarget == null || !eTarget.IsValid)
            {
                return;
            }

            var useE = this.Menu.Item("ElEasy.Nasus.Harass.E").GetValue<bool>();

            if (useE && spells[Spells.E].IsReady() && eTarget.IsValidTarget() && spells[Spells.E].IsInRange(eTarget))
            {
                var pred = spells[Spells.E].GetPrediction(eTarget).Hitchance;
                if (pred >= HitChance.High)
                {
                    spells[Spells.E].Cast(eTarget);
                }
            }
        }

        private void OnLastHit()
        {
            var minion =
                MinionManager.GetMinions(
                    this.Player.Position,
                    spells[Spells.Q].Range + 100,
                    MinionTypes.All,
                    MinionTeam.NotAlly,
                    MinionOrderTypes.MaxHealth).OrderByDescending(m => this.GetBonusDmg(m) > m.Health).FirstOrDefault();

            if (minion == null)
            {
                return;
            }

            if (this.GetBonusDmg(minion) > minion.Health && spells[Spells.Q].IsReady())
            {
                Orbwalker.SetAttack(false);
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                Orbwalker.SetAttack(true);
            }
        }

        private void OnUpdate(EventArgs args)
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

                case Orbwalking.OrbwalkingMode.LaneClear:
                    this.Laneclear();
                    this.Jungleclear();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    this.OnHarass();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    this.OnLastHit();
                    break;
            }

            if (this.Menu.Item("ElEasy.Nasus.Lasthit.Activated").GetValue<KeyBind>().Active)
            {
                this.AutoLastHit();
            }
        }

        #endregion
    }
}