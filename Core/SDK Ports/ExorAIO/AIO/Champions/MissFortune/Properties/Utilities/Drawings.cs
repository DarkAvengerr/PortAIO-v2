
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.MissFortune
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    using Color = System.Drawing.Color;
    using Geometry = ExorAIO.Utilities.Geometry;

    /// <summary>
    ///     The prediction drawings class.
    /// </summary>
    internal class ConeDrawings
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
                    ///     Loads the Passive Target drawing.
                    /// </summary>
                    if (MissFortune.PassiveTarget.IsValidTarget()
                        && Vars.Menu["drawings"]["p"].GetValue<MenuBool>().Value)
                    {
                        Render.Circle.DrawCircle(
                            MissFortune.PassiveTarget.Position,
                            MissFortune.PassiveTarget.BoundingRadius,
                            Color.LightGreen,
                            1);
                    }

                    /// <summary>
                    ///     Loads the Q Cone drawings.
                    /// </summary>
                    if (Vars.Q.IsReady() && Vars.Menu["drawings"]["qc"].GetValue<MenuBool>().Value)
                    {
                        foreach (var obj in
                            ObjectManager.Get<Obj_AI_Base>()
                                .Where(m => !(m is Obj_AI_Turret) && m.IsValidTarget(Vars.Q.Range)))
                        {
                            var polygon = new Geometry.Sector(
                                (Vector2)obj.ServerPosition,
                                (Vector2)
                                obj.ServerPosition.Extend(
                                    GameObjects.Player.ServerPosition,
                                    -(Vars.Q2.Range - Vars.Q.Range)),
                                40f * (float)Math.PI / 180f,
                                Vars.Q2.Range - Vars.Q.Range - 50f);
                            var target =
                                GameObjects.EnemyHeroes.FirstOrDefault(
                                    t =>
                                    !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q2.Range)
                                    && (t.NetworkId == MissFortune.PassiveTarget?.NetworkId
                                        || Targets.Minions.All(m => polygon.IsOutside((Vector2)m.ServerPosition))));
                            if (target != null)
                            {
                                polygon.Draw(
                                    !polygon.IsOutside((Vector2)target.ServerPosition)
                                    && !polygon.IsOutside(
                                        (Vector2)
                                        Movement.GetPrediction(
                                            target,
                                            GameObjects.Player.Distance(target) / Vars.Q.Speed + Vars.Q.Delay)
                                            .UnitPosition)
                                        ? Color.Green
                                        : Color.Red);
                            }
                        }
                    }
                };
        }

        #endregion
    }
}