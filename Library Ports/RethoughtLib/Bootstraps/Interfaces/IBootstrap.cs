using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Bootstraps.Interfaces
{
    public interface IBootstrap
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Initializes the bootstrap.
        /// </summary>
        void Run();

        #endregion
    }
}