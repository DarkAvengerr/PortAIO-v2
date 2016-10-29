using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The managers class.
    /// </summary>
    internal class Managers
    {
        #region Public Properties

        /// <summary>
        ///     Sets the minimum necessary health percent to use a health potion.
        /// </summary>
        public static int MinHealthPercent => Vars.Menu["consumables"]["health"].GetValue<MenuSlider>().Value;

        /// <summary>
        ///     Sets the minimum necessary mana percent to use a mana potion.
        /// </summary>
        public static int MinManaPercent => Vars.Menu["consumables"]["mana"].GetValue<MenuSlider>().Value;

        #endregion
    }
}