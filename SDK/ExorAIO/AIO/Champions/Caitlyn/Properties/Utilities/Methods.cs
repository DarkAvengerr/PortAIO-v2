using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Caitlyn
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
            Game.OnUpdate += Caitlyn.OnUpdate;
            Spellbook.OnCastSpell += Caitlyn.OnCastSpell;
            Events.OnGapCloser += Caitlyn.OnGapCloser;
            Events.OnInterruptableTarget += Caitlyn.OnInterruptableTarget;
            Obj_AI_Base.OnSpellCast += Caitlyn.OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += Caitlyn.OnProcessSpellCast;
        }

        #endregion
    }
}