using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Tristana
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
            Game.OnUpdate += Tristana.OnUpdate;
            Events.OnGapCloser += Tristana.OnGapCloser;
            Obj_AI_Base.OnBuffGain += Tristana.OnBuffAdd;
            Variables.Orbwalker.OnAction += Tristana.OnAction;
        }

        #endregion
    }
}