// --------------------------------------------------------------------------------------------------------------------  
// <summary>
//   The Load.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Jayce
{
    #region

    using System;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using Events = LeagueSharp.SDK.Events;

    #endregion

    /// <summary>
    ///     The Load.
    /// </summary>
    internal class Load
    {
        #region Methods

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main()
        {
            Bootstrap.Init(null);
            OnLoad();
        }

        /// <summary>
        /// The on load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName == "Jayce")
            {
                Extensions.Events.Initialize();
                Extensions.Spells.Initialize();
                Extensions.Config.Initialize();
            }
        }

        #endregion
    }
}