using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; namespace RethoughtLib.Classes.Injectables
{
    public abstract class DependencyInjectionBase<T>
    {
        /// <summary>
        /// The element
        /// </summary>
        private T element;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionBase{T}"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        protected DependencyInjectionBase(T element)
        {
            this.element = element;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Changes the element.
        /// </summary>
        /// <param name="newElement">The new element.</param>
        public virtual void ChangeElement(T newElement)
        {
            this.element = newElement;
        }
    }
}
