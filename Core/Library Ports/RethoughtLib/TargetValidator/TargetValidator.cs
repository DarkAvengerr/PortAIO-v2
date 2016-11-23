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
 namespace RethoughtLib.TargetValidator
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    using RethoughtLib.TargetValidator.Interfaces;

    #endregion

    public class TargetValidator : TargetValidatorBase
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Adds the check.
        /// </summary>
        /// <param name="check">The check.</param>
        public override sealed void AddCheck(ICheckable check)
        {
            if (this.ChecksList.Contains(check)) throw new InvalidOperationException("Can't add multiple similiar checks");

            base.AddCheck(check);
        }

        /// <summary>
        ///     Adds the checks.
        /// </summary>
        /// <param name="checks">The checks.</param>
        public override sealed void AddChecks(IEnumerable<ICheckable> checks)
        {
            base.AddChecks(checks);
        }

        /// <summary>
        ///     Checks the specified object.
        /// </summary>
        /// <param name="object">The object.</param>
        public override sealed bool Check(Obj_AI_Base @object)
        {
            return base.Check(@object);
        }

        /// <summary>
        ///     Removes the check.
        /// </summary>
        /// <param name="check">The check.</param>
        public override sealed void RemoveCheck(ICheckable check)
        {
            if (!this.ChecksList.Contains(check)) throw new InvalidOperationException("Can't remove checks that are not there");

            base.RemoveCheck(check);
        }

        /// <summary>
        ///     Removes the checks.
        /// </summary>
        /// <param name="checks">The checks.</param>
        public override sealed void RemoveChecks(IEnumerable<ICheckable> checks)
        {
            base.RemoveChecks(checks);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        protected override sealed void SoftReset()
        {
            base.SoftReset();
        }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        protected override sealed void Start()
        {
            base.Start();
        }

        #endregion
    }
}