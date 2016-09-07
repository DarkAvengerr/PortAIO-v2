using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Diana
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
        ///     Initializes the methods.
        /// </summary>
        public static void Initialize()
        {
            Game.OnUpdate += Diana.OnUpdate;
            Obj_AI_Base.OnSpellCast += Diana.OnSpellCast;
            Events.OnGapCloser += Diana.OnGapCloser;
            Events.OnInterruptableTarget += Diana.OnInterruptableTarget;
        }

        #endregion
    }
}