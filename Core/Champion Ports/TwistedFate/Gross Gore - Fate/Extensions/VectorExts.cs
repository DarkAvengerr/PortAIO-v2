using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Extensions
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

        #endregion
    }
}