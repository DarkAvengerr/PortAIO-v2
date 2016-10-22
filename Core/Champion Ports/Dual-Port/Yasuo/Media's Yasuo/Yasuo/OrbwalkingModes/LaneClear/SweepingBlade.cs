// TODO: Add new Dash Object to make things easier

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.OrbwalkingModes.LaneClear
{
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
    using global::YasuoMedia.Yasuo.Menu.MenuSets.OrbwalkingModes.LaneClear;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Dash = global::YasuoMedia.CommonEx.Objects.Dash;

    internal class SweepingBlade : FeatureChild<LaneClear>
    {
        #region Fields

        /// <summary>
        ///     The blacklist
        /// </summary>
        public BlacklistMenu BlacklistMenu;

        /// <summary>
        ///     The provider e
        /// </summary>
        private SweepingBladeLogicProvider providerE;

        /// <summary>
        ///     The provider turret
        /// </summary>
        private TurretLogicProvider providerTurret;

        /// <summary>
        /// The units
        /// </summary>
        protected List<Obj_AI_Base> Units;

        /// <summary>
        /// The dashes
        /// </summary>
        protected List<Dash> Dashes; 

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SweepingBlade" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public SweepingBlade(LaneClear parent)
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
            this.providerTurret = new TurretLogicProvider();

            this.Units = new List<Obj_AI_Base>();
            this.Dashes = new List<Dash>();

            base.OnInitialize();
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected sealed override void OnLoad()
        {
            this.Menu = new Menu(this.Name, this.Name);


            this.BlacklistMenu = new BlacklistMenu(this.Menu, "Don't dash into");

            var menuGenerator = new MenuGenerator(new SweepingBladeMenu(this.Menu));

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
            if (unit.IsValidTarget() && unit != null)
            {
                GlobalVariables.CastManager.Queque.Enqueue(3, () => GlobalVariables.Spells[SpellSlot.E].CastOnUnit(unit));
            }
        }

        // TODO: Decomposite
        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            this.SoftReset();

            if (GlobalVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear
                || !GlobalVariables.Spells[SpellSlot.E].IsReady() || GlobalVariables.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            this.GetUnits();

            this.SetDashes();

            this.ValidateDashes();

            this.LogicLaneClear();        
        }

        /// <summary>
        /// Executes LaneClear logic
        /// </summary>
        private void LogicLaneClear()
        {
            switch (this.Menu.Item(this.Menu.Name + "DashOrientation").GetValue<StringList>().SelectedIndex)
            {
                // Mouse oriented LaneClearing
                case 0:
                    if (this.Dashes.Any())
                    {
                        foreach (
                            var dash in
                                this.Dashes.Where(
                                    dash =>
                                    dash.EndPosition.Distance(Game.CursorPos)
                                    < dash.StartPosition.Distance(Game.CursorPos)))
                        {
                            if (dash.IsWallDash)
                            {
                                continue;
                            }

                            // Minion will die and no other minions are in killable range
                            if (dash.Unit.Health < this.providerE.GetDamage(dash.Unit)
                                && !this.Units.Any(
                                    x =>
                                    !x.Equals(dash.Unit)
                                    && (x.Distance(GlobalVariables.Player) <= GlobalVariables.Player.AttackRange)
                                    || (GlobalVariables.Spells[SpellSlot.Q].IsReady(100)
                                        && x.Distance(GlobalVariables.Player)
                                        <= GlobalVariables.Spells[SpellSlot.Q].Range)))
                            {
                                Execute(dash.Unit);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the units.
        /// </summary>
        private void GetUnits()
        {
            this.Units = MinionManager.GetMinions(
                GlobalVariables.Player.ServerPosition,
                GlobalVariables.Spells[SpellSlot.E].Range,
                MinionTypes.All,
                MinionTeam.NotAlly,
                MinionOrderTypes.None);
        }

        /// <summary>
        /// Sets the dashes.
        /// </summary>
        private void SetDashes()
        {
            if (!this.Units.Any())
            {
                return;
            }

            this.Dashes =
                this.Units.Where(
                    x =>
                    !x.HasBuff("YasuoDashScalar")
                    && x.Distance(GlobalVariables.Player) <= GlobalVariables.Spells[SpellSlot.E].Range)
                    .Select(minion => new Dash(minion))
                    .ToList();
        }

        /// <summary>
        /// Resets some properties/fields
        /// </summary>
        private void SoftReset()
        {
            this.Units = new List<Obj_AI_Base>();
            this.Dashes = new List<Dash>();
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

                if (this.Menu.Item(this.Menu.Name + "NoSkillshot").GetValue<bool>())
                {
                    if (dash.InSkillshot)
                    {
                        remove = true;
                    }
                }

                if (this.Menu.Item(this.Menu.Name + "NoTurret").GetValue<bool>())
                {
                    if (!this.providerTurret.IsSafePosition(dash.EndPosition))
                    {
                        remove = true;
                    }
                }

                //if (this.Menu.Item(this.Menu.Name + "NoEnemy").GetValue<bool>())
                //{
                //    if (this.ChampionSliderMenu.Values.Any(
                //            entry => dash.EndPosition.Distance(entry.Key.ServerPosition) > entry.Value))
                //    {
                //        remove = true;
                //    }
                //}

                if (this.Menu.Item(this.Menu.Name + "NoWallDash").GetValue<bool>())
                {
                    if (dash.IsWallDash)
                    {
                        remove = true;
                    }
                }

                switch (this.Menu.Item(this.Menu.Name + "DashOrientation").GetValue<StringList>().SelectedIndex)
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