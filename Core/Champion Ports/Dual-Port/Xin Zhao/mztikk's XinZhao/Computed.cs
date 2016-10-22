using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using mztikkXinZhao.Extensions;

    internal static class Computed
    {
        #region Public Methods and Operators

        public static Vector3 GetBestAllyPlace(this Vector3 position, float range, float inRange = 750)
        {
            Obj_AI_Base bestAlly =
                HeroManager.Allies.Where(
                    x => x.Distance(ObjectManager.Player.Position) <= range && !x.IsMe && !x.IsDead && x.IsValid)
                    .OrderByDescending(x => x.CountAlliesInRange(inRange))
                    .FirstOrDefault();
            if (bestAlly != null)
            {
                var bestAllyMasz =
                    HeroManager.Allies.Where(
                        a => a.Distance(bestAlly.Position) <= inRange && !a.IsMe && !a.IsDead && a.IsValid).ToArray();
                var bestallv2 = new Vector2[bestAllyMasz.Count()];
                for (var i = 0; i < bestAllyMasz.Count(); i++)
                {
                    bestallv2[i] = bestAllyMasz[i].Position.To2D();
                }

                return bestallv2.CenterPoint().To3D();
            }

            var closeTurret =
                ObjectManager.Get<Obj_AI_Turret>()
                    .Where(
                        x => x.Team == ObjectManager.Player.Team && x.Distance(ObjectManager.Player.Position) <= range)
                    .OrderBy(t => t.Distance(ObjectManager.Player.Position))
                    .FirstOrDefault();
            if (closeTurret != null)
            {
                return closeTurret.Position;
            }

            var nex = ObjectManager.Get<Obj_Building>().FirstOrDefault(x => x.Name.StartsWith("HQ") && x.IsAlly);
            return nex?.Position ?? Vector3.Zero;
        }

        #endregion
    }
}