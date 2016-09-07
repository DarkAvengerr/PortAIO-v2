using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Ezreal
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
            Game.OnUpdate += Ezreal.OnUpdate;
            Obj_AI_Base.OnSpellCast += Ezreal.OnSpellCast;
            Events.OnGapCloser += Ezreal.OnGapCloser;
            Obj_AI_Base.OnBuffGain += Ezreal.OnBuffAdd;
        }

        #endregion
    }
}