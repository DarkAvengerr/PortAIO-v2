using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Jax
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
            Game.OnUpdate += Jax.OnUpdate;
            Obj_AI_Base.OnSpellCast += Jax.OnSpellCast;
            Events.OnGapCloser += Jax.OnGapCloser;
        }

        #endregion
    }
}