using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.DrMundo
{
    using LeagueSharp;

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
            Game.OnUpdate += DrMundo.OnUpdate;
            Obj_AI_Base.OnSpellCast += DrMundo.OnSpellCast;
        }

        #endregion
    }
}