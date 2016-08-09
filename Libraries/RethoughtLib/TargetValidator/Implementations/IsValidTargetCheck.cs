using EloBuddy; namespace RethoughtLib.TargetValidator.Implementations
{
    #region Using Directives

    using global::RethoughtLib.TargetValidator.Interfaces;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    public class IsValidTargetCheck : ICheckable
    {
        #region Public Methods and Operators

        public bool Check(Obj_AI_Base target)
        {
            return target.IsValidTarget();
        }

        #endregion
    }
}