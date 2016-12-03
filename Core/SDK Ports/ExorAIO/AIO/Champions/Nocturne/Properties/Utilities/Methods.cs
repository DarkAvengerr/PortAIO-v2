using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Nocturne
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
            Game.OnUpdate += Nocturne.OnUpdate;
            Events.OnGapCloser += Nocturne.OnGapCloser;
            Events.OnInterruptableTarget += Nocturne.OnInterruptableTarget;
            Obj_AI_Base.OnProcessSpellCast += Nocturne.OnProcessSpellCast;
        }

        #endregion
    }
}