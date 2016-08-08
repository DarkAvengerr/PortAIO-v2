using EloBuddy; namespace RethoughtLib.TargetValidator
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::RethoughtLib.TargetValidator.Interfaces;

    using LeagueSharp;

    #endregion

    public class TargetValidator : TargetValidatorBase
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Adds the check.
        /// </summary>
        /// <param name="check">The check.</param>
        public sealed override void AddCheck(ICheckable check)
        {
            if (this.ChecksList.Contains(check))
            {
                throw new InvalidOperationException("Can't add multiple similiar checks");
            }

            base.AddCheck(check);
        }

        /// <summary>
        ///     Removes the check.
        /// </summary>
        /// <param name="check">The check.</param>
        public sealed override void RemoveCheck(ICheckable check)
        {
            if (!this.ChecksList.Contains(check))
            {
                throw new InvalidOperationException("Can't remove checks that are not there");
            }

            base.RemoveCheck(check);
        }

        /// <summary>
        ///     Adds the checks.
        /// </summary>
        /// <param name="checks">The checks.</param>
        public sealed override void AddChecks(IEnumerable<ICheckable> checks)
        {
            base.AddChecks(checks);
        }

        /// <summary>
        ///     Checks the specified object.
        /// </summary>
        /// <param name="object">The object.</param>
        public sealed override bool Check(Obj_AI_Base @object)
        {
            return base.Check(@object);
        }

        /// <summary>
        ///     Removes the checks.
        /// </summary>
        /// <param name="checks">The checks.</param>
        public sealed override void RemoveChecks(IEnumerable<ICheckable> checks)
        {
            base.RemoveChecks(checks);
        }

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        protected sealed override void SoftReset()
        {
            base.SoftReset();
        }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        protected sealed override void Start()
        {
            base.Start();
        }

        #endregion
    }
}