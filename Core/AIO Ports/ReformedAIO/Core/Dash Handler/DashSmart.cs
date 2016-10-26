namespace ReformedAIO.Core.Dash_Handler
{
    using System;

    using EloBuddy;
    using LeagueSharp.Common;

    using SharpDX;

    internal class DashSmart
    {
        public Vector3 ToSafePosition(AIHeroClient target, Vector3 targetPosition, double distance)
        {
            return IsDangerous(target, targetPosition) 
                ? Deviation(ObjectManager.Player.Position.To2D(), target.Position.To2D(), 65).To3D() // We wont go in, nor will we go backwards. 
                : Game.CursorPos;
        }

        public Vector2 Deviation(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI / 200.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0)
            {
                X = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4,
                Y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4
            };

            result = Vector2.Add(result, point1);
            return result;
        }

        private static bool IsDangerous(Obj_AI_Base target, Vector3 position)
        {
            return (ObjectManager.Player.CountEnemiesInRange(1000) > ObjectManager.Player.CountAlliesInRange(1000))
                   || (target.Position.UnderTurret(true) && target.HealthPercent >= 70)
                   || (target.IsMelee && target.Distance(ObjectManager.Player) <= (target.AttackRange + target.BoundingRadius) / 2);
        }
    }
}
