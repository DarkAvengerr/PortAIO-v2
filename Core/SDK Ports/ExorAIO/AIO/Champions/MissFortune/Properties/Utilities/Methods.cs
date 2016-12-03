using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.MissFortune
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
            Game.OnUpdate += MissFortune.OnUpdate;
            Obj_AI_Base.OnSpellCast += MissFortune.OnSpellCast;
            Events.OnGapCloser += MissFortune.OnGapCloser;
            Variables.Orbwalker.OnAction += MissFortune.OnAction;
        }

        #endregion
    }
}