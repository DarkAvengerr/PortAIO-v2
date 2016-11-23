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
 namespace RethoughtLib.Events
{
    #region Using Directives

    using System;

    using LeagueSharp;

    using RethoughtLib.Classes.General_Intefaces;

    #endregion

    public delegate void EventHandler(EventArgs args);

    /// <summary>
    ///     Class that offers additional events
    /// </summary>
    public class Events : ILoadable
    {
        #region Public Events

        /// <summary>
        ///     Occurs when [on post update].
        /// </summary>
        public static event EventHandler OnPostUpdate;

        /// <summary>
        ///     Occurs when [on pre update].
        /// </summary>
        public static event EventHandler OnPreUpdate;

        /// <summary>
        ///     Occurs when [on update].
        /// </summary>
        public static event EventHandler OnUpdate;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; } = nameof(Events);

        #endregion

        #region Public Methods and Operators

        #region ILoadable Members

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Game.OnUpdate += OnGameUpdate;
        }

        #endregion

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <see cref="E:GameUpdate" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                OnPreUpdate?.Invoke(args);

                OnUpdate?.Invoke(args);

                OnPostUpdate?.Invoke(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}