using EloBuddy; namespace RethoughtLib.PriorityQueque
{
    #region Using Directives

    

    #endregion

    /// <summary>
    ///     Class that offers the access to a priorityqueque
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T>
    {
        #region Fields

        /// <summary>
        ///     The queque implementation
        /// </summary>
        public IPriorityQueque<T> QuequeImplementation;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PriorityQueue{T}" /> class.
        /// </summary>
        public PriorityQueue()
        {
            this.QuequeImplementation = new DefaultPriorityQueque<T>(0);
        }

        #endregion


    }
}