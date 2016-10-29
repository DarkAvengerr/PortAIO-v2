
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
    using System;
    using System.Linq;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The activator class.
    /// </summary>
    internal partial class Activator
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Cleansers(EventArgs args)
        {
            if (!Vars.Menu["cleansers"].GetValue<MenuSliderButton>().BValue)
            {
                return;
            }

            /// <summary>
            ///     The Mikaels Crucible Logic.
            /// </summary>
            if (Items.CanUseItem(3222))
            {
                foreach (var ally in
                    GameObjects.AllyHeroes.Where(a => Bools.ShouldCleanse(a) && a.IsValidTarget(750f, false)))
                {
                    DelayAction.Add(
                        Vars.Menu["cleansers"].GetValue<MenuSliderButton>().SValue,
                        () => { Items.UseItem(3222, ally); });
                }
            }

            if (Bools.ShouldUseCleanser() || !SpellSlots.Cleanse.IsReady() && Bools.ShouldCleanse(GameObjects.Player))
            {
                /// <summary>
                ///     The Quicksilver Sash Logic.
                /// </summary>
                if (Items.CanUseItem(3140))
                {
                    DelayAction.Add(
                        Vars.Menu["cleansers"].GetValue<MenuSliderButton>().SValue,
                        () => { Items.UseItem(3140); });
                }

                /// <summary>
                ///     The Dervish Blade Logic.
                /// </summary>
                if (Items.CanUseItem(3137))
                {
                    DelayAction.Add(
                        Vars.Menu["cleansers"].GetValue<MenuSliderButton>().SValue,
                        () => { Items.UseItem(3137); });
                }

                /// <summary>
                ///     The Mercurial Scimitar Logic.
                /// </summary>
                if (Items.CanUseItem(3139))
                {
                    DelayAction.Add(
                        Vars.Menu["cleansers"].GetValue<MenuSliderButton>().SValue,
                        () => { Items.UseItem(3139); });
                }
            }
            if (GameObjects.Player.HealthPercent < 10)
            {
                /// <summary>
                ///     The Dervish Blade Logic.
                /// </summary>
                if (Items.CanUseItem(3137))
                {
                    DelayAction.Add(
                        Vars.Menu["cleansers"].GetValue<MenuSliderButton>().SValue,
                        () => { Items.UseItem(3137); });
                }

                /// <summary>
                ///     The Mercurial Scimitar Logic.
                /// </summary>
                if (Items.CanUseItem(3139))
                {
                    DelayAction.Add(
                        Vars.Menu["cleansers"].GetValue<MenuSliderButton>().SValue,
                        () => { Items.UseItem(3139); });
                }
            }
        }

        #endregion
    }
}