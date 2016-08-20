using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.CastManager
{
    #region Using Directives

    using System;
    using System.Linq;

    using global::RethoughtLib.PriorityQuequeV2;

    #endregion

    /// <summary>
    ///     The CastManager. Uses a singleton pattern. Access it over the Instance property.
    /// </summary>
    /// <seealso cref="Action" />
    public class CastManager
    {
        #region Static Fields

        private static CastManager instance;

        #endregion

        #region Fields

        internal PriorityQueue<int, Action> Queque = new PriorityQueue<int, Action>();

        #endregion

        #region Constructors and Destructors

        private CastManager()
        {
        }

        #endregion

        #region Public Properties

        public static CastManager Instance => instance ?? (instance = new CastManager());

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Forces the action if there are no extremely high valued actions in the queque
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="priority">If an action with a "higher" priority than specified is quequed the action won't get forced</param>
        public void ForceAction(Action action, int priority)
        {
            if (this.Queque.Dictionary.Any(x => x.Key == 0))
            {
#if DEBUG
                Console.WriteLine(
                    "CastManager: ForceAction(Action) > Returned and quequed because there was a priorized action");
#endif

                this.Queque.Enqueue(priority, action);

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
                for (var i = 0; i < this.Queque.Dictionary.ToList().Count; i++)
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