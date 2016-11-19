using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Darius
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
            Game.OnUpdate += Darius.OnUpdate;
            Obj_AI_Base.OnSpellCast += Darius.OnSpellCast;
            Events.OnGapCloser += Darius.OnGapCloser;
            Events.OnInterruptableTarget += Darius.OnInterruptableTarget;
        }

        #endregion
    }
}