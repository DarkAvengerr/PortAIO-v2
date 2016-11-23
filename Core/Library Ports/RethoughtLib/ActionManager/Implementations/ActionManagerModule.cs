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
//     Last Edited: 04.10.2016 1:43 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.ActionManager.Implementations
{
    #region Using Directives

    using System;
    using System.Linq;

    using RethoughtLib.ActionManager.Abstract_Classes;
    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.PriorityQuequeV2;

    #endregion

    /// <summary>
    ///     The action manager module.
    /// </summary>
    public class ActionManagerModule : ChildBase, IActionManager
    {
        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Cast Manager";

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Events.OnPostUpdate -= this.OnPostUpdate;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        /// <param name="sender">the sender of the input</param>
        /// <param name="eventArgs">the contextual information</param>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Events.OnPostUpdate += this.OnPostUpdate;
        }

        /// <summary>
        ///     Raises the <see cref="E:PostUpdate" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnPostUpdate(EventArgs args)
        {
            this.Process();
        }

        #endregion

        #region IActionManager Members

        /// <summary>
        ///     Gets or sets the queue.
        /// </summary>
        /// <value>
        ///     The queue.
        /// </value>
        public virtual PriorityQueue<int, Action> Queue { get; set; }

        /// <summary>
        ///     Processes all items that are supposed to get casted.
        /// </summary>
        public virtual void Process()
        {
            for (var i = 0; i < this.Queue.Dictionary.ToList().Count; i++)
            {
                var action = this.Queue.Dequeue();

                action?.Invoke();
            }
        }

        #endregion
    }
}