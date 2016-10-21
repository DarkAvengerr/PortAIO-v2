using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Sivir
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
            Game.OnUpdate += Sivir.OnUpdate;
            Obj_AI_Base.OnSpellCast += Sivir.OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += Sivir.OnProcessSpellCast;
        }

        #endregion
    }
}