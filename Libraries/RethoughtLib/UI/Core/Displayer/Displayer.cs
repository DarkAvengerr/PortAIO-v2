using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.UI.Core.Displayer
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using global::RethoughtLib.Design;

    using SharpDX;

    #endregion

    /// <summary>
    ///     Displayer are declaring zones and behaiviors to display Notifications
    /// </summary>
    /// <typeparam name="T">Class of type Notification</typeparam>
    public abstract class Displayer<T>
        where T : Element
    {
        #region Fields

        /// <summary>
        ///     The active elements
        /// </summary>
        protected Dictionary<T, Vector2> ElementsDictionary = new Dictionary<T, Vector2>();

        /// <summary>
        ///     The priority queque
        /// </summary>
        protected PriorityQueque.PriorityQueue<Element> PriorityQueque = new PriorityQueque.PriorityQueue<Element>();

        #endregion

        #region Delegates

        /// <summary>
        /// </summary>
        /// <param name="eventArgs">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <param name="object">The object.</param>
        public delegate void DisplayerEvent(EventArgs eventArgs, T @object);

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when [on element add].
        /// </summary>
        public event DisplayerEvent OnElementAdd;

        /// <summary>
        ///     Occurs when [on element delete].
        /// </summary>
        public event DisplayerEvent OnElementRemove;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the bot left point.
        /// </summary>
        /// <value>
        ///     The bot left point.
        /// </value>
        public Point BotLeftPoint => new Point(this.DistanceLeft, this.DistanceTop + this.MaxHeight);

        /// <summary>
        ///     Gets the bot right point.
        /// </summary>
        /// <value>
        ///     The bot right point.
        /// </value>
        public Point BotRightPoint => new Point(this.DistanceLeft + this.MaxWidth, this.DistanceTop + this.MaxHeight);

        /// <summary>
        ///     Gets the distance left (in pixels).
        /// </summary>
        /// <value>
        ///     The distance left.
        /// </value>
        public virtual int DistanceLeft { get; set; } = 40;

        /// <summary>
        ///     Gets the distance top (in pixels).
        /// </summary>
        /// <value>
        ///     The distance left.
        /// </value>
        public virtual int DistanceTop { get; set; } = 195;

        /// <summary>
        ///     Gets the maximum height (in pixels)
        /// </summary>
        /// <value>
        ///     The maximum height.
        /// </value>
        public virtual int MaxHeight { get; set; } = 700;

        /// <summary>
        ///     Gets the maximum width (in pixels)
        /// </summary>
        /// <value>
        ///     The maximum width.
        /// </value>
        public virtual int MaxWidth { get; set; } = 300;

        /// <summary>
        ///     Gets the top left point.
        /// </summary>
        /// <value>
        ///     The top left point.
        /// </value>
        public Point TopLeftPoint => new Point(this.DistanceLeft, this.DistanceTop);

        /// <summary>
        ///     Gets the top right point.
        /// </summary>
        /// <value>
        ///     The top right point.
        /// </value>
        public Point TopRightPoint => new Point(this.DistanceLeft + this.MaxWidth, this.DistanceTop);

        /// <summary>
        ///     Gets or sets the spacing between elements.
        /// </summary>
        /// <value>
        ///     The spacing between elements.
        /// </value>
        public virtual int VerticalSpaceBetweenElements { get; set; } = 25;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Displays the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public virtual void Add(T element)
        {
            this.OnOnElementAdd(element);
        }

        /// <summary>
        ///     Displays all elements.
        /// </summary>
        public virtual void Display()
        {
            foreach (var element in this.ElementsDictionary)
            {
                element.Key.Draw();
            }
        }

        /// <summary>
        ///     Removes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public virtual void Remove(T element)
        {
            this.OnOnElementRemove(element);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the position for an element in the dictionary.
        /// </summary>
        /// <returns></returns>
        protected virtual Vector2 GetPosition(T element)
        {
            var offset = new Offset<int>();

            foreach (var entry in this.ElementsDictionary)
            {
                offset.Top += entry.Key.Design.Height + this.VerticalSpaceBetweenElements;
            }

            offset.Top += element.Design.Height / 2;
            offset.Left += this.MaxWidth / 2;

            return new Vector2(offset.Left, offset.Top);
        }

        /// <summary>
        ///     Raises the <see cref="E:Add" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <param name="object">The object.</param>
        /// <exception cref="ArgumentException">ElementsDictionary already contains this element.</exception>
        protected virtual void OnAdd(EventArgs args, T @object)
        {
            if (this.ElementsDictionary.ContainsKey(@object))
            {
                throw new InvalidOperationException("Displayer already contains this element.");
            }

            this.ElementsDictionary.Add(@object, this.GetPosition(@object));
        }

        /// <summary>
        ///     Raises the <see cref="E:Delete" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <param name="object">The object.</param>
        /// <exception cref="ArgumentException">ElementsDictionary does not contain this element.</exception>
        protected virtual void OnRemove(EventArgs args, T @object)
        {
            if (!this.ElementsDictionary.ContainsKey(@object))
            {
                throw new InvalidOperationException("Displayer does not contain this element.");
            }

            this.ElementsDictionary.Remove(@object);

            foreach (var element in this.ElementsDictionary)
            {
                this.ElementsDictionary[element.Key] = this.GetPosition(@object);
            }
        }

        /// <summary>
        ///     Called when [on element add].
        /// </summary>
        /// <param name="sender">The object.</param>
        protected virtual void OnOnElementAdd(T sender)
        {
            this.OnElementAdd?.Invoke(EventArgs.Empty, sender);
        }

        /// <summary>
        ///     Called when [on element delete].
        /// </summary>
        /// <param name="sender">The object.</param>
        protected virtual void OnOnElementRemove(T sender)
        {
            this.OnElementRemove?.Invoke(EventArgs.Empty, sender);
        }

        #endregion
    }
}