using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.PriorityQueque
{
    #region Using Directives



    #endregion

    /// <summary>
    ///     Class that offers the access to a priorityqueque
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueueContainer<T>
    {
        #region Fields

        /// <summary>
        ///     The queque implementation
        /// </summary>
        public IPriorityQueque<T> PriorityQueque { get; set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PriorityQueueContainer{T}" /> class.
        /// </summary>
        public PriorityQueueContainer()
        {
            this.PriorityQueque = new PriorityQueque<T>(0);
        }

        #endregion
    }
}