using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.OrbwalkingModes.LastHit
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::YasuoMedia.CommonEx.Classes;
    using global::YasuoMedia.CommonEx.Extensions;
    using global::YasuoMedia.CommonEx.Menu;
    using global::YasuoMedia.CommonEx.Utility;
    using global::YasuoMedia.Yasuo.LogicProvider;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.LastHit;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SebbyLib;

    using SharpDX;

    using HealthPrediction = SebbyLib.HealthPrediction;
    using Orbwalking = LeagueSharp.Common.Orbwalking;

    #endregion

    internal class SteelTempest : FeatureChild<LastHit>
    {
        #region Fields

        /// <summary>
        ///     The minions
        /// </summary>
        protected List<Obj_AI_Base> Units;

        /// <summary>
        ///     The good circumstances
        /// </summary>
        private bool goodCircumstances;

        /// <summary>
        ///     The Q logic provider
        /// </summary>
        private SteelTempestLogicProvider providerQ;

        /// <summary>
        ///     The Turret logic provider
        /// </summary>
        private TurretLogicProvider providerTurret;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SteelTempest" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public SteelTempest(LastHit parent)
            : base(parent)
        {
            this.OnLoad();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name => "(Q) Steel Tempest";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUpdate(EventArgs args)
        {
            this.SoftReset();

            if (GlobalVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit
                && GlobalVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                && GlobalVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed
                || !GlobalVariables.Spells[SpellSlot.Q].IsReady() || GlobalVariables.Player.IsDashing()
                || GlobalVariables.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            this.GetUnits();

            this.ValidateUnits();

            this.CheckCircumstances();

            if (!this.goodCircumstances)
            {
                return;
            }

            this.LogicLastHit();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable()
        {
            Events.OnUpdate -= this.OnUpdate;
            base.OnDisable();
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        protected override void OnEnable()
        {
            Events.OnUpdate += this.OnUpdate;
            base.OnEnable();
        }

        /// <summary>
        ///     Called when [initialize].
        /// </summary>
        protected override void OnInitialize()
        {
            this.providerQ = new SteelTempestLogicProvider();

            this.providerTurret = new TurretLogicProvider();

            this.Units = new List<Obj_AI_Base>();

            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            var menuGenerator = new MenuGenerator(new SteelTempestMenu(this.Menu));

            menuGenerator.Generate();

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        /// <summary>
        ///     Executes on the specified unit.
        /// </summary>
        /// <param name="unit">The unit.</param>
        private static void Execute(Obj_AI_Base unit)
        {
            if (!unit.IsValid) return;

            Execute(unit.ServerPosition.To2D());
        }

        /// <summary>
        ///     Executes on the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        private static void Execute(Vector2 position)
        {
            if (!position.IsValid()) return;

            GlobalVariables.CastManager.Queque.Enqueue(2, () => GlobalVariables.Spells[SpellSlot.Q].Cast(position));
        }

        /// <summary>
        ///     Checks the circumstances.
        /// </summary>
        private void CheckCircumstances()
        {
            if (this.providerQ.HasQ3()
                && GlobalVariables.Player.CountEnemiesInRange(
                    this.Menu.Item(this.Name + "NoQ3Range").GetValue<Slider>().Value)
                >= this.Menu.Item(this.Name + "NoQ3Count").GetValue<Slider>().Value)
            {
                this.goodCircumstances = false;
                return;
            }

            var settingsAdv = this.Menu.SubMenu(this.Name + "Advanced");

            if (!this.Units.Any())
            {
                this.goodCircumstances = false;
                return;
            }

            if (settingsAdv.Item(settingsAdv.Name + "TurretCheck").GetValue<bool>())
            {
                if (!this.providerTurret.IsSafePosition(GlobalVariables.Player.ServerPosition))
                {
                    this.goodCircumstances = false;
                }
            }
        }

        /// <summary>
        ///     Gets the minions and targets.
        /// </summary>
        private void GetUnits()
        {
            this.Units = Cache.GetMinions(
                GlobalVariables.Player.ServerPosition,
                GlobalVariables.Spells[SpellSlot.Q].Range,
                MinionTeam.NotAlly);
        }

        /// <summary>
        ///     LastHits with ranged/stacked Q
        /// </summary>
        private void LogicLastHit()
        {
            var validunits =
                this.Units.Where(x => x.IsValidTarget(GlobalVariables.Spells[SpellSlot.Q].Range - x.BoundingRadius / 2))
                    .ToList();

            var farmlocation = MinionManager.GetBestLineFarmLocation(
                validunits.ToVector3S().To2D(),
                GlobalVariables.Spells[SpellSlot.Q].Width,
                GlobalVariables.Spells[SpellSlot.Q].Range);

            var unit = validunits.MaxOrDefault(x => x.FlatGoldRewardMod);

            if (unit != null && this.providerQ.HasQ3()
                && GlobalVariables.Player.Distance(unit) < GlobalVariables.Player.AttackRange + unit.Health)
            {
                return;
            }

            if (!farmlocation.Position.IsValid())
            {
                if (unit != null)
                {
                    Execute(unit);
                }
            }
            else
            {
                Execute(farmlocation.Position);
            }
        }

        /// <summary>
        ///     Resets some fields
        /// </summary>
        private void SoftReset()
        {
            this.goodCircumstances = true;

            this.Units = new List<Obj_AI_Base>();
        }

        /// <summary>
        ///     Validates the minions.
        /// </summary>
        private void ValidateUnits()
        {
            if (!this.Units.Any())
            {
                return;
            }

            foreach (var unit in this.Units.ToList())
            {
                var predictedHealth = HealthPrediction.GetHealthPrediction(unit, (int)this.providerQ.TravelTime(unit));

                if (predictedHealth <= 0 || predictedHealth > this.providerQ.GetDamage(unit))
                {
                    this.Units.Remove(unit);
                }
            }
        }

        #endregion
    }
}