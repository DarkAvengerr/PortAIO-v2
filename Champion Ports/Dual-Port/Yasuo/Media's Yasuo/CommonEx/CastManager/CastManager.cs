using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.CastManager
{
    #region Using Directives

    using System;
    using System.Linq;

    using global::YasuoMedia.CommonEx.PriorityQuequev2;

    #endregion

    /// <summary>
    ///     The CastManager. There should only be one instance of it.
    /// </summary>
    /// <seealso cref="Action" />
    class CastManager
    {
        #region Fields

        internal PriorityQueue<int, Action> Queque;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CastManager" /> class.
        /// </summary>
        public CastManager()
        {
            this.Queque = new PriorityQueue<int, Action>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Forces the action if there are no extremely high valued actions in the queque
        /// </summary>
        /// <param name="action">The action.</param>
        public void ForceAction(Action action)
        {
            if (this.Queque.Dictionary.Any(x => x.Key == 0))
            {
                if (GlobalVariables.Debug)
                {
                    Console.WriteLine(
                        string.Format(
                            "CastManager: ForceAction(Action) > Returned because there was a priorized action"));
                }

                return;
            }

            action.Invoke();
        }

        /// <summary>
        ///     Processes all items that are supposed to get casted.
        /// </summary>
        public void Process()
        {
            try
            {
                foreach (var queque in this.Queque.Dictionary.ToList())
                {
                    var action = this.Queque.Dequeue();

                    action?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex: " + ex);
            }
        }

        #endregion
    }
}