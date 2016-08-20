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

        protected internal readonly Dictionary<Base, bool> Children = new Dictionary<Base, bool>();

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
        public void AddChild(Base child)
        {
            this.OnChildAddInvoker(new ParentBaseEventArgs() { Child = child });
        }

        /// <summary>
        ///     Adds the children.
        /// </summary>
        /// <param name="children">The children.</param>
        public void AddChildren(IEnumerable<Base> children)
        {
            foreach (var child in children)
            {
                this.OnChildAddInvoker(new ParentBaseEventArgs() { Child = child });
            }
        }

        /// <summary>
        ///     Called when [uninitialize].
        /// </summary>
        public override void OnTerminate(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnTerminate(sender, featureBaseEventArgs);

            this.OnChildAddEvent -= this.OnChildAdd;

            foreach (var child in this.Children.Keys.ToList())
            {
                child.OnTerminateInvoker();
            }
        }

        /// <summary>
        ///     Removes the children.
        /// </summary>
        /// <param name="child">The child.</param>
        public void RemoveChildren(Base child)
        {
            if (!this.Children.ContainsKey(child))
            {
                throw new InvalidOperationException(
                    $"{this}, can't remove a child from the list children that has not been added to the list.");
            }

            this.Children.Remove(child);
            child.OnUnLoadInvoker();
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return "ParentBase " + this.Name;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [child add].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="parentBaseEventArgs">The <see cref="ParentBaseEventArgs" /> instance containing the event data.</param>
        protected virtual void OnChildAdd(object sender, ParentBaseEventArgs parentBaseEventArgs)
        {
            var child = parentBaseEventArgs.Child;

            child.OnInitializeInvoker();

            child.OnEnableEvent += this.OnChildEnabled;
            child.OnDisableEvent += this.OnChildDisabled;

            this.Children.Add(child, child.Enabled);

            if (this.Menu.SubMenu(child.Menu.Name) != null)
            {
                this.MergeChild(child);
            }
            else
            {
                this.Menu.AddSubMenu(child.Menu);
            }
        }

        /// <summary>
        /// Merges the child with another children with the same Name
        /// </summary>
        /// <param name="child">The child.</param>
        protected virtual void MergeChild(Base child)
        {
            foreach (var menuEntry in child.Menu.Items)
            {
                if (this.Menu.SubMenu(child.Menu.Name).Items.Contains(menuEntry))
                {
                    continue;
                }

                this.Menu.SubMenu(child.Menu.Name).AddItem(menuEntry);
            }

            child.Menu = this.Menu.SubMenu(child.Menu.Name);
        }

        /// <summary>
        ///     Raises the <see cref="E:ChildAddInvoker" /> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="ParentBaseEventArgs" /> instance containing the event data.</param>
        protected virtual void OnChildAddInvoker(ParentBaseEventArgs eventArgs)
        {
            if (!this.Initialized)
            {
                this.OnInitializeInvoker();
            }

            this.OnChildAddEvent?.Invoke(this, eventArgs);
        }

        /// <summary>
        ///     Called when [child disabled].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="featureBaseEventArgs">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected virtual void OnChildDisabled(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            var child = featureBaseEventArgs.Sender;

            if (child == null)
            {
                return;
            }

            this.Children[child] = child.Enabled;

            // Disables the Parent if all Children are disabled
            if (this.Children.All(x => !x.Value))
            {
                this.OnDisableInvoker();
            }
        }

        /// <summary>
        ///     Called when [child enabled].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="featureBaseEventArgs">The <see cref="Base.FeatureBaseEventArgs" /> instance containing the event data.</param>
        protected virtual void OnChildEnabled(object o, FeatureBaseEventArgs featureBaseEventArgs)
        {
            var child = featureBaseEventArgs.Sender;

            if (child == null)
            {
                return;
            }

            this.Children[child] = child.Enabled;

            // Enables the Parent if one Children is enabled
            this.OnEnableInvoker();
        }

        /// <summary>
        ///     Called when [child remove].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="parentBaseEventArgs">The <see cref="ParentBaseEventArgs" /> instance containing the event data.</param>
        protected virtual void OnChildRemove(object sender, ParentBaseEventArgs parentBaseEventArgs)
        {
            this.Children.Remove(parentBaseEventArgs.Child);

            parentBaseEventArgs.Child.OnTerminateInvoker();

            this.Menu.RemoveMenu(parentBaseEventArgs.Child.Menu);
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
            var menuItem = this.Menu.Item(this.Name + "Enabled");

            if (menuItem.GetValue<bool>() == true)
            {
                menuItem.SetValue(false);
                return;
            }

            foreach (var child in this.Children.ToList())
            {
                if (!child.Value)
                {
                    continue;
                }

                child.Key.OnDisableInvoker();

                this.Children[child.Key] = true;
            }
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            var menuItem = this.Menu.Item(this.Name + "Enabled");

            if (menuItem.GetValue<bool>() == false)
            {
                menuItem.SetValue(true);
                return;
            }

            foreach (var child in this.Children.ToList())
            {
                if (!child.Value)
                {
                    continue;
                }

                child.Key.OnEnableInvoker();
            }
        }

        /// <summary>
        ///     Called when [initialize].
        /// </summary>
        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnInitialize(sender, featureBaseEventArgs);

            this.OnChildAddEvent += this.OnChildAdd;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            foreach (var child in this.Children.ToList())
            {
                child.Key.OnLoadInvoker();
            }
        }

        /// <summary>
        ///     Called when [refresh].
        /// </summary>
        protected override void OnRefresh(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnRefresh(sender, featureBaseEventArgs);

            foreach (var child in this.Children.ToList())
            {
                child.Key.OnRefreshInvoker();
            }
        }

        /// <summary>
        ///     Called when [unload].
        /// </summary>
        protected override void OnUnload(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnUnload(sender, featureBaseEventArgs);

            foreach (var child in this.Children.ToList())
            {
                child.Key.OnUnLoadInvoker();
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