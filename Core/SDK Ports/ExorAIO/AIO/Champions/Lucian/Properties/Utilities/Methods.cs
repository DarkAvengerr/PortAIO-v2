using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Lucian
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
        ///     The methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Lucian.OnUpdate;
            Obj_AI_Base.OnSpellCast += Lucian.OnSpellCast;
            Events.OnGapCloser += Lucian.OnGapCloser;
            Obj_AI_Base.OnPlayAnimation += Lucian.OnPlayAnimation;
            Variables.Orbwalker.OnAction += Lucian.OnAction;
        }

        #endregion
    }
}