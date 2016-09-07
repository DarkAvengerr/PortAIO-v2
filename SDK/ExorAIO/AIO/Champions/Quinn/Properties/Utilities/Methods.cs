using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Quinn
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
            Game.OnUpdate += Quinn.OnUpdate;
            Obj_AI_Base.OnSpellCast += Quinn.OnSpellCast;
            Events.OnGapCloser += Quinn.OnGapCloser;
            Events.OnInterruptableTarget += Quinn.OnInterruptableTarget;
            Variables.Orbwalker.OnAction += Quinn.OnAction;
            Obj_AI_Base.OnProcessSpellCast += Quinn.OnProcessSpellCast;
        }

        #endregion
    }
}