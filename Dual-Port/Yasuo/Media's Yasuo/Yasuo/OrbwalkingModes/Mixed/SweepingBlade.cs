using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.OrbwalkingModes.Mixed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommonEx;
    using CommonEx.Classes;
    using CommonEx.Extensions;
    using CommonEx.Menu;
    using CommonEx.Menu.Presets;

    using global::YasuoMedia.CommonEx.Utility;
    using global::YasuoMedia.Yasuo.LogicProvider;
    using global::YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.Mixed;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Dash = CommonEx.Objects.Dash;

    internal class SweepingBlade : FeatureChild<Mixed>
    {
        #region Fields

        /// <summary>
        ///     The champion slider menu
        /// </summary>
        public ChampionSliderMenu ChampionSliderMenu;

        /// <summary>
        ///     The provider e
        /// </summary>
        private SweepingBladeLogicProvider providerE;

        /// <summary>
        ///     The provider q
        /// </summary>
        private SteelTempestLogicProvider providerQ;

        /// <summary>
        ///     The provider turret
        /// </summary>
        private TurretLogicProvider providerTurret;

        /// <summary>
        ///     The possible dashes
        /// </summary>
        protected List<Dash> Dashes = new List<Dash>();

        /// <summary>
        ///     The possible minions
        /// </summary>
        protected List<Obj_AI_Base> Units = new List<Obj_AI_Base>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SweepingBlade" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public SweepingBlade(Mixed parent)
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
        public override string Name => "(E) Sweeping Blade";

        #endregion

        #region Public Methods and Operators

        // TODO: PRIORITY LOW > Some more settings but that should work for now
        /// <summary>
        ///     Executes the LastHit logic.
        /// </summary>
        public void LogicLastHit()
        {
            if (!this.Dashes.Any())
            {
                return;
            }

            var dash = this.Dashes.MaxOrDefault(x => x.EndPosition.CountMinionsInRange(GlobalVariables.Spells[SpellSlot.Q].Range));

            Execute(dash.Unit);
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUpdate(EventArgs args)
        {
            this.SoftReset();

            if (GlobalVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit
                || !GlobalVariables.Spells[SpellSlot.E].IsReady()
                || GlobalVariables.Player.IsDashing()
                || GlobalVariables.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            this.GetMinions();

            this.BuildDashes();

            this.ValidateDashes();

            this.LogicLastHit();
        }

        private void SoftReset()
        {
            this.Dashes = new List<Dash>();
            this.Units = new List<Obj_AI_Base>();
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
            this.providerE = new SweepingBladeLogicProvider();
            this.providerQ = new SteelTempestLogicProvider();
            this.providerTurret = new TurretLogicProvider();

            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);


            this.ChampionSliderMenu = new ChampionSliderMenu(this.Menu, "Min Distance to Enemy");

            var menuGenerator = new MenuGenerator(new SweepingBladeMenu(this.Menu));

            menuGenerator.Generate();
            

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

            this.Parent.Menu.AddSubMenu(this.Menu);
        }

        /// <summary>
        ///     Gets the minions.
        /// </summary>
        private void GetMinions()
        {
            this.Units = SebbyLib.Cache.GetMinions(GlobalVariables.Player.ServerPosition, GlobalVariables.Spells[SpellSlot.E].Range, MinionTeam.NotAlly);
        }

        /// <summary>
        ///     Builds the dashes.
        /// </summary>
        private void BuildDashes()
        {

        }

        /// <summary>
        ///     Executes on the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        private static void Execute(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                GlobalVariables.Spells[SpellSlot.E].CastOnUnit(target);
            }
        }

        /// <summary>
        ///     Validates the dashes recording to the menu settings.
        /// </summary>
        private void ValidateDashes()
        {
            if (this.Dashes == null || !this.Dashes.Any())
            {
                return;
            }

            foreach (var dash in this.Dashes.ToList())
            {
                var remove = false;

                if (this.Menu.Item(this.Name + "NoSkillshot").GetValue<bool>())
                {
                    if (dash.InSkillshot)
                    {
                        remove = true;
                    }
                }

                if (this.Menu.Item(this.Name + "NoTurret").GetValue<bool>())
                {
                    if (!this.providerTurret.IsSafePosition(dash.EndPosition))
                    {
                        remove = true;
                    }
                }

                if (this.Menu.Item(this.Name + "NoEnemy").GetValue<bool>())
                {
                    if (this.ChampionSliderMenu.Values.Any(
                            entry => dash.EndPosition.Distance(entry.Key.ServerPosition) > entry.Value))
                    {
                        remove = true;
                    }
                }

                if (this.Menu.Item(this.Name + "NoWallDash").GetValue<bool>())
                {
                    if (dash.IsWallDash)
                    {
                        remove = true;
                    }
                }

                switch (this.Menu.Item(this.Name + "ModeTarget").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (dash.EndPosition.Distance(Game.CursorPos) >= dash.StartPosition.Distance(Game.CursorPos))
                        {
                            remove = true;
                        }
                        break;
                }

                var range = GlobalVariables.Spells[SpellSlot.Q].IsReady((int)this.providerE.Speed() - 100) ? GlobalVariables.Spells[SpellSlot.Q].Range : GlobalVariables.Player.AttackRange;

                if (dash.StartPosition.CountMinionsInRange(range) > 1)
                {
                    remove = true;
                }

                if (remove)
                {
                    this.Dashes.Remove(dash);
                }
            }
        }

        #endregion
    }
}