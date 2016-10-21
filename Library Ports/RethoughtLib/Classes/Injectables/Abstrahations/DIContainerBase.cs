using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Classes.Injectables.Abstrahations
{
    using System.Collections.Generic;

    public abstract class DiContainerBase<T> : DependencyInjectionBase<T>
    {
        private readonly IList<T> elements = default(IList<T>);

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionBase{T}"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        protected DiContainerBase(T element)
            : base(element)
        { }

        public virtual void AddElement(T element)
        {
            this.elements.Add(element);
        }
    }
}
