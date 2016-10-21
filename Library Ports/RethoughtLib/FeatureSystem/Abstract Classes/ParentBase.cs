using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.FeatureSystem.Abstract_Classes
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public abstract class ParentBase : Base
    {
        #region Fields

        public readonly Dictionary<Base, Tuple<bool, bool>> Children = new Dictionary<Base, Tuple<bool, bool>>();

        #endregion

        #region Constructors and Destructors

        protected ParentBase()
        {
            this.OnChildAddEvent += this.OnChildAdd;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when [on disable event].
        /// </summary>
        public event EventHandler<ParentBaseEventArgs> OnChildAddEvent;

        /// <summary>
        ///     Occurs when [on disable event].
        /// </summary>
        public event EventHandler<ParentBaseEventArgs> OnChildRemoveEvent;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the child.
        /// </summary>
        /// <param name="child">The child.</param>
        public void Add(Base child)
        {
            this.OnChildAddInvoker(new ParentBaseEventArgs() { Child = child });
        }

        /// <summary>
        ///     Adds the children.
        /// </summary>
        /// <param name="children">The children.</param>
        public void Add(IEnumerable<Base> children)
        {
            foreach (var child in children)
            {
                this.Add(child);
            }
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        public override void Load()
        {
            if (this.Loaded) return;

            base.Load();

            foreach (var keyValuePair in this.Children.ToList())
            {
                var child = keyValuePair.Key;

                child.Initialize();

                child.Path = this.Path + child.Path;

                child.Switch.OnEnableEvent += this.OnChildEnabled;
                child.Switch.OnDisableEvent += this.OnChildDisabled;

                child.Load();

                if (!child.Menu.Children.Any() && !child.Menu.Items.Any()) continue;

                this.Menu.AddSubMenu(child.Menu);
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return "Parent " + this.Name;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Merges the child with another children with the same Name
        /// </summary>
        /// <param name="child">The child.</param>
        protected virtual void MergeChild(Base child)
        {
            foreach (var menuEntry in child.Menu.Items)
            {
                if (this.Menu.SubMenu(child.Menu.Name).Items.Contains(menuEntry))
                {
                    Console.WriteLine("Merging");
                    continue;
                }

                this.Menu.SubMenu(child.Menu.Name).AddItem(menuEntry);
            }

            child.Menu = this.Menu.SubMenu(child.Menu.Name);
        }

        /// <summary>
        ///     Called when [child add].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="parentBaseEventArgs">The <see cref="ParentBaseEventArgs" /> instance containing the event data.</param>
        protected virtual void OnChildAdd(object sender, ParentBaseEventArgs parentBaseEventArgs)
        {
            var child = parentBaseEventArgs.Child;

            this.Children.Add(child, new Tuple<bool, bool>(false, false));
        }

        /// <summary>
        ///     Raises the <see cref="E:ChildAddInvoker" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="ParentBaseEventArgs" /> instance containing the event data.</param>
        protected virtual void OnChildAddInvoker(ParentBaseEventArgs eventArgs)
        {
            this.OnChildAddEvent?.Invoke(this, eventArgs);
        }

        /// <summary>
        ///     Called when [child disabled].
        ///     Default Behavior:
        ///     > if the sender is the parent do nothing
        ///     > else if the sender is a child and all children are disabled then the parent will get disabled if it was enabled
        /// </summary>
        /// <param name="o">The sender.</param>
        /// <param name="featureBaseEventArgs"></param>
        protected virtual void OnChildDisabled(object o, FeatureBaseEventArgs featureBaseEventArgs)
        {
            var sender = featureBaseEventArgs.Sender;

            if (sender == null || sender == this)
            {
                return;
            }

            this.Children[sender] = new Tuple<bool, bool>(false, false);

            if (this.Children.Any(x => !x.Value.Item1)) return;

            this.Disable(sender);
        }

        /// <summary>
        ///     Called when [child enabled].
        ///     Default Behavior:
        ///     > if the sender is the parent do nothing
        ///     > else if the sender is a child enable the parent if the parent was disabled
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="featureBaseEventArgs"></param>
        protected virtual void OnChildEnabled(object o, FeatureBaseEventArgs featureBaseEventArgs)
        {
            var sender = featureBaseEventArgs.Sender;

            if (sender == null || sender == this)
            {
                return;
            }

            this.Children[sender] = new Tuple<bool, bool>(true, false);

            this.Enable(sender);
        }

        /// <summary>
        ///     Raises the <see cref="E:ChildRemoveInvoker" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="ParentBaseEventArgs" /> instance containing the event data.</param>
        protected virtual void OnChildRemoveInvoker(ParentBaseEventArgs eventArgs)
        {
            this.OnChildRemoveEvent?.Invoke(this, eventArgs);
        }

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            foreach (var keyValuePair in this.Children.ToList())
            {
                if (!keyValuePair.Value.Item1) continue;

                keyValuePair.Key.Disable(this);

                this.Children[keyValuePair.Key] = new Tuple<bool, bool>(true, true);
            }
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            foreach (var keyValuePair in this.Children.ToList())
            {
                if (!keyValuePair.Value.Item1 || !keyValuePair.Value.Item2)
                {
                    continue;
                }

                keyValuePair.Key.Enable(this);
            }
        }

        #endregion

        /// <summary>
        ///     Custom Event Args.
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class ParentBaseEventArgs : EventArgs
        {
            #region Public Properties

            public Base Child { get; set; }

            #endregion
        }
    }
}