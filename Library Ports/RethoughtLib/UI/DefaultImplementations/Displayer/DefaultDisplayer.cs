using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.UI.DefaultImplementations.Displayer
{
    #region Using Directives

    using System;

    using global::RethoughtLib.Design;
    using global::RethoughtLib.Design.Implementations;
    using global::RethoughtLib.UI.Core;
    using global::RethoughtLib.UI.Core.Displayer;

    #endregion

    /// <summary>
    ///     The default Displayer
    /// </summary>
    /// <seealso cref="Displayer{T}" />
    public sealed class DefaultDisplayer<T> : Displayer<T> where T : Element
    {
        #region Fields

        /// <summary>
        ///     The new offset
        /// </summary>
        private IntOffset offset;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultDisplayer{T}" /> class.
        /// </summary>
        public DefaultDisplayer()
        {
            this.OnElementAdd += this.OnAdd;

            this.OnElementRemove += this.OnRemove;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Displays the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Add(T element)
        {
            base.Add(element);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <see cref="E:Add" /> event.
        /// </summary>
        /// <param name="eventargs">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <param name="object">The object.</param>
        /// <exception cref="ArgumentException">The element is already getting displayed</exception>
        protected override void OnAdd(EventArgs eventargs, T @object)
        {
            base.OnAdd(eventargs, @object);

            this.SetOffset();
        }

        /// <summary>
        ///     Raises the <see cref="E:Delete" /> event.
        /// </summary>
        /// <param name="eventargs">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <param name="object">The object.</param>
        /// <exception cref="ArgumentException">The element is not getting displayed</exception>
        protected override void OnRemove(EventArgs eventargs, T @object)
        {
            base.OnRemove(eventargs, @object);

            this.SetOffset();
        }

        /// <summary>
        ///     Sets the offset.
        /// </summary>
        private void SetOffset()
        {
            var tempOffset = new IntOffset();

            foreach (var element in this.ElementsDictionary)
            {
                tempOffset.Top += element.Key.Design.Height;
                tempOffset.Top += this.VerticalSpaceBetweenElements;
            }

            this.offset = tempOffset;
        }

        #endregion
    }
}