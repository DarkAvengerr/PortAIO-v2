
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.MissFortune
{
    using System;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["bool"].GetValue<MenuBool>().Value)
            {
                if (Targets.Target.IsValidTarget(Vars.E.IsReady() ? Vars.E.Range : Vars.R.Range)
                    && !GameObjects.Player.HasBuff("missfortunebulletsound")
                    && Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
                {
                    if (Vars.E.IsReady())
                    {
                        Vars.E.Cast(Targets.Target.ServerPosition);
                    }
                    Vars.R.Cast(Targets.Target.ServerPosition);
                }
                else if (GameObjects.Player.HasBuff("missfortunebulletsound")
                         && !Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
                {
                    Variables.Orbwalker.Move(Game.CursorPos);
                }
            }
        }

        #endregion
    }
}