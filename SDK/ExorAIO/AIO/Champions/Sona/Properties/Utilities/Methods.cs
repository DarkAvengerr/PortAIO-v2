using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Sona
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
            Game.OnUpdate += Sona.OnUpdate;
            Events.OnGapCloser += Sona.OnGapCloser;
            Variables.Orbwalker.OnAction += Sona.OnAction;
            Obj_AI_Base.OnProcessSpellCast += Sona.OnProcessSpellCast;
            Events.OnInterruptableTarget += Sona.OnInterruptableTarget;
        }

        #endregion
    }
}