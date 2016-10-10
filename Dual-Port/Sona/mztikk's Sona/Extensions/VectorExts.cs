using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.Extensions
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    public static class VectorExts
    {
        #region Public Methods and Operators

        public static bool UnderAllyTurret(this Vector3 position)
            =>
                ObjectManager.Get<Obj_AI_Turret>()
                    .Where(x => !x.IsDead && x.Team == ObjectManager.Player.Team)
                    .Any(x => x.Distance(position) < 775);

        public static bool UnderEnemyTurret(this Vector3 position)
            =>
                ObjectManager.Get<Obj_AI_Turret>()
                    .Where(x => !x.IsDead && x.Team != ObjectManager.Player.Team)
                    .Any(x => x.Distance(position) < 775);

        public static Vector2 CenterPoint(this Vector2[] v2Arr)
        {
            float totalX = 0, totalY = 0;
            foreach (Vector2 p in v2Arr)
            {
                totalX += p.X;
                totalY += p.Y;
            }
            float centerX = totalX / v2Arr.Length;
            float centerY = totalY / v2Arr.Length;
            return new Vector2(centerX, centerY);
        }

        #endregion
    }
}