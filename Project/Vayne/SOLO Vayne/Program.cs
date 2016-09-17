using System;
using LeagueSharp.Common;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne
{
    class Program
    {
        /// <summary>
        /// The entry point of the assembly.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        /// <summary>
        /// Raises the <see cref="E:Load" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void OnLoad(EventArgs args)
        {
            Variables.Instance = new SOLOBootstrap();
        }
    }
}
