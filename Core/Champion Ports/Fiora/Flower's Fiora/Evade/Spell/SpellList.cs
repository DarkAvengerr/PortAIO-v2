using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Fiora.Evade
{
    using System;
    using System.Collections.Generic;

    internal class SpellList<T> : List<T>
    {
        public event EventHandler OnAdd;

        public new void Add(T item)
        {
            OnAdd?.Invoke(this, null);

            base.Add(item);
        }
    }
}
