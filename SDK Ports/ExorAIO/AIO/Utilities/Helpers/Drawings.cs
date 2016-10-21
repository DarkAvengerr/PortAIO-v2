
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Utilities
{
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class Drawings
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Loads the drawings.
        /// </summary>
        public static void Initialize()
        {
            Drawing.OnEndScene += delegate
                {
                    /// <summary>
                    ///     Loads the R Minimap drawing.
                    /// </summary>
                    if (Vars.R != null && Vars.R.IsReady() && Vars.Menu["drawings"]["r"] != null
                        && Vars.Menu["drawings"]["r"].GetValue<MenuBool>().Value)
                    {
                        if (GameObjects.Player.ChampionName.Equals("Lux")
                            || GameObjects.Player.ChampionName.Equals("Jhin")
                            || GameObjects.Player.ChampionName.Equals("Ryze")
                            || GameObjects.Player.ChampionName.Equals("Taliyah")
                            || GameObjects.Player.ChampionName.Equals("Caitlyn"))
                        {
                            Geometry.DrawCircleOnMinimap(GameObjects.Player.Position, Vars.R.Range, Color.White);
                        }
                    }
                };
            Drawing.OnDraw += delegate
                {
                    /// <summary>
                    ///     Loads the Q drawing,
                    ///     Loads the Extended Q drawing.
                    /// </summary>
                    if (Vars.Q != null && Vars.Q.IsReady())
                    {
                        if (Vars.Menu["drawings"]["q"] != null && Vars.Menu["drawings"]["q"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.Q.Range, Color.LightGreen, 2);
                        }
                        if (Vars.Menu["drawings"]["qe"] != null
                            && Vars.Menu["drawings"]["qe"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.Q2.Range, Color.Yellow, 2);
                        }
                    }

                    /// <summary>
                    ///     Loads the W drawing.
                    /// </summary>
                    if (Vars.W != null && Vars.W.IsReady() && Vars.Menu["drawings"]["w"] != null
                        && Vars.Menu["drawings"]["w"].GetValue<MenuBool>().Value)
                    {
                        Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.W.Range, Color.Purple, 2);
                    }

                    /// <summary>
                    ///     Loads the E drawing.
                    /// </summary>
                    if (Vars.E != null && Vars.E.IsReady() && Vars.Menu["drawings"]["e"] != null
                        && Vars.Menu["drawings"]["e"].GetValue<MenuBool>().Value)
                    {
                        Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.E.Range, Color.Cyan, 2);
                    }

                    /// <summary>
                    ///     Loads the R drawing.
                    /// </summary>
                    if (Vars.R != null && Vars.R.IsReady())
                    {
                        if (Vars.Menu["drawings"]["r"] != null && Vars.Menu["drawings"]["r"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.R.Range, Color.Red, 2);
                        }
                        if (Vars.Menu["drawings"]["r2"] != null
                            && Vars.Menu["drawings"]["r2"].GetValue<MenuBool>().Value)
                        {
                            Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.R2.Range, Color.Blue, 2);
                        }
                    }
                };
        }

        #endregion
    }
}