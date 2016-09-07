using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Orianna
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
            Game.OnUpdate += Orianna.OnUpdate;
            Events.OnGapCloser += Orianna.OnGapCloser;
            Spellbook.OnCastSpell += Orianna.OnCastSpell;
            Events.OnInterruptableTarget += Orianna.OnInterruptableTarget;
            Obj_AI_Base.OnProcessSpellCast += Orianna.OnProcessSpellCast;
        }

        #endregion
    }
}