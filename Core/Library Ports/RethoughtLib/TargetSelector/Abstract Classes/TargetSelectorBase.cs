//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:44 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.TargetSelector.Abstract_Classes
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.TargetSelector.Interfaces;

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

        #region Constructors

        protected TargetSelectorBase(Menu menu)
        {
            this.RootMenu = menu;

            this.ModeAdded += this.OnModeAdded;
            this.ModeRemoved += this.OnModeRemoved;
            this.ModeChanged += this.OnModeChanged;

            this.Initialize();
        }

        #endregion

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
            this.OnModeAddedInvoker(new TargetSelectorEventArgs { AddedMode = mode });
        }

        // TODO
        public AIHeroClient GetTarget()
        {
            return this.activeSelectionMode.GetTarget(null, null);
        }

        public void RemoveMode(ITargetSelectionMode mode)
        {
            this.OnModeRemovedInvoker(new TargetSelectorEventArgs { RemovedMode = mode });
        }

        public void SetMode(ITargetSelectionMode mode)
        {
            if (mode.Equals(this.activeSelectionMode)) return;

            this.OnModeChangedInvoker(
                new ValueChangedEventArgs { NewValue = mode, OldValue = this.activeSelectionMode });
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
            if (this.Modes.Contains(targetSelectorEventArgs.AddedMode)) return;

            this.Modes.Add(targetSelectorEventArgs.AddedMode);
        }

        protected void OnModeAddedInvoker(TargetSelectorEventArgs eventArgs)
        {
            this.ModeAdded?.Invoke(this, eventArgs);
        }

        protected void OnModeChanged(object sender, ValueChangedEventArgs valueChangedEventArgs)
        {
            if (this.Modes.Contains(valueChangedEventArgs.NewValue)) this.activeSelectionMode = valueChangedEventArgs.NewValue;
        }

        protected void OnModeChangedInvoker(ValueChangedEventArgs eventArgs)
        {
            this.ModeChanged?.Invoke(this, eventArgs);
        }

        protected void OnModeRemoved(object sender, TargetSelectorEventArgs targetSelectorEventArgs)
        {
            if (this.Modes.Contains(targetSelectorEventArgs.RemovedMode)) this.Modes.Remove(targetSelectorEventArgs.RemovedMode);
        }

        protected void OnModeRemovedInvoker(TargetSelectorEventArgs eventArgs)
        {
            this.ModeRemoved?.Invoke(this, eventArgs);
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