// TODO: Much.

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.LogicProvider
{
    using LeagueSharp.Common;

    // TODO
    internal class PotionLogicProvider
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Predicted health recovered
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public float HealthRecovered(int time = int.MaxValue, int amount = 0)
        {
            // No potions
            if (!Items.HasItem(2003, GlobalVariables.Player))
            {
                return 0;
            }

            //TODO: amount = item count
            // Time is relative, so its amount * bufftime
            if (time == int.MaxValue)
            {
                time = amount * 15000;
            }

            if (amount == 0)
            {
            }
            return this.PotionValue(amount);
        }

        /// <summary>
        ///     Value of one potion(s)
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public int PotionValue(int amount)
        {
            var value = 150;
            var mod = (int)GlobalVariables.Player.FlatHPRegenMod;

            // TODO: Add Resolve Masteries

            return (value * mod) * amount;
        }

        #endregion
    }
}