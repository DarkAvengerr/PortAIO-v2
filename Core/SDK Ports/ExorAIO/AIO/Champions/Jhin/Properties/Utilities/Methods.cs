using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Jhin
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
            Game.OnUpdate += Jhin.OnUpdate;
            Obj_AI_Base.OnSpellCast += Jhin.OnSpellCast;
            Events.OnGapCloser += Jhin.OnGapCloser;
            Obj_AI_Base.OnProcessSpellCast += Jhin.OnProcessSpellCast;
            Variables.Orbwalker.OnAction += Jhin.OnAction;
        }

        #endregion
    }
}