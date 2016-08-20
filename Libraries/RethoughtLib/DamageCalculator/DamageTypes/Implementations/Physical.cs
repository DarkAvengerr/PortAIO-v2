using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.DamageCalculator.DamageTypes.Implementations
{
    using LeagueSharp;

    public class Physical : IDamageType
    {
        /// <summary>
        ///     Calculates the expected damage output.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public double Calculate(double value, Obj_AI_Base target, Obj_AI_Base source = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
