
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Orianna
{
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     The prediction drawings class.
    /// </summary>
    internal class BallDrawings
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
                    var ballPos = Orianna.GetBallPosition();

                    if (ballPos == Vector3.Zero)
                    {
                        var objGeneralParticleEmitter =
                            ObjectManager.Get<Obj_GeneralParticleEmitter>()
                                .FirstOrDefault(o => o.Name.Equals("Orianna_Base_Q_Ghost_mis.troy"));
                        if (objGeneralParticleEmitter != null)
                        {
                            ballPos = objGeneralParticleEmitter.Position;
                        }
                    }

                    if (Vars.Menu["drawings"]["ball"].GetValue<MenuBool>().Value)
                    {
                        Render.Circle.DrawCircle(
                            ballPos == GameObjects.Player.ServerPosition ? GameObjects.Player.Position : ballPos,
                            100f,
                            Color.Blue,
                            4);
                    }
                    if (Vars.W.IsReady() && Vars.Menu["drawings"]["ballw"].GetValue<MenuBool>().Value)
                    {
                        Render.Circle.DrawCircle(
                            ballPos == GameObjects.Player.ServerPosition ? GameObjects.Player.Position : ballPos,
                            Vars.W.Range,
                            Color.Purple);
                    }
                    if (Vars.R.IsReady() && Vars.Menu["drawings"]["ballr"].GetValue<MenuBool>().Value)
                    {
                        Render.Circle.DrawCircle(
                            ballPos == GameObjects.Player.ServerPosition ? GameObjects.Player.Position : ballPos,
                            Vars.R.Range,
                            Color.Red);
                    }
                };
        }

        #endregion
    }
}