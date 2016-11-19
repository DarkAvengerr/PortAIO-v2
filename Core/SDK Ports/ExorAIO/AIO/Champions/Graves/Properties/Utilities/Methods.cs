using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Graves
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
            Game.OnUpdate += Graves.OnUpdate;
            Obj_AI_Base.OnSpellCast += Graves.OnSpellCast;
            Events.OnGapCloser += Graves.OnGapCloser;
        }

        #endregion
    }
}