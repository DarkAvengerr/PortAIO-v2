using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.Logic
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using SharpDX;

    internal class WallDetection
    {
        public Vector3 GetFirstWallPoint(Vector3 start, Vector3 end, int step = 1)
        {
            if (!start.IsValid() || !end.IsValid())
            {
                return Vector3.Zero;
            }

            var distance = start.Distance(end);

            for (var i = 0; i < distance; i = i + step)
            {
                var newPoint = start.Extend(end, i);

                if (NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall || newPoint.IsWall())
                {
                    return newPoint;
                }
            }

            return Vector3.Zero;
        }

        public bool IsWall(AIHeroClient t, Vector3 direction)
        {
            var x = false;

            var istrue = t.Position.Extend(direction, Spells.R2.Range);
            var firstwallpoint = this.GetFirstWallPoint(t.Position, istrue);

            if (firstwallpoint == Vector3.Zero)
            {
                x = false;
            }

            if (istrue.IsWall())
            {
                x = true;
            }

            return x;
        }
    }
}
