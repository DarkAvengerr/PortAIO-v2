using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.TargetSelector.Abstract_Classes
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::RethoughtLib.TargetSelector.Interfaces;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class TargetSelectorBase
    {
        #region Fields

        protected string DisplayName;

        protected Menu Menu;

        protected Menu RootMenu;

        private ITargetSelectionMode activeSelectionMode;

        #endregion

        #region Constructors and Destructors

        protected TargetSelectorBase(Menu menu)
        {
            this.RootMenu = menu;

            this.ModeAdded += this.OnModeAdded;
            this.ModeRemoved += this.OnModeRemoved;
            this.ModeChanged += this.OnModeChanged;

            this.Initialize();
        }

        #endregion

        #region Public Events

        public event EventHandler<TargetSelectorEventArgs> ModeAdded;

        public event EventHandler<ValueChangedEventArgs> ModeChanged;

        public event EventHandler<TargetSelectorEventArgs> ModeRemoved;

        #endregion

        #region Properties

        protected List<ITargetSelectionMode> Modes { get; set; } = new List<ITargetSelectionMode>();

        #endregion

        #region Public Methods and Operators

        public void AddMode(ITargetSelectionMode mode)
        {
            this.OnModeAddedInvoker(new TargetSelectorEventArgs() { AddedMode = mode });
        }

        // TODO
        public AIHeroClient GetTarget()
        {
            return this.activeSelectionMode.GetTarget(null, null);
        }

        public void RemoveMode(ITargetSelectionMode mode)
        {
            this.OnModeRemovedInvoker(new TargetSelectorEventArgs() { RemovedMode = mode });
        }

        public void SetMode(ITargetSelectionMode mode)
        {
            if (mode.Equals(this.activeSelectionMode))
            {
                return;
            }

            this.OnModeChangedInvoker(
                new ValueChangedEventArgs() { NewValue = mode, OldValue = this.activeSelectionMode });
        }

        #endregion

        #region Methods

        protected void Initialize()
        {
            this.Menu = new Menu(this.DisplayName, this.DisplayName);

            this.RootMenu.AddSubMenu(this.Menu);
        }

        protected void OnModeAdded(object sender, TargetSelectorEventArgs targetSelectorEventArgs)
        {
            if (this.Modes.Contains(targetSelectorEventArgs.AddedMode))
            {
                return;
            }

            this.Modes.Add(targetSelectorEventArgs.AddedMode);
        }

        protected void OnModeAddedInvoker(TargetSelectorEventArgs eventArgs)
        {
            this.ModeAdded?.Invoke(this, eventArgs);
        }

        protected void OnModeChangedInvoker(ValueChangedEventArgs eventArgs)
        {
            this.ModeChanged?.Invoke(this, eventArgs);
        }

        protected void OnModeRemoved(object sender, TargetSelectorEventArgs targetSelectorEventArgs)
        {
            if (this.Modes.Contains(targetSelectorEventArgs.RemovedMode))
            {
                this.Modes.Remove(targetSelectorEventArgs.RemovedMode);
            }
        }

        protected void OnModeRemovedInvoker(TargetSelectorEventArgs eventArgs)
        {
            this.ModeRemoved?.Invoke(this, eventArgs);
        }

        protected void OnModeChanged(object sender, ValueChangedEventArgs valueChangedEventArgs)
        {
            if (this.Modes.Contains(valueChangedEventArgs.NewValue))
            {
                this.activeSelectionMode = valueChangedEventArgs.NewValue;
            }
        }

        #endregion
    }

    public class TargetSelectorEventArgs : EventArgs
    {
        #region Public Properties

        public ITargetSelectionMode AddedMode { get; set; }

        public ITargetSelectionMode RemovedMode { get; set; }

        #endregion
    }

    public class ValueChangedEventArgs : EventArgs
    {
        #region Public Properties

        public ITargetSelectionMode NewValue { get; set; }

        public ITargetSelectionMode OldValue { get; set; }

        #endregion
    }
}