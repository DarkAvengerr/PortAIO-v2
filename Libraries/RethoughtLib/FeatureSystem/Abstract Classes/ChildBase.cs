using EloBuddy; namespace RethoughtLib.FeatureSystem.Abstract_Classes
{
    #region Using Directives



    #endregion

    public abstract class ChildBase : Base
    {
        #region Public Methods and Operators

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return "ChildBase " + this.Name;
        }

        #endregion
    }
}