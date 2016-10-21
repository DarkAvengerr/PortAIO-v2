using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.OrbwalkingModes.LaneClear
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommonEx;
    using CommonEx.Classes;

    using global::YasuoMedia.CommonEx.Utility;
    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Dash = CommonEx.Objects.Dash;

    #endregion

    internal class Eq : FeatureChild<LaneClear>
    {
        #region Fields

        /// <summary>
        ///     The E logic provider
        /// </summary>
        private SweepingBladeLogicProvider providerE;

        /// <summary>
        ///     The Q logic provider
        /// </summary>
        private SteelTempestLogicProvider providerQ;

        /// <summary>
        ///     The Turret logic provider
        /// </summary>
        private TurretLogicProvider providerTurret;

        /// <summary>
        /// The minions
        /// </summary>
        protected List<Obj_AI_Base> Units;

        /// <summary>
        /// The dashes
        /// </summary>
        protected List<Dash> Dashes;

        /// <summary>
        /// The dash
        /// </summary>
        private Dash dash;

        /// <summary>
        /// The good circumstances
        /// </summary>
        private bool validCircumstances;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrbwalkingModes.LastHit.SteelTempest" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public Eq(LaneClear parent)
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
        public override string Name => "EQ-Combo";

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
                || !GlobalVariables.Spells[SpellSlot.Q].IsReady() || !GlobalVariables.Spells[SpellSlot.E].IsReady())
            {
                return;
            }

            if (!GlobalVariables.Player.IsDashing())
            {
                this.GetMinions();

                this.CheckCircumstances();

                if (!this.validCircumstances)
                {
                    return;
                }

                this.BuildDashes();

                this.ValidateDashes();

                this.LogicE();
            }

            this.LogicQ();
        }

        /// <summary>
        ///     Checks the circumstances.
        /// </summary>
        private void CheckCircumstances()
        {
            if (this.providerQ.HasQ3() && this.Menu.Item(this.Name + "OnlyNotStacked").GetValue<bool>())
            {
                this.validCircumstances = false;
                return;
            }

            if (!this.Units.Any())
            {
                this.validCircumstances = false;
                return;
            }

            var settingsAdv = this.Menu.SubMenu(this.Name + "Advanced");

            if (settingsAdv.Item(settingsAdv.Name + "TurretCheck").GetValue<bool>())
            {
                if (!this.providerTurret.IsSafePosition(GlobalVariables.Player.ServerPosition))
                {
                    this.validCircumstances = false;
                    return;
                }
            }
        }

        /// <summary>
        /// Builds the dashes.
        /// </summary>
        private void BuildDashes()
        {
            if (!this.Units.Any())
            {
                return;
            }

            foreach (var minion in this.Units)
            {
                this.Dashes.Add(new Dash(minion));
            }
        }

        /// <summary>
        /// Validates the dashes.
        /// </summary>
        private void ValidateDashes()
        {
            if (!this.Dashes.Any())
            {
                return;
            }

            foreach (var entry in this.Dashes.ToList())
            {
                if (this.Menu.Item(this.Name + "NoTurretDive").GetValue<bool>()
                    && !this.providerTurret.IsSafePosition(entry.EndPosition))
                {
                    this.Dashes.Remove(entry);
                }

                if (this.Menu.Item(this.Name + "MinHitAOE").GetValue<Slider>().Value
                    > entry.MinionsHitCircular.Count(x => x.Health <= this.providerQ.GetDamage(x)))
                {
                    this.Dashes.Remove(entry);
                }

                if (entry.MinionsHitCircular.Contains(entry.Unit) &&
                    this.providerE.GetDamage(entry.Unit) >= entry.Unit.Health * 1.2
                    && entry.MinionsHitCircular.Count == 1)
                {
                    this.Dashes.Remove(entry);
                }
            }
        }

        /// <summary>
        ///     Gets the minions and targets.
        /// </summary>
        private void GetMinions()
        {
            this.Units = SebbyLib.Cache.GetMinions(
                GlobalVariables.Player.ServerPosition,
                GlobalVariables.Spells[SpellSlot.E].Range,
                MinionTeam.NotAlly);
        }

        /// <summary>
        /// Resets some fields
        /// </summary>
        private void SoftReset()
        {
            if (GlobalVariables.Player.IsDashing())
            {
                return;
            }

            this.validCircumstances = true;

            this.Units = new List<Obj_AI_Base>();
            this.Dashes = new List<Dash>();
            this.dash = null;
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
            this.providerE = new SweepingBladeLogicProvider();
            this.providerTurret = new TurretLogicProvider();

            this.Units = new List<Obj_AI_Base>();
            this.Dashes = new List<Dash>();

            base.OnInitialize();
        }

        // TODO: Decomposite
        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);


            this.SetupEqMenu();

            this.SetupAdvancedMenu();

            this.SetupGeneralMenu();


            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        /// <summary>
        /// Logics the e.
        /// </summary>
        private void LogicE()
        {
            if (GlobalVariables.Player.IsDashing() || !this.Dashes.Any())
            {
                return;
            }

            this.dash = this.Dashes.MaxOrDefault(x => x.MinionsHitCircular.Count);

            if (this.dash == null) return;

            GlobalVariables.CastManager.Queque.Enqueue(2, () => GlobalVariables.Spells[SpellSlot.E].CastOnUnit(this.dash.Unit));

            Orbwalking.Attack = false;
        }

        /// <summary>
        ///     LastHits with Q while dashing
        /// </summary>
        private void LogicQ()
        {
            if (!GlobalVariables.Player.IsDashing() || this.dash == null)
            {
                Orbwalking.Attack = true;

                return;
            }

            GlobalVariables.CastManager.Queque.Enqueue(1, () => GlobalVariables.Spells[SpellSlot.Q].Cast(this.dash.Unit.ServerPosition));

            Orbwalking.Attack = true;
        }

        /// <summary>
        ///     Setups the advanced menu.
        /// </summary>
        private void SetupAdvancedMenu()
        {
            var settingsAdv = new Menu("Advanced", this.Name + "Advanced");

            settingsAdv.AddItem(
                new MenuItem(settingsAdv.Name + "TurretCheck", "Check for enemy turret").SetValue(true)
                    .SetTooltip(
                        "if this is enabled, the assembly will try to not use spells inside the enemy turret. But it will use them if the turret is focusing something else!"));

            this.Menu.AddSubMenu(settingsAdv);
        }

        /// <summary>
        ///     Setups the eq menu.
        /// </summary>
        private void SetupEqMenu()
        {
            this.Menu.AddItem(new MenuItem(this.Name + "MinHitAOE", "Min Hit Count").SetValue(new Slider(1, 1, 15)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "OnlyNotStacked", "Don't cast if stacked Q (Q3)").SetValue(true)
                    .SetTooltip("If this is enabled, the assembly won't do EQ if you have stacked/charged Q"));

            this.Menu.AddItem(new MenuItem(this.Name + "NoTurretDive", "Don't dash into turret").SetValue(true));
        }

        /// <summary>
        ///     Setups the general menu.
        /// </summary>
        private void SetupGeneralMenu()
        {
            return;
            this.Menu.AddItem(
                new MenuItem(this.Name + "NoQ3Count", "Don't use Q3 if >= Enemies around").SetValue(
                    new Slider(2, 0, 5)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "NoQ3Range", "Check for enemies in range of").SetValue(
                    new Slider(1000, 0, 5000)));
        }

        #endregion
    }
}