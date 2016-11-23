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

    public abstract class TargetValidatorBase
    {
        #region Fields

        /// <summary>
        ///     The invalid states
        /// </summary>
        protected List<ICheckable> ChecksList = new List<ICheckable>();

        /// <summary>
        ///     The object
        /// </summary>
        protected Obj_AI_Base Target;

        /// <summary>
        ///     Whether target is valid
        /// </summary>
        protected bool Valid = true;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the check.
        /// </summary>
        /// <param name="check">The check.</param>
        public virtual void AddCheck(ICheckable check)
        {
            this.ChecksList.Add(check);
        }

        /// <summary>
        ///     Adds the checks.
        /// </summary>
        /// <param name="checks">The checks.</param>
        public virtual void AddChecks(IEnumerable<ICheckable> checks)
        {
            foreach (var check in checks) this.AddCheck(check);
        }

        /// <summary>
        ///     Checks the specified object.
        /// </summary>
        /// <param name="object">The object.</param>
        public virtual bool Check(Obj_AI_Base @object)
        {
            this.SoftReset();

            this.Target = @object;

            this.Start();

            return this.Valid;
        }

        /// <summary>
        ///     Removes the check.
        /// </summary>
        /// <param name="check">The check.</param>
        public virtual void RemoveCheck(ICheckable check)
        {
            this.ChecksList.Remove(check);
        }

        /// <summary>
        ///     Removes the checks. If no checks are given every check gets removed.
        /// </summary>
        public virtual void RemoveChecks()
        {
            foreach (var check in this.ChecksList) this.RemoveCheck(check);
        }

        /// <summary>
        ///     Removes the checks. If no checks are given every check gets removed.
        /// </summary>
        /// <param name="checks">The checks.</param>
        public virtual void RemoveChecks(IEnumerable<ICheckable> checks)
        {
            if (checks == null)
            {
                this.RemoveChecks();
                return;
            }

            foreach (var check in checks) this.RemoveCheck(check);
        }

        /// <summary>
        ///     Resets this instance. Usually not needed and you should use RemoveChecks with null parameters given.
        /// </summary>
        public virtual void Reset()
        {
            this.SoftReset();

            this.ChecksList = new List<ICheckable>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        protected virtual void SoftReset()
        {
            this.Target = null;
            this.Valid = true;
        }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        protected virtual void Start()
        {
            if (this.Target == null)
            {
                this.Valid = false;
                return;
            }

            foreach (var check in this.ChecksList)
            {
                if (this.Valid == false) break;

                this.Valid = check.Check(this.Target);

                Console.WriteLine($"The target {this.Target} got checked with {check}, result was {this.Valid}");
            }

#if DEBUG
            Console.WriteLine($"[TargetValidator] Target {this.Target.Name} is an valid target: {this.Valid}");
#endif
        }

        #endregion
    }
}