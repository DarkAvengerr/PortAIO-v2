using EloBuddy; namespace RethoughtLib.TargetValidator.Interfaces
{
    #region Using Directives

    using LeagueSharp;

    #endregion

    /// <summary>
    ///     Checks for something.
    /// </summary>
    public interface ICheckable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Checks the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        bool Check(Obj_AI_Base target);

        #endregion
    }
}