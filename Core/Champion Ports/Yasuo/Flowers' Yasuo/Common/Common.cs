using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Common
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public static class Common
    {
        public static bool CanCastDelayR(AIHeroClient target)
        {
            //copy from valvesharp
            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);

            return buff != null &&
                   buff.EndTime - Game.Time <=
                   (buff.EndTime - buff.StartTime)/(buff.EndTime - buff.StartTime <= 0.5 ? 1.5 : 3);
        }

        public static bool UnderTower(Vector3 pos)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.IsValidTarget(950, true, pos));
        }

        public static Vector3 PosAfterE(Obj_AI_Base target)
        {
            var pred = Prediction.GetPrediction(target, 200);

            return ObjectManager.Player.ServerPosition.Extend(pred.UnitPosition, 475f);
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
    }
}
