using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Core.Condemn_Logic
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class ReformedCondemn : ICondemnType
    {
        public Vector3 Execute(Obj_AI_Base target, float range, Spell spell)
        {
            if (!Orbwalking.CanMove(5) || !Orbwalking.CanAttack())
            {
                return Vector3.Zero;
            }

            var prediction = spell.GetPrediction(target).UnitPosition;

            for (float i = 0; i < range; i += range / 5f)
            {
                var newprediction = prediction.Extend(ObjectManager.Player.ServerPosition, -i);

                if (NavMesh.GetCollisionFlags(newprediction) == CollisionFlags.Wall
                || NavMesh.GetCollisionFlags(newprediction) == CollisionFlags.Building
                || newprediction.IsWall())
                {
                    return newprediction;
                }
            }

            var finalPosition = prediction.Extend(ObjectManager.Player.ServerPosition, -range);

            if (NavMesh.GetCollisionFlags(finalPosition) == CollisionFlags.Wall
                || NavMesh.GetCollisionFlags(finalPosition) == CollisionFlags.Building
                || finalPosition.IsWall())
            {
                return finalPosition;
            }
            return Vector3.Zero;
        }
    }
}
