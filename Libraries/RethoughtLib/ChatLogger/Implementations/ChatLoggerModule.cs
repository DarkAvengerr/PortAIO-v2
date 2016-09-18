using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.ChatLogger.Implementations
{
    #region Using Directives

    using global::RethoughtLib.ChatLogger.Abstract_Classes;
    using global::RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    public class ChatLoggerModule : ChildBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatLoggerModule"/> class.
        /// </summary>
        /// <param name="chatLogger">The chat logger.</param>
        public ChatLoggerModule(ChatLoggerBase chatLogger)
        {
            this.ChatLogger = chatLogger;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the chat logger.
        /// </summary>
        /// <value>
        /// The chat logger.
        /// </value>
        public ChatLoggerBase ChatLogger { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; }

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);
        }

        #endregion
    }
}