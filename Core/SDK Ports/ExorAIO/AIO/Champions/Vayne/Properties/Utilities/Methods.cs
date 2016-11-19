using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Vayne
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
            Game.OnUpdate += Vayne.OnUpdate;
            Obj_AI_Base.OnSpellCast += Vayne.OnSpellCast;
            Events.OnGapCloser += Vayne.OnGapCloser;
            Events.OnInterruptableTarget += Vayne.OnInterruptableTarget;
            Variables.Orbwalker.OnAction += Vayne.OnAction;
        }

        #endregion
    }
}