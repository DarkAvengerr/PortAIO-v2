using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.ActionManager.Abstract_Classes
{
    #region Using Directives

    using System;

    using RethoughtLib.PriorityQuequeV2;

    #endregion

    public interface IActionManager
    {
        /// <summary>
        ///     Gets or sets the queque.
        /// </summary>
        /// <value>
        ///     The queque.
        /// </value>
        PriorityQueue<int, Action> Queque { get; set; }

        /// <summary>
        ///     Processes all items that are supposed to get casted.
        /// </summary>
        void Process();
    }
}