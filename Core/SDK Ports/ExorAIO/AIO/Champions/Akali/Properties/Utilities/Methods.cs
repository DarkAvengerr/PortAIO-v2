using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Akali
{
    using LeagueSharp;

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
            Game.OnUpdate += Akali.OnUpdate;
            Obj_AI_Base.OnSpellCast += Akali.OnSpellCast;
        }

        #endregion
    }
}