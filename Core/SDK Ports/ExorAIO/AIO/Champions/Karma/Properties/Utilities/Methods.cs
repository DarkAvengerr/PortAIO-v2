using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Karma
{
    using LeagueSharp;
    using LeagueSharp.SDK;

    /// <summary>
    ///     The methods class.
    /// </summary>
    internal class Methods
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Sets the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Karma.OnUpdate;
            Events.OnGapCloser += Karma.OnGapCloser;
            Obj_AI_Base.OnProcessSpellCast += Karma.OnProcessSpellCast;
            Variables.Orbwalker.OnAction += Karma.OnAction;
        }

        #endregion
    }
}