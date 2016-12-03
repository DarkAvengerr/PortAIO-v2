
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Champions.Vayne
{
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     The prediction drawings class.
    /// </summary>
    internal class PredictionDrawings
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Loads the range drawings.
        /// </summary>
        public static void Initialize()
        {
            if (GameObjects.Player.IsDead)
            {
                return;
            }

            Drawing.OnDraw += delegate
                {
                    /// <summary>
                    ///     Loads the E drawing.
                    /// </summary>
                    if (Vars.E != null && Vars.E.IsReady() && Vars.Menu["drawings"]["epred"] != null
                        && Vars.Menu["drawings"]["epred"].GetValue<MenuBool>().Value)
                    {
                        foreach (var target in
                            GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(Vars.E.Range)))
                        {
                            /// <summary>
                            ///     The Position Line.
                            /// </summary>
                            Drawing.DrawLine(
                                Drawing.WorldToScreen(GameObjects.Player.Position).X,
                                Drawing.WorldToScreen(GameObjects.Player.Position).Y,
                                Drawing.WorldToScreen(
                                    target.Position
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                                Drawing.WorldToScreen(
                                    target.Position
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                                1,
                                (target.Position
                                 + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).IsWall()
                                    ? Color.Green
                                    : Color.Red);

                            /// <summary>
                            ///     The Angle-Check Position Line.
                            /// </summary>
                            Drawing.DrawLine(
                                Drawing.WorldToScreen(
                                    target.Position
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                                Drawing.WorldToScreen(
                                    target.Position
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                                Drawing.WorldToScreen(
                                    target.Position
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).X,
                                Drawing.WorldToScreen(
                                    target.Position
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).Y,
                                1,
                                (target.Position
                                 + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).IsWall()
                                    ? Color.Green
                                    : Color.Red);

                            /// <summary>
                            ///     The Angle-Check Prediction Line.
                            /// </summary>
                            Drawing.DrawLine(
                                Drawing.WorldToScreen(
                                    Vars.E.GetPrediction(target).UnitPosition
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                                Drawing.WorldToScreen(
                                    Vars.E.GetPrediction(target).UnitPosition
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                                Drawing.WorldToScreen(
                                    Vars.E.GetPrediction(target).UnitPosition
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).X,
                                Drawing.WorldToScreen(
                                    Vars.E.GetPrediction(target).UnitPosition
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).Y,
                                1,
                                (Vars.E.GetPrediction(target).UnitPosition
                                 + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 440).IsWall()
                                    ? Color.Green
                                    : Color.Red);

                            /// <summary>
                            ///     The Prediction Assurance Line.
                            /// </summary>
                            Drawing.DrawLine(
                                Drawing.WorldToScreen(GameObjects.Player.Position).X,
                                Drawing.WorldToScreen(GameObjects.Player.Position).Y,
                                Drawing.WorldToScreen(
                                    Vars.E2.GetPrediction(target).UnitPosition
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).X,
                                Drawing.WorldToScreen(
                                    Vars.E2.GetPrediction(target).UnitPosition
                                    + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).Y,
                                1,
                                (Vars.E2.GetPrediction(target).UnitPosition
                                 + Vector3.Normalize(target.Position - GameObjects.Player.Position) * 420).IsWall()
                                    ? Color.Green
                                    : Color.Red);
                        }
                    }
                };
        }

        #endregion
    }
}