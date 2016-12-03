using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Lux
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
            Game.OnUpdate += Lux.OnUpdate;
            GameObject.OnCreate += Lux.OnCreate;
            GameObject.OnDelete += Lux.OnDelete;
            Events.OnGapCloser += Lux.OnGapCloser;
            Obj_AI_Base.OnProcessSpellCast += Lux.OnProcessSpellCast;
            Variables.Orbwalker.OnAction += Lux.OnAction;
        }

        #endregion
    }
}