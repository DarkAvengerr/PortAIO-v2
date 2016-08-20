using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Exceptions
{
    #region Using Directives

    using System;
    using System.Runtime.Serialization;

    #endregion

    /// <summary>
    ///     Exceptions that gets thrown when the PlaySharpBootstrap fails loading because of any unspecified reason
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    internal class BootstrapFailedLoadingException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BootstrapFailedLoadingException" /> class.
        /// </summary>
        public BootstrapFailedLoadingException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BootstrapFailedLoadingException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BootstrapFailedLoadingException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BootstrapFailedLoadingException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in
        ///     Visual Basic) if no inner exception is specified.
        /// </param>
        public BootstrapFailedLoadingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BootstrapFailedLoadingException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        protected BootstrapFailedLoadingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }

    /// <summary>
    ///     Exceptions that gets thrown when a menu failes generating
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    internal class MenuGenerationException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuGenerationException" /> class.
        /// </summary>
        public MenuGenerationException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuGenerationException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MenuGenerationException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuGenerationException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in
        ///     Visual Basic) if no inner exception is specified.
        /// </param>
        public MenuGenerationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuGenerationException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        protected MenuGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }

    /// <summary>
    ///     Exceptions that gets thrown when a the MenuTranslator fails translating something to any unspecific reasoning
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    internal class TranslationException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TranslationException" /> class.
        /// </summary>
        public TranslationException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TranslationException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TranslationException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TranslationException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in
        ///     Visual Basic) if no inner exception is specified.
        /// </param>
        public TranslationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TranslationException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        protected TranslationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}