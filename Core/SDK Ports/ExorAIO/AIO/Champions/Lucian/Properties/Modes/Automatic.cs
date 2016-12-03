
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Lucian
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

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
            ///     The Automatic R Orbwalking.
            /// </summary>
            if (Vars.Menu["spells"]["r"]["bool"].GetValue<MenuBool>().Value
                && Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
            {
                DelayAction.Add(
                    (int)(100 + Game.Ping / 2f),
                    () => { EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos); });
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["bool"].GetValue<MenuBool>().Value)
            {
                if (!GameObjects.Player.HasBuff("LucianR") && Targets.Target.IsValidTarget(Vars.R.Range)
                    && Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
                {
                    if (!Vars.W.GetPrediction(Targets.Target).CollisionObjects.Any())
                    {
                        Vars.W.Cast(Vars.W.GetPrediction(Targets.Target).UnitPosition);
                    }
                    Vars.R.Cast(Vars.R.GetPrediction(Targets.Target).UnitPosition);
                }
                else if (GameObjects.Player.HasBuff("LucianR")
                         && !Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
                {
                    Vars.R.Cast();
                }
            }
        }

        #endregion
    }
}