using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Base.Modules.Assembly
{
    #region Using Directives

    using System;
    using System.Linq;

    using global::YasuoMedia.CommonEx.Classes;
    using global::YasuoMedia.CommonEx.Extensions;
    using global::YasuoMedia.CommonEx.Utility;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class SpellManager : FeatureChild<Assembly>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CastManager" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public SpellManager(Assembly parent)
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
        public override string Name => "Spell Manager";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Method that adjusts the spells to the current circumstances
        /// </summary>
        public void SetSpells()
        {
            if (GlobalVariables.Spells == null)
            {
                return;
            }

            if (GlobalVariables.Player.IsDashing())
            {
                GlobalVariables.Spells[SpellSlot.Q].SetSkillshot(
                    GlobalVariables.providerQ.GetQDelay,
                    this.Menu.Item(this.Name + "QE.Width").GetValue<Slider>().Value,
                    this.Menu.Item(this.Name + "QE.Costs").GetValue<Slider>().Value,
                    false,
                    SkillshotType.SkillshotCircle);
                GlobalVariables.Spells[SpellSlot.Q].Range =
                    this.Menu.Item(this.Name + "QE.Range").GetValue<Slider>().Value;
                GlobalVariables.Spells[SpellSlot.Q].MinHitChance = HitChance.High;
            }
            else
            {
                if (GlobalVariables.Player.HasQ3())
                {
                    GlobalVariables.Spells[SpellSlot.Q].SetSkillshot(
                        GlobalVariables.providerQ.GetQDelay,
                        this.Menu.Item(this.Name + "Q3.Width").GetValue<Slider>().Value,
                        this.Menu.Item(this.Name + "Q3.Costs").GetValue<Slider>().Value,
                        false,
                        SkillshotType.SkillshotLine);
                    GlobalVariables.Spells[SpellSlot.Q].Range =
                        this.Menu.Item(this.Name + "Q3.Range").GetValue<Slider>().Value;
                    GlobalVariables.Spells[SpellSlot.Q].MinHitChance = HitChance.VeryHigh;
                }
                else
                {
                    GlobalVariables.Spells[SpellSlot.Q].SetSkillshot(
                        GlobalVariables.providerQ.GetQDelay,
                        this.Menu.Item(this.Name + "Q.Width").GetValue<Slider>().Value,
                        this.Menu.Item(this.Name + "Q.Costs").GetValue<Slider>().Value,
                        false,
                        SkillshotType.SkillshotLine);
                    GlobalVariables.Spells[SpellSlot.Q].Range =
                        this.Menu.Item(this.Name + "Q.Range").GetValue<Slider>().Value;
                    GlobalVariables.Spells[SpellSlot.Q].MinHitChance = HitChance.VeryHigh;
                }
            }

            GlobalVariables.Spells[SpellSlot.W].SetSkillshot(
                0,
                this.Menu.Item(this.Name + "W.BaseWidth").GetValue<Slider>().Value
                + (GlobalVariables.Spells[SpellSlot.W].Level
                   * this.Menu.Item(this.Name + "W.Width").GetValue<Slider>().Value),
                this.Menu.Item(this.Name + "W.Costs").GetValue<Slider>().Value,
                false,
                SkillshotType.SkillshotCone);
            GlobalVariables.Spells[SpellSlot.W].Range = this.Menu.Item(this.Name + "W.Range").GetValue<Slider>().Value;

            GlobalVariables.Spells[SpellSlot.E].SetTargetted(0, 1025);
            GlobalVariables.Spells[SpellSlot.E].Speed = this.Menu.Item(this.Name + "E.Costs").GetValue<Slider>().Value;
            GlobalVariables.Spells[SpellSlot.E].Range = this.Menu.Item(this.Name + "E.Range").GetValue<Slider>().Value;

            GlobalVariables.Spells[SpellSlot.R].SetTargetted(0, float.MaxValue);
            GlobalVariables.Spells[SpellSlot.R].Range = this.Menu.Item(this.Name + "R.Range").GetValue<Slider>().Value;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable()
        {
            Events.OnPreUpdate -= this.OnPreUpdate;
            base.OnDisable();
        }

        /// <summary>
        ///     Called when [enable].
        /// </summary>
        protected override void OnEnable()
        {
            Events.OnPreUpdate += this.OnPreUpdate;
            base.OnEnable();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);

            // TODO: Add slider to adjust spells in lenght, speed etc.

            this.Menu.AddItem(
                new MenuItem(this.Name + "Q.Range", "Q (Non-Stacked) Range: ").SetValue(new Slider(475, 0, 2000)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "Q.Width", "Q (Non-Stacked) Width: ").SetValue(new Slider(20, 0, 2000)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "Q.Costs", "Q (Non-Stacked) Costs: ").SetValue(new Slider(10000, 0, 10000)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "Q3.Range", "Q (Stacked) Range: ").SetValue(new Slider(950, 0, 2000)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "Q3.Width", "Q (Stacked) Width: ").SetValue(new Slider(90, 0, 2000)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "Q3.Costs", "Q (Stacked) Costs: ").SetValue(new Slider(1250, 0, 2000)));

            this.Menu.AddItem(new MenuItem(this.Name + "QE.Range", "Q (E) Range: ").SetValue(new Slider(0, 0, 2000)));

            this.Menu.AddItem(new MenuItem(this.Name + "QE.Width", "Q (E) Width: ").SetValue(new Slider(350, 0, 2000)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "QE.Costs", "Q (E) Costs: ").SetValue(new Slider(5000, 0, 10000)));

            this.Menu.AddItem(new MenuItem(this.Name + "W.Range", "W Range: ").SetValue(new Slider(0, 0, 2000)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "W.BaseWidth", "W (Base) Width: ").SetValue(new Slider(250, 0, 2000)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "W.Width", "W (Per Level) Width: ").SetValue(new Slider(50, 0, 2000)));

            this.Menu.AddItem(new MenuItem(this.Name + "W.Costs", "W Costs: ").SetValue(new Slider(400, 0, 10000)));

            this.Menu.AddItem(new MenuItem(this.Name + "E.Range", "E Range: ").SetValue(new Slider(475, 0, 2000)));

            this.Menu.AddItem(new MenuItem(this.Name + "E.Costs", "E Costs: ").SetValue(new Slider(1025, 0, 10000)));

            this.Menu.AddItem(new MenuItem(this.Name + "R.Range", "R Range: ").SetValue(new Slider(900, 0, 2000)));

            foreach (var item in
                this.Menu.Items.Where(x => x.Name != (this.Name + "Enabled")))
            {
                item.DontSave();
            }

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        /// <summary>
        ///     Raises the <see cref="E:PreUpdate" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnPreUpdate(EventArgs args)
        {
            this.SetSpells();
        }

        #endregion
    }
}