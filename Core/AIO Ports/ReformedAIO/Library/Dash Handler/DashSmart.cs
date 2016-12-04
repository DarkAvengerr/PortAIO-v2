using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Library.Dash_Handler
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class DashSmart
    {
        /// <summary>
        /// Dashes to side if dangerous, else to cursor. (Needs work)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetPosition"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Vector3 ToSafePosition(Obj_AI_Base target, Vector3 targetPosition, double distance)
        {
            return IsDangerous(target) 
                ? Kite(target.Position.To2D(), distance).To3D()
                : Game.CursorPos;
        }

        /// <summary>
        /// Dashes to the side extended with target position
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector2 Kite(Vector2 targetPos, double angle)
        {
            angle *= Math.PI / 180.0;
            var temp = Vector2.Subtract(targetPos, ObjectManager.Player.Position.To2D());
            var result = new Vector2(0)
            {
                X = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4,
                Y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4
            };

            result = Vector2.Add(result, ObjectManager.Player.Position.To2D());
            return result;
        }

        /// <summary>
        /// Checks if the position is dangerous
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static bool IsDangerous(Obj_AI_Base target)
        {
            return (ObjectManager.Player.CountEnemiesInRange(1000) > ObjectManager.Player.CountAlliesInRange(1000))
                   || (target.Position.UnderTurret(true) && target.HealthPercent >= 80)
                   || (target.IsMelee && target.Distance(ObjectManager.Player) <= (target.AttackRange + target.BoundingRadius) / 2);
        }
    }
}
