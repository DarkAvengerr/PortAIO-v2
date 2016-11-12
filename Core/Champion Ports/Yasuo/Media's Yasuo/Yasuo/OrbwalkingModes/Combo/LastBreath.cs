using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.OrbwalkingModes.Combo
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::YasuoMedia.CommonEx;
    using global::YasuoMedia.CommonEx.Classes;
    using global::YasuoMedia.CommonEx.Extensions;
    using global::YasuoMedia.CommonEx.Menu;
    using global::YasuoMedia.CommonEx.Menu.Presets;
    using global::YasuoMedia.CommonEx.Utility;
    using global::YasuoMedia.Yasuo.LogicProvider;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.Combo;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class LastBreath : FeatureChild<Combo>
    {
        #region Fields

        /// <summary>
        ///     The blacklist
        /// </summary>
        public BlacklistMenu BlacklistMenu;

        /// <summary>
        ///     The possible executions
        /// </summary>
        private List<CommonEx.Objects.LastBreath> executions = new List<CommonEx.Objects.LastBreath>();

        /// <summary>
        ///     The R logicprovider
        /// </summary>
        private LastBreathLogicProvider provider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastBreath" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public LastBreath(Combo parent)
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
        public override string Name => "(R) Last Breath";

        #endregion

        #region Properties

        /// <summary>
        ///     The valid execution
        /// </summary>
        private CommonEx.Objects.LastBreath ValidExecution { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUpdate(EventArgs args)
        {
            this.SoftReset();

            if (GlobalVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !GlobalVariables.Spells[SpellSlot.R].IsReady() || !this.GoodCircumstances())
            {
                return;
            }

            this.BuildExecutions();

            this.ValidateExecutions();

            this.DecideAndExecute();
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
            this.provider = new LastBreathLogicProvider();

            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            this.BlacklistMenu = new BlacklistMenu(this.Menu, "Blacklist");

            var menuGenerator = new MenuGenerator(new LastBreathMenu(this.Menu));

            menuGenerator.Generate();

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        private void BuildExecutions()
        {
            foreach (var hero in HeroManager.Enemies.Where(x => ((Obj_AI_Base)x).IsAirbone()))
            {
                var execution = new CommonEx.Objects.LastBreath(hero);

                if (!this.executions.Contains(execution))
                {
                    this.executions.Add(execution);
                }
            }
        }

        /// <summary>
        ///     Decides for an execution and executes.
        /// </summary>
        private void DecideAndExecute()
        {
            var advanced = this.Menu.SubMenu(this.Menu.Name + "Advanced");

            if (!this.executions.Any())
            {
                return;
            }

            // TODO: ADD safetyvalue/dangervalue auto mode
            switch (advanced.Item(advanced.Name + "EvaluationLogic").GetValue<StringList>().SelectedIndex)
            {
                // Damage
                case 0:
                    this.ValidExecution = this.executions.MaxOrDefault(x => x.DamageDealt);
                    break;
                // Count
                case 1:
                    this.ValidExecution = this.executions.MaxOrDefault(x => x.AffectedEnemies.Count);
                    break;
                // Priority
                case 2:
                    this.ValidExecution = this.executions.MaxOrDefault(x => x.Priority);
                    break;
                // Auto
                case 3:
                    break;
            }

            if (this.ValidExecution != null)
            {
                this.Execute(this.ValidExecution);
            }
        }

        /// <summary>
        ///     Executes the specified execution.
        /// </summary>
        /// <param name="execution">The execution.</param>
        private void Execute(CommonEx.Objects.LastBreath execution)
        {
            if (this.provider.ShouldCastNow(execution, SweepingBlade.PathBaseCopy))
            {
                GlobalVariables.CastManager.Queque.Enqueue(1,
                    () => GlobalVariables.Spells[SpellSlot.R].CastOnUnit(execution.Target));
            }
        }

        /// <summary>
        ///     Checks the circumstances.
        /// </summary>
        /// <returns></returns>
        private bool GoodCircumstances()
        {
            if (GlobalVariables.Player.HealthPercent
                < this.Menu.Item(this.Menu.Name + "MinPlayerHealth").GetValue<Slider>().Value)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Resets the properties
        /// </summary>
        private void SoftReset()
        {
            if (!this.executions.Any() || this.ValidExecution == null || !this.ValidExecution.IsValid())
            {
                return;
            }

            this.executions = new List<CommonEx.Objects.LastBreath>();
            this.ValidExecution = new CommonEx.Objects.LastBreath();
        }

        /// <summary>
        ///     Validates the executions.
        /// </summary>
        private void ValidateExecutions()
        {
            var advanced = this.Menu.SubMenu(this.Menu.Name + "Advanced");

            if (!this.executions.Any())
            {
                return;
            }

            foreach (var execution in this.executions.ToList())
            {
                // Invalid
                if (!execution.IsValid())
                {
                    this.executions.Remove(execution);
                }

                // Count
                if (execution.AffectedEnemies.Count
                    < this.Menu.Item(this.Menu.Name + "MinHitAOE").GetValue<Slider>().Value)
                {
                    this.executions.Remove(execution);
                }

                // Mean Health
                if ((execution.AffectedEnemies.Sum(x => x.HealthPercent) / execution.AffectedEnemies.Count)
                    > this.Menu.Item(this.Menu.Name + "MaxTargetsMeanHealth").GetValue<Slider>().Value)
                {
                    this.executions.Remove(execution);
                }

                // Max Health Percentage Difference
                if ((execution.AffectedEnemies.Sum(x => x.HealthPercent) / execution.AffectedEnemies.Count)
                    - GlobalVariables.Player.HealthPercent
                    > advanced.Item(advanced.Name + "MaxHealthPercDifference").GetValue<Slider>().Value)
                {
                    this.executions.Remove(execution);
                }

                // Overkill
                if (advanced.Item(advanced.Name + "OverkillCheck").GetValue<bool>() && execution.IsOverkill)
                {
                    this.executions.Remove(execution);
                }
            }
        }

        #endregion
    }
}