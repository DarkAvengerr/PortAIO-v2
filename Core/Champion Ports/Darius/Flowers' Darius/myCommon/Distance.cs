using EloBuddy; 
using LeagueSharp.Common; 
namespace FlowersDariusCommon
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public static class Distance
    {
        public static Vector2 Deviation(this Vector3 point1, Vector3 point2, double angle = 70)
        {
            angle *= Math.PI / 180.0;

            var temp = Vector2.Subtract(point2.To2D(), point1.To2D());
            var result = new Vector2(0)
            {
                X = (float) (temp.X*Math.Cos(angle) - temp.Y*Math.Sin(angle))/4,
                Y = (float) (temp.X*Math.Sin(angle) + temp.Y*Math.Cos(angle))/4
            };

            result = Vector2.Add(result, point1.To2D());

            return result;
        }

        public static float DistanceToPlayer(this Obj_AI_Base source)
        {
            return ObjectManager.Player.Distance(source);
        }

        public static float DistanceToPlayer(this Vector3 position)
        {
            return position.To2D().DistanceToPlayer();
        }

        public static float DistanceToPlayer(this Vector2 position)
        {
            return ObjectManager.Player.Distance(position);
        }

        public static float DistanceToMouse(this Obj_AI_Base source)
        {
            return Game.CursorPos.Distance(source.Position);
        }

        public static float DistanceToMouse(this Vector3 position)
        {
            return position.To2D().DistanceToMouse();
        }

        public static float DistanceToMouse(this Vector2 position)
        {
            return Game.CursorPos.Distance(position.To3D());
        }

        public static float DistanceSquared(this Obj_AI_Base source, Vector3 position)
        {
            return source.DistanceSquared(position.To2D());
        }

        public static float DistanceSquared(this Obj_AI_Base source, Vector2 position)
        {
            return source.ServerPosition.DistanceSquared(position);
        }

        public static float DistanceSquared(this Vector3 vector3, Vector2 toVector2)
        {
            return vector3.To2D().DistanceSquared(toVector2);
        }

        public static float DistanceSquared(this Vector2 vector2, Vector2 toVector2)
        {
            return Vector2.DistanceSquared(vector2, toVector2);
        }

        public static float DistanceSquared(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return source.DistanceSquared(target.ServerPosition);
        }

        public static float DistanceSquared(this Vector3 vector3, Vector3 toVector3)
        {
            return vector3.To2D().DistanceSquared(toVector3);
        }

        public static float DistanceSquared(this Vector2 vector2, Vector3 toVector3)
        {
            return Vector2.DistanceSquared(vector2, toVector3.To2D());
        }

        public static float DistanceSquared(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd,
            bool onlyIfOnSegment = false)
        {
            return point.ProjectOn(segmentStart, segmentEnd).IsOnSegment || onlyIfOnSegment == false
                ? Vector2.DistanceSquared(point.ProjectOn(segmentStart, segmentEnd).SegmentPoint, point)
                : float.MaxValue;
        }
    }
}
